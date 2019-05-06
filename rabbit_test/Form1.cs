using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rabbit_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void SendHello(object sender, MouseEventArgs e)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "192.168.133.128";//RabbitMQ服务在本地运行
            factory.UserName = "augus";//用户名
            factory.Password = "123456";//密码

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);//创建一个名称为hello的消息队列
                    string message = "Hello World 123 321 456 654 789 987"; //传递的消息内容
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "hello", null, body); //开始传递
                    Console.WriteLine("已发送： {0}", message);
                    Console.ReadLine();
                   // Form1_Load(null,null);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "192.168.133.128";
            factory.UserName = "augus";
            factory.Password = "123456";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("hello", false, consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var arr = message.Split(' ');
                        var x = from m in arr where m.Length == 3 select m;
                        label1.Text = "";
                        foreach (var i in x) {
                            label1.Text += i+" ";
                        }
                    };
                    Console.ReadLine();
                }
            }
        }
    }
}
