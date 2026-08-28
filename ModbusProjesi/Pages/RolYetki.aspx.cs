using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class RolYetki : System.Web.UI.Page
{
    private int gelenId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROL_YETKI, IslemTurleri.GORUNTULE))
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (Request.QueryString["id"] != null)
        {
            int.TryParse(Request.QueryString["id"], out gelenId);
        }

        if (gelenId <= 0)
        {
            Response.Redirect("~/Pages/RolListeleme.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (Page.IsPostBack == false)
        {
            RolBilgisiDoldur();
            EkranlariDoldur();
            MevcutYetkileriDoldur();

            btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ROL_YETKI, IslemTurleri.GUNCELLE);
        }
    }

    private void RolBilgisiDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Roller roller = new Roller(veritabaniIslemleri);

            roller.Id = gelenId;

            if (roller.Doldur())
            {
                lblRolBilgisi.Text = roller.RolKodu + " - " + roller.Adi;
            }
            else
            {
                Response.Redirect("~/Pages/RolListeleme.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.ListelemeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    private void EkranlariDoldur()
    {
        DataTable veriTablosu = new DataTable();

        veriTablosu.Columns.Add("Ekran");

        foreach (Ekranlar ekran in Enum.GetValues(typeof(Ekranlar)))
        {
            DataRow veriSatiri = veriTablosu.NewRow();

            veriSatiri["Ekran"] = ekran.ToString();

            veriTablosu.Rows.Add(veriSatiri);
        }

        grdYetkiler.DataSource = veriTablosu;
        grdYetkiler.DataBind();
    }

    private void MevcutYetkileriDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);

            rolYetkiler.RolId = gelenId;
            rolYetkiler.RoleGoreGetir();

            if (rolYetkiler.VeriTablosu == null || rolYetkiler.VeriTablosu.Rows.Count == 0)
            {
                return;
            }

            foreach (GridViewRow satir in grdYetkiler.Rows)
            {
                string ekran = satir.Cells[0].Text;

                DataRow[] bulunanYetkiler = rolYetkiler.VeriTablosu.Select(RolYetkiler.C_Sutun_ekran + " = '" + ekran + "'");

                if (bulunanYetkiler.Length == 0)
                {
                    continue;
                }

                DataRow yetkiSatiri = bulunanYetkiler[0];

                CheckBox chkGoruntuleme = (CheckBox)satir.FindControl("chkGoruntuleme");
                CheckBox chkEkleme = (CheckBox)satir.FindControl("chkEkleme");
                CheckBox chkGuncelleme = (CheckBox)satir.FindControl("chkGuncelleme");
                CheckBox chkSilme = (CheckBox)satir.FindControl("chkSilme");
                chkGoruntuleme.Checked = Convert.ToBoolean(yetkiSatiri[RolYetkiler.C_Sutun_goruntuleme]);
                chkEkleme.Checked = Convert.ToBoolean(yetkiSatiri[RolYetkiler.C_Sutun_ekleme]);
                chkGuncelleme.Checked = Convert.ToBoolean(yetkiSatiri[RolYetkiler.C_Sutun_guncelleme]);
                chkSilme.Checked = Convert.ToBoolean(yetkiSatiri[RolYetkiler.C_Sutun_silme]);
            }
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.ListelemeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROL_YETKI, IslemTurleri.GUNCELLE))
        {
            Mesaj.Ver(Mesajlar.YetkinizYok, Mesaj.MesajTurleri.WARNING, Page.Master);

            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            veritabaniIslemleri.LogYasak = true;

            RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);

            rolYetkiler.RolId = gelenId;
            rolYetkiler.RoleGoreGetir();

            DataTable mevcutYetkiler = rolYetkiler.VeriTablosu;

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            foreach (GridViewRow satir in grdYetkiler.Rows)
            {
                string ekran = satir.Cells[0].Text;

                CheckBox chkGoruntuleme = (CheckBox)satir.FindControl("chkGoruntuleme");
                CheckBox chkEkleme = (CheckBox)satir.FindControl("chkEkleme");
                CheckBox chkGuncelleme = (CheckBox)satir.FindControl("chkGuncelleme");
                CheckBox chkSilme = (CheckBox)satir.FindControl("chkSilme");

                rolYetkiler.RolId = gelenId;
                rolYetkiler.Ekran = ekran;
                rolYetkiler.Goruntuleme = chkGoruntuleme.Checked;
                rolYetkiler.Ekleme = chkEkleme.Checked;
                rolYetkiler.Guncelleme = chkGuncelleme.Checked;
                rolYetkiler.Silme = chkSilme.Checked;

                rolYetkiler.AktifMi = true;

                DataRow[] mevcutYetki = mevcutYetkiler.Select(RolYetkiler.C_Sutun_ekran + " = '" + ekran.Replace("'", "''") + "'");

                if (mevcutYetki.Length > 0)
                {
                    rolYetkiler.Id = Convert.ToInt32(mevcutYetki[0][RolYetkiler.C_Sutun_id]);
                    rolYetkiler.GuncelleyenId = currentInfo.KullaniciId;
                    rolYetkiler.GuncelleyenIp = Utility.IpNoGetir();

                    if (!rolYetkiler.Guncelle())
                    {
                        throw new Exception(Mesajlar.RolYetkisiGuncellenemedi);
                    }
                }
                else
                {
                    rolYetkiler.EkleyenId = currentInfo.KullaniciId;
                    rolYetkiler.EkleyenIp = Utility.IpNoGetir();

                    if (!rolYetkiler.Ekle())
                    {
                        throw new Exception(Mesajlar.RolYetkisiKaydedilemedi);
                    }
                }
            }

            veritabaniIslemleri.Uygula();

            LogIslemleri.OlayKaydet("Rol Yetkileri Güncellendi", "RolYetkiler", "Rol Id: " + gelenId);

            Mesaj.Ver(Mesajlar.KayitBasarili, Mesaj.MesajTurleri.SUCCESS, Page.Master);
        }
        catch (Exception ex)
        {
            veritabaniIslemleri.GeriAl();

            Mesaj.Ver(Mesajlar.GenelHata + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnGeri_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Pages/RolEkleme.aspx?id=" + gelenId, false);

        Context.ApplicationInstance.CompleteRequest();
        return;
    }
}
