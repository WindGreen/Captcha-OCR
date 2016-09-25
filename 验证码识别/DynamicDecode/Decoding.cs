using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using DecodeHelper;

namespace DynamicDecode
{
    public class Decoding
    {
        /*
         * 基于笔迹的判断以及推测
         * 后期加入色差的判断
        */

        public Color dColor = Color.Red;//描绘用d颜色
        int pWidth = 3;//笔记宽度

        public string Decode(Bitmap bitmap)
        {
            JWDecode jwdc = new JWDecode();
            bitmap= jwdc.ChangeToBinary(bitmap);
            bitmap = jwdc.RemoveNoise(bitmap);

            List<Bitmap> bitmapList = jwdc.CutBitmap(bitmap);
            List<Bitmap> newList = jwdc.SaveCharacter(ref bitmapList);

            //算出笔迹的平均宽度 //先默认3 ^^ 懒得求了
            pWidth = 3;

            for (int i = 0; i < newList.Count; i++)
            {
                Amplification(newList[i]);
            }

            return "";
        }

        private Bitmap Amplification(Bitmap bitmap)
        {
            int w=bitmap.Width * 8;
            int h=bitmap.Height * 8;
            Bitmap newBitmap = new Bitmap(w,h );

            Graphics g = Graphics.FromImage(newBitmap);
            //设置高质量插值法   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //消除锯齿 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawImage(bitmap, new Rectangle(0, 0, w, h), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            newBitmap.Save(System.Environment.CurrentDirectory + "\\test\\放大" + DateTime.Now.Ticks.ToString() + ".bmp");
            g.Dispose();

            return newBitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        private void Drawing(Bitmap bitmap)
        {
            //找起点
            Point startP = new Point(-1, -1);
            //先找到靠左的第一个点
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    if (bitmap.GetPixel(x, y).R == 0)
                    {
                        startP.X = x;
                        startP.Y = y;

                        //break;
                        y = bitmap.Height;
                        x = bitmap.Width;
                    }
                }
            }




        }
       
    }
}
