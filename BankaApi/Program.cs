using BankaApi;
using Microsoft.EntityFrameworkCore;  
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
// OpenAPI dökümantasyon desteğini ekle
builder.Services.AddOpenApi(); 

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
app.MapControllers(); 

app.Run();