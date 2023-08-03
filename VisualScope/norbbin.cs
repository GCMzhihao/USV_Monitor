using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 地面站
{
    class Norbbin//输入舵角，通过norbbin模型输出艏向角
    {
        double psi_deriv;
        double r;
        double r_deriv;
        readonly double gamma;
        readonly double k;
        readonly double alpha;
        double delta_deriv;
        double delta_m;
        double delta_m_deriv;
        readonly double xi;
        readonly double w;
        readonly double k_delta;
        double delta_r;//设定舵角
        public State state;
        double beta;
        double U;
        public struct State
        {
            public double delta;//实时舵角
            public double psi;//实时艏向角
            public double x;//实时坐标
            public double y;//实时坐标
        }
        public Norbbin(double x,double y)//蓝信号Norbbin模型参数
        {
            gamma = 0.332;
            k = 0.707;
            alpha = 0.001;
            xi = 0.811;
            w = 0.958;
            k_delta = 0.923;
            state = new State();
            Clear(x,y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">设定舵角</param>
        /// <param name="b">漂角</param>
        /// <param name="u">船速</param>
        public void UpdateData(double d, double b,double u)
        {
            delta_r = d * Math.PI / 180;
            beta = b;
            U = u;
        }
        public void Clear(double x,double y)
        {
            state.x = x;
            state.y = y;
            delta_m = 0;
            state.delta = 0;
            r = 0;
            state.psi = 0;
        }
        public void Calculate(double T)
        {
            delta_m_deriv = -2 * xi * w * delta_m - w * w * state.delta + k_delta * w * w * delta_r;
            delta_m += delta_m_deriv * T;
            delta_deriv = delta_m;
            state.delta += delta_deriv * T;
            r_deriv = -r / gamma - alpha / gamma * r * r * r + k / gamma * state.delta;
            r += r_deriv * T;
            psi_deriv = r;
            state.psi += psi_deriv * T;
            state.x += U * Math.Cos(state.psi + beta) * T;
            state.y += U * Math.Sin(state.psi + beta) * T;
        }
    }
}
