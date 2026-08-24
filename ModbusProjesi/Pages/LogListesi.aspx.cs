using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class LogListesi : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        GridHazirla();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack == false)
        {
            LogIslemleri.IslemKaydet();

            TabloAdlariniDoldur();
            IslemAdlariniDoldur();
            Listele();
        }
    }

    private void TabloAdlariniDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.TabloAdlarGetir();

            ddlTabloAdi.DataSource = loglar.VeriTablosu;
            ddlTabloAdi.DataTextField = Loglar.C_Sutun_tablo_adi;
            ddlTabloAdi.DataValueField = Loglar.C_Sutun_tablo_adi;
            ddlTabloAdi.DataBind();

            ddlTabloAdi.Items.Insert(0, new ListItem("Tümü", ""));
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

    private void IslemAdlariniDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.IslemAdlarGetir();

            ddlIslemAdi.DataSource = loglar.VeriTablosu;
            ddlIslemAdi.DataTextField = Loglar.C_Sutun_islem_adi;
            ddlIslemAdi.DataValueField = Loglar.C_Sutun_islem_adi;
            ddlIslemAdi.DataBind();

            ddlIslemAdi.Items.Insert(0, new ListItem("Tümü", ""));
        }
        catch (Exception ex)
        {
            Mesaj.Ver(
                Mesajlar.ListelemeHatasi + ex.Message,
                Mesaj.MesajTurleri.FAIL,
                Page.Master
            );
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    private void GridHazirla()
    {
        ucLogGrid.KolonEkle(Loglar.C_Sutun_id, "ID");
        ucLogGrid.KolonEkle(Loglar.C_Sutun_kullanici_id, "Kullanıcı ID");
        ucLogGrid.KolonEkle(Kullanicilar.C_Sutun_kullanici_adi, "Kullanıcı Adı");
        ucLogGrid.BirlesikKolonEkle("Ad Soyad", Kullanicilar.C_Sutun_ad, Kullanicilar.C_Sutun_soyad);
        ucLogGrid.KolonEkle(Loglar.C_Sutun_tablo_adi, "Tablo Adı");
        ucLogGrid.KolonEkle(Loglar.C_Sutun_islem_adi, "İşlem Adı");
        ucLogGrid.KolonEkle(Loglar.C_Sutun_islem_tipi, "İşlem Tipi");
        ucLogGrid.KolonEkle(Loglar.C_Sutun_ip_adres, "IP Adresi");
        ucLogGrid.KolonEkle(Loglar.C_Sutun_islem_tarihi, "İşlem Tarihi");

        ucLogGrid.ButonEkle("İşlemler", Loglar.C_Sutun_id, ucMyGrid.ButonTip.DETAY);
    }

    private void Listele()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            loglar.TumunuGetir();

            ucLogGrid.Doldur(loglar.VeriTablosu);
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

    protected void ucLogGrid_ButonTiklandi(object sender, ucMyGrid.MyGridButonEventArgs e)
    {
        if (e.ButonTip == ucMyGrid.ButonTip.DETAY)
        {
            Response.Redirect("~/Pages/LogDetay.aspx?id=" + e.Id, false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
    }

    protected void btnFiltrele_Click(object sender, EventArgs e)
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Loglar loglar = new Loglar(veritabaniIslemleri);

            DateTime baslangic;
            DateTime bitis;

            DateTime? baslangicTarih = null;
            DateTime? bitisTarih = null;

            if (DateTime.TryParse(txtBaslangicTarih.Text, out baslangic))
            {
                baslangicTarih = baslangic;
            }

            if (DateTime.TryParse(txtBitisTarih.Text, out bitis))
            {
                bitisTarih = bitis;
            }

            if (baslangicTarih.HasValue && bitisTarih.HasValue)
            {
                if (baslangicTarih.Value > bitisTarih.Value)
                {
                    Mesaj.Ver(Mesajlar.BaslangicTarihiBitisTarihindenBuyukOlamaz, Mesaj.MesajTurleri.WARNING, Page.Master);

                    return;
                }
            }

            loglar.FiltreliGetir(txtKullanici.Text.Trim(), ddlTabloAdi.SelectedValue, ddlIslemAdi.SelectedValue, ddlIslemTipi.SelectedValue, baslangicTarih, bitisTarih);

            ucLogGrid.Doldur(loglar.VeriTablosu);
        }
        catch (Exception ex)
        {
            Mesaj.Ver(
                Mesajlar.ListelemeHatasi + ex.Message,
                Mesaj.MesajTurleri.FAIL,
                Page.Master
            );
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnTemizle_Click(object sender, EventArgs e)
    {
        txtKullanici.Text = "";

        ddlTabloAdi.SelectedIndex = 0;
        ddlIslemAdi.SelectedIndex = 0;
        ddlIslemTipi.SelectedIndex = 0;

        txtBaslangicTarih.Text = "";
        txtBitisTarih.Text = "";

        Listele();
    }
}
