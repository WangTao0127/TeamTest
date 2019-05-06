using MySql.Data.MySqlClient;
using ServerWeb_sh.model;
using System;
using System.ServiceProcess;
using System.Timers;
using Finalapp.model;
using ServerWeb_sh.c;
using System.Threading;

namespace ServerWeb_sh
{

    public partial class Service1 : ServiceBase
    {
        const int YanChi = 12;
        //Timer不要声明成局部变量，否则会被GC回收（网上原话，我觉得不太可能）
        private static System.Timers.Timer aTimer; //设备检测timer
        private static bool isStart = false;
        private DeviceStatusMonitor deviceStatusMonitor;
        private Diaoxian diaoxian;
        private Shangxian shangxian;
        private Yujing yujing;
        private int count = 0;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            MyLog.log("服务启动");
            deviceStatusMonitor = new DeviceStatusMonitor();
            diaoxian = new Diaoxian();
            shangxian = new Shangxian();
            yujing = new Yujing();
            Start();
        }
        private void Start()
        {
            if (!isStart)
            {
                isStart = true;
                //实例化Timer类，设置间隔时间
                aTimer = new System.Timers.Timer(15 * 60 * 1000);
                //注册计时器的事件
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                //设置是执行一次（false）还是一直执行(true)，默认为true
                aTimer.AutoReset = true;
                delayedStart();//寻找适合时间打开启动设备检测timer
            }
        }
        //寻找适合时间打开启动设备检测timer
        private void delayedStart()
        {
            int min = DateTime.Now.Minute;
            if (min % 15 < YanChi)
            {
                System.Timers.Timer delayed = new System.Timers.Timer((YanChi - min % 15) * 60 * 1000);
                delayed.Elapsed += new ElapsedEventHandler(delayedTimed);
                delayed.AutoReset = false;
                delayed.Enabled = true;
            }
            else
            {
                delayedTimed(null, null);
            }
        }

        private void delayedTimed(object sender, ElapsedEventArgs e)
        {

            MyLog.log("检测定时器启动");
            aTimer.Enabled = true;//启动设备检测timer
            OnTimedEvent(null, null);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            int min = DateTime.Now.Minute;
            if (min % 15 < YanChi)
            {
                Thread.Sleep((YanChi - min % 15) * 60 * 1000);
            }
            MyLog.log("|定时器第" + ++count + "次运行开始");
            MyDBConnect myDBConnect = null;
            try
            {
                myDBConnect = new MyDBConnect();
                myDBConnect.Open();
                deviceStatusMonitor.RequestToken();//获取微信token
                MyLog.log("||开始检测掉线设备是否上线");
                shangxian.Check(Diaoxian.diaoxianSet, myDBConnect, deviceStatusMonitor);//检测上线
                MyLog.log("||检测掉线设备是否上线已结束");
                MyLog.log("||开始检测设备是否掉线");
                diaoxian.Check(myDBConnect, deviceStatusMonitor);//检测掉线
                MyLog.log("||检测设备是否掉线已结束");
                MyLog.log("||开始检测设备粉尘");
                yujing.Check(myDBConnect, deviceStatusMonitor);//检测预警
                MyLog.log("||检测设备粉尘已结束");
            }
            catch (Exception e1)
            {
                MyLog.log("error|代码异常信息：" + "方法=OnTimedEvent，异常=" + e1.Message);
            }
            finally
            {
                if (myDBConnect != null)
                {
                    myDBConnect.Close();
                }
                MyLog.log("|检测定时器第" + count + "次运行结束");
            }
        }
        /* 
        private String testBaojing()
        {
            int count = 10;
            string error = "";
            do
            {
                count--;
                Baojing baojing = new Baojing();
                baojing.text = "testBaojing";
                baojing.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                baojing.type = "报警测试";
                baojing.point = "这只是测试，所以没有";
                baojing.BaojingObject = "hello augus";
                string result = deviceStatusMonitor.SendBaojing(baojing, "oqYVQwMv9Df7sy5CMTNL-i9ljTZ8");
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (((int)jo["errcode"]) == 40001)
                {
                    deviceStatusMonitor.RequestToken();
                }
                else if (((int)jo["errcode"]) == 0)
                {
                    return result;

                }
                error = jo["errmsg"].ToString();
            } while (count>0);
            return error;
        }*/

        protected override void OnStop()
        {
            MyLog.log("服务停止");
            isStart = false;
            aTimer = null;
        }
    }
}
