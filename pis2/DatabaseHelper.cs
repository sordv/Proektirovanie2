using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public bool LoginUser(string login, string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                errorMessage = "Заполните все поля!";
                return false;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", login);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            errorMessage = "Такого аккаунта не существует!";
                            return false;
                        }

                        string dbPassword = reader["password"].ToString();
                        if (dbPassword != password)
                        {
                            errorMessage = "Неверно введен пароль!";
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool RegisterUser(string login, string password, string passwordConfirm, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
            {
                errorMessage = "Заполните все поля!";
                return false;
            }

            if (password.Length < 8)
            {
                errorMessage = "Пароль должен содержать 8 и более символов!";
                return false;
            }

            if (password != passwordConfirm)
            {
                errorMessage = "Пароли не совпадают!";
                return false;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", login);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            errorMessage = "Такой логин уже занят!";
                            return false;
                        }
                    }
                }

                using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", conn))
                {
                    cmd.Parameters.AddWithValue("username", login);
                    cmd.Parameters.AddWithValue("password", password);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (PostgresException ex)
                    {
                        errorMessage = "Ошибка: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        public List<KeyValuePair<int, string>> GetData(string table)
        {
            var data = new List<KeyValuePair<int, string>>();
            string querry = "";

            switch (table)
            {
                case "citizenships":
                    querry = "SELECT id, name FROM citizenships";
                    break;
                case "conditions":
                    querry = "SELECT id, value FROM conditions";
                    break;
                case "targets":
                    querry = "SELECT id, name FROM targets";
                    break;
                default:
                    throw new ArgumentException("Incorrect table name!");
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(querry, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return data.OrderBy(kvp => kvp.Value).ToList(); ;
        }

        public void UpdateUser(string username, int citizenship, int[] conditions, DateTime? entry)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE users SET citizenship = @citizenship, conditions = @conditions, entry = @entry WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("citizenship", citizenship);
                    cmd.Parameters.AddWithValue("conditions", conditions);
                    cmd.Parameters.AddWithValue("entry", entry.HasValue ? (object)entry.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public (int Citizenship, int[] Conditions, DateTime? EntryDate) GetUserData(string username)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT citizenship, conditions, entry FROM users WHERE username = @username", conn))
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
                            DateTime? entryDate = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2);
                            return (citizenship, conditions, entryDate);
                        }
                    }
                }
            }
            return (-1, Array.Empty<int>(), null);
        }

        public List<Rule> GetRulesByTarget(int targetId)
        {
            var rules = new List<Rule>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM rules WHERE target = @target", conn))
                {
                    cmd.Parameters.AddWithValue("target", targetId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rule = new Rule
                            {
                                Id = reader.GetInt32(0),
                                Target = reader.GetInt32(1),
                                Citizenship = reader.GetFieldValue<int[]>(2),
                                Condition = reader.GetFieldValue<int[]>(3),
                                //RoadmapItem = reader.GetInt32(4)
                                RoadmapItem = reader.GetFieldValue<int[]>(4)
                            };
                            rules.Add(rule);
                        }
                    }
                }
            }
            return rules;
        }

        public List<KeyValuePair<int, string>> GetRoadmapItems(int[] itemIds)
        {
            var items = new List<KeyValuePair<int, string>>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, value FROM roadmapitems WHERE id = ANY(@ids)", conn))
                {
                    cmd.Parameters.AddWithValue("ids", itemIds);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }
            return items;
        }
    }

    public class Rule
    {
        public int Id { get; set; }
        public int Target { get; set; }
        public int[] Citizenship { get; set; }
        public int[] Condition { get; set; }
        public int[] RoadmapItem { get; set; }
    }
}

