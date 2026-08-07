using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ModbusProjesi.AppCode
{
    public class Kullanicilar:OrtakMetotlar
    {
        private VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

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

        #region METOTLAR

        public override bool Ekle()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Ekle);
                
                veritabaniIslemleri.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ad, Ad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_soyad, Soyad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);
                veritabaniIslemleri.ParametreEkle(C_Sutun_telefon, Telefon);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_rol_id, RolId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_profil_resim, ProfilResim);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

                return veritabaniIslemleri.Calistir();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public override bool Guncelle()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Guncelle);

                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);
                veritabaniIslemleri.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);

                if (string.IsNullOrEmpty(Sifre))
                {
                    veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, null);
                }
                else
                {
                    veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);
                }

                veritabaniIslemleri.ParametreEkle(C_Sutun_ad, Ad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_soyad, Soyad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_telefon, Telefon);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_rol_id, RolId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_aktif_mi, AktifMi);

                if (string.IsNullOrEmpty(ProfilResim))
                {
                    veritabaniIslemleri.ParametreEkle(C_Sutun_profil_resim, null);
                }
                else
                {
                    veritabaniIslemleri.ParametreEkle(C_Sutun_profil_resim, ProfilResim);
                }

                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

                return veritabaniIslemleri.Calistir();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public override bool Sil()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Sil);

                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);

                return veritabaniIslemleri.Calistir();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool Getir()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Getir);

                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);
                DataRow satir = veritabaniIslemleri.SatirGetir();
                
                if (satir==null)
                {
                    return false;
                }
                
                Id = Convert.ToInt32(satir[C_Sutun_id]);
                KullaniciAdi = satir[C_Sutun_kullanici_adi].ToString();
                Sifre = satir[C_Sutun_sifre].ToString();
                Ad = satir[C_Sutun_ad].ToString();
                Soyad = satir[C_Sutun_soyad].ToString();
                Telefon = satir[C_Sutun_telefon].ToString();
                Mail = satir[C_Sutun_mail].ToString();
                RolId = Convert.ToInt32(satir[C_Sutun_rol_id]);
                AktifMi = Convert.ToBoolean(satir[C_Sutun_aktif_mi]);
                ProfilResim = satir[C_Sutun_profil_resim].ToString();

                return true;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public DataTable Listele()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Listele);
                return veritabaniIslemleri.TabloGetir();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool Giris()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Giris);

                veritabaniIslemleri.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);

                DataRow satir = veritabaniIslemleri.SatirGetir();
                if (satir == null)
                {
                    return false;
                }

                Id = Convert.ToInt32(satir[C_Sutun_id]);
                Ad = satir[C_Sutun_ad].ToString();
                Soyad = satir[C_Sutun_soyad].ToString();
                AktifMi = Convert.ToBoolean(satir[C_Sutun_aktif_mi]);
                ProfilResim = satir[C_Sutun_profil_resim].ToString();

                return true;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool SifreKontrol()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_SifreKontrol);

                veritabaniIslemleri.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);

                DataRow satir = veritabaniIslemleri.SatirGetir();
                if (satir == null)
                {
                    return false;
                }

                Ad = satir[C_Sutun_ad].ToString();
                Id = Convert.ToInt32(satir[C_Sutun_id]);

                return true;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool SifreGuncelle()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_SifreGuncelle);

                veritabaniIslemleri.ParametreEkle(C_Sutun_kullanici_adi, KullaniciAdi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);

                return veritabaniIslemleri.Calistir();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool KayitVarMi()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_KayitVarMi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_id,Id);
                veritabaniIslemleri.ParametreEkle(C_Sutun_kullanici_adi,KullaniciAdi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_telefon,Telefon);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail,Mail);

                object sonuc = veritabaniIslemleri.DegerGetir();
                int kayitSayisi = Convert.ToInt32(sonuc);

                if (kayitSayisi > 0)
                {
                    return true;
                }

                return false;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        #endregion
    }
}