using System;
using System.Windows;
using Microsoft.Data.SqlClient;
using ResearchHub.Database;

namespace ResearchHub.Services
{
    public class ThemeService
    {
        private readonly DbConnection _db;

        public ThemeService(DbConnection db)
        {
            _db = db;
        }

        // 1. Pobieranie motywu zalogowanego użytkownika z SQL
        public async Task<string> GetUserThemeAsync(int userId)
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    await conn.OpenAsync();
                    string query = "SELECT Motyw FROM dbo.Uzytkownik WHERE ID = @UserId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = await cmd.ExecuteScalarAsync();
                        return result != null && result != DBNull.Value ? result.ToString() : "Dark";
                    }
                }
            }
            catch
            {
                return "Dark"; // W razie błędu domyślnie ładujemy ciemny motyw
            }
        }

        // 2. Zapisywanie nowego motywu dla użytkownika w SQL
        public void SaveUserTheme(int userId, string themeName)
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE dbo.Uzytkownik SET Motyw = @Motyw WHERE ID = @UserId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Motyw", themeName);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu motywu w bazie danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 3. Dynamiczne przełączanie zasobów w aplikacji WPF
        public void ApplyTheme(string themeName)
        {
            string themeUri = $"/Themes/{themeName}Theme.xaml";

            try
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri(themeUri, UriKind.RelativeOrAbsolute)
                };

                var mergedDicts = Application.Current.Resources.MergedDictionaries;

                // Szukamy i usuwamy stary plik motywu z pamięci aplikacji, by style się nie nakładały
                for (int i = mergedDicts.Count - 1; i >= 0; i--)
                {
                    if (mergedDicts[i].Source != null && mergedDicts[i].Source.OriginalString.Contains("Themes/"))
                    {
                        mergedDicts.RemoveAt(i);
                    }
                }

                // Dodajemy nowo wybrany słownik zasobów
                mergedDicts.Add(dict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie znaleziono pliku motywu: {themeUri}. Szczegóły: {ex.Message}", "Błąd motywu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}