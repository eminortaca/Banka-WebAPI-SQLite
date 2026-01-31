using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankaApi.Models;
using BankaApi.Dtos;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BankaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankaController : ControllerBase
    {
        private readonly BankaDbContext _context;

        public BankaController(BankaDbContext context)
        {
            _context = context;
        }

        // 1. Hesap Bilgilerini Getir
        [HttpGet]
        public IActionResult BilgileriGetir()
        {
            var kullaniciAdi = User.Identity?.Name;
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);
            if (kullanici == null) return Unauthorized();

            var hesaplar = _context.Hesaplar.Where(h => h.KullaniciId == kullanici.Id).ToList();
            return Ok(hesaplar);
        }

        // 2. Para Transferi
        [HttpPost("transfer")]
        public IActionResult ParaTransferi([FromBody] TransferDto istek)
        {
            var gonderenAdi = User.Identity?.Name;
            var gonderenUser = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == gonderenAdi);
            var gonderenHesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == gonderenUser.Id);

            var aliciHesap = _context.Hesaplar.FirstOrDefault(h => h.HesapNo == istek.AliciHesapNo);

            if (gonderenHesap == null) return BadRequest("Hesabınız bulunamadı.");
            if (aliciHesap == null) return BadRequest("Alıcı hesap numarası yanlış.");
            
            // ✅ SENİN İSTEDİĞİN KONTROL:
            if (gonderenHesap.HesapNo == aliciHesap.HesapNo) 
                return BadRequest("Kendinize transfer yapamazsınız. 'Para Yatır' menüsünü kullanın.");

            if (istek.Tutar <= 0) return BadRequest("Geçersiz tutar.");
            if (gonderenHesap.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye!");

            // İşlem
            gonderenHesap.Bakiye -= istek.Tutar;
            aliciHesap.Bakiye += istek.Tutar;

            _context.SaveChanges();

            return Ok(new { mesaj = $"Transfer başarılı. {istek.Tutar} TL gönderildi.", yeniBakiye = gonderenHesap.Bakiye });
        }

        // 3. Para Yatır (Bakiyeyi Artırır)
        [HttpPost("yatir")]
        public IActionResult ParaYatir([FromBody] BakiyeIslemDto istek)
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == user.Id);

            if (istek.Tutar <= 0) return BadRequest("0'dan büyük bir tutar girin.");

            hesap.Bakiye += istek.Tutar;
            _context.SaveChanges();

            return Ok(new { mesaj = $"{istek.Tutar} TL yatırıldı.", yeniBakiye = hesap.Bakiye });
        }

        // 4. Para Çek (Bakiyeyi Azaltır)
        [HttpPost("cek")]
        public IActionResult ParaCek([FromBody] BakiyeIslemDto istek)
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == user.Id);

            if (istek.Tutar <= 0) return BadRequest("Geçersiz tutar.");
            if (hesap.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye!");

            hesap.Bakiye -= istek.Tutar;
            _context.SaveChanges();

            return Ok(new { mesaj = $"{istek.Tutar} TL çekildi.", yeniBakiye = hesap.Bakiye });
        }
    }
}