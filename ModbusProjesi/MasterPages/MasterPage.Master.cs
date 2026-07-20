using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ModbusProjesi.MasterPages
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["kullaniciAdSoyad"] != null)
                {
                    lblGirisYapanKullanici.Text = Session["kullaniciAdSoyad"].ToString();

                    if (Session["kullaniciFoto"] != null && !string.IsNullOrEmpty(Session["kullaniciFoto"].ToString()))
                    {
                        string fotoAdi = Session["kullaniciFoto"].ToString();
                        imgSolMenuProfil.ImageUrl = "~/Files/" + fotoAdi;
                    }
                    else
                    {
                        imgSolMenuProfil.ImageUrl = "~/Files/no-image.png";
                    }
                }
                else
                {
                    Response.Redirect("~/Pages/Login.aspx");
                }
            }
        }

        protected void btnCikis_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("~/Pages/Login.aspx"); 
        }
    }
}