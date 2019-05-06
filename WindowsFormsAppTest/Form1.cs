using MySql.Data.MySqlClient;
using ServerWeb_sh;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class from : Form
    {
        public from()
        {
            InitializeComponent();
        }

        public int countNumber = 0;

        private void FrmMain_Load(object sender, EventArgs e)
         {
             //设置Timer控件可用
             this.timer.Enabled = true;
             //设置时间间隔（毫秒为单位）
             this.timer.Interval = 1000;
         }
        private void timer_Tick(object sender, EventArgs e)
         {
            count.Text = ++countNumber+"";
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
                    string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+": projectId=" + reader.GetString("projectId") + " lasttime=" + reader.GetString("lasttime") + " userid=" + reader.GetString("userid") + " openid=" + reader.GetString("openid") + " nickname=" + reader.GetString("nickname");
                    content.Text = content.Text +"\r\n"+ text;

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
