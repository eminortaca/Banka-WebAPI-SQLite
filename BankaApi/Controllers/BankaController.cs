using Microsoft.AspNetCore.Mvc;
using BankaApi.Models; // Modeline ulaşmak için

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
        

        // 1. Hesap Bilgilerini Getir (GET)
        [HttpGet]
        public IActionResult GetHesap()
        {
            // 1. Veritabanındaki ilk hesabı bulmaya çalış
            var hesap = _context.Hesaplar.FirstOrDefault();

            // 2. Eğer veritabanı bomboşsa (ilk çalıştırma)
            if (hesap == null)
            {
                hesap = new Hesap("Emin", "Ortaca", 1000, 500); // Yeni bir nesne oluştur
                _context.Hesaplar.Add(hesap); // Listeye ekle
                _context.SaveChanges(); // VERİTABANINA KALICI OLARAK YAZ
            }

            return Ok(hesap);
        }

        // 2. Para Yatır (POST)
        [HttpPost("yatir")]
        public IActionResult ParaYatir([FromBody] decimal miktar)
        {
            var hesap = _context.Hesaplar.FirstOrDefault();
            if (hesap == null) return NotFound("Hesap bulunamadı.");

            hesap.ParaYatir(miktar); // Senin yazdığın iş mantığı
            
            _context.SaveChanges(); // Değişikliği banka.db dosyasına kaydet
            return Ok($"{miktar} TL yatırıldı. Yeni bakiye: {hesap.Bakiye}");
        }
        // 3. Para Çek (POST)
        [HttpPost("cek")]
        public IActionResult ParaCek([FromBody] decimal miktar)
        {
            var hesap = _context.Hesaplar.FirstOrDefault();
            if (hesap == null) return NotFound("Hesap bulunamadı.");

            if (hesap.Bakiye < miktar) 
                return BadRequest("Yetersiz bakiye!");

            hesap.ParaCek(miktar);
            _context.SaveChanges(); // Değişikliği banka.db dosyasına kaydet
            return Ok($"{miktar} TL çekildi. Kalan bakiye: {hesap.Bakiye}");
        }
    }
}