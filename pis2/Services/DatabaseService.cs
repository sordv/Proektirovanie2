using Npgsql;
using pis2.Models;
using System.Data;

namespace pis2.Services
{
    public class DatabaseService
    {
        private string connectionString;
        internal string ConnectionString => connectionString;

        public DatabaseService(string connection) { connectionString = connection; } // создание объекта для работы с БД

        // метод для авториации пользователя
        public bool LoginUser(string login, string password, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                error = "Заполните все поля!";
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
                            error = "Такого аккаунта не существует!";
                            return false;
                        }
                        string realPassword = reader["password"].ToString();
                        if (realPassword != password)
                        {
                            error = "Неверно введен пароль!";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        // метод для регистрации пользователя
        public bool RegisterUser(string login, string password, string passwordConfirm, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
            {
                error = "Заполните все поля!";
                return false;
            }

            if (password.Length < 8)
            {
                error = "Пароль должен содержать 8 и более символов!";
                return false;
            }

            if (password != passwordConfirm)
            {
                error = "Пароли не совпадают!";
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
                            error = "Такой логин уже занят!";
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
                        error = "Ошибка: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        // метод для получения всех гражданств и флагов (для отрисовки страницы)
        public (List<KeyValuePair<int, string>>, List<KeyValuePair<int, string>>) GetData()
        {
            var allCitizenships = new List<KeyValuePair<int, string>>();
            var allFlags = new List<KeyValuePair<int, string>>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM citizenships ORDER BY NAME ASC", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allCitizenships.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }

                using (var cmd = new NpgsqlCommand("SELECT id, value FROM flags ORDER BY VALUE ASC", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allFlags.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return (allCitizenships, allFlags);
        }

        // обновить информацию о пользователе
        public void UpdateUser(User user)
        {
            Type userType = typeof(User);

            string username = (string)userType.GetProperty("Login").GetValue(user);
            int citizenship = (int)userType.GetProperty("Citizenship").GetValue(user);
            int[] flags = (int[])userType.GetProperty("Flags").GetValue(user);
            DateTime? entry = (DateTime?)userType.GetProperty("Entry").GetValue(user);

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE users SET citizenship = @citizenship, flags = @flags, entry = @entry WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("citizenship", citizenship);
                    cmd.Parameters.AddWithValue("flags", flags);
                    cmd.Parameters.AddWithValue("entry", entry);
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // получить информацию о пользователе
        public (int Citizenship, int[] Flags, DateTime? EntryDate) GetUserData(string username)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT citizenship, flags, entry FROM users WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int citizenship = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                            int[] flags = Array.Empty<int>();
                            if (!reader.IsDBNull(1)) { flags = (int[])reader.GetValue(1); }
                            DateTime? entryDate = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2);
                            return (citizenship, flags, entryDate);
                        }
                    }
                }
            }
            return (-1, Array.Empty<int>(), null);
        }

        // создать пользователя
        public User GetUser(string username)
        {
            var (citizenship, flags, entry) = GetUserData(username);
            return new User(username, citizenship, flags, entry);
        }

        // метод для получения всех правил и текстов Roadmapitem (для генерации дорожной карты)
        public (List<Models.Rule>, Dictionary<int, string>) GetRoadmapData()
        {
            var rules = new List<Models.Rule>();
            var roadmapItems = new Dictionary<int, string>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                // запрос получения правил
                using (var cmd = new NpgsqlCommand("SELECT * FROM rules", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rules.Add(new Models.Rule
                        {
                            Id = reader.GetInt32(0),
                            Citizenship = reader.IsDBNull(1) ? Array.Empty<int>() : (int[])reader.GetValue(1),
                            Flag = reader.IsDBNull(2) ? Array.Empty<int>() : (int[])reader.GetValue(2),
                            BannedCitizenships = reader.IsDBNull(3) ? Array.Empty<int>() : (int[])reader.GetValue(3),
                            BannedFlags = reader.IsDBNull(4) ? Array.Empty<int>() : (int[])reader.GetValue(4),
                            RoadmapItem = reader.GetInt32(5),
                            Period = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                        });
                    }
                }
                // запрос получения значений roadmapitems
                using (var cmd = new NpgsqlCommand("SELECT id, value FROM roadmapitems", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roadmapItems[reader.GetInt32(0)] = reader.GetString(1);
                    }
                }
            }
            return (rules, roadmapItems);
        }

        // метод для редактирования БД (через админ панель)
        public DataTable ExecuteEditorRequest(string query)
        {
            var dataTable = new DataTable();

            using (var conn = new NpgsqlConnection(connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();

                if (query.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
                else
                {
                    cmd.ExecuteNonQuery();
                    // После изменения данных возвращаем обновленную таблицу
                    return ExecuteEditorRequest(GetSelectQueryForTable(GetTableNameFromQuery(query)));
                }
            }

            return dataTable;
        }

        private string GetTableNameFromQuery(string query)
        {
            if (query.Contains("citizenships")) return "citizenships";
            if (query.Contains("flags")) return "flags";
            if (query.Contains("roadmapitems")) return "roadmapitems";
            return string.Empty;
        }

        private string GetSelectQueryForTable(string tableName)
        {
            return $"SELECT * FROM {tableName} ORDER BY id ASC";
        }

        public DataTable ExecuteParameterizedQuery(string query, params NpgsqlParameter[] parameters)
        {
            var dataTable = new DataTable();

            using (var conn = new NpgsqlConnection(connectionString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }
    }
}