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

        public void JoinProject(int projektId, int userId)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO dbo.ProjektStudenci (ProjektID, StudentID) VALUES (@ProjId, @UserId)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProjId", projektId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LeaveProject(int projektId, int userId)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM dbo.ProjektStudenci WHERE ProjektID = @ProjId AND StudentID = @UserId";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProjId", projektId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}