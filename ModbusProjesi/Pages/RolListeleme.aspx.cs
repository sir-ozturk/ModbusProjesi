using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class RolListeleme : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        GridHazirla();
    }

    private void GridHazirla()
    {
        ucRolGrid.KolonEkle(Roller.C_Sutun_id, "ID");
        ucRolGrid.KolonEkle(Roller.C_Sutun_rol_kodu, "Rol Kodu");
        ucRolGrid.KolonEkle(Roller.C_Sutun_adi, "Rol Adı");
        ucRolGrid.KolonEkle(Roller.C_Sutun_aciklama, "Açıklama");
        ucRolGrid.DurumKolonEkle(Roller.C_Sutun_aktif_mi, "Durum", "Aktif", "Pasif");
        ucRolGrid.ButonEkle("İşlemler", Roller.C_Sutun_id, ucMyGrid.ButonTip.DETAY);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROL_LISTELE, IslemTurleri.GORUNTULE))
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

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

            Roller roller = new Roller(veritabaniIslemleri);

            roller.TumunuGetir();

            ucRolGrid.Doldur(roller.VeriTablosu);
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

    protected void ucRolGrid_ButonTiklandi(object sender, ucMyGrid.MyGridButonEventArgs e)
    {
        if (e.ButonTip == ucMyGrid.ButonTip.DETAY)
        {
            Response.Redirect("~/Pages/RolEkleme.aspx?id=" + e.Id, false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
    }


}
