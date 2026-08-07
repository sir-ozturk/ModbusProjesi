using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModbusProjesi
{
    public class OrtakAlanlar
    {
        #region SABİTLER

        public const string C_Sutun_id = "id";
        public const string C_Sutun_ekleyen_id = "ekleyen_id";
        public const string C_Sutun_ekleyen_ip = "ekleyen_ip";
        public const string C_Sutun_eklenme_tarih = "eklenme_tarih";
        public const string C_Sutun_guncelleyen_id = "guncelleyen_id";
        public const string C_Sutun_guncelleyen_ip = "guncelleyen_ip";
        public const string C_Sutun_guncellenme_tarih = "guncellenme_tarih";
        public const string C_Sutun_aktif_mi = "aktif_mi";

        #endregion

        #region NESNELER

        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        private int ekleyenId;
        public int EkleyenId
        {
            get
            {
                return ekleyenId;
            }
            set
            {
                ekleyenId = value;
            }
        }

        private string ekleyenIp;
        public string EkleyenIp
        {
            get
            {
                return ekleyenIp;
            }
            set
            {
                ekleyenIp = value;
            }
        }

        private DateTime eklenmeTarih;
        public DateTime EklenmeTarih
        {
            get
            {
                return eklenmeTarih;
            }
            set
            {
                eklenmeTarih = value;
            }
        }

        private int guncelleyenId;
        public int GuncelleyenId
        {
            get
            {
                return guncelleyenId;
            }
            set
            {
                guncelleyenId = value;
            }
        }

        private string guncelleyenIp;
        public string GuncelleyenIp
        {
            get
            {
                return guncelleyenIp;
            }
            set
            {
                guncelleyenIp = value;
            }
        }

        private DateTime guncellenmeTarih;
        public DateTime GuncellenmeTarih
        {
            get
            {
                return guncellenmeTarih;
            }
            set
            {
                guncellenmeTarih = value;
            }
        }


        private bool aktifMi;
        public bool AktifMi
        {
            get
            {
                return aktifMi;
            }
            set
            {
                aktifMi = value;
            }
        }
        #endregion
    }
}