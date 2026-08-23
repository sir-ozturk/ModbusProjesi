using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SifremiUnuttum : System.Web.UI.Page
{
    VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string siteKey = ConfigurationManager.AppSettings["TurnstileSiteKey"];
            turnstileWidget.Attributes["data-sitekey"] = siteKey;
        }

        if (Session["GeciciSifre"] != null)
        {
            string geciciSifreMesaji = Mesajlar.GeciciSifreBaslik + "<b>" + Session["GeciciSifre"].ToString() + "</b>";
            Mesaj.Ver(geciciSifreMesaji, Mesaj.MesajTurleri.SUCCESS, Page);
            Session.Remove("GeciciSifre");
        }
    }

    protected void btnSifirla_Click(object sender, EventArgs e)
    {
        string resetMail = TxtResetMail.Text.Trim();
        string resetKullaniciAdi = TxtResetKullaniciAdi.Text.Trim();

        if (string.IsNullOrEmpty(resetMail) || string.IsNullOrEmpty(resetKullaniciAdi))
        {
            Mesaj.Ver(Mesajlar.EmailVeKullaniciKoduDoldurunuz, Mesaj.MesajTurleri.WARNING, Page);
            return;
        }

        string token = Request.Form["cf-turnstile-response"];

        TurnstileIslemleri turnstileIslemleri = new TurnstileIslemleri();

        if (!turnstileIslemleri.Dogrula(token, Request.UserHostAddress))
        {
            Mesaj.Ver(Mesajlar.RobotDogrulamasiYapiniz, Mesaj.MesajTurleri.WARNING, Page);
            return;
        }

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

            kullanicilar.KullaniciAdi = resetKullaniciAdi;
            kullanicilar.Mail = resetMail;

            if (kullanicilar.SifreKontrol())
            {
                string yeniSifre = Utility.RastgeleSifreOlustur();

                kullanicilar.Sifre = yeniSifre;
                kullanicilar.GuncelleyenId = kullanicilar.Id;
                kullanicilar.GuncelleyenIp = Request.UserHostAddress;

                kullanicilar.SifreGuncelle();
                Session["GeciciSifre"] = yeniSifre;
                Response.Redirect("~/Pages/SifremiUnuttum.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            else
            {
                Mesaj.Ver(Mesajlar.KullaniciKoduVeyaMailHatali, Mesaj.MesajTurleri.FAIL, Page);
            }
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.SifreSifirlamaHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}