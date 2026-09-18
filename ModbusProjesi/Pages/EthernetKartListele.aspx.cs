using System;
public partial class EthernetKartListele : System.Web.UI.Page
{
    protected void Page_Load(object sender,EventArgs e)
    {
        if(!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_LISTELE,IslemTurleri.GORUNTULE))
        { Response.Redirect("~/Default.aspx",false); Context.ApplicationInstance.CompleteRequest(); return; }
        lnkEkle.Visible=IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE,IslemTurleri.EKLE);
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
        ucGrid.KolonEkle("kart_adi","Kart");
        ucGrid.KolonEkle("model","Model");
        ucGrid.KolonEkle("ip","IP");
        ucGrid.KolonEkle("http_port","HTTP Portu");
        ucGrid.DurumKolonEkle("aktif_mi","Durum","Aktif","Pasif");
        ucGrid.ButonEkle("İşlemler","id",ucMyGrid.ButonTip.GUNCELLE,ucMyGrid.ButonTip.SIL);
    }
    private void Listele()
    {
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            EthernetKartlari kayit=new EthernetKartlari(db); kayit.TumunuGetir(); ucGrid.Doldur(kayit.VeriTablosu);
        }
        catch { Hata("Donanım listesi alınamadı. Veritabanı geçişini kontrol ediniz."); }
        finally { db.Bitir(); }
    }
    protected void ucGrid_ButonTiklandi(object sender,ucMyGrid.MyGridButonEventArgs e)
    {
        if(!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_LISTELE,IslemTurleri.GORUNTULE)) { Hata(Mesajlar.YetkinizYok); return; }
        if(e.ButonTip==ucMyGrid.ButonTip.GUNCELLE)
        {
            if(!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE,IslemTurleri.GUNCELLE)) { Hata(Mesajlar.YetkinizYok); return; }
            Response.Redirect("~/Pages/EthernetKartEkle.aspx?id="+e.Id,false); Context.ApplicationInstance.CompleteRequest(); return;
        }
        if(e.ButonTip!=ucMyGrid.ButonTip.SIL) return;
        if(!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_LISTELE,IslemTurleri.SIL)) { Hata(Mesajlar.YetkinizYok); return; }
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            EthernetKartlari kayit=new EthernetKartlari(db); kayit.Id=e.Id;
            if(!kayit.Sil()) { db.GeriAl(); Hata(db.SonHataMesaji ?? "Kayıt silinemedi."); }
            else { db.Uygula(); Session["DonanimBasari"]="Kayıt silindi."; Response.Redirect("~/Pages/EthernetKartListele.aspx",false); Context.ApplicationInstance.CompleteRequest(); }
        }
        catch { db.GeriAl(); Hata("Kayıt silinemedi."); }
        finally { db.Bitir(); }
        Listele();
    }
}
