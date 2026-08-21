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
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (currentInfo == null || currentInfo.LoginYapildiMi == false)
            {
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!Page.IsPostBack)
            {
                    lblGirisYapanKullanici.Text = currentInfo.Ad + " " + currentInfo.Soyad;

                    if (!string.IsNullOrEmpty(currentInfo.ProfilResim))
                    {
                        imgSolMenuProfil.ImageUrl = "~/Files/" + currentInfo.ProfilResim;
                    }
                    else
                    {
                        imgSolMenuProfil.ImageUrl = "~/Files/no-image.png";
                    }        
            }
        }

        protected void btnCikis_Click(object sender, EventArgs e)
        {
            Sessionlar sessionlar = new Sessionlar();

            sessionlar.Current._CurrentInfo = null;

            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Pages/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}