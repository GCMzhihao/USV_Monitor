using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Timers;
using MavLink;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using Steema.TeeChart.Styles;
using System.Windows.Markup;
using System.Collections;
using System.Xml.Linq;
using static 地面站.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Linq.Expressions;

namespace 地面站
{
    class TCP
    {
        int?[] IDJ = new int?[3] { null, null, null };
        double X_Standard;
        double Y_Standard;
        public double latitude;
        public double longitude;
        public double speed;
        public double heading;
        public double voltage;
        private TCP tcp;
        public Mavlink mavlink_DTU = new Mavlink();
        private MavlinkPacket mavlink_DTU_Packet = new MavLink.MavlinkPacket();
        private MavlinkProxy mavlinkProxy;
        byte[] result = new byte[1024];
        Font font = new Font("宋体", 12);
        private int port;
        private int maxClientCount;
        private string ip;
        private Form1 form1;
        private IPEndPoint ipEndPoint;
        private Socket ServerSocket;
        private List<Socket> ClientSockets;
        byte[] byteTemp = new byte[4];
        private System.Timers.Timer aTimer;
        public Socket[] ClientSocketIndex = new Socket[256];
        private byte[] cmd = new byte[8] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x08, 0x44, 0x0C };
        USV_Position USV_0 = new USV_Position(0, 0);
        USV_Position USV_1 = new USV_Position(0, 0);
        USV_Position USV_2 = new USV_Position(0, 0);

        public TCP(Form1 f1)
        {
            form1 = f1;

            mavlink_DTU.PacketReceived += Mavlink_DTU_PacketReceived;
            try { 
            using (Socket skt = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                skt.Connect("8.8.8.8", 65530);//远程主机的IP地址和端口号
                IPEndPoint ipEnd = skt.LocalEndPoint as IPEndPoint;//加上下面两行程序 显示本地IP地址
                ip = ipEnd.Address.ToString();
                // MessageBox.Show(ipEnd.ToString());//窗口显示当前端口号
                form1.ipadress.Invoke(new MethodInvoker(delegate ()
                {
                    form1.ipadress.Text = ip;
                }));
            }
        }
        catch{
                if (form1.connectstate.IsHandleCreated)
                {
                    form1.connectstate.Invoke(new MethodInvoker(delegate ()
                {
                    form1.connectstate.AppendText(System.DateTime.Now.ToString() + $" 服务端未打开" + "\r\n");
                }));
                }

            }

            port = 8899;//服务端口，理论自定义范围 1-65535，不可与以开放端口冲突   //本地端口号  开放端口是什么意思？
            maxClientCount = 256; //设定最大连接数
            ClientSockets = new List<Socket>();
            IPAddress iPAddress = IPAddress.Parse(ip);//本地地址？
            ipEndPoint = new IPEndPoint(iPAddress, port);      //初始化IP终端 放入本地IP和端口号
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);     //初始化服务端Socket，新建一个socket
            ServerSocket.Bind(ipEndPoint);      //端口绑定
            ServerSocket.Listen(maxClientCount);     //设置监听数目的最大值
            form1.portnumber.Invoke(new MethodInvoker(delegate ()
            {
                form1.portnumber.Text = port.ToString();

            }));
            Thread ServerThread = new Thread(() =>
            {
                ListenClientConnect();         //开启监听，后台线程，非阻塞
            })
            {
                IsBackground = true,
            };

            ServerThread.Start();//服务端线程开启

            form1.connectstate.Invoke(new MethodInvoker(delegate ()
            {
                //form1.richTextBox1.AppendText($"{DateTime.Now} 服务端已开启:{ipEndPoint}\r\n", System.Drawing.Color.Lime, font);
                form1.connectstate.AppendText(System.DateTime.Now.ToString() + $" 服务端已打开：" + ipEndPoint + "\r\n");
            }));


            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(TimerEvent);  //到达时间的时候执行事件；
                                                                    // 设置引发时间的时间间隔　此处设置为１秒（１０００毫秒）
            aTimer.Interval = 1000;
            aTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            aTimer.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
            aTimer.Start();
        }

        private void ListenClientConnect()
        {
            while (true)
            {
                    try
                    {
                        Socket ClientSocket = ServerSocket.Accept();//使用ACCEPT后 上文的监听最大值将没有用处 client表示客户端 //抓取连接请求
                                                                  //将获取到的客户端添加到客户端列表
                        ClientSockets.Add(ClientSocket);

                        form1.connectstate.Invoke(new MethodInvoker(delegate ()
                        { 
                            form1.connectstate.AppendText($"{System.DateTime.Now.ToString()} 客户端接入:{ClientSocket.RemoteEndPoint} \r\n");
                        }));

                        ParameterizedThreadStart ThreadFun = new ParameterizedThreadStart(ReceiveClient);//创建一个obj传参的线程委托
                                                                                                         //创建客户端消息线程，实现客户端消息的循环监听
                        Thread ReveiveThread = new Thread(ThreadFun)
                        {
                            IsBackground = true,
                        };
                        ReveiveThread.Start(ClientSocket);
                    }
                    catch (Exception ex)

                    {
                    if (form1.connectstate.IsHandleCreated)
                    {
                        form1.connectstate.Invoke(new MethodInvoker(delegate ()
                        {
                            form1.connectstate.AppendText($"{System.DateTime.Now.ToString()} 连接侦听异常:{ex.Message.ToString()}  \r\n");
                        }));
                    }
                    }
                
            }
        }

        public static bool isNumber(string str)//判断字符串是否为数字
        {
            bool isMatch = Regex.IsMatch(str, @"^\d+$"); // 判断字符串是否为数字 的正则表达式
            return isMatch;
        }
        Socket _ClientSocket;
        private void ReceiveClient(object obj)
        {
            _ClientSocket = (Socket)obj;
            while (true)
            {
                try
                {
                    // 获取数据长度
                    int receiveLength = _ClientSocket.Receive(result);//获取收到的字节数？
                    string strData = Encoding.UTF8.GetString(result, 0, receiveLength);//数据                                                  //获取客户端发来的数据
                    string hexString = BitConverter.ToString(result, 0, receiveLength).Replace("-", "");
                    byte[] result1 = new byte[receiveLength];
                    result1 = result;

                    if (receiveLength <= 0)
                        continue;
                    if (receiveLength == 5 && !isNumber(strData))
                    {
                        if (strData.Contains("USV_0"))
                        {
                            ClientSocketIndex[0] = _ClientSocket;
                            form1.connectstate.Invoke(new MethodInvoker(delegate ()
                            {
                                form1.connectstate.AppendText($"{DateTime.Now} DTU:{strData}已注册  \r\n");
                            }));
                        }
                        //if (e.Message.ToString().Contains("Msg_usv_state"))
                            if (strData.Contains( "USV_1"))
                        {
                            ClientSocketIndex[1] = _ClientSocket;
                            form1.connectstate.Invoke(new MethodInvoker(delegate ()
                            {
                                form1.connectstate.AppendText($"{DateTime.Now} DTU:{strData}已注册  \r\n");
                            }));
                        }
                        if (strData.Contains("USV_2"))
                        {
                            ClientSocketIndex[2] = _ClientSocket;
                            form1.connectstate.Invoke(new MethodInvoker(delegate ()
                            {
                                form1.connectstate.AppendText($"{DateTime.Now} DTU:{strData}已注册  \r\n");
                            }));
                        }
                    }
                    else
                    {
                        if (form1.datastart.Text == "停止上传")
                        {
                            mavlink_DTU.ParseBytes(result1);//MAV程序
                        }
                        else 
                        {

                        }
                    }
                }
                catch (Exception e)
                {
                    if (form1.connectstate.IsHandleCreated)
                    {
                        form1.connectstate.Invoke(new MethodInvoker(delegate ()
                    {
                        form1.connectstate.AppendText($"{System.DateTime.Now.ToString()} 客户端{_ClientSocket.RemoteEndPoint}从服务器断开,断开原因：{e.Message.ToString()}  \r\n");
                    }));

                        // 从客户端列表中移除该客户端
                        this.ClientSockets.Remove(_ClientSocket);
                        //断开连接
                        _ClientSocket.Shutdown(SocketShutdown.Both);
                        _ClientSocket.Close();
                        break;
                    }//不清楚这里break是否有影响？0905
                }
            }

            
        }

        private void TimerEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                if (ClientSockets.Contains(ClientSocketIndex[0]))
                {
                    //ClientSocketIndex[0].Send(cmd, cmd.Length, SocketFlags.None);
                }
            }
            catch (Exception ex)//通讯出现异常
            {
                form1.connectstate.Invoke(new MethodInvoker(delegate ()
                {
                    form1.connectstate.AppendText($"{DateTime.Now} 客户端{ClientSocketIndex[0].RemoteEndPoint}从服务器断开,断开原因:{ex.Message}  \r\n");
                }));
                //从客户端列表中移除该客户端
                this.ClientSockets.Remove(ClientSocketIndex[0]);
                //断开连接
                ClientSocketIndex[0].Shutdown(SocketShutdown.Both);
                ClientSocketIndex[0].Close();
            }
            try
            {
                if (ClientSockets.Contains(ClientSocketIndex[2]))
                {
                    //ClientSocketIndex[2].Send(cmd, cmd.Length, SocketFlags.None);
                }
            }
            catch (Exception ex)//通讯出现异常
            {
                form1.connectstate.Invoke(new MethodInvoker(delegate ()
                {
                    form1.connectstate.AppendText($"{DateTime.Now} 客户端{ClientSocketIndex[2].RemoteEndPoint}从服务器断开,断开原因:{ex.Message}  \r\n");
                }));
                //从客户端列表中移除该客户端
                this.ClientSockets.Remove(ClientSocketIndex[2]);
                //断开连接
                ClientSocketIndex[2].Shutdown(SocketShutdown.Both);
                ClientSocketIndex[2].Close();
            }
        }

        public void Mavlink_DTU_PacketReceived(object sender, MavLink.MavlinkPacket e)
        {
            double x;
            double y;


            if (IDJ[0] == null)
            {
                IDJ[0] = e.ComponentId;
            }
            else if (IDJ[0] != e.ComponentId && IDJ[1] == null)
            {
                IDJ[1] = e.ComponentId;

            }
            else if (IDJ[0] != e.ComponentId && IDJ[1] != e.ComponentId && IDJ[2] == null)
            {
                IDJ[2] = e.ComponentId;

            }

            string ID0 = IDJ[0].ToString();//设定当前界面监视的ID号 
            string ID1 = IDJ[1].ToString();
            string ID2 = IDJ[2].ToString();


               int Dev_id =e.ComponentId;//获取设备ID与状态
                string ID = Dev_id.ToString();
                latitude = ((Msg_usv_state)e.Message).latitude;
                longitude = ((Msg_usv_state)e.Message).longitude;
                speed = ((Msg_usv_state)e.Message).speed;
                heading = ((Msg_usv_state)e.Message).heading;
                voltage = ((Msg_usv_state)e.Message).battery_voltage;


            //form1.ID.Invoke(new MethodInvoker(delegate ()
            //{
            //    form1.ID.Text = ID;
            //}));

            if (IDJ[0] != null)
            {
                if (IDJ[0] == Dev_id)
                {
                    form1.ID1.Invoke(new MethodInvoker(delegate ()//显示ID
                {
                    form1.ID1.Text = ID0;
                }));
                    form1.fastLine22.Add(speed);
                    form1.fastLine23.Add(heading);
                    form1.fastLine24.Add(voltage);
                    form1.fastLine31.Add(latitude);
                    form1.fastLine32.Add(longitude);
                    form1.MCT84Bl2xy(latitude, longitude, out USV_0.X, out USV_0.Y);
                }
            }



            if (IDJ[1] != null)
            {
                if (IDJ[1] == Dev_id)
                {
                    form1.ID2.Invoke(new MethodInvoker(delegate ()//显示ID
                {
                    form1.ID2.Text = ID1;
                }));
                    form1.fastLine27.Add(speed);
                    form1.fastLine28.Add(heading);
                    form1.fastLine29.Add(voltage);
                    form1.fastLine33.Add(latitude);
                    form1.fastLine34.Add(longitude);
                    form1.MCT84Bl2xy(latitude, longitude, out USV_1.X, out USV_1.Y);
                }
            }
             if (IDJ[2] != null)
            {
                if (IDJ[2] == Dev_id)
                {
                    form1.ID3.Invoke(new MethodInvoker(delegate ()//显示ID
                {
                    form1.ID3.Text = ID2;
                }));
                    form1.fastLine25.Add(speed);
                    form1.fastLine26.Add(heading);
                    form1.fastLine30.Add(voltage);
                    form1.fastLine35.Add(latitude);
                    form1.fastLine36.Add(longitude);
                    form1.MCT84Bl2xy(latitude, longitude, out USV_2.X, out USV_2.Y);
                }
            }
        }


        public void drawtrack()
        {
            Draw(form1.horizLine21, USV_0.X - X_Standard, USV_0.Y - Y_Standard);
            form1.x1.Text = (USV_0.X - X_Standard).ToString("0.00");
            form1.y1.Text = (USV_0.Y - Y_Standard).ToString("0.00");

            Draw(form1.horizLine22, USV_1.X - X_Standard, USV_1.Y - Y_Standard);
            form1.x2.Text = (USV_1.X - X_Standard).ToString("0.00");
            form1.y2.Text = (USV_1.Y - Y_Standard).ToString("0.00");

            Draw(form1.horizLine23, USV_2.X - X_Standard, USV_2.Y - Y_Standard);
            form1.x3.Text = (USV_2.X - X_Standard).ToString("0.00");
            form1.y3.Text = (USV_2.Y - Y_Standard).ToString("0.00");
        }

        public void Draw(HorizLine horizLine, double X, double Y)
        {
            horizLine.Add(X, Y);
        }
        public void _Click()//坐标矫正
        {
            X_Standard += System.Convert.ToDouble(form1.x1.Text);
            Y_Standard += System.Convert.ToDouble(form1.y1.Text);
            form1.horizLine21.Clear();
            form1.horizLine22.Clear();
            form1.horizLine23.Clear();
        }


        public void back()//向DTU发送数据
        {
            byte[] la = new byte[8];
            la = Encoding.UTF8.GetBytes("x:"+ form1.textBox15.Text +"  "+"y:"+ form1.textBox16.Text);
            ClientSocketIndex[2].Send(la, la.Length, SocketFlags.None);
            //ClientSocketIndex[2].Send(cmd, cmd.Length, SocketFlags.None);
        }
    }
}