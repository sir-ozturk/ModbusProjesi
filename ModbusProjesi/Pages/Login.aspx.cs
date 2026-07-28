using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModbusProjesi.AppCode;

namespace ModbusProjesi.Pages
{
    public partial class Login : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGiris_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKullaniciAdi.Text) || string.IsNullOrEmpty(txtSifre.Text))
            {
                Response.Write("<script>alert('Lütfen kullanıcı adı ve şifre giriniz!');</script>");
                return;
            }

            try
            {
                Kullanicilar kullanicilar = new Kullanicilar();

                kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                kullanicilar.Sifre = txtSifre.Text.Trim();

                if (kullanicilar.Giris())
                {
                    Session["kullaniciAdSoyad"] = kullanicilar.Ad + "" + kullanicilar.Soyad;
                    Session["kullaniciFoto"] = kullanicilar.ProfilResim;

                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Kullanıcı adı veya şifre hatalı!');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Giriş Hatası: " + ex.Message + "');</script>");
            }
        }
    }
}