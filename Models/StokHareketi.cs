using System;

namespace TeknikServisOtomasyon.Models
{
    public class StokHareketi
    {
        public int Id { get; set; }
        public int ParcaId { get; set; }
        public string HareketTipi { get; set; } = string.Empty; // Giriş, Çıkış
        public int Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public int? ServisKaydiId { get; set; }
        public string Aciklama { get; set; } = string.Empty;
        public int KullaniciId { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;

        // Navigation
        public Parca? Parca { get; set; }
        public ServisKaydi? ServisKaydi { get; set; }
        public Kullanici? Kullanici { get; set; }
    }
}
