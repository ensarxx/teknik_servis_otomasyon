using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class ParcaRepository
    {
        private readonly string _connectionString;

        public ParcaRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<List<Parca>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Parca>("SELECT * FROM Parcalar WHERE Aktif = TRUE ORDER BY ParcaAdi");
            return result.ToList();
        }

        public async Task<Parca?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Parca>("SELECT * FROM Parcalar WHERE Id = @Id", new { Id = id });
        }

        public async Task<Parca?> GetByKodAsync(string parcaKodu)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Parca>(
                "SELECT * FROM Parcalar WHERE ParcaKodu = @ParcaKodu", 
                new { ParcaKodu = parcaKodu });
        }

        public async Task<List<Parca>> GetLowStockAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Parca>(
                "SELECT * FROM Parcalar WHERE Aktif = TRUE AND StokMiktari <= MinStokMiktari ORDER BY StokMiktari");
            return result.ToList();
        }

        public async Task<List<Parca>> SearchAsync(string searchTerm)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT * FROM Parcalar 
                          WHERE Aktif = TRUE AND (ParcaKodu LIKE @Search OR ParcaAdi LIKE @Search OR Marka LIKE @Search OR Kategori LIKE @Search)
                          ORDER BY ParcaAdi";
            var result = await connection.QueryAsync<Parca>(query, new { Search = $"%{searchTerm}%" });
            return result.ToList();
        }

        public async Task<int> InsertAsync(Parca parca)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"INSERT INTO Parcalar (ParcaKodu, ParcaAdi, Kategori, Marka, Model, StokMiktari, MinStokMiktari, AlisFiyati, SatisFiyati, Birim, Konum, Aciklama, Aktif) 
                          VALUES (@ParcaKodu, @ParcaAdi, @Kategori, @Marka, @Model, @StokMiktari, @MinStokMiktari, @AlisFiyati, @SatisFiyati, @Birim, @Konum, @Aciklama, @Aktif);
                          SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, parca);
        }

        public async Task<bool> UpdateAsync(Parca parca)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"UPDATE Parcalar SET 
                          ParcaKodu = @ParcaKodu, ParcaAdi = @ParcaAdi, Kategori = @Kategori,
                          Marka = @Marka, Model = @Model, MinStokMiktari = @MinStokMiktari,
                          AlisFiyati = @AlisFiyati, SatisFiyati = @SatisFiyati, Birim = @Birim,
                          Konum = @Konum, Aciklama = @Aciklama, Aktif = @Aktif
                          WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, parca);
            return affected > 0;
        }

        public async Task<bool> UpdateStokAsync(int id, int miktar)
        {
            using var connection = new MySqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(
                "UPDATE Parcalar SET StokMiktari = StokMiktari + @Miktar WHERE Id = @Id",
                new { Id = id, Miktar = miktar });
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            // Soft delete
            var affected = await connection.ExecuteAsync("UPDATE Parcalar SET Aktif = FALSE WHERE Id = @Id", new { Id = id });
            return affected > 0;
        }

        public async Task<List<string>> GetKategorilerAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<string>(
                "SELECT DISTINCT Kategori FROM Parcalar WHERE Aktif = TRUE AND Kategori IS NOT NULL AND Kategori != '' ORDER BY Kategori");
            return result.ToList();
        }

        public async Task<bool> ParcaKoduMevcutMuAsync(string parcaKodu, int? haricId = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = haricId.HasValue
                ? "SELECT COUNT(*) FROM Parcalar WHERE ParcaKodu = @ParcaKodu AND Id != @Id"
                : "SELECT COUNT(*) FROM Parcalar WHERE ParcaKodu = @ParcaKodu";
            var count = await connection.ExecuteScalarAsync<int>(query, new { ParcaKodu = parcaKodu, Id = haricId ?? 0 });
            return count > 0;
        }
    }
}
