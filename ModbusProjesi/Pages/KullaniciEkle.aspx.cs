using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModbusProjesi.AppCode;
using System.IO;


namespace ModbusProjesi.Pages
{
    public partial class KullaniciEkle : System.Web.UI.Page
    {
        string gelenId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            gelenId = Request.QueryString["id"];

            if (Page.IsPostBack == false)
            {
                try
                {
                    if (Session["BasariMesaji"] != null)
                    {
                        pnlMesaj.Visible = true;
                        pnlMesaj.CssClass = "mesaj-kutusu basarili";
                        lblMesaj.Text = Session["BasariMesaji"].ToString();

                        Session.Remove("BasariMesaji");
                    }

                    Roller roller = new Roller();

                    ddlRoller.DataSource = roller.Listele();
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

                        Kullanicilar kullanicilar = new Kullanicilar();

                        kullanicilar.Id = Convert.ToInt32(gelenId);
                        kullanicilar.Getir();

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
            }
        }


        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAd.Text) ||
                string.IsNullOrEmpty(txtSoyad.Text) ||
                string.IsNullOrEmpty(txtTelefon.Text) ||
                string.IsNullOrEmpty(txtMail.Text) ||
                string.IsNullOrEmpty(txtKullaniciAdi.Text) ||
                ddlRoller.SelectedValue == "0" ||
                ddlAktiflik.SelectedValue == "Seçiniz...")
            {
                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                lblMesaj.Text = "Lütfen tüm alanları eksiksiz doldurunuz, rol ve aktiflik durumu seçiniz!";

                return;
            }

            try
            {
                Kullanicilar kullanicilar = new Kullanicilar();

                if (!string.IsNullOrEmpty(gelenId))
                {
                    kullanicilar.Id = Convert.ToInt32(gelenId);
                }
                else
                {
                    kullanicilar.Id = 0;
                }

                kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                kullanicilar.Telefon = txtTelefon.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();

                if (kullanicilar.KayitVarMi())
                {
                    pnlMesaj.Visible = true;
                    pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                    lblMesaj.Text = "Bu kullanıcı adı, mail adresi veya telefon numarası daha önce kullanılmıştır.";

                    return;
                }

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

                    yeniDosyaAdi = dosyaIslemleri.ResimKaydet(fuProfilResmi.PostedFile);
                }

                // GÜNCELLEME
                if (!string.IsNullOrEmpty(gelenId))
                {

                    // Eski kullanıcı bilgilerini getirir.
                    kullanicilar.Getir();

                    string eskiFotoAdi = kullanicilar.ProfilResim;
                    kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                    kullanicilar.Sifre = txtSifre.Text.Trim();
                    kullanicilar.Ad = txtAd.Text.Trim();
                    kullanicilar.Soyad = txtSoyad.Text.Trim();
                    kullanicilar.Telefon = txtTelefon.Text.Trim();
                    kullanicilar.Mail = txtMail.Text.Trim();
                    kullanicilar.RolId = Convert.ToInt32(ddlRoller.SelectedValue);
                    kullanicilar.AktifMi = Convert.ToBoolean(ddlAktiflik.SelectedValue);

                    // Yeni resim seçilmişse resim adını değiştir.
                    if (fuProfilResmi.HasFile)
                    {
                        kullanicilar.ProfilResim = yeniDosyaAdi;
                    }

                    if (Session["kullaniciId"] != null)
                    {
                        kullanicilar.GuncelleyenId = Convert.ToInt32(Session["kullaniciId"]);
                    }
                    kullanicilar.GuncelleyenIp = Request.UserHostAddress;

                    if (kullanicilar.Guncelle())
                    {
                        // Eski dosyayı siler
                        if (fuProfilResmi.HasFile)
                        {
                            dosyaIslemleri.ResimSil(eskiFotoAdi);
                        }

                        Session["BasariMesaji"] = "Kullanıcı başarıyla güncellendi.";
                        Response.Redirect("~/Pages/KullaniciEkle.aspx");
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

                    kullanicilar.KullaniciAdi = txtKullaniciAdi.Text.Trim();
                    kullanicilar.Sifre = geciciSifre;
                    kullanicilar.Ad = txtAd.Text.Trim();
                    kullanicilar.Soyad = txtSoyad.Text.Trim();
                    kullanicilar.Telefon = txtTelefon.Text.Trim();
                    kullanicilar.Mail = txtMail.Text.Trim();
                    kullanicilar.RolId = Convert.ToInt32(ddlRoller.SelectedValue);
                    kullanicilar.AktifMi = Convert.ToBoolean(ddlAktiflik.SelectedValue);
                    kullanicilar.ProfilResim = yeniDosyaAdi;

                    if (Session["kullaniciId"] != null)
                    {
                        kullanicilar.EkleyenId = Convert.ToInt32(Session["kullaniciId"]);
                    }
                    kullanicilar.EkleyenIp = Request.UserHostAddress;

                    if (kullanicilar.Ekle())
                    {
                        Session["BasariMesaji"] = "Kullanıcı başarıyla eklendi.<br/>Geçici Şifre: <b>" + geciciSifre + "</b>";
                        Response.Redirect("~/Pages/KullaniciEkle.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                lblMesaj.Text = "Hata Oluştu: " + ex.Message;
            }
        }

    }
}
