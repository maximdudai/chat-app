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

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case CANNOT_CONNECT_TO_THE_SERVER: // Error code for cannot connect to server
                        Console.WriteLine("[DB] Cannot connect to server. Contact administrator");
                        break;
                    case INVALID_USERNAME_PASSWORD: // Error code for invalid username/password
                        Console.WriteLine("[DB] Invalid username/password, please try again");
                        break;
                    default:
                        Console.WriteLine("[DB] An error occurred: " + ex.Message);
                        break;
                }
                return false;
            }
        }


        //Close connection
        private bool CloseConnection()
        {
            // confirm if connection is null before closing
            if (connection == null)
            {
                return true;
            }

            try
            {
                // close the connection
                connection.Close();
                return true;
            }
            // catch any exception that may occur
            catch (MySqlException ex)
            {
                Console.WriteLine("[DB] An error occurred: " + ex.Message);
                return false;
            }
        }

        //method to send any query to the database
        public void sendDatabasQuery(string query)
        {
            try
            {
                // confirm if connection is open before executing query
                if (this.OpenConnection() == false)
                    this.OpenConnection();

                // create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[DB] An error occurred: " + ex.Message);
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            string query = "SELECT username, password FROM users WHERE username = @username AND password = @password";

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            string query = "INSERT INTO users (username, password, email) VALUES (@username, @password, @email)";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@email", email);

                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[DB] An error occurred: " + ex.Message);
                return false;
            }
        }
    }

}
