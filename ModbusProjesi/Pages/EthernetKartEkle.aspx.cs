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

        if (Request.QueryString["id"] != null && (!int.TryParse(Request.QueryString["id"], out gelenId) || gelenId <= 0))
        {
            Hata("Geçersiz kayıt numarası.");
            btnKaydet.Enabled = false;
            return;
        }

        if (gelenId > 0)
        {
            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GUNCELLE);
        }
        else
        {
            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.EKLE);
        }

        if (gelenId > 0)
        {
            litBaslik.Text = "Ethernet Kartı " + "Düzenle";
        }
        else
        {
            litBaslik.Text = "Ethernet Kartı " + "Ekle";
        }

        if (gelenId > 0)
        {
            btnKaydet.Text = "Güncelle";
        }
        else
        {
            btnKaydet.Text = "Kaydet";
        }

        if (!IsPostBack)
        {
            if (Session["DonanimBasari"] != null)
            {
                Mesaj.Ver(Session["DonanimBasari"].ToString(), Mesaj.MesajTurleri.SUCCESS, Master);
                Session.Remove("DonanimBasari");
            }

            Doldur();
        }
    }

    private void Hata(string mesaj)
    {
        Mesaj.Ver(mesaj, Mesaj.MesajTurleri.FAIL, Master);
    }

    private void Doldur()
    {
        VeritabaniIslemleri db = new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            EthernetKartlari kayit = new EthernetKartlari(db);
            kayit.Id = gelenId;
            if (gelenId > 0 && !kayit.Doldur())
            {
                Hata("Kayıt bulunamadı.");
                btnKaydet.Enabled = false;
                return;
            }

            if (gelenId > 0)
            {
                ModelSecenekleriniDoldur(db, kayit.Model);
            }
            else
            {
                ModelSecenekleriniDoldur(db, null);
            }

            if (gelenId > 0)
            {
                txtKartAdi.Text = kayit.KartAdi.ToString();
                ddlModel.SelectedValue = kayit.Model.ToString();
                txtIp.Text = kayit.Ip.ToString();
                txtHttpPort.Text = kayit.HttpPort.ToString();
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
                txtHttpPort.Text = "8080";
            }
        }
        catch
        {
            Hata("Donanım bilgileri alınamadı. Veritabanı geçişinin uygulandığını kontrol ediniz.");
            btnKaydet.Enabled = false;
        }
        finally
        {
            db.Bitir();
        }
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (gelenId > 0)
        {
            if (!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GORUNTULE)
                || !IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GUNCELLE))
            {
                Hata(Mesajlar.YetkinizYok);
                return;
            }
        }
        else
        {
            if (!IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.GORUNTULE)
                || !IslemYetki.Kontrol(Ekranlar.ETHERNET_KART_EKLE, IslemTurleri.EKLE))
            {
                Hata(Mesajlar.YetkinizYok);
                return;
            }
        }

        if (Request.QueryString["id"] != null && gelenId <= 0)
        {
            Hata("Geçersiz kayıt.");
            return;
        }

        IPAddress adres;
        int port;
        if (string.IsNullOrWhiteSpace(txtKartAdi.Text)
            || txtKartAdi.Text.Trim().Length > 100
            || string.IsNullOrWhiteSpace(ddlModel.SelectedValue)
            || !IPAddress.TryParse(txtIp.Text.Trim(), out adres)
            || adres.AddressFamily != AddressFamily.InterNetwork
            || !int.TryParse(txtHttpPort.Text, out port)
            || port < 1
            || port > 65535)
        {
            Hata("Kart adı, IPv4 adresi ve 1–65535 arası HTTP portu giriniz.");
            return;
        }

        if (ddlAktiflik.SelectedValue != "1" && ddlAktiflik.SelectedValue != "0")
        {
            Hata("Durum seçiniz.");
            return;
        }

        VeritabaniIslemleri db = new VeritabaniIslemleri();
        try
        {
            db.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            EthernetKartlari kayit = new EthernetKartlari(db);
            kayit.Id = gelenId;
            kayit.KartAdi = txtKartAdi.Text.Trim();
            kayit.Model = ddlModel.SelectedValue;
            kayit.Ip = adres.ToString();
            kayit.HttpPort = port;
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
                db.GeriAl();
                var islemHataMesaji1 = db.SonHataMesaji;
                if (islemHataMesaji1 != null)
                {
                    Hata(islemHataMesaji1);
                }
                else
                {
                    Hata("Kayıt tamamlanamadı.");
                }

                return;
            }

            db.Uygula();
            Session["DonanimBasari"] = "Donanım kaydı kaydedildi.";
            Response.Redirect("~/Pages/EthernetKartListele.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch
        {
            db.GeriAl();
            Hata("Kayıt işlemi tamamlanamadı.");
        }
        finally
        {
            db.Bitir();
        }
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
