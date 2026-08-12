using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BusinessLayer.Work
{
    public class VeritabaniIslemleri
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlTransaction sqlTransaction;

        public enum IslemTip
        {
            BAGIMLI,
            BAGIMSIZ
        }

        private IslemTip islemTip;

        public void Baslat(IslemTip tip)
        {
            islemTip = tip;

            string connectionString = ConfigurationManager.ConnectionStrings["ModbusDb"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            if (islemTip == IslemTip.BAGIMLI)
            {
                sqlTransaction = sqlConnection.BeginTransaction();
            }
        }

        public void ProsedurSec(string prosedurAdi)
        {
            if (sqlConnection == null)
            {
                throw new Exception("Veritabanı bağlantısı oluşturulmamış.");
            }

            if (sqlConnection.State != ConnectionState.Open)
            {
                throw new Exception("Veritabanı bağlantısı açık değil.");
            }

            sqlCommand = new SqlCommand(prosedurAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (sqlTransaction != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }
        }

        public void ParametreEkle(string parametreAdi, object parametreDegeri)
        {
            string tamParametreAdi = "@" + parametreAdi;

            if (parametreDegeri == null)
            {
                sqlCommand.Parameters.AddWithValue(tamParametreAdi, DBNull.Value);
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

        public void Uygula()
        {
            if (sqlTransaction == null)
            {
                return;
            }

            sqlTransaction.Commit();
            sqlTransaction.Dispose();
            sqlTransaction = null;
        }

        public void GeriAl()
        {
            if (sqlTransaction == null)
            {
                return;
            }

            sqlTransaction.Rollback();
            sqlTransaction.Dispose();
            sqlTransaction = null;
        }
    }
}
