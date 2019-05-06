
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Finalapp.model
{
    class MyLog
    {
        public static void log(String text) {

            const string filename = "c:\\log.txt";
            FileStream fs = new FileStream(filename, FileMode.Append);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"---"+text);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }
}