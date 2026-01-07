using System;

namespace TeknikServisOtomasyon.Models
{
    public class EmailAyarlari
    {
        public int Id { get; set; }
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string GondericiEmail { get; set; } = string.Empty;
        public string GondericiSifre { get; set; } = string.Empty; // App Password
        public string GondericiAdi { get; set; } = "Teknik Servis";
        public bool SslKullan { get; set; } = true;
        public bool EmailAktif { get; set; } = false;
        public DateTime GuncellemeTarihi { get; set; } = DateTime.Now;
    }
}
