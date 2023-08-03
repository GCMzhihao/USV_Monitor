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
        public TChartScaling(TChart tchart)//构造函数
        {
            tchart.MouseDown += new MouseEventHandler(TChart_MouseDown);
            tchart.MouseMove += new MouseEventHandler(TChart_MouseMove);
            tchart.MouseUp += new MouseEventHandler(TChart_MouseUp);
            tchart.MouseWheel += new MouseEventHandler(TChart_MouseWheel);
            tchart.MouseDoubleClick += new MouseEventHandler(TChart_MouseDoubleClick);
            
            MousePointLast = new Dot();
            MousePointNext = new Dot();
            D_screen = new Dot();
            P_ScreenToAxis = new Dot();

            tchart.Axes.Bottom.SetMinMax(-100, 100);
            tchart.Axes.Left.SetMinMax(-100, 100);
        }

        public void TChart_MouseDown(object sender, MouseEventArgs e)//鼠标被按下
        {
            TChart tchart;
            tchart = (TChart)sender;
            MousePointLast.x = e.X;
            MousePointLast.y = e.Y;
            LeftMinimum = tchart.Axes.Left.Minimum;
            LeftMaximum = tchart.Axes.Left.Maximum;
            BottomMinimum = tchart.Axes.Bottom.Minimum;
            BottomMaximum = tchart.Axes.Bottom.Maximum;
            D_screen.x = tchart.Axes.Bottom.CalcPosValue(tchart.Axes.Bottom.Maximum) - tchart.Axes.Bottom.CalcPosValue(tchart.Axes.Bottom.Minimum);
            D_screen.y = tchart.Axes.Left.CalcPosValue(tchart.Axes.Left.Minimum) - tchart.Axes.Left.CalcPosValue(tchart.Axes.Left.Maximum);
            if (D_screen.x != 0 && D_screen.y != 0)
            {
                P_ScreenToAxis.x = (BottomMaximum - BottomMinimum) / D_screen.x;
                P_ScreenToAxis.y = (LeftMaximum - LeftMinimum) / D_screen.y;
            }
            tchart.Cursor = Cursors.Hand;
            TChartMouseDown = true;
        }
        public void TChart_MouseMove(object sender, MouseEventArgs e)
        {
            TChart tchart;
            double err_x, err_y;
            Dot D_axis;
            tchart = (TChart)sender;

            if (TChartMouseDown == false)
                return;
            MousePointNext.x = e.X;
            MousePointNext.y = e.Y;
            err_x = (MousePointNext.x - MousePointLast.x);
            err_y = (MousePointNext.y - MousePointLast.y);
            D_axis.x = -err_x * P_ScreenToAxis.x;
            D_axis.y = err_y * P_ScreenToAxis.y;
            tchart.Axes.Bottom.SetMinMax(BottomMinimum + D_axis.x, BottomMaximum + D_axis.x);
            tchart.Axes.Left.SetMinMax(LeftMinimum + D_axis.y, LeftMaximum + D_axis.y);
        }
        public void TChart_MouseUp(object sender, MouseEventArgs e)
        {
            TChart tchart;
            tchart = (TChart)sender;
            TChartMouseDown = false;
            tchart.Cursor = Cursors.Arrow;
        }
        public void TChart_MouseWheel(object sender, MouseEventArgs e)
        {
            TChart tchart;
            tchart = (TChart)sender;
            double zoom_scale = 0.1;
            Dot Distance;
            Dot mouse_pos;
            Dot center;
            Dot bias;
            Distance.x = (tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum) * zoom_scale;
            Distance.y = (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum) * zoom_scale;
            center.x = (tchart.Axes.Bottom.Maximum + tchart.Axes.Bottom.Minimum) / 2;
            center.y = (tchart.Axes.Left.Maximum + tchart.Axes.Left.Minimum) / 2;
            mouse_pos.x = tchart.Axes.Bottom.CalcPosPoint(e.X);
            mouse_pos.y = tchart.Axes.Left.CalcPosPoint(e.Y);
            bias.x = (center.x - mouse_pos.x) * zoom_scale * 2;
            bias.y = (center.y - mouse_pos.y) * zoom_scale * 2;
            if (e.Delta > 0)//放大
            {
                tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.Minimum + Distance.x - bias.x, tchart.Axes.Bottom.Maximum - Distance.x - bias.x);
                tchart.Axes.Left.SetMinMax(tchart.Axes.Left.Minimum + Distance.y - bias.y, tchart.Axes.Left.Maximum - Distance.y - bias.y);
            }
            else
            {
                tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.Minimum - Distance.x + bias.x, tchart.Axes.Bottom.Maximum + Distance.x + bias.x);
                tchart.Axes.Left.SetMinMax(tchart.Axes.Left.Minimum - Distance.y + bias.y, tchart.Axes.Left.Maximum + Distance.y + bias.y);
            }
        }
        public void TChart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double x_scale,y_scale,max_scale;
            double x_distance, y_distance;
            double x_mid, y_mid;
            TChart tchart;
            tchart = (TChart)sender;
            x_scale = (tchart.Axes.Bottom.MaxXValue - tchart.Axes.Bottom.MinXValue) / Math.Abs(tchart.Axes.Bottom.IStartPos - tchart.Axes.Bottom.IEndPos);
            y_scale = (tchart.Axes.Left.MaxYValue - tchart.Axes.Left.MinYValue) / Math.Abs(tchart.Axes.Left.IStartPos - tchart.Axes.Left.IEndPos);
            if (x_scale > y_scale)
                max_scale = x_scale;
            else
                max_scale = y_scale;
            x_distance = max_scale * Math.Abs(tchart.Axes.Bottom.IStartPos - tchart.Axes.Bottom.IEndPos) * 1.1;
            y_distance = max_scale * Math.Abs(tchart.Axes.Left.IStartPos - tchart.Axes.Left.IEndPos) * 1.1;
            x_mid = (tchart.Axes.Bottom.MaxXValue + tchart.Axes.Bottom.MinXValue) / 2;
            y_mid = (tchart.Axes.Left.MaxYValue + tchart.Axes.Left.MinYValue) / 2;
            if(tchart.Axes.Bottom.MaxXValue == tchart.Axes.Bottom.MinXValue|| tchart.Axes.Left.MaxYValue ==tchart.Axes.Left.MinYValue)
            {
                if(tchart.Axes.Left.MaxYValue == tchart.Axes.Left.MinYValue)
                    tchart.Axes.Left.SetMinMax(tchart.Axes.Left.MinYValue - (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum) / 2,
                            tchart.Axes.Left.MaxYValue + (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum) / 2);
                if(tchart.Axes.Bottom.MaxXValue == tchart.Axes.Bottom.MinXValue)
                    tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.MinYValue - (tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum) / 2,
                       tchart.Axes.Bottom.MaxYValue + (tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum) / 2);
            }
            else
            {
                tchart.Axes.Bottom.SetMinMax(x_mid - x_distance / 2, x_mid + x_distance / 2);
                tchart.Axes.Left.SetMinMax(y_mid - y_distance / 2, y_mid + y_distance / 2);
            }
            
        }
    }
}
