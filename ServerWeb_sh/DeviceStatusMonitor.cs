using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWeb_sh
{
    class DeviceStatusMonitor
    {
        private static readonly String baseUrl = "https://api.weixin.qq.com";
        private Mtoken token = null;
        public bool RequestToken()
        {
            RestClient client = new RestClient(baseUrl);
            IRestResponse response;
            try
            {
                long time = GetCurrentTimeSecond();
                RestRequest request = new RestRequest("/cgi-bin/token?grant_type=client_credential&appid=wx63b0565355f22a19&secret=b6b42baa4efce8fab64f7565a0c400a1", Method.GET);
                response = client.Execute(request);
                Mtoken mtoken = JsonConvert.DeserializeObject<Mtoken>(response.Content);
                mtoken.time = time;
                token = mtoken;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool CheckToken()
        {
            if (token == null) return false;
            bool b = GetCurrentTimeSecond() - token.time < token.expires_in;
            return b;
        }

        private long GetCurrentTimeSecond()
        {
            return DateTime.Now.ToUniversalTime().Ticks / 10000000;
        }

        public string SendBaojing(Baojing baojing, string openid)
        {
            RestClient client = new RestClient(baseUrl);
            IRestResponse response;
            RestRequest request = new RestRequest("/cgi-bin/message/template/send?access_token=" + token.access_token, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                touser = openid,
                template_id = "xV_7Gk1bofKKkAJhCvphHi_JY2bo7pjhZA8WnnDeK3Y",
                url = baojing.url,
                data = new
                {
                    first = new { value = baojing.text },
                    keyword1 = new { value = baojing.BaojingObject },
                    keyword2 = new { value = baojing.time },
                    keyword3 = new { value = baojing.type },
                    remark = new { value = baojing.point }
                }

            });
            response = client.Execute(request);
            return response.Content;
        }
    }


    public class Mtoken
    {
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expires_in { get; set; }
        public long time { get; set; }
    }
}
