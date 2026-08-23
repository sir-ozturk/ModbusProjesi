using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ucMyGrid : System.Web.UI.UserControl
{
    #region ÖZELLİKLER

    public DataTable Table
    {
        get
        {
            return Session[SessionTabloAdiOlustur()] as DataTable;
        }
        set
        {
            Session[SessionTabloAdiOlustur()] = value;
        }
    }

    public int PageSize
    {
        get
        {
            return grdMyGrid.PageSize;
        }
        set
        {
            grdMyGrid.PageSize = value;
        }
    }

    private string SiralamaAlani
    {
        get
        {
            return ViewState["SiralamaAlani"] == null ? string.Empty : ViewState["SiralamaAlani"].ToString();
        }
        set
        {
            ViewState["SiralamaAlani"] = value;
        }
    }

    private string SiralamaYonu
    {
        get
        {
            return ViewState["SiralamaYonu"] == null ? "ASC" : ViewState["SiralamaYonu"].ToString();
        }
        set
        {
            ViewState["SiralamaYonu"] = value;
        }
    }

    #endregion

    #region ENUM

    public enum FormatTip
    {
        YOK,
        TELEFON
    }

    public enum ButonTip
    {
        GUNCELLE,
        SIL,
        DETAY
    }

    #endregion

    #region EVENT ARGUMENTS

    public class MyGridButonEventArgs : EventArgs
    {
        public int Id { get; set; }
        public ButonTip ButonTip { get; set; }
    }

    public event EventHandler<MyGridButonEventArgs> ButonTiklandi;

    #endregion

    #region SAYFA OLAYLARI

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion

    #region GRID METOTLARI

    public void Doldur(DataTable veriTablosu)
    {
        Table = veriTablosu;
        grdMyGrid.PageIndex = 0;
        GridYenile();
    }

    private void GridYenile()
    {
        if (Table == null)
        {
            return;
        }

        DataTable gosterilecekTablo = Table;
        string aranan = txtArama.Text.Trim().ToLower();

        if (!string.IsNullOrEmpty(aranan))
        {
            DataTable filtreliTablo = Table.Clone();

            foreach (DataRow satir in Table.Rows)
            {
                foreach (object deger in satir.ItemArray)
                {
                    if (deger != null && deger != DBNull.Value && deger.ToString().ToLower().Contains(aranan))
                    {
                        filtreliTablo.ImportRow(satir);
                        break;
                    }
                }
            }

            gosterilecekTablo = filtreliTablo;
        }

        DataView dataView = gosterilecekTablo.DefaultView;

        if (!string.IsNullOrEmpty(SiralamaAlani))
        {
            dataView.Sort = SiralamaAlani + " " + SiralamaYonu;
        }

        grdMyGrid.DataSource = dataView;
        grdMyGrid.DataBind();
        lblKayitSayisi.Text = "Toplam Kayıt: " + gosterilecekTablo.Rows.Count;
    }

    #endregion

    #region KOLON METOTLARI

    public void KolonEkle(string veriAlani, string baslik)
    {
        BoundField kolon = new BoundField();
        kolon.DataField = veriAlani;
        kolon.HeaderText = baslik;
        kolon.SortExpression = veriAlani;
        grdMyGrid.Columns.Add(kolon);
    }

    public void FormatliKolonEkle(string veriAlani, string baslik, FormatTip formatTip)
    {
        TemplateField kolon = new TemplateField();
        kolon.HeaderText = baslik;
        kolon.SortExpression = veriAlani;
        kolon.ItemTemplate = new FormatliAlanTemplate(veriAlani, formatTip);
        grdMyGrid.Columns.Add(kolon);
    }

    public void BirlesikKolonEkle(string baslik, string veriAlani1, string veriAlani2)
    {
        TemplateField kolon = new TemplateField();
        kolon.HeaderText = baslik;
        kolon.SortExpression = veriAlani1;
        kolon.ItemTemplate = new BirlesikAlanTemplate(veriAlani1, veriAlani2);
        grdMyGrid.Columns.Add(kolon);
    }

    public void DurumKolonEkle(string veriAlani, string baslik, string trueMetin, string falseMetin)
    {
        TemplateField kolon = new TemplateField();
        kolon.HeaderText = baslik;
        kolon.SortExpression = veriAlani;
        kolon.ItemTemplate = new DurumAlanTemplate(veriAlani, trueMetin, falseMetin);
        grdMyGrid.Columns.Add(kolon);
    }

    public void ButonEkle(string baslik, string idAlani, params ButonTip[] butonTipleri)
    {
        TemplateField kolon = new TemplateField();
        kolon.HeaderText = baslik;
        kolon.ItemTemplate = new ButonAlanTemplate(idAlani, butonTipleri);
        grdMyGrid.Columns.Add(kolon);
    }

    #endregion

    #region FORMATLI ALAN TEMPLATE

    private class FormatliAlanTemplate : ITemplate
    {
        private string veriAlani;
        private FormatTip formatTip;

        public FormatliAlanTemplate(string _veriAlani, FormatTip _formatTip)
        {
            veriAlani = _veriAlani;
            formatTip = _formatTip;
        }

        public void InstantiateIn(Control container)
        {
            Label label = new Label();

            label.DataBinding += delegate
            {
                GridViewRow satir = (GridViewRow)label.NamingContainer;
                object deger = DataBinder.Eval(satir.DataItem, veriAlani);

                if (deger == null || deger == DBNull.Value)
                {
                    label.Text = string.Empty;
                    return;
                }

                string metin = deger.ToString();

                switch (formatTip)
                {
                    case FormatTip.TELEFON:
                        label.Text = Utility.TelefonFormatla(metin);
                        break;

                    default:
                        label.Text = metin;
                        break;
                }
            };

            container.Controls.Add(label);
        }
    }

    #endregion

    #region BİRLEŞİK ALAN TEMPLATE

    private class BirlesikAlanTemplate : ITemplate
    {
        private string veriAlani1;
        private string veriAlani2;

        public BirlesikAlanTemplate(string _veriAlani1, string _veriAlani2)
        {
            veriAlani1 = _veriAlani1;
            veriAlani2 = _veriAlani2;
        }

        public void InstantiateIn(Control container)
        {
            Label label = new Label();

            label.DataBinding += delegate
            {
                GridViewRow satir = (GridViewRow)label.NamingContainer;
                object deger1 = DataBinder.Eval(satir.DataItem, veriAlani1);
                object deger2 = DataBinder.Eval(satir.DataItem, veriAlani2);
                string metin1 = deger1 == null || deger1 == DBNull.Value ? string.Empty : deger1.ToString();
                string metin2 = deger2 == null || deger2 == DBNull.Value ? string.Empty : deger2.ToString();
                label.Text = (metin1 + " " + metin2).Trim();
            };

            container.Controls.Add(label);
        }
    }

    #endregion

    #region DURUM ALAN TEMPLATE

    private class DurumAlanTemplate : ITemplate
    {
        private string veriAlani;
        private string trueMetin;
        private string falseMetin;

        public DurumAlanTemplate(string _veriAlani, string _trueMetin, string _falseMetin)
        {
            veriAlani = _veriAlani;
            trueMetin = _trueMetin;
            falseMetin = _falseMetin;
        }

        public void InstantiateIn(Control container)
        {
            Label label = new Label();

            label.DataBinding += delegate
            {
                GridViewRow satir = (GridViewRow)label.NamingContainer;
                object deger = DataBinder.Eval(satir.DataItem, veriAlani);

                if (deger == null || deger == DBNull.Value)
                {
                    label.Text = string.Empty;
                    return;
                }

                bool durum = Convert.ToBoolean(deger);

                if (durum)
                {
                    label.Text = trueMetin;
                    label.CssClass = "badge bg-success";
                }
                else
                {
                    label.Text = falseMetin;
                    label.CssClass = "badge bg-danger";
                }
            };

            container.Controls.Add(label);
        }
    }

    #endregion

    #region BUTON ALAN TEMPLATE

    private class ButonAlanTemplate : ITemplate
    {
        private string idAlani;
        private ButonTip[] butonTipleri;

        public ButonAlanTemplate(string _idAlani, ButonTip[] _butonTipleri)
        {
            idAlani = _idAlani;
            butonTipleri = _butonTipleri;
        }

        public void InstantiateIn(Control container)
        {
            Panel panel = new Panel();

            foreach (ButonTip butonTip in butonTipleri)
            {
                LinkButton buton = new LinkButton();

                switch (butonTip)
                {
                    case ButonTip.GUNCELLE:
                        buton.Text = "<i class='fa-solid fa-pen-to-square'></i> Güncelle";
                        buton.CommandName = "GUNCELLE";
                        buton.CssClass = "btn btn-warning btn-sm me-2";
                        break;

                    case ButonTip.SIL:
                        buton.Text = "<i class='fa-solid fa-trash-can'></i> Sil";
                        buton.CommandName = "SIL";
                        buton.CssClass = "btn btn-danger btn-sm me-2";
                        buton.OnClientClick = "silOnayiGoster(this); return false;";
                        break;

                    case ButonTip.DETAY:
                        buton.Text = "<i class='fa-solid fa-eye'></i> Detay";
                        buton.CommandName = "DETAY";
                        buton.CssClass = "btn btn-dark btn-sm me-2";
                        break;
                }

                buton.DataBinding += delegate
                {
                    GridViewRow satir = (GridViewRow)buton.NamingContainer;
                    object idDegeri = DataBinder.Eval(satir.DataItem, idAlani);
                    buton.CommandArgument = idDegeri.ToString();
                };

                panel.Controls.Add(buton);
            }

            container.Controls.Add(panel);
        }
    }

    #endregion

    #region GRID OLAYLARI

    protected void grdMyGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ButonTip butonTip;

        if (!Enum.TryParse(e.CommandName, out butonTip))
        {
            return;
        }

        if (ButonTiklandi == null)
        {
            return;
        }

        MyGridButonEventArgs eventArgs = new MyGridButonEventArgs();
        eventArgs.Id = Convert.ToInt32(e.CommandArgument);
        eventArgs.ButonTip = butonTip;
        ButonTiklandi(this, eventArgs);
    }

    protected void txtArama_TextChanged(object sender, EventArgs e)
    {
        grdMyGrid.PageIndex = 0;
        GridYenile();
    }

    protected void grdMyGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdMyGrid.PageIndex = e.NewPageIndex;
        GridYenile();
    }

    protected void ddlKayitSayisi_SelectedIndexChanged(object sender, EventArgs e)
    {
        int kayitSayisi = Convert.ToInt32(ddlKayitSayisi.SelectedValue);
        grdMyGrid.PageIndex = 0;

        if (kayitSayisi == 0)
        {
            grdMyGrid.AllowPaging = false;
        }
        else
        {
            grdMyGrid.AllowPaging = true;
            PageSize = kayitSayisi;
        }

        GridYenile();
    }

    protected void grdMyGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (SiralamaAlani == e.SortExpression)
        {
            SiralamaYonu = SiralamaYonu == "ASC" ? "DESC" : "ASC";
        }
        else
        {
            SiralamaAlani = e.SortExpression;
            SiralamaYonu = "ASC";
        }

        grdMyGrid.PageIndex = 0;
        GridYenile();
    }

    protected void grdMyGrid_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.Header)
        {
            return;
        }

        foreach (TableCell hucre in e.Row.Cells)
        {
            if (hucre.Controls.Count == 0)
            {
                continue;
            }

            LinkButton linkButton = hucre.Controls[0] as LinkButton;

            if (linkButton == null)
            {
                continue;
            }

            linkButton.CssClass = "grid-siralama-link";

            if (linkButton.CommandArgument == SiralamaAlani)
            {
                linkButton.Text += SiralamaYonu == "ASC" ? " ↑" : " ↓";
            }
            else
            {
                linkButton.Text += " ↕";
            }
        }
    }

    #endregion

    #region YARDIMCI METOTLAR

    private string SessionTabloAdiOlustur()
    {
        return "ucMyGrid_" + ClientID + "_Table";
    }

    #endregion
}