using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace ModbusProjesi.AppCode
{
    public class Roller:VeritabaniIslemleri
    {
        public DataTable Listele()
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Roller_LISTELE", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }
    }
}