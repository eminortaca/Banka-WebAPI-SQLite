namespace BankaApi.Models
{
    public class Kullanici
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Ad { get; set; } = string.Empty;    // ✅ Giriş için bu lazım
        public string Soyad { get; set; } = string.Empty; // ✅ Giriş için bu lazım
        public string Sifre { get; set; } = string.Empty;
        public string Role { get; set; } = "Musteri";
    }
}