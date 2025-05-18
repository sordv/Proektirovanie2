using Npgsql;
using pis2.Services;
using pis2.Views;
using System.Data;

namespace pis2.Controllers
{
    public class EditorFormController
    {
        private readonly EditorFormView _view;
        private readonly DatabaseService _dbService;

        public EditorFormController(EditorFormView view, string connectionString)
        {
            _view = view;
            _dbService = new DatabaseService(connectionString);

            AddCitizenshipsTab();
            AddFlagsTab();
            AddRoadmapItemsTab();
            AddRulesTab();
        }

        private void AddTabWithTable(string tabName, string tableName)
        {
            try
            {
                var tab = new TabPage(tabName);
                var panel = new Panel { Dock = DockStyle.Fill };

                var dataGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AllowUserToAddRows = true,
                    AllowUserToDeleteRows = true,
                    ReadOnly = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    EditMode = DataGridViewEditMode.EditOnEnter
                };

                var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
                var saveButton = new Button { Text = "Сохранить изменения", Dock = DockStyle.Right, Width = 150 };
                saveButton.Click += (s, e) => SaveChanges(dataGridView, tableName);

                buttonPanel.Controls.Add(saveButton);
                panel.Controls.Add(dataGridView);
                panel.Controls.Add(buttonPanel);
                tab.Controls.Add(panel);
                _view.TabControl.TabPages.Add(tab);

                LoadTableData(dataGridView, tableName);
            }
            catch (Exception ex)
            {
                _view.AddConsoleMessage($"Ошибка создания вкладки {tabName}: {ex.Message}");
            }
        }

        private void AddCitizenshipsTab()
        {
            AddTabWithTable("Гражданства", "citizenships");
        }

        private void AddFlagsTab()
        {
            AddTabWithTable("Флаги", "flags");
        }

        private void AddRoadmapItemsTab()
        {
            AddTabWithTable("Пункты дорожной карты", "roadmapitems");
        }

        private void AddRulesTab()
        {
            try
            {
                var tab = new TabPage("Правила");
                var panel = new Panel { Dock = DockStyle.Fill };

                var dataGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AllowUserToAddRows = true,
                    AllowUserToDeleteRows = true,
                    ReadOnly = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    EditMode = DataGridViewEditMode.EditOnEnter
                };

                var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
                var saveButton = new Button { Text = "Сохранить изменения", Dock = DockStyle.Right, Width = 150 };
                saveButton.Click += (s, e) => SaveChanges(dataGridView, "rules");

                buttonPanel.Controls.Add(saveButton);
                panel.Controls.Add(dataGridView);
                panel.Controls.Add(buttonPanel);
                tab.Controls.Add(panel);
                _view.TabControl.TabPages.Add(tab);

                LoadTableData(dataGridView, "rules");
            }
            catch (Exception ex)
            {
                _view.AddConsoleMessage($"Ошибка создания вкладки правил: {ex.Message}");
            }
        }

        private void SaveChanges(DataGridView dataGridView, string tableName)
        {
            try
            {
                var dataTable = (DataTable)dataGridView.DataSource;
                var changes = dataTable.GetChanges();

                if (changes != null)
                {
                    using (var conn = new NpgsqlConnection(_dbService.ConnectionString))
                    {
                        conn.Open();
                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                foreach (DataRow row in changes.Rows)
                                {
                                    if (row.RowState == DataRowState.Added)
                                    {
                                        InsertRow(row, tableName, conn);
                                    }
                                    else if (row.RowState == DataRowState.Modified)
                                    {
                                        UpdateRow(row, tableName, conn);
                                    }
                                    else if (row.RowState == DataRowState.Deleted)
                                    {
                                        DeleteRow(row, tableName, conn, DataRowVersion.Original);
                                    }
                                }
                                transaction.Commit();
                                _view.AddConsoleMessage($"Изменения в таблице {tableName} сохранены");
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                _view.AddConsoleMessage($"Ошибка при сохранении: {ex.Message}");
                                throw;
                            }
                        }
                    }
                }

                LoadTableData(dataGridView, tableName);
            }
            catch (Exception ex)
            {
                _view.AddConsoleMessage($"Ошибка сохранения изменений: {ex.Message}");
            }
        }

        private void InsertRow(DataRow row, string tableName, NpgsqlConnection conn)
        {
            if (tableName == "rules")
            {
                string query = @"INSERT INTO rules 
                    (citizenships, flags, bannedcitizenships, bannedflags, roadmapitem, period) 
                    VALUES (
                    string_to_array(@citizenships, ',')::int[],
                    string_to_array(@flags, ',')::int[],
                    string_to_array(@bannedcitizenships, ',')::int[],
                    string_to_array(@bannedflags, ',')::int[],
                    @roadmapitem, @period)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@citizenships", row["citizenships"] ?? "");
                    cmd.Parameters.AddWithValue("@flags", row["flags"] ?? "");
                    cmd.Parameters.AddWithValue("@bannedcitizenships", row["bannedcitizenships"] ?? "");
                    cmd.Parameters.AddWithValue("@bannedflags", row["bannedflags"] ?? "");
                    cmd.Parameters.AddWithValue("@roadmapitem", row["roadmapitem"]);
                    cmd.Parameters.AddWithValue("@period", row["period"]);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                var columns = string.Join(", ", row.Table.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName != "id")
                    .Select(c => c.ColumnName));

                var values = string.Join(", ", row.Table.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName != "id")
                    .Select(c => $"@p{c.Ordinal}"));

                string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        if (column.ColumnName != "id")
                        {
                            cmd.Parameters.AddWithValue($"@p{column.Ordinal}", row[column] ?? DBNull.Value);
                        }
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateRow(DataRow row, string tableName, NpgsqlConnection conn)
        {
            if (tableName == "rules")
            {
                string query = @"UPDATE rules SET 
                    citizenships = string_to_array(@citizenships, ',')::int[],
                    flags = string_to_array(@flags, ',')::int[],
                    bannedcitizenships = string_to_array(@bannedcitizenships, ',')::int[],
                    bannedflags = string_to_array(@bannedflags, ',')::int[],
                    roadmapitem = @roadmapitem,
                    period = @period
                    WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@citizenships", row["citizenships"] ?? "");
                    cmd.Parameters.AddWithValue("@flags", row["flags"] ?? "");
                    cmd.Parameters.AddWithValue("@bannedcitizenships", row["bannedcitizenships"] ?? "");
                    cmd.Parameters.AddWithValue("@bannedflags", row["bannedflags"] ?? "");
                    cmd.Parameters.AddWithValue("@roadmapitem", row["roadmapitem"]);
                    cmd.Parameters.AddWithValue("@period", row["period"]);
                    cmd.Parameters.AddWithValue("@id", row["id"]);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                var setClause = string.Join(", ", row.Table.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName != "id")
                    .Select(c => $"{c.ColumnName} = @p{c.Ordinal}"));

                string query = $"UPDATE {tableName} SET {setClause} WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        if (column.ColumnName != "id")
                        {
                            cmd.Parameters.AddWithValue($"@p{column.Ordinal}", row[column] ?? DBNull.Value);
                        }
                    }
                    cmd.Parameters.AddWithValue("@id", row["id"]);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteRow(DataRow row, string tableName, NpgsqlConnection conn, DataRowVersion version)
        {
            string query = $"DELETE FROM {tableName} WHERE id = @id";
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", row["id", version]);
                cmd.ExecuteNonQuery();
            }
        }

        private void LoadTableData(DataGridView dataGridView, string tableName)
        {
            try
            {
                bool allowAdd = dataGridView.AllowUserToAddRows;
                bool allowDelete = dataGridView.AllowUserToDeleteRows;

                string query = tableName == "rules" ?
                    @"SELECT id, array_to_string(citizenships, ',') as citizenships,
                    array_to_string(flags, ',') as flags,
                    array_to_string(bannedcitizenships, ',') as bannedcitizenships,
                    array_to_string(bannedflags, ',') as bannedflags,
                    roadmapitem,
                    period
                    FROM rules ORDER BY id ASC" :
                    $"SELECT * FROM {tableName} ORDER BY id ASC";

                var dataTable = _dbService.ExecuteEditorRequest(query);

                DataTable editableTable = new DataTable();
                foreach (DataColumn column in dataTable.Columns)
                {
                    var newColumn = new DataColumn(column.ColumnName, column.DataType)
                    {
                        Caption = column.Caption,
                        DefaultValue = column.DefaultValue,
                        MaxLength = column.MaxLength,
                        ReadOnly = false
                    };
                    editableTable.Columns.Add(newColumn);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    editableTable.ImportRow(row);
                }

                dataGridView.DataSource = null;
                dataGridView.DataSource = editableTable;

                dataGridView.AllowUserToAddRows = allowAdd;
                dataGridView.AllowUserToDeleteRows = allowDelete;
                dataGridView.ReadOnly = false;
            }
            catch (Exception ex)
            {
                _view.AddConsoleMessage($"Ошибка загрузки таблицы {tableName}: {ex.Message}");
            }
        }
    }
}