using MySql.Data.MySqlClient;
using ServerWeb_sh;
using System;
using System.Windows.Forms;

namespace Test1
{
    internal class Form1 : Form
    {
        private Label count;
        private Label label3;
        private Label content;
        private Timer timer;
        private System.ComponentModel.IContainer components;
        private Label label1;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.count = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.content = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "查询次数";
            // 
            // count
            // 
            this.count.AutoSize = true;
            this.count.Location = new System.Drawing.Point(73, 13);
            this.count.Name = "count";
            this.count.Size = new System.Drawing.Size(11, 12);
            this.count.TabIndex = 1;
            this.count.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "内容";
            // 
            // content
            // 
            this.content.AutoSize = true;
            this.content.Location = new System.Drawing.Point(13, 64);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(0, 12);
            this.content.TabIndex = 3;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(865, 261);
            this.Controls.Add(this.content);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.count);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public int countNumber = 0;

        private void FrmMain_Load(object sender, System.EventArgs e)
        {
            //设置Timer控件可用
            this.timer.Enabled = true;
            //设置时间间隔（毫秒为单位）
            this.timer.Interval = 1000;
        }
        private void timer_Tick(object sender, System.EventArgs e)
        {
            count.Text = ++countNumber + "";
            MyDBConnect myDBConnect = new MyDBConnect();
            myDBConnect.Open();
            try
            {
                string qian45 = DateTime.Now.AddMinutes(-45).ToString("yyyy-MM-dd HH:mm:ss");
                string qian30 = DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "SELECT diaoxian.Project_ID projectId,diaoxian.`max` lasttime,pro.userid userid,wxuser.openid openid,wxuser.nickname FROM `tb_qd_projectinuser` pro LEFT JOIN (SELECT Project_ID,MAX(tb.`Date`) `max` FROM tb_monitordata15 tb WHERE tb.`Date` > '" + qian45 + "' GROUP BY Project_ID HAVING `max` < '" + qian30 + "') diaoxian  ON diaoxian.Project_ID = pro.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON pro.userid = wxuser.userid WHERE diaoxian.Project_ID IS NOT NULL";
                MySqlDataReader reader = myDBConnect.Reader(sql);
                while (reader.NextResult())
                {
                    string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": projectId=" + reader.GetString("projectId") + " lasttime=" + reader.GetString("lasttime") + " userid=" + reader.GetString("userid") + " openid=" + reader.GetString("openid") + " nickname=" + reader.GetString("nickname");
                    content.Text = content.Text + "\r\n" + text;

                }
            }
            catch (Exception e1)
            {

            }
            finally
            {
                myDBConnect.Close();
            }
        }
        
    }
}