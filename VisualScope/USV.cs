using MavLink;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public Msg_cmd_write Msg_cmd=new Msg_cmd_write();
        public LOS Los;
        HorizLine horizLine;
        public MavlinkPacket mavlinkPacket;
        LOS.Result result;


        USV_State_Info usv_state_info;

        public USV(object sender, byte DEV_ID_)
        {
            form1 = (Form1)sender;
            DEV_ID = DEV_ID_;
        }

        public void Init(byte DEV_ID_)
        {
            DEV_ID = DEV_ID_;
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

        public void UNLOCK()
        {
            Msg_cmd.DEV_ID = DEV_ID;
            Msg_cmd.cmd_id = (byte)MavLink.CMD_TYPE.CMD_UNLOCK;
            form1.mavlinkPacket.Message = Msg_cmd;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);

        }

        public void LOCK()
        {
            Msg_cmd.DEV_ID = DEV_ID; 
            Msg_cmd.cmd_id = (byte)MavLink.CMD_TYPE.CMD_LOCK;
            form1.mavlinkPacket.Message = Msg_cmd;
            form1.SendMavMsgToRocker(form1.mavlinkPacket);

        }

        

        private void ll2XY()
        {
            double X;
            double Y;
            form1.MCT84Bl2xy(state.longitude, state.latitude, out X, out Y);
            Position.X = X - form1.X_Standard;
            Position.Y = Y - form1.Y_Standard;
        }
        private void Draw_Line()
        {
            horizLine.Add(Position.X, Position.Y);
        }

        public void USV_Info_Display()
        {
            //ll2XY();
            usv_state_info.ID.Text= DEV_ID.ToString();
            usv_state_info.VEL.Text = state.speed.ToString("0.00");
            usv_state_info.VOL.Text = state.battery_voltage.ToString("0.00");
            usv_state_info.X.Text = Position.X.ToString("0.00");
            usv_state_info.Y.Text = Position.Y.ToString("0.00");
        }

        public void LOS_Follower(double T)
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
                Los.UpdateData(Position.X - form1.X_Standard, Position.Y - form1.Y_Standard, heading, course, u);

                result = Los.Caculate_Follower(T, Convert.ToDouble(usv_state_info.L.Text), Convert.ToDouble(usv_state_info.angle.Text));
                result.psi_d = result.psi_d * 180 / Math.PI;
                set.Heading = (float)result.psi_d;
                set.Speed = (float)result.vel;
                if (set.Speed >= 5)
                {
                    set.Speed = 5;
                }

                if (set.Speed <= 0)
                {
                    set.Speed = 0;
                }
                set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
                set.DEV_ID = DEV_ID;

                form1.mavlinkPacket.Message = set;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);

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

            //ll2XY();//经纬度转XY坐标
            
           

            USV_Info_Display();

            
            if (form1.radioButton_Simulation.Checked)//仿真
            {
                
                Position.X = Los.x;
                Position.Y = Los.y;

                beta = 0.1;
                heading = Los.psi;
                course = heading + beta;

                if (form1.radioButton_Trajectory.Checked)//轨迹
                {

                    u = result.vel;
                    Los.UpdateData(Position.X, Position.Y, heading, course, u);
                    result = Los.Calculate_trajectory(T);
                  
                    Los.UpdateData(Position.X, Position.Y, heading, course, result.vel);
                    Los.UpdateSimulationPosition(result.psi_d, beta, T);//更新 x y 的值
                    state.speed = (float)result.vel;
                }
                else if (form1.radioButton_Trace.Checked)//路径
                {
                    Los.UpdateData(Position.X, Position.Y, heading, course, Convert.ToSingle(form1.textBox_Speed_Set.Text));
                    heading_set = Los.Calculate(T);
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
                Los.UpdateData(Position.X - form1.X_Standard, Position.Y - form1.Y_Standard, heading, course, u);

                set.SYS_TYPE = (byte)SYS_TYPE.SYS_USV;
                set.DEV_ID = DEV_ID;
                if (set.Speed >= 5)
                {
                    set.Speed = 5;
                }

                if (set.Speed <= 0)
                {
                    set.Speed = 0;
                }
                form1.mavlinkPacket.Message = set;
                form1.SendMavMsgToRocker(form1.mavlinkPacket);

            }

        }
    }
}
