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
                    pnlMesaj.Visible = true;
                    pnlMesaj.CssClass = "mesaj-kutusu basarili";
                    lblMesaj.Text = Session["BasariMesaji"].ToString();

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
                    if (!string.IsNullOrEmpty(kullanicilar.Telefon) &&
                        kullanicilar.Telefon.Length == 10)
                    {
                        txtTelefon.Text =
                            kullanicilar.Telefon.Substring(0, 3) + "-" +
                            kullanicilar.Telefon.Substring(3, 3) + "-" +
                            kullanicilar.Telefon.Substring(6, 2) + "-" +
                            kullanicilar.Telefon.Substring(8, 2);
                    }
                    else
                    {
                        txtTelefon.Text = kullanicilar.Telefon;
                    }
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
                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";

                lblMesaj.Text = "Veriler yüklenirken hata oluştu: " + ex.Message;
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
            pnlMesaj.Visible = true;
            pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
            lblMesaj.Text = "Lütfen tüm alanları eksiksiz doldurunuz, rol ve aktiflik durumu seçiniz!";

            return;
        }

        Sessionlar sessionlar = new Sessionlar();
        CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
        DosyaIslemleri dosyaIslemleri = new DosyaIslemleri();

        string yeniDosyaAdi = "";

        // Resim seçilmişse uzantısını kontrol et ve kaydet
        if (fuProfilResmi.HasFile)
        {
            string uzanti =
                Path.GetExtension(fuProfilResmi.FileName).ToLower();

            if (uzanti != ".jpg" &&
                uzanti != ".jpeg" &&
                uzanti != ".png")
            {
                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                lblMesaj.Text = "Lütfen sadece .jpg, .jpeg veya .png uzantılı fotoğraflar seçiniz.";

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

                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                lblMesaj.Text = "Bu mail adresi veya telefon numarası daha önce kullanılmıştır.";

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

                    Session["BasariMesaji"] = "Kullanıcı başarıyla güncellendi.";
                }
            }

            // YENİ KULLANICI EKLEME
            else
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

                string geciciSifre = rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;

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

                        Session["BasariMesaji"] = "Kullanıcı başarıyla eklendi.<br/>" + "Kullanıcı Adı: <b>" + kullanicilar.KullaniciAdi + "</b><br/>" + "Geçici Şifre: <b>" + geciciSifre + "</b>";
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

                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                lblMesaj.Text = "İşlem gerçekleştirilemedi.";
            }
        }
        catch (Exception ex)
        {
            // İşlemlerden herhangi biri hata verirse
            // yapılan SQL işlemlerini geri al.
            veritabaniIslemleri.GeriAl();

            pnlMesaj.Visible = true;
            pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
            lblMesaj.Text = "Hata Oluştu: " + ex.Message;
        }
        finally
        {
            // Transaction başarılı veya başarısız
            // her durumda bağlantıyı kapat.
            veritabaniIslemleri.Bitir();
        }
    }

}
