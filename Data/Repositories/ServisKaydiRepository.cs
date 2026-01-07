using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class ServisKaydiRepository
    {
        private readonly string _connectionString;

        public ServisKaydiRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<List<ServisKaydi>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, m.AdSoyad as MusteriAdi, c.Marka, c.Model, c.CihazTuru
                          FROM ServisKayitlari s
                          INNER JOIN Musteriler m ON s.MusteriId = m.Id
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          ORDER BY s.GirisTarihi DESC";
            var result = await connection.QueryAsync<ServisKaydi>(query);
            return result.ToList();
        }

        public async Task<ServisKaydi?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, m.AdSoyad as MusteriAdi, m.Telefon as MusteriTelefon,
                          c.Marka, c.Model, c.CihazTuru, c.SeriNo, c.IMEI
                          FROM ServisKayitlari s
                          INNER JOIN Musteriler m ON s.MusteriId = m.Id
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          WHERE s.Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<ServisKaydi>(query, new { Id = id });
        }

        public async Task<ServisKaydi?> GetByServisNoAsync(string servisNo)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, m.AdSoyad as MusteriAdi, m.Telefon as MusteriTelefon,
                          c.Marka, c.Model, c.CihazTuru, c.SeriNo, c.IMEI
                          FROM ServisKayitlari s
                          INNER JOIN Musteriler m ON s.MusteriId = m.Id
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          WHERE s.ServisNo = @ServisNo";
            return await connection.QueryFirstOrDefaultAsync<ServisKaydi>(query, new { ServisNo = servisNo });
        }

        public async Task<List<ServisKaydi>> GetByMusteriIdAsync(int musteriId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, c.Marka, c.Model, c.CihazTuru
                          FROM ServisKayitlari s
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          WHERE s.MusteriId = @MusteriId
                          ORDER BY s.GirisTarihi DESC";
            var result = await connection.QueryAsync<ServisKaydi>(query, new { MusteriId = musteriId });
            return result.ToList();
        }

        public async Task<List<ServisKaydi>> GetByDurumAsync(string durum)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, m.AdSoyad as MusteriAdi, m.Telefon as MusteriTelefon,
                          c.Marka, c.Model, c.CihazTuru
                          FROM ServisKayitlari s
                          INNER JOIN Musteriler m ON s.MusteriId = m.Id
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          WHERE s.Durum = @Durum
                          ORDER BY s.GirisTarihi DESC";
            var result = await connection.QueryAsync<ServisKaydi>(query, new { Durum = durum });
            return result.ToList();
        }

        public async Task<string> GenerateServisNoAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var today = DateTime.Now.ToString("yyyyMMdd");
            var query = $"SELECT COUNT(*) + 1 FROM ServisKayitlari WHERE ServisNo LIKE 'SRV{today}%'";
            var count = await connection.ExecuteScalarAsync<int>(query);
            return $"SRV{today}{count:D4}";
        }

        public async Task<int> InsertAsync(ServisKaydi servisKaydi)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"INSERT INTO ServisKayitlari 
                          (ServisNo, MusteriId, CihazId, GirisTarihi, Ariza, ArizaDetay, Durum, OncelikDurumu, TeknikerId, Notlar) 
                          VALUES (@ServisNo, @MusteriId, @CihazId, @GirisTarihi, @Ariza, @ArizaDetay, @Durum, @OncelikDurumu, @TeknikerId, @Notlar);
                          SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, servisKaydi);
        }

        public async Task<bool> UpdateAsync(ServisKaydi servisKaydi)
        {
            using var connection = new MySqlConnection(_connectionString);
            servisKaydi.GuncellenmeTarihi = DateTime.Now;
            var query = @"UPDATE ServisKayitlari SET 
                          Ariza = @Ariza, ArizaDetay = @ArizaDetay, YapilanIslemler = @YapilanIslemler,
                          KullanilanParcalar = @KullanilanParcalar, IscilikUcreti = @IscilikUcreti,
                          ParcaUcreti = @ParcaUcreti, ToplamUcret = @ToplamUcret, Durum = @Durum,
                          OncelikDurumu = @OncelikDurumu, TeknikerId = @TeknikerId, TahsilatDurumu = @TahsilatDurumu,
                          OdenenTutar = @OdenenTutar, Notlar = @Notlar, TeslimTarihi = @TeslimTarihi,
                          GarantiBitisTarihi = @GarantiBitisTarihi, GuncellenmeTarihi = @GuncellenmeTarihi
                          WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, servisKaydi);
            return affected > 0;
        }

        public async Task<bool> UpdateDurumAsync(int id, string durum)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"UPDATE ServisKayitlari SET Durum = @Durum, GuncellenmeTarihi = @Tarih WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, new { Id = id, Durum = durum, Tarih = DateTime.Now });
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync("DELETE FROM ServisKayitlari WHERE Id = @Id", new { Id = id });
            return affected > 0;
        }

        public async Task<List<ServisKaydi>> SearchAsync(string searchTerm)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, m.AdSoyad as MusteriAdi, m.Telefon as MusteriTelefon,
                          c.Marka, c.Model, c.CihazTuru
                          FROM ServisKayitlari s
                          INNER JOIN Musteriler m ON s.MusteriId = m.Id
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          WHERE s.ServisNo LIKE @Search OR m.AdSoyad LIKE @Search OR m.Telefon LIKE @Search 
                          OR c.Marka LIKE @Search OR c.Model LIKE @Search
                          ORDER BY s.GirisTarihi DESC";
            var result = await connection.QueryAsync<ServisKaydi>(query, new { Search = $"%{searchTerm}%" });
            return result.ToList();
        }

        public async Task<List<ServisKaydi>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT s.*, m.AdSoyad as MusteriAdi, c.Marka, c.Model, c.CihazTuru
                          FROM ServisKayitlari s
                          INNER JOIN Musteriler m ON s.MusteriId = m.Id
                          INNER JOIN Cihazlar c ON s.CihazId = c.Id
                          WHERE s.GirisTarihi BETWEEN @StartDate AND @EndDate
                          ORDER BY s.GirisTarihi DESC";
            var result = await connection.QueryAsync<ServisKaydi>(query, new { StartDate = startDate, EndDate = endDate });
            return result.ToList();
        }

        // Dashboard istatistikleri
        public async Task<int> GetCountByDurumAsync(string durum)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ServisKayitlari WHERE Durum = @Durum", 
                new { Durum = durum });
        }

        public async Task<int> GetTodayCountAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ServisKayitlari WHERE DATE(GirisTarihi) = CURDATE()");
        }

        public async Task<decimal> GetTodayIncomeAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(OdenenTutar), 0) FROM ServisKayitlari WHERE DATE(GirisTarihi) = CURDATE()");
        }
    }
}
