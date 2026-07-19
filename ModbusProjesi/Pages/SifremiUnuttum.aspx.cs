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
        SqlBaglanti bglSinifi = new SqlBaglanti();
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

            using (SqlConnection bgl = bglSinifi.Baglanti())
            {
                string sorgu = "Select * From Kullanicilar Where kullanici_adi=@p1 And mail=@p2 And aktiflik_durumu=1";

                using (SqlCommand komut = new SqlCommand(sorgu, bgl))
                {
                    komut.Parameters.AddWithValue("@p1", resetKullaniciAdi);
                    komut.Parameters.AddWithValue("@p2", resetMail);

                    try
                    {
                        bgl.Open();
                        SqlDataReader dr = komut.ExecuteReader();

                        if (dr.Read())
                        {
                            string KullaniciAd = dr["ad"].ToString();
                            dr.Close();

                            Random rnd = new Random();

                            string[] harfler1 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                            string rastgeleHarf1 = harfler1[rnd.Next(0, harfler1.Length)];

                            string[] harfler2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                            string rastgeleHarf2 = harfler2[rnd.Next(0, harfler2.Length)];

                            string[] harfler3 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                            string rastgeleHarf3 = harfler3[rnd.Next(0, harfler3.Length)];

                            string[] harfler4 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                            string rastgeleHarf4 = harfler4[rnd.Next(0, harfler4.Length)];

                            string[] karakterler = { "!", "?", "*", "-", "_", "+", "#", "$" };
                            string rastgeleKarakter = karakterler[rnd.Next(0, karakterler.Length)];

                            int rastgeleSayi = rnd.Next(1000, 999999);
                            string yeniSifre = rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;

                            SqlCommand guncellemeKomutu = new SqlCommand("Update Kullanicilar set sifre=@sifre where mail=@p1 and kullanici_adi=@p2",bgl);
                            guncellemeKomutu.Parameters.AddWithValue("@sifre", yeniSifre);
                            guncellemeKomutu.Parameters.AddWithValue("@p1", resetMail);
                            guncellemeKomutu.Parameters.AddWithValue("@p2", resetKullaniciAdi);
                            guncellemeKomutu.ExecuteNonQuery();

                            Session["GeciciSifre"] = yeniSifre;
                            Response.Redirect("SifremiUnuttum.aspx");

                            TxtResetKullaniciAdi.Text = "";
                            TxtResetMail.Text = "";

                        }
                        else
                        {
                            dr.Close();
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