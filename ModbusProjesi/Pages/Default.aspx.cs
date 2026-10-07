using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.UI;

public partial class Default : System.Web.UI.Page
{
    private const string C_Session_DashboardBasari = "MakineDashboardBasari";
    private bool makineYonetimYetkisiVar;
    protected void Page_Load(object sender, EventArgs e)
    {
        makineYonetimYetkisiVar = IslemYetki.Kontrol(Ekranlar.MAKINE_LISTELE, IslemTurleri.GUNCELLE);
        btnSiralamaAc.Visible = makineYonetimYetkisiVar;
        if (makineYonetimYetkisiVar)
        {
            DurusNedenleriniGetir();
        }

        if (!IsPostBack)
        {
            BasariMesajiniGoster();
            RegisterAsyncTask(new PageAsyncTask(DonanimDurumunuYenileAsync));
        }
    }

    private void DurusNedenleriniGetir()
    {
        VeritabaniIslemleri veritabani = new VeritabaniIslemleri();
        btnDurdurmayiOnayla.Enabled = false;
        try
        {
            veritabani.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            using (DataTable tablo = new Parametreler(veritabani).GrubaGoreGetir(ParametreGruplari.C_Grup_MakineDurusNedeni))
            {
                rptDurusNedenleri.DataSource = tablo;
                rptDurusNedenleri.DataBind();
                btnDurdurmayiOnayla.Enabled = tablo.Rows.Count > 0;
                if (tablo.Rows.Count == 0)
                {
                    lblDurusNedeniBilgi.Text = "Aktif duruş nedeni bulunamadı.";
                }
                else
                {
                    lblDurusNedeniBilgi.Text = "";
                }
            }
        }
        catch (Exception ex)
        {
            RelayPulseService.LogError("Duruş nedenleri yükleme: " + ex);
            lblDurusNedeniBilgi.Text = "Duruş nedenleri yüklenemedi. Sayfayı yenileyiniz.";
        }
        finally
        {
            veritabani.Bitir();
        }
    }

    protected void btnDurumYenile_Click(object sender, EventArgs e)
    {
        RegisterAsyncTask(new PageAsyncTask(DonanimDurumunuYenileAsync));
    }

    private Task DonanimDurumunuYenileAsync()
    {
        CurrentInfo kullanici = new Sessionlar().Current._CurrentInfo;
        if (kullanici == null || !kullanici.LoginYapildiMi)
        {
            return Task.FromResult(0);
        }

        // Makinenin durumu duruş kayıtlarından okunur.
        lblDonanimDurumu.Visible = false;
        MakineleriGetir();
        return Task.FromResult(0);
    }

    private void BasariMesajiniGoster()
    {
        object basariMesaji = Session[C_Session_DashboardBasari];
        if (basariMesaji == null)
        {
            return;
        }

        Session.Remove(C_Session_DashboardBasari);
        DashboardMesajiGoster(basariMesaji.ToString(), "SUCCESS");
    }

    private void DashboardMesajiGoster(string metin, string tur)
    {
        Mesaj.Ver(metin, (Mesaj.MesajTurleri)Enum.Parse(typeof(Mesaj.MesajTurleri), tur), Master);
    }

    private void MakineleriGetir()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.DashboardGetir();
            DataTable makineTablosu = makineler.VeriTablosu;
            if (makineTablosu != null)
            {
                makineTablosu.Columns.Add("talimat_durum_metni", typeof(string));
                makineTablosu.Columns.Add("talimat_sonuc_metni", typeof(string));
                makineTablosu.Columns.Add("talimat_devam_ediyor_mu", typeof(bool));
                makineTablosu.Columns.Add("talimat_durumu", typeof(int));
                var talimatSorgusu = new MakineDurdurmaTalimatlari(veritabaniIslemleri);
                foreach (DataRow makine in makineTablosu.Rows)
                {
                    makine["talimat_durum_metni"] = "Talimat yok";
                    makine["talimat_sonuc_metni"] = "";
                    makine["talimat_devam_ediyor_mu"] = false;
                    makine["talimat_durumu"] = -1;
                    int makineId = Convert.ToInt32(makine["id"]);
                    using (DataTable talimatlar = talimatSorgusu.Listele(makineId, null, 1))
                    {
                        if (talimatlar.Rows.Count == 0)
                        {
                            continue;
                        }

                        DataRow talimat = talimatlar.Rows[0];
                        var durum = (TalimatDurumu)Convert.ToByte(talimat["islem_durumu"]);
                        makine["talimat_durumu"] = (int)durum;
                        string durumMetni;
                        switch (durum)
                        {
                            case TalimatDurumu.Bekliyor:
                                durumMetni = "Bekliyor";
                                break;
                            case TalimatDurumu.Isleniyor:
                                durumMetni = "İşleniyor";
                                break;
                            case TalimatDurumu.Tamamlandi:
                                durumMetni = "Tamamlandı";
                                break;
                            case TalimatDurumu.Hatali:
                                durumMetni = "Hatalı";
                                break;
                            case TalimatDurumu.KontrolGerekli:
                                durumMetni = "Kontrol gerekli";
                                break;
                            default:
                                durumMetni = "Bilinmeyen durum";
                                break;
                        }

                        makine["talimat_durum_metni"] = "Talimat #" + talimat["id"] + " — " + durumMetni;
                        makine["talimat_sonuc_metni"] = Convert.ToString(talimat["sonuc"]);
                        makine["talimat_devam_ediyor_mu"] = durum == TalimatDurumu.Bekliyor || durum == TalimatDurumu.Isleniyor;
                    }
                }
            }

            pnlMakineYok.Visible = makineTablosu == null || makineTablosu.Rows.Count == 0;
            rptMakineler.DataSource = makineTablosu;
            rptMakineler.DataBind();
            rptSiralanabilirMakineler.DataSource = makineTablosu;
            rptSiralanabilirMakineler.DataBind();
            TakipBilgileriniGetir(makineTablosu);
        }
        catch
        {
            pnlTakip.Visible = false;
            DashboardMesajiGoster(Mesajlar.MakineBilgileriAlinamadi, "FAIL");
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    private void TakipBilgileriniGetir(DataTable makineler)
    {
        pnlTakip.Visible = makineler != null && makineler.Rows.Count > 0;
        if (!pnlTakip.Visible) return;

        var kayitlar = new DataTable();
        kayitlar.Columns.Add("id", typeof(int));
        kayitlar.Columns.Add("makine", typeof(string));
        kayitlar.Columns.Add("durum", typeof(string));
        kayitlar.Columns.Add("aciklama", typeof(string));
        kayitlar.Columns.Add("sinif", typeof(string));
        kayitlar.Columns.Add("oncelik", typeof(int));
        kayitlar.Columns.Add("dakika", typeof(int));
        int durus = 0, bekleyen = 0, kontrol = 0;
        foreach (DataRow makine in makineler.Rows)
        {
            bool duruyor = Convert.ToBoolean(makine["duruyor_mu"]);
            int talimat = Convert.ToInt32(makine["talimat_durumu"]);
            int dakika = duruyor ? Convert.ToInt32(makine["durus_dakika"]) : 0;
            if (duruyor) durus++;
            if (talimat == (int)TalimatDurumu.Bekliyor || talimat == (int)TalimatDurumu.Isleniyor) bekleyen++;
            if (talimat == (int)TalimatDurumu.KontrolGerekli) kontrol++;

            string durum, aciklama, sinif;
            int oncelik;
            if (talimat == (int)TalimatDurumu.KontrolGerekli)
            {
                durum = "Kontrol gerekli";
                aciklama = "Talimat sonucu için makineyi ve röle bağlantısını kontrol edin.";
                sinif = "takip-kontrol";
                oncelik = 0;
            }
            else if (talimat == (int)TalimatDurumu.Bekliyor || talimat == (int)TalimatDurumu.Isleniyor)
            {
                bool isleniyor = talimat == (int)TalimatDurumu.Isleniyor;
                durum = isleniyor ? "İşleniyor" : "Bekliyor";
                aciklama = isleniyor ? "Durdurma talimatı uygulanıyor." : "Talimatın uygulanması bekleniyor.";
                sinif = "takip-bekleyen";
                oncelik = 1;
            }
            else if (duruyor)
            {
                durum = dakika + " dk duruş";
                aciklama = Convert.ToString(makine["islem_nedeni"]);
                sinif = "takip-durus";
                oncelik = 2;
            }
            else continue;

            kayitlar.Rows.Add(makine["id"], makine["makine_adi"], durum, aciklama, sinif, oncelik, dakika);
        }

        lblAcikDurus.Text = durus.ToString();
        lblBekleyenTalimat.Text = bekleyen.ToString();
        lblKontrolTalimat.Text = kontrol.ToString();
        var sirali = kayitlar.AsEnumerable().OrderBy(r => r.Field<int>("oncelik"))
            .ThenByDescending(r => r.Field<int>("dakika")).Take(5).ToList();
        rptTakip.DataSource = sirali.Count > 0 ? sirali.CopyToDataTable() : kayitlar;
        rptTakip.DataBind();
        pnlTakipBos.Visible = kayitlar.Rows.Count == 0;
        lblTakipBilgi.Text = kayitlar.Rows.Count > 5 ? "Öncelikli 5 makine gösteriliyor." : "";
    }

    protected void btnSiralamayiKaydet_Click(object sender, EventArgs e)
    {
        if (!makineYonetimYetkisiVar)
        {
            DashboardMesajiGoster(Mesajlar.YetkinizYok, "FAIL");
            MakineleriGetir();
            return;
        }

        List<int> makineIdleri;
        if (!MakineIdleriniGetir(out makineIdleri))
        {
            DashboardMesajiGoster(Mesajlar.MakineSiralamasiGuncellenemedi, "FAIL");
            MakineleriGetir();
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        bool siralamaBasarili = false;
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.GuncelleyenId = currentInfo.KullaniciId;
            makineler.GuncelleyenIp = Utility.IpNoGetir();
            if (!makineler.SiralamayiGuncelle(SiralamaXmlOlustur(makineIdleri)))
            {
                veritabaniIslemleri.GeriAl();
                DashboardMesajiGoster(Mesajlar.MakineSiralamasiGuncellenemedi, "FAIL");
                MakineleriGetir();
                return;
            }

            veritabaniIslemleri.Uygula();
            siralamaBasarili = true;
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
            DashboardMesajiGoster(Mesajlar.MakineSiralamasiGuncellenemedi, "FAIL");
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }

        if (siralamaBasarili)
        {
            Session[C_Session_DashboardBasari] = Mesajlar.MakineSiralamasiGuncellendi;
            Response.Redirect("~/Pages/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        MakineleriGetir();
    }

    protected void btnDurdurmayiOnayla_Click(object sender, EventArgs e)
    {
        DurdurmaTalimatiniOlustur();
    }

    private void DurdurmaTalimatiniOlustur()
    {
        if (!makineYonetimYetkisiVar)
        {
            DashboardMesajiGoster(Mesajlar.YetkinizYok, "FAIL");
            MakineleriGetir();
            return;
        }

        int makineId;
        int durusNedeniId;
        if (!int.TryParse(hdnDurdurMakineId.Value, out makineId) || makineId <= 0)
        {
            DashboardMesajiGoster("Geçerli bir makine seçiniz.", "FAIL");
            MakineleriGetir();
            return;
        }

        if (!int.TryParse(hdnDurusNedeni.Value, out durusNedeniId) || durusNedeniId <= 0)
        {
            DashboardMesajiGoster(Mesajlar.DurusNedeniSeciniz, "FAIL");
            MakineleriGetir();
            return;
        }

        var veritabani = new VeritabaniIslemleri();
        bool talimatOlustu = false;
        string hataMesaji = "Talimat kaydı doğrulanamadı. Tekrar denemeden önce " + "talimat tablosunu kontrol ediniz.";
        try
        {
            CurrentInfo kullanici = new Sessionlar().Current._CurrentInfo;
            if (kullanici == null || !kullanici.LoginYapildiMi)
            {
                throw new InvalidOperationException("Oturumunuz sona ermiş. Tekrar giriş yapınız.");
            }

            veritabani.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            // Bağlantıyı kontrol eder ve cihaz kilidini alır.
            // Röleye herhangi bir komut göndermez.
            var baglanti = new MakineRoleBaglantilari(veritabani)
            {
                MakineId = makineId
            };
            if (!baglanti.KomutBaglantisiniGetir())
            {
                throw new InvalidOperationException(Mesajlar.MakineRoleAtamasiYok);
            }

            var talimat = new MakineDurdurmaTalimatlari(veritabani)
            {
                MakineId = makineId,
                DurusNedeniParametreId = durusNedeniId,
                DurusAciklamasi = txtOzelDurusNedeni.Text,
                EkleyenId = kullanici.KullaniciId,
                EkleyenIp = Utility.IpNoGetir()
            };
            if (!talimat.Ekle())
            {
                var islemHataMesaji1 = veritabani.SonHataMesaji;
                if (islemHataMesaji1 != null)
                {
                    throw new InvalidOperationException(islemHataMesaji1);
                }
                else
                {
                    throw new InvalidOperationException("Durdurma talimatı oluşturulamadı.");
                }
            }

            veritabani.Uygula();
            talimatOlustu = true;
        }
        catch (Exception ex)
        {
            RelayPulseService.LogError("Makine " + makineId + " talimat ekleme: " + ex);
            var sqlHatasi = ex as SqlException;
            if (sqlHatasi != null)
            {
                if (sqlHatasi.Number >= 51100 && sqlHatasi.Number <= 51118)
                {
                    hataMesaji = sqlHatasi.Message;
                }
                else
                {
                    if (sqlHatasi.Number == 2601 || sqlHatasi.Number == 2627)
                    {
                        hataMesaji = "Bu makine için bekleyen veya işlenen " + "bir talimat zaten var.";
                    }
                }
            }
            else
            {
                if (ex is InvalidOperationException)
                {
                    hataMesaji = ex.Message;
                }
            }

            // Prosedür işlemi zaten geri almış olabilir.
            try
            {
                veritabani.GeriAl();
            }
            catch (Exception geriAlmaHatasi)
            {
                RelayPulseService.LogError("Talimat işlemi geri alma: " + geriAlmaHatasi);
            }
        }
        finally
        {
            veritabani.Bitir();
        }

        if (talimatOlustu)
        {
            Session[C_Session_DashboardBasari] = "Durdurma talimatı alındı. İşleyici uygulamanın " + "talimatı uygulaması bekleniyor.";
            Response.Redirect("~/Pages/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        DashboardMesajiGoster(hataMesaji, "FAIL");
        lblDurusNedeniBilgi.Text = Server.HtmlEncode(hataMesaji);
        MakineleriGetir();
        ScriptManager.RegisterStartupScript(this, GetType(), "DurusSeciminiGeriYukle", "window.addEventListener('load', function () { durusSeciminiGeriYukle(); });", true);
    }

    private bool MakineIdleriniGetir(out List<int> makineIdleri)
    {
        makineIdleri = new List<int>();
        if (string.IsNullOrWhiteSpace(hdnMakineSiralamasi.Value))
        {
            return false;
        }

        string[] idDegerleri = hdnMakineSiralamasi.Value.Split(',');
        foreach (string idDegeri in idDegerleri)
        {
            int makineId;
            if (!int.TryParse(idDegeri, out makineId) || makineId <= 0)
            {
                return false;
            }

            makineIdleri.Add(makineId);
        }

        return makineIdleri.Count > 0 && makineIdleri.Count == makineIdleri.Distinct().Count();
    }

    private string SiralamaXmlOlustur(List<int> makineIdleri)
    {
        StringBuilder xml = new StringBuilder();
        xml.Append("<Makineler>");
        for (int i = 0; i < makineIdleri.Count; i++)
        {
            xml.Append("<Makine id=\"");
            xml.Append(makineIdleri[i]);
            xml.Append("\" sira=\"");
            xml.Append(i + 1);
            xml.Append("\" />");
        }

        xml.Append("</Makineler>");
        return xml.ToString();
    }

    protected string DurdurmaModalAcmaKodu(object id, object makineAdi, object makineNo, object ip)
    {
        string guvenliMakineAdi = System.Web.HttpUtility.JavaScriptStringEncode(makineAdi.ToString());
        string guvenliMakineNo = System.Web.HttpUtility.JavaScriptStringEncode(makineNo.ToString());
        string guvenliIp = System.Web.HttpUtility.JavaScriptStringEncode(ip.ToString());
        return "makineDurdurmaModaliniAc(" + Convert.ToInt32(id) + ", '" + guvenliMakineAdi + "', '" + guvenliMakineNo + "', '" + guvenliIp + "'); return false;";
    }

    protected bool MakineRoleKontrolYetkisiVarMi(object roleBagliMi)
    {
        return makineYonetimYetkisiVar && Convert.ToBoolean(roleBagliMi);
    }

    protected string RoleBaglantiMetni(object roleBagliMi, object roleAdi, object kanalNo)
    {
        if (Convert.ToBoolean(roleBagliMi))
        {
            return Server.HtmlEncode(roleAdi + " / Kanal " + kanalNo);
        }
        else
        {
            return "Aktif bağlantı yok";
        }
    }

    protected string AciklamaZorunlulukDegeri(object aciklamaZorunluMu)
    {
        if (Convert.ToBoolean(aciklamaZorunluMu))
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

    protected string MakineKartSinifi(object duruyorMu)
    {
        if (Convert.ToBoolean(duruyorMu))
        {
            return "makine-kart makine-kart-duruyor h-100";
        }
        else
        {
            return "makine-kart makine-kart-calisiyor h-100";
        }
    }

    protected string MakineDurumIkonu(object duruyorMu)
    {
        if (Convert.ToBoolean(duruyorMu))
        {
            return "fa-solid fa-circle-stop me-1";
        }
        else
        {
            return "fa-solid fa-circle-play me-1";
        }
    }

    protected string MakineDurumMetni(object duruyorMu)
    {
        if (Convert.ToBoolean(duruyorMu))
        {
            return "Duruyor";
        }
        else
        {
            return "Çalışıyor";
        }
    }

    protected string DurusSuresiMetni(object duruyorMu, object durusDakika)
    {
        if (!Convert.ToBoolean(duruyorMu))
        {
            return "Aktif";
        }

        return Convert.ToInt32(durusDakika) + " dk";
    }

    protected string DurusNedeniMetni(object duruyorMu, object islemNedeni)
    {
        if (!Convert.ToBoolean(duruyorMu))
        {
            return "-";
        }

        if (islemNedeni == null || islemNedeni == DBNull.Value || string.IsNullOrWhiteSpace(islemNedeni.ToString()))
        {
            return "Belirtilmedi";
        }

        return Server.HtmlEncode(islemNedeni.ToString());
    }
}
