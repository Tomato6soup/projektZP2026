using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ResearchHub.Commands;
using ResearchHub.Database;
using Microsoft.Data.SqlClient;

namespace ResearchHub.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _statusMessage = string.Empty;
        private readonly DbConnection _db;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            _db = new DbConnection();
            LoginCommand = new RelayCommand(_ => ExecuteLogin(), _ => CanExecuteLogin());
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    // 1. Zmieniamy zapytanie, aby pobierało Rola ORAZ ID
                    string query = "SELECT Rola, ID FROM dbo.Uzytkownik WHERE Login = @Login AND Haslo = @Haslo";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Login", Username);
                        cmd.Parameters.AddWithValue("@Haslo", Password);

                        // 2. Używamy ExecuteReader zamiast ExecuteScalar
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Jeśli użytkownik istnieje i dane są poprawne
                            {
                                // 3. Pobieramy dane z czytnika (indeks 0 to Rola, indeks 1 to ID)
                                string role = reader.GetString(0);
                                int userId = reader.GetInt32(1);

                                StatusMessage = $"Zalogowano pomyślnie jako {role}";

                                // 4. Przejście do MainWindow z poprawnymi danymi
                                var mainWindow = new MainWindow(role, userId);
                                mainWindow.Show();

                                // Zamknięcie okna logowania
                                Application.Current.Windows[0]?.Close();
                            }
                            else
                            {
                                StatusMessage = "Błędny login lub hasło.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Błąd bazy SQL: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name ?? string.Empty));
        }
    }
}

