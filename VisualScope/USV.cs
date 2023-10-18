using MavLink;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Themes;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;
using static Steema.TeeChart.Styles.SeriesMarks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static 地面站.Form1;
using static 地面站.LOS;

namespace 地面站
{
    public class USV_State_Info
    {
        public Label ID;
        public Label VEL;
        public Label VOL;
        public Label X;
        public Label Y;
        public Label HEADING;
        public Label LOS_VEL;
        public Label LOS_HEA;
        public TextBox L;
        public TextBox angle;
        public TextBox kp;
        public TextBox delta;
        public USV_State_Info(Label ID_, Label VEL_, Label VOL_, Label X_, Label Y_, Label HEADING_,Label LOS_VEL_,Label LOS_HEA_, TextBox L_,TextBox angle_, TextBox kp_, TextBox delta_)
        {
            ID = ID_;
            VEL = VEL_;
            VOL = VOL_;
            X = X_;
            Y = Y_;
            L = L_;
            HEADING = HEADING_;
            LOS_VEL = LOS_VEL_;
            LOS_HEA = LOS_HEA_;
            angle = angle_;
            kp = kp_;
            delta = delta_;
            
        }
        
    }
    public class USV_Point_PID_INFO
    {
        public TextBox X_Kp;
        public TextBox X_Ki;
        public TextBox X_Kd;
        public TextBox Y_Kp;
        public TextBox Y_Ki;
        public TextBox Y_Kd;

        public USV_Point_PID_INFO(TextBox X_Kp_, TextBox X_Ki_, TextBox X_Kd_, TextBox Y_Kp_, TextBox Y_Ki_, TextBox Y_Kd_)
        { 
            X_Kp = X_Kp_;
            X_Ki = X_Ki_;
            X_Kd = X_Kd_;
            Y_Kp = Y_Kp_;
            Y_Ki = Y_Ki_;
            Y_Kd = Y_Kd_;
        }

    }
    public class USV_PID_Info
    {
        public TextBox VEL_Kp;
        public TextBox VEL_Ki;
        public TextBox VEL_Kd;
        public TextBox HEA_Kp;
        public TextBox HEA_Ki;
        public TextBox HEA_Kd;
        public int index=0;
        public float[] PID_Value = new float[10];
        public TextBox[] textBoxes = new TextBox[10];

        
        public USV_PID_Info(TextBox VEL_Kp_, TextBox VEL_Ki_, TextBox VEL_Kd_, TextBox HEA_Kp_, TextBox HEA_Ki_, TextBox HEA_Kd_)
        {
            VEL_Kp= VEL_Kp_;
            VEL_Ki= VEL_Ki_;
            VEL_Kd= VEL_Kd_;
            HEA_Kp= HEA_Kp_;
            HEA_Ki= HEA_Ki_;
            HEA_Kd= HEA_Kd_;
            textBoxes[0] = VEL_Kp_;
            textBoxes[1] = VEL_Ki_;
            textBoxes[2] = VEL_Kd_;
            textBoxes[3] = HEA_Kp_;
            textBoxes[4] = HEA_Ki_;
            textBoxes[5] = HEA_Kd_;

        }
        public void USV_PID_Value_Recive(Msg_param_read_ack Param)
        {
            PID_Value[Param.param_id-1]= Param.value;
            index++;
        }
        public void USV_PID_Info_Display()
        {
            if (index <= 6)
                return;
            VEL_Kp.Text = PID_Value[0].ToString("0.0");
            VEL_Ki.Text = PID_Value[1].ToString("0.0");
            VEL_Kd.Text = PID_Value[2].ToString("0.0");
            HEA_Kp.Text = PID_Value[3].ToString("0.0");
            HEA_Ki.Text = PID_Value[4].ToString("0.0");
            HEA_Kd.Text = PID_Value[5].ToString("0.0");
            index = 0;

        }

    }
    
    public class USV
    {
        private readonly Form1 form1;

        public byte DEV_ID;
        public bool param_read_flag=false;
        public Msg_usv_state state = new Msg_usv_state();
        public USV_Position Position = new USV_Position(0, 0);
        public Msg_usv_set set = new Msg_usv_set();
        public Msg_cmd_write Msg_cmd=new Msg_cmd_write();
        public Msg_param_read Msg_param = new Msg_param_read();
        public Msg_param_write param_write=new Msg_param_write();
        public LOS Los;
        HorizLine horizLine;
        public MavlinkPacket mavlinkPacket;
        LOS.Result result;
        //public Norbbin norbbin = new Norbbin(0, 0);
        public CyberShip_ii_MMG mmg = new CyberShip_ii_MMG();
        USV_State_Info usv_state_info;
        public USV_PID_Info usv_PID_info;
        public USV_Point_PID_INFO usv_Point_PID_info;
        public double speed_exp;

        public USV(object sender, byte DEV_ID_)
        {
            form1 = (Form1)sender;
            DEV_ID = DEV_ID_;
        }
        public void Clear(double x0, double y0,double psi0)
        {
            mmg.Clear(x0, y0,psi0);
        }
        public void Init(byte DEV_ID_)
        {
            DEV_ID = DEV_ID_;
            Read_Param();
        }
        public void Init(HorizLine horizline_)
        {
            horizLine = horizline_;
        }
        public void Init(LOS los_)
        {
            Los = los_;
        }
        public void Init(MavlinkPacket mavlinkPacket_)
        {
            mavlinkPacket = mavlinkPacket_;
            mavlinkPacket.SystemId = (int)MavLink.SYS_TYPE.SYS_GSTATION;
            mavlinkPacket.ComponentId = DEV_ID;
        }
        public void Init(USV_State_Info usv_state_info_)
        {
            usv_state_info = usv_state_info_;
        }
        public void Init(USV_PID_Info usv_PID_info_)
        { 
        usv_PID_info = usv_PID_info_;
        }
        public void Init(USV_Point_PID_INFO uSV_Point_PID_INFO)
        {
            usv_Point_PID_info = uSV_Point_PID_INFO;
        }

        public void UNLOCK()
        {
            form1.mavlinkPacket.ComponentId = DEV_ID;
           // Msg_cmd.DEV_ID = DEV_ID;
            Msg_cmd.cmd_id = (byte)MavLink.CMD_TYPE.CMD_UNLOCK;
            form1.mavlinkPacket.Message = Msg_cmd;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);

        }
        public void LOCK_EVENT(object sender, EventArgs e)
        {
            Button bt = sender as Button;
            if (bt.Text == "解锁")
            {
                UNLOCK();
                bt.Text = "锁定";
            }
            else if (bt.Text == "锁定")
            { 
                LOCK();
                bt.Text = "解锁";
            }
        
        }

        public void LOCK()
        {
            //Msg_cmd.DEV_ID = DEV_ID;
            set.Speed = 0;
            form1.mavlinkPacket.Message = set;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);

            form1.mavlinkPacket.ComponentId = DEV_ID;
            Msg_cmd.cmd_id = (byte)MavLink.CMD_TYPE.CMD_LOCK;
            form1.mavlinkPacket.Message = Msg_cmd;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);
            Auto_sail();

        }
        public void Auto_sail()
        {
            Msg_cmd.DEV_ID = DEV_ID;
            Msg_cmd.cmd_id = (byte)MavLink.CMD_TYPE.CMD_AUTO_DRIVE;
            form1.mavlinkPacket.Message = Msg_cmd;
            form1.mavlinkPacket.ComponentId = DEV_ID;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);

        }
        public void Read_Param(object sender, EventArgs e)
        {
            if (param_read_flag == true)
                return;
            if (!form1.serialPort1.IsOpen)
                form1.SystemInfo("请打开串口!");
            else
            {
                
                Msg_param.param_id = (byte)MavLink.PARAM_TYPE.PARAM_LIST_REQUEST;
                form1.mavlinkPacket.ComponentId = DEV_ID;
                form1.mavlinkPacket.Message= Msg_param;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);
                form1.SystemInfo("参数读取中..");
                
            }
        }
        public void Read_Param()
        {
            if (param_read_flag == true)
                return;
            if (!form1.serialPort1.IsOpen)
                form1.SystemInfo("请打开串口!");
            else
            {

                Msg_param.param_id = (byte)MavLink.PARAM_TYPE.PARAM_LIST_REQUEST;
                form1.mavlinkPacket.ComponentId = DEV_ID;
                form1.mavlinkPacket.Message = Msg_param;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);
                form1.SystemInfo("参数读取中..");

            }
        }


        public void PID_Updata(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            form1.mavlinkPacket.ComponentId = DEV_ID;
            param_write.value = System.Convert.ToSingle(tb.Text);
            param_write.param_id= (Byte)(Array.IndexOf(usv_PID_info.textBoxes, tb)+1);

            form1.mavlinkPacket.Message = param_write;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);

        }


        public void ll2XY()
        {
            double X;
            double Y;
            form1.MCT84Bl2xy(state.longitude, state.latitude, out X, out Y);
            Position.X = X - form1.X_Standard;
            Position.Y = Y - form1.Y_Standard;
        }
        public void Draw_Line()
        {
            try
            {
                horizLine.Add(Position.X, Position.Y);
            }
            catch
            {
                form1.SystemInfo("输出坐标为非法值！已结束本次实验！");
                form1.button3_Click(null,null);
            }
        }

        public void USV_Info_Display()
        {
            usv_state_info.ID.Text= DEV_ID.ToString();
            usv_state_info.VEL.Text = state.speed.ToString("0.00");
            usv_state_info.VOL.Text = state.battery_voltage.ToString("0.00");
            usv_state_info.HEADING.Text = state.heading.ToString("0.00");
            usv_state_info.LOS_VEL.Text = set.Speed.ToString("0.00");
            usv_state_info.LOS_HEA.Text = set.Heading.ToString("0.00");
            usv_state_info.X.Text = Position.X.ToString("0.00");
            usv_state_info.Y.Text = Position.Y.ToString("0.00");
        }

        public void Point_VirtualLeader(double T)
        {
            double U;
            double beta=0*Math.PI/180;//漂角

            double heading;
            double course;
            double tau_r;//仿真mmg模型 pid输出

            heading = mmg.state.psi;
            course = mmg.state.course;
            U = speed_exp;

            Los.UpdateData(Position.X, Position.Y, heading, course, U);
            result = Los.Calculate_Point_Leader(T);

            double err;
            err = result.psi_d - heading;
            while (err >= Math.PI)
                err -= Math.PI * 2;
            while (err < -Math.PI)
                err += Math.PI * 2;
            heading = result.psi_d - err;
            tau_r = Los.pid_r.Calculate(result.psi_d, heading, T);
            mmg.Calculate_Psi( U,beta,tau_r, T);

            Position.X = mmg.state.x;
            Position.Y = mmg.state.y;
            Draw_Line();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="T"></param>
        /// <param name="x_l">领航者X坐标</param>
        /// <param name="y_l">领航者Y坐标</param>
        /// <param name="psi_l">领航者艏向角</param>
        public void LOS_Point_Follower(double T,double x_l,double y_l,double psi_l,double Vel_L)
        {
            double beta;//漂角
            double u;//船速
            double x, y;
            double heading;
            double course;
            double speed;
            double tau_u, tau_r;//仿真mmg模型 pid输出

            USV_Info_Display();
            if (form1.radioButton_Simulation.Checked)//仿真
            {
                heading = mmg.state.psi;
                course = mmg.state.course;
                speed = mmg.state.U;
                Los.UpdateData(Position.X, Position.Y, heading, course, speed);
                result = Los.Calculate_Point_Follower(x_l,y_l,psi_l,T, 
                                        Convert.ToDouble(usv_state_info.L.Text), 
                                        Convert.ToDouble(usv_state_info.angle.Text),
                                        Vel_L
                                        );
                tau_u = Los.pid_u.Calculate(result.vel, speed, T);
                double err;
                err = result.psi_d - heading;
                while (err >= Math.PI)
                    err -= Math.PI * 2;
                while (err < -Math.PI)
                    err += Math.PI * 2;
                heading = result.psi_d - err;
                tau_r = Los.pid_r.Calculate(result.psi_d, heading, T);
                mmg.Calculate(tau_u, tau_r, T);

                Position.X = mmg.state.x;
                Position.Y = mmg.state.y;
                state.speed = Convert.ToSingle(mmg.state.U);
                state.heading = Convert.ToSingle(mmg.state.psi * 180 / Math.PI);
                state.battery_voltage = Convert.ToSingle(err * 180 / Math.PI);
                Draw_Line();

            }
            else if (form1.radioButton_Real_USV.Checked)//实船
            {
                ll2XY();

                heading = state.heading / 180 * Math.PI;//弧度
                u = state.speed;
                course = state.Track / 180 * Math.PI;//弧度

                Draw_Line();
                Los.UpdateData(Position.X, Position.Y, heading, course, u);

                result = Los.Calculate_Point_Follower(x_l, y_l, psi_l, T,
                                        Convert.ToDouble(usv_state_info.L.Text),
                                        Convert.ToDouble(usv_state_info.angle.Text),
                                        Vel_L );
                result.psi_d = result.psi_d * 180 / Math.PI;
                set.Heading = (float)result.psi_d;
                set.Speed = (float)result.vel;
                set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
                set.DEV_ID = DEV_ID;

                form1.mavlinkPacket.ComponentId = DEV_ID;
                form1.mavlinkPacket.Message = set;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);
                form1.mavlinkProxy.TChart1Display("usv" + DEV_ID.ToString() + ".set.speed", set.Speed);//添加到波形显示
                form1.mavlinkProxy.TChart1Display("usv" + DEV_ID.ToString() + ".set.heading", set.Heading);//添加到波形显示


            }
        }
        public void LOS_Follower(double T)
        {
            double beta;//漂角
            double u;//船速
            double x, y;
            double heading;
            double course;

            USV_Info_Display();



            if (form1.radioButton_Simulation.Checked)//仿真
            {
                beta = 0.1;
                heading = Los.psi;
                course = heading + beta;

                Position.X = Los.x;
                Position.Y = Los.y;

                Los.UpdateData(Position.X, Position.Y, heading, course, result.vel);
                result = Los.Caculate_Follower(T, Convert.ToDouble(usv_state_info.L.Text), Convert.ToDouble(usv_state_info.angle.Text));
                state.speed = (float)result.vel;
                Los.UpdateSimulationPosition(result.psi_d, beta, T);//更新 x y 的值
                Draw_Line();

            }
            else if(form1.radioButton_Real_USV.Checked)//实船
            {
                ll2XY();

                heading = state.heading / 180 * Math.PI;//弧度
                u = state.speed;
                course = state.Track / 180 * Math.PI;//弧度

                Draw_Line();
                Los.UpdateData(Position.X , Position.Y , heading, course, u);

                result = Los.Caculate_Follower(T, Convert.ToDouble(usv_state_info.L.Text), Convert.ToDouble(usv_state_info.angle.Text));
                result.psi_d = result.psi_d * 180 / Math.PI;
                set.Heading = (float)result.psi_d;
                set.Speed = (float)result.vel;
                set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
                set.DEV_ID = DEV_ID;

                form1.mavlinkPacket.ComponentId = DEV_ID;
                form1.mavlinkPacket.Message = set;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);
                form1.mavlinkProxy.TChart1Display("usv" + DEV_ID.ToString() + ".set.speed", set.Speed);//添加到波形显示
                form1.mavlinkProxy.TChart1Display("usv" + DEV_ID.ToString() + ".set.heading", set.Heading);//添加到波形显示
            }

        }
     
        public void LOS_Control(double T)
        {
            double heading_set;//设定艏向角
            double rudder_set;//设定舵角
            double beta;//漂角
            double u;//船速
            double x, y;
            double heading;
            double rudder;
            double course;

            USV_Info_Display();

            
            if (form1.radioButton_Simulation.Checked)//仿真
            {

                //Position.X = norbbin.state.x;
                //Position.Y = norbbin.state.y;

                beta = 0.1;
                heading = state.heading ;
                while (heading > Math.PI)
                    heading -= 2 * Math.PI;
                while (heading < -Math.PI)
                    heading += 2 * Math.PI;
                course = heading + beta;
                double delta_r;
                if (form1.radioButton_Trajectory.Checked)//轨迹
                {

                    u = result.vel;
                    Los.UpdateData(Position.X, Position.Y, heading, course, u);
                    result = Los.Calculate_trajectory(T);
                    //double err = (result.psi_d - norbbin.state.psi);
                    //while (err > Math.PI)
                    //    err -= 2 * Math.PI;
                    //while (err < -Math.PI)
                    //    err += 2 * Math.PI;
                    //delta_r = 30 * err;
                    //norbbin.UpdateData(delta_r, beta, u);
                    //norbbin.Calculate(T);
                    Los.UpdateData(Position.X, Position.Y, heading, course, result.vel);
                    Los.UpdateSimulationPosition(result.psi_d, beta, T);//更新 x y 的值

                    state.speed = (float)result.vel;
                }
                else if (form1.radioButton_Trace.Checked)//路径
                {
                    Los.UpdateData(Position.X, Position.Y, heading, course, Convert.ToSingle(form1.textBox_Speed_Set.Text));
                    heading_set = Los.Calculate(T);
                    //double err = (heading_set-norbbin.state.psi);
                    //while(err>Math.PI)
                    //    err -= 2 * Math.PI;
                    //while (err <- Math.PI)
                    //    err += 2 * Math.PI;
                    //delta_r = 80 * err;
                    //norbbin.UpdateData(delta_r, beta, Convert.ToSingle(form1.textBox_Speed_Set.Text));
                    //norbbin.Calculate(T);
                    Los.UpdateSimulationPosition(heading_set, beta, T);
                    state.speed = Convert.ToSingle(form1.textBox_Speed_Set.Text);
                }


                Draw_Line();

            }
            else if (form1.radioButton_Real_USV.Checked)//实船实验
            {
                ll2XY();
                heading = state.heading / 180 * Math.PI;//弧度
                u = state.speed;
                course = state.Track / 180 * Math.PI;//弧度


                if (form1.radioButton_Trajectory.Checked)//轨迹
                {
                    u = state.speed;

                    Los.UpdateData(Position.X, Position.Y, heading, course, u);
                    result = Los.Calculate_trajectory(T);
                    set.Heading = Convert.ToSingle( result.psi_d*180/Math.PI);
                    while (set.Heading > 180)
                        set.Heading -= 360;
                    while (set.Heading < -180)
                        set.Heading += 360;
                    set.Speed = (float)result.vel;

                }
                else if (form1.radioButton_Trace.Checked)//路径
                {
                    Los.UpdateData(Position.X, Position.Y, heading, course, Convert.ToSingle(form1.textBox_Speed_Set.Text));
                    heading_set =  Los.Calculate(T);
                    heading_set = heading_set * 180 / Math.PI;
                    set.Heading = (float)heading_set;
                    set.Speed = Convert.ToSingle(form1.textBox_Speed_Set.Text);
                }

                Draw_Line();
                //Los.UpdateData(Position.X - form1.X_Standard, Position.Y - form1.Y_Standard, heading, course, u);

                set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
                set.DEV_ID = DEV_ID;

                form1.mavlinkPacket.ComponentId = DEV_ID;
                form1.mavlinkPacket.Message = set;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);
                form1.mavlinkProxy.TChart1Display("usv" + DEV_ID.ToString() + ".set.speed", set.Speed);//添加到波形显示
                form1.mavlinkProxy.TChart1Display("usv" + DEV_ID.ToString() + ".set.heading", set.Heading);//添加到波形显示

            }

        }
    }
}
