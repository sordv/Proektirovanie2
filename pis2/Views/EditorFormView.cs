namespace pis2.Views
{
    public class EditorFormView : Form
    {
        public TabControl TabControl { get; private set; }
        public TextBox ConsoleTextBox { get; private set; }

        public EditorFormView()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Редактор правил дорожной карты";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(mainPanel);

            TabControl = new TabControl { Dock = DockStyle.Fill };
            mainPanel.Controls.Add(TabControl);

            var consolePanel = new Panel { Dock = DockStyle.Bottom, Height = 150 };
            ConsoleTextBox = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true
            };
            consolePanel.Controls.Add(ConsoleTextBox);
            mainPanel.Controls.Add(consolePanel);

            var backButton = new Button
            {
                Text = "Назад",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            backButton.Click += (s, e) => this.Close();
            mainPanel.Controls.Add(backButton);
        }

        public void AddConsoleMessage(string message)
        {
            ConsoleTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}