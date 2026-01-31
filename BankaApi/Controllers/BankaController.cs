using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankaApi.Models;
using BankaApi.Dtos;
using System.Security.Claims;

namespace BankaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece giriş yapanlar kullanabilir
    public class BankaController : ControllerBase
    {
        private readonly BankaDbContext _context;

        public BankaController(BankaDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idClaim);
        }

        // 1. Bilgileri ve SON İŞLEMLERİ Getir
        [HttpGet]
        public IActionResult BilgileriGetir()
        {
            var userId = GetUserId();
            var kullanici = _context.Kullanicilar.Find(userId);
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);

            if (hesap == null) return NotFound("Hesap bulunamadı.");

            // ✅ Geçmiş işlemleri tarihe göre tersten sıralayıp (en yeni en üstte) çekiyoruz
            var sonIslemler = _context.Islemler
                                      .Where(i => i.KullaniciId == userId)
                                      .OrderByDescending(i => i.Tarih)
                                      .Take(10) // Sadece son 10 işlem
                                      .ToList();

            return Ok(new { 
                ad = kullanici.Ad,
                soyad = kullanici.Soyad,
                hesapNo = hesap.HesapNo,
                bakiye = hesap.Bakiye,
                gecmis = sonIslemler // Frontend'e listeyi yolluyoruz
            });
        }

        // 2. Para Yatır (+Kayıt)
        [HttpPost("yatir")]
        public IActionResult ParaYatir([FromBody] BakiyeIslemDto istek)
        {
            var userId = GetUserId();
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);

            if (istek.Tutar <= 0) return BadRequest("Tutar 0'dan büyük olmalı.");

            hesap.Bakiye += istek.Tutar;

            // ✅ Kayıt Tut
            _context.Islemler.Add(new Islem {
                KullaniciId = userId,
                IslemTuru = "Para Yatırma",
                Tutar = istek.Tutar,
                Aciklama = "ATM/Online Yatırma"
            });

            _context.SaveChanges();
            return Ok(new { mesaj = $"{istek.Tutar} TL yatırıldı." });
        }

        // 3. Para Çek (+Kayıt)
        [HttpPost("cek")]
        public IActionResult ParaCek([FromBody] BakiyeIslemDto istek)
        {
            var userId = GetUserId();
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);

            if (hesap.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye!");

            hesap.Bakiye -= istek.Tutar;

            // ✅ Kayıt Tut
            _context.Islemler.Add(new Islem {
                KullaniciId = userId,
                IslemTuru = "Para Çekme",
                Tutar = -istek.Tutar, // Çıkan para eksi görünür
                Aciklama = "Nakit Çekim"
            });

            _context.SaveChanges();
            return Ok(new { mesaj = $"{istek.Tutar} TL çekildi." });
        }

        // 4. Transfer (+Kayıt)
        [HttpPost("transfer")]
        public IActionResult ParaTransferi([FromBody] TransferDto istek)
        {
            var userId = GetUserId();
            var gonderenHesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);
            var aliciHesap = _context.Hesaplar.FirstOrDefault(h => h.HesapNo == istek.AliciHesapNo);

            if (aliciHesap == null) return BadRequest("Alıcı hesap bulunamadı.");
            if (gonderenHesap.HesapNo == aliciHesap.HesapNo) return BadRequest("Kendinize transfer yapamazsınız.");
            if (gonderenHesap.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye.");

            // Parayı taşı
            gonderenHesap.Bakiye -= istek.Tutar;
            aliciHesap.Bakiye += istek.Tutar;

            // ✅ Gönderen İçin Kayıt
            _context.Islemler.Add(new Islem {
                KullaniciId = userId,
                IslemTuru = "Transfer (Giden)",
                Tutar = -istek.Tutar,
                Aciklama = $"Alıcı: {istek.AliciHesapNo}"
            });

            // ✅ Alan Kişi İçin Kayıt (Onun da geçmişinde gözüksün)
            _context.Islemler.Add(new Islem {
                KullaniciId = aliciHesap.KullaniciId,
                IslemTuru = "Transfer (Gelen)",
                Tutar = istek.Tutar,
                Aciklama = $"Gönderen: {gonderenHesap.HesapNo}"
            });

            _context.SaveChanges();
            return Ok(new { mesaj = "Transfer başarılı." });
        }
    }
}