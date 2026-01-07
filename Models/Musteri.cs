using System;
using System.Collections.Generic;

namespace TeknikServisOtomasyon.Models
{
    public class Musteri
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Telefon2 { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string TCKimlikNo { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public string Notlar { get; set; } = string.Empty;
        public bool Aktif { get; set; } = true;

        // Navigation
        public List<Cihaz>? Cihazlar { get; set; }
        public List<ServisKaydi>? ServisKayitlari { get; set; }
    }
}
