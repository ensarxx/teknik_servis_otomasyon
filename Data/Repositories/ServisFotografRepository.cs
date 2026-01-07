using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class ServisFotografRepository
    {
        private readonly string _connectionString;

        public ServisFotografRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<List<ServisFotograf>> GetByServisIdAsync(int servisId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = "SELECT * FROM servis_fotograflar WHERE ServisId = @ServisId ORDER BY YuklenmeTarihi DESC";
            var result = await connection.QueryAsync<ServisFotograf>(query, new { ServisId = servisId });
            return result.AsList();
        }

        public async Task<ServisFotograf?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = "SELECT * FROM servis_fotograflar WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<ServisFotograf>(query, new { Id = id });
        }

        public async Task<int> AddAsync(ServisFotograf fotograf)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"INSERT INTO servis_fotograflar 
                (ServisId, DosyaAdi, DosyaYolu, Aciklama, FotografTipi, YuklenmeTarihi) 
                VALUES (@ServisId, @DosyaAdi, @DosyaYolu, @Aciklama, @FotografTipi, @YuklenmeTarihi);
                SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, fotograf);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = "DELETE FROM servis_fotograflar WHERE Id = @Id";
            await connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<int> GetCountByServisIdAsync(int servisId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = "SELECT COUNT(*) FROM servis_fotograflar WHERE ServisId = @ServisId";
            return await connection.ExecuteScalarAsync<int>(query, new { ServisId = servisId });
        }
    }
}
