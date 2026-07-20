using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ModbusProjesi.AppCode;

namespace ModbusProjesi.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGiris_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = sqlBaglanti.Baglanti())
            {
                string sorgu = "Select * From Kullanicilar Where kullanici_adi=@p1 And sifre=@p2 And aktiflik_durumu=1";

                using (SqlCommand sqlCommand = new SqlCommand(sorgu, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@p1", txtKullaniciAdi.Text.Trim());
                    sqlCommand.Parameters.AddWithValue("@p2", txtSifre.Text.Trim());

                    try
                    {
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.Read())
                            {
                                bool aktiflikDurumu = Convert.ToBoolean(sqlDataReader["aktiflik_durumu"]);

                                if (aktiflikDurumu == true)
                                {
                                    Session["kullaniciAdSoyad"] = sqlDataReader["ad"].ToString() + " " + sqlDataReader["soyad"].ToString();
                                    Session["kullaniciFoto"] = sqlDataReader["profil_resim"].ToString();
                                    Response.Redirect("~/Default.aspx");
                                }
                                else
                                {
                                    Response.Write("<script>alert('Kullanıcı kodu veya şifre hatalı!');</script>");
                                }
                            }
                            else
                            {
                                Response.Write("<script>alert('Bu kullanıcı aktif değildir, giriş yapılamaz!');</script>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('Giriş Hatası: " + ex.Message + "');</script>");
                    }
                }
            }
        }
    }
}