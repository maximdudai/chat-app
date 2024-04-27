using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Security;
using System.Net;
using System.Net.Sockets;

namespace chat_server.connection
{
    internal class Database
    {
        private MySqlConnection connection;

        // Error codes
        const int CANNOT_CONNECT_TO_THE_SERVER = 0;
        const int INVALID_USERNAME_PASSWORD = 1045;

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
        
        //method to send any query to the database
        public async Task<int?> LoginAsync(string username, string password)
        {
            string query = "SELECT ID FROM users WHERE username = @username AND password = @password";

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Get the column index for "ID"
                            int idColumnIndex = reader.GetOrdinal("ID");

                            // Use the index to retrieve the ID
                            return reader.GetInt32(idColumnIndex); 
                        }
                        else
                        {
                            return null; // No user found
                        }
                    }
                }
            }
        }

        public async Task<int?> RegisterAsync(string username, string password, string email)
        {
            // SQL query to check for existing username
            string queryCheckUsername = "SELECT COUNT(*) FROM users WHERE username = @username;";

            // SQL query to insert data
            string queryInsert = "INSERT INTO users (username, password, email) VALUES (@username, @password, @email);";

            // SQL query to retrieve the last inserted ID
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

                    using (var cmd = new MySqlCommand(queryInsert, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
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
    }

}
