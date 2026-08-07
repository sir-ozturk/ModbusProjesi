using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModbusProjesi.AppCode;

namespace ModbusProjesi.Pages
{
    public partial class SifremiUnuttum : System.Web.UI.Page
    {
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

            try
            {
                Kullanicilar kullanicilar = new Kullanicilar();

                kullanicilar.KullaniciAdi = resetKullaniciAdi;
                kullanicilar.Mail = resetMail;

                if (kullanicilar.SifreKontrol())
                {
                    Random random = new Random();

                    string[] harfler =
                    {"A", "B", "C", "D", "E", "F", "G", "H",
                     "I", "J", "K", "L", "M", "N", "O", "P",
                     "Q", "R", "S", "T", "U", "V", "W", "X",
                     "Y", "Z",
                     "a", "b", "c", "d", "e", "f", "g", "h",
                     "i", "j", "k", "l", "m", "n", "o", "p",
                     "q", "r", "s", "t", "u", "v", "w", "x",
                     "y", "z"
                    };

                    string[] karakterler = { "!", "?", "*", "-", "_", "+", "#", "$" };

                    string rastgeleHarf1 = harfler[random.Next(0, harfler.Length)];
                    string rastgeleHarf2 = harfler[random.Next(0, harfler.Length)];
                    string rastgeleHarf3 = harfler[random.Next(0, harfler.Length)];
                    string rastgeleHarf4 = harfler[random.Next(0, harfler.Length)];

                    string rastgeleKarakter = karakterler[random.Next(0, karakterler.Length)];

                    int rastgeleSayi = random.Next(1000, 999999);

                    string yeniSifre = rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;

                    kullanicilar.Sifre = yeniSifre;
                    kullanicilar.GuncelleyenId = kullanicilar.Id;
                    kullanicilar.GuncelleyenIp = Request.UserHostAddress;

                    kullanicilar.SifreGuncelle();
                    Session["GeciciSifre"] = yeniSifre;
                    Response.Redirect("~/Pages/SifremiUnuttum.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Kullanıcı kodu veya mail hatalı!');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Şifre Sıfırlama Hatası: " + ex.Message + "');</script>");
            }
        }
    }
}