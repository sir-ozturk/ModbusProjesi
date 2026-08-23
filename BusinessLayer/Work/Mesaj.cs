using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;


public class Mesaj
{
    public enum MesajTurleri
    {
        SUCCESS,
        FAIL,
        INFO,
        WARNING
    }

    public static void Ver(string mesajMetni, MesajTurleri mesajTuru, MasterPage master)
    {
        try
        {
            Label lbl_success = (Label)master.FindControl("lbl_success");
            Label lbl_warning = (Label)master.FindControl("lbl_warning");
            Label lbl_info = (Label)master.FindControl("lbl_info");
            Label lbl_error = (Label)master.FindControl("lbl_error");
            lbl_error.Text = "";
            lbl_info.Text = "";
            lbl_success.Text = "";
            lbl_warning.Text = "";
            if (mesajTuru == MesajTurleri.SUCCESS)
            {
                lbl_success.Text = mesajMetni;
            }
            if (mesajTuru == MesajTurleri.FAIL)
            {
                lbl_error.Text = mesajMetni;
            }
            if (mesajTuru == MesajTurleri.INFO)
            {
                lbl_info.Text = mesajMetni;
            }
            if (mesajTuru == MesajTurleri.WARNING)
            {
                lbl_warning.Text = mesajMetni;
            }
        }
        catch
        {

        }
    }

    public static void Ver(string mesajMetni, MesajTurleri mesajTuru, Page page)
    {
        try
        {
            Label lbl_success = (Label)page.FindControl("lbl_success");
            Label lbl_warning = (Label)page.FindControl("lbl_warning");
            Label lbl_info = (Label)page.FindControl("lbl_info");
            Label lbl_error = (Label)page.FindControl("lbl_error");

            lbl_error.Text = "";
            lbl_info.Text = "";
            lbl_success.Text = "";
            lbl_warning.Text = "";

            if (mesajTuru == MesajTurleri.SUCCESS)
            {
                lbl_success.Text = mesajMetni;
            }

            if (mesajTuru == MesajTurleri.FAIL)
            {
                lbl_error.Text = mesajMetni;
            }

            if (mesajTuru == MesajTurleri.INFO)
            {
                lbl_info.Text = mesajMetni;
            }

            if (mesajTuru == MesajTurleri.WARNING)
            {
                lbl_warning.Text = mesajMetni;
            }
        }
        catch
        {

        }
    }
}

