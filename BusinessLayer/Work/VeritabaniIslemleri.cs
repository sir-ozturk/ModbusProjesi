using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


public class VeritabaniIslemleri
{
    private SqlConnection sqlConnection;
    private SqlCommand sqlCommand;
    private SqlTransaction sqlTransaction;

    public string SpAdi { get; set; }

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

        sqlCommand = new SqlCommand();
        sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();

        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandType = CommandType.StoredProcedure;

        if (islemTip == IslemTip.BAGIMLI)
        {
            sqlTransaction = sqlConnection.BeginTransaction();
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
        sqlCommand.CommandText = SpAdi;

        int sonuc = sqlCommand.ExecuteNonQuery();

        ParametreleriSil();

        return sonuc > 0;
    }

    public DataTable TabloGetir()
    {
        sqlCommand.CommandText = SpAdi;

        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

        DataTable dataTable = new DataTable();

        sqlDataAdapter.Fill(dataTable);

        ParametreleriSil();

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
        sqlCommand.CommandText = SpAdi;

        object sonuc = sqlCommand.ExecuteScalar();

        ParametreleriSil();

        return sonuc;
    }

    public bool Bitir()
    {
        try
        {
            if (sqlTransaction != null)
            {
                sqlTransaction.Dispose();
                sqlTransaction = null;
            }

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

                SqlConnection.ClearAllPools();
            }

            return true;
        }
        catch
        {
            return false;
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

    public void ParametreleriSil()
    {
        try
        {
            if (sqlCommand != null)
            {
                sqlCommand.Parameters.Clear();
            }
        }
        catch
        {

        }
    }
}

