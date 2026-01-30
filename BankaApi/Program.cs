using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Bu satırı sakın silme!
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// --- SERVİSLER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Banka API", Version = "v1" });

    // 1. Kilit Butonunu Tanımla
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token'ı şuraya yapıştırın: 'Bearer [Tokenınız]'"
    });

    // 2. Kilit Gerektiren Endpointleri Belirle
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
// --------------------------------------------------

// Veritabanı Ayarı (Seninki zaten var, burayı elleme)
builder.Services.AddDbContext<BankaApi.BankaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- GÜVENLİK (AUTH) AYARLARI ---
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
// ----------------------------------

var app = builder.Build();

// --- MIDDLEWARE (SIRASI ÖNEMLİ) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 1. Kimlik Sor
app.UseAuthorization();  // 2. Yetki Ver

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BankaApi.BankaDbContext>();
    
    // Veritabanını zorla güncelle (Tablo yoksa oluşturur)
    context.Database.Migrate();
}
app.Run();