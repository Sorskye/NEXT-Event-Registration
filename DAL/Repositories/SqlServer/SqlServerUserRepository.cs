using System;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories.SqlServer
{
    public class SqlServerUserRepository : IUserRepository
    {
        private readonly string connectionString;

        public SqlServerUserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT ID, Auth0_id, Name, Email, Role, Created_at FROM Users WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadUser(reader);
                        }
                    }
                }
            }

            return null;
        }

        public bool IsUserAdmin(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Role FROM Users WHERE ID = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return string.Equals(result.ToString(), "admin", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }

            return false;
        }

        public User GetUserById(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT ID, Auth0_id, Name, Email, Role, Created_at FROM Users WHERE ID = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadUser(reader);
                        }
                    }
                }
            }

            return null;
        }

        private static User ReadUser(SqlDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["ID"]),
                Auth0Id = reader["Auth0_id"] == DBNull.Value ? null : reader["Auth0_id"].ToString(),
                Name = reader["Name"] == DBNull.Value ? null : reader["Name"].ToString(),
                Email = reader["Email"] == DBNull.Value ? null : reader["Email"].ToString(),
                Role = reader["Role"] == DBNull.Value ? "member" : reader["Role"].ToString() ?? "member",
                CreatedAt = reader["Created_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["Created_at"])
            };
        }
    }
}

