using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace ModbusProjesi.AppCode
{
    public class SqlBaglanti
    {
        public SqlConnection Baglanti()
        {
            string bglString = ConfigurationManager.ConnectionStrings["ModbusDb"].ConnectionString;
            SqlConnection baglan = new SqlConnection(bglString);
            baglan.Open();
            return baglan;
        }
    }
}