using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Entity;
using BusinessLayer.Interfaces;
using System.Data;

namespace BusinessLayer.Work
{
    public class KullaniciIslemleri
    {
        private VeritabaniIslemleri veritabaniIslemleri;

        public KullaniciIslemleri(VeritabaniIslemleri veritabaniIslemleri)
        {
            this.veritabaniIslemleri = veritabaniIslemleri;
        }

        #region METOTLAR
        public bool Ekle(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_Ekle);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_kullanici_adi, kullanicilar.KullaniciAdi);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_ad, kullanicilar.Ad);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_soyad, kullanicilar.Soyad);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_sifre, kullanicilar.Sifre);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_telefon, kullanicilar.Telefon);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_mail, kullanicilar.Mail);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_rol_id, kullanicilar.RolId);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_aktif_mi, kullanicilar.AktifMi);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_profil_resim, kullanicilar.ProfilResim);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_ekleyen_id, kullanicilar.EkleyenId);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_ekleyen_ip, kullanicilar.EkleyenIp);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_guncelleyen_id, kullanicilar.GuncelleyenId);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_guncelleyen_ip, kullanicilar.GuncelleyenIp);

            return veritabaniIslemleri.Calistir();
        }

        public bool Guncelle(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_Guncelle);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_id, kullanicilar.Id);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_kullanici_adi, kullanicilar.KullaniciAdi);

            if (string.IsNullOrEmpty(kullanicilar.Sifre))
            {
                veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_sifre, null);
            }
            else
            {
                veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_sifre, kullanicilar.Sifre);
            }

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_ad, kullanicilar.Ad);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_soyad, kullanicilar.Soyad);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_telefon, kullanicilar.Telefon);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_mail, kullanicilar.Mail);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_rol_id, kullanicilar.RolId);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_aktif_mi, kullanicilar.AktifMi);

            if (string.IsNullOrEmpty(kullanicilar.ProfilResim))
            {
                veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_profil_resim, null);
            }
            else
            {
                veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_profil_resim, kullanicilar.ProfilResim);
            }

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_guncelleyen_id, kullanicilar.GuncelleyenId);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_guncelleyen_ip, kullanicilar.GuncelleyenIp);

            return veritabaniIslemleri.Calistir();
        }

        public bool Sil(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_Sil);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_id, kullanicilar.Id);

            return veritabaniIslemleri.Calistir();
        }

        public bool Getir(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_Getir);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_id, kullanicilar.Id);
            DataRow satir = veritabaniIslemleri.SatirGetir();

            if (satir == null)
            {
                return false;
            }

            kullanicilar.Id = Convert.ToInt32(satir[Kullanicilar.C_Sutun_id]);
            kullanicilar.KullaniciAdi = satir[Kullanicilar.C_Sutun_kullanici_adi].ToString();
            kullanicilar.Sifre = satir[Kullanicilar.C_Sutun_sifre].ToString();
            kullanicilar.Ad = satir[Kullanicilar.C_Sutun_ad].ToString();
            kullanicilar.Soyad = satir[Kullanicilar.C_Sutun_soyad].ToString();
            kullanicilar.Telefon = satir[Kullanicilar.C_Sutun_telefon].ToString();
            kullanicilar.Mail = satir[Kullanicilar.C_Sutun_mail].ToString();
            kullanicilar.RolId = Convert.ToInt32(satir[Kullanicilar.C_Sutun_rol_id]);
            kullanicilar.AktifMi = Convert.ToBoolean(satir[Kullanicilar.C_Sutun_aktif_mi]);
            kullanicilar.ProfilResim = satir[Kullanicilar.C_Sutun_profil_resim].ToString();

            return true;
        }

        public DataTable Listele()
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_Listele);
            return veritabaniIslemleri.TabloGetir();
        }

        public bool Giris(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_Giris);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_kullanici_adi, kullanicilar.KullaniciAdi);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_sifre, kullanicilar.Sifre);

            DataRow satir = veritabaniIslemleri.SatirGetir();
            if (satir == null)
            {
                return false;
            }

            kullanicilar.Id = Convert.ToInt32(satir[Kullanicilar.C_Sutun_id]);
            kullanicilar.Ad = satir[Kullanicilar.C_Sutun_ad].ToString();
            kullanicilar.Soyad = satir[Kullanicilar.C_Sutun_soyad].ToString();
            kullanicilar.AktifMi = Convert.ToBoolean(satir[Kullanicilar.C_Sutun_aktif_mi]);
            kullanicilar.ProfilResim = satir[Kullanicilar.C_Sutun_profil_resim].ToString();

            return true;

        }

        public bool SifreKontrol(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_SifreKontrol);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_kullanici_adi, kullanicilar.KullaniciAdi);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_mail, kullanicilar.Mail);

            DataRow satir = veritabaniIslemleri.SatirGetir();
            if (satir == null)
            {
                return false;
            }

            kullanicilar.Ad = satir[Kullanicilar.C_Sutun_ad].ToString();
            kullanicilar.Id = Convert.ToInt32(satir[Kullanicilar.C_Sutun_id]);

            return true;
        }

        public bool SifreGuncelle(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_SifreGuncelle);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_kullanici_adi, kullanicilar.KullaniciAdi);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_sifre, kullanicilar.Sifre);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_mail, kullanicilar.Mail);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_guncelleyen_id, kullanicilar.GuncelleyenId);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_guncelleyen_ip, kullanicilar.GuncelleyenIp);

            return veritabaniIslemleri.Calistir();
        }

        public bool KayitVarMi(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_KayitVarMi);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_id, kullanicilar.Id);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_telefon, kullanicilar.Telefon);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_mail, kullanicilar.Mail);

            object sonuc = veritabaniIslemleri.DegerGetir();
            int kayitSayisi = Convert.ToInt32(sonuc);

            return kayitSayisi > 0;
        }

        public bool KullaniciAdiGuncelle(Kullanicilar kullanicilar)
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_KullaniciAdiGuncelle);

            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_id, kullanicilar.Id);
            veritabaniIslemleri.ParametreEkle(Kullanicilar.C_Sutun_kullanici_adi, kullanicilar.KullaniciAdi);

            return veritabaniIslemleri.Calistir();
        }

        public int MaxIdGetir()
        {
            veritabaniIslemleri.ProsedurSec(Kullanicilar.C_Sp_MaxIdGetır);
            object sonuc = veritabaniIslemleri.DegerGetir();
            return Convert.ToInt32(sonuc);
        }

        #endregion
    }
}
