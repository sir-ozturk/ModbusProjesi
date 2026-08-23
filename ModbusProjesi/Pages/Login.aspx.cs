using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class Login : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            string siteKey = ConfigurationManager.AppSettings["TurnstileSiteKey"];
            turnstileWidget.Attributes["data-sitekey"] = siteKey;
        }
    }

    protected void btnGiris_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtKullaniciAdi.Text) || string.IsNullOrEmpty(txtSifre.Text))
        {
            Mesaj.Ver(Mesajlar.KullaniciAdiVeSifreGiriniz, Mesaj.MesajTurleri.WARNING, Page);
            return;
        }

        string token = Request.Form["cf-turnstile-response"];
        TurnstileIslemleri turnstileIslemleri = new TurnstileIslemleri();

        if (!turnstileIslemleri.Dogrula(token, Request.UserHostAddress))
        {
            Mesaj.Ver(Mesajlar.RobotDogrulamasiYapiniz, Mesaj.MesajTurleri.WARNING, Page);
            return;
        }


        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            // Giriş tek başına çalışan bir işlem olduğu için BAĞIMSIZ
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

            kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
            kullanicilar.Sifre = txtSifre.Text.Trim();

            if (kullanicilar.Giris())
            {
                Sessionlar sessionlar = new Sessionlar();
                CurrentInfo currentInfo = new CurrentInfo();

                currentInfo.KullaniciId = kullanicilar.Id;
                currentInfo.KullaniciAdi = kullanicilar.KullaniciAdi;
                currentInfo.Ad = kullanicilar.Ad;
                currentInfo.Soyad = kullanicilar.Soyad;
                currentInfo.RolId = kullanicilar.RolId;
                currentInfo.ProfilResim = kullanicilar.ProfilResim;
                currentInfo.Ip = Request.UserHostAddress;
                currentInfo.LoginYapildiMi = true;

                sessionlar.Current._CurrentInfo = currentInfo;

                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            else
            {
                Mesaj.Ver(Mesajlar.KullaniciAdiVeyaSifreHatali, Mesaj.MesajTurleri.FAIL, Page);
            }
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.GirisHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}