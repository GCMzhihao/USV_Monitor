using Steema.TeeChart;
using Steema.TeeChart.Drawing;
using System.Windows.Forms;
using Steema.TeeChart.Styles;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace 地面站
{
    public class TChartZoom
    {
        private bool MouseLeftDown = false;
        private bool MouseRightDown = false;
        private bool tChartIsYAutoShowed = true;
        private bool tChartIsXAutoShowed = true;
        private bool ClickLegendFlag = false;
        
        private List<TChart> tcharts =new List<TChart>();
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
        public TChartZoom(TChart tchart)//构造函数
        {
            tcharts.Add(tchart);
            MousePointLast = new Dot();
            MousePointNext = new Dot();
            D_screen = new Dot();
            P_ScreenToAxis = new Dot();
            tchart.MouseWheel += new MouseEventHandler(TChart_MouseWheel);
            tchart.MouseDown += new MouseEventHandler(TChart_MouseDown);
            tchart.MouseMove += new MouseEventHandler(TChart_MouseMove);
            tchart.MouseDoubleClick += new MouseEventHandler(TChart_MouseDoubleClick);
            tchart.MouseUp += new MouseEventHandler(TChart_MouseUp);
            //tchart.AfterDraw += new PaintChartEventHandler(TChart_DataAdd);
            tchart.ClickLegend += new MouseEventHandler(TChart_ClickLegend);

            foreach (Series s in tchart.Series)
            {
                s.AfterDrawValues += TChart_DataAdd;
            }

            tchart.Axes.Bottom.SetMinMax(0, 1000);
            tchart.Axes.Left.SetMinMax(-500, 500);
            tchart.Axes.Left.Grid.DrawEvery = 1;
            tchart.Axes.Bottom.Grid.DrawEvery = 1;
        }
        private void TChart_MouseDown(object sender, MouseEventArgs e)//鼠标被按下
        {
            TChart tchart;
            tchart = (TChart)sender;
            if (ClickLegendFlag == true)//tchart被单击
            {
                tChartIsXAutoShowed = false;
                tChartIsYAutoShowed = true;
                ClickLegendFlag = false;
                return;
            }
            tChartIsXAutoShowed = false;
            tChartIsYAutoShowed = false;

            if (e.Button == MouseButtons.Left)//按下了左键
                MouseLeftDown = true;
            if (e.Button == MouseButtons.Right)
                MouseRightDown = true;
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
        }
        private void TChart_MouseMove(object sender, MouseEventArgs e)
        {
            TChart tchart;
            tchart = (TChart)sender;
            double err_x, err_y;
            Dot D_axis;
            MousePointNext.x = e.X;
            MousePointNext.y = e.Y;
            err_x = (MousePointNext.x - MousePointLast.x);
            err_y = (MousePointNext.y - MousePointLast.y);
            D_axis.x = -err_x * P_ScreenToAxis.x;
            D_axis.y = err_y * P_ScreenToAxis.y;
            if (MouseLeftDown == true)
            {
                tchart.Axes.Bottom.SetMinMax(BottomMinimum + D_axis.x, BottomMaximum + D_axis.x);
                tchart.Axes.Left.SetMinMax(LeftMinimum + D_axis.y, LeftMaximum + D_axis.y);
            }
            else if (MouseRightDown == true)
            {
                tchart.Axes.Bottom.SetMinMax(BottomMinimum - D_axis.x, BottomMaximum + D_axis.x);
                tchart.Axes.Left.SetMinMax(LeftMinimum - D_axis.y, LeftMaximum + D_axis.y);
            }
        }
        private void TChart_MouseUp(object sender, MouseEventArgs e)
        {
            TChart tchart;
            tchart = (TChart)sender;
            MouseLeftDown = false;
            MouseRightDown = false;
            tchart.Cursor = Cursors.Arrow;
        }
        private void TChart_MouseWheel(object sender, MouseEventArgs e)
        {
            TChart tchart;
            tchart = (TChart)sender;
            double zoom_scale = 0.1;
            Dot Distance;
            Dot mouse_pos;
            Dot center;
            Dot bias;
            tChartIsXAutoShowed = false;
            tChartIsYAutoShowed = false;
            Distance.x = (tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum)* zoom_scale;
            Distance.y = (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum) * zoom_scale;
            center.x = (tchart.Axes.Bottom.Maximum + tchart.Axes.Bottom.Minimum)/2;
            center.y = (tchart.Axes.Left.Maximum + tchart.Axes.Left.Minimum)/2;
            mouse_pos.x = tchart.Axes.Bottom.CalcPosPoint(e.X);
            mouse_pos.y = tchart.Axes.Left.CalcPosPoint(e.Y);
            bias.x = (center.x - mouse_pos.x) * zoom_scale*2;
            bias.y = (center.y - mouse_pos.y) * zoom_scale*2;
            if (e.Delta > 0)//放大
            {
                tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.Minimum + Distance.x - bias.x, tchart.Axes.Bottom.Maximum - Distance.x - bias.x);
                tchart.Axes.Left.SetMinMax(tchart.Axes.Left.Minimum + Distance.y -bias.y, tchart.Axes.Left.Maximum - Distance.y - bias.y);
            }
            else
            {
                tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.Minimum - Distance.x + bias.x, tchart.Axes.Bottom.Maximum + Distance.x + bias.x);
                tchart.Axes.Left.SetMinMax(tchart.Axes.Left.Minimum - Distance.y + bias.y, tchart.Axes.Left.Maximum + Distance.y + bias.y);
            }
        }
        private void TChart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TChart tchart;

            tChartIsXAutoShowed = true;//X自动跟踪
            tChartIsYAutoShowed = true;//Y自动跟踪

            tchart = (TChart)sender;
            if (tChartIsXAutoShowed == true)//X自动跟踪
            {
                if (tchart.Axes.Bottom.MaxXValue - tchart.Axes.Bottom.MinXValue < tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum)
                    tchart.Axes.Bottom.SetMinMax(0, tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum);
                else
                    tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.MaxXValue - (tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum),
                                                     tchart.Axes.Bottom.MaxXValue);
            }
            if (tChartIsYAutoShowed == true)//Y自动跟踪
            {
                if(tchart.Axes.Left.MinYValue== tchart.Axes.Left.MaxYValue)
                {
                    tchart.Axes.Left.SetMinMax(tchart.Axes.Left.MinYValue-(tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum)/2,
                        tchart.Axes.Left.MaxYValue+ (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum)/2);
                }
                else
                    tchart.Axes.Left.SetMinMax(tchart.Axes.Left.MinYValue, tchart.Axes.Left.MaxYValue);
            }

        }
        private void TChart_DataAdd(object sender, Graphics3D g)
        {
            TChart tchart;
            for(int i =0;i< tcharts.Count;i++)
            {
                foreach (Series s in tcharts[i].Series)
                {
                    if (s.ToString() == ((Series)sender).ToString())
                    {
                        tchart = tcharts[i];
                        if ((int)tchart.Axes.Bottom.Maximum == (int)tchart.Axes.Bottom.MaxXValue)
                        {
                            tChartIsXAutoShowed = true;
                        }
                        if (tChartIsXAutoShowed == true)//X自动跟踪
                        {
                            if (tchart.Axes.Bottom.MaxXValue - tchart.Axes.Bottom.MinXValue < tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum)
                                tchart.Axes.Bottom.SetMinMax(0, tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum);
                            else
                                tchart.Axes.Bottom.SetMinMax(tchart.Axes.Bottom.MaxXValue - (tchart.Axes.Bottom.Maximum - tchart.Axes.Bottom.Minimum),
                                                                 tchart.Axes.Bottom.MaxXValue);
                        }
                        if (tChartIsYAutoShowed == true)//Y自动跟踪
                        {
                            if (tchart.Axes.Left.MinYValue == tchart.Axes.Left.MaxYValue)
                            {
                                tchart.Axes.Left.SetMinMax(tchart.Axes.Left.MinYValue - (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum)/2,
                                    tchart.Axes.Left.MaxYValue + (tchart.Axes.Left.Maximum - tchart.Axes.Left.Minimum)/2);
                            }
                            else
                                tchart.Axes.Left.SetMinMax(tchart.Axes.Left.MinYValue, tchart.Axes.Left.MaxYValue);
                        }
                        return;
                    }    
                }
            }



        }
        private void TChart_ClickLegend(object sender, MouseEventArgs e)
        {
            ClickLegendFlag = true;
        }
    }
}
