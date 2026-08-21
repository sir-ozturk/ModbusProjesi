using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IOrtakMetotlar
{
    bool Ekle();
    bool Sil();
    bool Guncelle();
    void TumunuGetir();
    bool Doldur();
}

