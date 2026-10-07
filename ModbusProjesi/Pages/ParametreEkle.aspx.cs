using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class ParametreEkle : System.Web.UI.Page
{
    private int gelenId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!GoruntulemeYetkisi())
        {
            return;
        }

        if (!KayitNumarasiOku())
        {
            btnKaydet.Enabled = false;
            Hata("Geçersiz kayıt numarası.");
            return;
        }

        if (gelenId > 0)
        {
            litBaslik.Text = "Parametre Düzenle";
        }
        else
        {
            litBaslik.Text = "Parametre Ekle";
        }

        if (gelenId > 0)
        {
            btnKaydet.Text = "Güncelle";
        }
        else
        {
            btnKaydet.Text = "Kaydet";
        }

        if (gelenId > 0)
        {
            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GUNCELLE);
        }
        else
        {
            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.EKLE);
        }

        ddlGruplar.Enabled = gelenId == 0;
        txtKod.ReadOnly = true;
        if (!IsPostBack)
        {
            ViewState["FormHazir"] = FormuDoldur();
        }

        if (!Convert.ToBoolean(ViewState["FormHazir"]))
        {
            btnKaydet.Enabled = false;
        }

        ListeBaglantisiniAyarla();
    }

    private bool GoruntulemeYetkisi()
    {
        if (!IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GORUNTULE))
        {
            pnlIcerik.Visible = false;
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        return true;
    }

    private bool KayitNumarasiOku()
    {
        gelenId = 0;
        return Request.QueryString["id"] == null || (int.TryParse(Request.QueryString["id"], out gelenId) && gelenId > 0);
    }

    private bool FormuDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            if (gelenId > 0)
            {
                Parametreler kayit = new Parametreler(veritabaniIslemleri);
                kayit.Id = gelenId;
                if (!kayit.Doldur())
                {
                    Hata("Düzenlenecek parametre bulunamadı.");
                    return false;
                }

                ddlGruplar.Items.Add(new ListItem(kayit.SonucKayit["grup_adi"].ToString(), kayit.GrupId.ToString()));
                txtKod.Text = kayit.Kod;
                txtAdi.Text = kayit.Adi;
                txtAciklama.Text = kayit.Aciklama;
                txtSiraNo.Text = kayit.SiraNo.ToString();
                chkAciklamaZorunlu.Checked = kayit.AciklamaZorunluMu;
                if (kayit.AktifMi)
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
                ParametreGruplari gruplar = new ParametreGruplari(veritabaniIslemleri);
                txtKod.Text = "";
                txtKod.Attributes["placeholder"] = "Kaydedildiğinde otomatik oluşturulacak";
                ddlGruplar.DataSource = gruplar.Listele(true);
                ddlGruplar.DataTextField = ParametreGruplari.C_Sutun_adi;
                ddlGruplar.DataValueField = OrtakAlanlar.C_Sutun_id;
                ddlGruplar.DataBind();
                ddlGruplar.Items.Insert(0, new ListItem("Grup seçiniz", ""));
                int grupId;
                if (int.TryParse(Request.QueryString["grup_id"], out grupId) && grupId > 0)
                {
                    ListItem secim = ddlGruplar.Items.FindByValue(grupId.ToString());
                    if (secim != null)
                    {
                        ddlGruplar.SelectedValue = secim.Value;
                    }
                    else
                    {
                        Hata("Seçili grup pasif veya bulunamadı. Aktif bir grup seçiniz.");
                    }
                }

                if (ddlGruplar.Items.Count == 1)
                {
                    Hata("Yeni parametre için aktif bir grup bulunamadı.");
                    return false;
                }
            }

            return true;
        }
        catch
        {
            Hata("Form bilgileri alınamadı. Sayfayı yeniden yükleyiniz.");
            return false;
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!GoruntulemeYetkisi())
        {
            return;
        }

        if (!KayitNumarasiOku() || !Convert.ToBoolean(ViewState["FormHazir"]))
        {
            Hata("Geçerli bir kayıt formu yüklenmelidir.");
            return;
        }

        if (gelenId > 0)
        {
            if (!IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GUNCELLE))
            {
                Hata(Mesajlar.YetkinizYok);
                return;
            }
        }
        else
        {
            if (!IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.EKLE))
            {
                Hata(Mesajlar.YetkinizYok);
                return;
            }
        }

        int siraNo;
        if (!int.TryParse(txtSiraNo.Text, out siraNo) || siraNo < 0)
        {
            Hata("Sıra numarası sıfır veya daha büyük bir tam sayı olmalıdır.");
            return;
        }

        if (ddlAktiflik.SelectedValue != "1" && ddlAktiflik.SelectedValue != "0")
        {
            Hata("Geçerli bir durum seçiniz.");
            return;
        }

        int grupId = 0;
        if (gelenId == 0 && (!int.TryParse(ddlGruplar.SelectedValue, out grupId) || grupId <= 0))
        {
            Hata("Aktif bir parametre grubu seçiniz.");
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Parametreler kayit = new Parametreler(veritabaniIslemleri);
            kayit.Id = gelenId;
            if (gelenId > 0)
            {
                if (!kayit.Doldur())
                {
                    veritabaniIslemleri.GeriAl();
                    Hata("Düzenlenecek parametre bulunamadı.");
                    return;
                }
            }
            else
            {
                kayit.GrupId = grupId;
            }

            kayit.Adi = txtAdi.Text;
            kayit.Aciklama = txtAciklama.Text;
            kayit.SiraNo = siraNo;
            kayit.AciklamaZorunluMu = chkAciklamaZorunlu.Checked;
            kayit.AktifMi = ddlAktiflik.SelectedValue == "1";
            CurrentInfo kullanici = new Sessionlar().Current._CurrentInfo;
            kayit.EkleyenId = kayit.GuncelleyenId = kullanici.KullaniciId;
            kayit.EkleyenIp = kayit.GuncelleyenIp = Utility.IpNoGetir();
            bool kayitKaydedildi;
            if (gelenId > 0)
            {
                kayitKaydedildi = kayit.Guncelle();
            }
            else
            {
                kayitKaydedildi = kayit.Ekle();
            }

            if (!kayitKaydedildi)
            {
                veritabaniIslemleri.GeriAl();
                var islemHataMesaji1 = veritabaniIslemleri.SonHataMesaji;
                if (islemHataMesaji1 != null)
                {
                    Hata(islemHataMesaji1);
                }
                else
                {
                    Hata("Parametre kaydedilemedi.");
                }

                return;
            }

            veritabaniIslemleri.Uygula();
            if (gelenId > 0)
            {
                Session["ParametreBasari"] = "Parametre güncellendi.";
            }
            else
            {
                Session["ParametreBasari"] = "Parametre eklendi.";
            }

            if (kayit.AktifMi)
            {
                Response.Redirect("~/Pages/ParametreListele.aspx?grup_id=" + kayit.GrupId + "&aktif_mi=" + "1", false);
            }
            else
            {
                Response.Redirect("~/Pages/ParametreListele.aspx?grup_id=" + kayit.GrupId + "&aktif_mi=" + "0", false);
            }

            Context.ApplicationInstance.CompleteRequest();
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
            Hata("Parametre işlemi tamamlanamadı. Lütfen tekrar deneyiniz.");
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    private void ListeBaglantisiniAyarla()
    {
        int grupId;
        if (int.TryParse(ddlGruplar.SelectedValue, out grupId) && grupId > 0)
        {
            lnkListe.HRef = "ParametreListele.aspx?grup_id=" + grupId;
        }
        else
        {
            lnkListe.HRef = "ParametreListele.aspx";
        }
    }

    private void Hata(string mesaj)
    {
        Mesaj.Ver(mesaj, Mesaj.MesajTurleri.FAIL, Master);
    }
}
