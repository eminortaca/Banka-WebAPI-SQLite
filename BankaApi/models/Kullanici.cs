namespace BankaApi.Models // <--- Burası "BankaApi.Models" olmalı
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Role { get; set; } = "Musteri";
    }
}