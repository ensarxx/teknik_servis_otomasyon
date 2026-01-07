using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class CihazRepository
    {
        private readonly string _connectionString;

        public CihazRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<List<Cihaz>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Cihaz>("SELECT * FROM Cihazlar ORDER BY KayitTarihi DESC");
            return result.ToList();
        }

        public async Task<Cihaz?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cihaz>("SELECT * FROM Cihazlar WHERE Id = @Id", new { Id = id });
        }

        public async Task<List<Cihaz>> GetByMusteriIdAsync(int musteriId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Cihaz>(
                "SELECT * FROM Cihazlar WHERE MusteriId = @MusteriId ORDER BY KayitTarihi DESC", 
                new { MusteriId = musteriId });
            return result.ToList();
        }

        public async Task<int> InsertAsync(Cihaz cihaz)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"INSERT INTO Cihazlar (CihazTuru, Marka, Model, SeriNo, IMEI, MusteriId, Aciklama) 
                          VALUES (@CihazTuru, @Marka, @Model, @SeriNo, @IMEI, @MusteriId, @Aciklama);
                          SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, cihaz);
        }

        public async Task<bool> UpdateAsync(Cihaz cihaz)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"UPDATE Cihazlar SET 
                          CihazTuru = @CihazTuru, Marka = @Marka, Model = @Model, 
                          SeriNo = @SeriNo, IMEI = @IMEI, Aciklama = @Aciklama 
                          WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, cihaz);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync("DELETE FROM Cihazlar WHERE Id = @Id", new { Id = id });
            return affected > 0;
        }

        public async Task<List<Cihaz>> SearchAsync(string searchTerm)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT c.*, m.AdSoyad as MusteriAdi FROM Cihazlar c
                          INNER JOIN Musteriler m ON c.MusteriId = m.Id
                          WHERE c.Marka LIKE @Search OR c.Model LIKE @Search OR c.SeriNo LIKE @Search OR c.IMEI LIKE @Search
                          ORDER BY c.KayitTarihi DESC";
            var result = await connection.QueryAsync<Cihaz>(query, new { Search = $"%{searchTerm}%" });
            return result.ToList();
        }
    }
}
