using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Sessionlar sessionlar = new Sessionlar();
        CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

        if (currentInfo == null || currentInfo.LoginYapildiMi == false)
        {
            Response.Redirect("~/Pages/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (!Page.IsPostBack)
        {
            MenuYetkileriniAyarla();
            LogIslemleri.IslemKaydet();

            lblGirisYapanKullanici.Text = currentInfo.Ad + " " + currentInfo.Soyad;

            if (!string.IsNullOrEmpty(currentInfo.ProfilResim))
            {
                imgSolMenuProfil.ImageUrl = "~/Files/" + currentInfo.ProfilResim;
            }
            else
            {
                imgSolMenuProfil.ImageUrl = "~/Files/Images/no-image.svg";
            }
        }
    }

    protected void btnCikis_Click(object sender, EventArgs e)
    {
        Sessionlar sessionlar = new Sessionlar();

        sessionlar.Current._CurrentInfo = null;

        Session.Clear();
        Session.Abandon();

        Response.Redirect("~/Pages/Login.aspx", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    private void MenuYetkileriniAyarla()
    {
        lnkEthernetKartEkle.Visible = IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GORUNTULE);
        lnkEthernetKartListele.Visible = IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_LISTELE, IslemTurleri.GORUNTULE);
        lnkRoleKartEkle.Visible = IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.GORUNTULE);
        lnkRoleKartListele.Visible = IslemYetki.Kontrol(Ekranlar.ROLE_KART_LISTELE, IslemTurleri.GORUNTULE);
        lnkMakineRoleBaglantilari.Visible = IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI, IslemTurleri.GORUNTULE);
        pnlRoleDonanimIslemleri.Visible = lnkEthernetKartEkle.Visible || lnkEthernetKartListele.Visible || lnkRoleKartEkle.Visible || lnkRoleKartListele.Visible || lnkMakineRoleBaglantilari.Visible;

        lnkMakineEkle.Visible = IslemYetki.Kontrol(Ekranlar.MAKINE_EKLE, IslemTurleri.GORUNTULE);
        lnkMakineListele.Visible = IslemYetki.Kontrol(Ekranlar.MAKINE_LISTELE, IslemTurleri.GORUNTULE);
        lnkKullaniciEkle.Visible = IslemYetki.Kontrol(Ekranlar.KULLANICI_EKLE, IslemTurleri.GORUNTULE);
        lnkKullaniciListele.Visible = IslemYetki.Kontrol(Ekranlar.KULLANICI_LISTELE, IslemTurleri.GORUNTULE);
        lnkLogListele.Visible = IslemYetki.Kontrol(Ekranlar.LOG_LISTELE, IslemTurleri.GORUNTULE);
        lnkRolEkle.Visible = IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.GORUNTULE);
        lnkRolListele.Visible = IslemYetki.Kontrol(Ekranlar.ROL_LISTELE, IslemTurleri.GORUNTULE);

        pnlMakineIslemleri.Visible = lnkMakineEkle.Visible || lnkMakineListele.Visible;
        pnlKullaniciIslemleri.Visible = lnkKullaniciEkle.Visible || lnkKullaniciListele.Visible;
        pnlRolIslemleri.Visible = lnkRolEkle.Visible || lnkRolListele.Visible;
        pnlRaporlar.Visible = lnkLogListele.Visible;
    }
}
