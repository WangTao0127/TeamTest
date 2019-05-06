using Finalapp.model;
using MySql.Data.MySqlClient;
using ServerWeb_sh.model;
using System;
using System.Collections.Generic;

namespace ServerWeb_sh.c
{
    class Yujing
    {
        private static string day = "";
        private static Dictionary<int, Int32> overProofaNote = new Dictionary<int, int>();
        private static Dictionary<int, Int32> overProofbNote = new Dictionary<int, int>();
        public void Check(MyDBConnect myDBConnect, DeviceStatusMonitor deviceStatusMonitor)
        {
            try
            {
                Dictionary<int, List<Project>> earlyWarningDic = new Dictionary<int, List<Project>>();
                Dictionary<int, List<Project>> overProofaDic = new Dictionary<int, List<Project>>();
                Dictionary<int, List<Project>> overProofbDic = new Dictionary<int, List<Project>>();
                string nowDate = DateTime.Now.ToString("yyyy-MM-dd");
                //检查是否到了该清零的日子
                if (!nowDate.Equals(day))
                {
                    day = nowDate;
                    overProofaNote.Clear();
                    overProofbNote.Clear();
                }
                string qian15 = DateTime.Now.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "SELECT yujing.Project_ID projectId,yujing.`Dust` , yujing.`EarlyWarning` ,yujing.`OverProofa`,yujing.`OverProofaValue`,yujing.`OverProofb`,yujing.`OverProofbValue`,wxuser.openid openid, project.`Project_Name` `name`,project.Project_Address address FROM `tb_qd_projectinuser` prouser LEFT JOIN (SELECT config.Project_ID ,data15.`Dust` , config.`EarlyWarning` ,config.`OverProofa`,config.`OverProofaValue`,config.`OverProofb`,config.`OverProofbValue` FROM tb_projectwarnconfig config LEFT JOIN tb_monitordata15 data15 ON config.`Project_ID` = data15.`Project_ID`WHERE data15.`Date` > '" + qian15 + "' AND ((config.`EarlyWarning` !=0 AND data15.`Dust` >= config.`EarlyWarning`) OR (config.`OverProofa` !=0 AND config.`OverProofaValue` !=0 AND data15.`Dust` >= config.`OverProofaValue`) OR (config.`OverProofb` !=0 AND config.`OverProofbValue` !=0 AND data15.`Dust` >= config.`OverProofbValue`))) yujing ON yujing.Project_ID = prouser.Project_ID LEFT JOIN `tb_qd_weixininuser` wxuser ON prouser.userid = wxuser.userid LEFT JOIN `tb_project` project ON prouser.`Project_ID`= project.`Project_ID` WHERE yujing.Project_ID IS NOT NULL AND wxuser.`openid` IS NOT NULL AND wxuser.`openid` != ''";
                MyLog.log("粉尘检测sql: " + sql);
                myDBConnect.Open();
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
                    int Project_ID = reader.GetInt32("projectId");
                    Double Dust = reader.GetDouble("Dust");
                    Double EarlyWarning = reader.GetDouble("EarlyWarning");
                    int OverProofa = reader.GetInt32("OverProofa");
                    Double OverProofaValue = reader.GetDouble("OverProofaValue");
                    int OverProofb = reader.GetInt32("OverProofb");
                    Double OverProofbValue = reader.GetDouble("OverProofbValue");
                    if (EarlyWarning != 0 && Dust > EarlyWarning)
                    {
                        if (!earlyWarningDic.ContainsKey(Project_ID)) earlyWarningDic[Project_ID] = new List<Project>();
                        earlyWarningDic[Project_ID].Add(project);
                    }
                    if (OverProofa != 0 && OverProofaValue != 0 && Dust > OverProofaValue)
                    {
                        if (!earlyWarningDic.ContainsKey(Project_ID))
                        {
                            overProofaNote[Project_ID] = 0;
                        }
                        overProofaNote[Project_ID]++;
                        if (overProofaNote[Project_ID] >= OverProofa)
                        {
                            overProofaNote[Project_ID] = 0;
                            if (!overProofaDic.ContainsKey(Project_ID)) overProofaDic[Project_ID] = new List<Project>();
                            overProofaDic[Project_ID].Add(project);

                        }
                    }
                    if (OverProofb != 0 && OverProofbValue != 0 && Dust > OverProofbValue)
                    {
                        try
                        {
                            overProofbNote[Project_ID]++;
                        }
                        catch
                        {
                            overProofbNote[Project_ID] = 1;
                        }
                        if (overProofbNote[Project_ID] >= OverProofb)
                        {
                            overProofbNote[Project_ID] = 0;
                            if (!overProofbDic.ContainsKey(Project_ID)) overProofbDic[Project_ID] = new List<Project>();
                            overProofbDic[Project_ID].Add(project);
                        }
                    }
                }
                reader.Close();
                foreach (var map in earlyWarningDic)
                {
                    List<Project> projects = map.Value;
                    foreach (var project in projects)
                    {
                        MyLog.log("project_id为" + project.projectId + "的设备粉尘超过预警值了");
                        Baojing baojing = new Baojing();
                        baojing.text = project.name + "设备超出预警";
                        baojing.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        baojing.type = "预警";
                        baojing.point = project.address;
                        baojing.BaojingObject = project.name;
                        string result = deviceStatusMonitor.SendBaojing(baojing, project.openId);
                        MyLog.log("报警发送结果：" + result);
                    }
                }
                foreach (var map in overProofaDic)
                {
                    List<Project> projects = map.Value;
                    foreach (var project in projects)
                    {
                        MyLog.log("project_id为" + project.projectId + "的设备粉尘超过OverProofaValue值了");
                        Baojing baojing = new Baojing();
                        baojing.text = project.name + "设备告警";
                        baojing.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        baojing.type = "告警";
                        baojing.point = project.address;
                        baojing.BaojingObject = project.name;
                        string result = deviceStatusMonitor.SendBaojing(baojing, project.openId);
                        MyLog.log("报警发送结果：" + result);
                    }
                }
                foreach (var map in overProofbDic)
                {
                    List<Project> projects = map.Value;
                    foreach (var project in projects)
                    {
                        MyLog.log("project_id为" + project.projectId + "的设备粉尘超过OverProofbValue值了");
                        Baojing baojing = new Baojing();
                        baojing.text = project.name + "设备告警";
                        baojing.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        baojing.type = "告警";
                        baojing.point = project.address;
                        baojing.BaojingObject = project.name;
                        string result = deviceStatusMonitor.SendBaojing(baojing, project.openId);
                        MyLog.log("报警发送结果：" + result);
                    }
                }
            }
            catch (Exception e1)
            {
                MyLog.log("error|代码异常信息：" + e1.ToString());
            }
        }
    }
}
