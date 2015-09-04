using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;


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
                    if (System.IO.File.Exists(d)) xFileProcess(d);   //ファイルの場合
                    else if (System.IO.Directory.Exists(d))
                    {
                        string[] files = System.IO.Directory.GetFiles(d, "*", System.IO.SearchOption.AllDirectories);
                        foreach (string d2 in files)
                        {
                            xFileProcess(d2);    //ディレクトリ内のファイルの場合
                        }
                    }
                }
//                e.Effect = DragDropEffects.Copy;
            }
        }
        private void xFileProcess(string d)
        {
            string cname="null";
            DateTime ctime=DateTime.Now;
            int changedateflag = 0;

            //拡張子を取得
            string ext = System.IO.Path.GetExtension(d);
            
            //exif情報を取得
            if (ext.Equals(".jpg") || ext.Equals(".JPG"))
            {
                xGetExifInfo(d, out ctime, out cname);
                changedateflag = 1;
            }
            else if (ext.Equals(".gif") || ext.Equals(".GIF"))
            {
                cname = "gif";
            }

            xLog(d + " : " + ext + " : " + cname + " : " + ctime.ToString());

            //ファイルの移動
            string dstfn = Path.GetFileName(d);
            string dstpathfn = System.IO.Path.Combine(cname, dstfn);
            xLog(dstpathfn);

            

            //System.IO.File.Copy(d,dstpathfn);
            FileSystem.CopyFile(d, dstpathfn,UIOption.AllDialogs, UICancelOption.DoNothing);
            // メッセージ処理を促して表示を更新する
            Application.DoEvents();
        }
        private void xGetExifInfo(string d,out DateTime exiftime,out string cameraname)
        {
            exiftime = DateTime.Now;
            cameraname = "jpg";
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
                    cameraname = val;
                }
                //Exif情報から撮影時間を取得する
                if (item.Id == 0x9003 && item.Type == 2)
                {
                    //文字列に変換する
                    string val = System.Text.Encoding.ASCII.GetString(item.Value);
                    val = val.Trim(new char[] { '\0' });
                    //DateTimeに変換
                    DateTime dt = DateTime.ParseExact(val, "yyyy:MM:dd HH:mm:ss", null);
                    //ファイルの作成日時を変更
                    //System.IO.File.SetCreationTime(imgFile, dt);
                    exiftime = dt;
                }
            }
            bmp.Dispose();

            
        }
        private void xLog(String s)
        {
            //ログ出力
            DateTime dt = DateTime.Now;
            textBox1.AppendText(dt.ToString() + " " + s + "\n");
            System.IO.StreamWriter sw = new System.IO.StreamWriter(@"log.txt", true, System.Text.Encoding.GetEncoding("shift_jis"));
            sw.Write(dt.ToString() + s + "\r\n");
            sw.Close();
        }

    }
}
