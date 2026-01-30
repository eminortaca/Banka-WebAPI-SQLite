namespace BankaApi.Dtos // <--- Burasının "BankaApi.Dtos" olması ÇOK ÖNEMLİ
{
    public class KayitOlDto
    {
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
    }

    public class GirisYapDto
    {
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
    }
}