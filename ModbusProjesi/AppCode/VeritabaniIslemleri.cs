using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ModbusProjesi.AppCode
{
    public class VeritabaniIslemleri
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;

        public void Baslat(string prosedurAdi)
        {
            string connectionString =ConfigurationManager.ConnectionStrings["ModbusDb"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            sqlCommand = new SqlCommand(prosedurAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
        }

        public void ParametreEkle(string parametreAdi, object parametreDegeri)
        {
            string tamParametreAdi = "@" + parametreAdi;

            if (parametreDegeri == null)
            {
                sqlCommand.Parameters.AddWithValue(tamParametreAdi,DBNull.Value);
            }
            else
            {
                sqlCommand.Parameters.AddWithValue(tamParametreAdi, parametreDegeri);
            }
        }

        public bool Calistir()
        {
            int sonuc = sqlCommand.ExecuteNonQuery();

            return sonuc > 0;
        }

        public DataTable TabloGetir()
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DataRow SatirGetir()
        {
            DataTable dataTable = TabloGetir();

            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0];
            }

            return null;
        }

        public object DegerGetir()
        {
            return sqlCommand.ExecuteScalar();
        }

        public void Bitir()
        {
            if (sqlCommand != null)
            {
                sqlCommand.Dispose();
                sqlCommand = null;
            }

            if (sqlConnection != null)
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }

                sqlConnection.Dispose();
                sqlConnection = null;
            }
        }
    }
}