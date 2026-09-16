using System;
public partial class RoleKartListele : System.Web.UI.Page
{
    protected void Page_Load(object sender,EventArgs e)
    {
        if(!IslemYetki.Kontrol(Ekranlar.ROLE_KART_LISTELE,IslemTurleri.GORUNTULE))
        { Response.Redirect("~/Default.aspx",false); Context.ApplicationInstance.CompleteRequest(); return; }
        lnkEkle.Visible=IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE,IslemTurleri.EKLE);
        if(!IsPostBack)
        {
            if(Session["DonanimBasari"]!=null)
            { pnlBasari.Visible=true; lblBasari.Text=Server.HtmlEncode(Session["DonanimBasari"].ToString()); Session.Remove("DonanimBasari"); }
            Listele();
        }
    }
    private void Hata(string mesaj) { pnlHata.Visible=true; lblHata.Text=Server.HtmlEncode(mesaj); }
    protected void Page_Init(object sender,EventArgs e)
    {
        ucGrid.KolonEkle("role_adi","Röle Kartı");
        ucGrid.KolonEkle("kart_adi","Ethernet Kartı");
        ucGrid.KolonEkle("adres","Cihaz Adresi");
        ucGrid.KolonEkle("kanal_sayisi","Kanal Sayısı");
        ucGrid.KolonEkle("dolu_kanal","Atanan Kanal");
        ucGrid.DurumKolonEkle("aktif_mi","Durum","Aktif","Pasif");
        ucGrid.ButonEkle("İşlemler","id",ucMyGrid.ButonTip.GUNCELLE,ucMyGrid.ButonTip.SIL);
    }
    private void Listele()
    {
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            RoleKartlari kayit=new RoleKartlari(db); kayit.TumunuGetir(); ucGrid.Doldur(kayit.VeriTablosu);
        }
        catch { Hata("Donanım listesi alınamadı. Veritabanı geçişini kontrol ediniz."); }
        finally { db.Bitir(); }
    }
    protected void ucGrid_ButonTiklandi(object sender,ucMyGrid.MyGridButonEventArgs e)
    {
        if(!IslemYetki.Kontrol(Ekranlar.ROLE_KART_LISTELE,IslemTurleri.GORUNTULE)) { Hata(Mesajlar.YetkinizYok); return; }
        if(e.ButonTip==ucMyGrid.ButonTip.GUNCELLE)
        {
            if(!IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE,IslemTurleri.GUNCELLE)) { Hata(Mesajlar.YetkinizYok); return; }
            Response.Redirect("~/Pages/RoleKartEkle.aspx?id="+e.Id,false); Context.ApplicationInstance.CompleteRequest(); return;
        }
        if(e.ButonTip!=ucMyGrid.ButonTip.SIL) return;
        if(!IslemYetki.Kontrol(Ekranlar.ROLE_KART_LISTELE,IslemTurleri.SIL)) { Hata(Mesajlar.YetkinizYok); return; }
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            RoleKartlari kayit=new RoleKartlari(db); kayit.Id=e.Id;
            if(!kayit.Sil()) Hata(db.SonHataMesaji ?? "Kayıt silinemedi.");
            else { Session["DonanimBasari"]="Kayıt silindi."; Response.Redirect("~/Pages/RoleKartListele.aspx",false); Context.ApplicationInstance.CompleteRequest(); }
        }
        catch { Hata("Kayıt silinemedi."); }
        finally { db.Bitir(); }
        Listele();
    }
}
