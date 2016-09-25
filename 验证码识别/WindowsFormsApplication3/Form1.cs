using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using UncodeHelper;
using System.IO;
using DecodeHelper;
using DynamicDecode;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        static Color WHITE = Color.FromArgb(255, 255, 255, 255);
        static Color BLACK = Color.FromArgb(255, 0, 0, 0);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            this.webBrowser1.Navigate("http://jw2.ahu.cn");
            //this.webBrowser1.DocumentCompleted+=new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

            //Bitmap bitmap = new Bitmap(Application.StartupPath + "\\numbers\\635410432599516320.bmp");
            //this.pictureBox1.Image = bitmap;
            
            //UnCode unCode = new UnCode(bitmap, Application.StartupPath + "\\numbers");
            //label1.Text="hh:"+unCode.GetCode();
            /*
            Bitmap bitmap = new Bitmap(Application.StartupPath + "\\numbers\\635410528658838875.bmp");
            this.pictureBox1.Image = new Bitmap(Application.StartupPath + "\\numbers\\635410528658838875.bmp");
            Decode(bitmap);*/
        }


        
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ///
            /// 获取webbrowser里的图片
            ///
            HTMLDocument html = (HTMLDocument)webBrowser1.Document.DomDocument;//注意是HTMLDocument而不是HtmlDocument
            //寻找验证码图片，因为没有ID所以得自己定位
            HtmlElement elem = webBrowser1.Document.GetElementById("icode");
            IHTMLControlElement img = (IHTMLControlElement)elem.DomElement;
            IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)html.body).createControlRange();
            range.add(img);
            range.execCommand("Copy", false, null);

            if (Clipboard.ContainsImage())
            {
                Image pic = Clipboard.GetImage();
                this.pictureBox1.Image = pic;

                Bitmap bitmap = (Bitmap)pic.Clone();

                bitmap.Save(Application.StartupPath + "\\numbers\\" + DateTime.Now.Ticks.ToString() + ".bmp");

                JWDecode decode = new JWDecode();
                decode.Decode(bitmap, Application.StartupPath + "\\numbers2\\");
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///
            /// 获取webbrowser里的图片
            ///
            HTMLDocument html = (HTMLDocument)webBrowser1.Document.DomDocument;//注意是HTMLDocument而不是HtmlDocument
            //寻找验证码图片，因为没有ID所以得自己定位
            HtmlElement elem = webBrowser1.Document.GetElementById("icode");
            IHTMLControlElement img = (IHTMLControlElement)elem.DomElement;
            IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)html.body).createControlRange();
            range.add(img);
            range.execCommand("Copy", false, null);

            if (Clipboard.ContainsImage())
            {
                Image pic = Clipboard.GetImage();
                this.pictureBox1.Image = pic;

                Bitmap bitmap = (Bitmap)pic.Clone();

                bitmap.Save(Application.StartupPath + "\\numbers\\" + DateTime.Now.Ticks.ToString() + ".bmp");

                JWDecode decode = new JWDecode();
                string result=decode.Decode(bitmap, Application.StartupPath + "\\numbers2\\");
                label1.Text = result;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ///
            /// 获取webbrowser里的图片
            ///
            HTMLDocument html = (HTMLDocument)webBrowser1.Document.DomDocument;//注意是HTMLDocument而不是HtmlDocument
            //寻找验证码图片，因为没有ID所以得自己定位
            HtmlElement elem = webBrowser1.Document.GetElementById("icode");
            IHTMLControlElement img = (IHTMLControlElement)elem.DomElement;
            IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)html.body).createControlRange();
            range.add(img);
            range.execCommand("Copy", false, null);

            if (Clipboard.ContainsImage())
            {
                Image pic = Clipboard.GetImage();
                this.pictureBox1.Image = pic;

                Bitmap bitmap = (Bitmap)pic.Clone();

                bitmap.Save(Application.StartupPath + "\\numbers\\" + DateTime.Now.Ticks.ToString() + ".bmp");

                Decoding decode = new Decoding();
                string result = decode.Decode(bitmap);
                label1.Text = result;
            }
        }

    }
}
