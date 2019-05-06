using Finalapp.model;
using MySql.Data.MySqlClient;
using ServerWeb_sh.model;
using System;
using System.Collections.Generic;

namespace ServerWeb_sh.c
{
    class Shangxian
    {
        public void Check(HashSet<int> diaoxianSet, MyDBConnect myDBConnect, DeviceStatusMonitor deviceStatusMonitor)
        {
            if (diaoxianSet == null || diaoxianSet.Count == 0 || myDBConnect == null || deviceStatusMonitor == null)
            {
                if (diaoxianSet == null)
                {
                    MyLog.log("掉线设备集合引用为空");
                }
                else if (diaoxianSet.Count == 0)
                {
                    MyLog.log("当前没有设备掉线");
                }
                return;
            }
            HashSet<Project> projectSet = new HashSet<Project>();
            string qian15 = DateTime.Now.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
            string inDiaoxian = "";
            int n = 0;
            foreach (int i in diaoxianSet)
            {
                n++;
                inDiaoxian += i;
                if(n < diaoxianSet.Count)
                {
                    inDiaoxian += ",";
                }
            }

            string sql = "SELECT shangxian.Project_ID projectId, wxuser.openid openid, project.`Project_Name` `name`,project.Project_Address address FROM `tb_qd_projectinuser` prouser LEFT JOIN (SELECT Project_ID FROM tb_monitordata15 tb WHERE tb.`Project_ID` IN (" + inDiaoxian + ") AND tb.`Date` > '" + qian15 + "') shangxian ON shangxian.Project_ID = prouser.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON prouser.userid = wxuser.userid LEFT JOIN `tb_project` project ON prouser.`Project_ID`= project.`Project_ID` WHERE prouser.`Project_ID` IN (" + inDiaoxian + ") AND shangxian.`Project_ID` IS NOT NULL AND wxuser.`openid` IS NOT NULL AND wxuser.`openid` != ''";
            MyLog.log("查询上线的SQL语句:" + sql);
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
            foreach (var project in projectSet)
            {
                diaoxianSet.Remove(project.projectId);
                Baojing baojing = new Baojing();
                baojing.text = project.name + "设备上线";
                baojing.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                baojing.type = "上线";
                baojing.point = project.address;
                baojing.BaojingObject = project.name;
                string result = deviceStatusMonitor.SendBaojing(baojing, project.openId);
                MyLog.log("设备上线：" + "openid=" + project.openId + ",name=" + project.name + ",address=" + project.address);
                MyLog.log("上线通知发送结果：" + result);
            }
        }
    }
}
