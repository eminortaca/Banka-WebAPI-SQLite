using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BankaApi.Models;
using BankaApi.Dtos;
using BCrypt.Net;
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
                Sifre = passwordHash,
                Role = "Musteri"
            };

            _context.Kullanicilar.Add(yeniKullanici);
            _context.SaveChanges();

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] GirisYapDto istek)
        {
            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == istek.KullaniciAdi);

            if (kullanici == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            if (!BCrypt.Net.BCrypt.Verify(istek.Sifre, kullanici.Sifre))
            {
                return BadRequest("Şifre yanlış!");
            }

            string token = TokenUret(kullanici);

            return Ok(new { Token = token });
        }

        private string TokenUret(Kullanici kullanici)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Role),
                new Claim("Id", kullanici.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
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