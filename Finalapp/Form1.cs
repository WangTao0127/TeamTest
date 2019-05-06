using MySql.Data.MySqlClient;
using ServerWeb_sh;
using ServerWeb_sh.model;
using System;
using System.IO;
using System.Windows.Forms;

namespace Finalapp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public long startTime = 0;
        public int countNumber = 0;
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private void FrmMain_Load(object sender, System.EventArgs e)
        { 
            logger.Error("FrmMain_Load","afdsdf");
            startTime = DateTime.Now.Ticks;
            //设置Timer控件可用
            this.timer.Enabled = true;
            //设置时间间隔（毫秒为单位）
            this.timer.Interval = 1000;
            labelContent.Text = "";
            timer_Tick(null,null);
        }
        private void timer_Tick(object sender, System.EventArgs e)
        {
            count.Text = ++countNumber + "";
            MyDBConnect myDBConnect = new MyDBConnect();
            myDBConnect.Open();
            string sql = null;
            try
            {
                string qian45 = DateTime.Now.AddMinutes(-45).ToString("yyyy-MM-dd HH:mm:ss");
                string qian30 = DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
                sql = "SELECT wxuser.openid openid, project.`Project_Name` `name`,project.Project_Address address FROM `tb_qd_projectinuser` prouser LEFT JOIN (SELECT Project_ID,MAX(tb.`Date`) `max` FROM tb_monitordata15 tb WHERE tb.`Date` > '" + qian45 + "' GROUP BY Project_ID HAVING `max` < '" + qian30 + "') diaoxian  ON diaoxian.Project_ID = prouser.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON prouser.userid = wxuser.userid left join `tb_project` project on prouser.`Project_ID`= project.`Project_ID` WHERE diaoxian.Project_ID IS NOT NULL And wxuser.`openid` is not null and wxuser.`openid` != ''";
                MySqlDataReader reader = myDBConnect.Reader(sql);
                if (reader.NextResult())
                {
                    do
                    {
                        Project project = new Project();
                        project.openId = reader.GetString("openid");
                        project.name = reader.GetString("name");
                        project.address = reader.GetString("address");
                        string text = "openid=" + project.openId + ";name=" + project.name + ";address=" + project.address;
                        labelContent.Text = labelContent.Text + "\r\n" + text;
                    } while (reader.NextResult());
                }
                else {
                    labelContent.Text = labelContent.Text + "\r\n" +  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": 没有掉线设备"+(DateTime.Now.Ticks-startTime);
                }
            }
            catch (Exception e1)
            {
                labelContent.Text = labelContent.Text + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": 查询出错";
            }
            finally
            {
                const string filename = "log.txt";
                FileStream fs = new FileStream(filename, FileMode.Append);
                byte[] bytes = System.Text.Encoding.Default.GetBytes("sql=" + sql );
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                myDBConnect.Close();
            }
        }
    }


}
