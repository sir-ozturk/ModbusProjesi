using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace ModbusProjesi.AppCode
{
    public class Roller:OrtakAlanlar
    {
        private VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

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

        #region METOTLAR

        public DataTable Listele()
        {
            try
            {
                veritabaniIslemleri.Baslat(C_Sp_Listele);

                DataTable rollerTablosu = veritabaniIslemleri.TabloGetir();

                return rollerTablosu;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        #endregion
    }
}