using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankaApi.Models;
using BankaApi.Dtos;
using System.Security.Claims;

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

        // Yardımcı Metot: Giriş yapmış kullanıcının ID'sini bulur
        private Guid GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idClaim);
        }

        [HttpGet]
        public IActionResult BilgileriGetir()
        {
            var userId = GetUserId();
            
            // Kullanıcıyı bul (Adını ekrana yazdırmak için)
            var kullanici = _context.Kullanicilar.Find(userId);
            
            // Hesabını bul
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);

            if (hesap == null) return NotFound("Hesap bulunamadı.");

            // Frontend'e hem hesap hem isim bilgisini tek pakette yolluyoruz
            return Ok(new { 
                ad = kullanici.Ad,
                soyad = kullanici.Soyad,
                hesapNo = hesap.HesapNo,
                bakiye = hesap.Bakiye
            });
        }

        [HttpPost("yatir")]
        public IActionResult ParaYatir([FromBody] BakiyeIslemDto istek)
        {
            var userId = GetUserId();
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);

            if (istek.Tutar <= 0) return BadRequest("0'dan büyük tutar girin.");

            hesap.Bakiye += istek.Tutar;
            _context.SaveChanges();

            return Ok(new { mesaj = $"Başarılı! {istek.Tutar} TL yatırıldı." });
        }

        [HttpPost("cek")]
        public IActionResult ParaCek([FromBody] BakiyeIslemDto istek)
        {
            var userId = GetUserId();
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);

            if (hesap.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye!");

            hesap.Bakiye -= istek.Tutar;
            _context.SaveChanges();

            return Ok(new { mesaj = $"Başarılı! {istek.Tutar} TL çekildi." });
        }

        [HttpPost("transfer")]
        public IActionResult ParaTransferi([FromBody] TransferDto istek)
        {
            var userId = GetUserId();
            var gonderenHesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == userId);
            var aliciHesap = _context.Hesaplar.FirstOrDefault(h => h.HesapNo == istek.AliciHesapNo);

            if (aliciHesap == null) return BadRequest("Alıcı hesap bulunamadı.");
            if (gonderenHesap.HesapNo == aliciHesap.HesapNo) return BadRequest("Kendinize transfer yapamazsınız.");
            if (gonderenHesap.Bakiye < istek.Tutar) return BadRequest("Yetersiz bakiye.");

            gonderenHesap.Bakiye -= istek.Tutar;
            aliciHesap.Bakiye += istek.Tutar;
            _context.SaveChanges();

            return Ok(new { mesaj = $"Transfer Başarılı! {istek.Tutar} TL gönderildi." });
        }
    }
}