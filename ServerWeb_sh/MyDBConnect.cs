using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace ServerWeb_sh
{

        public class MyDBConnect
        {
            private static readonly string constr = "server=101.200.45.217;User Id=root;password=c9ra@86hhd;Database=dustmonitor_sh;Charset=utf8;SslMode=none";
            private MySqlConnection _SqlConnection;

            private MySqlDataReader _Reader;

            public void Open()
            {
                _SqlConnection = new MySqlConnection(constr);
                try
                {
                    _SqlConnection.Open();
                }
                catch (InvalidOperationException e)
                {
                    System.Diagnostics.Trace.Assert(false, "CreateConnection错误：" + e.Message);
                }
                catch (SqlException e)
                {
                    System.Diagnostics.Trace.Assert(false, "CreateConnection错误：" + e.Message);
                }
            }
            public MySqlDataReader Reader(string sql)
            {
                MySqlCommand cmd = new MySqlCommand(sql, _SqlConnection);
                _Reader = cmd.ExecuteReader();
                return _Reader;

            }
            public void Close()
            {
            try
            {
                _Reader?.Close();
            }
            catch { }
                _Reader = null;
                _SqlConnection?.Close();
                _SqlConnection = null;
            }
        }
}