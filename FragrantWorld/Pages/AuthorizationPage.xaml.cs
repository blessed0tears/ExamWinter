using DatabaseLibrary.Models;
using DatabaseLibrary.Services;
using System.Windows;
using System.Windows.Controls;

namespace FragrantWorld.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private readonly UserService _userService = new();

        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private async void AuthorizationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = LoginTextBox.Text;
                string password = PasswordTextBox.Password;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получаем пользователя с ролью
                var user = await _userService.GetUserWithRole(login, password);

                if (user != null)
                {
                    // Сохраняем данные пользователя в статических свойствах
                    App.CurrentUser = user;
                    App.CurrentUserRole = user.Role?.Name;

                    MessageBox.Show($"Добро пожаловать, {user.Name} {user.Surname}!\nРоль: {App.CurrentUserRole}",
                        "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                    App.CurrentFrame.Navigate(new ShopPage());
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            App.CurrentUserRole = "Гость";
            MessageBox.Show("Вы вошли как гость", "Гостевой вход", MessageBoxButton.OK, MessageBoxImage.Information);
            App.CurrentFrame.Navigate(new ShopPage());
        }
    }
}