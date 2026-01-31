namespace BankaApi.Dtos
{
    // Para Transferi İçin (Başka hesaba yollama)
    public class TransferDto
    {
        public int AliciHesapNo { get; set; } // Kime gidecek?
        public decimal Tutar { get; set; }    // Ne kadar?
    }

    // Para Yatırma ve Çekme İçin (Kendi hesabın)
    public class BakiyeIslemDto
    {
        public decimal Tutar { get; set; }
    }
}