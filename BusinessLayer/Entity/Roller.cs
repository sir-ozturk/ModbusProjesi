using System;
using System.Data;
using BusinessLayer.Work;

namespace BusinessLayer.Entity
{
    public class Roller : OrtakAlanlar
    {
        #region SABİTLER

        public const string C_Sp_Listele = "dbo.SP_Roller_LISTELE";
        public const string C_Sutun_rol_adi = "rol_adi";
        public const string C_Sutun_aciklama = "aciklama";

        #endregion

        #region NESNELER

        private string rolAdi;
        public string RolAdi
        {
            get
            {
                return rolAdi;
            }
            set
            {
                rolAdi = value;
            }
        }

        private string aciklama;
        public string Aciklama
        {
            get
            {
                return aciklama;
            }
            set
            {
                aciklama = value;
            }
        }

        #endregion
    }

}
