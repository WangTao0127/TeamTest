
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Finalapp.model
{
    class MyLog
    {
        static string date;
        private const string logDir = "c:\\log\\";

        public static void log(string text) {
            FileStream fs = null;
            try
            {
                if (!DateTime.Now.ToString("yyyy-MM-dd").Equals(date)) {
                    date = DateTime.Now.ToString("yyyy-MM-dd");
                }
                if (false == System.IO.Directory.Exists(logDir))
                {
                    //创建pic文件夹
                    System.IO.Directory.CreateDirectory(logDir);
                }
                string filename;
                if (true == System.IO.Directory.Exists(logDir))
                {
                    filename = logDir;
                }
                else {
                    filename = "c:\\";
                }
                filename = filename + date + "-log.txt";
                fs = new FileStream(filename, FileMode.Append);
                byte[] bytes = System.Text.Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "---" + text + "\r\n");
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch
            {
            }
            finally {
                if (fs != null) {
                    fs.Close();
                }
            }
        }
    }
}