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
                    Console.WriteLine(email.Trim());
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

        public bool CreateUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Users (Auth0_id, Name, Email, Role, Created_at)
                                 VALUES (@Auth0Id, @Name, @Email, @Role, @CreatedAt)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Auth0Id", (object?)user.Auth0Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", (object?)user.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
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

        public bool CreateUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO Users (Auth0_id,Name,Email,Role,Created_at) VALUES (@Auth0_id ,@Name ,@Email,@Role, @Date);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Auth0_id", user.Auth0Id);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            
        }
    }
}

