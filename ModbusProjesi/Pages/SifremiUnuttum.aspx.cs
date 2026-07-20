using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ModbusProjesi.AppCode;

namespace ModbusProjesi.Pages
{
    public partial class SifremiUnuttum : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["GeciciSifre"] != null)
            {
                lblYeniSifre.Text = "Geçici Şifreniz: " + Session["GeciciSifre"].ToString();
                Session["GeciciSifre"] = null;
            }
        }

        protected void btnSifirla_Click(object sender, EventArgs e)
        {
            string resetMail = TxtResetMail.Text.Trim();
            string resetKullaniciAdi = TxtResetKullaniciAdi.Text.Trim();

            if (string.IsNullOrEmpty(resetMail) || string.IsNullOrEmpty(resetKullaniciAdi))
            {
                Response.Write("<script>alert('Lütfen email ve kullanıcı kodu alanlarını doldurunuz!');</script>");
                return;
            }

            using (SqlConnection sqlConnection = sqlBaglanti.Baglanti())
            {
                string sorgu = "Select * From Kullanicilar Where kullanici_adi=@p1 And mail=@p2 And aktiflik_durumu=1";

                using (SqlCommand sqlCommand = new SqlCommand(sorgu, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@p1", resetKullaniciAdi);
                    sqlCommand.Parameters.AddWithValue("@p2", resetMail);

                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                        if (sqlDataReader.Read())
                        {
                            string KullaniciAd = sqlDataReader["ad"].ToString();
                            sqlDataReader.Close();

                            Random random = new Random();

                            string[] harfler = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                                 "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};

                            string rastgeleHarf1 = harfler[random.Next(0, harfler.Length)];
                            string rastgeleHarf2 = harfler[random.Next(0, harfler.Length)];
                            string rastgeleHarf3 = harfler[random.Next(0, harfler.Length)];
                            string rastgeleHarf4 = harfler[random.Next(0, harfler.Length)];

                            string[] karakterler = { "!", "?", "*", "-", "_", "+", "#", "$" };
                            string rastgeleKarakter = karakterler[random.Next(0, karakterler.Length)];

                            int rastgeleSayi = random.Next(1000, 999999);

                            string yeniSifre = rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;

                            SqlCommand sqlCommand1 = new SqlCommand("Update Kullanicilar set sifre=@sifre where mail=@p1 and kullanici_adi=@p2", sqlConnection);
                            sqlCommand1.Parameters.AddWithValue("@sifre", yeniSifre);
                            sqlCommand1.Parameters.AddWithValue("@p1", resetMail);
                            sqlCommand1.Parameters.AddWithValue("@p2", resetKullaniciAdi);
                            sqlCommand1.ExecuteNonQuery();

                            Session["GeciciSifre"] = yeniSifre;
                            Response.Redirect("SifremiUnuttum.aspx");

                            TxtResetKullaniciAdi.Text = "";
                            TxtResetMail.Text = "";

                        }
                        else
                        {
                            sqlDataReader.Close();
                            Response.Write("<script>alert('Kullanıcı kodu veya mail hatalı!');</script>");
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('Bağlantı Hatası: " + ex.Message + "');</script>");
                    }
                }
            }
        }
    }
}