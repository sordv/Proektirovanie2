using System;
using System.Collections.Generic;
using Npgsql;

namespace pis2
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<KeyValuePair<int, string>> GetCitizenships()
        {
            var citizenships = new List<KeyValuePair<int, string>>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM citizenship", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        citizenships.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return citizenships;
        }

        public List<KeyValuePair<int, string>> GetConditions()
        {
            var conditions = new List<KeyValuePair<int, string>>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, value FROM conditions", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        conditions.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return conditions;
        }

        public void UpdateUser(string username, int citizenship, int[] conditions)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE users SET citizenship = @citizenship, conditions = @conditions WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("citizenship", citizenship);
                    cmd.Parameters.AddWithValue("conditions", conditions);
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public (int Citizenship, int[] Conditions) GetUserData(string username)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT citizenship, conditions FROM users WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int citizenship = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                            int[] conditions = Array.Empty<int>();
                            if (!reader.IsDBNull(1))
                            {
                                var conditionsStrings = (string[])reader.GetValue(1);
                                conditions = conditionsStrings.Select(int.Parse).ToArray();
                            }
                            return (citizenship, conditions);
                        }
                    }
                }
            }
            return (-1, Array.Empty<int>());
        }
    }
}