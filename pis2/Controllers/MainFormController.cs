using pis2.Views;
using pis2.Models;
using pis2.Services;
using System.Text;
using Npgsql;

namespace pis2.Controllers
{
    public class MainFormController
    {
        private readonly MainFormView _view;
        private readonly DatabaseService _dbService;
        private User _currentUser;
        private List<KeyValuePair<int, string>> _allCitizenships;
        private List<KeyValuePair<int, string>> _allFlags;

        public MainFormController(MainFormView view, string connectionString)
        {
            _view = view;
            _dbService = new DatabaseService(connectionString);

            InitializeEvents();
            (_allCitizenships, _allFlags) = _dbService.GetData();
            _view.UpdateFlagsCheckboxes(_allFlags);
        }

        private void InitializeEvents()
        {
            _view.ButtonTabLogin.Click += ButtonTabLogin_Click;
            _view.ButtonTabRegister.Click += ButtonTabRegister_Click;
            _view.ButtonLogin.Click += ButtonLogin_Click;
            _view.ButtonRegister.Click += ButtonRegister_Click;
            _view.ButtonSave.Click += ButtonSave_Click;
            _view.ButtonLogout.Click += ButtonLogout_Click;
            _view.ButtonGenerateRoadmap.Click += ButtonGenerateRoadmap_Click;
            _view.ButtonEditRoadmap.Click += ButtonEditRoadmap_Click;
            _view.Resize += FormResize;
        }

        private void ButtonTabLogin_Click(object sender, EventArgs e)
        {
            _view.ShowLoginPanel();
        }

        private void ButtonTabRegister_Click(object sender, EventArgs e)
        {
            _view.ShowRegisterPanel();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string login = _view.TextboxLoginLogin.Text;
            string password = _view.TextboxLoginPassword.Text;
            string errorMessage;

            if (_dbService.LoginUser(login, password, out errorMessage))
            {
                var (citizenship, flags, entry) = _dbService.GetUserData(login);
                _currentUser = new User(login, citizenship, flags, entry);

                _view.ShowUserPanel(_allCitizenships, _currentUser);
                _view.ButtonEditRoadmap.Visible = (login == "admin");
            }
            else { _view.LabelLoginError.Text = errorMessage; }
        }

        private void ButtonRegister_Click(object sender, EventArgs e)
        {
            string login = _view.TextboxRegisterLogin.Text;
            string password = _view.TextboxRegisterPassword.Text;
            string passwordConfirm = _view.TextboxRegisterPasswordConfirm.Text;
            string errorMessage;

            if (_dbService.RegisterUser(login, password, passwordConfirm, out errorMessage))
            {
                _view.ShowLoginPanel();
            }
            else { _view.LabelRegisterError.Text = errorMessage; }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (_view.ComboBoxCitizenship.SelectedItem != null)
            {
                var chosenCitizenshipName = _view.ComboBoxCitizenship.SelectedItem.ToString();
                _currentUser.Citizenship = _allCitizenships.FirstOrDefault(item => item.Value == chosenCitizenshipName).Key;
            }
            else { _currentUser.Citizenship = -1; }

            _currentUser.Flags = _view.CheckBoxesFlags
                .Where(item => item.Checked)
                .Select(item => (int)item.Tag)
                .ToArray();

            _currentUser.Entry = _view.DateTimePickerEntry.Value;

            _dbService.UpdateUser(_currentUser);

            MessageBox.Show("Данные сохранены!");
        }

        private void ButtonLogout_Click(object sender, EventArgs e)
        {
            _view.ShowLoginPanel();
        }

        private void ButtonGenerateRoadmap_Click(object sender, EventArgs e)
        {
            var (rules, roadmapItems) = _dbService.GetRoadmapData();

            var filteredRules = rules
                .Where(rule =>
                {
                    // гражданство юзера должно быть в массиве РАЗРЕШЕННЫХ гражданствах
                    // ИЛИ массив РАЗРЕШЕННЫХ гражданств должен быть пустой
                    bool ctzOk = rule.Citizenship.Length == 0 ||
                        rule.Citizenship.Contains(_currentUser.Citizenship);
                    // гражданство юзера НЕ должно быть в массиве ЗАПРЕЩЕННЫХ гражданствах
                    // ИЛИ массив ЗАПРЕЩЕННЫХ гражданств должен быть пустой
                    bool banctzOk = rule.BannedCitizenships.Length == 0 ||
                        !rule.BannedCitizenships.Contains(_currentUser.Citizenship);
                    // все флаги юзера должны быть в массиве ОБЯЗАТЕЛНЫХ флагов
                    // ИЛИ массив ОБЯЗАТЕЛЬНЫХ флагов должен быть пустой
                    bool flgOk = rule.Flag.Length == 0 ||
                        rule.Flag.All(i => _currentUser.Flags.Contains(i));
                    // ни один флаг юзера НЕ должен быть в массиве ЗАПРЕЩЕННЫХ флагов
                    // ИЛИ массив ЗАПРЕЩЕННЫХ флагов должен быть пустой
                    bool banflgOk = rule.BannedFlags.Length == 0 ||
                        !_currentUser.Flags.Any(i => rule.BannedFlags.Contains(i));
                    // подходят те праила, где все 4 условия соблюдены
                    return ctzOk && banctzOk && flgOk && banflgOk;
                })
                .GroupBy(r => r.RoadmapItem)
                .Select(g => g
                    .OrderByDescending(r => r.Period ?? int.MinValue)
                    .First())
                .ToList();

            filteredRules = filteredRules
                .OrderBy(r => r.Period.HasValue ? 0 : 1)
                .ThenBy(r => r.Period ?? int.MaxValue)
                .ToList();

            var result = new Roadmap();

            foreach (var rule in filteredRules)
            {
                var roadmapItem = roadmapItems.FirstOrDefault(item => item.Id == rule.RoadmapItem);
                if (rule.Period.HasValue)
                {
                    if (roadmapItem != null) { roadmapItem.Period = rule.Period; }
                }
                result.AddItem(roadmapItem);
            }

            MessageBox.Show(result.Render());
        }

        private void ButtonEditRoadmap_Click(object sender, EventArgs e)
        {
            var editorView = new EditorFormView();
            var editorController = new EditorFormController(editorView, _dbService.ConnectionString);
            editorView.ShowDialog();
        }

        private void FormResize(object sender, EventArgs e)
        {
            _view.OnFormResize();
        }
    }
}