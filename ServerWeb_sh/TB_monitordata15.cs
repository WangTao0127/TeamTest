using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWeb_sh
{
    class TB_monitordata15
    {
        public const string ID = "ID";
        public const string Project_ID = "Project_ID";
        public const string Device_Code = "Device_Code";
        public const string Date = "Date";
        public const string Dust = "Dust";
        public const string Noise = "Noise";
        public const string PM10 = "PM10";
        public const string PM25 = "PM25";
        public const string PM01 = "PM01";
        public int id;
        public int projectId;
        public string deviceCode;
        public DateTime date;
        public int dust;
        public int noise;
        public float pm10;
        public float pm25;
        public float pm01;

    }
}
