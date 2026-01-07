using System;

namespace TeknikServisOtomasyon.Models
{
    public class ServisFotograf
    {
        public int Id { get; set; }
        public int ServisId { get; set; }
        public string DosyaAdi { get; set; } = string.Empty;
        public string DosyaYolu { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string FotografTipi { get; set; } = "Giriş"; // Giriş, Arıza, Onarım, Çıkış
        public DateTime YuklenmeTarihi { get; set; } = DateTime.Now;

        // Navigation
        public ServisKaydi? ServisKaydi { get; set; }
    }
}
