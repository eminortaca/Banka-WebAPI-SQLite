using System;
using System.Collections.Generic;

namespace BankaApi.Dtos
{
    // Bu sınıf veritabanına kaydedilmez. 
    // Sadece kullanıcıdan "Gönderen", "Alıcı" ve "Tutar" bilgisini paket halinde almak içindir.
    public class ParaTransferiDto
    {
        public Guid GonderenHesapId { get; set; }
        public Guid AliciHesapId { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }
    }
}