namespace BankaApi.Models
{
    public class Hesap
    {
        // ✅ Kendi ID'si Guid oldu
        public Guid Id { get; set; } = Guid.NewGuid();

        // ✅ BAĞLANTI BURADA: Artık Kullanıcı'nın Guid ID'sini tutacak
        public Guid KullaniciId { get; set; } 

        // Transfer için kullanılan 6 haneli Banka Numarası (Bu int kalmalı, IBAN gibi düşün)
        public int HesapNo { get; set; } 

        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        
        public decimal Bakiye { get; set; }
    }
}