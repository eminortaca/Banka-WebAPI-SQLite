using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BankaApi.Models;
using BankaApi.Dtos;
using BCrypt.Net; // Şifreleme Paketi
using Microsoft.IdentityModel.Tokens; // Token İmzalama
using System.IdentityModel.Tokens.Jwt; // JWT Oluşturma
using System.Security.Claims; // Kimlik Bilgileri
using System.Text; // Encoding işlemleri

namespace BankaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BankaDbContext _context;
        private readonly IConfiguration _configuration; // Ayar dosyasını okumak için

        public AuthController(BankaDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // 1. KAYIT OL (Register)
        [HttpPost("register")]
        public IActionResult Register([FromBody] KayitOlDto istek)
        {
            // Kullanıcı zaten var mı?
            if (_context.Kullanicilar.Any(u => u.KullaniciAdi == istek.KullaniciAdi))
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }

            // Şifreyi Hash'le (BCrypt ile güvenli hale getir)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(istek.Sifre);

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = istek.KullaniciAdi,
                Sifre = passwordHash, // Asla düz şifreyi kaydetmeyiz!
                Role = "Musteri"
            };

            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        // 2. GİRİŞ YAP (Login)
        [HttpPost("login")]
        public IActionResult Login([FromBody] GirisYapDto istek)
        {
            // Kullanıcıyı bul
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == istek.KullaniciAdi);

            if (kullanici == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            // Şifreyi Kontrol Et (Hash'li şifre ile girilen şifreyi kıyasla)
            if (!BCrypt.Net.BCrypt.Verify(istek.Sifre, kullanici.Sifre))
            {
                return BadRequest("Şifre yanlış!");
            }

            // --- TOKEN (BİLET) ÜRETME KISMI ---
            string token = TokenUret(kullanici);

            return Ok(new { Token = token });
        }

        // Yardımcı Metot: Token Oluşturucu
        private string TokenUret(Kullanici kullanici)
        {
            // 1. Token'ın içine gömülecek bilgiler (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Role),
                new Claim("Id", kullanici.Id.ToString()) // ID'yi de gömelim
            };

            // 2. İmza için gizli anahtarımızı alıyoruz
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            
            // 3. Şifreleme algoritmasını seçiyoruz (HmacSha256)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Token ayarlarını yapıyoruz
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token 1 saat geçerli olsun
                signingCredentials: creds
            );

            // 5. Token'ı oluşturup string olarak döndür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}