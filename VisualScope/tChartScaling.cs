using Steema.TeeChart;
using Steema.TeeChart.Drawing;
using System.Windows.Forms;
using System;
namespace 地面站
{
    /// <summary>
    /// 功能：tchart等比缩放
    /// 1.双击tchart等比缩放至合适大小,并开启自动缩放
    /// 2.单击或者滑动取消自动缩放
    /// 3.滑动滑轮会以鼠标位置为中心等比缩放
    /// 4.按住鼠标左键或右键可以拖动图表
    /// </summary>
    class TChartScaling
    {
        //private bool TChartAutoScaling = true;//XY轴自动等比例缩放标志
        private bool TChartMouseDown = false;//鼠标按下标志
        public struct Dot
        {
            public double x;
            public double y;
        }
        public Dot MousePointLast;
        public Dot MousePointNext;
        public Dot D_screen, P_ScreenToAxis;
        double LeftMinimum, LeftMaximum;
        double BottomMinimum, BottomMaximum;
        TChart chart;
        public TChartScaling(TChart tchart)//构造函数
        {
            chart = tchart;
            chart.MouseDown += new MouseEventHandler(TChart_MouseDown);
            chart.MouseMove += new MouseEventHandler(TChart_MouseMove);
            chart.MouseUp += new MouseEventHandler(TChart_MouseUp);
            chart.MouseWheel += new MouseEventHandler(TChart_MouseWheel);
            chart.MouseDoubleClick += new MouseEventHandler(TChart_MouseDoubleClick);
            
            MousePointLast = new Dot();
            MousePointNext = new Dot();
            D_screen = new Dot();
            P_ScreenToAxis = new Dot();

            chart.Axes.Bottom.SetMinMax(-100, 100);
            chart.Axes.Left.SetMinMax(-100, 100);
        }

        public void TChart_MouseDown(object sender, MouseEventArgs e)//鼠标被按下
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                MousePointLast.x = e.X;
                MousePointLast.y = e.Y;
                LeftMinimum = chart.Axes.Left.Minimum;
                LeftMaximum = chart.Axes.Left.Maximum;
                BottomMinimum = chart.Axes.Bottom.Minimum;
                BottomMaximum = chart.Axes.Bottom.Maximum;
                D_screen.x = chart.Axes.Bottom.CalcPosValue(chart.Axes.Bottom.Maximum) - chart.Axes.Bottom.CalcPosValue(chart.Axes.Bottom.Minimum);
                D_screen.y = chart.Axes.Left.CalcPosValue(chart.Axes.Left.Minimum) - chart.Axes.Left.CalcPosValue(chart.Axes.Left.Maximum);
                if (D_screen.x != 0 && D_screen.y != 0)
                {
                    P_ScreenToAxis.x = (BottomMaximum - BottomMinimum) / D_screen.x;
                    P_ScreenToAxis.y = (LeftMaximum - LeftMinimum) / D_screen.y;
                }
                chart.Cursor = Cursors.Hand;
                TChartMouseDown = true;
            }
        }
        public void TChart_MouseMove(object sender, MouseEventArgs e)
        {
            double err_x, err_y;
            Dot D_axis;

            if (TChartMouseDown == false)
                return;
            MousePointNext.x = e.X;
            MousePointNext.y = e.Y;
            err_x = (MousePointNext.x - MousePointLast.x);
            err_y = (MousePointNext.y - MousePointLast.y);
            D_axis.x = -err_x * P_ScreenToAxis.x;
            D_axis.y = err_y * P_ScreenToAxis.y;
            chart.Axes.Bottom.SetMinMax(BottomMinimum + D_axis.x, BottomMaximum + D_axis.x);
            chart.Axes.Left.SetMinMax(LeftMinimum + D_axis.y, LeftMaximum + D_axis.y);
        }
        public void TChart_MouseUp(object sender, MouseEventArgs e)
        {
            TChartMouseDown = false;
            chart.Cursor = Cursors.Arrow;
        }
        public void TChart_MouseWheel(object sender, MouseEventArgs e)
        {
            double zoom_scale = 0.1;
            Dot Distance;
            Dot mouse_pos;
            Dot center;
            Dot bias;
            Distance.x = (chart.Axes.Bottom.Maximum - chart.Axes.Bottom.Minimum) * zoom_scale;
            Distance.y = (chart.Axes.Left.Maximum - chart.Axes.Left.Minimum) * zoom_scale;
            center.x = (chart.Axes.Bottom.Maximum + chart.Axes.Bottom.Minimum) / 2;
            center.y = (chart.Axes.Left.Maximum + chart.Axes.Left.Minimum) / 2;
            mouse_pos.x = chart.Axes.Bottom.CalcPosPoint(e.X);
            mouse_pos.y = chart.Axes.Left.CalcPosPoint(e.Y);
            bias.x = (center.x - mouse_pos.x) * zoom_scale * 2;
            bias.y = (center.y - mouse_pos.y) * zoom_scale * 2;
            if (e.Delta > 0)//放大
            {
                chart.Axes.Bottom.SetMinMax(chart.Axes.Bottom.Minimum + Distance.x - bias.x, chart.Axes.Bottom.Maximum - Distance.x - bias.x);
                chart.Axes.Left.SetMinMax(chart.Axes.Left.Minimum + Distance.y - bias.y, chart.Axes.Left.Maximum - Distance.y - bias.y);
            }
            else
            {
                chart.Axes.Bottom.SetMinMax(chart.Axes.Bottom.Minimum - Distance.x + bias.x, chart.Axes.Bottom.Maximum + Distance.x + bias.x);
                chart.Axes.Left.SetMinMax(chart.Axes.Left.Minimum - Distance.y + bias.y, chart.Axes.Left.Maximum + Distance.y + bias.y);
            }
        }
        public void TChart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double x_scale,y_scale,max_scale;
            double x_distance, y_distance;
            double x_mid, y_mid;
            x_scale = (chart.Axes.Bottom.MaxXValue - chart.Axes.Bottom.MinXValue) / Math.Abs(chart.Axes.Bottom.IStartPos - chart.Axes.Bottom.IEndPos);
            y_scale = (chart.Axes.Left.MaxYValue - chart.Axes.Left.MinYValue) / Math.Abs(chart.Axes.Left.IStartPos - chart.Axes.Left.IEndPos);
            if (x_scale > y_scale)
                max_scale = x_scale;
            else
                max_scale = y_scale;
            x_distance = max_scale * Math.Abs(chart.Axes.Bottom.IStartPos - chart.Axes.Bottom.IEndPos) * 1.1;
            y_distance = max_scale * Math.Abs(chart.Axes.Left.IStartPos - chart.Axes.Left.IEndPos) * 1.1;
            x_mid = (chart.Axes.Bottom.MaxXValue + chart.Axes.Bottom.MinXValue) / 2;
            y_mid = (chart.Axes.Left.MaxYValue + chart.Axes.Left.MinYValue) / 2;
            if(chart.Axes.Bottom.MaxXValue == chart.Axes.Bottom.MinXValue|| chart.Axes.Left.MaxYValue ==chart.Axes.Left.MinYValue)
            {
                if(chart.Axes.Left.MaxYValue == chart.Axes.Left.MinYValue)
                    chart.Axes.Left.SetMinMax(chart.Axes.Left.MinYValue - (chart.Axes.Left.Maximum - chart.Axes.Left.Minimum) / 2,
                            chart.Axes.Left.MaxYValue + (chart.Axes.Left.Maximum - chart.Axes.Left.Minimum) / 2);
                if(chart.Axes.Bottom.MaxXValue == chart.Axes.Bottom.MinXValue)
                    chart.Axes.Bottom.SetMinMax(chart.Axes.Bottom.MinYValue - (chart.Axes.Bottom.Maximum - chart.Axes.Bottom.Minimum) / 2,
                       chart.Axes.Bottom.MaxYValue + (chart.Axes.Bottom.Maximum - chart.Axes.Bottom.Minimum) / 2);
            }
            else
            {
                chart.Axes.Bottom.SetMinMax(x_mid - x_distance / 2, x_mid + x_distance / 2);
                chart.Axes.Left.SetMinMax(y_mid - y_distance / 2, y_mid + y_distance / 2);
            }
            
        }
    }
}
