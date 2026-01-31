using Microsoft.AspNetCore.Mvc;
using BankaApi.Models;
using BankaApi.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BankaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BankaDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BankaDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ KAYIT OL (Ad ve Soyad Ekli)
        [HttpPost("register")]
        public IActionResult Register(KayitOlDto istek)
        {
            if (_context.Kullanicilar.Any(k => k.KullaniciAdi == istek.KullaniciAdi))
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }

            // 1. Kullanıcıyı Oluştur
            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = istek.KullaniciAdi,
                Sifre = istek.Sifre,
                Role = "Musteri"
            };

            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();

            // 2. Hesabı Oluştur (Artık Ad ve Soyad DTO'dan geliyor)
            var yeniHesap = new Hesap
            {
                KullaniciId = yeniKullanici.Id,
                Ad = istek.Ad,       // ✨ YENİ: Formdan gelen Ad
                Soyad = istek.Soyad, // ✨ YENİ: Formdan gelen Soyad
                HesapNo = new Random().Next(100000, 999999), 
                Bakiye = 0 
            };

            _context.Hesaplar.Add(yeniHesap);
            _context.SaveChanges();

            return Ok(new { mesaj = "Kayıt başarılı. Hesabınız oluşturuldu.", hesapNo = yeniHesap.HesapNo });
        }

        // ✅ GİRİŞ YAP (Soyad Kontrollü)
        [HttpPost("login")]
        public IActionResult Login(GirisYapDto istek)
        {
            // 1. Kullanıcı Adı ve Şifre Kontrolü
            var user = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == istek.KullaniciAdi && k.Sifre == istek.Sifre);
            
            if (user == null) return Unauthorized("Kullanıcı adı veya şifre yanlış.");

            // 2. ✨ YENİ: Soyad Kontrolü (Güvenlik Önlemi)
            // Kullanıcının hesabını buluyoruz
            var hesap = _context.Hesaplar.FirstOrDefault(h => h.KullaniciId == user.Id);

            // Eğer hesap yoksa veya girilen soyad veritabanındakiyle uyuşmuyorsa REDDET
            // ToLower() kullanarak büyük/küçük harf hatasını engelliyoruz
            if (hesap == null || hesap.Soyad.ToLower() != istek.Soyad.ToLower())
            {
                return Unauthorized("Girdiğiniz Soyad kayıtlı bilgilerle uyuşmuyor!");
            }

            var token = TokenUret(user);
            return Ok(new { token = token });
        }

        private string TokenUret(Kullanici user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.KullaniciAdi),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "varsayilan_gizli_anahtar_123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}