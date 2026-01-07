using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class MusteriRepository
    {
        private readonly string _connectionString;

        public MusteriRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<List<Musteri>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Musteri>("SELECT * FROM Musteriler WHERE Aktif = TRUE ORDER BY AdSoyad");
            return result.ToList();
        }

        public async Task<Musteri?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Musteri>("SELECT * FROM Musteriler WHERE Id = @Id", new { Id = id });
        }

        public async Task<List<Musteri>> SearchAsync(string searchTerm)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"SELECT * FROM Musteriler 
                          WHERE Aktif = TRUE AND (AdSoyad LIKE @Search OR Telefon LIKE @Search OR Email LIKE @Search)
                          ORDER BY AdSoyad";
            var result = await connection.QueryAsync<Musteri>(query, new { Search = $"%{searchTerm}%" });
            return result.ToList();
        }

        public async Task<int> InsertAsync(Musteri musteri)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"INSERT INTO Musteriler (AdSoyad, Telefon, Telefon2, Email, Adres, TCKimlikNo, Notlar, Aktif) 
                          VALUES (@AdSoyad, @Telefon, @Telefon2, @Email, @Adres, @TCKimlikNo, @Notlar, @Aktif);
                          SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, musteri);
        }

        public async Task<bool> UpdateAsync(Musteri musteri)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"UPDATE Musteriler SET 
                          AdSoyad = @AdSoyad, Telefon = @Telefon, Telefon2 = @Telefon2, 
                          Email = @Email, Adres = @Adres, TCKimlikNo = @TCKimlikNo, 
                          Notlar = @Notlar, Aktif = @Aktif 
                          WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, musteri);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            // Soft delete
            var affected = await connection.ExecuteAsync("UPDATE Musteriler SET Aktif = FALSE WHERE Id = @Id", new { Id = id });
            return affected > 0;
        }

        public async Task<Musteri?> GetByPhoneAsync(string phone)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Musteri>(
                "SELECT * FROM Musteriler WHERE Telefon = @Phone OR Telefon2 = @Phone", 
                new { Phone = phone });
        }

        public async Task<int> GetCountAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Musteriler WHERE Aktif = TRUE");
        }
    }
}
