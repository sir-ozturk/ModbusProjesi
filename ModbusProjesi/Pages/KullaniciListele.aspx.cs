using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
            kullanicilar.TumunuGetir();
            repeaterKullanicilar.DataSource = kullanicilar.VeriTablosu;
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

    public string TelefonFormatla(object telefonObj)
    {
        if (telefonObj == null || telefonObj == DBNull.Value)
            return string.Empty;

        string telefon = telefonObj.ToString();

        if (telefon.Length != 10)
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
            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
            kullanicilar.Id = Id;
            kullanicilar.Sil();
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
