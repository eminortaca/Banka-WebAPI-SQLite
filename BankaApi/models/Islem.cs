using System;

namespace BankaApi.Models
{
    public class Islem
    {
        public Guid Id { get; set; } // İşlemin kendi ID'si de Guid olsun
        public string Aciklama { get; set; }
        public decimal Tutar { get; set; }
        public DateTime Tarih { get; set; }

        public Guid HesapId { get; set; } // ARTIK GUID! (Çünkü Hesap.Id Guid oldu)
    }
}