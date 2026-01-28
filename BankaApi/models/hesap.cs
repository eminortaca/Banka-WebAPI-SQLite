using System;
using System.Collections.Generic; // List ve IReadOnlyList için gerekli
using System.ComponentModel.DataAnnotations;

namespace BankaApi.Models
{
    // İşlem detaylarını tutan yapı - Class dışında olması daha düzenli durur
    public struct IslemKaydi 
    {
        public DateTime Tarih { get; init; }
        public string Tur { get; init; }
        public decimal Miktar { get; init; }
    }

    public class Hesap
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TamAd => $"{Ad} {Soyad}";
        public decimal Bakiye { get; private set; }
        public decimal GunlukLimit { get; set; }

        // 1. ADIM: Geçmişi tutacak gizli liste (Encapsulation)
        private readonly List<IslemKaydi> _gecmis = new List<IslemKaydi>();
        
        // 2. ADIM: Listeyi dışarıya sadece okunabilir olarak açıyoruz
        public IReadOnlyList<IslemKaydi> Gecmis => _gecmis.AsReadOnly();

        public Hesap() { }

        public Hesap(string ad, string soyad, decimal baslangicBakiyesi = 0, decimal gunlukLimit = 1000)
        {
            Ad = ad;
            Soyad = soyad;
            Bakiye = baslangicBakiyesi;
            GunlukLimit = gunlukLimit;
        }

        public void ParaYatir(decimal miktar)
        {
            if (miktar <= 0) return;

            Bakiye += miktar;
            
            // 3. ADIM: Yatırma işlemini geçmişe kaydet
            _gecmis.Add(new IslemKaydi { Tarih = DateTime.Now, Tur = "Yatırma", Miktar = miktar });
        }

        public void ParaCek(decimal miktar)
        {
            if (miktar > GunlukLimit || miktar > Bakiye)
            {
                Console.WriteLine("Hata: İşlem gerçekleştirilemedi.");
                return;
            }

            Bakiye -= miktar;

            // 4. ADIM: Çekme işlemini geçmişe kaydet
            _gecmis.Add(new IslemKaydi { Tarih = DateTime.Now, Tur = "Para Çekme", Miktar = miktar });
        }

        // 5. ADIM: Özeti ekrana basan metot
        public void GecmisiYazdir()
        {
            Console.WriteLine($"\n--- {TamAd} (ID: {Id}) Hesap Özeti ---");
            if (_gecmis.Count == 0) Console.WriteLine("Henüz bir işlem bulunmuyor.");
            
            foreach (var kayit in _gecmis)
            {
                Console.WriteLine($"{kayit.Tarih:G} | {kayit.Tur}: {kayit.Miktar:C2}");
            }
            Console.WriteLine($"Güncel Toplam Bakiye: {Bakiye:C2}\n");
        }
    }
}