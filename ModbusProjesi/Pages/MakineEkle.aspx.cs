using System;
using System.Web.UI;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Data;
using System.Web.UI.WebControls;

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
                ModelleriDoldur();
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
                ModelleriDoldur(veritabaniIslemleri, makineler.ModelAd);
                txtEntegrasyonKod.Text = makineler.EntegrasyonKod;
                txtGgNo.Text = makineler.GgNo;
                txtMakineNo.Text = makineler.MakineNo;
                txtMakineAdi.Text = makineler.MakineAdi;
                txtBandNo.Text = makineler.BandNo;
                txtIp.Text = makineler.Ip;
                txtMfg.Text = makineler.Mfg;
                if (makineler.AktifMi)
                {
                    ddlAktiflik.SelectedValue = "1";
                }
                else
                {
                    ddlAktiflik.SelectedValue = "0";
                }
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

    private void ModelleriDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            ModelleriDoldur(veritabaniIslemleri, null);
        }
        catch (Exception ex)
        {
            btnKaydet.Enabled = false;
            Mesaj.Ver(Mesajlar.ListelemeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    private void ModelleriDoldur(VeritabaniIslemleri veritabaniIslemleri, string mevcutModel)
    {
        ddlModelAd.Items.Clear();
        ddlModelAd.Items.Add(new ListItem("Model seçiniz", ""));
        Parametreler parametreler = new Parametreler(veritabaniIslemleri);
        using (DataTable tablo = parametreler.GrubaGoreGetir(ParametreGruplari.C_Grup_MakineModeli))
        {
            foreach (DataRow satir in tablo.Rows)
            {
                string modelAdi = satir[Parametreler.C_Sutun_adi].ToString();
                if (!string.IsNullOrWhiteSpace(modelAdi) && modelAdi.Length <= 100 && ddlModelAd.Items.FindByValue(modelAdi) == null)
                {
                    ddlModelAd.Items.Add(new ListItem(modelAdi, modelAdi));
                }
            }
        }

        // Pasif veya listede bulunmayan eski model düzenlemede korunur.
        if (!string.IsNullOrWhiteSpace(mevcutModel))
        {
            if (ddlModelAd.Items.FindByValue(mevcutModel) == null)
            {
                ddlModelAd.Items.Add(new ListItem(mevcutModel + " (Mevcut model)", mevcutModel));
            }

            ddlModelAd.SelectedValue = mevcutModel;
        }
    }

    private bool ModelSeciminiKontrol(VeritabaniIslemleri veritabaniIslemleri)
    {
        string modelAdi = ddlModelAd.SelectedValue;
        if (string.IsNullOrWhiteSpace(modelAdi) || modelAdi.Length > 100)
        {
            return false;
        }

        if (gelenId > 0)
        {
            Makineler mevcutMakine = new Makineler(veritabaniIslemleri);
            mevcutMakine.Id = gelenId;
            if (!mevcutMakine.Doldur())
            {
                return false;
            }

            if (string.Equals(mevcutMakine.ModelAd, modelAdi, StringComparison.Ordinal))
            {
                return true;
            }
        }

        Parametreler parametreler = new Parametreler(veritabaniIslemleri);
        using (DataTable tablo = parametreler.GrubaGoreGetir(ParametreGruplari.C_Grup_MakineModeli))
        {
            foreach (DataRow satir in tablo.Rows)
            {
                if (string.Equals(satir[Parametreler.C_Sutun_adi].ToString(), modelAdi, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
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

        if (string.IsNullOrEmpty(txtMakineAdi.Text.Trim())
            || string.IsNullOrEmpty(ddlModelAd.SelectedValue)
            || string.IsNullOrEmpty(txtEntegrasyonKod.Text.Trim())
            || string.IsNullOrEmpty(txtGgNo.Text.Trim())
            || string.IsNullOrEmpty(txtMakineNo.Text.Trim())
            || string.IsNullOrEmpty(txtBandNo.Text.Trim())
            || string.IsNullOrEmpty(txtIp.Text.Trim())
            || string.IsNullOrEmpty(txtMfg.Text.Trim())
            || string.IsNullOrEmpty(ddlAktiflik.SelectedValue))
        {
            Mesaj.Ver(Mesajlar.MakineAlanlarBos, Mesaj.MesajTurleri.WARNING, Page.Master);
            return;
        }

        IPAddress ipAdres;
        if (!IPAddress.TryParse(txtIp.Text.Trim(), out ipAdres) || ipAdres.AddressFamily != AddressFamily.InterNetwork)
        {
            Mesaj.Ver(Mesajlar.GecersizIpAdresi, Mesaj.MesajTurleri.WARNING, Page.Master);
            return;
        }

        if (!txtGgNo.Text.Trim().All(char.IsDigit) || !txtMakineNo.Text.Trim().All(char.IsDigit) || !txtMfg.Text.Trim().All(char.IsDigit))
        {
            Mesaj.Ver(Mesajlar.MakineSayisalAlanHatasi, Mesaj.MesajTurleri.WARNING, Page.Master);
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Makineler makineler = new Makineler(veritabaniIslemleri);
            if (!ModelSeciminiKontrol(veritabaniIslemleri))
            {
                Mesaj.Ver("Aktif bir makine modeli seçiniz. Mevcut makinenin eski modeli korunabilir.", Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }

            makineler.ModelAd = ddlModelAd.SelectedValue;
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

            var islemHataMesaji1 = veritabaniIslemleri.SonHataMesaji;
            if (islemHataMesaji1 != null)
            {
                Mesaj.Ver(islemHataMesaji1, Mesaj.MesajTurleri.FAIL, Page.Master);
            }
            else
            {
                Mesaj.Ver(Mesajlar.MakineGuncellemeHatasi, Mesaj.MesajTurleri.FAIL, Page.Master);
            }
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
