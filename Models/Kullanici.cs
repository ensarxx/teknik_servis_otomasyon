using System;

namespace TeknikServisOtomasyon.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Rol { get; set; } = "Tekniker"; // Admin, Tekniker, Kasiyer
        public bool Aktif { get; set; } = true;
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public DateTime? SonGirisTarihi { get; set; }
    }
}
