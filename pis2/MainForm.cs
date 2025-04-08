using Microsoft.VisualBasic.ApplicationServices;
using System.Text;

namespace pis2
{
    public partial class MainForm : Form
    {
        private bool isLoggedIn = false;
        private string connectionString = "Host=localhost;Username=postgres;Password=admin;Database=pis";
        private DatabaseHelper dbHelper;
        private User currentUser;

        private Panel panelTabsLogin;
        private Button buttonTabLogin;
        private Button buttonTabRegister;

        private Panel panelLogin;
        private TextBox textboxLoginLogin;
        private TextBox textboxLoginPassword;
        private Button buttonLogin;
        private Label labelLoginError;
        private Panel panelRegister;
        private TextBox textboxRegisterLogin;
        private TextBox textboxRegisterPassword;
        private TextBox textboxRegisterPasswordConfirm;
        private Button buttonRegister;
        private Label labelRegisterError;

        private Panel panelUser;
        private Label labelInfo;
        private Label labelCitizenship;
        private ComboBox comboBoxCitizenship;
        private Label labelDataEntry;
        private DateTimePicker dateTimePickerEntry;
        private List<CheckBox> checkBoxesFlags;
        private Button buttonSave;
        private Button buttonLogout;
        private Button buttonGenerateRoadmap;
        private Button buttonEditRoadmap;

        public MainForm()
        {
            dbHelper = new DatabaseHelper(connectionString);
            DrawLoginPage();
            ShowLoginPanel();
        }

        private void DrawLoginPage()
        {
            this.Text = "Проектирование информационных систем";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 500);
            this.Resize += MainForm_Resize;

            // PANEL TABS LOGIN WITH BUTTONS
            panelTabsLogin = new Panel();
            panelTabsLogin.Dock = DockStyle.Top;
            panelTabsLogin.Height = 40;
            this.Controls.Add(panelTabsLogin);

            buttonTabLogin = new Button();
            buttonTabLogin.Text = "Вход";
            buttonTabLogin.Dock = DockStyle.Left;
            buttonTabLogin.Width = this.ClientSize.Width / 2;
            buttonTabLogin.Click += buttonTabLogin_Click;
            panelTabsLogin.Controls.Add(buttonTabLogin);

            buttonTabRegister = new Button();
            buttonTabRegister.Text = "Регистрация";
            buttonTabRegister.Dock = DockStyle.Right;
            buttonTabRegister.Width = this.ClientSize.Width / 2;
            buttonTabRegister.Click += buttonTabRegister_Click;
            panelTabsLogin.Controls.Add(buttonTabRegister);

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

            DrawUserPage();
        }

        private void DrawUserPage()
        {
            // LOGGED IN PANEL
            panelUser = new Panel();
            panelUser.Dock = DockStyle.Fill;
            this.Controls.Add(panelUser);

            // USER PANEL CONTANT
            labelInfo = new Label();
            labelInfo.Text = "Укажите информацию о себе:";
            labelInfo.AutoSize = true;
            labelInfo.Location = new Point(20, 20);
            panelUser.Controls.Add(labelInfo);

            labelCitizenship = new Label();
            labelCitizenship.Text = "Укажите страну гражданства:";
            labelCitizenship.Location = new Point(20, 50);
            panelUser.Controls.Add(labelCitizenship);

            comboBoxCitizenship = new ComboBox();
            comboBoxCitizenship.Location = new Point(200, 50);
            comboBoxCitizenship.Width = 250;
            comboBoxCitizenship.DropDownStyle = ComboBoxStyle.DropDownList;
            panelUser.Controls.Add(comboBoxCitizenship);

            labelDataEntry = new Label();
            labelDataEntry.Text = "Укажите дату въезда в РФ:";
            labelDataEntry.Location = new Point(20, 80);
            panelUser.Controls.Add(labelDataEntry);

            dateTimePickerEntry = new DateTimePicker();
            dateTimePickerEntry.Location = new Point(200, 80);
            dateTimePickerEntry.Format = DateTimePickerFormat.Short;
            dateTimePickerEntry.Width = 250;
            panelUser.Controls.Add(dateTimePickerEntry);

            var citizenships = dbHelper.GetData("citizenships");
            foreach (var citizenship in citizenships) { comboBoxCitizenship.Items.Add(citizenship.Value); }

            checkBoxesFlags = new List<CheckBox>();
            var flags = dbHelper.GetData("flags");
            int y = 120;
            foreach (var flag in flags)
            {
                var checkBox = new CheckBox();
                checkBox.Text = flag.Value;
                checkBox.Tag = flag.Key;
                checkBox.Location = new Point(20, y);
                checkBox.AutoSize = true;

                panelUser.Controls.Add(checkBox);
                checkBoxesFlags.Add(checkBox);
                y += 30;
            }

            buttonSave = new Button();
            buttonSave.Text = "Сохранить";
            buttonSave.Location = new Point(20, y + 30);
            buttonSave.Click += buttonSave_Click;
            panelUser.Controls.Add(buttonSave);

            buttonLogout = new Button();
            buttonLogout.Text = "Выход";
            buttonLogout.Location = new Point(100, y + 30);
            buttonLogout.Click += buttonLogout_Click;
            panelUser.Controls.Add(buttonLogout);

            buttonGenerateRoadmap = new Button();
            buttonGenerateRoadmap.Text = "Показать дорожную карту";
            buttonGenerateRoadmap.Location = new Point(20, y + 60);
            buttonGenerateRoadmap.Size = new Size(120, 40);
            buttonGenerateRoadmap.Click += buttonGenerateRoadmap_Click;
            panelUser.Controls.Add(buttonGenerateRoadmap);

            buttonEditRoadmap = new Button();
            buttonEditRoadmap.Text = "Редактировать правила";
            buttonEditRoadmap.Location = new Point(150, y + 60);
            buttonEditRoadmap.Size = new Size(120, 40);
            buttonEditRoadmap.Click += buttonEditRoadmap_Click;
            panelUser.Controls.Add(buttonEditRoadmap);
        }

        private void LoadDataUserPanel(User currentUser)
        {
            comboBoxCitizenship.SelectedIndex = -1;

            foreach (var checkBox in checkBoxesFlags) { checkBox.Checked = false; }

            if (currentUser.Citizenship != -1)
            {
                var citizenships = dbHelper.GetData("citizenships");
                var chosenCitizenship = citizenships.FirstOrDefault(item => item.Key == currentUser.Citizenship);
                if (chosenCitizenship.Value != null) { comboBoxCitizenship.SelectedItem = chosenCitizenship.Value; }
            }

            if (currentUser.Flags != null)
            {
                foreach (var flag in currentUser.Flags)
                {
                    var checkBox = checkBoxesFlags.FirstOrDefault(item => (int)item.Tag == flag);
                    if (checkBox != null) { checkBox.Checked = true; }
                }
            }

            if (currentUser.Entry.HasValue) { dateTimePickerEntry.Value = currentUser.Entry.Value; }
            else { dateTimePickerEntry.Value = DateTime.Now; }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxCitizenship.SelectedItem != null)
            {
                var chosenCitizenshipName = comboBoxCitizenship.SelectedItem.ToString();
                currentUser.Citizenship = dbHelper.GetData("citizenships").FirstOrDefault(item => item.Value == chosenCitizenshipName).Key;
            }
            else { currentUser.Citizenship = -1; }

            currentUser.Flags = checkBoxesFlags
                .Where(item => item.Checked)
                .Select(item => (int)item.Tag)
                .ToArray();

            currentUser.Entry = dateTimePickerEntry.Value;

            dbHelper.UpdateUser(currentUser.Login, currentUser.Citizenship, currentUser.Flags, currentUser.Entry);

            MessageBox.Show("Данные сохранены!");
        }

        private void ShowLoginPanel()
        {
            panelTabsLogin.Visible = true;
            panelLogin.Visible = true;
            panelRegister.Visible = false;

            panelUser.Visible = false;

            labelLoginError.Text = "";
            textboxLoginLogin.Text = "";
            textboxLoginPassword.Text = "";

            CenterPanelContent(panelLogin);
        }

        private void ShowRegisterPanel()
        {
            panelRegister.Visible = true;
            panelLogin.Visible = false;

            labelRegisterError.Text = "";
            textboxRegisterLogin.Text = "";
            textboxRegisterPassword.Text = "";
            textboxRegisterPasswordConfirm.Text = "";

            CenterPanelContent(panelRegister);
        }

        private void ShowUserPanel(User currentUser)
        {
            panelTabsLogin.Visible = false;
            panelLogin.Visible = false;
            panelRegister.Visible = false;

            panelUser.Visible = true;

            LoadDataUserPanel(currentUser);
        }

        private void buttonTabLogin_Click(object sender, EventArgs e) { ShowLoginPanel(); }

        private void buttonTabRegister_Click(object sender, EventArgs e) { ShowRegisterPanel(); }

        private void buttonTabUser_Click(object sender, EventArgs e) { ShowUserPanel(currentUser); }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string login = textboxLoginLogin.Text;
            string password = textboxLoginPassword.Text;
            string errorMessage;

            if (dbHelper.LoginUser(login, password, out errorMessage))
            {
                isLoggedIn = true;

                var (citizenship, flags, entry) = dbHelper.GetUserData(login);
                currentUser = new User(login, citizenship, flags, entry);

                ShowUserPanel(currentUser);
                buttonEditRoadmap.Visible = (login == "admin");
            }
            else { labelLoginError.Text = errorMessage; }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string login = textboxRegisterLogin.Text;
            string password = textboxRegisterPassword.Text;
            string passwordConfirm = textboxRegisterPasswordConfirm.Text;
            string errorMessage;

            if (dbHelper.RegisterUser(login, password, passwordConfirm, out errorMessage)) { ShowLoginPanel(); }
            else { labelRegisterError.Text = errorMessage; }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            isLoggedIn = false;
            ShowLoginPanel();
        }

        private void buttonGenerateRoadmap_Click(Object sender, EventArgs e)
        {
            MessageBox.Show("Пока нельзя");
        }

        private void buttonEditRoadmap_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Пока тоже нельзя");
        }

        private void CenterPanelContent(Panel panel)
        {
            foreach (Control control in panel.Controls) { control.Left = (panel.Width - control.Width) / 2; }
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