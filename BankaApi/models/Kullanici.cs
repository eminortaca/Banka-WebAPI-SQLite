namespace BankaApi.Models
{
    public class Kullanici
    {
        // ✅ DEĞİŞİKLİK BURADA: int yerine Guid yaptık
        public Guid Id { get; set; } = Guid.NewGuid(); 

        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string Role { get; set; } = "Musteri";
    }
}