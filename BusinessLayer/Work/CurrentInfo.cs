using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CurrentInfo
{
    private int kullaniciId;
    public int KullaniciId
    {
        get { return kullaniciId; }
        set { kullaniciId = value; }
    }

    private string kullaniciAdi;
    public string KullaniciAdi
    {
        get { return kullaniciAdi; }
        set { kullaniciAdi = value; }
    }

    private string ad;
    public string Ad
    {
        get { return ad; }
        set { ad = value; }
    }

    private string soyad;
    public string Soyad
    {
        get { return soyad; }
        set { soyad = value; }
    }

    private int rolId;
    public int RolId
    {
        get { return rolId; }
        set { rolId = value; }
    }

    private string ip;
    public string Ip
    {
        get { return ip; }
        set { ip = value; }
    }

    private bool loginYapildiMi;
    public bool LoginYapildiMi
    {
        get { return loginYapildiMi; }
        set { loginYapildiMi = value; }
    }

    private string profilResim;
    public string ProfilResim
    {
        get { return profilResim; }
        set { profilResim = value; }
    }

    private string kullaniciEmail;
    public string KullaniciEmail
    {
        get { return kullaniciEmail; }
        set { kullaniciEmail = value; }
    }

    public CurrentInfo()
    {
    }
}

