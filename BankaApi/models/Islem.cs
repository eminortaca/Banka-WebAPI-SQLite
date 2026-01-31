namespace BankaApi.Models
{
    public class Islem
    {
        public int Id { get; set; }
        public Guid KullaniciId { get; set; } // Hangi kullanıcı yaptı?
        public string IslemTuru { get; set; } = string.Empty; // "Yatırma", "Çekme", "Transfer"
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now; // İşlem saati
        public string Aciklama { get; set; } = string.Empty; // "Ahmet'e gönderildi" gibi
    }
}