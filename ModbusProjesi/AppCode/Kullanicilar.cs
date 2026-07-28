using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace ModbusProjesi.AppCode
{
    public class Kullanicilar:VeritabaniIslemleri
    {

        public int Id;
        public string KullaniciAdi;
        public string Sifre;
        public string Ad;
        public string Soyad;
        public string Telefon;
        public string Mail;
        public int RolId;
        public bool AktiflikDurumu;
        public string ProfilResim;

        public bool Ekle() 
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_EKLE", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi);
                sqlCommand.Parameters.AddWithValue("@Sifre", Sifre);
                sqlCommand.Parameters.AddWithValue("@Ad", Ad);
                sqlCommand.Parameters.AddWithValue("@Soyad", Soyad);
                sqlCommand.Parameters.AddWithValue("@Telefon", Telefon);
                sqlCommand.Parameters.AddWithValue("@Mail", Mail);
                sqlCommand.Parameters.AddWithValue("@RolId", RolId);
                sqlCommand.Parameters.AddWithValue("@AktiflikDurumu", AktiflikDurumu);
                sqlCommand.Parameters.AddWithValue("@ProfilResim", ProfilResim);

                sqlCommand.ExecuteNonQuery();
                return true;
            }
        }

        public void Guncelle() 
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_GUNCELLE", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Id", Id);
                sqlCommand.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi);

                if (string.IsNullOrEmpty(Sifre))
                {
                    sqlCommand.Parameters.AddWithValue("@Sifre", DBNull.Value);
                }
                else
                {
                    sqlCommand.Parameters.AddWithValue("@Sifre", Sifre);
                }

                sqlCommand.Parameters.AddWithValue("@Ad", Ad);
                sqlCommand.Parameters.AddWithValue("@Soyad", Soyad);
                sqlCommand.Parameters.AddWithValue("@Telefon", Telefon);
                sqlCommand.Parameters.AddWithValue("@Mail", Mail);
                sqlCommand.Parameters.AddWithValue("@RolId", RolId);
                sqlCommand.Parameters.AddWithValue("@AktiflikDurumu", AktiflikDurumu);

                if (string.IsNullOrEmpty(ProfilResim))
                {
                    sqlCommand.Parameters.AddWithValue("@ProfilResim", DBNull.Value);
                }
                else
                {
                    sqlCommand.Parameters.AddWithValue("@ProfilResim", ProfilResim);
                }

                sqlCommand.ExecuteNonQuery();
            }
        }

        public void Sil() 
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_SIL", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Id", Id);
                sqlCommand.ExecuteNonQuery();
            }
        }

        public void Getir() 
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_GETIR", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Id", Id);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    Id = Convert.ToInt32(sqlDataReader["id"]);
                    KullaniciAdi = sqlDataReader["kullanici_adi"].ToString();
                    Sifre = sqlDataReader["sifre"].ToString();
                    Ad = sqlDataReader["ad"].ToString();
                    Soyad = sqlDataReader["soyad"].ToString();
                    Telefon = sqlDataReader["telefon"].ToString();
                    Mail = sqlDataReader["mail"].ToString();
                    RolId = Convert.ToInt32(sqlDataReader["rol_id"]);
                    AktiflikDurumu = Convert.ToBoolean(sqlDataReader["aktiflik_durumu"]);
                    ProfilResim = sqlDataReader["profil_resim"].ToString();
                }
            }
        }

        public DataTable Listele()
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_LISTELE",sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        public bool Giris()
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_GIRIS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi);
                sqlCommand.Parameters.AddWithValue("@Sifre", Sifre);

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    if (sqlDataReader.Read())
                    {
                        Id = Convert.ToInt32(sqlDataReader["id"]);
                        Ad = sqlDataReader["ad"].ToString();
                        Soyad = sqlDataReader["soyad"].ToString();
                        ProfilResim = sqlDataReader["profil_resim"].ToString();
                        AktiflikDurumu = Convert.ToBoolean(sqlDataReader["aktiflik_durumu"]);

                        return true;
                    }

                    return false;
                }
            }
        }

        public bool SifreKontrol()
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_SIFREKONTROL", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi);
                sqlCommand.Parameters.AddWithValue("@Mail", Mail);

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    if (sqlDataReader.Read())
                    {
                        Ad = sqlDataReader["ad"].ToString();
                        return true;
                    }

                    return false;
                }
            }
        }

        public void SifreGuncelle()
        {
            using (SqlConnection sqlConnection = Baglanti())
            {
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_SIFREGUNCELLE", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@KullaniciAdi", KullaniciAdi);
                sqlCommand.Parameters.AddWithValue("@Mail", Mail);
                sqlCommand.Parameters.AddWithValue("@Sifre", Sifre);

                sqlCommand.ExecuteNonQuery();
            }
        }

    }
}