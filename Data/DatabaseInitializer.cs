using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace TeknikServisOtomasyon.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task InitializeDatabaseAsync()
        {
            // Önce veritabanını oluştur
            await CreateDatabaseIfNotExistsAsync();

            // Tabloları oluştur
            await CreateTablesAsync();

            // Varsayılan admin kullanıcısı ekle
            await InsertDefaultDataAsync();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var builder = new MySqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.Database;
            builder.Database = "";

            using var connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            var createDbQuery = $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            using var command = new MySqlCommand(createDbQuery, connection);
            await command.ExecuteNonQueryAsync();
        }

        private async Task CreateTablesAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var createTablesQuery = @"
                -- Kullanıcılar tablosu
                CREATE TABLE IF NOT EXISTS Kullanicilar (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    KullaniciAdi VARCHAR(50) NOT NULL UNIQUE,
                    Sifre VARCHAR(255) NOT NULL,
                    AdSoyad VARCHAR(100) NOT NULL,
                    Email VARCHAR(100),
                    Telefon VARCHAR(20),
                    Rol VARCHAR(20) NOT NULL DEFAULT 'Tekniker',
                    Aktif BOOLEAN DEFAULT TRUE,
                    KayitTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    SonGirisTarihi DATETIME
                );

                -- Müşteriler tablosu
                CREATE TABLE IF NOT EXISTS Musteriler (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    AdSoyad VARCHAR(100) NOT NULL,
                    Telefon VARCHAR(20) NOT NULL,
                    Telefon2 VARCHAR(20),
                    Email VARCHAR(100),
                    Adres TEXT,
                    TCKimlikNo VARCHAR(11),
                    KayitTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Notlar TEXT,
                    Aktif BOOLEAN DEFAULT TRUE,
                    INDEX idx_telefon (Telefon),
                    INDEX idx_adsoyad (AdSoyad)
                );

                -- Cihazlar tablosu
                CREATE TABLE IF NOT EXISTS Cihazlar (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    CihazTuru VARCHAR(50) NOT NULL,
                    Marka VARCHAR(50) NOT NULL,
                    Model VARCHAR(100),
                    SeriNo VARCHAR(100),
                    IMEI VARCHAR(20),
                    MusteriId INT NOT NULL,
                    KayitTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Aciklama TEXT,
                    FOREIGN KEY (MusteriId) REFERENCES Musteriler(Id),
                    INDEX idx_musteri (MusteriId),
                    INDEX idx_cihaz_turu (CihazTuru)
                );

                -- Servis Kayıtları tablosu
                CREATE TABLE IF NOT EXISTS ServisKayitlari (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ServisNo VARCHAR(20) NOT NULL UNIQUE,
                    MusteriId INT NOT NULL,
                    CihazId INT NOT NULL,
                    GirisTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    TeslimTarihi DATETIME,
                    Ariza VARCHAR(255) NOT NULL,
                    ArizaDetay TEXT,
                    YapilanIslemler TEXT,
                    KullanilanParcalar TEXT,
                    IscilikUcreti DECIMAL(10,2) DEFAULT 0,
                    ParcaUcreti DECIMAL(10,2) DEFAULT 0,
                    ToplamUcret DECIMAL(10,2) DEFAULT 0,
                    Durum VARCHAR(30) DEFAULT 'Beklemede',
                    OncelikDurumu VARCHAR(20) DEFAULT 'Normal',
                    TeknikerId INT,
                    TahsilatDurumu VARCHAR(20) DEFAULT 'Ödenmedi',
                    OdenenTutar DECIMAL(10,2) DEFAULT 0,
                    Notlar TEXT,
                    GarantiBitisTarihi DATETIME,
                    KayitTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    GuncellenmeTarihi DATETIME,
                    FOREIGN KEY (MusteriId) REFERENCES Musteriler(Id),
                    FOREIGN KEY (CihazId) REFERENCES Cihazlar(Id),
                    FOREIGN KEY (TeknikerId) REFERENCES Kullanicilar(Id),
                    INDEX idx_servis_no (ServisNo),
                    INDEX idx_musteri_id (MusteriId),
                    INDEX idx_durum (Durum),
                    INDEX idx_giris_tarihi (GirisTarihi)
                );

                -- Parçalar tablosu
                CREATE TABLE IF NOT EXISTS Parcalar (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ParcaKodu VARCHAR(50) NOT NULL UNIQUE,
                    ParcaAdi VARCHAR(100) NOT NULL,
                    Kategori VARCHAR(50),
                    Marka VARCHAR(50),
                    Model VARCHAR(100),
                    StokMiktari INT DEFAULT 0,
                    MinStokMiktari INT DEFAULT 5,
                    AlisFiyati DECIMAL(10,2) DEFAULT 0,
                    SatisFiyati DECIMAL(10,2) DEFAULT 0,
                    Birim VARCHAR(20) DEFAULT 'Adet',
                    Konum VARCHAR(50),
                    Aciklama TEXT,
                    Aktif BOOLEAN DEFAULT TRUE,
                    KayitTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    INDEX idx_parca_kodu (ParcaKodu),
                    INDEX idx_kategori (Kategori)
                );

                -- Stok Hareketleri tablosu
                CREATE TABLE IF NOT EXISTS StokHareketleri (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ParcaId INT NOT NULL,
                    HareketTipi VARCHAR(20) NOT NULL,
                    Miktar INT NOT NULL,
                    BirimFiyat DECIMAL(10,2),
                    ServisKaydiId INT,
                    Aciklama TEXT,
                    KullaniciId INT NOT NULL,
                    Tarih DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (ParcaId) REFERENCES Parcalar(Id),
                    FOREIGN KEY (ServisKaydiId) REFERENCES ServisKayitlari(Id),
                    FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id),
                    INDEX idx_parca (ParcaId),
                    INDEX idx_tarih (Tarih)
                );

                -- Ödemeler tablosu
                CREATE TABLE IF NOT EXISTS Odemeler (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ServisKaydiId INT NOT NULL,
                    Tutar DECIMAL(10,2) NOT NULL,
                    OdemeTipi VARCHAR(30) NOT NULL DEFAULT 'Nakit',
                    OdemeTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Aciklama TEXT,
                    KullaniciId INT NOT NULL,
                    FOREIGN KEY (ServisKaydiId) REFERENCES ServisKayitlari(Id),
                    FOREIGN KEY (KullaniciId) REFERENCES Kullanicilar(Id),
                    INDEX idx_servis_kaydi (ServisKaydiId),
                    INDEX idx_tarih (OdemeTarihi)
                );

                -- Servis Fotoğrafları tablosu
                CREATE TABLE IF NOT EXISTS servis_fotograflar (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ServisId INT NOT NULL,
                    DosyaAdi VARCHAR(255) NOT NULL,
                    DosyaYolu VARCHAR(500) NOT NULL,
                    Aciklama TEXT,
                    FotografTipi VARCHAR(30) DEFAULT 'Giriş',
                    YuklenmeTarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (ServisId) REFERENCES ServisKayitlari(Id) ON DELETE CASCADE,
                    INDEX idx_servis (ServisId),
                    INDEX idx_tip (FotografTipi)
                );
            ";

            using var command = new MySqlCommand(createTablesQuery, connection);
            await command.ExecuteNonQueryAsync();
        }

        private async Task InsertDefaultDataAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Admin kullanıcısı var mı kontrol et
            var checkQuery = "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = 'admin'";
            using var checkCommand = new MySqlCommand(checkQuery, connection);
            var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

            if (count == 0)
            {
                // Varsayılan admin kullanıcısı ekle (şifre: 123456)
                var insertQuery = @"
                    INSERT INTO Kullanicilar (KullaniciAdi, Sifre, AdSoyad, Email, Rol, Aktif) 
                    VALUES ('admin', '123456', 'Sistem Yöneticisi', 'admin@teknikservis.com', 'Admin', TRUE)";
                using var insertCommand = new MySqlCommand(insertQuery, connection);
                await insertCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
