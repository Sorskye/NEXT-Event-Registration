using System;
using DAL.Models;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories
{
    public class UserRepository_SQLServer
    {
        private readonly string connectionString;

        public UserRepository_SQLServer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public User GetUserByEmailAndPassword(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM [User] WHERE Email = @Email AND PasswordHash = @Password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email.Trim());
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Role = (bool)reader["Role"],
                                Created_at = (DateTime)reader["Created_at"]
                            };
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

                string query = "SELECT Role FROM [User] WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return (bool)result;
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

                string query = "SELECT * FROM [User] WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                Role = (bool)reader["Role"],
                                Created_at = (DateTime)reader["Created_at"]
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}

