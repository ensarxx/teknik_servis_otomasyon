using System;

namespace TeknikServisOtomasyon.Models
{
    public class Parca
    {
        public int Id { get; set; }
        public string ParcaKodu { get; set; } = string.Empty;
        public string ParcaAdi { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int StokMiktari { get; set; }
        public int MinStokMiktari { get; set; } = 5;
        public decimal AlisFiyati { get; set; }
        public decimal SatisFiyati { get; set; }
        public string Birim { get; set; } = "Adet";
        public string Konum { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public bool Aktif { get; set; } = true;
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
    }
}
