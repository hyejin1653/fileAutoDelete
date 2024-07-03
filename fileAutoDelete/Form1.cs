using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fileAutoDelete
{
   
    public partial class Form1 : Form
    {
        bool runFlag = false;
        System.Threading.Timer myTimer;
        System.Threading.Timer myTimer2;

        public void fn_start(TimerCallback callback, int starttime, int sendtime)
        {
            myTimer = new System.Threading.Timer(callback, null, starttime, sendtime);
        }

        public void fn_start2(TimerCallback callback, int starttime, int sendtime)
        {
            myTimer2 = new System.Threading.Timer(callback, null, starttime, sendtime);
        }

        public void fn_end()
        {
            myTimer2.Dispose();
        }

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Work(object o)
        {
            this.textBox1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboMonth.Text = "3개월";

            
            fn_start(Work, 0, 1000);

            this.textBox1.Focus();
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            //OpenFileDialog folderBrowser = new OpenFileDialog();
            //folderBrowser.FileName = "Folder Selection";
            //if (folderBrowser.ShowDialog() == DialogResult.OK)
            //{
            //    string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
            //    this.listView1.Items.Add(folderPath);
            //}
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.listView1.Items.Add(folderBrowserDialog1.SelectedPath);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(this.listView1.Items.Count == 0)
            {
                // Displays the MessageBox.
                MessageBox.Show("삭제 리스트 선택후에 실행하세요", "Alert", MessageBoxButtons.OK);
                return;
            }
            if(!runFlag)
            {
                this.button3.Text = "실행취소";
                this.lblStatus.Text = "실행중..";
                
                this.btnAddFolder.Enabled = false;
                this.btnRowDelete.Enabled = false;

                //timer1_Tick(sender, e);
                //timer1.Interval = 1000;
                //timer1.Start();
                
                fn_start2(Work2, 0, 1000);
                

                runFlag = true;
            }
            else
            {
                this.button3.Text = "실행";
                this.lblStatus.Text = "";

                this.btnAddFolder.Enabled = true;
                this.btnRowDelete.Enabled = true;

                
                //timer1.Stop();
                fn_end();

                runFlag = false;
            }
        }

        private void Work2(object o)
        {
            string setDt = Convert.ToDateTime(this.dateTimePicker1.Value).ToString("yyyy-MM-dd");
            string systemDt = DateTime.Now.ToString("yyyy-MM-dd");

            string setTime = Convert.ToDateTime(this.dateTimePicker2.Value).ToString("HH:mm");
            string systemTime = DateTime.Now.ToString("HH:mm");

            int dtDiff = DateTime.Compare(Convert.ToDateTime(systemDt), Convert.ToDateTime(setDt));
            if (dtDiff > 0 || dtDiff == 0)
            {
                if (setTime == systemTime)
                {
                    deleteFolderAll();
                }
            }
            if(runFlag)
            {
                myTimer2.Change(0, 1000000);
            }
            
        }

        public bool isYYYYMMDD(string date)
        {
            return Regex.IsMatch(date, @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1]$)");
        }

        //폴더 삭제
        public void deleteFolderAll()
        {
            int month = Convert.ToInt32((cboMonth.Text).Replace("개월", ""));
            DateTime setDt = Convert.ToDateTime(this.dateTimePicker1.Value); //.ToString("yyyy-MM-dd");
            string realDt = setDt.AddMonths(-month).ToString("yyyy-MM-dd");

            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                string folderPath = (this.listView1.Items[i].SubItems[0].Text.ToString()).Replace("\\", @"\");

                DirectoryInfo di = new DirectoryInfo(folderPath);
                DirectoryInfo[] di_sub = di.GetDirectories();

                foreach (DirectoryInfo d in di_sub)
                {
                    string fileName = d.Name;
                    string filePath = d.FullName;
                    if (!isYYYYMMDD(fileName))
                    {
                        continue;
                    }
                    int dtDiff = DateTime.Compare(Convert.ToDateTime(realDt), Convert.ToDateTime(fileName));
                    if (dtDiff > 0 || dtDiff == 0)
                    {
                        d.Delete(true); //하위여부 상관없이 삭제
                        createLog(filePath);

                        this.listView2.Items.Add("삭제 폴더 : " + filePath);
                    }
                }
            }
        }

        //LOG 파일 생성
        public void createLog(string filePath)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\LOG";
            string nowDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string folderPath = path + "\\" + nowDate;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //로그파일 작성
            String strFilePath = folderPath + ".txt"; //반환후 저장되야하는 txt파일
            FileInfo fileInfo = new FileInfo(strFilePath);
            StreamWriter writer_;

            if (fileInfo.Exists)
            {
                writer_ = File.AppendText(strFilePath);
            }
            else
            {
                writer_ = File.CreateText(strFilePath);
            }

            writer_.WriteLine(DateTime.Now.ToString() + " " +  filePath);
            writer_.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string setDt = Convert.ToDateTime(this.dateTimePicker1.Value).ToString("yyyy-MM-dd");
            string systemDt = DateTime.Now.ToString("yyyy-MM-dd");

            string setTime = Convert.ToDateTime(this.dateTimePicker2.Value).ToString("HH:mm");
            string systemTime = DateTime.Now.ToString("HH:mm");

            int dtDiff = DateTime.Compare(Convert.ToDateTime(systemDt), Convert.ToDateTime(setDt));
            if (dtDiff > 0 || dtDiff == 0)
            {
                if (setTime == systemTime)
                {
                    deleteFolderAll();
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
        }

        private void btnFolderOpen_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\LOG";
            if(Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        private void btnRowDelete_Click(object sender, EventArgs e)
        {
            for (int i = (this.listView1.Items.Count - 1); i >= 0; i--)
            {
                if (this.listView1.Items[i].Checked)
                {
                    this.listView1.Items[i].Remove();
                }
            }
        }

        private void btnHidden_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 실행ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.notifyIcon1.Visible = false;
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
