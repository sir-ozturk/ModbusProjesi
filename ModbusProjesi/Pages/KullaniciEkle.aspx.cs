using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class KullaniciEkle : System.Web.UI.Page
{
    string gelenId = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        gelenId = Request.QueryString["id"];

        if (Page.IsPostBack == false)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

                if (Session["BasariMesaji"] != null)
                {
                    Mesaj.Ver(Session["BasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Page.Master);
                    Session.Remove("BasariMesaji");
                }

                Roller roller = new Roller(veritabaniIslemleri);
                roller.TumunuGetir();

                ddlRoller.DataSource = roller.VeriTablosu;
                ddlRoller.DataTextField = "rol_adi";
                ddlRoller.DataValueField = "id";
                ddlRoller.DataBind();

                ddlRoller.Items.Insert(0, new ListItem("Rol Seçiniz...", "0"));

                if (!string.IsNullOrEmpty(gelenId))
                {
                    litSayfaBaslik.Text = "Kullanıcı Bilgilerini Güncelle";

                    btnKaydet.Text = "Güncelle";

                    phYeniKayitNotu.Visible = false;
                    phGuncellemeSifreAlani.Visible = true;

                    Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

                    kullanicilar.Id = Convert.ToInt32(gelenId);
                    kullanicilar.Doldur();

                    txtAd.Text = kullanicilar.Ad;
                    txtSoyad.Text = kullanicilar.Soyad;
                    txtTelefon.Text = Utility.TelefonFormatla(kullanicilar.Telefon);
                    txtMail.Text = kullanicilar.Mail;
                    txtKullaniciAdi.Text = kullanicilar.KullaniciAdi;
                    txtSifre.Text = kullanicilar.Sifre;
                    ddlRoller.SelectedValue = kullanicilar.RolId.ToString();
                    ddlAktiflik.SelectedValue = kullanicilar.AktifMi.ToString();

                    if (!string.IsNullOrEmpty(kullanicilar.ProfilResim))
                    {
                        imgProfil.ImageUrl = "~/Files/" + kullanicilar.ProfilResim;

                        imgProfil.Style["display"] = "inline-block";
                    }
                }
                else
                {
                    litSayfaBaslik.Text = "Yeni Kullanıcı Ekle";

                    btnKaydet.Text = "Kaydet";

                    phYeniKayitNotu.Visible = true;
                    phGuncellemeSifreAlani.Visible = false;
                }
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
    }


    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtAd.Text) ||
            string.IsNullOrEmpty(txtSoyad.Text) ||
            string.IsNullOrEmpty(txtTelefon.Text) ||
            string.IsNullOrEmpty(txtMail.Text) ||
            ddlRoller.SelectedValue == "0" ||
            ddlAktiflik.SelectedValue == "Seçiniz...")
        {
            Mesaj.Ver(Mesajlar.TumAlanlariDoldurunuz, Mesaj.MesajTurleri.WARNING, Page.Master);

            return;
        }

        Sessionlar sessionlar = new Sessionlar();
        CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
        DosyaIslemleri dosyaIslemleri = new DosyaIslemleri();

        string yeniDosyaAdi = "";

        // Resim seçilmişse uzantısını kontrol et ve kaydet
        if (fuProfilResmi.HasFile)
        {
            string uzanti = Path.GetExtension(fuProfilResmi.FileName).ToLower();

            if (uzanti != ".jpg" && uzanti != ".jpeg" && uzanti != ".png")
            {
                Mesaj.Ver(Mesajlar.GecerliProfilResmiSeciniz, Mesaj.MesajTurleri.WARNING, Page.Master);

                return;
            }
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        bool sonuc = false;
        string eskiFotoAdi = "";

        try
        {
            // KayitVarMi + Getir + Ekle/Guncelle
            // aynı işlem akışında olduğu için BAGIMLI.
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

            if (!string.IsNullOrEmpty(gelenId))
            {
                kullanicilar.Id = Convert.ToInt32(gelenId);
                kullanicilar.Doldur();
            }
            else
            {
                kullanicilar.Id = 0;
                kullanicilar.KullaniciAdi = null;
            }

            kullanicilar.Telefon = txtTelefon.Text.Trim();
            kullanicilar.Mail = txtMail.Text.Trim();

            if (kullanicilar.KayitVarMi())
            {
                veritabaniIslemleri.GeriAl();

                Mesaj.Ver(Mesajlar.KayitZatenMevcut, Mesaj.MesajTurleri.WARNING, Page.Master);

                return;
            }

            // Dosyayı kaydet
            if (fuProfilResmi.HasFile)
            {
                yeniDosyaAdi = dosyaIslemleri.ResimKaydet(fuProfilResmi.PostedFile);
            }

            // GÜNCELLEME
            if (!string.IsNullOrEmpty(gelenId))
            {

                // Eski kullanıcı bilgilerini getirir.
                kullanicilar.Doldur();

                eskiFotoAdi = kullanicilar.ProfilResim;
                kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                kullanicilar.Sifre = txtSifre.Text.Trim();
                kullanicilar.Ad = txtAd.Text.Trim();
                kullanicilar.Soyad = txtSoyad.Text.Trim();
                kullanicilar.Telefon = txtTelefon.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.RolId = Convert.ToInt32(ddlRoller.SelectedValue);
                kullanicilar.AktifMi = Convert.ToBoolean(ddlAktiflik.SelectedValue);
                kullanicilar.GuncelleyenId = currentInfo.KullaniciId;
                kullanicilar.GuncelleyenIp = currentInfo.Ip;
                // Yeni resim seçilmişse resim adını değiştir.
                if (fuProfilResmi.HasFile)
                {
                    kullanicilar.ProfilResim = yeniDosyaAdi;
                }

                if (kullanicilar.Guncelle())
                {
                    sonuc = true;

                    Session["BasariMesaji"] = Mesajlar.KullaniciBasariylaGuncellendi;
                }
            }

            // YENİ KULLANICI EKLEME
            else
            {
                string geciciSifre = Utility.RastgeleSifreOlustur();

                kullanicilar.Sifre = geciciSifre;
                kullanicilar.Ad = txtAd.Text.Trim();
                kullanicilar.Soyad = txtSoyad.Text.Trim();
                kullanicilar.Telefon = txtTelefon.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.RolId = Convert.ToInt32(ddlRoller.SelectedValue);
                kullanicilar.AktifMi = Convert.ToBoolean(ddlAktiflik.SelectedValue);
                kullanicilar.ProfilResim = yeniDosyaAdi;
                kullanicilar.EkleyenId = currentInfo.KullaniciId;
                kullanicilar.EkleyenIp = currentInfo.Ip;
                //id oluşana kadar geçici bir id veriliyormuş gibi düşünülebilir
                kullanicilar.KullaniciAdi = Guid.NewGuid().ToString();

                if (kullanicilar.Ekle())
                {
                    int yeniId = kullanicilar.MaxIdGetir();
                    kullanicilar.Id = yeniId;
                    kullanicilar.KullaniciAdi = DateTime.Now.Year + yeniId.ToString("D4");

                    if (kullanicilar.KullaniciAdiGuncelle())
                    {
                        sonuc = true;

                        Session["BasariMesaji"] = Mesajlar.KullaniciBasariylaEklendi + "<br/>" + Mesajlar.KullaniciAdiBaslik + "<b>" + kullanicilar.KullaniciAdi + "</b><br/>" + Mesajlar.GeciciSifreBaslik + "<b>" + geciciSifre + "</b>";
                    }
                }
            }

            if (sonuc)
            {
                veritabaniIslemleri.Uygula();

                if (!string.IsNullOrEmpty(gelenId) && fuProfilResmi.HasFile && !string.IsNullOrEmpty(eskiFotoAdi))
                {
                    dosyaIslemleri.ResimSil(eskiFotoAdi);
                }

                Response.Redirect("~/Pages/KullaniciEkle.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            else
            {
                veritabaniIslemleri.GeriAl();

                Mesaj.Ver(Mesajlar.IslemGerceklestirilemedi, Mesaj.MesajTurleri.FAIL, Page.Master);
            }
        }
        catch (Exception ex)
        {
            // İşlemlerden herhangi biri hata verirse
            // yapılan SQL işlemlerini geri al.
            veritabaniIslemleri.GeriAl();
            Mesaj.Ver(Mesajlar.GenelHata + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            // Transaction başarılı veya başarısız
            // her durumda bağlantıyı kapat.
            veritabaniIslemleri.Bitir();
        }
    }

}
