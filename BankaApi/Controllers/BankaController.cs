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
        // 4. Havale / Para Transferi (ACID Transaction içerir)
        [HttpPost("transfer")]
        public IActionResult ParaTransferi([FromBody] Dtos.ParaTransferiDto istek)
        {
            // 1. Transaction Başlat (Ya Hep Ya Hiç Prensibi)
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Gönderen ve Alıcıyı Bul
                    var gonderen = _context.Hesaplar.FirstOrDefault(h => h.Id == istek.GonderenHesapId);
                    var alici = _context.Hesaplar.FirstOrDefault(h => h.Id == istek.AliciHesapId);

                    // Validasyonlar (Kontroller)
                    if (gonderen == null) return NotFound("Gönderen hesap bulunamadı.");
                    if (alici == null) return NotFound("Alıcı hesap bulunamadı.");
                    if (gonderen.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye!");

                    // A) Gönderenden Para Düş
                    gonderen.Bakiye -= istek.Tutar;
                    _context.Islemler.Add(new Islem
                    {
                        Id = Guid.NewGuid(),
                        Aciklama = $"Transfer Gönderilen -> {alici.Ad} {alici.Soyad}",
                        Tutar = -istek.Tutar,
                        Tarih = DateTime.Now,
                        HesapId = gonderen.Id
                    });

                    // B) Alıcıya Para Ekle
                    alici.Bakiye += istek.Tutar;
                    _context.Islemler.Add(new Islem
                    {
                        Id = Guid.NewGuid(),
                        Aciklama = $"Transfer Gelen <- {gonderen.Ad} {gonderen.Soyad}",
                        Tutar = istek.Tutar,
                        Tarih = DateTime.Now,
                        HesapId = alici.Id
                    });

                    // Değişiklikleri Veritabanına Yansıt
                    _context.SaveChanges();

                    // Hata çıkmadıysa işlemi onayla ve kalıcı hale getir
                    transaction.Commit(); 

                    return Ok($"Transfer başarılı. {gonderen.Ad} -> {alici.Ad}: {istek.Tutar} TL");
                }
                catch (Exception ex)
                {
                    // Herhangi bir hata olursa yapılan TÜM işlemleri geri al (Para iade edilir)
                    transaction.Rollback();
                    return StatusCode(500, "Transfer sırasında bir hata oluştu: " + ex.Message);
                }
            }
        }
    }
}