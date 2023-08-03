using MavLink;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static 地面站.Form1;

namespace 地面站
{
    internal class USV
    {
        private readonly Form1 form1;

        public UInt16 DEV_ID;
        public Msg_usv_state state = new Msg_usv_state();
        public USV_Position Position = new USV_Position(0, 0);
        public Msg_usv_set set = new Msg_usv_set();
        public LOS Los;
        HorizLine horizLine;
        public MavlinkPacket mavlinkPacket;

        public USV(object sender,UInt16 DEV_ID_)
        {
            form1 = (Form1)sender;
            DEV_ID =DEV_ID_;
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
            mavlinkPacket.SystemId= (int)MavLink.SYS_TYPE.SYS_GSTATION;
            mavlinkPacket.ComponentId = DEV_ID;
        }


        private void ll2XY()
        {
            form1.MCT84Bl2xy(state.longitude, state.latitude, out Position.X, out Position.Y);
        }
        private void Draw_Line()
        {
            form1.Tchart6_Draw(horizLine, Position.X-form1.X_Standard, Position.Y-form1.Y_Standard);
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
            ll2XY();//经纬度转XY坐标

            heading = state.heading / 180 * Math.PI;//弧度
            u = state.speed;
            course = state.Track / 180 * Math.PI;//弧度
            Draw_Line();
            Los.UpdateData(Position.X, Position.Y, heading, course, u);
            heading_set = Los.Calculate(T);//计算设定艏向角
            set.Heading = (float)heading_set;
            set.Speed = 1.5f;
            set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
            set.DEV_ID = (byte)DEV_ID;


            mavlinkPacket.Message = set;
            form1.SendMavMsgToRocker(mavlinkPacket);
            

        }



    }
}
