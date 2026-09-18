using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


public class VeritabaniIslemleri
{
    private SqlConnection sqlConnection;
    private SqlCommand sqlCommand;
    private SqlTransaction sqlTransaction;
    private List<SqlParameter> sqlParametreListesi;

    public bool LogYasak = false;
    // Güvenlik işlemlerinde SQL hatası çağırana iletilir; mevcut ekranların davranışı değişmez.
    public bool HatalariFirlat { get; set; }
    private bool baglantiHavuzuKullan = true;
    public string SonHataMesaji { get; private set; }

    public string SpAdi { get; set; }

    public bool HataBildir(string mesaj)
    {
        SonHataMesaji = mesaj;
        return false;
    }

    public bool UygulamaKilidiAl(string kaynak, bool paylasimli)
    {
        SonHataMesaji = null;

        if (sqlTransaction == null)
        {
            return HataBildir("Donanım işlemi için bağlı işlem başlatılmalıdır.");
        }

        using (SqlCommand komut = new SqlCommand("sys.sp_getapplock", sqlConnection, sqlTransaction))
        {
            komut.CommandType = CommandType.StoredProcedure;
            komut.Parameters.Add("@Resource", SqlDbType.NVarChar, 255).Value = kaynak;
            komut.Parameters.Add("@LockMode", SqlDbType.VarChar, 32).Value = paylasimli ? "Shared" : "Exclusive";
            komut.Parameters.Add("@LockOwner", SqlDbType.VarChar, 32).Value = "Transaction";
            komut.Parameters.Add("@LockTimeout", SqlDbType.Int).Value = 0;
            SqlParameter sonuc = komut.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
            sonuc.Direction = ParameterDirection.ReturnValue;

            komut.ExecuteNonQuery();

            if (Convert.ToInt32(sonuc.Value) < 0)
            {
                return HataBildir("Donanım için başka bir işlem devam ediyor. Tekrar deneyiniz.");
            }
        }

        return true;
    }

    public enum IslemTip
    {
        BAGIMLI,
        BAGIMSIZ
    }

    private IslemTip islemTip;

    public void Baslat(IslemTip tip)
    {
        Baslat(tip, null, true);
    }

    public void Baslat(IslemTip tip, int? zamanAsimiSaniye, bool havuzKullan)
    {
        islemTip = tip;

        string connectionString = ConfigurationManager.ConnectionStrings["ModbusDb"].ConnectionString;
        baglantiHavuzuKullan = havuzKullan;
        if (zamanAsimiSaniye.HasValue || !havuzKullan)
        {
            SqlConnectionStringBuilder ayarlar = new SqlConnectionStringBuilder(connectionString);
            if (zamanAsimiSaniye.HasValue) ayarlar.ConnectTimeout = zamanAsimiSaniye.Value;
            ayarlar.Pooling = havuzKullan;
            connectionString = ayarlar.ConnectionString;
        }

        sqlParametreListesi = new List<SqlParameter>();
        sqlCommand = new SqlCommand();
        if (zamanAsimiSaniye.HasValue) sqlCommand.CommandTimeout = zamanAsimiSaniye.Value;
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

        object deger = parametreDegeri == null ? DBNull.Value : parametreDegeri;

        SqlParameter sqlParameter = new SqlParameter(tamParametreAdi, deger);

        sqlCommand.Parameters.Add(sqlParameter);
        sqlParametreListesi.Add(sqlParameter);
    }

    public bool Calistir()
    {
        SonHataMesaji = null;
        try
        {
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = SpAdi;

            DataSet dSetTumVeriler = new DataSet();

            try
            {
                DataTable dataTableEskiKayitlar = LogIcinKayitGetir(sqlParametreListesi);

                if (dataTableEskiKayitlar != null)
                {
                    dataTableEskiKayitlar.TableName = "EskiKayitlar";
                    dSetTumVeriler.Tables.Add(dataTableEskiKayitlar);
                }
            }
            catch
            {
            }

            int sonuc = sqlCommand.ExecuteNonQuery();

            if (sonuc != 0)
            {
                try
                {
                    StackFrame frame = new StackFrame(1);
                    MethodBase method = frame.GetMethod();

                    string fonksAdi = method.Name;
                    string sinifAdi = method.DeclaringType.Name;

                    if (sinifAdi.ToLower() != "loglar" && LogYasak == false)
                    {
                        Loglar loglar = new Loglar(this);

                        loglar.Islem_tarihi = DateTime.Now;
                        loglar.Tablo_adi = sinifAdi;
                        loglar.Islem_adi = sinifAdi + " Tablosuna " + fonksAdi + " İşlemi";
                        loglar.Islem_tipi = "I";

                        if (fonksAdi.ToLower().Contains("ekle"))
                        {
                            loglar.Islem_tipi = "I";
                        }

                        if (fonksAdi.ToLower().Contains("gunc"))
                        {
                            loglar.Islem_tipi = "U";
                        }

                        if (fonksAdi.ToLower().Contains("sil"))
                        {
                            loglar.Islem_tipi = "D";
                        }

                        try
                        {
                            Sessionlar sessionlar = new Sessionlar();
                            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

                            loglar.Kullanici_id = currentInfo.KullaniciId;
                        }
                        catch
                        {
                            loglar.Kullanici_id = 0;
                        }

                        try
                        {
                            loglar.Url = System.Web.HttpContext.Current.Request.Url.ToString();
                        }
                        catch
                        {
                            loglar.Url = "";
                        }

                        loglar.Ip_adres = Utility.IpNoGetir();

                        if (loglar.Islem_tipi == "I")
                        {
                            for (int i = 0; i < sqlParametreListesi.Count; i++)
                            {
                                loglar.YeniDetayEkle(sqlParametreListesi[i].ParameterName, sqlParametreListesi[i].Value);
                            }

                            loglar.Detay = loglar.DetayGetir();
                        }
                        else
                        {
                            try
                            {
                                DataTable dataTableYeniKayitlar = LogIcinKayitGetir(sqlParametreListesi);

                                if (dataTableYeniKayitlar != null)
                                {
                                    dataTableYeniKayitlar.TableName = "YeniKayitlar";
                                    dSetTumVeriler.Tables.Add(dataTableYeniKayitlar);
                                }
                            }
                            catch
                            {
                            }

                            StringWriter stringWriter = new StringWriter();
                            dSetTumVeriler.WriteXml(stringWriter);
                            loglar.Detay = stringWriter.ToString();
                        }

                        ParametreleriSil();

                        loglar.Ekle();

                        return true;
                    }
                }
                catch
                {

                }

                ParametreleriSil();
                return true;
            }

            ParametreleriSil();
            return false;
        }
        catch (SqlException ex)
        {
            SonHataMesaji = ex.Number >= 51000 && ex.Number <= 51010
                ? ex.Message
                : "Kayıt işlemi tamamlanamadı. Bağlantı veya benzersizlik kurallarını kontrol ediniz.";
            ParametreleriSil();
            if (HatalariFirlat) throw;
            return false;
        }
        catch
        {
            ParametreleriSil();
            if (HatalariFirlat) throw;
            return false;
        }
    }

    private DataTable LogIcinKayitGetir(List<SqlParameter> sqlParametreListesi)
    {
        try
        {
            string procedureAdi = SpAdi;

            if (SpAdi.Contains("."))
            {
                procedureAdi = SpAdi.Split('.')[1];
            }

            string tabloAdi = procedureAdi.Split('_')[1];

            if (tabloAdi == "Loglar")
            {
                return null;
            }

            string procedureSorgusu = "SELECT OBJECT_DEFINITION(OBJECT_ID('" + procedureAdi + "'))";

            SqlCommand procedureCommand = new SqlCommand(procedureSorgusu, sqlConnection);

            if (sqlTransaction != null)
            {
                procedureCommand.Transaction = sqlTransaction;
            }

            object procedureIcerigiObject = procedureCommand.ExecuteScalar();

            if (procedureIcerigiObject == null || procedureIcerigiObject == DBNull.Value)
            {
                return null;
            }

            string procedureIcerigi = procedureIcerigiObject.ToString();

            string[] tumSorguStringleri = procedureIcerigi.Split(new string[] { "WHERE" }, StringSplitOptions.None);

            if (tumSorguStringleri.Length < 2)
            {
                return null;
            }

            string whereSonrasi = tumSorguStringleri[tumSorguStringleri.Length - 1];

            string filtrelemeSorgusu = whereSonrasi.Split(new string[] { "RETURN" }, StringSplitOptions.None)[0];

            for (int i = 0; i < sqlParametreListesi.Count; i++)
            {
                string parametreAdi = sqlParametreListesi[i].ParameterName;
                object parametreDegeri = sqlParametreListesi[i].Value;

                string parametreDegeriMetin = parametreDegeri == DBNull.Value ? "NULL" : parametreDegeri.ToString();

                filtrelemeSorgusu = filtrelemeSorgusu.Replace(parametreAdi, parametreDegeriMetin);
            }

            string sorguDetay = "SELECT * FROM " + tabloAdi + " WHERE " + filtrelemeSorgusu;

            SqlCommand sorguCommand = new SqlCommand(sorguDetay, sqlConnection);

            if (sqlTransaction != null)
            {
                sorguCommand.Transaction = sqlTransaction;
            }

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sorguCommand);

            DataTable dataTable = new DataTable();

            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return dataTable;
            }

            return null;
        }
        catch
        {
            return null;
        }
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

                if (baglantiHavuzuKullan) SqlConnection.ClearAllPools();
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

            if (sqlParametreListesi != null)
            {
                sqlParametreListesi.Clear();
            }
        }
        catch
        {

        }
    }
}

