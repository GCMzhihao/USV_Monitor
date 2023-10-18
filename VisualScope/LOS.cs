using MavLink;
using Steema.TeeChart.Editors.Series;
using Steema.TeeChart.Styles;
using Steema.TeeChart.Themes;
using System;
using System.Web.UI.WebControls;
using System.Windows;
using static 地面站.LOS;

namespace 地面站
{
    public class LOS//船的初始位置必须为期望路径中的点。如果不是期望路径中的点，需要给x,y,Ze等需要积分算出的变量赋初值
    {
        public double w;//期望路径的参数方程自变量
        double w_deriv;//期望路径的参数方程自变量的导数
        public double x, y;//实际位置
        public double x_F, y_F;//期望路径的参数方程因变量
        double x_F_last, y_F_last;
        string x_F_expression, y_F_expression;//表达式
        double U;//航速
        public double psi;//艏向角ψ
        double psi_F;//期望路径切向角ψF
        double psi_F_last;//上一时刻期望路径切向角ψF
        double psi_F_deriv;//期望路径切向角ψF的导数
        public double x_Start,y_Start;
        public double x_End, y_End;
        

        private readonly Form1 form1;
        public  PID pid_u, pid_r;

        public struct Result
        {
            public double psi_d;
            public double vel;
        }
        Result result;//轨迹算法计算结果
        double psi_d;//输出设定航向
        double beta;//漂角β（航向角-艏向角）
        //double beta_est;//漂角估计
        //double beta_est_deriv;//漂角估计的导数
        public double x_err, y_err;//位置误差
        //double x_err_est, y_err_est;//位置误差估计
        //double x_err_est_deriv, y_err_est_deriv;//位置误差估计的导数
        //double x_err_est_err, y_err_est_err;//位置误差估计的误差
        double gamma1;//设计参数Γ1
        double kp;//设计参数kp
        double kx;//设计参数kx
        double ky;//设计参数ky
        double delta;//设计参数△
        public void LOS_Clear()
        {
            w = 0;
            w_deriv = 0;
            x = 0;
            y = 0;
            x_F = 0;
            y_F = 0;
            U = 0;
            psi = 0;
            psi_F = 0;
            psi_F_last = 0;
            psi_F_deriv = 0;
            //beta_est = 0;
            //beta_est_deriv = 0;
            x_err = 0;
            y_err = 0;
            //x_err_est = 0;
            //y_err_est = 0;
            //x_err_est_deriv = 0;
            //y_err_est_deriv = 0;
            //x_err_est_err = 0;
            //y_err_est_err = 0;
            result.psi_d = 0;
            result.vel=0;
            pid_u.Clear();
            pid_r.Clear();
        }
        public LOS(object sender, string x,string y, double kp_, double delta_)
        {
            form1 = (Form1)sender;
            UpdateExpectedPath(x, y);
            
            kp = kp_;
            delta = delta_;
            pid_u = new PID();
            pid_r = new PID();

            LOS_Clear();
        }
        public void UpadataParam(double kp_,double delta_)
        {
            kp = kp_;
            delta = delta_;
        }
        public void Update_XY_F(double X_E,double Y_E, double X_S, double Y_S)
        {
            x_End = X_E;
            y_End = Y_E;
            x_Start = X_S;
            y_Start = Y_S;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_">实际坐标x</param>
        /// <param name="y_">实际坐标y</param>
        /// <param name="heading">艏向角</param>
        /// <param name="course">航向角</param>
        /// <param name="speed">航速</param>
        public void UpdateData(double x_,double y_, double heading, double course, double speed)//heading艏向角；course航向角；speed船速
        {
            x = x_;
            y = y_;
            psi = heading;
            U = speed;
            beta = course - heading;
            while (beta >= Math.PI)
                beta -= Math.PI * 2;
            while(beta<-Math.PI)
                beta += Math.PI * 2;
        }
        public void UpdateExpectedPath(string x, string y)
        {
            x_F_expression = "var x=" + x + ";";
            y_F_expression = "var y=" + y + ";";
        }
        //输入：T为周期
        //输出:期望航向
        public double Calculate(double T)
        {
            double h = 0.0001;
            double x_F_1, y_F_1; //期望的xy坐标
            double x_F_deriv_w, y_F_deriv_w;
            double x_sig;
            double y_sig;

            x_F = Eval.Calculate(w, x_F_expression);
            y_F = Eval.Calculate(w, y_F_expression); 
            x_F_1 = Eval.Calculate(w + h, x_F_expression);
            y_F_1 = Eval.Calculate(w + h, y_F_expression);
            x_F_deriv_w = (x_F_1 - x_F) / h;//对参数w求导
            y_F_deriv_w = (y_F_1 - y_F) / h;//对参数w求导
            psi_F = Math.Atan2(y_F_deriv_w, x_F_deriv_w);
            psi_F_deriv = (psi_F - psi_F_last) / T;
            x_err = Math.Cos(psi_F) * (x - x_F) + Math.Sin(psi_F) *(y - y_F);
            y_err = -Math.Sin(psi_F) * (x - x_F) + Math.Cos(psi_F) * (y - y_F);

            

            //x_err_est_err = x_err_est - x_err;
            //y_err_est_err = y_err_est - y_err;
            //beta_est_deriv = gamma1 * (U * Math.Sin(psi - psi_F) * x_err_est_err - U * Math.Cos(psi - psi_F) * y_err_est_err);
            //beta_est += beta_est_deriv * T;
            //x_err_est_deriv = psi_F_deriv * y_err_est - kp * x_err_est - kx * x_err_est_err;
            //y_err_est_deriv = U * Math.Sin(psi - psi_F) + U * Math.Cos(psi - psi_F) * beta_est - psi_F_deriv * x_err_est - ky * y_err_est_err;
            //x_err_est += x_err_est_deriv * T;
            //y_err_est += y_err_est_deriv * T;

            x_sig = Math.Pow(Math.Abs(x_err), 1.2) * Math.Sign(x_err)+Math.Pow(Math.Abs(x_err), 0.8) * Math.Sign(x_err);
            y_sig = Math.Pow(Math.Abs(y_err), 1.2) * Math.Sign(y_err)+Math.Pow(Math.Abs(y_err),0.8) * Math.Sign(y_err);
            //los
            psi_d = psi_F + Math.Atan(-y_err / delta - beta);
            w_deriv = (U * Math.Cos(psi - psi_F) - U * Math.Sin(psi - psi_F) * beta + kp * x_err) / (Math.Sqrt(x_F_deriv_w * x_F_deriv_w + y_F_deriv_w * y_F_deriv_w));

            //固定时间los
            //psi_d = psi_F + Math.Atan(-y_sig / delta - beta);
            //w_deriv = (U * Math.Cos(psi - psi_F) - U * Math.Sin(psi - psi_F) * beta + kp * x_sig) / (Math.Sqrt(x_F_deriv_w * x_F_deriv_w + y_F_deriv_w * y_F_deriv_w));

            w += w_deriv * T;//更新路径参数
            psi_F_last = psi_F;

            return psi_d;
        }

        public Result Calculate_trajectory(double T)
        {
            double h = 0.0001;
            double x_F_deriv_w, y_F_deriv_w;
            double x_F_1, y_F_1; //期望的xy坐标
            double U_d;
            double U_p;
            double t;
            t = T * form1.track_time;

            x_F = Eval.Calculate(t, x_F_expression);
            y_F = Eval.Calculate(t, y_F_expression);
            x_F_1 = Eval.Calculate(t + h, x_F_expression);
            y_F_1 = Eval.Calculate(t + h, y_F_expression);
            x_F_deriv_w = (x_F_1 - x_F) / h;//对参数w求导
            y_F_deriv_w = (y_F_1 - y_F) / h;//对参数w求导
            psi_F = Math.Atan2(y_F_deriv_w, x_F_deriv_w);
            psi_F_deriv = (psi_F - psi_F_last) / T;
            x_err = Math.Cos(psi_F) * (x - x_F) + Math.Sin(psi_F) * (y - y_F);
            y_err = -Math.Sin(psi_F) * (x - x_F) + Math.Cos(psi_F) * (y - y_F);

            result.psi_d=psi_F+Math.Atan(-y_err/delta)-beta;
            U_d = Math.Sqrt(x_F_deriv_w*x_F_deriv_w+y_F_deriv_w*y_F_deriv_w);
            U_p = ((U_d-kp*x_err)*Math.Sqrt(y_err*y_err+delta*delta)) / delta;
            result.vel = U_p*Math.Cos(beta);
            if (result.vel > U_d * 2.5)//限制输出速度，防止初始误差过大，设定值太大
                result.vel = U_d * 2.5;
            else if (result.vel < 0)
                result.vel = 0;
            psi_F_last = psi_F;

            return result;
        }
        public Result Calculate_Point_Leader(double T)
        {
            double k,b;

            x_F = w;
            psi_F = Math.Atan2(y_End-y_Start, x_End-x_Start);
            if (x_End - x_Start ==0)
            {
                y_F = y_Start;
                k = 0;
            }
            else
            {
                k = Math.Tan(psi_F);
                b = y_Start - k * x_Start;
                y_F = k * x_F + b;
            }

            x_err = Math.Cos(psi_F) * (x - x_F) + Math.Sin(psi_F) * (y - y_F);
            y_err = -Math.Sin(psi_F) * (x - x_F) + Math.Cos(psi_F) * (y - y_F);

            //los
            psi_d = psi_F + Math.Atan(-y_err / delta - beta);

            w_deriv = (U * Math.Cos(psi - psi_F) 
                - U * Math.Sin(psi - psi_F) * beta + kp * x_err) / (Math.Sqrt(1 
                + k * k));
            w_deriv *= Math.Sign(x_F - x_Start);
            w += w_deriv * T;//更新路径参数

            result.psi_d = psi_d;
            double ud = Convert.ToSingle(form1.textBox_Speed_Set.Text);
            result.vel = ud;
            while (result.psi_d >= Math.PI)
                result.psi_d -= Math.PI * 2;
            while (result.psi_d < -Math.PI)
                result.psi_d += Math.PI * 2;
            return result;
        }
        public Result Calculate_Point_Follower(double x_l, double y_l, double psi_l,
                        double dt, double L, double Theta, double ud)
        {
            double x_f_d, y_f_d;
            double theta;
            double U_d;
            double U_p;
            double d;

            theta = Theta * Math.PI / 180;//弧度
            psi_l = psi_l * Math.PI / 180;//弧度

            x_f_d = x_l + L * Math.Cos(theta + psi_l);
            y_f_d = y_l + L * Math.Sin(theta + psi_l);

            x_F = x_f_d;
            y_F = y_f_d;
            d = Math.Sqrt((x - x_F) * (x - x_F) + (y - y_F) * (y - y_F));//与虚拟船距离
            if (d > 10)//距离较远
                psi_F = Math.Atan2(y_F - y, x_F - x);
            else
                psi_F = psi_l;
            x_err = Math.Cos(psi_F) * (x - x_F) + Math.Sin(psi_F) * (y - y_F);
            y_err = -Math.Sin(psi_F) * (x - x_F) + Math.Cos(psi_F) * (y - y_F);

            result.psi_d = psi_F + Math.Atan(-y_err / delta) - beta;
            U_d = ud;
            U_p = ((U_d - kp * x_err) * Math.Sqrt(y_err * y_err + delta * delta)) / delta;
            if (U_p <= 0)
                U_p = U_d ;

            result.vel = U_p * Math.Cos(beta);
            if (result.vel > U_d * 2)//限制输出速度，防止初始误差过大，设定值太大
                result.vel = U_d * 2;
            else if (result.vel < 0)
                result.vel = 0;
            //psi_F_last = psi_F;
            //x_F_last = x_F;
            //y_F_last = y_F;
            return result;
        }

        public Result Caculate_Follower(double T,double L,double Theta)
        {
            double h = 0.0001;
            double t;
            double x_L, y_L;//期望领航者的xy坐标
            double x_L_1, y_L_1;
            double x_L_deriv_w, y_L_deriv_w;//期望xy坐标的导数
            double x_F_1, y_F_1; //期望的跟随者xy坐标
            double x_F_deriv_w, y_F_deriv_w;//期望xy坐标的导数
            double U_d;
            double U_p;
            double psi_F_deriv_W;
            double psi_L, psi_L_1;
            double theta;
           

            t = T * form1.track_time;

            x_L = Eval.Calculate(t, x_F_expression);
            y_L = Eval.Calculate(t, y_F_expression);
            x_L_1 = Eval.Calculate(t + h, x_F_expression);
            y_L_1 = Eval.Calculate(t + h, y_F_expression);
            x_L_deriv_w = (x_L_1 - x_L) / h;//对参数w求导
            y_L_deriv_w = (y_L_1 - y_L) / h;//对参数w求导
            psi_L = Math.Atan2(y_L_deriv_w, x_L_deriv_w);
            psi_F = psi_L;
            //theta = Theta;
            theta = Theta * Math.PI / 180;//弧度
            //psi_L = psi_L * Math.PI / 180;//弧度

            x_F = x_L + L * Math.Cos(theta + psi_L);
            y_F = y_L + L * Math.Sin(theta + psi_L);
            //转换成跟随者的期望xy以及psi

            
            x_L = Eval.Calculate(t + h, x_F_expression);
            y_L = Eval.Calculate(t + h, y_F_expression);
            x_L_1 = Eval.Calculate(t + 2*h, x_F_expression);
            y_L_1 = Eval.Calculate(t + 2*h, y_F_expression);
            x_L_deriv_w = (x_L_1 - x_L) / h;//对参数w求导
            y_L_deriv_w = (y_L_1 - y_L) / h;//对参数w求导
            psi_L_1 = Math.Atan2(y_L_deriv_w, x_L_deriv_w);


           // theta = Theta * Math.PI / 180;//弧度
            //psi_L_1 = psi_L_1 * Math.PI / 180;//弧度

            x_F_1 = x_L + L * Math.Cos(theta + psi_L_1);
            y_F_1 = y_L + L * Math.Sin(theta + psi_L_1);


            x_F_deriv_w = (x_F_1 - x_F) / h;
            y_F_deriv_w = (y_F_1 - y_F) / h;//计算出跟随者的xy坐标的导数

            psi_F_deriv = (psi_F - psi_F_last) / T;
            x_err = Math.Cos(psi_F) * (x - x_F) + Math.Sin(psi_F) * (y - y_F);
            y_err = -Math.Sin(psi_F) * (x - x_F) + Math.Cos(psi_F) * (y - y_F);

            result.psi_d = psi_F + Math.Atan(-y_err / delta) - beta;
            U_d = Math.Sqrt(x_F_deriv_w * x_F_deriv_w + y_F_deriv_w * y_F_deriv_w);
            U_p = ((U_d - kp * x_err) * Math.Sqrt(y_err * y_err + delta * delta)) / delta;

            if (U_p <= 0)
                U_p = U_d;

            result.vel = U_p * Math.Cos(beta);

            psi_F_last = psi_F;
            if (result.vel > U_d * 1.5)//限制输出速度，防止初始误差过大，设定值太大
                result.vel = U_d * 1.5;
            else if (result.vel < 0)
                result.vel = 0;
            return result;
        }
        /// <summary>
        /// 更新仿真位置，仅仿真LOS算法时使用
        /// </summary>
        /// <param name="psi_">艏向角</param>
        /// <param name="beta_">漂角</param>
        /// <param name="T">采样周期</param>
        public void UpdateSimulationPosition(double psi_,double beta_,double T)//仅仿真LOS算法时使用
        {
            x += U * Math.Cos(psi_ + beta_) * T;
            y += U * Math.Sin(psi_ + beta_) * T;
        }
    }
}
