using System;

namespace BusinessLayer.Entity
{
    public class Kullanicilar : OrtakAlanlar
    {

        #region SABİTLER

        public const string C_Tablo = "dbo.Kullanicilar";

        public const string C_Sp_Ekle = "dbo.SP_Kullanicilar_EKLE";
        public const string C_Sp_Sil = "dbo.SP_Kullanicilar_SIL";
        public const string C_Sp_Guncelle = "dbo.SP_Kullanicilar_GUNCELLE";
        public const string C_Sp_Getir = "dbo.SP_Kullanicilar_GETIR";
        public const string C_Sp_Listele = "dbo.SP_Kullanicilar_LISTELE";
        public const string C_Sp_Giris = "dbo.SP_Kullanicilar_GIRIS";
        public const string C_Sp_SifreKontrol = "dbo.SP_Kullanicilar_SIFREKONTROL";
        public const string C_Sp_SifreGuncelle = "dbo.SP_Kullanicilar_SIFREGUNCELLE";
        public const string C_Sp_KayitVarMi = "dbo.SP_Kullanicilar_KAYITVARMI";
        public const string C_Sp_KullaniciAdiGuncelle = "dbo.SP_Kullanicilar_KULLANICIADIGUNCELLE";
        public const string C_Sp_MaxIdGetır= "dbo.SP_Kullanicilar_MAXIDGETIR";


        public const string C_Sutun_kullanici_adi = "kullanici_adi";
        public const string C_Sutun_sifre = "sifre";
        public const string C_Sutun_ad = "ad";
        public const string C_Sutun_soyad = "soyad";
        public const string C_Sutun_telefon = "telefon";
        public const string C_Sutun_mail = "mail";
        public const string C_Sutun_rol_id = "rol_id";
        public const string C_Sutun_profil_resim = "profil_resim";

        #endregion

        #region NESNELER

        private string kullaniciAdi;
        public string KullaniciAdi
        {
            get
            {
                return kullaniciAdi;
            }
            set
            {
                kullaniciAdi = value;
            }
        }

        private string sifre;
        public string Sifre
        {
            get
            {
                return sifre;
            }
            set
            {
                sifre = value;
            }
        }

        private string ad;
        public string Ad
        {
            get
            {
                return ad;
            }
            set
            {
                ad = value;
            }
        }

        private string soyad;
        public string Soyad
        {
            get
            {
                return soyad;
            }
            set
            {
                soyad = value;
            }
        }

        private string telefon;

        public string Telefon
        {
            get
            {
                return telefon;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    telefon = value;
                }
                else
                {
                    telefon = value.Replace("-", "").Replace(" ", "");
                }
            }
        }

        private string mail;
        public string Mail
        {
            get
            {
                return mail;
            }
            set
            {
                mail = value;
            }
        }

        private int rolId;
        public int RolId
        {
            get
            {
                return rolId;
            }
            set
            {
                rolId = value;
            }
        }

        private string profilResim;
        public string ProfilResim
        {
            get
            {
                return profilResim;
            }
            set
            {
                profilResim = value;
            }
        }

        #endregion

    }
}
