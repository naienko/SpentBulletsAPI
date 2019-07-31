using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace SpentBulletsAPI.Models
{
    public class Context
    {
        public string ConnectionString { get; set; }

        public Context(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
