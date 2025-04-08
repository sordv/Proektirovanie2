using System.Data;
using Npgsql;

namespace pis2
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connection) { connectionString = connection; }

        public bool LoginUser(string login, string password, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                error = "��������� ��� ����!";
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
                            error = "������ �������� �� ����������!";
                            return false;
                        }
                        string realPassword = reader["password"].ToString();
                        if (realPassword != password)
                        {
                            error = "������� ������ ������!";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool RegisterUser(string login, string password, string passwordConfirm, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
            {
                error = "��������� ��� ����!";
                return false;
            }

            if (password.Length < 8)
            {
                error = "������ ������ ��������� 8 � ����� ��������!";
                return false;
            }

            if (password != passwordConfirm)
            {
                error = "������ �� ���������!";
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
                            error = "����� ����� ��� �����!";
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
                        error = "������: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        public List<KeyValuePair<int, string>> GetData(string table)
        {
            string query = "";
            var data = new List<KeyValuePair<int, string>>();

            switch (table)
            {
                case "citizenships":
                    query = "SELECT id, name FROM citizenships";
                    break;
                case "flags":
                    query = "SELECT id, value FROM flags";
                    break;
                default:
                    throw new ArgumentException("Incorrect table name!");
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return data.OrderBy(item => item.Value).ToList();
        }

        public void UpdateUser(string username, int citizenship, int[] flags, DateTime? entry)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE users SET citizenship = @citizenship, flags = @flags, entry = @entry WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("citizenship", citizenship);
                    cmd.Parameters.AddWithValue("flags", flags);
                    cmd.Parameters.AddWithValue("entry", entry.HasValue ? (object)entry.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

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

        public User GetUser(string username)
        {
            var (citizenship, flags, entry) = GetUserData(username);
            return new User(username, citizenship, flags, entry);
        }
    }
}