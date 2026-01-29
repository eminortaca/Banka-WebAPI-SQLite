using System;
using System.Collections.Generic;

namespace BankaApi.Models
{
    public class Hesap
    {
        public Guid Id { get; set; }  // int yerine Guid yaptık
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public decimal Bakiye { get; set; } // public set (açık)

        public List<Islem> Islemler { get; set; } = new List<Islem>();

        public Hesap(string ad, string soyad, decimal baslangicBakiyesi)
        {
            Id = Guid.NewGuid(); // ÖNEMLİ: Yeni oluşurken rastgele bir kimlik veriyoruz!
            Ad = ad;
            Soyad = soyad;
            Bakiye = baslangicBakiyesi;
        }

        public Hesap() 
        { 
             // Boş constructor çalışırsa da ID oluşsun
             Id = Guid.NewGuid(); 
        } 
    }
}