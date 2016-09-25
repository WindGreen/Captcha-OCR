using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace UncodeHelper
{
    public class UnCode
    {
        Bitmap bitmap=default(Bitmap);    
        String path=default(string);//模板路径
        public UnCode()
        {
        }
        public UnCode(Bitmap bitmap)
        {
            this.bitmap=bitmap;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <param name="path">模板路径</param>
        public UnCode(Bitmap bitmap,String path)
        {
            this.bitmap = bitmap;
            this.path = path;
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public String GetCode()
        {
            return GetCode(bitmap, path);
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public String GetCode(Bitmap bitmap,String path)
        {
            bitmap = Translate(bitmap);//灰度处理
            List<Bitmap> bitmapList = Cut(bitmap);            
            return Compare(bitmapList,path);
        }
        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="bitmap"></param>
        public void GetModels(Bitmap bitmap)
        {
        }
        /// <summary>
        /// 对图片进行二值化
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Bitmap Translate(Bitmap bitmap)
        {
                int blackNum = 0; //黑点个数
                ///二值化
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color c = bitmap.GetPixel(x,y);
                        int tc = (c.R + c.G + c.B) / 3;//灰度值                        
                        if (tc<130)//小于阈值,设为黑色
                        {
                            bitmap.SetPixel(x,y,Color.FromArgb(c.A,0,0,0));
                            blackNum++;
                        }       
                        else//大于阈值，设为白色
                        {
                            bitmap.SetPixel(x,y,Color.FromArgb(c.A,255,255,255));
                        }          
                    }
                }
                
                ///二值化过后需要判断图片的黑白比列，若果黑色比白色多，需要对图片反色处理
                if (blackNum * 1.0 / (bitmap.Height * bitmap.Width) > 0.5)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            Color c=bitmap.GetPixel(x,y);
                            bitmap.SetPixel(x,y,Color.FromArgb(c.A,255-c.R,255-c.G,255-c.B));
                        }
                    }
                }               
            return bitmap;
        }
        /// <summary>
        /// 图片分割
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public List<Bitmap> Cut(Bitmap bitmap)
        { 
                ///横向分割
                /// 直接拿R,G,B中的一个来跟0和255比较，Color.Black的Alfalfa值为FF      
                int top = -1;
                int button = -1;
                int left = -1;
                int right = -1;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Boolean isBlank = true;
                    int ct = 0;
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color c = bitmap.GetPixel(x, y);
                        if (c.R == 0)//如果为黑色 也就0
                        {
                            isBlank = false;
                            ct++;
                        } 
                    }
                    if (ct==1) isBlank = true;//校正单个点
                    if (button < 0)
                    {
                        if (isBlank)
                        {
                            top = y+1;//顶侧---------------如果开始不是白色呢~~~~
                        }
                        else
                        {
                            button = y;
                        }
                    }
                    else
                    {
                        if (isBlank)
                        {
                            button = y;//底侧 
                            break;
                        }
                    }
                } 
                ///纵向分割
                List<Bitmap> bitmapList = new List<Bitmap>();
                for(int x=0;x<bitmap.Width;x++)
                {
                    Boolean isBlank = true;
                    int ct = 0;
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color c = bitmap.GetPixel(x, y);
                        if (c.R == 0)//如果为黑色 也就0
                        {
                            isBlank = false;
                            ct++;
                        }
                    }
                    if (ct == 1) isBlank = true;//校正单个点
                    if (right <=left)
                    {
                        if (isBlank)
                        {
                            left = x+1;//左侧
                        }
                        else
                        {
                            right = x;
                        }
                    }
                    else
                    {
                        if (isBlank)
                        {
                            right = x;//右侧
                            Rectangle rect = new Rectangle(left, top, right - left, button - top);
                            bitmapList.Add(bitmap.Clone(rect, bitmap.PixelFormat)); 
                            left = right+1;
                        }
                    }                                   
                } 
            return bitmapList;
        }
        /// <summary>
        /// 跟数字模板对比
        /// </summary>
        /// <param name="bitmapList"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public String Compare(List<Bitmap> bitmapList,String path)
        {
            /// 导入对比模板
            List<Bitmap> numList = new List<Bitmap>();
            for (int i = 0; i < 10; i++)
            { 
                Bitmap t = new Bitmap(path+"\\"+i.ToString()+".bmp");
                numList.Add(t);
            }
            String result = "";
            foreach (Bitmap bm in bitmapList)
            {
                int[] count = new int[10];
                for (int i = 0; i < 10; i++)
                {
                    Bitmap t = numList[i];
                    int width = (bm.Width < t.Width ? bm.Width : t.Width);
                    int height = (bm.Height < t.Height ? bm.Height : t.Height);
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (bm.GetPixel(x, y).R == t.GetPixel(x, y).R)
                                count[i]++;//如果相同则加1
                        }
                    }

                }
                int ct = count[0];//统计中相同最多的
                int max = 0;//最大的数字；
                for (int i = 1; i < 10; i++)
                {
                    if (count[i] > ct)
                    {
                        ct = count[i];
                        max = i;
                    }
                }
                result += max.ToString();
            }
            return result;
        }
    }
}
