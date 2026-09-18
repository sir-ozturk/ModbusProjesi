using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.UI.WebControls;

public partial class MakineRoleBaglanti : System.Web.UI.Page
{
    private int gelenId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI, IslemTurleri.GORUNTULE))
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (Request.QueryString["id"] != null && (!int.TryParse(Request.QueryString["id"], out gelenId) || gelenId<=0))
        {
            Hata("Geçersiz kayıt numarası.");
            btnKaydet.Enabled=false;
            return;
        }
        btnKaydet.Enabled=IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI, gelenId>0 ? IslemTurleri.GUNCELLE : IslemTurleri.EKLE);
        litBaslik.Text="Makine–Röle Bağlantısı " + (gelenId>0 ? "Düzenle" : "Ekle");
        btnKaydet.Text=gelenId>0 ? "Güncelle" : "Kaydet";
        if (!IsPostBack)
        {
            if (Session["DonanimBasari"]!=null)
            {
                pnlBasari.Visible=true;
                lblBasari.Text=Server.HtmlEncode(Session["DonanimBasari"].ToString());
                Session.Remove("DonanimBasari");
            }
            Doldur();
            Listele();
        }
    }

    private void Hata(string mesaj)
    {
        pnlHata.Visible=true;
        lblHata.Text=Server.HtmlEncode(mesaj);
    }

    private void Doldur()
    {
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            MakineRoleBaglantilari kayit=new MakineRoleBaglantilari(db);
            kayit.Id=gelenId;
            if (gelenId>0 && !kayit.Doldur()) { Hata("Kayıt bulunamadı."); btnKaydet.Enabled=false; return; }
            SecenekleriDoldur(db, kayit.RoleKartId, kayit.KanalNo, kayit.MakineId);
            if (gelenId>0)
            {
                ddlAktiflik.SelectedValue=kayit.AktifMi ? "1" : "0";
            }
        }
        catch { Hata("Donanım bilgileri alınamadı. Veritabanı geçişinin uygulandığını kontrol ediniz."); btnKaydet.Enabled=false; }
        finally { db.Bitir(); }
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI, IslemTurleri.GORUNTULE)
            || !IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI, gelenId>0 ? IslemTurleri.GUNCELLE : IslemTurleri.EKLE))
        { Hata(Mesajlar.YetkinizYok); return; }
        if (Request.QueryString["id"]!=null && gelenId<=0) { Hata("Geçersiz kayıt."); return; }
        int roleId,kanal,makineId;
        if (!int.TryParse(ddlRole.SelectedValue,out roleId) || roleId<=0
            || !int.TryParse(ddlKanal.SelectedValue,out kanal) || kanal<1 || kanal>16
            || !int.TryParse(ddlMakine.SelectedValue,out makineId) || makineId<=0)
        { Hata("Röle kartı, boş bir kanal ve makine seçiniz."); return; }
        if (ddlAktiflik.SelectedValue!="1" && ddlAktiflik.SelectedValue!="0") { Hata("Durum seçiniz."); return; }
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            MakineRoleBaglantilari kayit=new MakineRoleBaglantilari(db);
            kayit.Id=gelenId;
            kayit.RoleKartId=roleId;
            kayit.KanalNo=kanal;
            kayit.MakineId=makineId;
            kayit.AktifMi=ddlAktiflik.SelectedValue=="1";
            CurrentInfo kullanici=new Sessionlar().Current._CurrentInfo;
            kayit.EkleyenId=kayit.GuncelleyenId=kullanici.KullaniciId;
            kayit.EkleyenIp=kayit.GuncelleyenIp=Utility.IpNoGetir();
            if (!(gelenId>0 ? kayit.Guncelle() : kayit.Ekle()))
            { db.GeriAl(); Hata(db.SonHataMesaji ?? "Kayıt tamamlanamadı."); return; }
            db.Uygula();
            Session["DonanimBasari"]="Donanım kaydı kaydedildi.";
            Response.Redirect("~/Pages/MakineRoleBaglantilari.aspx",false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch { db.GeriAl(); Hata("Kayıt işlemi tamamlanamadı."); }
        finally { db.Bitir(); }
    }

    private void SecenekleriDoldur(VeritabaniIslemleri db, int roleId, int kanal, int makineId)
    {
        RoleKartlari roleler=new RoleKartlari(db); roleler.TumunuGetir();
        ddlRole.Items.Clear(); ddlRole.Items.Add(new ListItem("Röle kartı seçiniz",""));
        foreach(DataRow r in roleler.VeriTablosu.Rows)
        {
            int id=Convert.ToInt32(r["id"]);
            if(Convert.ToBoolean(r["aktif_mi"]) || id==roleId)
                ddlRole.Items.Add(new ListItem(r["role_adi"]+" — "+r["adres"],id.ToString()));
        }
        if(ddlRole.Items.FindByValue(roleId.ToString())!=null) ddlRole.SelectedValue=roleId.ToString();
        MakineRoleBaglantilari baglantilar=new MakineRoleBaglantilari(db); baglantilar.TumunuGetir();
        Makineler makineler=new Makineler(db); makineler.TumunuGetir();
        ddlMakine.Items.Clear(); ddlMakine.Items.Add(new ListItem("Makine seçiniz",""));
        foreach(DataRow r in makineler.VeriTablosu.Rows)
        {
            int id=Convert.ToInt32(r["id"]);
            if(!Convert.ToBoolean(r["aktif_mi"]) && id!=makineId) continue;
            bool dolu=baglantilar.VeriTablosu.AsEnumerable().Any(x=>Convert.ToBoolean(x["aktif_mi"]) && Convert.ToInt32(x["makine_id"])==id && Convert.ToInt32(x["id"])!=gelenId);
            ListItem item=new ListItem("No "+r["makine_no"]+" — "+r["makine_adi"]+(dolu ? " (Bağlı)" : ""),id.ToString());
            item.Enabled=!dolu; ddlMakine.Items.Add(item);
        }
        if(ddlMakine.Items.FindByValue(makineId.ToString())!=null) ddlMakine.SelectedValue=makineId.ToString();
        KanallariDoldur(baglantilar.VeriTablosu,roleId,kanal);
    }

    private void KanallariDoldur(DataTable tablo,int roleId,int seciliKanal)
    {
        ddlKanal.Items.Clear(); ddlKanal.Items.Add(new ListItem("Kanal seçiniz",""));
        ddlKanal.Enabled=roleId>0;
        if(roleId<=0) return;
        for(int kanal=1;kanal<=16;kanal++)
        {
            DataRow bagli=tablo.AsEnumerable().FirstOrDefault(x=>Convert.ToBoolean(x["aktif_mi"])
                && Convert.ToInt32(x["role_kart_id"])==roleId && Convert.ToInt32(x["kanal_no"])==kanal);
            bool digerBaglanti=bagli!=null && Convert.ToInt32(bagli["id"])!=gelenId;
            ListItem item=new ListItem("Kanal "+kanal+(bagli==null ? " — Boş" : " — Makine No "+bagli["makine_no"]),kanal.ToString());
            item.Enabled=!digerBaglanti; ddlKanal.Items.Add(item);
            if(kanal==seciliKanal && !digerBaglanti) ddlKanal.SelectedValue=kanal.ToString();
        }
    }

    protected void ddlRole_SelectedIndexChanged(object sender,EventArgs e)
    {
        if(!IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI,IslemTurleri.GORUNTULE)) return;
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            MakineRoleBaglantilari baglantilar=new MakineRoleBaglantilari(db); baglantilar.TumunuGetir();
            int roleId; int.TryParse(ddlRole.SelectedValue,out roleId);
            KanallariDoldur(baglantilar.VeriTablosu,roleId,0);
        }
        catch { Hata("Kanal bilgileri alınamadı."); ddlKanal.Items.Clear(); }
        finally { db.Bitir(); }
    }
    protected void Page_Init(object sender,EventArgs e)
    {
        ucGrid.KolonEkle("role_adi","Röle Kartı");
        ucGrid.KolonEkle("adres","Ethernet Adresi");
        ucGrid.KolonEkle("kanal_no","Kanal");
        ucGrid.KolonEkle("makine_no","Makine No");
        ucGrid.KolonEkle("makine_adi","Makine");
        ucGrid.DurumKolonEkle("aktif_mi","Durum","Aktif","Pasif");
        ucGrid.ButonEkle("İşlemler","id",ucMyGrid.ButonTip.GUNCELLE,ucMyGrid.ButonTip.SIL);
    }
    private void Listele()
    {
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            MakineRoleBaglantilari kayit=new MakineRoleBaglantilari(db); kayit.TumunuGetir(); ucGrid.Doldur(kayit.VeriTablosu);
        }
        catch { Hata("Donanım listesi alınamadı. Veritabanı geçişini kontrol ediniz."); }
        finally { db.Bitir(); }
    }
    protected void ucGrid_ButonTiklandi(object sender,ucMyGrid.MyGridButonEventArgs e)
    {
        if(!IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI,IslemTurleri.GORUNTULE)) { Hata(Mesajlar.YetkinizYok); return; }
        if(e.ButonTip==ucMyGrid.ButonTip.GUNCELLE)
        {
            if(!IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI,IslemTurleri.GUNCELLE)) { Hata(Mesajlar.YetkinizYok); return; }
            Response.Redirect("~/Pages/MakineRoleBaglantilari.aspx?id="+e.Id,false); Context.ApplicationInstance.CompleteRequest(); return;
        }
        if(e.ButonTip!=ucMyGrid.ButonTip.SIL) return;
        if(!IslemYetki.Kontrol(Ekranlar.MAKINE_ROLE_BAGLANTI,IslemTurleri.SIL)) { Hata(Mesajlar.YetkinizYok); return; }
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            MakineRoleBaglantilari kayit=new MakineRoleBaglantilari(db); kayit.Id=e.Id;
            if(!kayit.Sil()) { db.GeriAl(); Hata(db.SonHataMesaji ?? "Kayıt silinemedi."); }
            else { db.Uygula(); Session["DonanimBasari"]="Kayıt silindi."; Response.Redirect("~/Pages/MakineRoleBaglantilari.aspx",false); Context.ApplicationInstance.CompleteRequest(); }
        }
        catch { db.GeriAl(); Hata("Kayıt silinemedi."); }
        finally { db.Bitir(); }
        Listele();
    }
}
