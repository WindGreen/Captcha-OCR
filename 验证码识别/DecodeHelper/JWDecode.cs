using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace DecodeHelper
{
    public class JWDecode
    {
        static Color WHITE = Color.FromArgb(255, 255, 255, 255);
        static Color BLACK = Color.FromArgb(255, 0, 0, 0);

        public string Decode(Bitmap bitmap, string modelPath)
        {
            bitmap.Save(System.Environment.CurrentDirectory + "\\temp\\" + DateTime.Now.Ticks.ToString() + ".bmp");
            //二值化
            bitmap = ChangeToBinary(bitmap);
          
            //去噪点
            bitmap = RemoveNoise(bitmap);

            //抠图
            List<Bitmap> bitmapList = CutBitmap(bitmap);
            
            for(int i=0;i<bitmapList.Count;++i)
                bitmapList[i].Save(System.Environment.CurrentDirectory + "\\test\\" + DateTime.Now.Ticks.ToString() + ".bmp");
            
            //取字符
            List<Bitmap> newList = SaveCharacter(ref bitmapList);


            // 对比模板
            string result = Compare(ref newList, modelPath);

            return result;
        }

        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Bitmap ChangeToBinary(Bitmap bitmap)
        {
            //统计
            List<Color> excColorList = new List<Color>();
            excColorList.Add(WHITE);
            Color maxColor = GetMaxColor(bitmap,excColorList);
            excColorList.Add(maxColor);
            Color secColor = GetMaxColor(bitmap,excColorList);
            excColorList.Add(secColor);
            Color thiColor = GetMaxColor(bitmap, excColorList);
            ///二值化
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (c == maxColor || c==secColor || c==thiColor)
                    {
                        bitmap.SetPixel(x, y, BLACK);
                    }
                    else bitmap.SetPixel(x, y, WHITE);
                }
            }

            bitmap.Save(System.Environment.CurrentDirectory + "\\temp\\二值化" + DateTime.Now.Ticks.ToString() + ".bmp");
            return bitmap;
        }

        /// <summary>
        /// 去噪点
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Bitmap RemoveNoise(Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (c.R == 0)//如果为黑色 也就0
                    {
                        if (x > 0)
                        {
                            if (y > 0)
                            {
                                if (bitmap.GetPixel(x - 1, y - 1).R == 0) continue;

                                if (bitmap.GetPixel(x, y - 1).R == 0) continue;

                                if (x < bitmap.Width - 1)
                                {
                                    if (bitmap.GetPixel(x + 1, y - 1).R == 0) continue;
                                }
                            }

                            if (bitmap.GetPixel(x - 1, y).R == 0) continue;

                            if (y < bitmap.Height - 1)
                            {
                                if (bitmap.GetPixel(x - 1, y + 1).R == 0) continue;
                            }
                        }

                        if (y < bitmap.Height - 1)
                        {
                            if (bitmap.GetPixel(x + 1, y + 1).R == 0) continue;
                        }

                        if (x < bitmap.Width - 1)
                        {
                            if (bitmap.GetPixel(x + 1, y).R == 0) continue;
                        }

                        if (y < bitmap.Height - 1)
                        {
                            if (bitmap.GetPixel(x, y + 1).R == 0) continue;
                        }

                        bitmap.SetPixel(x, y, Color.FromArgb(c.A, 255, 255, 255));
                    }
                }
            }

            bitmap.Save(System.Environment.CurrentDirectory+ "\\temp\\去噪点" + DateTime.Now.Ticks.ToString() + ".bmp");
            return bitmap;
        }

        /// <summary>
        /// 对比模板
        /// </summary>
        /// <param name="bitmapList"></param>
        /// <param name="modelPath"></param>
        /// <returns></returns>
        public string Compare(ref List<Bitmap> bitmapList, string modelPath)
        {
            Dictionary<string, Bitmap> chList = new Dictionary<string, Bitmap>();
            DirectoryInfo folder = new DirectoryInfo(modelPath);
            foreach (FileInfo file in folder.GetFiles("*.bmp"))
            {
                Bitmap t = new Bitmap(file.FullName);
                chList.Add(file.Name, t);
            }
            string result = "";
            foreach (Bitmap bitmap in bitmapList)
            {
                string maxKey = "";
                int maxCt = 0;
                int pxs = bitmap.Width * bitmap.Height;
                foreach (KeyValuePair<string, Bitmap> kv in chList)
                {
                    if (Math.Abs(bitmap.Width - kv.Value.Width) > 2 || Math.Abs(bitmap.Height - kv.Value.Height) > 3) continue;

                    int count = 0;
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        if (x >= kv.Value.Width) break;

                        for (int y = 0; y < bitmap.Height; ++y)
                        {
                            if (y >= kv.Value.Height) break;

                            if (bitmap.GetPixel(x, y).R == kv.Value.GetPixel(x, y).R)//&& bitmap.GetPixel(x,y).R==0)
                            {
                                ++count;
                            }
                        }
                    }
                    if (count > maxCt)
                    {
                        maxKey = kv.Key;
                        maxCt = count;
                    }
                    if (count == pxs) break;
                }
                string t = maxKey;
                if (maxKey == "") continue;
                if (t[0] == '_') continue;
                result += t[0];
            }
            return result;
        }
        /// <summary>
        /// 截取字符的有效部分，即去空白部分
        /// </summary>
        /// <param name="bitmapList">跟原图相同大小的图</param>
        /// <returns>返回字符大小的图</returns>
        public List<Bitmap> SaveCharacter(ref List<Bitmap> bitmapList)
        {
            List<Bitmap> newList = new List<Bitmap>();
            foreach (Bitmap bitmap in bitmapList)
            {
                int top = -1;
                int bottom = -1;
                int left = -1;
                int right = -1;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bool flag = true;
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        if (color.R == 0)
                        {
                            flag = false;
                            right = 0;
                            break;
                        }
                    }
                    if (flag && right == -1)
                    {
                        left = x;
                    }
                    else if (flag)
                    {
                        right = x;
                        break;
                    }
                }
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bool flag = true;
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        if (color.R == 0)
                        {
                            flag = false;
                            bottom = 0;
                            break;
                        }
                    }
                    if (flag && bottom == -1)
                    {
                        top = y;
                    }
                    else if (flag)
                    {
                        bottom = y;
                        break;
                    }
                }

                Rectangle rect = new Rectangle(left + 1, top + 1, right - left - 1, bottom - top - 1);
                if (rect.Width == 0 || rect.Height == 0) continue;
                Bitmap temp = bitmap.Clone(rect, bitmap.PixelFormat);
                //this.pictureBox6.Image = temp;
                //temp.Save(System.Environment.CurrentDirectory + "\\temp\\" + DateTime.Now.Ticks.ToString() + ".bmp");
                newList.Add(temp);
            }
            return newList;
        }
        /// <summary>
        /// 统计最多的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Color GetMaxColor(Bitmap bitmap)
        {            
            Dictionary<Color, int> colorList = new Dictionary<Color, int>();
            for (int x = 1; x < bitmap.Width-1; x++)
            {
                for (int y = 1; y < bitmap.Height-1; y++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    bool flag = false;
                    List<Color> keys = new List<Color>(colorList.Keys);
                    for (int i = 0; i < colorList.Count; i++)
                    {
                        if (keys[i] == color)
                        {
                            colorList[keys[i]]++;
                            flag = true;
                        }
                    }
                    if (!flag) colorList.Add(color, 1);
                }
            }
            Color max = new Color();
            int maxInt = 0;
            foreach (KeyValuePair<Color, int> c in colorList)
            {
                if (c.Value > maxInt)
                {
                    max = c.Key;
                    maxInt = c.Value;
                }
            }
            return max;//第二个为非白色
        }
        /// <summary>
        /// 统计最多的颜色
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="excColorList">除了此颜色</param>
        /// <returns></returns>
        public Color GetMaxColor(Bitmap bitmap,List<Color> excColorList)
        {
            Dictionary<Color, int> colorList = new Dictionary<Color, int>();
            for (int x = 1; x < bitmap.Width - 1; x++)
            {
                for (int y = 1; y < bitmap.Height - 1; y++)
                {
                    bool continueFlag=false;
                    Color color = bitmap.GetPixel(x, y);
                    foreach (Color excColor in excColorList)
                    {
                        if (color == excColor)//注意 判断 255与FF不能比较
                        {
                            continueFlag = true;
                            break;
                        }
                    }
                    if (continueFlag) continue;
                    bool flag = false;
                    List<Color> keys = new List<Color>(colorList.Keys);
                    for (int i = 0; i < colorList.Count; i++)
                    {
                        if (keys[i] == color)
                        {
                            colorList[keys[i]]++;
                            flag = true;
                        }
                    }
                    if (!flag) colorList.Add(color, 1);
                }
            }
            Color max = new Color();
            int maxInt = 0;
            foreach (KeyValuePair<Color, int> c in colorList)
            {
                if (c.Value > maxInt)
                {
                    max = c.Key;
                    maxInt = c.Value;
                }
            }
            return max;//第二个为非白色
        }
                
        /// <summary>
        /// 抠图
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>返回跟原图相同大小的 但只有一个字符的 图</returns>
        public List<Bitmap> CutBitmap(Bitmap bitmap)
        {
            List<Bitmap> bitmapList = new List<Bitmap>();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    if (color == BLACK)
                    {
                        Bitmap temp = new Bitmap(bitmap.Width, bitmap.Height);
                        for (int i = 0; i < temp.Width; i++)
                            for (int j = 0; j < temp.Height; j++)
                                temp.SetPixel(i, j, WHITE);

                        SetCuted(ref bitmap, ref temp, x, y);//递归抠图
                        bitmapList.Add(temp);
                    }
                }
            }
            //this.pictureBox4.Image = bitmap;
            return bitmapList;
        }
        /// <summary>
        /// 递归将x,y周围的黑色像素点“剪切”到temp上
        /// </summary>
        /// <param name="bitmap">源图</param>
        /// <param name="temp">目标图</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCuted(ref Bitmap bitmap, ref Bitmap temp, int x, int y)
        {
            if (x < 0 || y < 0 || x>=bitmap.Width || y>=bitmap.Height) return;
            Color c = bitmap.GetPixel(x, y);
            if (c == WHITE) return;
            temp.SetPixel(x, y, BLACK);
            bitmap.SetPixel(x, y, WHITE);
            SetCuted(ref bitmap, ref temp, x - 1, y - 1);
            SetCuted(ref bitmap, ref temp, x - 1, y);
            SetCuted(ref bitmap, ref temp, x - 1, y + 1);
            SetCuted(ref bitmap, ref temp, x, y - 1);
            //SetCuted(ref bitmap, ref temp, x , y );
            SetCuted(ref bitmap, ref temp, x, y + 1);
            SetCuted(ref bitmap, ref temp, x + 1, y - 1);
            SetCuted(ref bitmap, ref temp, x + 1, y);
            SetCuted(ref bitmap, ref temp, x + 1, y + 1);
        }

    }
}
