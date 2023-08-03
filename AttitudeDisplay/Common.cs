using System.Drawing;
using System.Drawing.Drawing2D;
namespace AttitudeDisplay
{
    class Common
    {
        /// <summary>
        /// 重合两张图片 
        /// </summary>
        /// <param name="btm1">图片1</param>
        /// <param name="btm2">图片2</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Bitmap Overlap(Bitmap btm1, Bitmap btm2, int x, int y, int w, int h)
        {
            Bitmap image = new Bitmap(btm1);
            Bitmap hi = new Bitmap(btm2);
            Graphics g = Graphics.FromImage(hi);
            g.DrawImage(image, new Rectangle(x, y, w, h));
            return hi;
        }
        /// <summary>
        /// 旋转一张图片
        /// </summary>
        /// <param name="image">bmp位图</param>
        /// <param name="angle">旋转角</param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// <returns></returns>
        public static Bitmap RotateBmp(Bitmap bitmp, double angle, int width, int height)
        {
            //创建一个新图片
            Bitmap pointImage = new Bitmap(width, height);
            Graphics gPoint = System.Drawing.Graphics.FromImage(pointImage);
            //使用双线性插值
            gPoint.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            //抗锯齿
            gPoint.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Rectangle rectPoint = new Rectangle(0, 0, width, height);
            gPoint.TranslateTransform(width / 2, height / 2);
            //旋转图片
            gPoint.RotateTransform((float)angle);
            //恢复图像在水平和垂直方向的平移 
            gPoint.TranslateTransform(-width / 2, -height / 2);
            //在旋转后的容器中加载原始图片
            gPoint.DrawImage(bitmp, rectPoint);
            //重至绘图的所有变换  
            gPoint.ResetTransform();
            return pointImage;
        }
        public static Bitmap CutEllipse(Image img, int width, int height, double ratio)
        {
            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);
            //创建截图路径（类似Ps里的路径）
            GraphicsPath gpath = new GraphicsPath
            {
                FillMode = 0
            };
            gpath.AddEllipse((float)(width * ratio), (float)(height * ratio), (float)(width * (1 - ratio * 2)), (float)(height * (1 - ratio * 2)));
            //设置画板的截图路径
            g.SetClip(gpath);
            //对图片进行截图
            g.DrawImage(img, 0, 0);
            return bm;
        }
    }
}
