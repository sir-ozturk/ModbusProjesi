using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using BusinessLayer.Entity;

namespace BusinessLayer.Work
{
    public class RolIslemleri
    {
        #region METOTLAR

        private VeritabaniIslemleri veritabaniIslemleri;

        public RolIslemleri(VeritabaniIslemleri veritabaniIslemleri)
        {
            this.veritabaniIslemleri = veritabaniIslemleri;
        }

        public DataTable Listele()
        {
            veritabaniIslemleri.ProsedurSec(Roller.C_Sp_Listele);

            return veritabaniIslemleri.TabloGetir();
        }

        #endregion
    }
}
