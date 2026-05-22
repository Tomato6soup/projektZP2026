using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ResearchHub.Database
{
    // Klasa odpowiedzialna za nawiązanie bezpiecznego połączenia z bazą SQL Server
    public class DbConnection
    {
        private readonly string _connectionString;

        public DbConnection()
        {
            // Przykładowy connection string dla SQL Server Express
            _connectionString = "Server=.\\SQLEXPRESS;Database=BazaPublikacjiUBB;Trusted_Connection=True;TrustServerCertificate=True;";
        }
        //Server=.\\SQLEXPRESS;Database=BazaPublikacjiUBB;Trusted_Connection=True;TrustServerCertificate=True;
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Test połączenia z bazą danych
        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}