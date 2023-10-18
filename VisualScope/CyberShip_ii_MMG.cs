using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 地面站
{
    public class CyberShip_ii_MMG
    {
        //动力学模型参数
        double m11 = 25.8;
        double m22 = 33.8;
        double m33 = 2.76;
        double d11 = 0.9257;
        double d22 = 2.8909;
        double d33 = 0.5;
        //运动学

        double u;
        double v;
        double r;
        public struct State
        {
            public double U, x, y, psi,course;
        }
        public State state;
        public CyberShip_ii_MMG()
        {
            state = new State();
            Clear(0,0,0);
        }
        public void Clear(double x0, double y0,double psi0)
        {
            state.x = x0;
            state.y = y0;
            state.psi = psi0;
            state.course = state.psi;
            state.U = 0;
            u = 0;
            v = 0.15;
            r = 0;
        }
        public void UpdateSpeed(double speed)
        { 
            u= speed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tau_u">推进器控制量</param>
        /// <param name="tau_r">舵机控制量</param>
        /// <param name="dT"></param>
        public void Calculate(double tau_u,double tau_r,double dT)
        {
            double du;
            double dv;
            double dr;
            double dx;
            double dy;

            du = (m22 * v * r - d11 * u + tau_u) / m11;
            dv = (-m11 * u * r - d22 * v) / m22;
            dr = ((m11 - m22) * u * v - d33 * r + tau_r) / m33;
            u += du * dT;
            v += dv * dT;
            r += dr * dT;
            
            state.psi += r * dT;
            dx = u * Math.Cos(state.psi) - v * Math.Sin(state.psi);
            dy = u * Math.Sin(state.psi) + v * Math.Cos(state.psi);


            state.U = Math.Sqrt(u * u + v * v);
            state.course = state.psi + Math.Atan2(v, u);
            state.x += dx * dT;
            state.y += dy * dT;

            while (state.psi >= Math.PI)
                state.psi -= Math.PI * 2;
            while (state.psi < -Math.PI)
                state.psi += Math.PI * 2;

            while (state.course >= Math.PI)
                state.course -= Math.PI * 2;
            while (state.course < -Math.PI)
                state.course += Math.PI * 2;
        }
        public void Calculate_Psi(double U,double beta,double tau_r, double dT)
        {
            double dx;
            double dy;
            double dr;
            u = U * Math.Cos(beta);
            v = U * Math.Sin(beta);

            dr = ((m11 - m22) * u * v - d33 * r + tau_r) / m33;

            r += dr * dT;

            state.psi += r * dT;
            dx = u * Math.Cos(state.psi) - v * Math.Sin(state.psi);
            dy = u * Math.Sin(state.psi) + v * Math.Cos(state.psi);

            state.course = state.psi + beta;
            state.x += dx * dT;
            state.y += dy * dT;

            while (state.psi >= Math.PI)
                state.psi -= Math.PI * 2;
            while (state.psi < -Math.PI)
                state.psi += Math.PI * 2;

            while (state.course >= Math.PI)
                state.course -= Math.PI * 2;
            while (state.course < -Math.PI)
                state.course += Math.PI * 2;
        }
    }
}
