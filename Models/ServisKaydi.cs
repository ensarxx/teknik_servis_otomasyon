using System;

namespace TeknikServisOtomasyon.Models
{
    public class ServisKaydi
    {
        public int Id { get; set; }
        public string ServisNo { get; set; } = string.Empty;
        public int MusteriId { get; set; }
        public int CihazId { get; set; }
        public DateTime GirisTarihi { get; set; } = DateTime.Now;
        public DateTime? TeslimTarihi { get; set; }
        public string Ariza { get; set; } = string.Empty;
        public string ArizaDetay { get; set; } = string.Empty;
        public string YapilanIslemler { get; set; } = string.Empty;
        public string KullanilanParcalar { get; set; } = string.Empty;
        public decimal IscilikUcreti { get; set; }
        public decimal ParcaUcreti { get; set; }
        public decimal ToplamUcret { get; set; }
        public string Durum { get; set; } = "Beklemede"; // Beklemede, İşlemde, Tamamlandı, Teslim Edildi, İptal
        public string OncelikDurumu { get; set; } = "Normal"; // Düşük, Normal, Yüksek, Acil
        public int? TeknikerId { get; set; }
        public string TahsilatDurumu { get; set; } = "Ödenmedi"; // Ödenmedi, Kısmi Ödeme, Ödendi
        public decimal OdenenTutar { get; set; }
        public string Notlar { get; set; } = string.Empty;
        public DateTime? GarantiBitisTarihi { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }

        // Navigation
        public Musteri? Musteri { get; set; }
        public Cihaz? Cihaz { get; set; }
        public Kullanici? Tekniker { get; set; }
    }
}
