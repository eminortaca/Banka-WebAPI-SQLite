using Microsoft.AspNetCore.Mvc;
using BankaApi.Models;
using BankaApi.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        [HttpPost("register")]
        public IActionResult Register(KayitOlDto istek)
        {
            // İsim ve Soyisim kontrolü (Kullanıcı Adı kontrolü yerine)
            if (_context.Kullanicilar.Any(k => k.Ad == istek.Ad && k.Soyad == istek.Soyad))
            {
                return BadRequest("Bu isim ve soyisimle zaten bir kayıt var.");
            }

            var yeniKullanici = new Kullanici
            {
                Ad = istek.Ad,
                Soyad = istek.Soyad,
                Sifre = istek.Sifre,
                Role = "Musteri"
            };

            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();

            var yeniHesap = new Hesap
            {
                KullaniciId = yeniKullanici.Id,
                HesapNo = new Random().Next(100000, 999999), 
                Bakiye = 0 
            };

            _context.Hesaplar.Add(yeniHesap);
            _context.SaveChanges();

            return Ok(new { mesaj = "Kayıt başarılı!", hesapNo = yeniHesap.HesapNo });
        }

        [HttpPost("login")]
        public IActionResult Login(GirisYapDto istek)
        {
            // Giriş kontrolü
            var user = _context.Kullanicilar.FirstOrDefault(k => 
                k.Ad.ToLower() == istek.Ad.ToLower() && 
                k.Soyad.ToLower() == istek.Soyad.ToLower() && 
                k.Sifre == istek.Sifre);
            
            if (user == null) return Unauthorized("İsim, Soyisim veya Şifre hatalı.");

            var token = TokenUret(user);
            return Ok(new { token = token });
        }

        private string TokenUret(Kullanici user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Ad),
                new Claim(ClaimTypes.Surname, user.Soyad),
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