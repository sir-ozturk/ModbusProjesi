using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IslemYetki
{
    public static bool Kontrol(Ekranlar ekran, IslemTurleri islemTuru)
    {
        Sessionlar sessionlar = new Sessionlar();
        CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

        if (currentInfo == null || currentInfo.LoginYapildiMi == false)
        {
            return false;
        }

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

            RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);

            return rolYetkiler.YetkiVarmi(currentInfo.RolId, ekran.ToString(), islemTuru.ToString());
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}

