using Finalapp.model;
using MySql.Data.MySqlClient;
using ServerWeb_sh.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWeb_sh.c
{
    class Diaoxian
    {
        public static HashSet<int> diaoxianSet = new HashSet<int>();
        public void Check(MyDBConnect myDBConnect, DeviceStatusMonitor deviceStatusMonitor)
        {
            HashSet<Project> projectSet = new HashSet<Project>();
            string qian45 = DateTime.Now.AddMinutes(-45).ToString("yyyy-MM-dd HH:mm:ss");
            string qian30 = DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
            // string sql = "SELECT diaoxian.Project_ID projectId,diaoxian.`max` lasttime,pro.userid userid,wxuser.openid openid,wxuser.nickname FROM `tb_qd_projectinuser` pro LEFT JOIN (SELECT Project_ID,MAX(tb.`Date`) `max` FROM tb_monitordata15 tb WHERE tb.`Date` > '" + qian45 + "' GROUP BY Project_ID HAVING `max` < '" + qian30 + "') diaoxian  ON diaoxian.Project_ID = pro.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON pro.userid = wxuser.userid WHERE diaoxian.Project_ID IS NOT NULL";
            //可以用的
            string sql = "SELECT diaoxian.Project_ID projectId, wxuser.openid openid, project.`Project_Name` `name`,project.Project_Address address FROM `tb_qd_projectinuser` prouser LEFT JOIN (SELECT Project_ID,MAX(tb.`Date`) `max` FROM tb_monitordata15 tb WHERE tb.`Date` > '" + qian45 + "' GROUP BY Project_ID HAVING `max` < '" + qian30 + "') diaoxian  ON diaoxian.Project_ID = prouser.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON prouser.userid = wxuser.userid left join `tb_project` project on prouser.`Project_ID`= project.`Project_ID` WHERE diaoxian.Project_ID IS NOT NULL And wxuser.`openid` is not null and wxuser.`openid` != ''";
            //测试用的
            //string sql = "SELECT wxuser.openid openid, project.`Project_Name` `name`,project.Project_Address address FROM `tb_qd_projectinuser` prouser LEFT JOIN (SELECT Project_ID,MAX(tb.`Date`) `max` FROM tb_monitordata15 tb WHERE tb.`Date` > '" + qian45 + "' GROUP BY Project_ID) diaoxian  ON diaoxian.Project_ID = prouser.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON prouser.userid = wxuser.userid left join `tb_project` project on prouser.`Project_ID`= project.`Project_ID` WHERE diaoxian.Project_ID IS NOT NULL And wxuser.`openid` is not null and wxuser.`openid` != ''";
            MyLog.log("查询掉线的SQL语句:" + sql);
            MySqlDataReader reader = myDBConnect.Reader(sql);
            while (reader.Read())
            {
                Project project = new Project
                {
                    projectId = reader.GetInt32("projectId"),
                    openId = reader.GetString("openid"),
                    name = reader.GetString("name"),
                    address = reader.GetString("address")
                };
                projectSet.Add(project);
                
            }
            reader.Close();
            foreach (var project in projectSet) {
                Baojing baojing = new Baojing();
                baojing.text = project.name + "设备掉线";
                baojing.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                baojing.type = "掉线";
                baojing.point = project.address;
                baojing.BaojingObject = project.name;
                string result = deviceStatusMonitor.SendBaojing(baojing, project.openId);
                MyLog.log("设备掉线：" + "openid=" + project.openId + ",name=" + project.name + ",address=" + project.address);
                MyLog.log("报警发送结果：" + result);
                diaoxianSet.Add(project.projectId);
            }
        }

    }
}
