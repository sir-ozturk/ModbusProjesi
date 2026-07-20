using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ModbusProjesi.AppCode;
using System.IO;

namespace ModbusProjesi.Pages
{
    public partial class KullaniciEkle : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();
        string gelenId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            gelenId = Request.QueryString["id"];

            if (Page.IsPostBack == false)
            {
                if (Session["BasariMesaji"] != null)
                {
                    pnlMesaj.Visible = true;
                    pnlMesaj.CssClass = "mesaj-kutusu basarili";
                    lblMesaj.Text = Session["BasariMesaji"].ToString();
                    Session.Remove("BasariMesaji");
                }

                using (SqlConnection sqlConnection = sqlBaglanti.Baglanti())
                {
                    string sorguRoller = "Select id, rol_adi From Roller";
                    string sorguGetir = "Select * From Kullanicilar Where id=@p1";

                    using (SqlCommand sqlCommand1 = new SqlCommand(sorguRoller, sqlConnection))
                    {
                        try
                        {
                            using (SqlDataReader sqlDataReader = sqlCommand1.ExecuteReader())
                            {
                                ddlRoller.DataSource = sqlDataReader;
                                ddlRoller.DataTextField = "rol_adi";
                                ddlRoller.DataValueField = "id";
                                ddlRoller.DataBind();
                            }
                            ddlRoller.Items.Insert(0, new ListItem("Rol Seçiniz...", "0"));

                            if (!string.IsNullOrEmpty(gelenId))
                            {
                                litSayfaBaslik.Text = "Kullanıcı Bilgilerini Güncelle";
                                btnKaydet.Text = "Güncelle";

                                phYeniKayitNotu.Visible = false;
                                phGuncellemeSifreAlani.Visible = true;

                                using (SqlCommand sqlCommand = new SqlCommand(sorguGetir, sqlConnection))
                                {
                                    sqlCommand.Parameters.AddWithValue("@p1", gelenId);

                                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                                    {
                                        if (sqlDataReader.Read())
                                        {
                                            txtAd.Text = sqlDataReader["ad"].ToString();
                                            txtSoyad.Text = sqlDataReader["soyad"].ToString();
                                            txtTelefon.Text = sqlDataReader["telefon"].ToString();
                                            txtMail.Text = sqlDataReader["mail"].ToString();
                                            txtKullaniciAdi.Text = sqlDataReader["kullanici_adi"].ToString();
                                            txtSifre.Text = sqlDataReader["sifre"].ToString();
                                            ddlRoller.SelectedValue = sqlDataReader["rol_id"].ToString();
                                            bool durum = Convert.ToBoolean(sqlDataReader["aktiflik_durumu"]);
                                            ddlAktiflik.SelectedValue = durum.ToString();
                                            string fotoAdi = sqlDataReader["profil_resim"].ToString();
                                            if (!string.IsNullOrEmpty(fotoAdi))
                                            {
                                                imgProfil.ImageUrl = "~/Files/" + fotoAdi;
                                                imgProfil.Style["display"] = "inline-block";
                                            }
                                        }
                                    }
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
                using (SqlConnection sqlConnection = sqlBaglanti.Baglanti())
                {
                    string yeniDosyaAdi = "";

                    if (fuProfilResmi.HasFile)
                    {
                        string uzanti = Path.GetExtension(fuProfilResmi.FileName).ToLower();
                        if (uzanti == ".jpg" || uzanti == ".jpeg" || uzanti == ".png")
                        {
                            yeniDosyaAdi = Guid.NewGuid().ToString() + uzanti;
                            string yuklemeYolu = Server.MapPath("~/Files/") + yeniDosyaAdi;
                            fuProfilResmi.SaveAs(yuklemeYolu);
                        }
                        else
                        {
                            pnlMesaj.Visible = true;
                            pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                            lblMesaj.Text = "Lütfen sadece .jpg, .jpeg veya .png uzantılı fotoğraflar seçiniz.";
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(gelenId))
                    {
                        // ESKİ FOTOĞRAFI BULUP SİLME VE SUNUCUYU KORUMA MANTIĞI
                        if (fuProfilResmi.HasFile)
                        {
                            string eskiFotoSorgu = "Select profil_resim From Kullanicilar Where id=@id";
                            using (SqlCommand sqlCommand = new SqlCommand(eskiFotoSorgu, sqlConnection))
                            {
                                sqlCommand.Parameters.AddWithValue("@id", gelenId);
                                object eskiFotoAd = sqlCommand.ExecuteScalar();

                                if (eskiFotoAd != null && eskiFotoAd != DBNull.Value && !string.IsNullOrEmpty(eskiFotoAd.ToString()))
                                {
                                    string resimYolu = Server.MapPath("~/Files/") + eskiFotoAd.ToString();
                                    if (File.Exists(resimYolu))
                                    {
                                        File.Delete(resimYolu);
                                    }
                                }
                            }
                        }

                        string sorguGuncelle = "";
                        if (string.IsNullOrEmpty(txtSifre.Text))
                        {
                            sorguGuncelle = fuProfilResmi.HasFile
                                ? @"Update Kullanicilar Set kullanici_adi=@p1, ad=@p2, soyad=@p3, telefon=@p4, mail=@p5, rol_id=@p6, aktiflik_durumu=@p7, profil_resim=@foto where id=@p8"
                                : @"Update Kullanicilar Set kullanici_adi=@p1, ad=@p2, soyad=@p3, telefon=@p4, mail=@p5, rol_id=@p6, aktiflik_durumu=@p7 where id=@p8";
                        }
                        else
                        {
                            sorguGuncelle = fuProfilResmi.HasFile
                                ? @"Update Kullanicilar Set kullanici_adi=@p1, ad=@p2, soyad=@p3, telefon=@p4, mail=@p5, rol_id=@p6, aktiflik_durumu=@p7, sifre=@sifre, profil_resim=@foto where id=@p8"
                                : @"Update Kullanicilar Set kullanici_adi=@p1, ad=@p2, soyad=@p3, telefon=@p4, mail=@p5, rol_id=@p6, aktiflik_durumu=@p7, sifre=@sifre where id=@p8";
                        }

                        using (SqlCommand sqlCommand = new SqlCommand(sorguGuncelle, sqlConnection))
                        {
                            if (!string.IsNullOrEmpty(txtSifre.Text))
                            {
                                sqlCommand.Parameters.AddWithValue("@sifre", txtSifre.Text.Trim());
                            }
                            if (fuProfilResmi.HasFile)
                            {
                                sqlCommand.Parameters.AddWithValue("@foto", yeniDosyaAdi);
                            }

                            sqlCommand.Parameters.AddWithValue("@p1", txtKullaniciAdi.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p2", txtAd.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p3", txtSoyad.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p4", txtTelefon.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p5", txtMail.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p6", ddlRoller.SelectedValue);
                            sqlCommand.Parameters.AddWithValue("@p7", Convert.ToBoolean(ddlAktiflik.SelectedValue));
                            sqlCommand.Parameters.AddWithValue("@p8", gelenId);

                            sqlCommand.ExecuteNonQuery();
                        }

                        Response.Redirect("~/Pages/KullaniciEkle.aspx");
                    }
                    else
                    {
                        Random random = new Random();

                        string[] harfler = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                             "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};

                        string rastgeleHarf1 = harfler[random.Next(0, harfler.Length)];
                        string rastgeleHarf2 = harfler[random.Next(0, harfler.Length)];
                        string rastgeleHarf3 = harfler[random.Next(0, harfler.Length)];
                        string rastgeleHarf4 = harfler[random.Next(0, harfler.Length)];

                        string[] karakterler = { "!", "?", "*", "-", "_", "+", "#", "$" };
                        string rastgeleKarakter = karakterler[random.Next(0, karakterler.Length)];

                        int rastgeleSayi = random.Next(1000, 999999);

                        string geciciSifre = rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;

                        string sorguEkle = @"Insert Into Kullanicilar 
                                            (kullanici_adi, sifre, ad, soyad, telefon, mail, rol_id, aktiflik_durumu, profil_resim) 
                                            Values (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @foto)";

                        using (SqlCommand sqlCommand = new SqlCommand(sorguEkle, sqlConnection))
                        {
                            sqlCommand.Parameters.AddWithValue("@p1", txtKullaniciAdi.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p2", geciciSifre);
                            sqlCommand.Parameters.AddWithValue("@p3", txtAd.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p4", txtSoyad.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p5", txtTelefon.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p6", txtMail.Text.Trim());
                            sqlCommand.Parameters.AddWithValue("@p7", ddlRoller.SelectedValue);
                            sqlCommand.Parameters.AddWithValue("@p8", Convert.ToBoolean(ddlAktiflik.SelectedValue));
                            sqlCommand.Parameters.AddWithValue("@foto", fuProfilResmi.HasFile ? (object)yeniDosyaAdi : DBNull.Value);

                            sqlCommand.ExecuteNonQuery();

                            Session["BasariMesaji"] = "Yeni kullanıcı başarıyla eklendi! Geçici Şifre: " + geciciSifre;
                            Response.Redirect("~/Pages/KullaniciEkle.aspx");
                        }
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