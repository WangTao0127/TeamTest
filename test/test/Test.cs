using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ServerWeb_sh.test
{
    class Test
    {

        NLog.Logger logger = NLog.LogManager.GetLogger("Test");
        //Timer不要声明成局部变量，否则会被GC回收
        private static System.Timers.Timer aTimer;
        private static bool isStart = false;
        public void test() {
            if (!isStart)
            {
                isStart = true;
                //实例化Timer类，设置间隔时间
                aTimer = new System.Timers.Timer(15 * 60 * 1000);
                //注册计时器的事件
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                //设置是执行一次（false）还是一直执行(true)，默认为true
                aTimer.AutoReset = true;
                delayedStart();
            }
        }

        private void delayedStart()
        {
            int min = DateTime.Now.Minute;
            if (min % 15 < 5)
            {
                System.Timers.Timer delayed = new System.Timers.Timer((5 - min % 15) * 60 * 1000);
                delayed.Elapsed += new ElapsedEventHandler(delayedTimed);
                delayed.AutoReset = false;
                delayed.Enabled = true;
            }
            else
            {
                aTimer.Enabled = true;
            }
        }

        private void delayedTimed(object sender, ElapsedEventArgs e)
        {
            aTimer.Enabled = true;
            OnTimedEvent(null, null);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
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
                    string text = " projectId=" + reader.GetString("projectId") + " lasttime=" + reader.GetString("lasttime") + " userid=" + reader.GetString("userid") + " openid=" + reader.GetString("openid") + " nickname=" + reader.GetString("nickname");
                    Console.WriteLine("报警设备的信息：" + text);
                    logger.Info("报警设备的信息：" + text);

                }
            }
            catch (Exception e1)
            {

                logger.Info("报警设备的信息：" + e1.Message);
            }
            finally
            {
                myDBConnect.Close();
            }
        }

    }
}
