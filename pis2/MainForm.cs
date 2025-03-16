using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;

namespace pis2
{
    public partial class MainForm : Form
    {
        private bool isLoggedIn = false;
        private string connectionString = "Host=localhost;Username=postgres;Password=admin;Database=pis";

        private Button buttonTabLogin;
        private Button buttonTabRegister;
        private Panel panelLogin;
        private Panel panelRegister;
        private TextBox textboxLoginLogin;
        private TextBox textboxLoginPassword;
        private TextBox textboxRegisterLogin;
        private TextBox textboxRegisterPassword;
        private TextBox textboxRegisterPasswordConfirm;
        private Button buttonLogin;
        private Button buttonRegister;
        private Label labelLoginError;
        private Label labelRegisterError;

        private Panel panelLoggedIn;
        private Button buttonLogout;
        private Label labelInfo;
        private ComboBox comboBoxCitizenship;
        private List<CheckBox> checkBoxesConditions;
        private Button buttonSave;
        private DatabaseHelper dbHelper;

        public MainForm()
        {
            dbHelper = new DatabaseHelper(connectionString);
            InitializeForm();
            ShowLoginPanel();
        }

        private void InitializeForm()
        {
            this.Text = "Проектирование информационных систем";
            this.Size = new Size(800, 500);
            this.MinimumSize = new Size(500, 300);
            this.Resize += MainForm_Resize;

            // PANEL MENU
            Panel panelTabs = new Panel();
            panelTabs.Dock = DockStyle.Top;
            panelTabs.Height = 40;
            this.Controls.Add(panelTabs);

            buttonTabLogin = new Button();
            buttonTabLogin.Text = "Вход";
            buttonTabLogin.Dock = DockStyle.Left;
            buttonTabLogin.Width = this.ClientSize.Width / 2;
            buttonTabLogin.Click += buttonTabLogin_Click;
            panelTabs.Controls.Add(buttonTabLogin);

            buttonTabRegister = new Button();
            buttonTabRegister.Text = "Регистрация";
            buttonTabRegister.Dock = DockStyle.Right;
            buttonTabRegister.Width = this.ClientSize.Width / 2;
            buttonTabRegister.Click += buttonTabRegister_Click;
            panelTabs.Controls.Add(buttonTabRegister);

            // LOGIN & REGISTER PANELS
            panelLogin = new Panel();
            panelLogin.Dock = DockStyle.Fill;
            this.Controls.Add(panelLogin);

            panelRegister = new Panel();
            panelRegister.Dock = DockStyle.Fill;
            this.Controls.Add(panelRegister);

            // LOGIN & REGISTER ERROR LABELS
            labelLoginError = new Label();
            labelLoginError.Location = new Point(50, 50);
            labelLoginError.Size = new Size(300, 20);
            labelLoginError.ForeColor = Color.Red;
            labelLoginError.TextAlign = ContentAlignment.MiddleCenter;
            panelLogin.Controls.Add(labelLoginError);

            labelRegisterError = new Label();
            labelRegisterError.Location = new Point(50, 50);
            labelRegisterError.Size = new Size(300, 20);
            labelRegisterError.ForeColor = Color.Red;
            labelRegisterError.TextAlign = ContentAlignment.MiddleCenter;
            panelRegister.Controls.Add(labelRegisterError);

            // LOGIN & REGISTER TEXTBOXES
            textboxLoginLogin = new TextBox();
            textboxLoginLogin.Location = new Point(50, 80);
            textboxLoginLogin.Size = new Size(200, 20);
            textboxLoginLogin.PlaceholderText = "Логин";
            panelLogin.Controls.Add(textboxLoginLogin);

            textboxLoginPassword = new TextBox();
            textboxLoginPassword.Location = new Point(50, 110);
            textboxLoginPassword.Size = new Size(200, 20);
            textboxLoginPassword.PlaceholderText = "Пароль";
            textboxLoginPassword.UseSystemPasswordChar = true;
            panelLogin.Controls.Add(textboxLoginPassword);

            textboxRegisterLogin = new TextBox();
            textboxRegisterLogin.Location = new Point(50, 80);
            textboxRegisterLogin.Size = new Size(200, 20);
            textboxRegisterLogin.PlaceholderText = "Логин";
            panelRegister.Controls.Add(textboxRegisterLogin);

            textboxRegisterPassword = new TextBox();
            textboxRegisterPassword.Location = new Point(50, 110);
            textboxRegisterPassword.Size = new Size(200, 20);
            textboxRegisterPassword.PlaceholderText = "Пароль";
            textboxRegisterPassword.UseSystemPasswordChar = true;
            panelRegister.Controls.Add(textboxRegisterPassword);

            textboxRegisterPasswordConfirm = new TextBox();
            textboxRegisterPasswordConfirm.Location = new Point(50, 140);
            textboxRegisterPasswordConfirm.Size = new Size(200, 20);
            textboxRegisterPasswordConfirm.PlaceholderText = "Повторите пароль";
            textboxRegisterPasswordConfirm.UseSystemPasswordChar = true;
            panelRegister.Controls.Add(textboxRegisterPasswordConfirm);

            // LOGIN & REGISTER BUTTONS
            buttonLogin = new Button();
            buttonLogin.Location = new Point(50, 140);
            buttonLogin.Size = new Size(140, 30);
            buttonLogin.Text = "Войти";
            buttonLogin.Click += buttonLogin_Click;
            panelLogin.Controls.Add(buttonLogin);

            buttonRegister = new Button();
            buttonRegister.Location = new Point(50, 170);
            buttonRegister.Size = new Size(140, 30);
            buttonRegister.Text = "Зарегистрироваться";
            buttonRegister.Click += buttonRegister_Click;
            panelRegister.Controls.Add(buttonRegister);

            // LOGIN & REGISTRATION CONTENT ALIGN
            CenterPanelContent(panelLogin);
            CenterPanelContent(panelRegister);

            // LOGGED IN PANEL
            panelLoggedIn = new Panel();
            panelLoggedIn.Dock = DockStyle.Fill;
            this.Controls.Add(panelLoggedIn);

            InitializeLoggedInPanel();
        }

        private void InitializeLoggedInPanel()
        {
            //panelLoggedIn.Controls.Clear();

            // FIX
            labelInfo = new Label();
            labelInfo.Text = "Укажите информацию о себе";
            labelInfo.Location = new Point(20, 20);
            labelInfo.AutoSize = true;
            panelLoggedIn.Controls.Add(labelInfo);

            comboBoxCitizenship = new ComboBox();
            comboBoxCitizenship.Location = new Point(20, 50);
            comboBoxCitizenship.Width = 200;
            panelLoggedIn.Controls.Add(comboBoxCitizenship);

            var citizenships = dbHelper.GetCitizenships();
            foreach (var citizenship in citizenships)
            {
                comboBoxCitizenship.Items.Add(citizenship.Value);
            }

            checkBoxesConditions = new List<CheckBox>();
            var conditions = dbHelper.GetConditions();
            int y = 80;
            foreach (var condition in conditions)
            {
                var checkBox = new CheckBox();
                checkBox.Text = condition.Value;
                checkBox.Tag = condition.Key;
                checkBox.Location = new Point(20, y);
                checkBox.AutoSize = true;
                panelLoggedIn.Controls.Add(checkBox);
                checkBoxesConditions.Add(checkBox);
                y += 30;
            }

            buttonSave = new Button();
            buttonSave.Text = "Сохранить";
            buttonSave.Location = new Point(20, y + 20);
            buttonSave.Click += buttonSave_Click;
            panelLoggedIn.Controls.Add(buttonSave);

            buttonLogout = new Button();
            buttonLogout.Text = "Выход";
            buttonLogout.Dock = DockStyle.Bottom;
            buttonLogout.Click += buttonLogout_Click;
            panelLoggedIn.Controls.Add(buttonLogout);
        }

        private void LoadData(string username)
        {
            var (citizenshipId, conditionsIds) = dbHelper.GetUserData(username);

            if (citizenshipId != -1)
            {
                var chosenCitizenship = dbHelper.GetCitizenships().FirstOrDefault(c => c.Key == citizenshipId);
                if (chosenCitizenship.Value != null) { comboBoxCitizenship.SelectedItem = chosenCitizenship.Value; }
            }

            foreach (var conditionId in conditionsIds)
            {
                var checkBox = checkBoxesConditions.FirstOrDefault(cb => (int)cb.Tag == conditionId);
                if (checkBox != null) { checkBox.Checked = true; }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            int chosenCitizenshipId = -1;
            if (comboBoxCitizenship.SelectedItem != null)
            {
                var chosenCitizenshipName = comboBoxCitizenship.SelectedItem.ToString();
                var chosenCitizenship = dbHelper.GetCitizenships()
                    .FirstOrDefault(c => c.Value == chosenCitizenshipName);

                if (!chosenCitizenship.Equals(default(KeyValuePair<int, string>)))
                {
                    chosenCitizenshipId = chosenCitizenship.Key;
                }
            }

            var chosenConditions = checkBoxesConditions
                .Where(cb => cb.Checked)
                .Select(cb => (int)cb.Tag)
                .ToArray();

            dbHelper.UpdateUser(textboxLoginLogin.Text, chosenCitizenshipId, chosenConditions);

            MessageBox.Show("Данные сохранены!");
        }

        private void CenterPanelContent(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                control.Left = (panel.Width - control.Width) / 2;
            }
        }

        private void UpdateUI()
        {
            buttonTabLogin.Visible = !isLoggedIn;
            buttonTabRegister.Visible = !isLoggedIn;
            buttonLogout.Visible = isLoggedIn;
            panelLogin.Visible = !isLoggedIn && buttonTabLogin.Visible;
            panelRegister.Visible = !isLoggedIn && buttonTabRegister.Visible;
            panelLoggedIn.Visible = isLoggedIn;
        }

        private void ShowLoginPanel()
        {
            panelLogin.Visible = true;
            panelRegister.Visible = false;

            labelLoginError.Text = "";
            textboxLoginLogin.Text = "";
            textboxLoginPassword.Text = "";
        }

        private void ShowRegisterPanel()
        {
            panelLogin.Visible = false;
            panelRegister.Visible = true;

            labelRegisterError.Text = "";
            textboxRegisterLogin.Text = "";
            textboxRegisterPassword.Text = "";
            textboxRegisterPasswordConfirm.Text = "";
        }

        private void ShowLoggedInPanel()
        {
            panelLogin.Visible = false;
            panelRegister.Visible = false;
            panelLoggedIn.Visible = true;
            LoadData(textboxLoginLogin.Text);
        }

        private void buttonTabLogin_Click(object sender, EventArgs e) { ShowLoginPanel(); }

        private void buttonTabRegister_Click(object sender, EventArgs e) { ShowRegisterPanel(); }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string login = textboxLoginLogin.Text;
            string password = textboxLoginPassword.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                labelLoginError.Text = "Заполните все поля!";
                return;
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
                            labelLoginError.Text = "Такого аккаунта не существует!";
                            return;
                        }

                        string dbPassword = reader["password"].ToString();
                        if (dbPassword != password)
                        {
                            labelLoginError.Text = "Неверно введен пароль!";
                            return;
                        }

                        isLoggedIn = true;
                        UpdateUI();
                        ShowLoggedInPanel();
                        LoadData(login);
                    }
                }
            }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string login = textboxRegisterLogin.Text;
            string password = textboxRegisterPassword.Text;
            string password2 = textboxRegisterPasswordConfirm.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password2))
            {
                labelRegisterError.Text = "Заполните все поля!";
                return;
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
                            labelRegisterError.Text = "Такой логин уже занят!";
                            return;
                        }
                    }
                }

                if (password.Length < 8)
                {
                    labelRegisterError.Text = "Пароль должен содержать 8 и более символов!";
                    return;
                }

                if (password != password2)
                {
                    labelRegisterError.Text = "Пароли не совпадают!";
                    return;
                }

                using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", conn))
                {
                    cmd.Parameters.AddWithValue("username", login);
                    cmd.Parameters.AddWithValue("password", password);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        ShowLoginPanel();
                    }
                    catch (PostgresException ex) { labelRegisterError.Text = "Ошибка: " + ex.Message; }
                }
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            isLoggedIn = false;
            UpdateUI();
            ShowLoginPanel();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            buttonTabLogin.Width = this.ClientSize.Width / 2;
            buttonTabRegister.Width = this.ClientSize.Width / 2;

            CenterPanelContent(panelLogin);
            CenterPanelContent(panelRegister);
        }
    }
}