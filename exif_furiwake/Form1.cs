using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
 
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags)
                {
                    if (System.IO.File.Exists(d)) xImgProcess(d);   //ファイルの場合
                    else if (System.IO.Directory.Exists(d))
                    {
                        string[] files = System.IO.Directory.GetFiles(d, "*", System.IO.SearchOption.AllDirectories);
                        foreach (string d2 in files)
                        {
                            xImgProcess(d2);    //ディレクトリ内のファイルの場合
                        }
                    }
                }
//                e.Effect = DragDropEffects.Copy;
            }
        }
        private void xImgProcess(string d)
        {
            string cname = xGetCameraName(d);
            textBox1.AppendText(d+" : "+cname + "\n");
        }
        private string xGetCameraName(string d)
        {
            string cname = "null";
            string imgFile = d;

            //読み込む
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgFile);
            //Exif情報を列挙する
            foreach (System.Drawing.Imaging.PropertyItem item in bmp.PropertyItems)
            {
                //データの型を判断
                if ((item.Type == 2)&&(item.Id==272))
                {
                    //ASCII文字の場合は、文字列に変換する
                    string val = System.Text.Encoding.ASCII.GetString(item.Value);
                    val = val.Trim(new char[] { '\0' });
                    //表示する
                    //textBox1.AppendText("id:" + item.Id + " type:" + item.Type + " val" + val + "\n");
                    cname = val;
                }
                else
                {
                    //表示する
                    //textBox1.AppendText("id:" + item.Id + " type:" + item.Type + " len:" + item.Len + "\n");
                }
            }
            bmp.Dispose();

            return cname;
        }

    }
}
