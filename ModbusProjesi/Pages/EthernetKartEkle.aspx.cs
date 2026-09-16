using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.UI.WebControls;

public partial class EthernetKartEkle : System.Web.UI.Page
{
    private int gelenId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GORUNTULE))
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
        btnKaydet.Enabled=IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, gelenId>0 ? IslemTurleri.GUNCELLE : IslemTurleri.EKLE);
        litBaslik.Text="Ethernet Kartı " + (gelenId>0 ? "Düzenle" : "Ekle");
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
            EthernetKartlari kayit=new EthernetKartlari(db);
            kayit.Id=gelenId;
            if (gelenId>0 && !kayit.Doldur()) { Hata("Kayıt bulunamadı."); btnKaydet.Enabled=false; return; }
            ModelSecenekleriniDoldur(db, gelenId > 0 ? kayit.Model : null);
            if (gelenId>0)
            {
                txtKartAdi.Text=kayit.KartAdi.ToString();
                ddlModel.SelectedValue=kayit.Model.ToString();
                txtIp.Text=kayit.Ip.ToString();
                txtHttpPort.Text=kayit.HttpPort.ToString();
                ddlAktiflik.SelectedValue=kayit.AktifMi ? "1" : "0";
            }
            else txtHttpPort.Text="8080";
        }
        catch { Hata("Donanım bilgileri alınamadı. Veritabanı geçişinin uygulandığını kontrol ediniz."); btnKaydet.Enabled=false; }
        finally { db.Bitir(); }
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GORUNTULE)
            || !IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, gelenId>0 ? IslemTurleri.GUNCELLE : IslemTurleri.EKLE))
        { Hata(Mesajlar.YetkinizYok); return; }
        if (Request.QueryString["id"]!=null && gelenId<=0) { Hata("Geçersiz kayıt."); return; }
        IPAddress adres;
        int port;
        if (string.IsNullOrWhiteSpace(txtKartAdi.Text) || txtKartAdi.Text.Trim().Length>100
            || string.IsNullOrWhiteSpace(ddlModel.SelectedValue) || !IPAddress.TryParse(txtIp.Text.Trim(),out adres)
            || adres.AddressFamily!=AddressFamily.InterNetwork
            || !int.TryParse(txtHttpPort.Text,out port) || port<1 || port>65535)
        { Hata("Kart adı, IPv4 adresi ve 1–65535 arası HTTP portu giriniz."); return; }
        if (ddlAktiflik.SelectedValue!="1" && ddlAktiflik.SelectedValue!="0") { Hata("Durum seçiniz."); return; }
        VeritabaniIslemleri db=new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            EthernetKartlari kayit=new EthernetKartlari(db);
            kayit.Id=gelenId;
            kayit.KartAdi=txtKartAdi.Text.Trim();
            kayit.Model=ddlModel.SelectedValue;
            kayit.Ip=adres.ToString();
            kayit.HttpPort=port;
            kayit.AktifMi=ddlAktiflik.SelectedValue=="1";
            CurrentInfo kullanici=new Sessionlar().Current._CurrentInfo;
            kayit.EkleyenId=kayit.GuncelleyenId=kullanici.KullaniciId;
            kayit.EkleyenIp=kayit.GuncelleyenIp=Utility.IpNoGetir();
            if (!(gelenId>0 ? kayit.Guncelle() : kayit.Ekle()))
            { Hata(db.SonHataMesaji ?? "Kayıt tamamlanamadı."); return; }
            Session["DonanimBasari"]="Donanım kaydı kaydedildi.";
            Response.Redirect("~/Pages/EthernetKartListele.aspx",false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch { Hata("Kayıt işlemi tamamlanamadı."); }
        finally { db.Bitir(); }
    }

    private void ModelSecenekleriniDoldur(VeritabaniIslemleri db, string seciliModel)
    {
        EthernetKartlari kartlar = new EthernetKartlari(db);
        kartlar.TumunuGetir();

        ddlModel.Items.Clear();
        ddlModel.Items.Add(new ListItem("Model seçiniz", ""));

        foreach (DataRow satir in kartlar.VeriTablosu.Rows)
        {
            string model = satir[EthernetKartlari.C_Sutun_model].ToString().Trim();

            if (model.Length > 0 && ddlModel.Items.FindByValue(model) == null)
            {
                ddlModel.Items.Add(new ListItem(model, model));
            }
        }

        // Düzenlenen kayıt eski bir model içeriyorsa listeden kaybolmasın.
        if (!string.IsNullOrWhiteSpace(seciliModel) && ddlModel.Items.FindByValue(seciliModel) == null)
        {
            ddlModel.Items.Add(new ListItem(seciliModel, seciliModel));
        }

        if (ddlModel.Items.FindByValue(seciliModel) != null)
        {
            ddlModel.SelectedValue = seciliModel;
        }
    }
}
