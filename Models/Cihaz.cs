using System;

namespace TeknikServisOtomasyon.Models
{
    public class Cihaz
    {
        public int Id { get; set; }
        public string CihazTuru { get; set; } = string.Empty; // Telefon, Tablet, Laptop
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SeriNo { get; set; } = string.Empty;
        public string IMEI { get; set; } = string.Empty;
        public int MusteriId { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public string Aciklama { get; set; } = string.Empty;

        // Navigation
        public Musteri? Musteri { get; set; }
    }
}
