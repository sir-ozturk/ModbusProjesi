using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLayer.Entity;
using BusinessLayer.Interfaces;
using BusinessLayer.Work;

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

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                // Giriş tek başına çalışan bir işlem olduğu için BAĞIMSIZ
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

                Kullanicilar kullanicilar = new Kullanicilar();

                kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                kullanicilar.Sifre = txtSifre.Text.Trim();

                KullaniciIslemleri kullaniciIslemleri = new KullaniciIslemleri(veritabaniIslemleri);

                if (kullaniciIslemleri.Giris(kullanicilar))
                {
                    Session["kullaniciId"] = kullanicilar.Id;
                    Session["kullaniciAdSoyad"] = kullanicilar.Ad + " " + kullanicilar.Soyad;
                    Session["kullaniciFoto"] = kullanicilar.ProfilResim;

                    Response.Redirect("~/Default.aspx",false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
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
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }
    }
}