using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.UI.WebControls;

public partial class RoleKartEkle : System.Web.UI.Page
{
    private int gelenId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.GORUNTULE))
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
            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.GUNCELLE);
        }
        else
        {
            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.EKLE);
        }

        if (gelenId > 0)
        {
            litBaslik.Text = "Röle Kartı " + "Düzenle";
        }
        else
        {
            litBaslik.Text = "Röle Kartı " + "Ekle";
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
            RoleKartlari kayit = new RoleKartlari(db);
            kayit.Id = gelenId;
            if (gelenId > 0 && !kayit.Doldur())
            {
                Hata("Kayıt bulunamadı.");
                btnKaydet.Enabled = false;
                return;
            }

            EthernetleriDoldur(db, kayit.EthernetKartId);
            if (gelenId > 0)
            {
                txtRoleAdi.Text = kayit.RoleAdi.ToString();
                if (kayit.AktifMi)
                {
                    ddlAktiflik.SelectedValue = "1";
                }
                else
                {
                    ddlAktiflik.SelectedValue = "0";
                }
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
            if (!IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.GORUNTULE) || !IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.GUNCELLE))
            {
                Hata(Mesajlar.YetkinizYok);
                return;
            }
        }
        else
        {
            if (!IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.GORUNTULE) || !IslemYetki.Kontrol(Ekranlar.ROLE_KART_EKLE, IslemTurleri.EKLE))
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

        int ethernetId;
        if (string.IsNullOrWhiteSpace(txtRoleAdi.Text)
            || txtRoleAdi.Text.Trim().Length > 100
            || !int.TryParse(ddlEthernet.SelectedValue, out ethernetId)
            || ethernetId <= 0)
        {
            Hata("Röle adı ve Ethernet kartı seçiniz.");
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
            RoleKartlari kayit = new RoleKartlari(db);
            kayit.Id = gelenId;
            kayit.RoleAdi = txtRoleAdi.Text.Trim();
            kayit.EthernetKartId = ethernetId;
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
            Response.Redirect("~/Pages/RoleKartListele.aspx", false);
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

    private void EthernetleriDoldur(VeritabaniIslemleri db, int seciliId)
    {
        EthernetKartlari kartlar = new EthernetKartlari(db);
        kartlar.TumunuGetir();
        RoleKartlari roleler = new RoleKartlari(db);
        roleler.TumunuGetir();
        ddlEthernet.Items.Clear();
        ddlEthernet.Items.Add(new ListItem("Ethernet kartı seçiniz", ""));
        foreach (DataRow r in kartlar.VeriTablosu.Rows)
        {
            int id = Convert.ToInt32(r["id"]);
            if (!Convert.ToBoolean(r["aktif_mi"]) && id != seciliId)
            {
                continue;
            }

            bool dolu = roleler.VeriTablosu.AsEnumerable().Any(x => Convert.ToInt32(x["ethernet_kart_id"]) == id && Convert.ToInt32(x["id"]) != gelenId);
            ListItem item;
            if (dolu)
            {
                item = new ListItem(r["kart_adi"] + " — " + r["adres"] + " (Dolu)", id.ToString());
            }
            else
            {
                item = new ListItem(r["kart_adi"] + " — " + r["adres"] + "", id.ToString());
            }

            item.Enabled = !dolu;
            ddlEthernet.Items.Add(item);
        }

        if (ddlEthernet.Items.FindByValue(seciliId.ToString()) != null)
        {
            ddlEthernet.SelectedValue = seciliId.ToString();
        }
    }
}
