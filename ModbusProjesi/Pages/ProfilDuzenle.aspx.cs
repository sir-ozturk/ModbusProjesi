using System;
using System.IO;
using System.Net.Mail;

public partial class ProfilDuzenle : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["ProfilBasariMesaji"] != null)
            {
                Mesaj.Ver(Session["ProfilBasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Page.Master);
                Session.Remove("ProfilBasariMesaji");
            }

            BilgileriYukle();
        }
    }

    private void BilgileriYukle()
    {
        CurrentInfo currentInfo = new Sessionlar().Current._CurrentInfo;
        if (currentInfo == null)
        {
            Response.Redirect("~/Pages/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Kullanicilar kullanici = new Kullanicilar(veritabaniIslemleri);
            kullanici.Id = currentInfo.KullaniciId;

            if (!kullanici.Doldur())
            {
                Mesaj.Ver(Mesajlar.KayitBulunamadi, Mesaj.MesajTurleri.FAIL, Page.Master);
                btnKaydet.Enabled = false;
                return;
            }

            txtAd.Text = kullanici.Ad;
            txtSoyad.Text = kullanici.Soyad;
            txtTelefon.Text = Utility.TelefonFormatla(kullanici.Telefon);
            txtMail.Text = kullanici.Mail;
            imgProfil.ImageUrl = string.IsNullOrEmpty(kullanici.ProfilResim) ? "~/Files/Images/no-image.svg" : "~/Files/" + kullanici.ProfilResim;
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.VeriYuklemeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        string ad = txtAd.Text.Trim();
        string soyad = txtSoyad.Text.Trim();
        string telefon = txtTelefon.Text.Trim();
        string mail = txtMail.Text.Trim();
        string mevcutSifre = txtMevcutSifre.Text;
        string yeniSifre = txtYeniSifre.Text;
        string yeniSifreTekrar = txtYeniSifreTekrar.Text;
        bool sifreDegistiriliyor = !string.IsNullOrEmpty(mevcutSifre) || !string.IsNullOrEmpty(yeniSifre) || !string.IsNullOrEmpty(yeniSifreTekrar);

        if (string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad) || string.IsNullOrWhiteSpace(telefon) || string.IsNullOrWhiteSpace(mail))
        {
            Mesaj.Ver(Mesajlar.ProfilAlanlariBos, Mesaj.MesajTurleri.WARNING, Page.Master);
            return;
        }

        try
        {
            MailAddress adres = new MailAddress(mail);
            if (!string.Equals(adres.Address, mail, StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException();
            }
        }
        catch
        {
            Mesaj.Ver(Mesajlar.GecersizEPostaAdresi, Mesaj.MesajTurleri.WARNING, Page.Master);
            return;
        }

        if (sifreDegistiriliyor)
        {
            if (string.IsNullOrEmpty(mevcutSifre) || string.IsNullOrEmpty(yeniSifre) || string.IsNullOrEmpty(yeniSifreTekrar))
            {
                Mesaj.Ver(Mesajlar.SifreAlanlariBos, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }

            if (!string.Equals(yeniSifre, yeniSifreTekrar, StringComparison.Ordinal))
            {
                Mesaj.Ver(Mesajlar.YeniSifrelerEslesmiyor, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }
        }

        if (fuProfilResmi.HasFile)
        {
            string uzanti = Path.GetExtension(fuProfilResmi.FileName).ToLowerInvariant();
            if (uzanti != ".jpg" && uzanti != ".jpeg" && uzanti != ".png")
            {
                Mesaj.Ver(Mesajlar.GecerliProfilResmiSeciniz, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }
        }

        CurrentInfo currentInfo = new Sessionlar().Current._CurrentInfo;
        if (currentInfo == null)
        {
            Response.Redirect("~/Pages/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        DosyaIslemleri dosyaIslemleri = new DosyaIslemleri();
        string yeniFotoAdi = "";
        string eskiFotoAdi = "";

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Kullanicilar kullanici = new Kullanicilar(veritabaniIslemleri);
            kullanici.Id = currentInfo.KullaniciId;

            if (!kullanici.Doldur())
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.KayitBulunamadi, Mesaj.MesajTurleri.FAIL, Page.Master);
                return;
            }

            if (sifreDegistiriliyor && !string.Equals(kullanici.Sifre, mevcutSifre, StringComparison.Ordinal))
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.MevcutSifreHatali, Mesaj.MesajTurleri.FAIL, Page.Master);
                return;
            }

            if (sifreDegistiriliyor && string.Equals(kullanici.Sifre, yeniSifre, StringComparison.Ordinal))
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.YeniSifreMevcutSifreyleAyni, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }

            eskiFotoAdi = kullanici.ProfilResim;
            kullanici.Telefon = telefon;
            kullanici.Mail = mail;

            if (kullanici.KayitVarMi())
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.KayitZatenMevcut, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }

            if (fuProfilResmi.HasFile)
            {
                yeniFotoAdi = dosyaIslemleri.ResimKaydet(fuProfilResmi.PostedFile);
            }

            kullanici.Ad = ad;
            kullanici.Soyad = soyad;
            kullanici.Telefon = telefon;
            kullanici.Mail = mail;
            kullanici.Sifre = sifreDegistiriliyor ? yeniSifre : null;
            kullanici.ProfilResim = string.IsNullOrEmpty(yeniFotoAdi) ? null : yeniFotoAdi;
            kullanici.GuncelleyenId = currentInfo.KullaniciId;
            kullanici.GuncelleyenIp = Utility.IpNoGetir();

            if (!kullanici.Guncelle())
            {
                veritabaniIslemleri.GeriAl();
                if (!string.IsNullOrEmpty(yeniFotoAdi))
                {
                    dosyaIslemleri.ResimSil(yeniFotoAdi);
                }
                Mesaj.Ver(Mesajlar.IslemGerceklestirilemedi, Mesaj.MesajTurleri.FAIL, Page.Master);
                return;
            }

            veritabaniIslemleri.Uygula();

            if (!string.IsNullOrEmpty(yeniFotoAdi) && !string.IsNullOrEmpty(eskiFotoAdi))
            {
                dosyaIslemleri.ResimSil(eskiFotoAdi);
            }

            currentInfo.Ad = ad;
            currentInfo.Soyad = soyad;
            currentInfo.KullaniciEmail = mail;
            if (!string.IsNullOrEmpty(yeniFotoAdi))
            {
                currentInfo.ProfilResim = yeniFotoAdi;
            }

            Session["ProfilBasariMesaji"] = Mesajlar.ProfilBasariylaGuncellendi;
            Response.Redirect("~/Pages/ProfilDuzenle.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            veritabaniIslemleri.GeriAl();
            if (!string.IsNullOrEmpty(yeniFotoAdi))
            {
                dosyaIslemleri.ResimSil(yeniFotoAdi);
            }
            Mesaj.Ver(Mesajlar.GenelHata + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}
