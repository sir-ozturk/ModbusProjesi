using System;
using System.Web.UI;

public partial class MakineListele : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        GridHazirla();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.MAKINE_LISTELE, IslemTurleri.GORUNTULE))
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (Session["BasariMesaji"] != null)
        {
            Mesaj.Ver(Session["BasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Page.Master);

            Session.Remove("BasariMesaji");
        }

        if (Page.IsPostBack == false)
        {
            Listele();
        }
    }

    private void GridHazirla()
    {
        ucMyGrid.KolonEkle(Makineler.C_Sutun_model_ad, "Model");
        ucMyGrid.KolonEkle(Makineler.C_Sutun_gg_no, "GG No");
        ucMyGrid.KolonEkle(Makineler.C_Sutun_makine_no, "Makine No");
        ucMyGrid.KolonEkle(Makineler.C_Sutun_makine_adi, "Makine Adı");
        ucMyGrid.KolonEkle(Makineler.C_Sutun_band_no, "Band No");
        ucMyGrid.KolonEkle(Makineler.C_Sutun_ip, "IP");
        ucMyGrid.KolonEkle(Makineler.C_Sutun_mfg, "MFG");
        ucMyGrid.DurumKolonEkle(Makineler.C_Sutun_aktif_mi, "Durum", "Aktif", "Pasif");
        ucMyGrid.ButonEkle("İşlemler", Makineler.C_Sutun_id, ucMyGrid.ButonTip.GUNCELLE, ucMyGrid.ButonTip.SIL);
    }

    private void Listele()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Makineler makineler = new Makineler(veritabaniIslemleri);

            makineler.TumunuGetir();

            ucMyGrid.Doldur(makineler.VeriTablosu);
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
            if (!IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.GUNCELLE))
            {
                Mesaj.Ver(Mesajlar.YetkinizYok, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }

            Response.Redirect("~/Pages/MakineEkle.aspx?id=" + e.Id, false);

            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (e.ButonTip == ucMyGrid.ButonTip.SIL)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

                Makineler makineler = new Makineler(veritabaniIslemleri);

                makineler.Id = e.Id;

                if (makineler.Sil())
                {
                    Mesaj.Ver(Mesajlar.SilmeBasarili, Mesaj.MesajTurleri.SUCCESS, Page.Master);
                }
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
