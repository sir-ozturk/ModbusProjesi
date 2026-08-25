using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class KullaniciListele : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        GridHazirla();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack == false)
        {
            Listele();
        }
    }

    private void GridHazirla()
    {
        ucMyGrid.KolonEkle(Kullanicilar.C_Sutun_id, "ID");
        ucMyGrid.KolonEkle(Kullanicilar.C_Sutun_kullanici_adi, "Kullanıcı Adı");
        ucMyGrid.BirlesikKolonEkle("Ad Soyad", Kullanicilar.C_Sutun_ad, Kullanicilar.C_Sutun_soyad);
        ucMyGrid.FormatliKolonEkle(Kullanicilar.C_Sutun_telefon, "Telefon", ucMyGrid.FormatTip.TELEFON);
        ucMyGrid.KolonEkle(Kullanicilar.C_Sutun_mail, "Mail");
        ucMyGrid.KolonEkle(Roller.C_Sutun_adi, "Rol Adı");
        ucMyGrid.DurumKolonEkle(Kullanicilar.C_Sutun_aktif_mi, "Durum", "Aktif", "Pasif");
        ucMyGrid.ButonEkle("İşlemler", Kullanicilar.C_Sutun_id, ucMyGrid.ButonTip.GUNCELLE, ucMyGrid.ButonTip.SIL);
    }

    private void Listele()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
            kullanicilar.TumunuGetir();

            ucMyGrid.Doldur(kullanicilar.VeriTablosu);
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.ListelemeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void ucMyGrid_ButonTiklandi(object sender, ucMyGrid.MyGridButonEventArgs e)
    {
        if (e.ButonTip == ucMyGrid.ButonTip.GUNCELLE)
        {
            Response.Redirect("~/Pages/KullaniciEkle.aspx?id=" + e.Id, false);

            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (e.ButonTip == ucMyGrid.ButonTip.SIL)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

                kullanicilar.Id = e.Id;
                kullanicilar.Sil();
            }
            catch (Exception ex)
            {
                Mesaj.Ver(Mesajlar.SilmeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }

            Listele();
        }
    }
}
