using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore; // <--- 1. YENİ EKLENEN KÜTÜPHANE

var builder = WebApplication.CreateBuilder(args);

// --- SERVİSLER ---
builder.Services.AddControllers();
// --- CORS AYARI (Frontend Bağlantısı İçin) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("IzinVer", b => 
        b.AllowAnyOrigin()   // Her yerden gelene izin ver (Geliştirme için)
         .AllowAnyMethod()   // GET, POST, PUT her şeye izin ver
         .AllowAnyHeader()); // Tüm başlıklara izin ver
});

builder.Services.AddEndpointsApiExplorer();

// Swagger Ayarları (JSON oluşturucu olarak kalmalı)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Banka API", Version = "v1" });

    // Kilit Ayarları
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token'ı şuraya yapıştırın: 'Bearer [Tokenınız]'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Veritabanı Ayarı
builder.Services.AddDbContext<BankaApi.BankaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth Ayarları
var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? "varsayilan_cok_gizli_anahtar_123";
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// --- MIDDLEWARE ---
app.UseStaticFiles(); // Statik dosyalar için (HTML, CSS, JS)
app.UseDefaultFiles(); // index.html'i otomatik göster
app.UseCors("IzinVer"); 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 2. SWAGGER UI YERİNE SCALAR AYARLARI
if (app.Environment.IsDevelopment())
{   
    app.UseSwagger(); // JSON dosyasını üretir (Bu kalmalı)

    // Eski çirkin ekranı kapattık:
    // app.UseSwaggerUI(); 

    // Yeni havalı ekranı açtık:
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
        options.WithTitle("Banka API - Scalar")
               .WithTheme(ScalarTheme.Mars) // Tema: Mars (Kırmızı/Siyah)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Otomatik Veritabanı Güncelleyici
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BankaApi.BankaDbContext>();
    context.Database.Migrate();
}
app.MapFallbackToFile("index.html");
app.Run();