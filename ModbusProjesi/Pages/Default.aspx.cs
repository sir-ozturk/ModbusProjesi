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

        if (!IsPostBack)
        {
            BasariMesajiniGoster();
            RegisterAsyncTask(new PageAsyncTask(DonanimDurumunuYenileAsync));
        }
    }

    protected void btnDurumYenile_Click(object sender, EventArgs e)
    {
        RegisterAsyncTask(new PageAsyncTask(DonanimDurumunuYenileAsync));
    }

    private Task DonanimDurumunuYenileAsync()
    {
        CurrentInfo kullanici = new Sessionlar().Current._CurrentInfo;
        if (kullanici == null || !kullanici.LoginYapildiMi) return Task.FromResult(0);
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
        pnlBasari.Visible = true;
        lblBasari.Text = basariMesaji.ToString();
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

                var talimatSorgusu = new MakineDurdurmaTalimatlari(veritabaniIslemleri);

                foreach (DataRow makine in makineTablosu.Rows)
                {
                    makine["talimat_durum_metni"] = "Talimat yok";
                    makine["talimat_sonuc_metni"] = "";
                    makine["talimat_devam_ediyor_mu"] = false;

                    int makineId = Convert.ToInt32(makine["id"]);

                    using (DataTable talimatlar = talimatSorgusu.Listele(makineId, null, 1))
                    {
                        if (talimatlar.Rows.Count == 0)
                            continue;

                        DataRow talimat = talimatlar.Rows[0];

                        var durum = (TalimatDurumu)Convert.ToByte(talimat["islem_durumu"]);

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
        }
        catch
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.MakineBilgileriAlinamadi;
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnSiralamayiKaydet_Click(object sender, EventArgs e)
    {
        if (!makineYonetimYetkisiVar)
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.YetkinizYok;
            MakineleriGetir();
            return;
        }

        List<int> makineIdleri;

        if (!MakineIdleriniGetir(out makineIdleri))
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.MakineSiralamasiGuncellenemedi;
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
                pnlHata.Visible = true;
                lblHata.Text = Mesajlar.MakineSiralamasiGuncellenemedi;
                MakineleriGetir();
                return;
            }

            veritabaniIslemleri.Uygula();
            siralamaBasarili = true;
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.MakineSiralamasiGuncellenemedi;
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
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.YetkinizYok;
            MakineleriGetir();
            return;
        }

        int makineId;
        string durusNedeni = hdnDurusNedeni.Value.Trim();

        if (!int.TryParse(hdnDurdurMakineId.Value, out makineId) || makineId <= 0)
        {
            pnlHata.Visible = true;
            lblHata.Text = "Geçerli bir makine seçiniz.";
            MakineleriGetir();
            return;
        }

        if (string.IsNullOrWhiteSpace(durusNedeni) || durusNedeni.Length > 500)
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.DurusNedeniSeciniz;
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
                throw new InvalidOperationException("Oturumunuz sona ermiş. Tekrar giriş yapınız.");

            veritabani.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);

            // Bağlantıyı kontrol eder ve cihaz kilidini alır.
            // Röleye herhangi bir komut göndermez.
            var baglanti = new MakineRoleBaglantilari(veritabani)
            {
                MakineId = makineId
            };

            if (!baglanti.KomutBaglantisiniGetir())
                throw new InvalidOperationException(Mesajlar.MakineRoleAtamasiYok);

            var talimat = new MakineDurdurmaTalimatlari(veritabani)
            {
                MakineId = makineId,
                IslemNedeni = durusNedeni,
                EkleyenId = kullanici.KullaniciId,
                EkleyenIp = Utility.IpNoGetir()
            };

            if (!talimat.Ekle())
                throw new InvalidOperationException("Durdurma talimatı oluşturulamadı.");

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
                else if (sqlHatasi.Number == 2601 || sqlHatasi.Number == 2627)
                {
                    hataMesaji = "Bu makine için bekleyen veya işlenen " + "bir talimat zaten var.";
                }
            }
            else if (ex is InvalidOperationException)
            {
                hataMesaji = ex.Message;
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

        pnlHata.Visible = true;
        lblHata.Text = Server.HtmlEncode(hataMesaji);
        MakineleriGetir();
    }

    protected void btnMakineCalistir_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
    {
        string makineIdDegeri = Convert.ToString(e.CommandArgument);
        RegisterAsyncTask(new PageAsyncTask(() => MakineBaslatAsync(makineIdDegeri)));
    }

    private async Task MakineBaslatAsync(string makineIdDegeri)
    {
        if (!makineYonetimYetkisiVar)
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.YetkinizYok;
            MakineleriGetir();
            return;
        }

        int makineId;

        if (!int.TryParse(makineIdDegeri, out makineId) || makineId <= 0)
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.MakineCalistirilamadi;
            MakineleriGetir();
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        bool calistirmaBasarili = false;
        string hataMesaji = Mesajlar.MakineCalistirilamadi;

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);

            MakineRoleBaglantilari baglanti = new MakineRoleBaglantilari(veritabaniIslemleri);
            baglanti.MakineId = makineId;
            if (!baglanti.KomutBaglantisiniGetir())
                throw new InvalidOperationException(Mesajlar.MakineRoleAtamasiYok);

            var talimatKontrol = new MakineDurdurmaTalimatlari(veritabaniIslemleri);

            bool bekleyenTalimatVar;

            using (DataTable bekleyenler = talimatKontrol.Listele(makineId, TalimatDurumu.Bekliyor, 1))
            {
                bekleyenTalimatVar = bekleyenler.Rows.Count > 0;
            }

            bool islenenTalimatVar;

            using (DataTable islenenler = talimatKontrol.Listele(makineId, TalimatDurumu.Isleniyor, 1))
            {
                islenenTalimatVar = islenenler.Rows.Count > 0;
            }

            if (bekleyenTalimatVar || islenenTalimatVar)
            {
                throw new DonanimIslemHatasi("Bu makine için durdurma talimatı bekliyor veya " + "işleniyor. Talimat sonuçlanmadan başlatma " + "işlemi yapılamaz.");
            }

            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.Id = makineId;
            bool roleAtamasiVar = makineler.Doldur() && makineler.AktifMi;

            MakineLoglari makineLoglari = new MakineLoglari(veritabaniIslemleri);
            makineLoglari.MakineId = makineId;

            if (!roleAtamasiVar)
            {
                hataMesaji = Mesajlar.MakineRoleAtamasiYok;
                veritabaniIslemleri.GeriAl();
            }
            else if (!makineLoglari.AcikKayitGetir())
            {
                hataMesaji = Mesajlar.MakineZatenCalisiyor;
                veritabaniIslemleri.GeriAl();
            }
            else
            {
                Sessionlar sessionlar = new Sessionlar();
                CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

                makineLoglari.GuncelleyenId = currentInfo.KullaniciId;
                makineLoglari.GuncelleyenIp = Utility.IpNoGetir();

                MakineRoleIslemleri.KanalDurumSonucu mevcutDurum = await MakineRoleIslemleri.KanalDurumunuGetirAsync(baglanti);
                if (!mevcutDurum.Basarili)
                {
                    hataMesaji = mevcutDurum.Hata;
                    veritabaniIslemleri.GeriAl();
                }
                else if (!mevcutDurum.DuruyorMu)
                {
                    if (makineLoglari.Kapat()) { veritabaniIslemleri.Uygula(); calistirmaBasarili = true; }
                    else veritabaniIslemleri.GeriAl();
                }
                else
                {
                    string roleHatasi = await MakineRoleIslemleri.MakineBaslatAsync(baglanti);
                    if (roleHatasi == null) roleHatasi = await MakineRoleIslemleri.KanalDurumunuDogrulaAsync(baglanti, false);
                    if (roleHatasi != null)
                    {
                        hataMesaji = roleHatasi;
                        veritabaniIslemleri.GeriAl();
                    }
                    else if (makineLoglari.Kapat())
                    {
                        hataMesaji = Mesajlar.RoleKomutuKaydedilemedi;
                        veritabaniIslemleri.Uygula();
                        calistirmaBasarili = true;
                    }
                    else veritabaniIslemleri.GeriAl();
                }
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number >= 51000 && ex.Number <= 51010) hataMesaji = ex.Message;
            veritabaniIslemleri.GeriAl();
        }
        catch (DonanimIslemHatasi ex)
        {
            hataMesaji = ex.Message;
            veritabaniIslemleri.GeriAl();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == Mesajlar.MakineRoleAtamasiYok) hataMesaji = ex.Message;
            veritabaniIslemleri.GeriAl();
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }

        if (calistirmaBasarili)
        {
            Session[C_Session_DashboardBasari] = Mesajlar.RoleBaslatmaKomutuGonderildi;
            Response.Redirect("~/Pages/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        pnlHata.Visible = true;
        lblHata.Text = Server.HtmlEncode(hataMesaji);
        MakineleriGetir();
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

    protected string MakineKartSinifi(object duruyorMu)
    {
        return Convert.ToBoolean(duruyorMu)
            ? "makine-kart makine-kart-duruyor h-100"
            : "makine-kart makine-kart-calisiyor h-100";
    }

    protected string MakineDurumIkonu(object duruyorMu)
    {
        return Convert.ToBoolean(duruyorMu)
            ? "fa-solid fa-circle-stop me-1"
            : "fa-solid fa-circle-play me-1";
    }

    protected string MakineDurumMetni(object duruyorMu)
    {
        return Convert.ToBoolean(duruyorMu) ? "Duruyor" : "Çalışıyor";
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
