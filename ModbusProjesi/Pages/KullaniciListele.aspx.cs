using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLayer.Entity;
using BusinessLayer.Interfaces;
using BusinessLayer.Work;

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
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                KullaniciIslemleri kullaniciIslemleri = new KullaniciIslemleri(veritabaniIslemleri);
                repeaterKullanicilar.DataSource = kullaniciIslemleri.Listele();
                repeaterKullanicilar.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Listeleme Hatası: " + ex.Message + "');</script>");
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public string TelefonFormatla(string telefon)
        {
            if (string.IsNullOrEmpty(telefon) || telefon.Length != 10)
            {
                return telefon;
            }

            return telefon.Substring(0, 3) + "-" +
                   telefon.Substring(3, 3) + "-" +
                   telefon.Substring(6, 2) + "-" +
                   telefon.Substring(8, 2);
        }

        protected void btnTabloSil_Click(object sender, EventArgs e)
        {
            // Tıklanan satırdaki LinkButton'ı yakalıyoruz
            LinkButton linkButton = (LinkButton)sender;

            // Butonun içine gizlediğimiz ID değerini alıyoruz
            int Id = Convert.ToInt32(linkButton.CommandArgument);

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar();
                kullanicilar.Id = Id;
                KullaniciIslemleri kullaniciIslemleri = new KullaniciIslemleri(veritabaniIslemleri);
                kullaniciIslemleri.Sil(kullanicilar);
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Silme Hatası : " + ex.Message + "');</script>");
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }

            Listele();

        }
    }
}