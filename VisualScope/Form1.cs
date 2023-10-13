using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using MavLink;
using Microsoft.JScript;
using Steema.TeeChart.Styles;
using Convert = System.Convert;


//Ctrl-M-O   折叠所有方法 
//Ctrl-M-P   展开所有方法并停止大纲显示（不可以再折叠了） 
//Ctrl-M-M   折叠或展开当前方法 
//Ctrl-M-L展开所有方法
namespace 地面站
{
    public partial class Form1 : Form
    {
        private TCP tcpserver;
        int TxCount = 0;
        int RxCount = 0;
        private Mavlink mavlink;
        public MavlinkProxy mavlinkProxy;
        public string CB_MsgSelText;
        public MavlinkPacket mavlinkPacket ;
        private Msg_param_write msg_Param_Write ;
        private Msg_param_read msg_Param_Read;
        private Msg_cmd_write msg_Cmd_Write;
        public Msg_rocker msg_Rocker;
        private MavLink.Msg_usv_set msg_Usv_Set;
        public MavLink.Msg_usv_state msg_usv_state=new Msg_usv_state();

        public double LongitudeStart;
        public double LatitudeStart;

        public double X_Standard= 4052589.2780501638;
        public double Y_Standard= 11726384.463664232;
        public double track_time;


        private StreamWriter usv_sw;

        Norbbin norbbin;
        Form form1;
        public class USV_Position
        {
            public double X;
            public double Y;
            public USV_Position(double X_, double Y_)
            {
                this.X = X_;
                this.Y = Y_;
            }
        };
        public USV_Position Usv_Position=new USV_Position(0,0);

        int mavlinkBytesUnused = 0;
        List<byte> mavsend = new List<byte>();
        public double[] tChart2Data = new double[20];
        public string[] tChart2Text = new string[20];

        float DataGridView1_CellOldValue;
        public bool MavlinkParamWriteAck = false;
        public bool MavlinkCMDAck = false;
        private int label11time = 0;
        private bool param_read_flag = false;
        
        static readonly byte UWB_Tag_Num = 10;
        private VirtualLeader VirtualLeader;
        readonly UAV_Followers[] UAV_Followers = new UAV_Followers[UWB_Tag_Num];


        /*USV相关初始化*/
        static readonly byte USV_NUM = 6;
        public USV[] USVs = new USV[USV_NUM];
        public LOS[] USVs_LOS=new LOS[USV_NUM];
        public Mavlink[] USVs_Mavlink=new Mavlink[USV_NUM];
        public List<byte> USV_ID_List = new List<byte>();
        public USV_State_Info[] USVs_State_Info = new USV_State_Info[USV_NUM];
        public USV_PID_Info[] USVs_PID_Info = new USV_PID_Info[USV_NUM];
        public USV_Point_PID_INFO[] USVs_Point_PID_ = new USV_Point_PID_INFO[USV_NUM];
        



        List<Expression> Expressions = new List<Expression>();
        List<Expression> Expressions_Copy=new List<Expression>();
        List<byte> UWB_TagUsed = new List<byte>();

       
        List<PointF>Points = new List<PointF>();
        List<PointF> Points_Copy = new List<PointF>();

        float Current_Normal_k;//法向量斜率
        string Current_Normal_Expression;//法向量方程

        public HorizLine[] HorizLines = new HorizLine[30];
        public FastLine[] FastLines = new FastLine[30];
        bool PathTrackingEnable = false;
        TChartZoom[] tchartzooms = new TChartZoom[10];
        TChartScaling[] tchartscalings = new TChartScaling[10];

        readonly OpenFileDialog ofd = new OpenFileDialog
        {
            RestoreDirectory = true,
            Filter = "TChart文件|*.ten"
        };
        public Form1()
        {
            InitializeComponent();
        }
        public void Delay_ms(long ms)//非阻塞延时
        {
            long t;
            t = ms * 10000;
            long start = DateTime.Now.Ticks;//单位：百纳秒
            while (DateTime.Now.Ticks - start < t)
            {
                Application.DoEvents();
            }
        }
        void Mavlink_BytesUnused(object sender, PacketCRCFailEventArgs e)//丢弃的数据计数
        {
            mavlinkBytesUnused += e.BadPacket.Length;
        }
        void Mavlink_FailedCRC(object sender, PacketCRCFailEventArgs e)//CRC校验失败，打印错误数据
        {
            string str = BitConverter.ToString(e.BadPacket);
            str = str.Replace("-", " ");
            str += " ";
            TB_Status.AppendText("错误数据：" + str + "\r\n");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//参数保存
        {   
            string name;
            string text;
            string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.ini");
            if(!File.Exists(FilePath))
            {
                StreamWriter streamWriter = new StreamWriter(FilePath, true);
                streamWriter.WriteLine("");
                streamWriter.Close();
            }
            ParamSave.Write("BAUD", CB_Baud_Sel.Text, FilePath);
            ParamSave.Write("BAUD1", comboBox2.Text, FilePath);
            FieldInfo[] fieldInfo = ((Form1)sender).GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                switch (fieldInfo[i].FieldType.Name)
                {
                    case "TextBox":
                        TextBox tb;
                        tb = (TextBox)fieldInfo[i].GetValue((Form1)sender);
                        name = tb.Name;
                        if (name == "TB_Recv" || name == "TB_Send" || name == "TB_Status")
                            break;
                        text = tb.Text;
                        ParamSave.Write(name, text, FilePath);
                        break;
                    case "RadioButton":
                        RadioButton rb;
                        rb = (RadioButton)fieldInfo[i].GetValue((Form1)sender);
                        name = rb.Name;
                        if (rb.Checked == true)
                            text = "true";
                        else
                            text = "false";
                        ParamSave.Write(name, text, FilePath);
                        break;
                    case "CheckBox":
                        CheckBox cb;
                        cb = (CheckBox)fieldInfo[i].GetValue((Form1)sender);
                        name = cb.Name;
                        if (cb.Checked == true)
                            text = "true";
                        else
                            text = "false";
                        ParamSave.Write(name, text, FilePath);
                        break;
                    default: break;
                }
            }
        }
        private void ParamLoad(object sender )//参数加载
        {
            string name;
            string text;

            string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.ini");
            CB_Baud_Sel.SelectedItem = ParamSave.Read("BAUD", "460800", FilePath);
            comboBox2.SelectedItem = ParamSave.Read("BAUD1", "921600", FilePath);
            FieldInfo[] fieldInfo = ((Form1)sender).GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fieldInfo.Length; i++)
            {
                switch (fieldInfo[i].FieldType.Name)
                {
                    case "TextBox":
                        TextBox tb;       
                        tb = (TextBox)fieldInfo[i].GetValue((Form1)sender);
                        name = tb.Name;
                        if (name == "TB_Recv" || name == "TB_Send" || name == "TB_Status")
                            break;
                        text = ParamSave.Read(name, "0", FilePath);
                        tb.Text = text;
                        break;
                    case "RadioButton":
                        RadioButton rb;
                        rb =(RadioButton) fieldInfo[i].GetValue((Form1)sender);
                        name = rb.Name;
                        text = ParamSave.Read(name, "false", FilePath);
                        if (text == "true")
                            rb.Checked = true;
                        else
                            rb.Checked = false;
                        break;
                    case "CheckBox":
                        CheckBox cb;
                        cb = (CheckBox)fieldInfo[i].GetValue((Form1)sender);
                        name = cb.Name;
                        text = ParamSave.Read(name, "false", FilePath);
                        if (text == "true")
                            cb.Checked = true;
                        else
                            cb.Checked = false;
                        break;
                    default:break;
                }
            }
        }
        private void TChartInit(Steema.TeeChart.TChart tchart)
        {
            tchart.Aspect.View3D = false;
            tchart.Axes.Automatic = false;
            tchart.Axes.Bottom.Automatic = false;
            tchart.Axes.Bottom.AutomaticMaximum = false;
            tchart.Axes.Bottom.AutomaticMinimum = false;
            tchart.Axes.Bottom.AxisPen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            tchart.Axes.Bottom.AxisPen.Transparency = 50;
            tchart.Axes.Bottom.AxisPen.Width = 1;
            tchart.Axes.Bottom.FixedLabelSize = false;
            tchart.Axes.Bottom.Grid.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            tchart.Axes.Bottom.Grid.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            tchart.Axes.Bottom.Grid.Transparency = 50;
            tchart.Axes.Bottom.Grid.Visible = true;
            tchart.Axes.Bottom.Labels.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            tchart.Axes.Bottom.Labels.Font.Shadow.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            tchart.Axes.Bottom.Labels.Font.Size = 9;
            tchart.Axes.Bottom.Labels.Font.SizeFloat = 9F;
            tchart.Axes.Bottom.Labels.RoundFirstLabel = false;
            tchart.Axes.Bottom.Labels.ValueFormat = "0.########";
            tchart.Axes.Bottom.Maximum = 1000D;
            tchart.Axes.Bottom.Minimum = 0D;
            tchart.Axes.Bottom.MinorGrid.Color = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            tchart.Axes.Bottom.MinorGrid.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            tchart.Axes.Bottom.MinorGrid.Transparency = 50;
            tchart.Axes.Bottom.MinorTicks.Length = 1;
            tchart.Axes.Bottom.Ticks.Length = 0;
            tchart.Axes.Bottom.Ticks.Visible = false;
            tchart.Axes.Bottom.Title.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            tchart.Axes.Bottom.Title.Font.Size = 11;
            tchart.Axes.Bottom.Title.Font.SizeFloat = 11F;
            tchart.Axes.Depth.Automatic = false;
            tchart.Axes.Depth.AutomaticMaximum = false;
            tchart.Axes.Depth.AutomaticMinimum = false;
            tchart.Axes.Depth.Maximum = 0D;
            tchart.Axes.Depth.Minimum = 0D;
            tchart.Axes.DepthTop.Automatic = false;
            tchart.Axes.DepthTop.AutomaticMaximum = false;
            tchart.Axes.DepthTop.AutomaticMinimum = false;
            tchart.Axes.DepthTop.Maximum = 0D;
            tchart.Axes.DepthTop.Minimum = 0D;
            tchart.Axes.DrawBehind = false;
            tchart.Axes.Left.Automatic = false;
            tchart.Axes.Left.AutomaticMaximum = false;
            tchart.Axes.Left.AutomaticMinimum = false;
            tchart.Axes.Left.AxisPen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            tchart.Axes.Left.AxisPen.Visible = true;
            tchart.Axes.Left.AxisPen.Width = 1;
            tchart.Axes.Left.FixedLabelSize = false;
            tchart.Axes.Left.Grid.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            tchart.Axes.Left.Grid.Transparency = 50;
            tchart.Axes.Left.Labels.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            tchart.Axes.Left.Labels.Font.Size = 9;
            tchart.Axes.Left.Labels.Font.SizeFloat = 9F;
            tchart.Axes.Left.Labels.RoundFirstLabel = false;
            tchart.Axes.Left.Labels.ValueFormat = "0.########";
            tchart.Axes.Left.Maximum = 939D;
            tchart.Axes.Left.Minimum = 165D;
            tchart.Axes.Left.MinorTicks.Length = 1;
            tchart.Axes.Left.MinorTicks.Visible = true;
            tchart.Axes.Left.Ticks.Length = 0;
            tchart.Axes.Left.Title.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            tchart.Axes.Left.Title.Font.Size = 11;
            tchart.Axes.Left.Title.Font.SizeFloat = 11F;
            tchart.Axes.Right.Automatic = false;
            tchart.Axes.Right.AutomaticMaximum = false;
            tchart.Axes.Right.AutomaticMinimum = false;
            tchart.Axes.Right.Labels.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            tchart.Axes.Right.Labels.Font.Size = 9;
            tchart.Axes.Right.Labels.Font.SizeFloat = 9F;
            tchart.Axes.Right.Maximum = 0D;
            tchart.Axes.Right.Minimum = 0D;
            tchart.Axes.Right.Visible = false;
            tchart.Axes.Top.Automatic = false;
            tchart.Axes.Top.AutomaticMaximum = false;
            tchart.Axes.Top.AutomaticMinimum = false;
            tchart.Axes.Top.Grid.Visible = false;
            tchart.Axes.Top.Labels.Font.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            tchart.Axes.Top.Labels.Font.Size = 9;
            tchart.Axes.Top.Labels.Font.SizeFloat = 9F;
            tchart.Axes.Top.Maximum = 0D;
            tchart.Axes.Top.Minimum = 0D;
            tchart.Axes.Top.Visible = false;
            tchart.CurrentTheme = Steema.TeeChart.ThemeType.Report;
            tchart.Cursor = System.Windows.Forms.Cursors.Arrow;
            tchart.Dock = System.Windows.Forms.DockStyle.Fill;
            tchart.Header.Font.Brush.Color = System.Drawing.Color.Gray;
            tchart.Header.Font.Size = 12;
            tchart.Header.Font.SizeFloat = 12F;
            tchart.Header.Visible = false;
            tchart.Legend.CheckBoxes = true;
            //tchart.Legend.DrawBehind = false;
            tchart.Legend.Font.Size = 9;
            tchart.Legend.Font.SizeFloat = 9F;
            tchart.Legend.FontSeriesColor = true;
            tchart.Legend.LegendStyle = Steema.TeeChart.LegendStyles.Series;
            tchart.Legend.ResizeChart = false;
            tchart.Legend.Shadow.Visible = false;
            tchart.Legend.TextSymbolGap = -3;
            tchart.Legend.TopLeftPos = 7;
            tchart.Legend.Transparent = true;
            tchart.Legend.VertSpacing = -3;
            tchart.Location = new System.Drawing.Point(1, 3);
            tchart.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            tchart.Panel.Bevel.Outer = Steema.TeeChart.Drawing.BevelStyles.None;
            tchart.Panel.Brush.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            tchart.Panel.Brush.Gradient.Direction = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            tchart.Panel.Brush.Gradient.Visible = false;
            tchart.Panel.MarginBottom = 0D;
            tchart.Panel.MarginLeft = 0D;
            tchart.Panel.MarginRight = 0D;
            tchart.Panel.MarginTop = 0D;
            tchart.Size = new System.Drawing.Size(1233, 651);
            tchart.Walls.Back.Brush.Visible = false;
            tchart.Walls.Back.Transparent = true;
            tchart.Walls.Back.Visible = false;
            tchart.Zoom.AnimatedSteps = 0;
            tchart.Zoom.Direction = Steema.TeeChart.ZoomDirections.None;
            tchart.Zoom.MinPixels = 100;
            tchart.Zoom.Pen.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            tchart.Zoom.Pen.Visible = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ParamLoad(sender);
            form1 = (Form)sender;
            TChartInit(tChart1);
            mavlink = new MavLink.Mavlink();
            mavlinkPacket = new MavLink.MavlinkPacket();
            msg_Param_Write = new MavLink.Msg_param_write();
            msg_Param_Read = new MavLink.Msg_param_read();
            msg_Cmd_Write = new MavLink.Msg_cmd_write();
            msg_Rocker = new MavLink.Msg_rocker();
            msg_Usv_Set = new Msg_usv_set();
            mavlinkProxy = new MavlinkProxy(sender);
            mavlink.PacketReceived += mavlinkProxy.Mavlink_PacketReceived;
            mavlink.BytesUnused += Mavlink_BytesUnused;
            mavlink.PacketFailedCRC += Mavlink_FailedCRC;
            mavlinkPacket.SystemId = (int)MavLink.SYS_TYPE.SYS_GSTATION;
            mavlinkPacket.ComponentId = 0;
            VirtualLeader = new VirtualLeader(TB_SetLeaderSpeed, textBox2, textBox3);

            UAV_Followers[0] = new UAV_Followers(TB_UAV1_L,
                                        TB_UAV1_Angle,
                                        TB_UAV1_x_Kp,
                                        TB_UAV1_x_Ki,
                                        TB_UAV1_x_Kd,
                                        TB_UAV1_y_Kp,
                                        TB_UAV1_y_Ki,
                                        TB_UAV1_y_Kd,
                                        TB_UAV_Filter);
            UAV_Followers[1] = new UAV_Followers(TB_UAV2_L,
                                        TB_UAV2_Angle,
                                        TB_UAV2_x_Kp,
                                        TB_UAV2_x_Ki,
                                        TB_UAV2_x_Kd,
                                        TB_UAV2_y_Kp,
                                        TB_UAV2_y_Ki,
                                        TB_UAV2_y_Kd,
                                        TB_UAV_Filter
                                        );
            UAV_Followers[2] = new UAV_Followers(TB_UAV3_L,
                                        TB_UAV3_Angle,
                                        TB_UAV3_x_Kp,
                                        TB_UAV3_x_Ki,
                                        TB_UAV3_x_Kd,
                                        TB_UAV3_y_Kp,
                                        TB_UAV3_y_Ki,
                                        TB_UAV3_y_Kd,
                                        TB_UAV_Filter
                                        );
            CB_Port_Sel.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (System.IO.Ports.SerialPort.GetPortNames().Length > 0)
            {
                CB_Port_Sel.SelectedIndex = 0;
                comboBox1.SelectedIndex = 0;
            }
            tchartzooms[0] = new TChartZoom(tChart1);
            tchartzooms[1] = new TChartZoom(tChart2);
            tchartzooms[2] = new TChartZoom(tChart3);
            tchartzooms[3] = new TChartZoom(tChart5);
            tchartzooms[4] = new TChartZoom(tChart7);
            tchartzooms[5] = new TChartZoom(tChart8);
            tchartzooms[6] = new TChartZoom(tChart9);
            tchartzooms[8] = new TChartZoom(tChart11);

            tchartscalings[0] = new TChartScaling(tChart4);
            tchartscalings[1] = new TChartScaling(tChart6);

            /******USV相关控件初始化*******/
            USVs_State_Info[0] = new USV_State_Info(label_USV1_ID, label_USV1_VEL, label_USV1_VOL, label_USV1_X, label_USV1_Y, label_USV1_HEADING, label_USV1_LOS_VEL, label_USV1_LOS_HEA, textBox_USV1_L, textBox_USV1_angle, textBox_USV1_kp, textBox_USV1_delta);
            USVs_State_Info[1] = new USV_State_Info(label_USV2_ID, label_USV2_VEL, label_USV2_VOL, label_USV2_X, label_USV2_Y, label_USV2_HEADING, label_USV2_LOS_VEL, label_USV2_LOS_HEA, textBox_USV2_L, textBox_USV2_angle, textBox_USV2_kp, textBox_USV2_delta);
            USVs_State_Info[2] = new USV_State_Info(label_USV3_ID, label_USV3_VEL, label_USV3_VOL, label_USV3_X, label_USV3_Y, label_USV3_HEADING, label_USV3_LOS_VEL, label_USV3_LOS_HEA, textBox_USV3_L, textBox_USV3_angle, textBox_USV3_kp, textBox_USV3_delta);

            USVs_PID_Info[0] = new USV_PID_Info(textBox_USV1_VEL_Kp, textBox_USV1_VEL_Ki, textBox_USV1_VEL_Kd, textBox_USV1_HEA_Kp, textBox_USV1_HEA_Ki, textBox_USV1_HEA_Kd);
            USVs_PID_Info[1] = new USV_PID_Info(textBox_USV2_VEL_Kp, textBox_USV2_VEL_Ki, textBox_USV2_VEL_Kd, textBox_USV2_HEA_Kp, textBox_USV2_HEA_Ki, textBox_USV2_HEA_Kd);
            USVs_PID_Info[2] = new USV_PID_Info(textBox_USV3_VEL_Kp, textBox_USV3_VEL_Ki, textBox_USV3_VEL_Kd, textBox_USV3_HEA_Kp, textBox_USV3_HEA_Ki, textBox_USV3_HEA_Kd);

            USVs_Point_PID_[0] = new USV_Point_PID_INFO(textBox_USV1_X_Kp, textBox_USV1_X_Ki, textBox_USV1_X_Kd, textBox_USV1_Y_Kp, textBox_USV1_Y_Ki, textBox_USV1_Y_Kd);
            USVs_Point_PID_[1] = new USV_Point_PID_INFO(textBox_USV2_X_Kp, textBox_USV2_X_Ki, textBox_USV2_X_Kd, textBox_USV2_Y_Kp, textBox_USV2_Y_Ki, textBox_USV2_Y_Kd);
            USVs_Point_PID_[2] = new USV_Point_PID_INFO(textBox_USV3_X_Kp, textBox_USV3_X_Ki, textBox_USV3_X_Kd, textBox_USV3_Y_Kp, textBox_USV3_Y_Ki, textBox_USV3_Y_Kd);

            USVs_LOS[0] = new LOS(sender, textBox13.Text, textBox12.Text, Convert.ToDouble(textBox_USV1_kp.Text), Convert.ToDouble(textBox_USV1_delta.Text));
            USVs_LOS[1] = new LOS(sender, textBox13.Text, textBox12.Text, Convert.ToDouble(textBox_USV2_kp.Text), Convert.ToDouble(textBox_USV2_delta.Text));
            USVs_LOS[2] = new LOS(sender, textBox13.Text, textBox12.Text, Convert.ToDouble(textBox_USV3_kp.Text), Convert.ToDouble(textBox_USV3_delta.Text));

            USVs[0] = new USV(form1, 16);
            USVs[0].Init(horizLine12);
            USVs[0].Init(USVs_LOS[0]);
            USVs[0].Init(USVs_State_Info[0]);
            USVs[0].Init(USVs_PID_Info[0]);
            USVs[0].Init(USVs_Point_PID_[0]);

            USVs[1] = new USV(form1, 16);
            USVs[1].Init(horizLine13);
            USVs[1].Init(USVs_LOS[1]);
            USVs[1].Init(USVs_State_Info[1]);
            USVs[1].Init(USVs_PID_Info[1]);
            USVs[0].Init(USVs_Point_PID_[1]);

            USVs[2] = new USV(form1, 16);
            USVs[2].Init(horizLine14);
            USVs[2].Init(USVs_LOS[2]);
            USVs[2].Init(USVs_State_Info[2]);
            USVs[2].Init(USVs_PID_Info[2]);
            USVs[0].Init(USVs_Point_PID_[2]);

            USVs_LOS[0].pid_x.UpdateParam(Convert.ToDouble(textBox_USV1_X_Kp.Text), Convert.ToDouble(textBox_USV1_X_Ki.Text), Convert.ToDouble(textBox_USV1_X_Kd.Text), 0);
            USVs_LOS[0].pid_y.UpdateParam(Convert.ToDouble(textBox_USV1_Y_Kp.Text), Convert.ToDouble(textBox_USV1_Y_Ki.Text), Convert.ToDouble(textBox_USV1_Y_Kd.Text), 0);

            USVs_LOS[1].pid_x.UpdateParam(Convert.ToDouble(textBox_USV2_X_Kp.Text), Convert.ToDouble(textBox_USV2_X_Ki.Text), Convert.ToDouble(textBox_USV2_X_Kd.Text), 0);
            USVs_LOS[1].pid_y.UpdateParam(Convert.ToDouble(textBox_USV2_Y_Kp.Text), Convert.ToDouble(textBox_USV2_Y_Ki.Text), Convert.ToDouble(textBox_USV2_Y_Kd.Text), 0);

            USVs_LOS[2].pid_x.UpdateParam(Convert.ToDouble(textBox_USV3_X_Kp.Text), Convert.ToDouble(textBox_USV3_X_Ki.Text), Convert.ToDouble(textBox_USV3_X_Kd.Text), 0);
            USVs_LOS[2].pid_y.UpdateParam(Convert.ToDouble(textBox_USV3_Y_Kp.Text), Convert.ToDouble(textBox_USV3_Y_Ki.Text), Convert.ToDouble(textBox_USV3_Y_Kd.Text), 0);



            //*************更新参数事件初始化**********************//




            textBox12.Leave += new EventHandler(UpdateExpectedPath);
            textBox13.Leave += new EventHandler(UpdateExpectedPath);
            textBox12.Enter += new EventHandler(UpdateExpectedPath);
            textBox13.Enter += new EventHandler(UpdateExpectedPath);


            richTextBox_EXpression.TextChanged += new EventHandler(UpdateExpectedPath);


            USVs_State_Info[0].L.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[0].kp.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[0].angle.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[0].delta.Leave += new EventHandler(LOS_UpdateParam);


            USVs_State_Info[1].L.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[1].kp.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[1].angle.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[1].delta.Leave += new EventHandler(LOS_UpdateParam);


            USVs_State_Info[2].L.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[2].kp.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[2].angle.Leave += new EventHandler(LOS_UpdateParam);
            USVs_State_Info[2].delta.Leave += new EventHandler(LOS_UpdateParam);

            groupBox_USV1_PID.DoubleClick += new EventHandler(USVs[0].Read_Param);
            groupBox_USV2_PID.DoubleClick += new EventHandler(USVs[1].Read_Param);
            groupBox_USV3_PID.DoubleClick += new EventHandler(USVs[2].Read_Param);

            USVs_PID_Info[0].VEL_Kp.Leave += new EventHandler(USVs[0].PID_Updata);
            USVs_PID_Info[0].VEL_Ki.Leave += new EventHandler(USVs[0].PID_Updata);
            USVs_PID_Info[0].VEL_Kd.Leave += new EventHandler(USVs[0].PID_Updata);
            USVs_PID_Info[0].HEA_Kp.Leave += new EventHandler(USVs[0].PID_Updata);
            USVs_PID_Info[0].HEA_Ki.Leave += new EventHandler(USVs[0].PID_Updata);
            USVs_PID_Info[0].HEA_Kd.Leave += new EventHandler(USVs[0].PID_Updata);

            USVs_PID_Info[1].VEL_Kp.Leave += new EventHandler(USVs[1].PID_Updata);
            USVs_PID_Info[1].VEL_Ki.Leave += new EventHandler(USVs[1].PID_Updata);
            USVs_PID_Info[1].VEL_Kd.Leave += new EventHandler(USVs[1].PID_Updata);
            USVs_PID_Info[1].HEA_Kp.Leave += new EventHandler(USVs[1].PID_Updata);
            USVs_PID_Info[1].HEA_Ki.Leave += new EventHandler(USVs[1].PID_Updata);
            USVs_PID_Info[1].HEA_Kd.Leave += new EventHandler(USVs[1].PID_Updata);

            USVs_PID_Info[2].VEL_Kp.Leave += new EventHandler(USVs[2].PID_Updata);
            USVs_PID_Info[2].VEL_Ki.Leave += new EventHandler(USVs[2].PID_Updata);
            USVs_PID_Info[2].VEL_Kd.Leave += new EventHandler(USVs[2].PID_Updata);
            USVs_PID_Info[2].HEA_Kp.Leave += new EventHandler(USVs[2].PID_Updata);
            USVs_PID_Info[2].HEA_Ki.Leave += new EventHandler(USVs[2].PID_Updata);
            USVs_PID_Info[2].HEA_Kd.Leave += new EventHandler(USVs[2].PID_Updata);

            USVs_Point_PID_[0].X_Kp.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[0].X_Ki.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[0].X_Kd.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[0].Y_Kp.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[0].Y_Ki.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[0].Y_Kd.Leave += new EventHandler(Point_PID_Update);

            USVs_Point_PID_[1].X_Kp.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[1].X_Ki.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[1].X_Kd.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[1].Y_Kp.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[1].Y_Ki.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[1].Y_Kd.Leave += new EventHandler(Point_PID_Update);

            USVs_Point_PID_[2].X_Kp.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[2].X_Ki.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[2].X_Kd.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[2].Y_Kp.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[2].Y_Ki.Leave += new EventHandler(Point_PID_Update);
            USVs_Point_PID_[2].Y_Kd.Leave += new EventHandler(Point_PID_Update);



            button_USV1_LOCK.Click += new EventHandler(USVs[0].LOCK_EVENT);
            button_USV2_LOCK.Click += new EventHandler(USVs[1].LOCK_EVENT);
            button_USV3_LOCK.Click += new EventHandler(USVs[2].LOCK_EVENT);

            radioButton_USV1_Rocker.Click += new EventHandler(Rocker_Switch);
            radioButton_USV2_Rocker.Click += new EventHandler(Rocker_Switch);
            radioButton_USV3_Rocker.Click += new EventHandler(Rocker_Switch);
            radioButton_USV_LOCK.Click += new EventHandler(Rocker_Switch);



            TrkBar_LeftX.ValueChanged += new System.EventHandler(TrackBarValueChanged);
            TrkBar_LeftY.ValueChanged += new System.EventHandler(TrackBarValueChanged);
            TrkBar_RightX.ValueChanged += new System.EventHandler(TrackBarValueChanged);
            TrkBar_RightY.ValueChanged += new System.EventHandler(TrackBarValueChanged);
            TrkBar_LeftX.MouseUp += new System.Windows.Forms.MouseEventHandler(TrkBarMouseUp);
            TrkBar_LeftY.MouseUp += new System.Windows.Forms.MouseEventHandler(TrkBarMouseUp);
            TrkBar_RightX.MouseUp += new System.Windows.Forms.MouseEventHandler(TrkBarMouseUp);
            TrkBar_RightY.MouseUp += new System.Windows.Forms.MouseEventHandler(TrkBarMouseUp);
            this.TrkBar_LeftX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrkBarMouseDown);
            this.TrkBar_LeftY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrkBarMouseDown);
            this.TrkBar_RightX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrkBarMouseDown);
            this.TrkBar_RightY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrkBarMouseDown);
            line1.Title = "Roll";
            line2.Title = "Pitch";
            line3.Title = "Yaw";
            line4.Title = "Roll";
            line5.Title = "Pitch";
            line6.Title = "Yaw";
            line7.Title = "RollSpeed";
            line8.Title = "PitchSpeed";
            line9.Title = "YawSpeed";
            line1.Color = Color.Red;
            line2.Color = Color.Green;
            line3.Color = Color.Blue;
            line4.Color = Color.Red;
            line5.Color = Color.Green;
            line6.Color = Color.Blue;
            line7.Color = Color.Cyan;
            line8.Color = Color.Indigo;
            line9.Color = Color.DarkOrange;

            FastLines[0] = fastLine1;
            FastLines[1] = fastLine2;
            FastLines[2] = fastLine3;
            FastLines[3] = fastLine4;
            FastLines[4] = fastLine5;
            FastLines[5] = fastLine6;
            FastLines[6] = fastLine7;
            FastLines[7] = fastLine8;
            FastLines[8] = fastLine9;
            FastLines[9] = fastLine10;
            FastLines[10] = fastLine11;
            FastLines[11] = fastLine12;
            FastLines[12] = fastLine13;
            FastLines[13] = fastLine14;
            FastLines[14] = fastLine15;
            FastLines[15] = fastLine16;
            FastLines[16] = fastLine17;
            FastLines[17] = fastLine18;
            FastLines[18] = fastLine19;
            FastLines[19] = fastLine20;
            FastLines[20] = fastLine21;
            for(int i=0;i<20;i++)
            {
                FastLines[i].Active = false;
                FastLines[i].Legend.Visible = false;
            }
            FastLines[20].Legend.Visible = false;

            HorizLines[0] = horizLine1;
            HorizLines[1] = horizLine2;
            HorizLines[2] = horizLine3;
            HorizLines[3] = horizLine4;
            HorizLines[4] = horizLine5;
            HorizLines[5] = horizLine6;
            HorizLines[6] = horizLine7;
            HorizLines[7] = horizLine8;
            HorizLines[8] = horizLine9;
            HorizLines[9] = horizLine10;
            HorizLines[10] = horizLine11;
            HorizLines[11] = horizLine12;
            HorizLines[12] = horizLine13;
            HorizLines[13] = horizLine14;
            HorizLines[14] = horizLine15;
            HorizLines[15] = horizLine16;
            HorizLines[16] = horizLine17;
            HorizLines[17] = horizLine18;
            HorizLines[18] = horizLine19;
            HorizLines[19] = horizLine20;
            for(int i=0;i<20;i++)
            {
                HorizLines[i].Legend.Visible = false;
                HorizLines[i].Active = false;
            }

            horizLine11.Active = true;
            horizLine11.Legend.Visible = true;
            horizLine12.Active = true;
            horizLine12.Legend.Visible = true;
            horizLine13.Active = true;
            horizLine13.Legend.Visible = true;
            horizLine14.Active = true;
            horizLine14.Legend.Visible = true;
            horizLine15.Active = true;
            horizLine15.Legend.Visible = true;

            VirtualLeader.horizLine = HorizLines[0];

            VirtualLeader.horizLine.Legend.Visible = true;
            VirtualLeader.horizLine.Active = true;

            for (int i = 0; i < 3; i++)
            {
                UAV_Followers[i].HorizLines[0] = HorizLines[i * 2 + 1];//期望曲线
                UAV_Followers[i].HorizLines[1] = HorizLines[i * 2 + 2];//实际曲线
                UAV_Followers[i].HorizLines[0].Active = true;
                UAV_Followers[i].HorizLines[0].Legend.Visible = true;
                UAV_Followers[i].HorizLines[1].Active = true;
                UAV_Followers[i].HorizLines[1].Legend.Visible = true;
            }
            VirtualLeader.horizLine.Active = true;
            VirtualLeader.horizLine.Visible = true;
            VirtualLeader.horizLine.Legend.Visible = true;
            VirtualLeader.horizLine.Title = "VirtualLeader";

            UAV_Followers[0].HorizLines[0].Title = "UAV_Follwer_1_Exp";
            UAV_Followers[0].HorizLines[1].Title = "UAV_Follwer_1_RTP";
            UAV_Followers[1].HorizLines[0].Title = "UAV_Follwer_2_Exp";
            UAV_Followers[1].HorizLines[1].Title = "UAV_Follwer_2_RTP";
            UAV_Followers[2].HorizLines[0].Title = "UAV_Follwer_3_Exp";
            UAV_Followers[2].HorizLines[1].Title = "UAV_Follwer_3_RTP";

            tabControl1.SelectedIndex = 6;//设置选项卡选中页
            new TBoxOnlyNumber(textBox4);
            new TBoxOnlyNumber(textBox5);
            RB_ProgramControlLeader.CheckedChanged += new EventHandler(VirtualLeaderSelectMode);
            RB_RockerControlLeader.CheckedChanged += new EventHandler(VirtualLeaderSelectMode);
            msg_Rocker.leftX = 1500;
            msg_Rocker.leftY = 1000;
            msg_Rocker.rightX = 1500;
            msg_Rocker.rightY = 1500;
            msg_Rocker.switchA = 1500;
            msg_Rocker.switchB = 1500;
            msg_Rocker.switchC = 1500;
            msg_Rocker.switchD = 1500;
            msg_Rocker.switchE = 1500;
            msg_Rocker.switchF = 1500;
            msg_Rocker.switchG = 1500;

            tChart5.Visible = true;
            tChart5.SendToBack();
            button12.Visible = true;
            button12.SendToBack();


            DrawExpectedPath(horizLine11);

            new TBoxOnlyNumber(TB_FormationGatherSpeed);
            tcpserver = new TCP((Form1)sender);//
        }

        private void Button_USV2_LOCK_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SendMavMsgToRocker(MavlinkPacket mp)//发送Mavlink消息给摇杆
        {
            mavsend.Clear();
            mavsend.AddRange(mavlink.Send(mp));
            SerialPortSend(mavsend);
                
        }         
        public void SerialPortSend(List<byte> buffer)//串口发送
        {
            if (!serialPort1.IsOpen)
            {
                SystemInfo("请打开串口！");
                Btn_OpenClose.Text = "打开串口";
                return;
            }
            serialPort1.Write(buffer.ToArray(), 0, buffer.Count);
            TxCount += buffer.Count;
            Lab_Send_Cnt.Text = "TX: " + TxCount.ToString();
        }
        public void SerialPortSend(string text)//串口发送
        {
            if (!serialPort1.IsOpen)
            {
                Btn_OpenClose.Text = "打开串口";
                return;
            }
            serialPort1.Write(text);
            TxCount += System.Text.Encoding.Default.GetByteCount(TB_Send.Text);
            Lab_Send_Cnt.Text = "TX: " + TxCount.ToString();
        }
        public static byte[] HexStringToByteArray(string s)//2018/04/03修改：修复转换出错
        {
            if (s.Length == 0)
                throw new Exception("将16进制字符串转换成字节数组时出错，错误信息：被转换的字符串长度为0。");
            s = s.ToUpper();
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = System.Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        public void SystemInfo(string str)//系统信息显示
        {
            label11.Text = str;
            label11time = 2000 / timer1.Interval;
        }
       
        private void Timer1_Tick(object sender, EventArgs e)
        {
            Lab_ErrBytesCnt.Text = "EB: " + mavlinkBytesUnused;
            if (RxCount == 0)
                Lab_ErrRate.Text = "ER: 0%";
            else
                Lab_ErrRate.Text = "ER: "+(mavlinkBytesUnused * 100.0 / RxCount).ToString("0.00")+"%";
            if (label11time > 0)
            {
                label11.Visible = true;
                label11time--;
            }
            else
                label11.Visible = false;
            int rx_count;
            if (!serialPort1.IsOpen)
            {
                Btn_OpenClose.Text = "打开串口";
            }
            else
            {
                rx_count = serialPort1.BytesToRead;
                if (rx_count <= 0)
                    return;
                RxCount += rx_count;
                Lab_Recv_Cnt.Text = "RX: " + RxCount.ToString();
                byte[] rx_data = new byte[rx_count];
                serialPort1.Read(rx_data, 0, rx_count);
                if (RB_Recv_Text.Checked)//文本显示
                {
                    string str = System.Text.Encoding.GetEncoding("GB2312").GetString(rx_data);
                    if (!checkBox1.Checked)
                    {
                        TB_Recv.AppendText(str);
                        TB_Status.AppendText("接收文本：" + str + "\r\n");
                    }
                }
                else//十六进制显示
                {
                    string str = BitConverter.ToString(rx_data);
                    str = str.Replace("-", " ");
                    str += " ";
                    if (!checkBox1.Checked)
                    {
                        TB_Recv.AppendText(str);
                        TB_Status.AppendText("接收Hex：" + str + "\r\n");
                    }
                }
                mavlink.ParseBytes(rx_data);
            }
            
            if(!serialPort2.IsOpen)//检查UWB是否连接
            {
                button13.Text = "连接UWB";
            }
            else
            {
                //UWB数据处理
                int len;
                len = serialPort2.BytesToRead;
                if (len <= 0)
                    return;
                byte[] uwb_buf = new byte[len];
                serialPort2.Read(uwb_buf, 0, len);
            }

        }
        private void CB_Port_Sel_Click(object sender, EventArgs e)//搜索可用串口
        {
            ComboBox cb = (ComboBox)sender;
            string LastPortName = null;
            if (cb.SelectedItem != null)
                LastPortName = cb.SelectedItem.ToString();
            cb.Items.Clear();//清空
            cb.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            for (int i = 0; i < cb.Items.Count; i++)
            {
                if (LastPortName == cb.GetItemText(cb.Items[i]))
                    cb.SelectedIndex = i;
            }
            if (cb.SelectedIndex != -1)
            {
                if (cb.GetItemText(cb.Items[cb.SelectedIndex]) != LastPortName)
                    cb.SelectedIndex = 0;
            }
        }
        private void CB_Port_Sel_DropDownClosed(object sender, EventArgs e)//串口端口设置
        {
            if(((ComboBox)sender).Text==comboBox1.Text)
            {
                if (button13.Text == "连接UWB")
                    return;
                Btn_UWB_OpenClose_Click(null, null);//先关闭串口
                Btn_UWB_OpenClose_Click(null, null);//接着打开串口
            }
            else if(((ComboBox)sender).Text == CB_Port_Sel.Text)
            {
                if (Btn_OpenClose.Text == "打开串口")
                    return;
                Btn_OpenClose_Click(null, null);//先关闭串口
                Btn_OpenClose_Click(null, null);//接着打开串口
            }
        }
        private void CB_Baud_Sel_DropDownClosed(object sender, EventArgs e)//串口波特率设置
        {
            if (((ComboBox)sender).Text == comboBox2.Text)
            {
                if (button13.Text == "连接UWB")
                    return;
                Btn_UWB_OpenClose_Click(null, null);//先关闭串口
                Btn_UWB_OpenClose_Click(null, null);//接着打开串口
            }
            else if (((ComboBox)sender).Text == CB_Baud_Sel.Text)
            {
                if (Btn_OpenClose.Text == "打开串口")
                    return;
                Btn_OpenClose_Click(null, null);//先关闭串口
                Btn_OpenClose_Click(null, null);//接着打开串口
            }
        }
        private void Btn_CLR_Click(object sender, EventArgs e)//清理缓存
        {
            TxCount = 0;
            RxCount = 0;
            mavlinkBytesUnused = 0;
            TB_Recv.Clear();
            TB_Status.Clear();
            Lab_Recv_Cnt.Text = "RX: 0";
            Lab_Send_Cnt.Text = "TX: 0";
            for(int i=0;i<20;i++)
            {
                FastLines[i].Clear();
                HorizLines[i].Clear();
                FastLines[i].Active = false;
                FastLines[i].Legend.Visible = false;
            }
            line1.Clear();
            line2.Clear();
            line3.Clear();
            line4.Clear();
            line5.Clear();
            line6.Clear();
            line7.Clear();
            line8.Clear();
            line9.Clear();
            tChart1.Axes.Bottom.SetMinMax(0, 1000);
            tChart1.Axes.Left.Grid.DrawEvery = 1;
            tChart1.Axes.Bottom.Grid.DrawEvery = 1;
            tChart2.Axes.Bottom.SetMinMax(0, 1000);
            tChart2.Axes.Left.Grid.DrawEvery = 1;
            tChart2.Axes.Bottom.Grid.DrawEvery = 1;
            tChart3.Axes.Bottom.SetMinMax(0, 1000);
            tChart3.Axes.Left.Grid.DrawEvery = 1;
            tChart3.Axes.Bottom.Grid.DrawEvery = 1;
            
            CKLBox_DataSel.Items.Clear();

            
            GC.Collect();
        }
        private void Btn_UWB_OpenClose_Click(object sender, EventArgs e)//连接UWB
        {
            if (button13.Text == "连接UWB")
            {
                try
                {
                    serialPort2.PortName = comboBox1.SelectedItem.ToString();
                    serialPort2.BaudRate = System.Convert.ToInt32(comboBox2.SelectedItem);
                    serialPort2.Encoding = System.Text.Encoding.GetEncoding("GB2312");
                    serialPort2.ReceivedBytesThreshold = 1;
                    serialPort2.Open();
                    button13.Text = "断开UWB";
                    SystemInfo(serialPort2.PortName + "已连接！波特率 = " + serialPort2.BaudRate + "\r\n");
                }
                catch (Exception)
                {
                    button13.Text = "连接UWB";
                    SystemInfo("端口不存在或被占用！\r\n");
                }
            }
            else
            {
                try
                {
                    button13.Text = "连接UWB";
                    serialPort2.Close();
                    SystemInfo(serialPort2.PortName + "已断开！\r\n");
                }
                catch (Exception ex)
                {
                    button13.Text = "断开UWB";
                    SystemInfo(ex.Message + "\r\n");
                }
            }
        }
        private void Btn_OpenClose_Click(object sender, EventArgs e)//打开/关闭串口
        {
            if (Btn_OpenClose.Text == "打开串口")
            {
                try
                {
                    serialPort1.PortName = CB_Port_Sel.SelectedItem.ToString();
                    serialPort1.BaudRate = Convert.ToInt32(CB_Baud_Sel.SelectedItem);
                    serialPort1.Encoding = Encoding.GetEncoding("GB2312");
                    serialPort1.ReceivedBytesThreshold = 1;
                    serialPort1.Open();
                    Btn_OpenClose.Text = "关闭串口";
                    SystemInfo(serialPort1.PortName + "已开启！波特率 = " + serialPort1.BaudRate + "\r\n");
                    for(int i =0;i<20;i++)
                    {
                        FastLines[i].Active = false;
                        FastLines[i].Legend.Visible = false;
                    }
                    param_read_flag = false;
                }
                catch (Exception)
                {
                    Btn_OpenClose.Text = "打开串口";
                    SystemInfo("端口不存在或被占用！\r\n");
                }
            }
            else
            {
                try
                {
                    Btn_OpenClose.Text = "打开串口";
                    serialPort1.Close();
                    SystemInfo(serialPort1.PortName + "已关闭！\r\n");
                }
                catch (Exception ex)
                {
                    Btn_OpenClose.Text = "关闭串口";
                    SystemInfo(ex.Message + "\r\n");
                }
            }
        }
        private void Btn_Send_Click(object sender, EventArgs e)//发送按钮
        {
            if (serialPort1.IsOpen)
            {
                if (RB_Send_Text.Checked)//文本发送
                {
                    SerialPortSend(TB_Send.Text);
                    if (TB_Send.TextLength > 0)
                        TB_Status.AppendText("发送文本：" + TB_Send.Text + "\r\n");
                }
                else//hex发送
                {
                    try
                    {
                        List<byte> str = new List<byte>();
                        str.AddRange(HexStringToByteArray(TB_Send.Text));
                        SerialPortSend(str);
                        string txt_tmp = BitConverter.ToString(str.ToArray()).Replace("-", " ");
                        TB_Status.AppendText("发送Hex：" + txt_tmp + "\r\n");
                    }
                    catch (Exception)
                    {
                        TB_Status.AppendText("请输入合法的16进制数据！\r\n");
                    }
                }
            }
        }
        private void TrkBarMouseDown(object sender, MouseEventArgs e)
        {
            int x, bias = 14;
            if (((TrackBar)sender).Name == TrkBar_LeftX.Name)
            {
                if (e.X <= bias)
                    x = 0;
                else if (e.X >= TrkBar_LeftX.Width - bias)
                    x = TrkBar_LeftY.Width - bias * 2;
                else
                    x = e.X - bias;
                TrkBar_LeftX.Value = System.Convert.ToInt32(x * System.Convert.ToDouble(TrkBar_LeftX.Maximum - TrkBar_LeftX.Minimum)
                                    / (TrkBar_LeftX.Width - bias * 2) + TrkBar_LeftX.Minimum);
            }
            else if (((TrackBar)sender).Name == TrkBar_LeftY.Name)
            {
                if (e.X <= bias)
                    x = 0;
                else if (e.X >= TrkBar_LeftY.Width - bias)
                    x = TrkBar_LeftY.Width - bias * 2;
                else
                    x = e.X - bias;
                TrkBar_LeftY.Value = System.Convert.ToInt32(x * System.Convert.ToDouble(TrkBar_LeftY.Maximum - TrkBar_LeftY.Minimum)
                                        / (TrkBar_LeftY.Width - bias * 2) + TrkBar_LeftY.Minimum);
            }
            else if (((TrackBar)sender).Name == TrkBar_RightX.Name)
            {
                if (e.X <= bias)
                    x = 0;
                else if (e.X >= TrkBar_RightX.Width - bias)
                    x = TrkBar_LeftY.Width - bias * 2;
                else
                    x = e.X - bias;
                TrkBar_RightX.Value = System.Convert.ToInt32(x * System.Convert.ToDouble(TrkBar_RightX.Maximum - TrkBar_RightX.Minimum)
                                    / (TrkBar_RightX.Width - bias * 2) + TrkBar_RightX.Minimum);
            }
            else if (((TrackBar)sender).Name == TrkBar_RightY.Name)
            {
                if (e.X <= bias)
                    x = 0;
                else if (e.X >= TrkBar_RightY.Width - bias)
                    x = TrkBar_LeftY.Width - bias * 2;
                else
                    x = e.X - bias;
                TrkBar_RightY.Value = System.Convert.ToInt32(x * System.Convert.ToDouble(TrkBar_RightY.Maximum - TrkBar_RightY.Minimum)
                                    / (TrkBar_RightY.Width - bias * 2) + TrkBar_RightY.Minimum);
            }
        }
        private void TrkBarMouseUp(object sender, MouseEventArgs e)
        {
            if(((TrackBar)sender).Name== TrkBar_LeftX.Name)
                TrkBar_LeftX.Value = 1500;
            else if (((TrackBar)sender).Name == TrkBar_LeftY.Name&&checkBox2.Checked)
                TrkBar_LeftY.Value = 1500;
            else if (((TrackBar)sender).Name == TrkBar_RightX.Name)
                TrkBar_RightX.Value = 1500;
            else if (((TrackBar)sender).Name == TrkBar_RightY.Name)
                TrkBar_RightY.Value = 1500;
        }
        private void TrackBarValueChanged(object sender, EventArgs e)
        {
            label7.Text = TrkBar_LeftY.Value.ToString();
            label8.Text = TrkBar_RightY.Value.ToString();
            label9.Text = TrkBar_RightX.Value.ToString();
            label10.Text = TrkBar_LeftX.Value.ToString();
            if (serialPort1.IsOpen == false)//串口未打开
            {
                SystemInfo("请打开串口！");
                return;
            }
            if (radioButton6.Checked)
            {
                msg_Rocker.leftX = (short)TrkBar_LeftX.Value;
                msg_Rocker.leftY = (short)TrkBar_LeftY.Value;
                msg_Rocker.rightX = (short)TrkBar_RightX.Value;
                msg_Rocker.rightY = (short)TrkBar_RightY.Value;
                MavlinkSendMsg(msg_Rocker);
            }
        }
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)//回中
        {
            if(checkBox2.Checked==true)
                TrkBar_LeftY.Value = 1500;
            else if(checkBox2.Checked==false)
                TrkBar_LeftY.Value = 1000;
        }
        private void Btn_FlightUnlock_Click(object sender, EventArgs e)//解锁+加锁
        {
            if (serialPort1.IsOpen == false)//串口未打开
            {
                SystemInfo("请打开串口！");
                return;
            }
            if (Btn_FlightUnlock.Text == "解 锁")
            {
                Btn_FlightUnlock.Text = "加 锁";
                Btn_FlightUnlock.BackColor = Color.Lime;
                msg_Cmd_Write.cmd_id = (byte)MavLink.CMD_TYPE.CMD_UNLOCK;
                mavlinkPacket.Message = msg_Cmd_Write;
                SendMavMsgToRocker(mavlinkPacket);
            }
            else if (Btn_FlightUnlock.Text == "加 锁")
            {
                Btn_FlightUnlock.Text = "解 锁";
                Btn_FlightUnlock.BackColor = Color.OrangeRed;
                msg_Cmd_Write.cmd_id = (byte)MavLink.CMD_TYPE.CMD_LOCK;
                mavlinkPacket.Message = msg_Cmd_Write;
                SendMavMsgToRocker(mavlinkPacket);
            }
            mavlinkPacket.Message = msg_Rocker;
            SendMavMsgToRocker(mavlinkPacket);
        }
        private void RadioButton7_CheckedChanged(object sender, EventArgs e)//摇杆控制
        {
            if (radioButton7.Checked)
            {
                groupBox4.Enabled = false;
            }
        }
        private void RadioButton6_CheckedChanged(object sender, EventArgs e)//虚拟摇杆控制
        {
            if (radioButton6.Checked)
            {
                groupBox4.Enabled = true;
            }
        }
        public void MavlinkSendMsg(MavLink.MavlinkMessage msg)//发送Mavlink消息
        {
            mavlinkPacket.Message = msg;
            SendMavMsgToRocker(mavlinkPacket);
        }
        private void CKLBox_DataSel_MouseEnter(object sender, EventArgs e)//鼠标经过数据选择框
        {
            if (CKLBox_DataSel.Items.Count > 0)
            {
                CKLBox_DataSel.Height = 300;
            }
        }
        private void CKLBox_DataSel_MouseLeave(object sender, EventArgs e)//鼠标离开数据选择框
        {
            CKLBox_DataSel.Height = 24;
        }
        private void CKLBox_DataSel_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (((CheckedListBox)sender).CheckedItems.Count>=20)
            {
                if (e.NewValue==CheckState.Checked)
                {
                    e.NewValue = CheckState.Unchecked;
                }
            }
        }
        private void CKLBox_DataSel_SelectedValueChanged(object sender, EventArgs e)//数据选择有变化
        {
            if (((CheckedListBox)sender).CheckedItems.IndexOf(((CheckedListBox)sender).Text) < 0)//取消选择
            {
                switch (((CheckedListBox)sender).CheckedItems.Count)
                {
                    case 0:
                        fastLine1.Active = false;
                        fastLine1.Legend.Visible = false;
                        break;
                    case 1:
                        fastLine2.Active = false;
                        fastLine2.Legend.Visible = false;
                        break;
                    case 2:
                        fastLine3.Active = false;
                        fastLine3.Legend.Visible = false;
                        break;
                    case 3:
                        fastLine4.Active = false;
                        fastLine4.Legend.Visible = false;
                        break;
                    case 4:
                        fastLine5.Active = false;
                        fastLine5.Legend.Visible = false;
                        break;
                    case 5:
                        fastLine6.Active = false;
                        fastLine6.Legend.Visible = false;
                        break;
                    case 6:
                        fastLine7.Active = false;
                        fastLine7.Legend.Visible = false;
                        break;
                    case 7:
                        fastLine8.Active = false;
                        fastLine8.Legend.Visible = false;
                        break;
                    case 8:
                        fastLine9.Active = false;
                        fastLine9.Legend.Visible = false;
                        break;
                    case 9:
                        fastLine10.Active = false;
                        fastLine10.Legend.Visible = false;
                        break;
                    case 10:
                        fastLine11.Active = false;
                        fastLine11.Legend.Visible = false;
                        break;
                    case 11:
                        fastLine12.Active = false;
                        fastLine12.Legend.Visible = false;
                        break;
                    case 12:
                        fastLine13.Active = false;
                        fastLine13.Legend.Visible = false;
                        break;
                    case 13:
                        fastLine14.Active = false;
                        fastLine14.Legend.Visible = false;
                        break;
                    case 14:
                        fastLine15.Active = false;
                        fastLine15.Legend.Visible = false;
                        break;
                    case 15:
                        fastLine16.Active = false;
                        fastLine16.Legend.Visible = false;
                        break;
                    case 16:
                        fastLine17.Active = false;
                        fastLine17.Legend.Visible = false;
                        break;
                    case 17:
                        fastLine18.Active = false;
                        fastLine18.Legend.Visible = false;
                        break;
                    case 18:
                        fastLine19.Active = false;
                        fastLine19.Legend.Visible = false;
                        break;
                    case 19:
                        fastLine20.Active = false;
                        fastLine20.Legend.Visible = false;
                        break;
                    default: break;
                }
            }
            else//选择
            {
                switch (((CheckedListBox)sender).CheckedItems.Count)
                {
                    case 1:
                        fastLine1.Active = true;
                        fastLine1.Legend.Visible = true;
                        break;
                    case 2:
                        fastLine2.Active = true;
                        fastLine2.Legend.Visible = true;
                        break;
                    case 3:
                        fastLine3.Active = true;
                        fastLine3.Legend.Visible = true;
                        break;
                    case 4:
                        fastLine4.Active = true;
                        fastLine4.Legend.Visible = true;
                        break;
                    case 5:
                        fastLine5.Active = true;
                        fastLine5.Legend.Visible = true;
                        break;
                    case 6:
                        fastLine6.Active = true;
                        fastLine6.Legend.Visible = true;
                        break;
                    case 7:
                        fastLine7.Active = true;
                        fastLine7.Legend.Visible = true;
                        break;
                    case 8:
                        fastLine8.Active = true;
                        fastLine8.Legend.Visible = true;
                        break;
                    case 9:
                        fastLine9.Active = true;
                        fastLine9.Legend.Visible = true;
                        break;
                    case 10:
                        fastLine10.Active = true;
                        fastLine10.Legend.Visible = true;
                        break;
                    case 11:
                        fastLine11.Active = true;
                        fastLine11.Legend.Visible = true;
                        break;
                    case 12:
                        fastLine12.Active = true;
                        fastLine12.Legend.Visible = true;
                        break;
                    case 13:
                        fastLine13.Active = true;
                        fastLine13.Legend.Visible = true;
                        break;
                    case 14:
                        fastLine14.Active = true;
                        fastLine14.Legend.Visible = true;
                        break;
                    case 15:
                        fastLine15.Active = true;
                        fastLine15.Legend.Visible = true;
                        break;
                    case 16:
                        fastLine16.Active = true;
                        fastLine16.Legend.Visible = true;
                        break;
                    case 17:
                        fastLine17.Active = true;
                        fastLine17.Legend.Visible = true;
                        break;
                    case 18:
                        fastLine18.Active = true;
                        fastLine18.Legend.Visible = true;
                        break;
                    case 19:
                        fastLine19.Active = true;
                        fastLine19.Legend.Visible = true;
                        break;
                    case 20:
                        fastLine20.Active = true;
                        fastLine20.Legend.Visible = true;
                        break;
                    default: break;
                }
            }
            
            fastLine1.Clear();
            fastLine2.Clear();
            fastLine3.Clear();
            fastLine4.Clear();
            fastLine5.Clear();
            fastLine6.Clear();
            fastLine7.Clear();
            fastLine8.Clear();
            fastLine9.Clear();
            fastLine10.Clear();
            fastLine11.Clear();
            fastLine12.Clear();
            fastLine13.Clear();
            fastLine14.Clear();
            fastLine15.Clear();
            fastLine16.Clear();
            fastLine17.Clear();
            fastLine18.Clear();
            fastLine19.Clear();
            fastLine20.Clear();
            tChart2.Axes.Bottom.SetMinMax(0, 1000);
            tChart2.Axes.Left.Grid.DrawEvery = 1;
            tChart2.Axes.Bottom.Grid.DrawEvery = 1;
        }
        private void ParamReadListRequest()//读参数列表请求
        {
            if (param_read_flag == true)
                return;
            if (!serialPort1.IsOpen)
                SystemInfo("请打开串口!");
            else
            {
                dataGridView1.Rows.Clear();
                msg_Param_Read.param_id = (byte)MavLink.PARAM_TYPE.PARAM_LIST_REQUEST;
                mavlinkPacket.Message = msg_Param_Read;
                SendMavMsgToRocker(mavlinkPacket);

                SystemInfo("参数读取中..");
                param_read_flag = true;
                Delay_ms(1000);
                if (dataGridView1.RowCount <= 0)
                {
                    param_read_flag = false;
                    SystemInfo("参数读取失败！请检查连接！");
                }
                else
                {
                    SystemInfo("参数读取完成！");
                }
            }
        }
        private void DataGridView1_Click(object sender, EventArgs e)
        {
            ParamReadListRequest();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)//datagridview回车不换行
        {
            if (keyData == Keys.Enter && dataGridView1.ContainsFocus)
            {
                dataGridView1.EndEdit();
                return true;
            }
            if (keyData == Keys.Enter && dataGridView2.ContainsFocus)
            {
                dataGridView1.EndEdit();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);//继续原来base.ProcessCmdKey中的处理
        }
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)//编辑开始，保存原始值
        {
            DataGridView1_CellOldValue = System.Convert.ToSingle(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)//编辑结束，如果值有变化，发送写参数命令至下位机。如果发送失败退回原始值
        {
            if (DataGridView1_CellOldValue!= System.Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value))//值有变化
            {
                msg_Param_Write.param_id = System.Convert.ToByte(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                msg_Param_Write.value = System.Convert.ToSingle(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                mavlinkPacket.Message = msg_Param_Write;
                SendMavMsgToRocker(mavlinkPacket);
                MavlinkParamWriteAck = false;
                SystemInfo("写入参数" + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + "。。");
                Delay_ms(1000);//非阻塞延时
                if (MavlinkParamWriteAck == false)//没有应答
                {
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = DataGridView1_CellOldValue;
                    SystemInfo("写入参数失败！请检查连接！");
                    param_read_flag = false;
                }
            }
        }
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)//datagridview只允许输入数字
        {
            TextBox DataGridView1_Tx = e.Control as TextBox;
            DataGridView1_Tx.KeyPress -= new KeyPressEventHandler(DataGridView1_KeyPress);
            DataGridView1_Tx.KeyPress += new KeyPressEventHandler(DataGridView1_KeyPress);
        }
        private void DataGridView1_KeyPress(object sender, KeyPressEventArgs e)//datagridview只允许输入数字
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
                e.Handled = true;
        }
        private void TabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 3)
            {
                ParamReadListRequest();
            }
        }
        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)//命令配置
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                msg_Cmd_Write.cmd_id = System.Convert.ToByte(dataGridView2.Rows[e.RowIndex].Cells[0].Value);
                MavlinkCMDAck = false;
                mavlinkPacket.Message = msg_Cmd_Write;
                SendMavMsgToRocker(mavlinkPacket);
                SystemInfo("写入命令" + dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString() + "。。");
                Delay_ms(1000);//非阻塞延时
                if (MavlinkCMDAck == false)//没有应答
                {
                    SystemInfo("写入命令失败！请检查连接！");
                }

            }
        }
        private void DrawPath(HorizLine horizLine, double x, double y)
        {
            horizLine.Add(x, y);
        }
        private void VirtualLeaderControl(double dt)
        {
            if (RB_ProgramControlLeader.Checked)//程序控制
            {
                if (PathTrackingEnable==true)
                {
                    //虚拟领航船
                    VirtualLeader.Simulation(dt);//假设虚拟船始终运行在预定轨迹上
                    DrawPath(VirtualLeader.horizLine, VirtualLeader.x, VirtualLeader.y);
                }
            }
            else if (RB_RockerControlLeader.Checked)//摇杆控制
            {
                if (serialPort1.IsOpen == false)//串口未打开
                {
                    SystemInfo("请打开串口！");
                    groupBox8.Enabled = false;
                    return;
                }
                if (msg_Rocker.switchG != 2000)
                {
                    SystemInfo("请将左摇杆切换为上下不回中状态！");
                    groupBox8.Enabled = false;
                    return;
                }
                else
                {
                    groupBox8.Enabled = true;
                }
                VirtualLeader.psi += -(msg_Rocker.rightX - 1500) / 500.0 * Convert.ToDouble(textBox5.Text);//艏向角
                while (VirtualLeader.psi > 180)
                    VirtualLeader.psi -= 360;
                while (VirtualLeader.psi < -180)
                    VirtualLeader.psi += 360;
                VirtualLeader.Speed_Exp = (msg_Rocker.leftY - 1000) / 1000.0 * Convert.ToDouble(textBox4.Text);//船速
                VirtualLeader.vx = VirtualLeader.Speed_Exp * Math.Cos(VirtualLeader.psi * Math.PI / 180);
                VirtualLeader.vy = VirtualLeader.Speed_Exp * Math.Sin(VirtualLeader.psi * Math.PI / 180);
                VirtualLeader.x += VirtualLeader.vx;
                VirtualLeader.y += VirtualLeader.vy;
                DrawPath(VirtualLeader.horizLine, VirtualLeader.x, VirtualLeader.y);
            }
        }
        private void Timer2_Tick(object sender, EventArgs e)
        {
            double dt;
            dt = timer2.Interval * 0.001;

            // LOS_Control(dt);
            textBox_X.Text = USVs[0].Position.X.ToString("0.00");
            textBox_Y.Text = USVs[0].Position.Y.ToString("0.00");
            if (radioButton_Single_USV.Checked)//单船
            {
                USVs[0].LOS_Control(dt);
            }
            else if (radioButton_formation.Checked)//编队
            {
                if (radioButton_Point.Checked)
                {
                    USVs[2].Los.Update_XY_F(Points_Copy[1].X, Points_Copy[1].Y, Points_Copy[0].X, Points_Copy[0].Y);//更新点
                    USVs[2].LOS_Point_Leader(dt);
                    USVs[0].LOS_Point_Follower(dt, USVs[2].Position.X, USVs[2].Position.Y, USVs[2].state.heading);
                    USVs[1].LOS_Point_Follower(dt, USVs[2].Position.X, USVs[2].Position.Y, USVs[2].state.heading);
                    if (Points_Copy.Count >= 2)
                    {
                        float distance, a, b, c;
                        float X0 = Points_Copy[1].X;
                        float Y0 = Points_Copy[1].Y;
                        double X = USVs[2].Position.X;
                        double Y = USVs[2].Position.Y;

                        float k = (Points_Copy[1].Y - Points_Copy[0].Y) / (Points_Copy[1].X - Points_Copy[0].X);
                        float X1 = Convert.ToSingle(Points_Copy[1].X - USVs[2].Position.X);
                        float Y1 = Convert.ToSingle(Points_Copy[1].Y - USVs[2].Position.Y);
                        float X2 = Convert.ToSingle(Points_Copy[1].X - Points_Copy[0].X);
                        float Y2 = Convert.ToSingle(Points_Copy[1].Y - Points_Copy[0].Y);
                        a = -1; b = k; c = X0 - k * Y0;
                        distance = Convert.ToSingle(Math.Abs(a * X + b * Y + c) / Math.Sqrt(a * a + b * b));//船到法线的距离
                        distance *= Math.Sign((X1 * X2 + Y1 * Y2) / (Math.Sqrt(X1 * X1 + Y1 * Y1) * Math.Sqrt(X2 * X2 + Y2 * Y2)));//船到法线的距离带方向，和起点同侧为正；
                        if (distance < 0 )
                        {
                            Points_Copy.RemoveAt(0);
                            USVs[0].Los.pid_x.Clear();
                            USVs[1].Los.pid_x.Clear();
                            USVs[0].Los.pid_y.Clear();
                            USVs[1].Los.pid_y.Clear();
                        }
                        if(Points_Copy.Count<=4)
                        {
                            button3_Click(null, null);//结束实验
                        }
                    }
                }
            }
            int cnt=Expressions.Count();
            if (cnt != 0)//表示使用分段轨迹
            {
                if (Expressions_Copy[0].Max*Math.PI<=track_time*0.2)
                {
                    track_time = Expressions_Copy[0].Min * Math.PI;
                    Expressions_Copy.RemoveAt(0);

                }
                USVs_LOS[0].UpdateExpectedPath(Expressions_Copy[0].X_Expression, Expressions_Copy[0].Y_Expression);
                USVs_LOS[1].UpdateExpectedPath(Expressions_Copy[0].X_Expression, Expressions_Copy[0].Y_Expression);
                USVs_LOS[2].UpdateExpectedPath(Expressions_Copy[0].X_Expression, Expressions_Copy[0].Y_Expression);

            }

            if (radioButton_Trajectory.Checked|radioButton_formation.Checked&&(!radioButton_Point.Checked))
            {
                track_time++;
                DrawExpectedTrack(horizLine11,dt);
            }


        }//定时器运行
        private void Button1_Click(object sender, EventArgs e)//编队开始
        {
            
        }
        private void Button4_Click(object sender, EventArgs e)//起飞
        {

        }
        private void Button6_Click(object sender, EventArgs e)//解锁
        {
            if(((Button)sender).Text=="解锁")
            {
                if (serialPort1.IsOpen == false)//串口未打开
                {
                    SystemInfo("请打开串口！");
                    return;
                }
                ((Button)sender).Text = "加锁";
                msg_Cmd_Write.cmd_id = (byte)MavLink.CMD_TYPE.CMD_UNLOCK;
                mavlinkPacket.Message = msg_Cmd_Write;
                SendMavMsgToRocker(mavlinkPacket);
            }
            else
            {
                if (serialPort1.IsOpen == false)//串口未打开
                {
                    SystemInfo("请打开串口！");
                    return;
                }
                ((Button)sender).Text = "解锁";
                msg_Cmd_Write.cmd_id = (byte)MavLink.CMD_TYPE.CMD_LOCK;
                mavlinkPacket.Message = msg_Cmd_Write;
                SendMavMsgToRocker(mavlinkPacket);
            }
        }
        private void Button5_Click(object sender, EventArgs e)//降落
        {
            if (serialPort1.IsOpen == false)//串口未打开
            {
                SystemInfo("请打开串口！");
                return;
            }
            mavlinkPacket.Message = msg_Cmd_Write;
            SendMavMsgToRocker(mavlinkPacket);
        }
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)//调整选项卡文字方向
        {
            TabControl tc = (TabControl)sender;
            if (tc == tabControl2)//Left
            {
                Color seletedColor = Color.DeepSkyBlue;   //选中项背景色
                Color unSeletedColor = Color.LightYellow;  //未选中项背景色
                Color borderColor = Color.Gray; //边框背景色，自行设置。
                Color backC = Color.White; //标题栏面板背景色
                int itemsWidth = 0; //所有项目宽度
                int itemHeight = 0; //项目高度
                #region 各标题背景颜色设置
                //标签文字填充颜色
                SolidBrush FrontBrush = new SolidBrush(Color.Black);
                StringFormat StringF = new StringFormat();
                //设置文字对齐方式
                StringF.Alignment = StringAlignment.Center;
                StringF.LineAlignment = StringAlignment.Center;
                for (int i = 0; i < tc.TabPages.Count; i++)
                {
                    //标签背景填充颜色
                    SolidBrush BackBrush = null;
                    //获取标签头工作区域
                    Rectangle Rec = tc.GetTabRect(i);
                    itemsWidth = Rec.Width;
                    itemHeight += Rec.Height;
                    if (tc.SelectedTab == tc.TabPages[i])
                    {
                        //绘制标签头背景颜色、边框 ->存在的问题，3D效果默认生成的边框，无法覆盖。
                        BackBrush = new SolidBrush(seletedColor);
                        e.Graphics.FillRectangle(BackBrush, Rec);
                        Pen pen = new Pen(borderColor);
                        e.Graphics.DrawRectangle(pen, Rec);
                        e.Graphics.DrawLine(new Pen(seletedColor), new Point(Rec.X, Rec.Y + Rec.Height), new Point(Rec.X + Rec.Width, Rec.Y + Rec.Height)); //额外画一条线，遮挡选中项的绘图的下边框。
                        e.Graphics.DrawLine(new Pen(backC), new Point(Rec.X, Rec.Y + Rec.Height + 1), new Point(Rec.X + Rec.Width, Rec.Y + Rec.Height + 1)); //额外画一条线，遮挡选中项的控件的下边框。
                    }
                    else
                    {
                        //将标签的颜色重置为  backC - 标题栏面板背景色
                        Rec = new Rectangle(Rec.X, Rec.Y - 2, Rec.Width, Rec.Height + 2);   //-2 是为了将边框颜色覆盖掉。
                        e.Graphics.FillRectangle(new SolidBrush(backC), Rec);
                        BackBrush = new SolidBrush(unSeletedColor);
                        //绘制标签头背景颜色、边框
                        Rec = new Rectangle(Rec.X + 3, Rec.Y, Rec.Width - 6, Rec.Height); //将边框高度降低 4 像素来凸显选中项
                        e.Graphics.FillRectangle(BackBrush, Rec);
                        Pen pen = new Pen(borderColor);
                        e.Graphics.DrawRectangle(pen, Rec);
                    }
                    //绘制标签头文字
                    e.Graphics.DrawString(tc.TabPages[i].Text, tc.Font, FrontBrush, Rec, StringF);
                }
                #endregion
                #region 标题栏面板背景颜色设置
                Rectangle recOfMainBack = tc.ClientRectangle;
                Rectangle newRecOfMainBack = new Rectangle(recOfMainBack.X, recOfMainBack.Y + itemHeight + 3, itemsWidth + 3, recOfMainBack.Height - itemHeight); //获取标题栏空白位置的 Rectangle
                e.Graphics.FillRectangle(new SolidBrush(backC), newRecOfMainBack);
                #endregion
            }
            else if (tc == tabControl3 || tc == tabControl4||tc==tabControl6)//Bottom
            {
                Color seletedColor = Color.DeepSkyBlue;   //选中项背景色
                Color unSeletedColor = Color.LightYellow;  //未选中项背景色
                Color borderColor = Color.Gray; //边框背景色，自行设置。
                Color backC = Color.White; //标题栏面板背景色
                int itemsWidth = 0; //所有项目宽度
                int itemHeight = 0; //项目高度
                #region 各标题背景颜色设置
                //标签文字填充颜色
                SolidBrush FrontBrush = new SolidBrush(Color.Black);
                StringFormat StringF = new StringFormat();
                //设置文字对齐方式
                StringF.Alignment = StringAlignment.Center;
                StringF.LineAlignment = StringAlignment.Center;
                for (int i = 0; i < tc.TabPages.Count; i++)
                {
                    //标签背景填充颜色
                    SolidBrush BackBrush = null;
                    //获取标签头工作区域
                    Rectangle Rec = tc.GetTabRect(i);
                    itemsWidth += Rec.Width;
                    itemHeight = Rec.Height;
                    if (tc.SelectedTab == tc.TabPages[i])
                    {
                        //绘制标签头背景颜色、边框 ->存在的问题，3D效果默认生成的边框，无法覆盖。
                        BackBrush = new SolidBrush(seletedColor);
                        e.Graphics.FillRectangle(BackBrush, Rec);
                        Pen pen = new Pen(borderColor);
                        e.Graphics.DrawRectangle(pen, Rec);
                        e.Graphics.DrawLine(new Pen(seletedColor), new Point(Rec.X, Rec.Y + Rec.Height), new Point(Rec.X + Rec.Width, Rec.Y + Rec.Height)); //额外画一条线，遮挡选中项的绘图的下边框。
                        e.Graphics.DrawLine(new Pen(backC), new Point(Rec.X, Rec.Y + Rec.Height + 1), new Point(Rec.X + Rec.Width, Rec.Y + Rec.Height + 1)); //额外画一条线，遮挡选中项的控件的下边框。
                    }
                    else
                    {
                        //将标签的颜色重置为  backC - 标题栏面板背景色
                        Rec = new Rectangle(Rec.X, Rec.Y - 2, Rec.Width, Rec.Height + 2);   //-2 是为了将边框颜色覆盖掉。
                        e.Graphics.FillRectangle(new SolidBrush(backC), Rec);
                        BackBrush = new SolidBrush(unSeletedColor);
                        //绘制标签头背景颜色、边框
                        Rec = new Rectangle(Rec.X, Rec.Y + 3, Rec.Width, Rec.Height - 6); //将边框高度降低 4 像素来凸显选中项
                        e.Graphics.FillRectangle(BackBrush, Rec);
                        Pen pen = new Pen(borderColor);
                        e.Graphics.DrawRectangle(pen, Rec);
                    }
                    //绘制标签头文字
                    e.Graphics.DrawString(tc.TabPages[i].Text, tc.Font, FrontBrush, Rec, StringF);
                }
                #endregion

                #region 标题栏面板背景颜色设置
                Rectangle recOfMainBack = tc.ClientRectangle;
                Rectangle newRecOfMainBack = new Rectangle(recOfMainBack.X + itemsWidth + 3, recOfMainBack.Y + recOfMainBack.Height - itemHeight, recOfMainBack.Width - itemsWidth + 3, itemHeight); //获取标题栏空白位置的 Rectangle
                e.Graphics.FillRectangle(new SolidBrush(backC), newRecOfMainBack);
                #endregion
            }
        }
        private void VirtualLeaderSelectMode(object sender, EventArgs e)
        {
            if (RB_ProgramControlLeader.Checked)//程序控制
            {
                groupBox5.Enabled = true;
                groupBox8.Enabled = false;
            }
            else if(RB_RockerControlLeader.Checked)//摇杆控制
            {
                groupBox5.Enabled = false;
                groupBox8.Enabled = true;
            }
        }
        private void Tchart4_DataSave_Click(object sender, EventArgs e)//tChart4保存数据
        {
            //tChart4.Export.Data.Excel.IncludeIndex = true;
            tChart4.Export.Data.Excel.IncludeHeader = true;
            tChart4.Export.Data.Excel.IncludeLabels = true;

            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/tChart4/";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);//不存在就创建目录
            }
            tChart4.Export.Data.Excel.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            tChart4.Export.Template.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ten");
            SystemInfo("保存成功！");
        }
        private void OpenTchart4DataSave_Click(object sender, EventArgs e)//打开tChart4保存目录
        {
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/tChart4/";
            Process.Start(FilePath);
        }
        private void Button9_Click(object sender, EventArgs e)//tChart1保存数据
        {
            //tChart1.Export.Data.Excel.IncludeHeader = true;
            //tChart1.Export.Data.Excel.IncludeLabels = true;
            tChart1.Export.Data.Excel.IncludeSeriesTitle = true;
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/tChart1/";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);//不存在就创建目录
            }
            tChart1.Export.Data.Excel.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            tChart1.Export.Template.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ten");
            SystemInfo("保存成功！");
        }
        private void Button10_Click(object sender, EventArgs e)//tchart1打开目录
        {
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/tChart1/";
            Process.Start(FilePath);
        }
        private void Button11_Click(object sender, EventArgs e)//读取数据
        {

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tChart5.Import.Template.Load(ofd.FileName);
                tChart5.BringToFront();
                button12.BringToFront();
            }
        } 
        private void Button12_Click(object sender, EventArgs e)//关闭数据
        {
            tChart5.SendToBack();
            button12.SendToBack();
        }
        public void MCT84Bl2xy(double l, double B, out double xc, out double yc)//经纬度转换成xy坐标
        {
            try
            {
                l = l * Math.PI / 180;
                B = B * Math.PI / 180;

                double B0 = 30 * Math.PI / 180;

                double N = 0, e = 0, a = 0, b = 0, e2 = 0, K = 0;
                a = 6378137;
                b = 6356752.3142;
                e = Math.Sqrt(1 - (b / a) * (b / a));
                e2 = Math.Sqrt((a / b) * (a / b) - 1);
                double CosB0 = Math.Cos(B0);
                N = (a * a / b) / Math.Sqrt(1 + e2 * e2 * CosB0 * CosB0);
                K = N * CosB0;

                double Pi = Math.PI;
                double SinB = Math.Sin(B);

                double tan = Math.Tan(Pi / 4 + B / 2);
                double E2 = Math.Pow((1 - e * SinB) / (1 + e * SinB), e / 2);
                double xx = tan * E2;

                xc = K * Math.Log(xx);
                yc = K * l;
                return;
            }
            catch (Exception ErrInfo)
            {
            }
            xc = -1;
            yc = -1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double course;
            double beta = 0 * Math.PI / 180;
            double x0 = 0;
            double y0 = 0;
            if (button3.Text == "开始实验")
            {
                button3.Text = "结束实验";

                timer2.Enabled = true;

                textBox10.Enabled = false;
                textBox11.Enabled = false;
                textBox12.Enabled = false;
                textBox13.Enabled = false;

                groupBox16.Enabled = false;
                groupBox17.Enabled = false;
                groupBox19.Enabled = false;


                horizLine12.Clear();
                horizLine13.Clear();
                horizLine14.Clear();

                USVs_LOS[0].LOS_Clear();
                USVs_LOS[1].LOS_Clear();
                USVs_LOS[2].LOS_Clear();
                USVs[0].norbbin.Clear(0, 0);
                USVs[1].norbbin.Clear(0, 0);
                USVs[2].norbbin.Clear(0, 0);
                timer2.Enabled = true;

                //Expressions_Copy= (List<Expression>)DeepCopy.deepcopy(Expressions);
                
                Points.Add(new PointF(20, 30));
                Points.Add(new PointF(25, 35));
                Points.Add(new PointF(30, 60));
                Points.Add(new PointF(35, 70));
                Points.Add(new PointF(50, 100));
                Points_Copy = (List<PointF>)DeepCopy.deepcopy<List<PointF>>(Points);

                if (radioButton_Real_USV.Checked)//实船
                {
                    if (serialPort1.IsOpen == false)//串口未打开
                    {
                        SystemInfo("请打开串口！");
                        track_time = 0;
                        return;
                    }

                    try
                    {
                        USVs[0].UNLOCK();
                        USVs[1].UNLOCK();
                        USVs[2].UNLOCK();
                    }
                    catch
                    { 
                    
                    }

                }

                if (radioButton_Single_USV.Checked)//单船
                {
                    if (radioButton_Trace.Checked)//路径
                    {
                        DrawExpectedPath(horizLine11);
                    }
                    else if(radioButton_Trajectory.Checked)//轨迹
                    {
                        horizLine11.Clear();
                    }
                }
                else if (radioButton_formation.Checked)//编队
                {
                    horizLine11.Clear();
                 
                }

            }
            else if (button3.Text == "结束实验")
            {
                button3.Text = "开始实验";
                textBox10.Enabled = true;
                textBox11.Enabled = true;
                textBox12.Enabled = true;
                textBox13.Enabled = true;

                groupBox16.Enabled = true;
                groupBox17.Enabled = true;
                groupBox19.Enabled = true;

                track_time = 0;
                timer2.Enabled = false;
                Points_Copy.Clear();
                Points.Clear();

                if (radioButton_Real_USV.Enabled)
                {

                    try
                    {
                        USVs[0].LOCK();
                        USVs[1].LOCK();
                        USVs[2].LOCK();
                    }
                    catch
                    {

                    }
                }
                

            }


        }//开始实验
        private void DrawExpectedTrack(HorizLine horizLine,double T)
        {
            double x, y;
            if (Expressions.Count() == 0)
            {
                x = Eval.Calculate(track_time * T, "var x=" + textBox13.Text + ";");
                y = Eval.Calculate(track_time * T, "var y=" + textBox12.Text + ";");
                horizLine.Add(x, y);
            }
            else
            {
                string X_Expression, Y_Expression;
                foreach (Expression item in Expressions)
                {
                    X_Expression = item.Out_X_Expression(track_time * T);
                    Y_Expression = item.Out_Y_Expression(track_time * T);
                    if (X_Expression != null && Y_Expression != null)
                    {
                        x = Eval.Calculate(track_time * T ,"var x=" + X_Expression + ";");
                        y = Eval.Calculate(track_time * T , "var y=" + Y_Expression + ";");
                        horizLine.Add(x, y);
                        break;
                    }
                }
             
            }

        }
        private void DrawExpectedPath(HorizLine horizLine)//绘制期望路径
        {
            
            double min, max, pionts;
            pionts = 500;//绘制点数
            horizLine.Clear();
            if (Expressions.Count() == 0)
            {
                try
                {
                    min = Convert.ToDouble(textBox11.Text) * Math.PI;
                    max = Convert.ToDouble(textBox10.Text) * Math.PI;
                }
                catch
                {
                    MessageBox.Show("w的取值范围输入错误！");
                    return;
                }
                for (int i = 0; i < pionts; i++)
                {
                    double x, y;
                    x = Eval.Calculate(min  + i * (max - min)  / pionts, "var x=" + textBox13.Text + ";");
                    y = Eval.Calculate(min  + i * (max - min) / pionts, "var y=" + textBox12.Text + ";");
                    horizLine.Add(x, y);
                }

            }
            else
            {
                double x, y;
                string X_Expression,Y_Expression;
                foreach (Expression item in Expressions)
                {
                    try
                    {
                        min = Convert.ToDouble(item.Min)*Math.PI;
                        max = Convert.ToDouble(item.Max)*Math.PI;
                    }
                    catch
                    {
                        MessageBox.Show("w的取值范围输入错误！");
                        return;
                    }
                    for (int i = 0; i < pionts; i++)
                    {
                        X_Expression = item.X_Expression;
                        Y_Expression = item.Y_Expression;
                        //X_Expression = item.Out_X_Expression(min * Math.PI + i * (max - min) * Math.PI / pionts);
                       // Y_Expression = item.Out_Y_Expression(min * Math.PI + i * (max - min) * Math.PI / pionts);
                        if (X_Expression != null && Y_Expression != null)
                        {
                            x = Eval.Calculate(min  + i * (max - min) / pionts, "var x=" + X_Expression + ";");
                            y = Eval.Calculate(min + i * (max - min) / pionts, "var y=" + Y_Expression + ";");
                            horizLine.Add(x, y);
                        }
                    }
                }
            }
           
        }
        private void UpdateExpectedPath(object sender, EventArgs e)//更新期望路径
        {

            USVs_LOS[0].UpdateExpectedPath(textBox13.Text, textBox12.Text);
            USVs_LOS[1].UpdateExpectedPath(textBox13.Text, textBox12.Text);
            USVs_LOS[2].UpdateExpectedPath(textBox13.Text, textBox12.Text);

            USVs_LOS[0].UpadataParam(Convert.ToDouble(textBox_USV1_kp.Text), Convert.ToDouble(textBox_USV1_delta.Text));
            USVs_LOS[1].UpadataParam(Convert.ToDouble(textBox_USV2_kp.Text), Convert.ToDouble(textBox_USV2_delta.Text));
            USVs_LOS[2].UpadataParam(Convert.ToDouble(textBox_USV3_kp.Text), Convert.ToDouble(textBox_USV3_delta.Text));

            horizLine11.Clear();
            track_time = 0;
            if (radioButton_Trajectory.Checked)
            {
                DrawExpectedPath(horizLine11);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            X_Standard += System.Convert.ToDouble(textBox_X.Text);
            Y_Standard += System.Convert.ToDouble(textBox_Y.Text);
        }
        private void LOS_UpdateParam(object sender, EventArgs e)//LOS参数更新
        {
            USVs_LOS[0].UpadataParam(Convert.ToDouble(textBox_USV1_kp.Text), Convert.ToDouble(textBox_USV1_delta.Text));
            USVs_LOS[1].UpadataParam(Convert.ToDouble(textBox_USV2_kp.Text), Convert.ToDouble(textBox_USV2_delta.Text));
            USVs_LOS[2].UpadataParam(Convert.ToDouble(textBox_USV3_kp.Text), Convert.ToDouble(textBox_USV3_delta.Text));

        }
        private void Point_PID_Update(object sender, EventArgs e)//LOS参数更新
        {
            USVs_LOS[0].pid_x.UpdateParam(Convert.ToDouble(textBox_USV1_X_Kp.Text), Convert.ToDouble(textBox_USV1_X_Ki.Text), Convert.ToDouble(textBox_USV1_X_Kd.Text),0);
            USVs_LOS[0].pid_y.UpdateParam(Convert.ToDouble(textBox_USV1_Y_Kp.Text), Convert.ToDouble(textBox_USV1_Y_Ki.Text), Convert.ToDouble(textBox_USV1_Y_Kd.Text), 0);

            USVs_LOS[1].pid_x.UpdateParam(Convert.ToDouble(textBox_USV2_X_Kp.Text), Convert.ToDouble(textBox_USV2_X_Ki.Text), Convert.ToDouble(textBox_USV2_X_Kd.Text), 0);
            USVs_LOS[1].pid_y.UpdateParam(Convert.ToDouble(textBox_USV2_Y_Kp.Text), Convert.ToDouble(textBox_USV2_Y_Ki.Text), Convert.ToDouble(textBox_USV2_Y_Kd.Text), 0);

            USVs_LOS[2].pid_x.UpdateParam(Convert.ToDouble(textBox_USV3_X_Kp.Text), Convert.ToDouble(textBox_USV3_X_Ki.Text), Convert.ToDouble(textBox_USV3_X_Kd.Text), 0);
            USVs_LOS[2].pid_y.UpdateParam(Convert.ToDouble(textBox_USV3_Y_Kp.Text), Convert.ToDouble(textBox_USV3_Y_Ki.Text), Convert.ToDouble(textBox_USV3_Y_Kd.Text), 0);

        }

        private void datastart_Click(object sender, EventArgs e)
        {
            if (datastart.Text == "开始上传")
            {

                datastart.Text = "停止上传";
                timer4.Enabled = true;

            }
            else {

                datastart.Text = "开始上传";
                timer4.Enabled = false;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            tcpserver.back();
        }

        private void savedata_Click_1(object sender, EventArgs e)
        {
            tChart7.Export.Data.Excel.IncludeHeader = true;
            tChart7.Export.Data.Excel.IncludeIndex = true;
            tChart7.Export.Data.Excel.IncludeLabels = true;
            tChart7.Export.Data.Excel.IncludeSeriesTitle = true;
            tChart7.Export.Data.Excel.IndexFieldName = "索引";

            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/USV_0/";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);//不存在就创建目录
            }
            tChart7.Export.Data.Excel.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            // tChart7.Export.Template.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ten");

            tChart9.Export.Data.Excel.IncludeHeader = true;
            tChart9.Export.Data.Excel.IncludeIndex = true;
            tChart9.Export.Data.Excel.IncludeLabels = true;
            tChart9.Export.Data.Excel.IncludeSeriesTitle = true;
            tChart9.Export.Data.Excel.IndexFieldName = "索引";

            string FilePath1 = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/USV_1/";
            if (!Directory.Exists(FilePath1))
            {
                Directory.CreateDirectory(FilePath1);//不存在就创建目录
            }
            tChart9.Export.Data.Excel.Save(FilePath1 + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            // tChart7.Export.Template.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ten");

            tChart11.Export.Data.Excel.IncludeHeader = true;
            tChart11.Export.Data.Excel.IncludeIndex = true;
            tChart11.Export.Data.Excel.IncludeLabels = true;
            tChart11.Export.Data.Excel.IncludeSeriesTitle = true;
            tChart11.Export.Data.Excel.IndexFieldName = "索引";

            string FilePath2 = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/USV_2/";
            if (!Directory.Exists(FilePath2))
            {
                Directory.CreateDirectory(FilePath2);//不存在就创建目2
            }
            tChart11.Export.Data.Excel.Save(FilePath2 + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            // tChart7.Export.Template.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ten");

            tChart8.Export.Data.Excel.IncludeHeader = true;
            tChart8.Export.Data.Excel.IncludeIndex = true;
            tChart8.Export.Data.Excel.IncludeLabels = true;
            tChart8.Export.Data.Excel.IncludeSeriesTitle = true;
            tChart8.Export.Data.Excel.IndexFieldName = "索引";

            string FilePath3 = AppDomain.CurrentDomain.BaseDirectory + "/实验数据/USV Trak/";
            if (!Directory.Exists(FilePath3))
            {
                Directory.CreateDirectory(FilePath3);//不存在就创建目2
            }
            tChart8.Export.Data.Excel.Save(FilePath3 + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            // tChart7.Export.Template.Save(FilePath + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ten");


            SystemInfo("保存成功！");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            tcpserver._Click();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            tcpserver.drawtrack();
        }

        private void btn_clear_t6_Click(object sender, EventArgs e)
        {
            Expressions.Clear();
            horizLine10.Clear();
            richTextBox_EXpression.Clear();
        }//清除多轨迹

        private void key_press(object sender, KeyPressEventArgs e)//回车事件 让texbox1获取焦点 其他控件失去焦点
        {
            if (e.KeyChar == ((char)Keys.Enter))
            {
                textBox1.Visible = true;
                textBox1.Focus();
                textBox1.Visible = false;
            }
        
        }

        private void Rocker_Switch(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            USVs[0].LOCK();
            USVs[1].LOCK();
            USVs[2].LOCK();
            if (rb.Name.Contains("USV1"))
            {
                USVs[0].UNLOCK();
            }
            else if (rb.Name.Contains("USV2"))
            {
                USVs[1].UNLOCK();
            }
            else if (rb.Name.Contains("USV3"))
            {
                USVs[2].UNLOCK();
            }


        }//单个船解锁
        private void UPdate_Expression()
        {   uint i = 0;
            richTextBox_EXpression.Clear();
            richTextBox_EXpression.AppendText("当前轨迹表达式：\r\n");
            foreach (var item in Expressions)
            {
                richTextBox_EXpression.AppendText("编号:" + i++.ToString()+"      W=："+item.Min.ToString()+'-'+item.Max.ToString()+"\r\n");
                richTextBox_EXpression.AppendText("X=" + item.X_Expression+"\r\n");
                richTextBox_EXpression.AppendText("Y=" + item.Y_Expression+"\r\n");
                richTextBox_EXpression.AppendText("\r\n");
            }
        }//更新多轨迹
        private void button_ADDEXP_Click(object sender, EventArgs e)
        {
            if (Expressions.Count > Convert.ToInt16(textBox_EXP_ID.Text))
            {
                Expressions[Convert.ToInt16(textBox_EXP_ID.Text)].change(textBox13.Text, textBox12.Text, Convert.ToDouble(textBox11.Text), Convert.ToDouble(textBox10.Text));
            }
            else 
            {
                Expressions.Add(new Expression(textBox13.Text, textBox12.Text, Convert.ToDouble(textBox11.Text), Convert.ToDouble(textBox10.Text)));
            }

            textBox_EXP_ID.Text= (Convert.ToInt16(textBox_EXP_ID.Text)+1).ToString();
            textBox11.Text = textBox10.Text;
            UPdate_Expression();
        }//添加多轨迹
    }
}
