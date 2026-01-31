namespace BankaApi.Dtos
{
    public class KayitOlDto
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
    }

    public class GirisYapDto
    {
        public string Ad { get; set; } = string.Empty;    
        public string Soyad { get; set; } = string.Empty; 
        public string Sifre { get; set; } = string.Empty;
    }
}