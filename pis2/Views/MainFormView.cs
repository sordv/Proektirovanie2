using pis2.Models;

namespace pis2.Views
{
    public partial class MainFormView : Form
    {
        public Panel PanelTabsLogin { get; private set; } // меню с двумя кнопками сверху
        public Button ButtonTabLogin { get; private set; } // кнопка ВХОД сверху
        public Button ButtonTabRegister { get; private set; } // кнопка РЕГИСТРАЦИЯ сверху
        public Panel PanelLogin { get; private set; } // страница входа
        public TextBox TextboxLoginLogin { get; private set; } // поле для логина
        public TextBox TextboxLoginPassword { get; private set; } // поле для пароля
        public Button ButtonLogin { get; private set; } // кнопка ВОЙТИ
        public Label LabelLoginError { get; private set; } // отображение ошибки входа
        public Panel PanelRegister { get; private set; } // страница регистрации
        public TextBox TextboxRegisterLogin { get; private set; } // поле для логина
        public TextBox TextboxRegisterPassword { get; private set; } // поле для пароля
        public TextBox TextboxRegisterPasswordConfirm { get; private set; } // поле для подтверждения пароля
        public Button ButtonRegister { get; private set; } // кнопка РЕГИСТРАЦИЯ
        public Label LabelRegisterError { get; private set; } // отображение ошибки регистрации
        public Panel PanelUser { get; private set; } // страница с данными пользователям
        public Label LabelInfo { get; private set; } // текст инструкция
        public Label LabelCitizenship { get; private set; } // текст выберите гражданство
        public ComboBox ComboBoxCitizenship { get; private set; } // комбобокс для выбора гражданства
        public Label LabelDataEntry { get; private set; } // текст выберите дату въезда
        public DateTimePicker DateTimePickerEntry { get; private set; } // календарь для выбора даты
        public List<CheckBox> CheckBoxesFlags { get; private set; } // массив для всех чекбоксов из БД
        public Button ButtonSave { get; private set; } // кнопка СОХРАНИТЬ данные
        public Button ButtonLogout { get; private set; } // кнопка ВЫЙТИ из аккаунта
        public Button ButtonGenerateRoadmap { get; private set; } // кнопка СГЕНЕРИРОВАТЬ ДОРОЖНУЮ КАРТУ
        public Button ButtonEditRoadmap { get; private set; } // кнопка РЕДАКТИРОВАТЬ ПРАВИЛА для администратора

        public MainFormView()
        {
            DrawLoginPage();
            DrawUserPage();
        }

        private void DrawLoginPage()
        {
            this.Text = "Проектирование информационных систем";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 500); // не уменьшать, а то не красиво

            // PANEL TABS LOGIN WITH BUTTONS
            PanelTabsLogin = new Panel();
            PanelTabsLogin.Dock = DockStyle.Top;
            PanelTabsLogin.Height = 40;
            this.Controls.Add(PanelTabsLogin);

            ButtonTabLogin = new Button();
            ButtonTabLogin.Text = "Вход";
            ButtonTabLogin.Dock = DockStyle.Left;
            ButtonTabLogin.Width = this.ClientSize.Width / 2;
            PanelTabsLogin.Controls.Add(ButtonTabLogin);

            ButtonTabRegister = new Button();
            ButtonTabRegister.Text = "Регистрация";
            ButtonTabRegister.Dock = DockStyle.Right;
            ButtonTabRegister.Width = this.ClientSize.Width / 2;
            PanelTabsLogin.Controls.Add(ButtonTabRegister);

            // LOGIN & REGISTER PANELS
            PanelLogin = new Panel();
            PanelLogin.Dock = DockStyle.Fill;
            this.Controls.Add(PanelLogin);

            PanelRegister = new Panel();
            PanelRegister.Dock = DockStyle.Fill;
            this.Controls.Add(PanelRegister);

            // LOGIN & REGISTER ERROR LABELS
            LabelLoginError = new Label();
            LabelLoginError.Location = new Point(50, 50);
            LabelLoginError.Size = new Size(300, 20);
            LabelLoginError.ForeColor = Color.Red;
            LabelLoginError.TextAlign = ContentAlignment.MiddleCenter;
            PanelLogin.Controls.Add(LabelLoginError);

            LabelRegisterError = new Label();
            LabelRegisterError.Location = new Point(50, 50);
            LabelRegisterError.Size = new Size(300, 20);
            LabelRegisterError.ForeColor = Color.Red;
            LabelRegisterError.TextAlign = ContentAlignment.MiddleCenter;
            PanelRegister.Controls.Add(LabelRegisterError);

            // LOGIN & REGISTER TEXTBOXES
            TextboxLoginLogin = new TextBox();
            TextboxLoginLogin.Location = new Point(50, 80);
            TextboxLoginLogin.Size = new Size(200, 20);
            TextboxLoginLogin.PlaceholderText = "Логин";
            PanelLogin.Controls.Add(TextboxLoginLogin);

            TextboxLoginPassword = new TextBox();
            TextboxLoginPassword.Location = new Point(50, 110);
            TextboxLoginPassword.Size = new Size(200, 20);
            TextboxLoginPassword.PlaceholderText = "Пароль";
            TextboxLoginPassword.UseSystemPasswordChar = true;
            PanelLogin.Controls.Add(TextboxLoginPassword);

            TextboxRegisterLogin = new TextBox();
            TextboxRegisterLogin.Location = new Point(50, 80);
            TextboxRegisterLogin.Size = new Size(200, 20);
            TextboxRegisterLogin.PlaceholderText = "Логин";
            PanelRegister.Controls.Add(TextboxRegisterLogin);

            TextboxRegisterPassword = new TextBox();
            TextboxRegisterPassword.Location = new Point(50, 110);
            TextboxRegisterPassword.Size = new Size(200, 20);
            TextboxRegisterPassword.PlaceholderText = "Пароль";
            TextboxRegisterPassword.UseSystemPasswordChar = true;
            PanelRegister.Controls.Add(TextboxRegisterPassword);

            TextboxRegisterPasswordConfirm = new TextBox();
            TextboxRegisterPasswordConfirm.Location = new Point(50, 140);
            TextboxRegisterPasswordConfirm.Size = new Size(200, 20);
            TextboxRegisterPasswordConfirm.PlaceholderText = "Повторите пароль";
            TextboxRegisterPasswordConfirm.UseSystemPasswordChar = true;
            PanelRegister.Controls.Add(TextboxRegisterPasswordConfirm);

            // LOGIN & REGISTER BUTTONS
            ButtonLogin = new Button();
            ButtonLogin.Location = new Point(50, 140);
            ButtonLogin.Size = new Size(140, 30);
            ButtonLogin.Text = "Войти";
            PanelLogin.Controls.Add(ButtonLogin);

            ButtonRegister = new Button();
            ButtonRegister.Location = new Point(50, 170);
            ButtonRegister.Size = new Size(140, 30);
            ButtonRegister.Text = "Зарегистрироваться";
            PanelRegister.Controls.Add(ButtonRegister);

            // LOGIN & REGISTRATION CONTENT ALIGN
            CenterPanelContent(PanelLogin);
            CenterPanelContent(PanelRegister);
        }

        private void DrawUserPage()
        {
            // LOGGED IN PANEL
            PanelUser = new Panel();
            PanelUser.Dock = DockStyle.Fill;
            this.Controls.Add(PanelUser);

            // USER PANEL CONTANT
            LabelInfo = new Label();
            LabelInfo.Text = "Укажите информацию о себе:";
            LabelInfo.AutoSize = true;
            LabelInfo.Location = new Point(20, 20);
            PanelUser.Controls.Add(LabelInfo);

            LabelCitizenship = new Label();
            LabelCitizenship.Text = "Укажите страну гражданства:";
            LabelCitizenship.Location = new Point(20, 50);
            PanelUser.Controls.Add(LabelCitizenship);

            ComboBoxCitizenship = new ComboBox();
            ComboBoxCitizenship.Location = new Point(200, 50);
            ComboBoxCitizenship.Width = 250;
            ComboBoxCitizenship.DropDownStyle = ComboBoxStyle.DropDownList;
            PanelUser.Controls.Add(ComboBoxCitizenship);

            LabelDataEntry = new Label();
            LabelDataEntry.Text = "Укажите дату въезда в РФ:";
            LabelDataEntry.Location = new Point(20, 80);
            PanelUser.Controls.Add(LabelDataEntry);

            DateTimePickerEntry = new DateTimePicker();
            DateTimePickerEntry.Location = new Point(200, 80);
            DateTimePickerEntry.Format = DateTimePickerFormat.Short;
            DateTimePickerEntry.Width = 250;
            PanelUser.Controls.Add(DateTimePickerEntry);

            CheckBoxesFlags = new List<CheckBox>();

            ButtonSave = new Button();
            ButtonSave.Text = "Сохранить";
            ButtonSave.Location = new Point(20, 120);
            PanelUser.Controls.Add(ButtonSave);

            ButtonLogout = new Button();
            ButtonLogout.Text = "Выход";
            ButtonLogout.Location = new Point(100, 120);
            PanelUser.Controls.Add(ButtonLogout);

            ButtonGenerateRoadmap = new Button();
            ButtonGenerateRoadmap.Text = "Показать дорожную карту";
            ButtonGenerateRoadmap.Location = new Point(20, 160);
            ButtonGenerateRoadmap.Size = new Size(120, 40);
            PanelUser.Controls.Add(ButtonGenerateRoadmap);

            ButtonEditRoadmap = new Button();
            ButtonEditRoadmap.Text = "Редактировать правила";
            ButtonEditRoadmap.Location = new Point(150, 160);
            ButtonEditRoadmap.Size = new Size(120, 40);
            PanelUser.Controls.Add(ButtonEditRoadmap);
        }

        public void LoadDataUserPanel(List<KeyValuePair<int, string>> allCitizenships, User currentUser)
        {
            ComboBoxCitizenship.Items.Clear();
            foreach (var i in allCitizenships) { ComboBoxCitizenship.Items.Add(i.Value); }
            ComboBoxCitizenship.SelectedIndex = -1;

            foreach (var i in CheckBoxesFlags) { i.Checked = false; }

            if (currentUser.Citizenship != -1)
            {
                var chosenCitizenship = allCitizenships.FirstOrDefault(item => item.Key == currentUser.Citizenship);
                if (chosenCitizenship.Value != null) { ComboBoxCitizenship.SelectedItem = chosenCitizenship.Value; }
            }

            if (currentUser.Flags != null)
            {
                foreach (var flag in currentUser.Flags)
                {
                    var checkBox = CheckBoxesFlags.FirstOrDefault(item => (int)item.Tag == flag);
                    if (checkBox != null) { checkBox.Checked = true; }
                }
            }

            if (currentUser.Entry.HasValue) { DateTimePickerEntry.Value = currentUser.Entry.Value; }
            else { DateTimePickerEntry.Value = DateTime.Now; }
        }

        public void ShowLoginPanel()
        {
            PanelTabsLogin.Visible = true;
            PanelLogin.Visible = true;
            PanelRegister.Visible = false;
            PanelUser.Visible = false;

            LabelLoginError.Text = "";
            TextboxLoginLogin.Text = "";
            TextboxLoginPassword.Text = "";

            CenterPanelContent(PanelLogin);
        }

        public void ShowRegisterPanel()
        {
            PanelRegister.Visible = true;
            PanelLogin.Visible = false;

            LabelRegisterError.Text = "";
            TextboxRegisterLogin.Text = "";
            TextboxRegisterPassword.Text = "";
            TextboxRegisterPasswordConfirm.Text = "";

            CenterPanelContent(PanelRegister);
        }

        public void ShowUserPanel(List<KeyValuePair<int, string>> allCitizenships, User currentUser)
        {
            PanelTabsLogin.Visible = false;
            PanelLogin.Visible = false;
            PanelRegister.Visible = false;
            PanelUser.Visible = true;

            LoadDataUserPanel(allCitizenships, currentUser);
        }

        public void CenterPanelContent(Panel panel)
        {
            foreach (Control control in panel.Controls) { control.Left = (panel.Width - control.Width) / 2; }
        }

        public void UpdateFlagsCheckboxes(List<KeyValuePair<int, string>> allFlags)
        {
            CheckBoxesFlags.Clear();
            int y = 120;

            foreach (var flag in allFlags)
            {
                var checkBox = new CheckBox();
                checkBox.Text = flag.Value;
                checkBox.Tag = flag.Key;
                checkBox.Location = new Point(20, y);
                checkBox.AutoSize = true;

                PanelUser.Controls.Add(checkBox);
                CheckBoxesFlags.Add(checkBox);
                y += 30;
            }

            ButtonSave.Location = new Point(20, y + 30);
            ButtonLogout.Location = new Point(100, y + 30);
            ButtonGenerateRoadmap.Location = new Point(20, y + 60);
            ButtonEditRoadmap.Location = new Point(150, y + 60);
        }

        public void OnFormResize()
        {
            ButtonTabLogin.Width = this.ClientSize.Width / 2;
            ButtonTabRegister.Width = this.ClientSize.Width / 2;

            CenterPanelContent(PanelLogin);
            CenterPanelContent(PanelRegister);
        }
    }
}