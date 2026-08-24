using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


public class Utility
{
    #region METOTLAR

    public static string TelefonFormatla(string telefon)
    {
        if (string.IsNullOrEmpty(telefon))
        {
            return string.Empty;
        }

        if (telefon.Length != 10)
        {
            return telefon;
        }

        return telefon.Substring(0, 3) + "-" +
               telefon.Substring(3, 3) + "-" +
               telefon.Substring(6, 2) + "-" +
               telefon.Substring(8, 2);
    }

    public static string RastgeleSifreOlustur()
    {
        Random random = new Random();

        string[] harfler = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        string[] karakterler = { "!", "?", "*", "-", "_", "+", "#", "$" };

        string rastgeleHarf1 = harfler[random.Next(0, harfler.Length)];
        string rastgeleHarf2 = harfler[random.Next(0, harfler.Length)];
        string rastgeleHarf3 = harfler[random.Next(0, harfler.Length)];
        string rastgeleHarf4 = harfler[random.Next(0, harfler.Length)];
        string rastgeleKarakter = karakterler[random.Next(0, karakterler.Length)];
        int rastgeleSayi = random.Next(1000, 999999);

        return rastgeleHarf1 + rastgeleHarf2 + rastgeleSayi + rastgeleHarf3 + rastgeleHarf4 + rastgeleKarakter;
    }

    public static string IpNoGetir()
    {
        try
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');

                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        catch
        {
            try
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                return "0.0.0.0";
            }
        }
    }

    #endregion
}

