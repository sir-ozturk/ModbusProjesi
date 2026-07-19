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
    public partial class KullaniciListele : System.Web.UI.Page
    {
        SqlBaglanti bglSinifi = new SqlBaglanti();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                Listele();
            }
        }

        private void Listele()
        {
            string sorgu = @"SELECT K.id, K.kullanici_adi, K.ad, K.soyad, K.telefon, K.mail, K.aktiflik_durumu, R.rol_adi
                             FROM Kullanicilar K 
                             INNER JOIN Roller R ON K.rol_id = R.id";

            using (SqlConnection bgl = bglSinifi.Baglanti())
            {
                using (SqlCommand komut = new SqlCommand(sorgu, bgl))
                {
                    try
                    {
                        using (SqlDataReader oku = komut.ExecuteReader())
                        {
                            repeaterKullanicilar.DataSource = oku;
                            repeaterKullanicilar.DataBind();
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('Listeleme Hatası: " + ex.Message + "');</script>");
                    }
                }
            }
        }

        protected void btnTabloSil_Click(object sender, EventArgs e)
        {
            // Tıklanan satırdaki LinkButton'ı yakalıyoruz
            LinkButton btn = (LinkButton)sender;

            // Butonun içine gizlediğimiz ID değerini alıyoruz
            string SilenecekId = btn.CommandArgument;

            if (!string.IsNullOrEmpty(SilenecekId))
            {
                using (SqlConnection bgl = bglSinifi.Baglanti())
                {
                    string silmeSorgusu = "DELETE FROM Kullanicilar WHERE id = @p1";

                    using (SqlCommand komutsil = new SqlCommand(silmeSorgusu, bgl))
                    {
                        komutsil.Parameters.AddWithValue("@p1", SilenecekId);

                        try
                        {
                            komutsil.ExecuteNonQuery();
                            Listele();
                        }
                        catch (Exception ex)
                        {
                            Response.Write("<script>alert('Silme Hatası: " + ex.Message + "');</script>");
                        }
                    }
                }
            }
        }
    }
}