using System.ComponentModel.DataAnnotations;

namespace BankaApi.Models
{
    public class Hesap
    {
        // ✅ SENİN İSTEDİĞİN: ID artık Guid
        public Guid Id { get; set; } = Guid.NewGuid(); 

        // ⚠️ DİKKAT: Kullanıcı tablon 'int' kullanıyorsa burası 'int' kalmalı. 
        // Eğer Kullanıcı tablon da Guid ise burayı da 'Guid' yapmalısın.
        public Guid KullaniciId { get; set; } 

        // Banka Numarası (Transferde kullandığımız 6 haneli sayı)
        public int HesapNo { get; set; } 

        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        
        public decimal Bakiye { get; set; }
    }
}