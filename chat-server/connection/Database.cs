using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace chat_server.connection
{
    internal class Database
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        string connectionString;

        const int CANNOT_CONNECT_TO_THE_SERVER = 0;
        const int INVALID_USERNAME_PASSWORD = 1045;

        //Constructor
        public Database()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "chat-app";
            uid = "root";
            password = "";

            connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};";

            connection = new MySqlConnection(connectionString);
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
                        Console.WriteLine("Cannot connect to server. Contact administrator");
                        break;
                    case INVALID_USERNAME_PASSWORD: // Error code for invalid username/password
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                    default:
                        Console.WriteLine("An error occurred: " + ex.Message);
                        break;
                }
                return false;
            }
        }


        //Close connection
        private bool CloseConnection()
        {
            // confirm if connection is null before closing
            if(connection == null)
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
                Console.WriteLine("An error occurred: " + ex.Message);
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
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        
        //Select statement
        //public List<string>[] Select() { }
    }
}
