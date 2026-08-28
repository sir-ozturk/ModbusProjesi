using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class RolEkleme : System.Web.UI.Page
{
    private int gelenId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.GORUNTULE))
        {
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (Request.QueryString["id"] != null)
        {
            int.TryParse(Request.QueryString["id"], out gelenId);
        }

        if (Page.IsPostBack == false)
        {
            if (gelenId > 0)
            {
                RolDoldur();

                btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.GUNCELLE);
                btnSil.Enabled = IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.SIL);
                btnYetkiler.Enabled = IslemYetki.Kontrol(Ekranlar.ROL_YETKI, IslemTurleri.GUNCELLE);
            }
            else
            {
                YeniRolHazirla();

                btnKaydet.Enabled = IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.EKLE);

                btnSil.Enabled = false;
                btnYetkiler.Enabled = false;
            }
        }
    }

    private void YeniRolHazirla()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Roller roller = new Roller(veritabaniIslemleri);

            int yeniId = roller.MaxIdGetir() + 1;

            txtRolKodu.Text = "R" + yeniId.ToString("D3");
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

    private void RolDoldur()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Roller roller = new Roller(veritabaniIslemleri);

            roller.Id = gelenId;

            if (roller.Doldur())
            {
                txtRolKodu.Text = roller.RolKodu;
                txtRolAdi.Text = roller.Adi;
                txtAciklama.Text = roller.Aciklama;
                ddlAktiflik.SelectedValue = roller.AktifMi ? "1" : "0";
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
        if (gelenId == 0)
        {
            if (!IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.EKLE))
            {
                return;
            }
        }
        else
        {
            if (!IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.GUNCELLE))
            {
                return;
            }
        }

        if (string.IsNullOrEmpty(txtRolAdi.Text.Trim()) || string.IsNullOrEmpty(ddlAktiflik.SelectedValue))
        {
            Mesaj.Ver(Mesajlar.RolAlanlarBos, Mesaj.MesajTurleri.WARNING, Page.Master);

            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            Roller roller = new Roller(veritabaniIslemleri);

            roller.RolKodu = txtRolKodu.Text.Trim();
            roller.Adi = txtRolAdi.Text.Trim();
            roller.Aciklama = txtAciklama.Text.Trim();
            roller.AktifMi = ddlAktiflik.SelectedValue == "1";

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (gelenId == 0)
            {
                roller.EkleyenId = currentInfo.KullaniciId;
                roller.EkleyenIp = Utility.IpNoGetir();

                if (roller.Ekle())
                {
                    int yeniId = roller.MaxIdGetir();

                    Session["BasariMesaji"] = Mesajlar.RolBasariylaEklendi;

                    Response.Redirect("~/Pages/RolEkleme.aspx?id=" + yeniId, false);

                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            else
            {
                roller.Id = gelenId;
                roller.GuncelleyenId = currentInfo.KullaniciId;
                roller.GuncelleyenIp = Utility.IpNoGetir();

                if (roller.Guncelle())
                {
                    Mesaj.Ver(Mesajlar.RolBasariylaGuncellendi, Mesaj.MesajTurleri.SUCCESS, Page.Master);
                }
            }
        }
        catch (Exception ex)
        {
            Mesaj.Ver(Mesajlar.GenelHata + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnSil_Click(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROL_EKLE, IslemTurleri.SIL))
        {
            return;
        }

        if (gelenId <= 0)
        {
            return;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            veritabaniIslemleri.LogYasak = true;

            RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
            rolYetkiler.RolId = gelenId;

            if (!rolYetkiler.RoleGoreSil())
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.RolYetkileriSilinemedi, Mesaj.MesajTurleri.FAIL, Page.Master);
                return;
            }

            Roller roller = new Roller(veritabaniIslemleri);

            roller.Id = gelenId;

            if (!roller.Sil())
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.RolSilinemedi, Mesaj.MesajTurleri.WARNING, Page.Master);
                return;
            }

            veritabaniIslemleri.Uygula();
            Session["BasariMesaji"] = Mesajlar.SilmeBasarili;

            Response.Redirect("~/Pages/RolListeleme.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        catch (Exception ex)
        {
            veritabaniIslemleri.GeriAl();
            Mesaj.Ver(Mesajlar.SilmeHatasi + ex.Message, Mesaj.MesajTurleri.FAIL, Page.Master);
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }

    protected void btnYetkiler_Click(object sender, EventArgs e)
    {
        if (!IslemYetki.Kontrol(Ekranlar.ROL_YETKI, IslemTurleri.GUNCELLE))
        {
            return;
        }

        if (gelenId <= 0)
        {
            return;
        }

        Response.Redirect("~/Pages/RolYetki.aspx?id=" + gelenId, false);

        Context.ApplicationInstance.CompleteRequest();
        return;
    }
}
