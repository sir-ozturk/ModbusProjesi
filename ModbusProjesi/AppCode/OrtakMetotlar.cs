using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModbusProjesi.AppCode
{
    public class OrtakMetotlar:OrtakAlanlar
    {
        public virtual bool Ekle() 
        {
            return true;
        }
        public virtual bool Sil()
        {
            return true;
        }
        public virtual bool Guncelle()
        {
            return true;
        }
    }
}