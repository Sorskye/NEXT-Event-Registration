using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories.SqlServer
{
    public class SqlServerLocationRepository : ILocationRepository
    {
        private readonly string connectionString;

        public SqlServerLocationRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int GetOrCreateLocationId(string locationName)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string trimmedLocationName = locationName.Trim();

            using SqlCommand findCmd = new SqlCommand("SELECT ID FROM Location WHERE Name = @Name", conn);
            findCmd.Parameters.AddWithValue("@Name", trimmedLocationName);

            object existingId = findCmd.ExecuteScalar();

            if (existingId != null)
            {
                return Convert.ToInt32(existingId);
            }

            using SqlCommand insertCmd = new SqlCommand(
                "INSERT INTO Location (Name) VALUES (@Name); SELECT CONVERT(int, SCOPE_IDENTITY());",
                conn);
            insertCmd.Parameters.AddWithValue("@Name", trimmedLocationName);

            return (int)insertCmd.ExecuteScalar();
        }
    }
}
