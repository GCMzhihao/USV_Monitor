using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace 地面站
{
    class VisualTestConnect
    {
        private Form1 form1;
        public bool usv_visual_test_flag = false;
        public VisualTestConnect(object t)
        {
            Recvived();
            form1 = (Form1)t;
        }
        //发送
        private Socket socket;
        private IPEndPoint ipEnd;
        private readonly string nickName="127.0.0.1";//ip
        private readonly string portname = "5566";//端口
        public void Transmited(string sendMsg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(sendMsg);
            IPAddress ip1 = IPAddress.Parse(nickName);
            ipEnd = new IPEndPoint(ip1, Convert.ToInt32(portname));
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SendTo(buffer, buffer.Length, SocketFlags.None, ipEnd);
        }

        //接受
        private readonly int port = 6666;//端口
        private Thread thread;
        public string[] usv_state;//收到的内容
        private readonly bool issleep = false;
        private void Recvived()
        {
            //开一个新线程接收UDP发送的数据
            thread = new Thread(delegate ()  //delegate()也可写成 ()=>
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
                UdpClient udp = new UdpClient(ipep);
                UDPState state = new UDPState(ipep, udp);
                udp.BeginReceive(CallBackRecvive, state); //异步接收
            })
            { IsBackground = true }; //设置为后台线程，并开启线程
            thread.Start();
        }
        Stopwatch watch = new Stopwatch();
        private void CallBackRecvive(IAsyncResult ar)
        {
            while (issleep) Thread.Sleep(500);
            if (ar.AsyncState is UDPState state)
            {
                IPEndPoint ipep = state.IPEP;
                //这里接收到数据后，应该做数据完整检查，这里只是单纯做一个接收
                byte[] data = state.UDPClient.EndReceive(ar, ref ipep);
                //这里可以做一个数据检查 如：void CheckData(data)

                usv_state = Encoding.Default.GetString(data).Split(',');
                //继续接收下一条消息
                state.UDPClient.BeginReceive(CallBackRecvive, state);
                usv_visual_test_flag = true;
                //if (usv_visual_test_flag == false)
                //    return;
                //form1.LOS_TLC_Control(0.05);
                //form1.SF_Control(0.05);
            }
        }

        private class UDPState//UDPState类
        {
            private readonly UdpClient udpClient;

            public UdpClient UDPClient
            {
                get { return udpClient; }
            }

            private readonly IPEndPoint ipep;
            public IPEndPoint IPEP
            {
                get { return ipep; }
            }

            //构造函数
            public UDPState(IPEndPoint ipep, UdpClient udpClient)
            {
                this.ipep = ipep;
                this.udpClient = udpClient;
            }

        }
    }
}
