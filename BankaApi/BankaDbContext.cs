using Microsoft.EntityFrameworkCore;
using BankaApi.Models;

namespace BankaApi
{
    public class BankaDbContext : DbContext
    {
        public BankaDbContext(DbContextOptions<BankaDbContext> options) : base(options) { }

        // Bu satır, veritabanında "Hesaplar" adında bir tablo oluşturur.
        public DbSet<Hesap> Hesaplar { get; set; }
    }
}