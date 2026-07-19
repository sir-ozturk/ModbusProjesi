using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ModbusProjesi.AppCode;

namespace ModbusProjesi.Pages
{
    public partial class KullaniciEkle : System.Web.UI.Page
    {
        SqlBaglanti bglSinifi = new SqlBaglanti();
        string GelenId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            GelenId = Request.QueryString["id"];

            if (Page.IsPostBack == false)
            {
                if (Session["BasariMesaji"] != null)
                {
                    pnlMesaj.Visible = true;
                    pnlMesaj.CssClass = "mesaj-kutusu basarili";
                    lblMesaj.Text = Session["BasariMesaji"].ToString();

                    Session.Remove("BasariMesaji");
                }

                using (SqlConnection bgl = bglSinifi.Baglanti())
                {
                    string sorguRoller = "Select id, rol_adi From Roller";
                    string sorguGetir = "Select * From Kullanicilar Where id=@p1";

                    using (SqlCommand komutRoller = new SqlCommand(sorguRoller, bgl))
                    {
                        try
                        {
                            using (SqlDataReader dr = komutRoller.ExecuteReader())
                            {
                                ddlRol.DataSource = dr;
                                ddlRol.DataTextField = "rol_adi";
                                ddlRol.DataValueField = "id";
                                ddlRol.DataBind();
                            }
                            ddlRol.Items.Insert(0, new ListItem("Rol Seçiniz...", "0"));

                            if (!string.IsNullOrEmpty(GelenId))
                            {
                                litSayfaBaslik.Text = "Kullanıcı Bilgilerini Güncelle";
                                btnKaydet.Text = "Güncelle";

                                phYeniKayitNotu.Visible = false;
                                phGuncellemeSifreAlani.Visible = true;

                                using (SqlCommand komutGetir = new SqlCommand(sorguGetir, bgl))
                                {
                                    komutGetir.Parameters.AddWithValue("@p1", GelenId);

                                    using (SqlDataReader drKullanici = komutGetir.ExecuteReader())
                                    {
                                        if (drKullanici.Read())
                                        {
                                            txtAd.Text = drKullanici["ad"].ToString();
                                            txtSoyad.Text = drKullanici["soyad"].ToString();
                                            txtTelefon.Text = drKullanici["telefon"].ToString();
                                            txtMail.Text = drKullanici["mail"].ToString();
                                            txtKullaniciAdi.Text = drKullanici["kullanici_adi"].ToString();
                                            txtSifre.Text = drKullanici["sifre"].ToString();
                                            ddlRol.SelectedValue = drKullanici["rol_id"].ToString();
                                            bool durum = Convert.ToBoolean(drKullanici["aktiflik_durumu"]);
                                            ddlAktiflik.SelectedValue = durum.ToString();
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
                ddlRol.SelectedValue == "0" ||
                ddlAktiflik.SelectedValue == "Seçiniz...")
            {
                pnlMesaj.Visible = true;
                pnlMesaj.CssClass = "mesaj-kutusu basarisiz";
                lblMesaj.Text = "Lütfen tüm alanları eksiksiz doldurunuz, rol ve aktiflik durumu seçiniz!";
                return;
            }

            try
            {
                using (SqlConnection bgl = bglSinifi.Baglanti())
                {
                    if (!string.IsNullOrEmpty(GelenId))
                    {
                        string sorguGuncelle = "";

                        if (string.IsNullOrEmpty(txtSifre.Text))
                        {
                            sorguGuncelle = @"Update Kullanicilar Set 
                                             kullanici_adi=@p1, ad=@p2, soyad=@p3, telefon=@p4, mail=@p5, rol_id=@p6, aktiflik_durumu=@p7 
                                             where id=@p8";
                        }
                        else
                        {
                            sorguGuncelle = @"Update Kullanicilar Set 
                                             kullanici_adi=@p1, ad=@p2, soyad=@p3, telefon=@p4, mail=@p5, rol_id=@p6, aktiflik_durumu=@p7, sifre=@sifre 
                                             where id=@p8";
                        }

                        using (SqlCommand komutGuncelle = new SqlCommand(sorguGuncelle, bgl))
                        {
                            if (!string.IsNullOrEmpty(txtSifre.Text))
                            {
                                komutGuncelle.Parameters.AddWithValue("@sifre", txtSifre.Text.Trim());
                            }

                            komutGuncelle.Parameters.AddWithValue("@p1", txtKullaniciAdi.Text.Trim());
                            komutGuncelle.Parameters.AddWithValue("@p2", txtAd.Text.Trim());
                            komutGuncelle.Parameters.AddWithValue("@p3", txtSoyad.Text.Trim());
                            komutGuncelle.Parameters.AddWithValue("@p4", txtTelefon.Text.Trim());
                            komutGuncelle.Parameters.AddWithValue("@p5", txtMail.Text.Trim());
                            komutGuncelle.Parameters.AddWithValue("@p6", ddlRol.SelectedValue);
                            komutGuncelle.Parameters.AddWithValue("@p7", Convert.ToBoolean(ddlAktiflik.SelectedValue));
                            komutGuncelle.Parameters.AddWithValue("@p8", GelenId);

                            komutGuncelle.ExecuteNonQuery();
                        }

                        Response.Redirect("~/Pages/KullaniciListele.aspx");
                    }

                    else
                    {
                        Random rnd = new Random();
                        
                        string[] harfler1 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                        string rastgeleHarf1 = harfler1[rnd.Next(0, harfler1.Length)];

                        string[] harfler2 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                        string rastgeleHarf2 = harfler2[rnd.Next(0, harfler2.Length)];

                        string[] harfler3 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                        string rastgeleHarf3 = harfler3[rnd.Next(0, harfler3.Length)];

                        string[] harfler4 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                              "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                        string rastgeleHarf4 = harfler4[rnd.Next(0, harfler4.Length)];

                        string[] karakterler = { "!", "?", "*", "-", "_", "+", "#", "$" };
                        string rastgeleKarakter = karakterler[rnd.Next(0, karakterler.Length)];

                        int rastgeleSayi = rnd.Next(1000, 999999);
                        string geciciSifre = rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;

                        string sorguEkle = @"Insert Into Kullanicilar 
                                            (kullanici_adi, sifre, ad, soyad, telefon, mail, rol_id, aktiflik_durumu) 
                                            Values (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

                        using (SqlCommand komutEkle = new SqlCommand(sorguEkle, bgl))
                        {
                            komutEkle.Parameters.AddWithValue("@p1", txtKullaniciAdi.Text.Trim());
                            komutEkle.Parameters.AddWithValue("@p2", geciciSifre);
                            komutEkle.Parameters.AddWithValue("@p3", txtAd.Text.Trim());
                            komutEkle.Parameters.AddWithValue("@p4", txtSoyad.Text.Trim());
                            komutEkle.Parameters.AddWithValue("@p5", txtTelefon.Text.Trim());
                            komutEkle.Parameters.AddWithValue("@p6", txtMail.Text.Trim());
                            komutEkle.Parameters.AddWithValue("@p7", ddlRol.SelectedValue);
                            komutEkle.Parameters.AddWithValue("@p8", Convert.ToBoolean(ddlAktiflik.SelectedValue));

                            komutEkle.ExecuteNonQuery();

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