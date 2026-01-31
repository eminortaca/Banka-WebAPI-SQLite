namespace BankaApi.Dtos
{
    public class BakiyeIslemDto
    {
        public decimal Tutar { get; set; }
    }

    public class TransferDto
    {
        public int AliciHesapNo { get; set; }
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; } = string.Empty; // ✅ YENİ: Kullanıcının notu
    }
}