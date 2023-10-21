using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 地面站
{
    public class PID
    {
        public struct Param
        {
            public double Err;
            public double Kp;//比例系数
            public double Ki;//积分系数
            public double Kd;//微分系数
            public double Gama;//微分先行系数
            public double LastValue;//上次反馈值
            public double Integral;//偏差积分
            public double IntMax;//最大积分
            public double Pout;  //比例输出
            public double Iout;  //积分输出
            public double Dout;  //微分输出
            public double OutPut;//PID输出
            public double OutPutMax;//输出限幅
        }
        public Param pid ;
        private double LIMIT(double x, double min, double max)
        {
            if (x < min)
                x = min;
            if (x > max)
                x = max;
            return x;
        }
        public PID()
        {
            pid = new Param();
        }
        public void UpdateParam(double kp, double ki, double kd, double gama)
        {
            pid.Kp = kp;
            pid.Ki = ki;
            pid.Kd = kd;
            pid.Integral = 0;
            pid.Gama = gama;
        }
        public void UpdateParam(double intmax,double outputmax)
        {
            pid.IntMax = intmax;
            pid.OutPutMax = outputmax;
        }

        public double Calculate(double setvalue, double feedback, double dt)
        {

            if (pid.Kp == 0)
                return 0;

            pid.Err = setvalue - feedback;
            pid.Pout = pid.Kp * pid.Err;
            pid.Integral += pid.Ki*pid.Err * dt;
            if (pid.Ki < 0.00001)
                pid.Integral = 0;
            if (pid.IntMax != 0)
                pid.Integral = LIMIT(pid.Integral, -pid.IntMax, pid.IntMax);
            pid.Iout =  pid.Integral;
            pid.Dout = pid.Kd * (pid.Err - pid.LastValue) / dt;
            pid.LastValue = pid.Err;

            if (pid.OutPutMax == 0)
                pid.OutPut = pid.Pout + pid.Iout+ pid.Dout;
            else
                pid.OutPut = LIMIT((pid.Pout + pid.Iout+ pid.Dout), -pid.OutPutMax, pid.OutPutMax);
            return pid.OutPut;
        }
        public double Calculate(double err, double dt)
        {

            if (pid.Kp == 0)
                return 0;

            pid.Err = err;
            pid.Pout = pid.Kp * pid.Err;
            pid.Integral += pid.Ki * pid.Err * dt;
            if (pid.Ki < 0.00001)
                pid.Integral = 0;
            if (pid.IntMax != 0)
                pid.Integral = LIMIT(pid.Integral, -pid.IntMax, pid.IntMax);
            pid.Iout = pid.Integral;
            pid.Dout = pid.Kd * (pid.Err - pid.LastValue) / dt;
            pid.LastValue = pid.Err;

            if (pid.OutPutMax == 0)
                pid.OutPut = pid.Pout + pid.Iout + pid.Dout;
            else
                pid.OutPut = LIMIT((pid.Pout + pid.Iout + pid.Dout), -pid.OutPutMax, pid.OutPutMax);
            return pid.OutPut;
        }
        public void Clear()
        {
            pid.Integral = 0;
            pid.LastValue = 0;
        }





        //public double Calculate(double setvalue, double feedback, double dt)
        //{
        //    double err;
        //    double temp;
        //    double c1, c2, c3;
        //    //double Td;
        //    if (pid.Kp == 0)
        //        return 0;

        //    //Td = pid.Kd / pid.Kp;
        //    //temp = pid.Gama * Td + dt;
        //    //c3 = Td / temp;
        //    //c2 = (Td + dt) / temp;
        //    //c1 = pid.Gama * c3;

        //    temp = pid.Gama * pid.Kd + pid.Kp;
        //    c3 = pid.Kd / temp;
        //    c2 = (pid.Kd + pid.Kp) / temp;
        //    c1 = pid.Gama * c3;
        //    pid.Dout = c1 * pid.Dout + c2 * feedback - c3 * pid.LastValue;//微分先行
        //    pid.LastValue = feedback;

        //    err = setvalue - pid.Dout;
        //    pid.Pout = pid.Kp * err;

        //    pid.Integral += pid.Ki * err * dt;
        //    if (pid.IntMax != 0)
        //        pid.Integral = LIMIT(pid.Integral, -pid.IntMax, pid.IntMax);
        //    pid.Iout = pid.Integral;

        //    if (pid.OutPutMax == 0)
        //        pid.OutPut = pid.Pout + pid.Iout;
        //    else
        //        pid.OutPut = LIMIT((pid.Pout + pid.Iout), -pid.OutPutMax, pid.OutPutMax);
        //    return pid.OutPut;
        //}
        //    public double Calculate(double setvalue, double feedback, double dt)
        //    {
        //        double err;
        //        double temp;
        //        double c1, c2, c3;
        //        double Td;
        //        if (pid.Kp == 0)
        //            return 0;

        //        //temp = pid.Gama * pid.Kd +pid.Kp;
        //        //c3 = pid.Kd / temp;
        //        //c2 = (pid.Kd + pid.Kp) / temp;
        //        //c1 = pid.Gama * c3;
        //        //pid.Dout = c1 * pid.Dout + c2 * feedback - c3 * pid.LastValue;//微分先行

        //        Td = pid.Kd / pid.Kp;
        //        temp = pid.Gama * Td + dt;
        //        c3 = Td / temp;
        //        c2 = (Td + dt) / temp;
        //        c1 = pid.Gama * c3;
        //        pid.Dout = c1 * pid.Dout + c2 * feedback - c3 * pid.LastValue;//微分先行

        //        pid.LastValue = feedback;
        //        if (pid.Kd == 0)
        //            pid.Dout = 0;
        //        err = setvalue - feedback;
        //        pid.Pout = pid.Kp * err;
        //        pid.Integral += pid.Ki * err * dt;

        //        if (pid.IntMax != 0)
        //            pid.Integral = LIMIT(pid.Integral, -pid.IntMax, pid.IntMax);
        //        pid.Iout = pid.Integral;

        //        if (pid.OutPutMax == 0)
        //            pid.OutPut = pid.Pout + pid.Iout+ pid.Dout;
        //        else
        //            pid.OutPut = LIMIT((pid.Pout + pid.Iout+ pid.Dout), -pid.OutPutMax, pid.OutPutMax);
        //        return pid.OutPut;
        //    }
    }
}
