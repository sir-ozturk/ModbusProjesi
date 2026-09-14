using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

public partial class Default : System.Web.UI.Page
{
    private const string C_Session_DashboardBasari = "MakineDashboardBasari";
    private bool makineYonetimYetkisiVar;
    private static readonly SemaphoreSlim roleIslemKilidi = new SemaphoreSlim(1, 1);

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

    private async Task DonanimDurumunuYenileAsync()
    {
        CurrentInfo kullanici = new Sessionlar().Current._CurrentInfo;
        if (kullanici == null || !kullanici.LoginYapildiMi) return;
        if (!await roleIslemKilidi.WaitAsync(0))
        {
            lblDonanimDurumu.Text = "Röle işlemi sürüyor; IO durumu sonraki okumada yenilenecek.";
            lblDonanimDurumu.Visible = true;
            lblDonanimDurumu.CssClass = "d-block text-secondary mb-2";
            MakineleriGetir();
            return;
        }
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            // Aynı kilit, okuma sırasında uygulamadan karşıt komut gönderilmesini önler.
            string durum = await MakineRoleIslemleri.DurumOkuAsync();
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.DashboardGetir();
            foreach (DataRow satir in makineler.VeriTablosu.Rows)
            {
                if (satir["relay_channel"] == DBNull.Value) continue;
                bool duruyor = MakineRoleIslemleri.DurumdanDuruyorMu(durum, Convert.ToInt32(satir["relay_channel"]));
                MakineLoglari log = new MakineLoglari(veritabaniIslemleri) { MakineId = Convert.ToInt32(satir["id"]) };
                bool acikKayit = log.AcikKayitGetir();
                if (duruyor == acikKayit) continue;
                // Kullanıcı kaydı gözlemleyen oturumdur; duruş nedeni donanım kaynağını belirtir.
                if (duruyor)
                {
                    log.IslemTipi = MakineLoglari.C_IslemTipi_Durdur;
                    log.IslemNedeni = "Donanım üzerinden durduruldu (IO Control)";
                    log.DevamEdiyorMu = true;
                    log.BasariliMi = true;
                    log.AktifMi = true;
                    log.EkleyenId = kullanici.KullaniciId;
                    log.EkleyenIp = Utility.IpNoGetir();
                    if (!log.Ekle()) throw new InvalidOperationException("Duruş kaydı oluşturulamadı.");
                }
                else if (!log.Kapat()) throw new InvalidOperationException("Duruş kaydı kapatılamadı.");
            }
            veritabaniIslemleri.Uygula();
            lblDonanimDurumu.Text = string.Empty;
            lblDonanimDurumu.Visible = false;
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
            lblDonanimDurumu.Text = "Donanım durumu doğrulanamadı veya kaydedilemedi. Gösterilen bilgiler son veritabanı kayıtlarıdır.";
            lblDonanimDurumu.Visible = true;
            lblDonanimDurumu.CssClass = "d-block text-danger mb-2";
        }
        finally
        {
            veritabaniIslemleri.Bitir();
            roleIslemKilidi.Release();
        }
        MakineleriGetir();
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
        RegisterAsyncTask(new PageAsyncTask(() => RoleIsleminiYurutAsync(MakineDurdurAsync)));
    }

    private async Task RoleIsleminiYurutAsync(Func<Task> islem)
    {
        // Aynı uygulamadaki farklı oturumlardan gelen karşıt komutları da engelle.
        if (!await roleIslemKilidi.WaitAsync(0))
        {
            pnlHata.Visible = true;
            lblHata.Text = Mesajlar.RoleKomutuIsleniyor;
            MakineleriGetir();
            return;
        }

        try
        {
            await islem();
        }
        finally
        {
            roleIslemKilidi.Release();
        }
    }

    private async Task MakineDurdurAsync()
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
            lblHata.Text = Mesajlar.MakineDurdurulamadi;
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

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        bool durdurmaBasarili = false;
        string hataMesaji = Mesajlar.MakineDurdurulamadi;

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);

            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.Id = makineId;

            if (!makineler.Doldur() || !makineler.AktifMi || makineler.RelayChannel != 1 || !MakineRoleIslemleri.RoleBirMakinesiMi(makineler.MakineNo))
            {
                hataMesaji = Mesajlar.MakineRoleAtamasiYok;
                veritabaniIslemleri.GeriAl();
            }
            else
            {
                MakineLoglari makineLoglari = new MakineLoglari(veritabaniIslemleri);
                makineLoglari.MakineId = makineId;

                if (makineLoglari.AcikKayitGetir())
                {
                    hataMesaji = Mesajlar.MakineZatenDuruyor;
                    veritabaniIslemleri.GeriAl();
                }
                else
                {
                    Sessionlar sessionlar = new Sessionlar();
                    CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

                    makineLoglari.MakineId = makineId;
                    makineLoglari.IslemTipi = MakineLoglari.C_IslemTipi_Durdur;
                    makineLoglari.IslemNedeni = durusNedeni;
                    makineLoglari.DevamEdiyorMu = true;
                    makineLoglari.BasariliMi = true;
                    makineLoglari.HataMesaji = null;
                    makineLoglari.AktifMi = true;
                    makineLoglari.EkleyenId = currentInfo.KullaniciId;
                    makineLoglari.EkleyenIp = Utility.IpNoGetir();

                    if (makineLoglari.Ekle())
                    {
                        string roleHatasi = await MakineRoleIslemleri.MakineDurdurAsync(makineler);
                        if (roleHatasi == null)
                        {
                            hataMesaji = Mesajlar.RoleKomutuKaydedilemedi;
                            veritabaniIslemleri.Uygula();
                            durdurmaBasarili = true;
                        }
                        else
                        {
                            hataMesaji = roleHatasi;
                            veritabaniIslemleri.GeriAl();
                        }
                    }
                    else
                    {
                        veritabaniIslemleri.GeriAl();
                    }
                }
            }
        }
        catch
        {
            veritabaniIslemleri.GeriAl();
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }

        if (durdurmaBasarili)
        {
            Session[C_Session_DashboardBasari] = Mesajlar.RoleDurdurmaKomutuGonderildi;
            Response.Redirect("~/Pages/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        pnlHata.Visible = true;
        lblHata.Text = hataMesaji;
        MakineleriGetir();
    }

    protected void btnMakineCalistir_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
    {
        string makineIdDegeri = Convert.ToString(e.CommandArgument);
        RegisterAsyncTask(new PageAsyncTask(() => RoleIsleminiYurutAsync(() => MakineBaslatAsync(makineIdDegeri))));
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

            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.Id = makineId;
            bool roleAtamasiVar = makineler.Doldur() && makineler.AktifMi && makineler.RelayChannel == 1
                && MakineRoleIslemleri.RoleBirMakinesiMi(makineler.MakineNo);

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

                if (makineLoglari.Kapat())
                {
                    string roleHatasi = await MakineRoleIslemleri.MakineBaslatAsync(makineler);
                    if (roleHatasi == null)
                    {
                        hataMesaji = Mesajlar.RoleKomutuKaydedilemedi;
                        veritabaniIslemleri.Uygula();
                        calistirmaBasarili = true;
                    }
                    else
                    {
                        hataMesaji = roleHatasi;
                        veritabaniIslemleri.GeriAl();
                    }
                }
                else
                {
                    veritabaniIslemleri.GeriAl();
                }
            }
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
        lblHata.Text = hataMesaji;
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

    protected bool MakineRoleKontrolYetkisiVarMi(object makineNo, object relayChannel)
    {
        return makineYonetimYetkisiVar && relayChannel != DBNull.Value && Convert.ToInt32(relayChannel) == 1
            && MakineRoleIslemleri.RoleBirMakinesiMi(Convert.ToString(makineNo));
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
