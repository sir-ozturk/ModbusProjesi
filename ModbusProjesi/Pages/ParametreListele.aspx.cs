using System;
using System.Data;
using System.Collections.Generic;
using System.Web.UI.WebControls;

public partial class ParametreListele : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        ucGrid.KolonEkle("grup_adi", "Grup");
        ucGrid.KolonEkle(Parametreler.C_Sutun_kod, "Kod");
        ucGrid.KolonEkle(Parametreler.C_Sutun_adi, "Ad");
        ucGrid.KolonEkle(Parametreler.C_Sutun_sira_no, "Sıra No");
        ucGrid.DurumKolonEkle(Parametreler.C_Sutun_aciklama_zorunlu_mu, "Açıklama Zorunlu", "Evet", "Hayır");
        ucGrid.DurumKolonEkle(OrtakAlanlar.C_Sutun_aktif_mi, "Durum", "Aktif", "Pasif");
        ucGrid.DurumKolonEkle("grup_aktif_mi", "Grup Durumu", "Aktif", "Pasif");
        ucGrid.AramaGorunur = false;
        ucGrid.SilmeKayitAdiAlani = Parametreler.C_Sutun_adi;
        List<ucMyGrid.ButonTip> butonlar = new List<ucMyGrid.ButonTip>();
        if (IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GORUNTULE) && IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GUNCELLE))
        {
            butonlar.Add(ucMyGrid.ButonTip.GUNCELLE);
        }

        if (IslemYetki.Kontrol(Ekranlar.PARAMETRE_LISTELE, IslemTurleri.SIL))
        {
            butonlar.Add(ucMyGrid.ButonTip.SIL);
        }

        if (butonlar.Count > 0)
        {
            ucGrid.ButonEkle("İşlemler", OrtakAlanlar.C_Sutun_id, butonlar.ToArray());
        }

        ucGrid.ButonTiklandi += ucGrid_ButonTiklandi;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!YetkiKontrol())
        {
            return;
        }

        lnkEkle.Visible = IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GORUNTULE) && IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.EKLE);
        if (!Page.IsPostBack)
        {
            if (Session["ParametreBasari"] != null)
            {
                Mesaj.Ver(Session["ParametreBasari"].ToString(), Mesaj.MesajTurleri.SUCCESS, Master);
                Session.Remove("ParametreBasari");
            }

            if (GruplariDoldur())
            {
                string aktifMi = Request.QueryString["aktif_mi"];
                if (aktifMi == "0" || aktifMi == "1")
                {
                    ddlDurum.SelectedValue = aktifMi;
                }

                Listele();
            }
        }
    }

    private bool YetkiKontrol()
    {
        if (!IslemYetki.Kontrol(Ekranlar.PARAMETRE_LISTELE, IslemTurleri.GORUNTULE))
        {
            pnlIcerik.Visible = false;
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return false;
        }

        return true;
    }

    private bool GruplariDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            ParametreGruplari gruplar = new ParametreGruplari(veritabaniIslemleri);
            ddlGruplar.DataSource = gruplar.Listele();
            ddlGruplar.DataTextField = ParametreGruplari.C_Sutun_adi;
            ddlGruplar.DataValueField = OrtakAlanlar.C_Sutun_id;
            ddlGruplar.DataBind();
            ddlGruplar.Items.Insert(0, new ListItem("Tüm Gruplar", "0"));
            string grupId = Request.QueryString["grup_id"];
            if (grupId != null)
            {
                int id;
                if (!int.TryParse(grupId, out id) || id < 0 || ddlGruplar.Items.FindByValue(id.ToString()) == null)
                {
                    Hata("Seçilen parametre grubu bulunamadı.");
                    return false;
                }

                ddlGruplar.SelectedValue = id.ToString();
            }

            return true;
        }
        catch
        {
            btnListele.Enabled = false;
            Hata("Parametre grupları alınamadı. Sayfayı yeniden yükleyiniz.");
            return false;
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnListele_Click(object sender, EventArgs e)
    {
        if (!YetkiKontrol())
        {
            return;
        }

        Listele();
    }

    private void Listele()
    {

        pnlBosListe.Visible = false;
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            int grupId;
            if (!int.TryParse(ddlGruplar.SelectedValue, out grupId)
                || grupId < 0
                || (ddlDurum.SelectedValue != ""
                && ddlDurum.SelectedValue != "1"
                && ddlDurum.SelectedValue != "0"))
            {
                Hata("Liste filtreleri geçersiz.");
                ucGrid.Doldur(new DataTable());
                return;
            }

            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Parametreler parametreler = new Parametreler(veritabaniIslemleri);
            bool? aktifMi;
            if (ddlDurum.SelectedValue == "")
            {
                aktifMi = (bool? )null;
            }
            else
            {
                aktifMi = ddlDurum.SelectedValue == "1";
            }

            DataTable tablo;
            if (grupId == 0)
            {
                tablo = parametreler.Listele((int? )null, aktifMi, txtArama.Text);
            }
            else
            {
                tablo = parametreler.Listele(grupId, aktifMi, txtArama.Text);
            }

            ucGrid.Doldur(tablo);
            lnkEkle.HRef = "ParametreEkle.aspx?grup_id=" + grupId;
            pnlBosListe.Visible = tablo.Rows.Count == 0;
        }
        catch (ArgumentException ex)
        {
            ucGrid.Doldur(new DataTable());
            Hata(ex.Message);
        }
        catch
        {
            ucGrid.Doldur(new DataTable());
            Hata("Parametre listesi alınamadı. Lütfen tekrar deneyiniz.");
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    private void Hata(string mesaj)
    {
        Mesaj.Ver(mesaj, Mesaj.MesajTurleri.FAIL, Master);
    }

    protected void ucGrid_ButonTiklandi(object sender, ucMyGrid.MyGridButonEventArgs e)
    {
        if (!YetkiKontrol())
        {
            return;
        }

        if (e.Id <= 0)
        {
            return;
        }

        if (e.ButonTip == ucMyGrid.ButonTip.SIL)
        {
            ParametreSil(e.Id);
            return;
        }

        if (e.ButonTip != ucMyGrid.ButonTip.GUNCELLE)
        {
            return;
        }

        if (!IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GORUNTULE) || !IslemYetki.Kontrol(Ekranlar.PARAMETRE_EKLE, IslemTurleri.GUNCELLE))
        {
            Hata(Mesajlar.YetkinizYok);
            return;
        }

        Response.Redirect("~/Pages/ParametreEkle.aspx?id=" + e.Id, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void ParametreSil(int id)
    {

        if (!IslemYetki.Kontrol(Ekranlar.PARAMETRE_LISTELE, IslemTurleri.SIL))
        {
            Hata(Mesajlar.YetkinizYok);
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        string hataMesaji = null;
        bool silindi = false;
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Parametreler kayit = new Parametreler(veritabaniIslemleri);
            kayit.Id = id;
            if (!kayit.Sil())
            {
                var islemHataMesaji1 = veritabaniIslemleri.SonHataMesaji;
                if (islemHataMesaji1 != null)
                {
                    hataMesaji = islemHataMesaji1;
                }
                else
                {
                    hataMesaji = "Parametre silinemedi.";
                }

                veritabaniIslemleri.GeriAl();
            }
            else
            {
                veritabaniIslemleri.Uygula();
                silindi = true;
            }
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
            hataMesaji = "Parametre silme işlemi tamamlanamadı. Lütfen tekrar deneyiniz.";
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }

        Listele();
        if (silindi)
        {
            Mesaj.Ver("Parametre silindi.", Mesaj.MesajTurleri.SUCCESS, Master);
        }
        else
        {
            Hata(hataMesaji);
        }
    }
}
