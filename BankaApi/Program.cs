using BankaApi;
using Microsoft.EntityFrameworkCore;  
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
// OpenAPI dökümantasyon desteğini ekle
builder.Services.AddOpenApi(); 
// --- GÜVENLİK AYARLARI BAŞLANGICI ---
// 1. JWT Ayarlarını 'appsettings.json'dan okuyoruz
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var key = Encoding.UTF8.GetBytes(secretKey);

// 2. Sisteme "Biz JWT kullanacağız" diyoruz
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // İmza kontrolü yap (Sahte bilet olmasın)
            IssuerSigningKey = new SymmetricSecurityKey(key), // Bizim anahtarı kullan
            
            ValidateIssuer = false, // (Şimdilik) Kimin dağıttığını kontrol etme
            ValidateAudience = false, // (Şimdilik) Kime verildiğini kontrol etme
            ValidateLifetime = true, // Süresi dolmuş mu bak
            ClockSkew = TimeSpan.Zero // Sunucu saat farkını yok say (Tam zamanında bitsin)
        };
    });
// --- GÜVENLİK AYARLARI BİTİŞİ ---
builder.Services.AddDbContext<BankaDbContext>(options =>
    options.UseSqlite("Data Source=banka.db"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI dosyasını oluşturur
    app.MapOpenApi(); 
    // Bu satır Scalar arayüzünü açar
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();
app.UseAuthentication(); //  Önce kimlik sorgulama kısmı
app.UseAuthorization();  //  Sonra yetki bakma kısmı
app.MapControllers(); 

app.Run();