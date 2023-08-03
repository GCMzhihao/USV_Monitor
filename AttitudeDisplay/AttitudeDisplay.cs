using System;
using System.Drawing;
using System.Windows.Forms;

namespace AttitudeDisplay
{
    public partial class AttitudeDisplay: UserControl
    {
        private Image imgtmp, imgtmp1;
        private Bitmap bmp_scale, bitmp, bm, bm1;
        private double att_rol, att_pit, att_yaw;
        public AttitudeDisplay()
        {
            InitializeComponent();
            bmp_scale = new Bitmap(this.Width, this.Height);
        }


        private void Timer_CLR_Tick(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void Timer_ATT_Tick(object sender, EventArgs e)
        {
            Attitude(att_rol, att_pit, att_yaw);
        }

        readonly double draw_ellipse_scale = 0.15;//画圆的起点坐标占控件宽度的比例
        /// <summary>
        /// 地平仪刻度划线函数
        /// </summary>
        /// <returns></returns>
        private Bitmap Hori_Line()
        {
            bitmp = new Bitmap(this.Width, this.Width);
            Graphics gscale = Graphics.FromImage(bitmp);
            #region 准心绘线
            Pen p1 = new Pen(Color.Cyan, 2);
            gscale.DrawLine(p1, (float)(this.Width * 22 / 70), (float)(this.Width / 2), (float)(this.Width * 48 / 70), (float)(this.Width / 2));
            gscale.DrawLine(p1, (float)(this.Width / 2), (float)(this.Width * (0.5 - 0.02)), (float)(this.Width / 2), (float)(this.Width / 2));
            #endregion
            #region 滚转刻度线
            int i, i1, j, j1, k;
            Font myfont = new Font("宋体", 12, FontStyle.Regular);
            Brush bush = new SolidBrush(Color.Black);
            for (k = 0; k < 72; k++)
            {
                i = Convert.ToInt32(this.Width * (0.5 - draw_ellipse_scale) * Math.Cos(k * Math.PI / 36) + this.Width / 2);//x坐标起点
                j = Convert.ToInt32(this.Width * (0.5 - draw_ellipse_scale) * Math.Sin(k * Math.PI / 36) + this.Width / 2);//y坐标起点
                if (k % 6 == 0)//30度一个长轴
                {
                    i1 = Convert.ToInt32(this.Width * (0.45 - draw_ellipse_scale) * Math.Cos(k * Math.PI / 36) + this.Width / 2);
                    j1 = Convert.ToInt32(this.Width * (0.45 - draw_ellipse_scale) * Math.Sin(k * Math.PI / 36) + this.Width / 2);
                    if (k % 18 == 0)//每隔90度标示一下刻度值
                    {
                        int tmp;
                        tmp = k * 5;
                        if (tmp > 180)
                            tmp -= 360;
                        if (tmp == 0)
                            gscale.DrawString("90", myfont, bush, i1 - 24, j1 - 8);
                        else if (tmp == 90)
                            gscale.DrawString("180", myfont, bush, i1 - 18, j1 - 18);
                        else if (tmp == 180)
                            gscale.DrawString("-90", myfont, bush, i1, j1 - 8);
                        else if (tmp == -90)
                            gscale.DrawString("0", myfont, bush, i1 - 8, j1);
                    }
                }
                else//5度一个短轴
                {
                    i1 = Convert.ToInt32(this.Width * (0.485 - draw_ellipse_scale) * Math.Cos(k * Math.PI / 36) + this.Width / 2);
                    j1 = Convert.ToInt32(this.Width * (0.485 - draw_ellipse_scale) * Math.Sin(k * Math.PI / 36) + this.Width / 2);
                }
                gscale.DrawLine(Pens.Black, i, j, i1, j1);
            }
            #endregion
            //p1.Dispose();
            //gscale.Dispose();
            //myfont.Dispose();
            //bush.Dispose();

            return bitmp;
        }
        public void AttitudeShow(double roll, double pitch, double yaw)
        {
            att_rol = roll;
            att_pit = pitch;
            att_yaw = yaw;
        }
        /// <summary>
        /// 姿态显示函数
        /// </summary>
        /// <param name="pitch">俯仰角 pitch 范围-90~90 度 </param>
        /// <param name="roll">滚动角 roll   范围-90~90 度</param>
        /// <param name="yaw">偏航角   范围-180~180 度</param>
        private void Attitude(double roll, double pitch, double yaw)
        {
            
            try
            {
                //1地平仪图像载入带平移
                int pic_position = Convert.ToInt32(pitch * this.Height / 90.67);
                //取得水平仪背景图--从ErrorImage中取得
                imgtmp = new Bitmap(AttitudeBox.ErrorImage);
                roll %= 360;
                Bitmap dsImage = new Bitmap(this.Width, this.Height);//目标位图
                Graphics g = Graphics.FromImage(dsImage);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;//双线性插值
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//抗锯齿
                Rectangle rect = new Rectangle(-this.Width / 2, -this.Height / 2 + pic_position, this.Width * 2, this.Height * 2);//计算偏移量
                g.TranslateTransform(this.Width / 2, this.Height / 2);//平移图形
                g.RotateTransform((float)roll);//旋转图形
                g.TranslateTransform(-this.Width / 2, -this.Height / 2);//恢复图像在水平和垂直方向的平移 
                g.DrawImage(imgtmp, rect);
                g.ResetTransform();//重至绘图的所有变换
                bm = dsImage;//保存旋转后的图片
                bm = Common.Overlap(bmp_scale, bm, 0, 0, this.Width, this.Height);//重合背景图和刻度盘
                #region 指针设计
                Bitmap point_bmp = new Bitmap(this.Width, this.Height);
                System.Drawing.Graphics gPoint = System.Drawing.Graphics.FromImage(point_bmp);
                //红色
                SolidBrush h = new SolidBrush(Color.Cyan);
                Point a = new Point(Convert.ToInt32(this.Width / 2), Convert.ToInt32(this.Height * (draw_ellipse_scale + 0.1)));
                Point b = new Point(a.X - Convert.ToInt32(this.Width * 0.02), a.Y + Convert.ToInt32(this.Height * 0.07));
                Point c = new Point(a.X, a.Y + Convert.ToInt32(this.Height * 0.05));
                Point d = new Point(a.X + Convert.ToInt32(this.Width * 0.02), a.Y + Convert.ToInt32(this.Height * 0.07));
                Point[] pointer = { a, b, c, d };
                //填充点所围的区域
                gPoint.FillPolygon(h, pointer);
                Bitmap aaa = Common.RotateBmp(point_bmp, roll, this.Width, this.Height);
                #endregion
                //重合【指针】与【背景图和刻度盘】
                bm = Common.Overlap(aaa, bm, 0, 0, this.Width, this.Height);
                bm = Common.CutEllipse(bm, this.Width, this.Height, draw_ellipse_scale);

                imgtmp1 = new Bitmap(AttitudeBox.InitialImage);//读取罗盘背景图
                Bitmap dsImage1 = new Bitmap(this.Width, this.Height);
                Graphics g1 = Graphics.FromImage(dsImage1);
                //双线性插值
                g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                //抗锯齿
                g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g1.TranslateTransform(this.Width / 2, this.Height / 2);
                g1.RotateTransform((float)-yaw);
                g1.TranslateTransform(-this.Width / 2, -this.Height / 2);
                g1.DrawImage(imgtmp1, new Rectangle(0, 0, this.Width, this.Height), new Rectangle(0, 0, AttitudeBox.InitialImage.Width, AttitudeBox.InitialImage.Height), GraphicsUnit.Pixel);
                g1.ResetTransform();
                bm1 = dsImage1;
                bm1 = Common.Overlap(bm, bm1, 0, 0, this.Width, this.Height);
                bm1 = Common.CutEllipse(bm1, this.Width, this.Height, 1.0);
                #region 罗盘指针设计
                Bitmap compass_point_img = new Bitmap(this.Width, this.Height);
                Graphics gCompPoint = Graphics.FromImage(compass_point_img);
                //红色
                SolidBrush sbr = new SolidBrush(Color.Cyan);
                ////填充点所围的区域
                Point p0 = new Point(Convert.ToInt32(this.Width / 2), Convert.ToInt32(this.Height * 0.04));
                Point p1 = new Point(p0.X - Convert.ToInt32(this.Width * 0.025), p0.Y - Convert.ToInt32(this.Height * 0.04));
                Point p2 = new Point(p0.X + Convert.ToInt32(this.Width * 0.025), p0.Y - Convert.ToInt32(this.Height * 0.04));
                Point[] compass_point = { p0, p1, p2 };
                //填充点所围的区域
                gCompPoint.FillPolygon(sbr, compass_point);
                #endregion
                bm1 = Common.Overlap(compass_point_img, bm1, 0, 0, this.Width, this.Height);
                AttitudeBox.Image = bm1;

                imgtmp.Dispose();
                imgtmp1.Dispose();
                bm.Dispose();

                //dsImage.Dispose();
                //g.Dispose();
                //point_bmp.Dispose();
                //aaa.Dispose();
                //dsImage1.Dispose();
                //g1.Dispose();
                //compass_point_img.Dispose();
                //gCompPoint.Dispose();
            }
            catch (Exception)
            {
                
            }
        }
        private void AttitudeBox_Resize(object sender, EventArgs e)
        {
            //画刻度盘
            bmp_scale = Hori_Line();
            AttitudeShow(0, 0, 0);
        }
    }
}
