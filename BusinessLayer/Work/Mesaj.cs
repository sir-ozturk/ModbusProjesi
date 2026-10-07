using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class Mesaj
{
    public enum MesajTurleri { SUCCESS, FAIL, INFO, WARNING }

    public static void Ver(string metin, MesajTurleri tur, MasterPage master)
    {
        Goster(metin, tur, master, master.Page);
    }

    public static void Ver(string metin, MesajTurleri tur, Page page)
    {
        Goster(metin, tur, page, page);
    }

    private static void Goster(string metin, MesajTurleri tur, Control kok, Page page)
    {
        string[] ids = { "lbl_success", "lbl_error", "lbl_info", "lbl_warning" };
        string secilen = ids[(int)tur];
        foreach (string id in ids)
        {
            Label etiket = kok.FindControl(id) as Label;
            if (etiket != null) etiket.Text = id == secilen ? HttpUtility.HtmlEncode(metin) : "";
        }
        ScriptManager manager = ScriptManager.GetCurrent(page);
        if (manager != null && manager.IsInAsyncPostBack)
        {
            string kod = "window.modbusMesajiGoster('" + HttpUtility.JavaScriptStringEncode(metin)
                + "', '" + tur.ToString() + "', 5000);";
            ScriptManager.RegisterStartupScript(page, page.GetType(), "ModbusMesaji", kod, true);
        }
    }
}