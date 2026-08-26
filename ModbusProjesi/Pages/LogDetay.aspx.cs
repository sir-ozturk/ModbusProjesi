using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class LogDetay : System.Web.UI.Page
{
    private int gelenId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (int.TryParse(Request.QueryString["id"], out gelenId) == false)
        {
            Response.Redirect("~/Pages/LogListele.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (Page.IsPostBack == false)
        {
            LogIslemleri.IslemKaydet();

            Doldur();
        }
    }

    private void Doldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.Id = gelenId;

            if (loglar.Doldur() == false)
            {
                Mesaj.Ver(Mesajlar.KayitBulunamadi, Mesaj.MesajTurleri.FAIL, Page.Master);

                return;
            }

            txtKullaniciId.Text = loglar.Kullanici_id.ToString();
            txtUrl.Text = loglar.Url;
            txtTabloAdi.Text = loglar.Tablo_adi;
            txtIslemAdi.Text = loglar.Islem_adi;
            txtIslemTipi.Text = loglar.Islem_tipi;
            txtIpAdresi.Text = loglar.Ip_adres;
            txtIslemTarihi.Text = loglar.Islem_tarihi.ToString("dd.MM.yyyy HH:mm:ss");
            txtDetay.Text = loglar.Detay;
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.KayitGetirmeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}
