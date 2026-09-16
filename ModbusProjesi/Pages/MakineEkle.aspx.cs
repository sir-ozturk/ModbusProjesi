using System;
using System.Web.UI;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public partial class MakineEkle : System.Web.UI.Page
{
    private int gelenId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.GORUNTULE))
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (Request.QueryString["id"] != null)
        {
            int.TryParse(Request.QueryString["id"], out gelenId);
        }

        if (Page.IsPostBack == false)
        {
            if (gelenId > 0)
            {
                litSayfaBaslik.Text = "Makine Düzenleme Paneli";
                btnKaydet.Text = "Güncelle";

                btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.GUNCELLE);

                MakineDoldur();
            }
            else
            {
                litSayfaBaslik.Text = "Makine Ekleme Paneli";
                btnKaydet.Text = "Kaydet";

                btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.EKLE);

                ddlAktiflik.SelectedValue = "";
            }
        }
    }

    private void MakineDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Makineler makineler = new Makineler(veritabaniIslemleri);

            makineler.Id = gelenId;

            if (makineler.Doldur())
            {
                txtModelAd.Text = makineler.ModelAd;
                txtEntegrasyonKod.Text = makineler.EntegrasyonKod;
                txtGgNo.Text = makineler.GgNo;
                txtMakineNo.Text = makineler.MakineNo;
                txtMakineAdi.Text = makineler.MakineAdi;
                txtBandNo.Text = makineler.BandNo;
                txtIp.Text = makineler.Ip;
                txtMfg.Text = makineler.Mfg;

                ddlAktiflik.SelectedValue = makineler.AktifMi ? "1" : "0";
            }
            else
            {
                Mesaj.Ver(Mesajlar.ListelemeHatasi, Mesaj.MesajTurleri.FAIL, Page.Master);
            }
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

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (gelenId == 0)
        {
            if (!IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.EKLE))
            {
                Mesaj.Ver(Mesajlar.YetkinizYok, Mesaj.MesajTurleri.WARNING, Page.Master);

                return;
            }
        }
        else
        {
            if (!IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.GUNCELLE))
            {
                Mesaj.Ver(Mesajlar.YetkinizYok, Mesaj.MesajTurleri.WARNING, Page.Master);

                return;
            }
        }

        if (string.IsNullOrEmpty(txtMakineAdi.Text.Trim()) ||
            string.IsNullOrEmpty(txtModelAd.Text.Trim()) ||
            string.IsNullOrEmpty(txtEntegrasyonKod.Text.Trim()) ||
            string.IsNullOrEmpty(txtGgNo.Text.Trim()) ||
            string.IsNullOrEmpty(txtMakineNo.Text.Trim()) ||
            string.IsNullOrEmpty(txtBandNo.Text.Trim()) ||
            string.IsNullOrEmpty(txtIp.Text.Trim()) ||
            string.IsNullOrEmpty(txtMfg.Text.Trim()) ||
            string.IsNullOrEmpty(ddlAktiflik.SelectedValue))
        {
            Mesaj.Ver(Mesajlar.MakineAlanlarBos, Mesaj.MesajTurleri.WARNING, Page.Master);

            return;
        }

        IPAddress ipAdres;

        if (!IPAddress.TryParse(txtIp.Text.Trim(), out ipAdres) ||
            ipAdres.AddressFamily != AddressFamily.InterNetwork)
        {
            Mesaj.Ver(Mesajlar.GecersizIpAdresi, Mesaj.MesajTurleri.WARNING, Page.Master);

            return;
        }

        if (!txtGgNo.Text.Trim().All(char.IsDigit) ||
            !txtMakineNo.Text.Trim().All(char.IsDigit) ||
            !txtMfg.Text.Trim().All(char.IsDigit))
        {
            Mesaj.Ver(Mesajlar.MakineSayisalAlanHatasi, Mesaj.MesajTurleri.WARNING, Page.Master);

            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Makineler makineler = new Makineler(veritabaniIslemleri);

            makineler.ModelAd = txtModelAd.Text.Trim();
            makineler.EntegrasyonKod = txtEntegrasyonKod.Text.Trim();
            makineler.GgNo = txtGgNo.Text.Trim();
            makineler.MakineNo = txtMakineNo.Text.Trim();
            makineler.MakineAdi = txtMakineAdi.Text.Trim();
            makineler.BandNo = txtBandNo.Text.Trim();
            makineler.Ip = txtIp.Text.Trim();
            makineler.Mfg = txtMfg.Text.Trim();
            makineler.AktifMi = ddlAktiflik.SelectedValue == "1";

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            makineler.Id = gelenId;
            if (makineler.KayitVarMi())
            {
                Mesaj.Ver(Mesajlar.MakineKayitli, Mesaj.MesajTurleri.WARNING, Page.Master);

                return;
            }


            if (gelenId == 0)
            {
                makineler.EkleyenId = currentInfo.KullaniciId;
                makineler.EkleyenIp = Utility.IpNoGetir();

                if (makineler.Ekle())
                {
                    Session["BasariMesaji"] = Mesajlar.MakineBasariylaEklendi;

                    Response.Redirect("~/Pages/MakineListele.aspx", false);

                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            else
            {
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = Utility.IpNoGetir();

                if (makineler.Guncelle())
                {
                    Session["BasariMesaji"] = Mesajlar.MakineBasariylaGuncellendi;

                    Response.Redirect("~/Pages/MakineListele.aspx", false);

                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }

            Mesaj.Ver(Server.HtmlEncode(veritabaniIslemleri.SonHataMesaji ?? Mesajlar.MakineGuncellemeHatasi), Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.GenelHata + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}
