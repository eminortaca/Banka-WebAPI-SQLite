using Microsoft.AspNetCore.Mvc;
using BankaApi.Models;
using Microsoft.EntityFrameworkCore; // Include için gerekli
using System.Linq;
using System;

namespace BankaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankaController : ControllerBase
    {
        private readonly BankaDbContext _context;

        public BankaController(BankaDbContext context)
        {
            _context = context;
        }

        // 1. Hesap Bilgilerini ve Geçmişi Getir
        [HttpGet]
        public IActionResult GetHesap()
        {
            // Veritabanından hesabı ve ona bağlı işlemleri çekiyoruz
            var hesap = _context.Hesaplar
                                .Include(h => h.Islemler) 
                                .FirstOrDefault();

            // Eğer hesap yoksa (ilk açılışta) otomatik oluştur
            if (hesap == null)
            {
                // Constructor içinde Id = Guid.NewGuid() zaten çalışıyor
                hesap = new Hesap("Emin", "Ortaca", 0); 
                _context.Hesaplar.Add(hesap);
                _context.SaveChanges();
            }

            // Sonsuz döngüye girmesin diye temiz bir çıktı veriyoruz
            return Ok(new 
            {
                hesap.Ad,
                hesap.Soyad,
                hesap.Bakiye,
                // İşlemleri tarihe göre sırala (En yeni en üstte)
                IslemGecmisi = hesap.Islemler.OrderByDescending(i => i.Tarih) 
            });
        }

        // 2. Para Yatır
        [HttpPost("yatir")]
        public IActionResult ParaYatir([FromBody] decimal miktar)
        {
            var hesap = _context.Hesaplar.FirstOrDefault();
            if (hesap == null) return NotFound("Hesap bulunamadı.");

            // A) Bakiyeyi artır
            hesap.Bakiye += miktar;

            // B) İşlem fişini kes (GUID BURADA ÖNEMLİ)
            var yeniIslem = new Islem
            {
                Id = Guid.NewGuid(), // <--- BAK BURASI YENİ: ID'yi biz üretiyoruz!
                Aciklama = "Para Yatırma",
                Tutar = miktar,
                Tarih = DateTime.Now,
                HesapId = hesap.Id // Hesabın GUID'ini buraya bağlıyoruz
            };

            _context.Islemler.Add(yeniIslem);
            _context.SaveChanges();

            return Ok($"{miktar} TL yatırıldı. Yeni Bakiye: {hesap.Bakiye}");
        }

        // 3. Para Çek
        [HttpPost("cek")]
        public IActionResult ParaCek([FromBody] decimal miktar)
        {
            var hesap = _context.Hesaplar.FirstOrDefault();
            if (hesap == null) return NotFound("Hesap bulunamadı.");

            if (hesap.Bakiye < miktar)
                return BadRequest("Yetersiz bakiye!");

            // A) Bakiyeyi düş
            hesap.Bakiye -= miktar;

            // B) İşlem fişini kes
            var yeniIslem = new Islem
            {
                Id = Guid.NewGuid(), // <--- BURASI DA YENİ
                Aciklama = "Para Çekme",
                Tutar = -miktar, // Eksi olarak kaydediyoruz
                Tarih = DateTime.Now,
                HesapId = hesap.Id
            };

            _context.Islemler.Add(yeniIslem);
            _context.SaveChanges();

            return Ok($"{miktar} TL çekildi. Kalan Bakiye: {hesap.Bakiye}");
        }
    }
}