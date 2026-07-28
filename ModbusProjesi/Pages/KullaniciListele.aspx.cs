using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModbusProjesi.AppCode;

namespace ModbusProjesi.Pages
{
    public partial class KullaniciListele : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                Listele();
            }
        }

        private void Listele()
        {
            try
            {
                Kullanicilar kullanicilar = new Kullanicilar();
                repeaterKullanicilar.DataSource = kullanicilar.Listele();
                repeaterKullanicilar.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Listeleme Hatası: " + ex.Message + "');</script>");
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
                try
                {
                    Kullanicilar kullanicilar = new Kullanicilar();
                    kullanicilar.Id = Convert.ToInt32(SilenecekId);
                    kullanicilar.Sil();
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