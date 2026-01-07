using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeknikServisOtomasyon.Models;

namespace TeknikServisOtomasyon.Data.Repositories
{
    public class KullaniciRepository
    {
        private readonly string _connectionString;

        public KullaniciRepository()
        {
            _connectionString = DatabaseConnection.Instance.ConnectionString;
        }

        public async Task<List<Kullanici>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Kullanici>("SELECT * FROM Kullanicilar WHERE Aktif = TRUE ORDER BY AdSoyad");
            return result.ToList();
        }

        public async Task<Kullanici?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Kullanici>("SELECT * FROM Kullanicilar WHERE Id = @Id", new { Id = id });
        }

        public async Task<Kullanici?> LoginAsync(string kullaniciAdi, string sifre)
        {
            using var connection = new MySqlConnection(_connectionString);
            var user = await connection.QueryFirstOrDefaultAsync<Kullanici>(
                "SELECT * FROM Kullanicilar WHERE KullaniciAdi = @KullaniciAdi AND Sifre = @Sifre AND Aktif = TRUE",
                new { KullaniciAdi = kullaniciAdi, Sifre = sifre });

            if (user != null)
            {
                // Son giriş tarihini güncelle
                await connection.ExecuteAsync(
                    "UPDATE Kullanicilar SET SonGirisTarihi = @Tarih WHERE Id = @Id",
                    new { Tarih = DateTime.Now, Id = user.Id });
            }

            return user;
        }

        public async Task<int> InsertAsync(Kullanici kullanici)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"INSERT INTO Kullanicilar (KullaniciAdi, Sifre, AdSoyad, Email, Telefon, Rol, Aktif) 
                          VALUES (@KullaniciAdi, @Sifre, @AdSoyad, @Email, @Telefon, @Rol, @Aktif);
                          SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(query, kullanici);
        }

        public async Task<bool> UpdateAsync(Kullanici kullanici)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = @"UPDATE Kullanicilar SET 
                          KullaniciAdi = @KullaniciAdi, AdSoyad = @AdSoyad, 
                          Email = @Email, Telefon = @Telefon, Rol = @Rol, Aktif = @Aktif 
                          WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, kullanici);
            return affected > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int id, string newPassword)
        {
            using var connection = new MySqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(
                "UPDATE Kullanicilar SET Sifre = @Sifre WHERE Id = @Id",
                new { Id = id, Sifre = newPassword });
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            // Soft delete
            var affected = await connection.ExecuteAsync("UPDATE Kullanicilar SET Aktif = FALSE WHERE Id = @Id", new { Id = id });
            return affected > 0;
        }

        public async Task<List<Kullanici>> GetTeknikerlerAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var result = await connection.QueryAsync<Kullanici>(
                "SELECT * FROM Kullanicilar WHERE Aktif = TRUE AND (Rol = 'Tekniker' OR Rol = 'Admin') ORDER BY AdSoyad");
            return result.ToList();
        }

        public async Task<bool> KullaniciAdiMevcutMuAsync(string kullaniciAdi, int? haricId = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            var query = haricId.HasValue
                ? "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = @KullaniciAdi AND Id != @Id"
                : "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi = @KullaniciAdi";
            var count = await connection.ExecuteScalarAsync<int>(query, new { KullaniciAdi = kullaniciAdi, Id = haricId ?? 0 });
            return count > 0;
        }
    }
}
