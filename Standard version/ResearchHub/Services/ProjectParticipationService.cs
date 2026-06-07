using Microsoft.Data.SqlClient;
using ResearchHub.Database;

namespace ResearchHub.Services
{
    public class ProjectParticipationService
    {
        private readonly DbConnection _db;

        public ProjectParticipationService(DbConnection db)
        {
            _db = db;
        }

        public async Task JoinProjectAsync(int projektId, int userId)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                string query = "INSERT INTO dbo.ProjektStudenci (ProjektID, StudentID) VALUES (@ProjId, @UserId)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProjId", projektId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task LeaveProjectAsync(int projektId, int userId)
        {
            using (var conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                string query = "DELETE FROM dbo.ProjektStudenci WHERE ProjektID = @ProjId AND StudentID = @UserId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProjId", projektId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}