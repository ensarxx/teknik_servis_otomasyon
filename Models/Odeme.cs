using System;

namespace TeknikServisOtomasyon.Models
{
    public class Odeme
    {
        public int Id { get; set; }
        public int ServisKaydiId { get; set; }
        public decimal Tutar { get; set; }
        public string OdemeTipi { get; set; } = "Nakit"; // Nakit, Kredi Kartı, Havale/EFT
        public DateTime OdemeTarihi { get; set; } = DateTime.Now;
        public string Aciklama { get; set; } = string.Empty;
        public int KullaniciId { get; set; }

        // Navigation
        public ServisKaydi? ServisKaydi { get; set; }
        public Kullanici? Kullanici { get; set; }
    }
}
