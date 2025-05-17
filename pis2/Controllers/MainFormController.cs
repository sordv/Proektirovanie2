using pis2.Views;
using pis2.Models;
using pis2.Services;

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

        private void ButtonGenerateRoadmap_Click(Object sender, EventArgs e)
        {
            MessageBox.Show("Пока нельзя");
        }

        private void ButtonEditRoadmap_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Пока тоже нельзя");
        }

        private void FormResize(object sender, EventArgs e)
        {
            _view.OnFormResize();
        }
    }
}