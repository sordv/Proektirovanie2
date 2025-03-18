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
        private DatabaseHelper dbHelper;

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

        private Panel panelTabsLoggedIn;
        private Button buttonTabUser;
        private Button buttonTabRoadmap;

        private Panel panelUser;
        private Label labelInfo;
        private ComboBox comboBoxCitizenship;
        private List<CheckBox> checkBoxesConditions;
        private Button buttonSave;
        private Button buttonLogout;
        private Panel panelRoadmap;

        public MainForm()
        {
            dbHelper = new DatabaseHelper(connectionString);
            InitializeLoginForm();
            ShowLoginPanel();
        }

        private void InitializeLoginForm()
        {
            this.Text = "�������������� �������������� ������";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(500, 400);
            this.Resize += MainForm_Resize;

            // PANEL TABS LOGIN WITH BUTTONS
            panelTabsLogin = new Panel();
            panelTabsLogin.Dock = DockStyle.Top;
            panelTabsLogin.Height = 40;
            this.Controls.Add(panelTabsLogin);

            buttonTabLogin = new Button();
            buttonTabLogin.Text = "����";
            buttonTabLogin.Dock = DockStyle.Left;
            buttonTabLogin.Width = this.ClientSize.Width / 2;
            buttonTabLogin.Click += buttonTabLogin_Click;
            panelTabsLogin.Controls.Add(buttonTabLogin);

            buttonTabRegister = new Button();
            buttonTabRegister.Text = "�����������";
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
            textboxLoginLogin.PlaceholderText = "�����";
            panelLogin.Controls.Add(textboxLoginLogin);

            textboxLoginPassword = new TextBox();
            textboxLoginPassword.Location = new Point(50, 110);
            textboxLoginPassword.Size = new Size(200, 20);
            textboxLoginPassword.PlaceholderText = "������";
            textboxLoginPassword.UseSystemPasswordChar = true;
            panelLogin.Controls.Add(textboxLoginPassword);

            textboxRegisterLogin = new TextBox();
            textboxRegisterLogin.Location = new Point(50, 80);
            textboxRegisterLogin.Size = new Size(200, 20);
            textboxRegisterLogin.PlaceholderText = "�����";
            panelRegister.Controls.Add(textboxRegisterLogin);

            textboxRegisterPassword = new TextBox();
            textboxRegisterPassword.Location = new Point(50, 110);
            textboxRegisterPassword.Size = new Size(200, 20);
            textboxRegisterPassword.PlaceholderText = "������";
            textboxRegisterPassword.UseSystemPasswordChar = true;
            panelRegister.Controls.Add(textboxRegisterPassword);

            textboxRegisterPasswordConfirm = new TextBox();
            textboxRegisterPasswordConfirm.Location = new Point(50, 140);
            textboxRegisterPasswordConfirm.Size = new Size(200, 20);
            textboxRegisterPasswordConfirm.PlaceholderText = "��������� ������";
            textboxRegisterPasswordConfirm.UseSystemPasswordChar = true;
            panelRegister.Controls.Add(textboxRegisterPasswordConfirm);

            // LOGIN & REGISTER BUTTONS
            buttonLogin = new Button();
            buttonLogin.Location = new Point(50, 140);
            buttonLogin.Size = new Size(140, 30);
            buttonLogin.Text = "�����";
            buttonLogin.Click += buttonLogin_Click;
            panelLogin.Controls.Add(buttonLogin);

            buttonRegister = new Button();
            buttonRegister.Location = new Point(50, 170);
            buttonRegister.Size = new Size(140, 30);
            buttonRegister.Text = "������������������";
            buttonRegister.Click += buttonRegister_Click;
            panelRegister.Controls.Add(buttonRegister);

            // LOGIN & REGISTRATION CONTENT ALIGN
            CenterPanelContent(panelLogin);
            CenterPanelContent(panelRegister);

            InitializeLoggedInForm();
        }

        private void InitializeLoggedInForm()
        {
            // PANEL TABS LOGGED IN WITH BUTTONS
            panelTabsLoggedIn = new Panel();
            panelTabsLoggedIn.Dock = DockStyle.Top;
            panelTabsLoggedIn.Height = 40;
            this.Controls.Add(panelTabsLoggedIn);

            buttonTabUser = new Button();
            buttonTabUser.Text = "������";
            buttonTabUser.Dock = DockStyle.Left;
            buttonTabUser.Width = this.ClientSize.Width / 2;
            buttonTabUser.Click += buttonTabUser_Click;
            panelTabsLoggedIn.Controls.Add(buttonTabUser);

            buttonTabRoadmap = new Button();
            buttonTabRoadmap.Text = "�������� �����";
            buttonTabRoadmap.Dock = DockStyle.Right;
            buttonTabRoadmap.Width = this.ClientSize.Width / 2;
            buttonTabRoadmap.Click += buttonTabRoadmap_Click;
            panelTabsLoggedIn.Controls.Add(buttonTabRoadmap);

            // USER & ROADMAP PANELS
            panelUser = new Panel();
            panelUser.Dock = DockStyle.Fill;
            this.Controls.Add(panelUser);

            panelRoadmap = new Panel();
            panelRoadmap.Dock = DockStyle.Fill;
            this.Controls.Add(panelRoadmap);

            // USER PANEL CONTANT
            labelInfo = new Label();
            labelInfo.Text = "������� ���������� � ����";
            labelInfo.Location = new Point(20, 50);
            labelInfo.AutoSize = true;
            panelUser.Controls.Add(labelInfo);

            comboBoxCitizenship = new ComboBox();
            comboBoxCitizenship.Location = new Point(20, 80);
            comboBoxCitizenship.Width = 200;
            panelUser.Controls.Add(comboBoxCitizenship);

            var citizenships = dbHelper.GetData("citizenships");
            foreach (var citizenship in citizenships)
            {
                comboBoxCitizenship.Items.Add(citizenship.Value);
            }

            checkBoxesConditions = new List<CheckBox>();
            var conditions = dbHelper.GetData("conditions");
            int y = 110;
            foreach (var condition in conditions)
            {
                var checkBox = new CheckBox();
                checkBox.Text = condition.Value;
                checkBox.Tag = condition.Key;
                checkBox.Location = new Point(20, y);
                checkBox.AutoSize = true;
                panelUser.Controls.Add(checkBox);
                checkBoxesConditions.Add(checkBox);
                y += 30;
            }

            buttonSave = new Button();
            buttonSave.Text = "���������";
            buttonSave.Location = new Point(20, y + 20);
            buttonSave.Click += buttonSave_Click;
            panelUser.Controls.Add(buttonSave);

            buttonLogout = new Button();
            buttonLogout.Text = "�����";
            buttonLogout.Location = new Point(100, y + 20);
            buttonLogout.Click += buttonLogout_Click;
            panelUser.Controls.Add(buttonLogout);

            // ROADMAP PANEL CONTANT
            Label labelRoadmap = new Label();
            labelRoadmap.Text = "�������� ���� ��������� � ���������";
            labelRoadmap.Location = new Point(20, 50);
            labelRoadmap.AutoSize = true;
            panelRoadmap.Controls.Add(labelRoadmap);

            ComboBox comboBoxTargets = new ComboBox();
            comboBoxTargets.Location = new Point(20, 80);
            comboBoxTargets.Width = 550;
            panelRoadmap.Controls.Add(comboBoxTargets);

            var targets = dbHelper.GetData("targets");
            foreach (var target in targets)
            {
                comboBoxTargets.Items.Add(target.Value);
            }
        }

        private void LoadData(string username)
        {
            var (citizenshipId, conditionsIds) = dbHelper.GetUserData(username);

            if (citizenshipId != -1)
            {
                var chosenCitizenship = dbHelper.GetData("citizenships").FirstOrDefault(c => c.Key == citizenshipId);
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
                var chosenCitizenship = dbHelper.GetData("citizenships")
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

            MessageBox.Show("������ ���������!");
        }

        private void ShowLoginPanel()
        {
            panelTabsLogin.Visible = true;
            panelLogin.Visible = true;
            panelRegister.Visible = false;

            panelTabsLoggedIn.Visible = false;
            panelUser.Visible = false;
            //panelRoadmap.Visible = false;

            labelLoginError.Text = "";
            textboxLoginLogin.Text = "";
            textboxLoginPassword.Text = "";
        }

        private void ShowRegisterPanel()
        {
            panelRegister.Visible = true;
            panelLogin.Visible = false;

            labelRegisterError.Text = "";
            textboxRegisterLogin.Text = "";
            textboxRegisterPassword.Text = "";
            textboxRegisterPasswordConfirm.Text = "";
        }

        private void ShowUserPanel()
        {
            panelTabsLogin.Visible = false;
            panelLogin.Visible = false;
            panelRegister.Visible = false;

            panelTabsLoggedIn.Visible = true;
            panelUser.Visible = true;
            LoadData(textboxLoginLogin.Text);
        }

        private void ShowRoadmapPanel()
        {
            panelRoadmap.Visible = true;
            panelUser.Visible = false;
        }

        private void buttonTabLogin_Click(object sender, EventArgs e)
        {
            ShowLoginPanel();
            CenterPanelContent(panelLogin);
        }

        private void buttonTabRegister_Click(object sender, EventArgs e)
        {
            ShowRegisterPanel();
            CenterPanelContent(panelRegister);
        }

        private void buttonTabUser_Click(object sender, EventArgs e)
        {
            ShowUserPanel();
        }

        private void buttonTabRoadmap_Click(object sender, EventArgs e)
        {
            ShowRoadmapPanel();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string login = textboxLoginLogin.Text;
            string password = textboxLoginPassword.Text;
            string errorMessage;

            if (dbHelper.LoginUser(login, password, out errorMessage))
            {
                isLoggedIn = true;
                ShowUserPanel();
                LoadData(login);
            }
            else { labelLoginError.Text = errorMessage; }
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string login = textboxRegisterLogin.Text;
            string password = textboxRegisterPassword.Text;
            string passwordConfirm = textboxRegisterPasswordConfirm.Text;
            string errorMessage;

            if (dbHelper.RegisterUser(login, password, passwordConfirm, out errorMessage))
            {
                ShowLoginPanel();
            }
            else { labelRegisterError.Text = errorMessage; }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            isLoggedIn = false;
            ShowLoginPanel();
        }

        private void CenterPanelContent(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                control.Left = (panel.Width - control.Width) / 2;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            buttonTabLogin.Width = this.ClientSize.Width / 2;
            buttonTabRegister.Width = this.ClientSize.Width / 2;
            buttonTabUser.Width = this.ClientSize.Width / 2;
            buttonTabRoadmap.Width = this.ClientSize.Width / 2;

            CenterPanelContent(panelLogin);
            CenterPanelContent(panelRegister);
        }
    }
}