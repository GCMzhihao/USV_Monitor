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
        public TextBox L;
        public TextBox angle;
        public TextBox kp;
        public TextBox delta;
        public USV_State_Info(Label ID_, Label VEL_, Label VOL_, Label X_, Label Y_, TextBox L_, TextBox angle_, TextBox kp_, TextBox delta_)
        {
            ID = ID_;
            VEL = VEL_;
            VOL = VOL_;
            X = X_;
            Y = Y_;
            L = L_;
            angle = angle_;
            kp = kp_;
            delta = delta_;

        }
    }
    public class USV
    {
        private readonly Form1 form1;

        public byte DEV_ID;
        public Msg_usv_state state = new Msg_usv_state();
        public USV_Position Position = new USV_Position(0, 0);
        public Msg_usv_set set = new Msg_usv_set();
        public LOS Los;
        HorizLine horizLine;
        public MavlinkPacket mavlinkPacket;
        LOS.Result result;

        USV_State_Info usv_state_info;

        public USV(object sender,byte DEV_ID_)
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
        public void Init(USV_State_Info usv_state_info_)
        {
            usv_state_info = usv_state_info_;
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
            Los.UpdateData(Position.X-form1.X_Standard, Position.Y-form1.Y_Standard, heading, course, u);

            result = Los.Calculate_trajectory(T);
            result.psi_d = result.psi_d * 180 / Math.PI;
            set.Heading = (float)result.psi_d;
            set.Speed = (float)result.vel;

            set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
            set.DEV_ID = (byte)DEV_ID;
            
            form1.mavlinkPacket.Message = set;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);
       
        }
    }
}
