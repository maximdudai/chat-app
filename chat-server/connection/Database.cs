using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Security;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace chat_server.connection
{
    internal class Database
    {
        private MySqlConnection connection;

        // Error codes
        private string connectionString = string.Format(
                   "server={0};uid={1};pwd={2};database={3}",
                   "localhost",
                   "root",
                   "",
                   "chatapp"
               );


        //Constructor
        public Database()
        {
            if(this.connection != null)
            {
                this.connection.Close();
            }

            // create connection
            this.connection = new MySqlConnection(connectionString);

            //open connection
            this.connection.Open();
        }

        public async Task<int?> LoginAsync(string username, string password)
        {
            try
            {
                // SQL query to retrieve the user data
                string query = "SELECT ID, saltedPasswordHash, salt FROM users WHERE username = @username";

                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Get the column indices
                                int idColumnIndex = reader.GetOrdinal("ID");
                                int hashColumnIndex = reader.GetOrdinal("saltedPasswordHash");
                                int saltColumnIndex = reader.GetOrdinal("salt");

                                // Retrieve the values
                                int userId = reader.GetInt32(idColumnIndex);
                                string storedHash = reader.GetString(hashColumnIndex);
                                string storedSalt = reader.GetString(saltColumnIndex);

                                // Convert the stored salt from base64
                                byte[] saltBytes = Convert.FromBase64String(storedSalt);

                                // Generate the hash of the provided password using the stored salt
                                byte[] passwordHash = ComputeHash(password, saltBytes, 10000, 32);
                                string passwordHashBase64 = Convert.ToBase64String(passwordHash);

                                // Compare the generated hash with the stored hash
                                if (passwordHashBase64 == storedHash)
                                {
                                    return userId; // Password matches, return user ID
                                }
                                else
                                {
                                    return null; // Password does not match
                                }
                            }
                            else
                            {
                                return null; // No user found with the provided username
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in LoginAsync: " + ex.Message);
                throw;
            }
        }



        public async Task<int?> RegisterAsync(string username, string password, string email)
        {
            string queryCheckUsername = "SELECT COUNT(*) FROM users WHERE username = @username;";

            string queryInsert = "INSERT INTO users (username, saltedPasswordHash, salt, email) VALUES (@username, @passwordHash, @salt, @email);";

            string queryLastId = "SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand(queryCheckUsername, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        int userExists = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        if (userExists > 0)
                        {
                            return null;
                        }
                    }

                    byte[] salt = GenerateSalt(16); // Generates a unique salt
                    byte[] hashedPassword = ComputeHash(password, salt, 10000, 32); // Computes hash of the password

                    string saltBase64 = Convert.ToBase64String(salt);
                    string hashedPasswordBase64 = Convert.ToBase64String(hashedPassword);

                    using (var cmd = new MySqlCommand(queryInsert, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", Convert.ToBase64String(hashedPassword));
                        cmd.Parameters.AddWithValue("@salt", Convert.ToBase64String(salt));
                        cmd.Parameters.AddWithValue("@email", email);

                        await cmd.ExecuteNonQueryAsync(); // Execute the insert command

                        cmd.CommandText = queryLastId;
                        var result = await cmd.ExecuteScalarAsync(); // Execute scalar returns the first column of the first row
                        if (result != null)
                        {
                            return Convert.ToInt32(result); // Convert the result to integer and return
                        }
                        return null; // Return null if no ID was found (should not happen in normal circumstances)
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[DB] An error occurred: " + ex.Message);
                throw;
            }
        }


        public async Task InsertChatLog(int id, string username, string chat)
        {
            string queryInsert = "INSERT INTO messages (userid, username, message) VALUES (@id, @username, @chat);";
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var cmd = new MySqlCommand(queryInsert, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@chat", chat);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                Console.WriteLine("[DB] Chat log inserted successfully.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[DB] MySQL error occurred: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DB] An unexpected error occurred: " + ex.Message);
                throw;
            }
        }

        // Method to generate a salt
        private static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Method to compute the hash of a password with a salt
        private static byte[] ComputeHash(string password, byte[] salt, int iterations, int hashSize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(hashSize);
            }
        }
    }

}
