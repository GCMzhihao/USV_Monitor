using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Steema.TeeChart.Styles;

namespace 地面站
{
    class VirtualLeader//假设虚拟船始终运行在预定轨迹上
    {
        public double x, y, psi, vx, vy;//虚拟仿真用，x坐标，y坐标，艏向角
        public double w;
        public double Speed_Exp;
        private string X_Exp;
        private string Y_Exp;
        private readonly TextBox TB_Speed;
        private readonly TextBox TB_X_Exp;
        private readonly TextBox TB_Y_Exp;
        private readonly TBoxOnlyNumber TBOnlyNumbers;
        public HorizLine horizLine;

        public VirtualLeader(TextBox speed,
                             TextBox x_exp,
                             TextBox y_exp)
        {
            TB_Speed = speed;
            TB_X_Exp = x_exp;
            TB_Y_Exp = y_exp;
            TBOnlyNumbers = new TBoxOnlyNumber(TB_Speed);
            TB_Speed.Leave -= new EventHandler(UpdateParameters);
            TB_Speed.Leave += new EventHandler(UpdateParameters);
            TB_X_Exp.Leave -= new EventHandler(UpdateParameters);
            TB_X_Exp.Leave += new EventHandler(UpdateParameters);
            TB_Y_Exp.Leave -= new EventHandler(UpdateParameters);
            TB_Y_Exp.Leave += new EventHandler(UpdateParameters);

            Speed_Exp = Convert.ToDouble(TB_Speed.Text);
            X_Exp = "var x=" + TB_X_Exp.Text + ";";
            Y_Exp = "var y=" + TB_Y_Exp.Text + ";";

        }
        public void Init(double w0)
        {
            w = w0;
            double x_1, y_1;
            double x_deriv_w, y_deriv_w;
            double h = 0.0001;
            x = Eval.Calculate(w, X_Exp);
            y = Eval.Calculate(w, Y_Exp);
            x_1 = Eval.Calculate(w + h, X_Exp);
            y_1 = Eval.Calculate(w + h, Y_Exp);
            x_deriv_w = (x_1 - x) / h;//对参数w求导
            y_deriv_w = (y_1 - y) / h;//对参数w求导
            psi = Math.Atan2(y_deriv_w, x_deriv_w) * 180 / Math.PI;//当前时刻艏向角，度
        }
        private void UpdateParameters(object sender, EventArgs e)
        {
            if(((TextBox)sender)== TB_Speed)
                Speed_Exp = Convert.ToDouble(TB_Speed.Text);
            else if (((TextBox)sender) == TB_X_Exp)
                X_Exp = "var x=" + TB_X_Exp.Text + ";";
            else if (((TextBox)sender) == TB_Y_Exp)
                Y_Exp = "var y=" + TB_Y_Exp.Text + ";";
        }
        public void Simulation(double dt)
        {
            double  x_1, y_1;
            double x_deriv_w, y_deriv_w;
            double h = 0.0001;
            x = Eval.Calculate(w, X_Exp);
            y = Eval.Calculate(w, Y_Exp);
            x_1 = Eval.Calculate(w + h, X_Exp);
            y_1 = Eval.Calculate(w + h, Y_Exp);
            x_deriv_w = (x_1 - x) / h;//对参数w求导
            y_deriv_w = (y_1 - y) / h;//对参数w求导
            psi = Math.Atan2(y_deriv_w, x_deriv_w) * 180 / Math.PI;//当前时刻艏向角，度

            w += Speed_Exp * dt / Math.Sqrt(x_deriv_w * x_deriv_w + y_deriv_w * y_deriv_w);//下一时刻参数w
            
        }
    }
   class UAV_Followers
    {
        double L;//跟随距离
        double Theta;//跟随角度，度
        public double x_f_exp, y_f_exp;//期望位置
        public double vx_set, vy_set;
        public double x, y,vx,vy;//仿真用
        private double Gama;
        private TextBox TB_L;
        private TextBox TB_Theta;
        private TextBox TB_kpx;
        private TextBox TB_kix;
        private TextBox TB_kdx;

        private TextBox TB_kpy;
        private TextBox TB_kiy;
        private TextBox TB_kdy;

        private TextBox TB_Gama;//微分滤波系数（0~1）

        public PID pid_x;
        public PID pid_y;

        public HorizLine[] HorizLines = new HorizLine[2];//HorizLine[0]为期望值曲线，HorizLine[1]为实际值曲线

        private readonly TBoxOnlyNumber[] TBOnlyNumbers = new TBoxOnlyNumber[10];

        public UAV_Followers(TextBox l,
                            TextBox theta,
                            TextBox kpx,
                            TextBox kix,
                            TextBox kdx,
                            TextBox kpy,
                            TextBox kiy,
                            TextBox kdy,
                            TextBox gama)
        {
            pid_x = new PID();
            pid_y = new PID();
            
            double kp_x, ki_x, kd_x;
            double kp_y, ki_y, kd_y;

            TB_L = l;
            TB_Theta = theta;
            TB_kpx = kpx;
            TB_kix = kix;
            TB_kdx = kdx;
            TB_kpy = kpy;
            TB_kiy = kiy;
            TB_kdy = kdy;
            TB_Gama = gama;

            TBOnlyNumbers[0] = new TBoxOnlyNumber(TB_L);
            TBOnlyNumbers[1] = new TBoxOnlyNumber(TB_Theta);
            TBOnlyNumbers[2] = new TBoxOnlyNumber(TB_kpx);
            TBOnlyNumbers[3] = new TBoxOnlyNumber(TB_kix);
            TBOnlyNumbers[4] = new TBoxOnlyNumber(TB_kdx);
            TBOnlyNumbers[5] = new TBoxOnlyNumber(TB_kpy);
            TBOnlyNumbers[6] = new TBoxOnlyNumber(TB_kiy);
            TBOnlyNumbers[7] = new TBoxOnlyNumber(TB_kdy);
            TBOnlyNumbers[8] = new TBoxOnlyNumber(TB_Gama);

            TB_L.Leave -= new EventHandler(UpdateParameters);
            TB_Theta.Leave -= new EventHandler(UpdateParameters);
            TB_kpx.Leave -= new EventHandler(UpdateParameters);
            TB_kix.Leave -= new EventHandler(UpdateParameters);
            TB_kdx.Leave -= new EventHandler(UpdateParameters);
            TB_kpy.Leave -= new EventHandler(UpdateParameters);
            TB_kiy.Leave -= new EventHandler(UpdateParameters);
            TB_kdy.Leave -= new EventHandler(UpdateParameters);
            TB_Gama.Leave -= new EventHandler(UpdateParameters);

            TB_L.Leave += new EventHandler(UpdateParameters);
            TB_Theta.Leave += new EventHandler(UpdateParameters);
            TB_kpx.Leave += new EventHandler(UpdateParameters);
            TB_kix.Leave += new EventHandler(UpdateParameters);
            TB_kdx.Leave += new EventHandler(UpdateParameters);
            TB_kpy.Leave += new EventHandler(UpdateParameters);
            TB_kiy.Leave += new EventHandler(UpdateParameters);
            TB_kdy.Leave += new EventHandler(UpdateParameters);
            TB_Gama.Leave += new EventHandler(UpdateParameters);

            L = Convert.ToDouble(TB_L.Text);
            Theta = Convert.ToDouble(TB_Theta.Text);
            kp_x = Convert.ToDouble(TB_kpx.Text);
            ki_x = Convert.ToDouble(TB_kix.Text);
            kd_x = Convert.ToDouble(TB_kdx.Text);

            kp_y = Convert.ToDouble(TB_kpy.Text);
            ki_y = Convert.ToDouble(TB_kiy.Text);
            kd_y = Convert.ToDouble(TB_kdy.Text);

            Gama = Convert.ToDouble(TB_Gama.Text);

            pid_x.UpdateParam(kp_x, ki_x, kd_x, Gama);
            pid_y.UpdateParam(kp_y, ki_y, kd_y, Gama);
        }
        private void UpdateParameters(object sender, EventArgs e)
        {
            double kp_x, ki_x, kd_x;
            double kp_y, ki_y, kd_y;

            L = Convert.ToDouble(TB_L.Text);
            Theta = Convert.ToDouble(TB_Theta.Text);

            kp_x = Convert.ToDouble(TB_kpx.Text);
            ki_x = Convert.ToDouble(TB_kix.Text);
            kd_x = Convert.ToDouble(TB_kdx.Text);

            kp_y = Convert.ToDouble(TB_kpy.Text);
            ki_y = Convert.ToDouble(TB_kiy.Text);
            kd_y = Convert.ToDouble(TB_kdy.Text);
            Gama = Convert.ToDouble(TB_Gama.Text);

            pid_x.UpdateParam(kp_x, ki_x, kd_x, Gama);
            pid_y.UpdateParam(kp_y, ki_y, kd_y, Gama);
        }
        public void Calculate(  double x_l, double y_l,double psi_l,
                                double x_f, double y_f,double dt)
        {
            double x_f_d, y_f_d;
            double theta;

            theta = Theta * Math.PI / 180;//弧度
            psi_l = psi_l * Math.PI / 180;//弧度

            x_f_d = x_l + L * Math.Cos(theta + psi_l);
            y_f_d = y_l + L * Math.Sin(theta + psi_l);
            x_f_exp = x_f_d;
            y_f_exp = y_f_d;

            vx_set = pid_x.Calculate(x_f_d, x_f, dt);
            vy_set = pid_y.Calculate(y_f_d, y_f, dt);

        }
        public void Simulation(double dt)
        {
            vx = vx_set;
            vy = vy_set;
            x += vx * dt;
            y += vy * dt;
        }
    }
}

