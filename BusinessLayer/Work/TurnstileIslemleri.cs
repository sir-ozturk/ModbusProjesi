using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

public class TurnstileIslemleri
{
    #region SABİTLER

    private const string C_DogrulamaAdresi = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
    private const string C_SecretKey = "TurnstileSecretKey";

    #endregion

    public TurnstileIslemleri()
    {
    }

    //Turnstile tarafından oluşturulan token'ı, Secret Key ve kullanıcı IP'siyle birlikte Cloudflare Siteverify servisine gönderir.
    //Cloudflare'dan gelen JSON cevabındaki success değerini kontrol ederek doğrulama başarılıysa true, başarısızsa false döndürür.
    public bool Dogrula(string token, string ip)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        string secretKey = ConfigurationManager.AppSettings[C_SecretKey];

        using (WebClient webClient = new WebClient())
        {
            NameValueCollection veriler = new NameValueCollection();

            veriler["secret"] = secretKey;
            veriler["response"] = token;
            veriler["remoteip"] = ip;

            byte[] cevap = webClient.UploadValues(C_DogrulamaAdresi, "POST", veriler);

            string json = Encoding.UTF8.GetString(cevap);

            JavaScriptSerializer serializer = new JavaScriptSerializer(); //Bu nesnenin görevi JSON ile C# arasında dönüşüm yapmak.

            Dictionary<string, object> sonuc = serializer.Deserialize<Dictionary<string, object>>(json);

            if (sonuc.ContainsKey("success"))
            {
                return Convert.ToBoolean(sonuc["success"]);
            }

            return false;
        }
    }
}

