using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ModbusProjesi.AppCode
{
    public class DosyaIslemleri
    {
        public string ResimKaydet(HttpPostedFile dosya)
        {
            string uzanti = Path.GetExtension(dosya.FileName).ToLower();

            string yeniDosyaAdi = System.Guid.NewGuid().ToString() + uzanti;

            string yol = HttpContext.Current.Server.MapPath("~/Files/") + yeniDosyaAdi;

            dosya.SaveAs(yol);

            return yeniDosyaAdi;
        }

        public void ResimSil(string dosyaAdi)
        {
            if (string.IsNullOrEmpty(dosyaAdi))
                return;

            string yol = HttpContext.Current.Server.MapPath("~/Files/") + dosyaAdi;

            if (File.Exists(yol))
            {
                File.Delete(yol);
            }
        }
    }
}