using Microsoft.Data.SqlClient;
using ResearchHub.Commands;
using ResearchHub.Database;
using ResearchHub.Model;
using ResearchHub.View;
using System;
using System.Collections.Generic;
using ResearchHub.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ResearchHub.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ==========================================
        // 1. POLA PRYWATNE
        // ==========================================
        private readonly DbConnection _db;
        private readonly ProjectParticipationService _projectService;
        private string _userRole;
        private int _currentUserId; // Dodano przechowywanie ID zalogowanego użytkownika

        private string _searchText = string.Empty;
        private string _selectedTypeFilter = string.Empty;

        // Pola formularza administratora (Dodawanie)
        private string _newTitle;
        private string _newAuthors;
        private int _newYear = DateTime.Now.Year;
        private string _newPublisher;
        private string _newType = "Artykuł";
        private string _newPdf;
        private int _newPages;
        private string _newPhoto;
        private readonly ThemeService _themeService;

        // ==========================================
        // 2. KOLEKCJE I WIDOKI
        // ==========================================
        public ObservableCollection<Projekt> ProjectsList { get; set; }
        public ObservableCollection<Konferencja> ConferencesList { get; set; }
        public ObservableCollection<Publikacja> PublicationsList { get; set; }
        // Ulubione
        public ObservableCollection<Publikacja> FavoritePublications { get; set; }
        public ObservableCollection<Projekt> FavoriteProjects { get; set; }
        public ObservableCollection<Konferencja> FavoriteConferences { get; set; }
        // NOWE: Kolekcja na projekty studenta
        public ObservableCollection<Projekt> MyProjectsList { get; set; }
        public ICollectionView ProjectsView { get; private set; }
        public ICollectionView ConferencesView { get; private set; }
        public ICollectionView PublicationsView { get; private set; }
        public ICommand EditPublicationCommand { get; set; }
        public ICommand EditProjectCommand { get; set; }
        public ICommand SelectPhotoCommand { get; set; }
        public ICommand EditConferenceCommand { get; set; }
        public ICommand JoinProjectCommand { get; }
        public ICommand LeaveProjectCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        // ==========================================
        // 3. WŁAŚCIWOŚCI BINDOWANE DO XAML
        // ==========================================
        public string UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(IsStudent));
            }
        }

        public int CurrentUserId
        {
            get => _currentUserId;
            set { _currentUserId = value; OnPropertyChanged(); }
        }

        public bool IsAdmin => UserRole == "Administrator";
        public bool IsStudent => UserRole == "Student";
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); 
                
               
                // Odświeżamy wszystkie 3 listy naraz po wpisaniu tekstu
                PublicationsView?.Refresh();
                ProjectsView?.Refresh();
                ConferencesView?.Refresh();
            }
        }

        public string SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set { _selectedTypeFilter = value; OnPropertyChanged(); PublicationsView?.Refresh(); }
        }

        // Bindowanie formularza
        public string NewTitle { get => _newTitle; set { _newTitle = value; OnPropertyChanged(); } }
        public string NewAuthors { get => _newAuthors; set { _newAuthors = value; OnPropertyChanged(); } }
        public int NewYear { get => _newYear; set { _newYear = value; OnPropertyChanged(); } }
        public string NewPublisher { get => _newPublisher; set { _newPublisher = value; OnPropertyChanged(); } }
        public string NewType { get => _newType; set { _newType = value; OnPropertyChanged(); } }
        public string NewPdf { get => _newPdf; set { _newPdf = value; OnPropertyChanged(); } }
        public int NewPages { get => _newPages; set { _newPages = value; OnPropertyChanged(); } }
        public string NewPhoto { get => _newPhoto; set { _newPhoto = value; OnPropertyChanged(); } }

        // ==========================================
        // 4. DEKLARACJE KOMEND
        // ==========================================
        public RelayCommand OpenAddWindowCommand { get; }
        public RelayCommand AddPublicationCommand { get; }
        public RelayCommand LogoutCommand { get; }

        // Rozdzielone komendy dla ulubionych (żeby XAML przesyłał tylko ID)
        public RelayCommand FavoritePublicationCommand { get; }
        public RelayCommand FavoriteProjectCommand { get; }
        public RelayCommand FavoriteConferenceCommand { get; }

        // Komendy do usuwania
        public RelayCommand DeletePublicationCommand { get; }
        public RelayCommand DeleteProjectCommand { get; }
        public RelayCommand DeleteConferenceCommand { get; }

        // ==========================================
        // 5. KONSTRUKTOR
        // ==========================================
        // UWAGA: Zaktualizuj miejsce tworzenia obiektu (np. w LoginWindow), 
        // aby przekazywać też ID użytkownika! Np.: new MainViewModel("Student", 1)
        public MainViewModel(string userRole, int userId)
        {
            UserRole = userRole;
            CurrentUserId = userId;
            _db = new DbConnection();

            PublicationsList = new ObservableCollection<Publikacja>();
            ProjectsList = new ObservableCollection<Projekt>();
            ConferencesList = new ObservableCollection<Konferencja>();
            MyProjectsList = new ObservableCollection<Projekt>();
            // Inicjalizacja Ulubionych
            FavoritePublications = new ObservableCollection<Publikacja>();
            FavoriteProjects = new ObservableCollection<Projekt>();
            FavoriteConferences = new ObservableCollection<Konferencja>();
            EditProjectCommand = new RelayCommand(ExecuteEditProject);
            EditConferenceCommand = new RelayCommand(ExecuteEditConference);
            // 2. W konstruktorze MainViewModel:
            EditPublicationCommand = new RelayCommand(ExecuteEditPublication);
            SelectPhotoCommand = new RelayCommand(ExecuteSelectPhoto);
            JoinProjectCommand = new RelayCommand(ExecuteJoinProject);
            LeaveProjectCommand = new RelayCommand(ExecuteLeaveProject);
            _projectService = new ProjectParticipationService(_db);
            LoadProjectsFromSql();
            LoadConferencesFromSql();
            LoadPublicationsFromSql();
            LoadFavoritesFromSql();

            // Inicjalizacja serwisu motywów
            _themeService = new ThemeService(_db);
            ChangeThemeCommand = new RelayCommand(ExecuteChangeTheme);

            // Ładowanie motywu użytkownika zapisanego w DB (wywołaj na końcu konstruktora, gdy znasz już CurrentUserId)
            string savedTheme = _themeService.GetUserTheme(CurrentUserId);
            _themeService.ApplyTheme(savedTheme);

            if (IsStudent) LoadMyProjectsFromSql(); // Ładujemy tylko jeśli to student

            // Dla publikacji:
            PublicationsView = CollectionViewSource.GetDefaultView(PublicationsList);
            PublicationsView.Filter = (obj) => FilterHelper.FilterPublications(obj, SearchText, SelectedTypeFilter);

            // Dla projektów:
            ProjectsView = CollectionViewSource.GetDefaultView(ProjectsList);
            ProjectsView.Filter = (obj) => FilterHelper.FilterProjects(obj, SearchText);

            // Dla konferencji:
            ConferencesView = CollectionViewSource.GetDefaultView(ConferencesList);
            ConferencesView.Filter = (obj) => FilterHelper.FilterConferences(obj, SearchText);

            // Inicjalizacja komend
            OpenAddWindowCommand = new RelayCommand(ExecuteOpenAddWindow, CanExecuteOpenAddWindow);
            AddPublicationCommand = new RelayCommand(ExecuteAddPublication, CanExecuteAddPublication);
            LogoutCommand = new RelayCommand(ExecuteLogout);

            FavoritePublicationCommand = new RelayCommand(id => ExecuteToggleFavorite(id, "Publikacja"));
            FavoriteProjectCommand = new RelayCommand(id => ExecuteToggleFavorite(id, "Projekt"));
            FavoriteConferenceCommand = new RelayCommand(id => ExecuteToggleFavorite(id, "Konferencja"));

            DeletePublicationCommand = new RelayCommand(id => ExecuteDeleteElement(id, "Publikacja"), CanExecuteAdminAction);
            DeleteProjectCommand = new RelayCommand(id => ExecuteDeleteElement(id, "Projekt"), CanExecuteAdminAction);
            DeleteConferenceCommand = new RelayCommand(id => ExecuteDeleteElement(id, "Konferencja"), CanExecuteAdminAction);
        }

        // ==========================================
        // 6. METODY DOSTĘPU DO DANYCH I LOGIKI
        // ==========================================
        private int ReadInt(SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return 0;
            var val = reader.GetValue(index);
            if (val is int i) return i;
            if (val is long l) return (int)l;
            if (val is short s) return (int)s;
            if (val is decimal d) return (int)d;
            return int.TryParse(val?.ToString(), out var parsed) ? parsed : 0;
        }

        private void LoadMyProjectsFromSql()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT p.ID, p.Tytul, p.Opis, p.DataRozpoczecia, p.DataZakonczenia, p.Zdjecie 
                        FROM dbo.Projekt p 
                        JOIN dbo.ProjektStudenci ps ON p.ID = ps.ProjektID 
                        WHERE ps.StudentID = @UserId";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            MyProjectsList.Clear();
                            while (reader.Read())
                            {
                                var projekt = new Projekt(
                                    ReadInt(reader, 0),
                                    reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    reader.GetDateTime(3),
                                    reader.GetDateTime(4)
                                );
                                projekt.NazwaPlikuZdjecia = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                projekt.CzyDolaczono = true;
                                MyProjectsList.Add(projekt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania Twoich projektów: {ex.Message}");
            }
        }
        private void LoadFavoritesFromSql()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    // 1. Publikacje - jawna kolejność indeksów: 0:ID, 1:Tytul, 2:Rok_Wydania, 3:Typ, 4:Wydawnictwo, 5:PlikPDF, 6:Strony, 7:Zdjecie
                    string queryPub = @"SELECT p.ID, p.Tytul, p.Rok_Wydania, p.Typ, p.Wydawnictwo, p.PlikPDF, p.Strony, p.Zdjecie 
                                FROM dbo.Publikacja p 
                                JOIN dbo.Ulubione u ON p.ID = u.ElementID 
                                WHERE u.TypElementu = 'Publikacja' AND u.UzytkownikID = @UserId";
                    LoadSpecificFavorites(conn, queryPub, "Publikacja");

                    // 2. Projekty - jawna kolejność indeksów: 0:ID, 1:Tytul, 2:Opis, 3:DataRozpoczecia, 4:DataZakonczenia, 5:Zdjecie
                    // UWAGA: Upewnij się, że nazwy kolumn (Tytul, Opis itd.) są identyczne jak w Twojej bazie!
                    string queryProj = @"SELECT p.ID, p.Tytul, p.Opis, p.DataRozpoczecia, p.DataZakonczenia, p.Zdjecie 
                                 FROM dbo.Projekt p 
                                 JOIN dbo.Ulubione u ON p.ID = u.ElementID 
                                 WHERE u.TypElementu = 'Projekt' AND u.UzytkownikID = @UserId";
                    LoadSpecificFavorites(conn, queryProj, "Projekt");

                    // 3. Konferencje - jawna kolejność indeksów: 0:ID, 1:Nazwa, 2:Data, 3:Miejsce, 4:Zdjecie
                    // UWAGA: Upewnij się, że nazwy kolumn (Nazwa, Data, Miejsce, Zdjecie) są identyczne jak w Twojej bazie!
                    string queryConf = @"SELECT k.ID, k.Nazwa, k.Data, k.Miejsce, k.Zdjecie 
                                 FROM dbo.Konferencja k 
                                 JOIN dbo.Ulubione u ON k.ID = u.ElementID 
                                 WHERE u.TypElementu = 'Konferencja' AND u.UzytkownikID = @UserId";
                    LoadSpecificFavorites(conn, queryConf, "Konferencja");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania ulubionych: {ex.Message}");
            }
        }
        // Pomocnicza metoda do ładowania konkretnego typu
        private void LoadSpecificFavorites(SqlConnection conn, string query, string type)
        {
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (type == "Publikacja")
                    {
                        FavoritePublications.Clear(); while (reader.Read())
                        {
                            //                        Tutaj analogiczne mapowanie jak w LoadPublicationsFromSql
                            var pub = new Publikacja(
                             ReadInt(reader, 0),
                             reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            ReadInt(reader, 2),
                             reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                             reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                             reader.IsDBNull(5) ? "" : reader.GetString(5),
                             reader.IsDBNull(6) ? 0 : ReadInt(reader, 6)
                         );
                            pub.NazwaPlikuZdjecia = reader.IsDBNull(7) ? "" : reader.GetString(7);
                            FavoritePublications.Add(pub);
                        }
                    }
                    else if (type == "Projekt")
                    {
                        FavoriteProjects.Clear(); while (reader.Read())
                        {
                            var projekt = new Projekt(
                            ReadInt(reader, 0),
                            reader.IsDBNull(1) ? "" : reader.GetString(1),
                            reader.IsDBNull(2) ? "" : reader.GetString(2),
                            reader.GetDateTime(3),
                            reader.GetDateTime(4)
                           );
                            projekt.NazwaPlikuZdjecia = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            FavoriteProjects.Add(projekt);


                        }
                    }
                    else if (type == "Konferencja")
                    {
                        FavoriteConferences.Clear(); // Tego brakowało

                        while (reader.Read())        // Tego brakowało
                        {
                            var konferencja = new Konferencja(
                                 ReadInt(reader, 0),
                                 reader.IsDBNull(1) ? "" : reader.GetString(1),
                                 reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                                 reader.IsDBNull(3) ? "" : reader.GetString(3)
                                );
                            konferencja.NazwaPlikuZdjecia = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            FavoriteConferences.Add(konferencja);
                        }
                    }
                }
            }
        }

        private void ExecuteChangeTheme(object obj)
        {
            if (obj == null) return;
            string selectedTheme = obj.ToString(); // Pobiera np. "Pink", "Light", "Blue" z parametru przycisku

            // 1. Zmień wygląd aplikacji natychmiastowo
            _themeService.ApplyTheme(selectedTheme);

            // 2. Zapisz ustawienie w bazie danych, aby pamiętało przy następnym logowaniu
            _themeService.SaveUserTheme(CurrentUserId, selectedTheme);
        }

        // 3. Metoda wykonująca komendę:
        private void ExecuteSelectPhoto(object obj)
        {
            // Tworzymy okno wyboru pliku z Windowsa
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Wybierz zdjęcie",
                Filter = "Pliki obrazów (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Wszystkie pliki (*.*)|*.*"
            };

            // Jeśli administrator wybrał plik i kliknął "OK"
            if (openFileDialog.ShowDialog() == true)
            {
                // openFileDialog.FileName zwraca pełną ścieżkę (np. C:\obrazy\okladka.png)
                // Jeśli chcesz zapisać TYLKO nazwę pliku (np. okladka.png), odkomentuj poniższą linijkę i usuń tę z NewPhoto:
                // NewPhoto = System.IO.Path.GetFileName(openFileDialog.FileName);

                NewPhoto = openFileDialog.FileName;
            }
        }
        private void ExecuteJoinProject(object obj)
        {
            if (!IsStudent)
            {
                MessageBox.Show("Tylko studenci mogą dołączać!", "Brak uprawnień", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (obj == null || !int.TryParse(obj.ToString(), out int projektId)) return;

            try
            {
                _projectService.JoinProject(projektId, CurrentUserId);

                UpdateProjectStatus(projektId); // Metoda aktualizująca UI
                MessageBox.Show("Udało się! Dołączyłeś do projektu.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                UpdateProjectStatus(projektId);
                MessageBox.Show("Jesteś już uczestnikiem tego projektu!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd SQL: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteLeaveProject(object obj)
        {
            if (!IsStudent) return;
            if (obj == null || !int.TryParse(obj.ToString(), out int projektId)) return;

            var result = MessageBox.Show("Czy na pewno chcesz zrezygnować?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _projectService.LeaveProject(projektId, CurrentUserId);

                // Aktualizacja UI (zostaje w ViewModel, bo to on "posiada" te listy)
                var projektNaGlownej = ProjectsList?.FirstOrDefault(p => p.ID == projektId);
                if (projektNaGlownej != null) projektNaGlownej.CzyDolaczono = false;

                var projektDoUsuniecia = MyProjectsList?.FirstOrDefault(p => p.ID == projektId);
                if (projektDoUsuniecia != null) MyProjectsList.Remove(projektDoUsuniecia);

                MessageBox.Show("Zrezygnowano z projektu.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Pomocnicza metoda, która aktualizuje status w Twojej kolekcji
        private void UpdateProjectStatus(int projektId)
        {
            var projekt = ProjectsList?.FirstOrDefault(p => p.ID == projektId);
            if (projekt != null)
            {
                projekt.CzyDolaczono = true;
            // Aktualizujemy listę Moje Projekty na bieżąco
            if (MyProjectsList != null && !MyProjectsList.Any(p => p.ID == projektId))
            {
                MyProjectsList.Add(projekt);
            }
            }
        }


        private void ExecuteEditProject(object obj)
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Brak uprawnień administratora.");
                return;
            }

            if (obj is Projekt wybranyProjekt)
            {
                var editWindow = new EditProjectWindow(wybranyProjekt, _db);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // 1. Odświeżamy główną listę projektów (wpisz swoją dokładną nazwę metody)
                    LoadProjectsFromSql();

                    // 2. Odświeżamy ulubione
                    LoadFavoritesFromSql();
                }
            }
        }

        private void ExecuteEditConference(object obj)
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Brak uprawnień administratora.");
                return;
            }

            if (obj is Konferencja wybranaKonferencja)
            {
                var editWindow = new EditConferenceWindow(wybranaKonferencja, _db);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Pamiętaj o odświeżeniu głównych list! Wywołaj tu swoje metody ładujące.
                    LoadFavoritesFromSql();
                }
            }
        }
        private void ExecuteEditPublication(object obj)
        {
            if (!IsAdmin)
            {
                MessageBox.Show("Brak uprawnień administratora.");
                return;
            }

            if (obj is Publikacja wybranaPublikacja)
            {
                // Otwieramy nowe okno i przekazujemy mu obiekt publikacji oraz połączenie do bazy
                var editWindow = new EditPublicationWindow(wybranaPublikacja, _db);

                // ShowDialog() zatrzymuje kod, dopóki okno się nie zamknie
                bool? result = editWindow.ShowDialog();

                // Jeśli w oknie edycji kliknięto "Zapisz" (zwrócono true), odświeżamy listę
                if (result == true)
                {
                    // 1. Odświeżamy główną listę publikacji (wpisz swoją dokładną nazwę metody)
                    LoadPublicationsFromSql();

                    // 2. Odświeżamy też ulubione, na wypadek gdyby edytowano element będący w ulubionych
                    LoadFavoritesFromSql();
                }
            }
        }
        private void LoadProjectsFromSql()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    // Zmienione zapytanie - sprawdza Ulubione i Dołączone za pomocą LEFT JOIN
                    string query = @"
                        SELECT p.ID, p.Tytul, p.Opis, p.DataRozpoczecia, p.DataZakonczenia, p.Zdjecie,
                               CASE WHEN u.ElementID IS NOT NULL THEN 1 ELSE 0 END AS CzyUlubione,
                               CASE WHEN ps.ProjektID IS NOT NULL THEN 1 ELSE 0 END AS CzyDolaczono
                        FROM dbo.Projekt p
                        LEFT JOIN dbo.Ulubione u ON p.ID = u.ElementID AND u.TypElementu = 'Projekt' AND u.UzytkownikID = @UserId
                        LEFT JOIN dbo.ProjektStudenci ps ON p.ID = ps.ProjektID AND ps.StudentID = @UserId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        // TA LINIJKA NAPRAWIA BŁĄD (przekazuje wartość ID logującego się użytkownika do SQL)
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            ProjectsList.Clear();
                            while (reader.Read())
                            {
                                var projekt = new Projekt(
                                 ReadInt(reader, 0),
                                 reader.IsDBNull(1) ? "" : reader.GetString(1),
                                 reader.IsDBNull(2) ? "" : reader.GetString(2),
                                 reader.GetDateTime(3),
                                 reader.GetDateTime(4)
                                );
                                projekt.NazwaPlikuZdjecia = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                projekt.CzyUlubione = ReadInt(reader, 6) == 1;
                                projekt.CzyDolaczono = ReadInt(reader, 7) == 1;
                                ProjectsList.Add(projekt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd odczytu projektów: {ex.Message}", "Błąd SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConferencesFromSql()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    //   string query = "SELECT ID, Nazwa, Data, Miejsce, Zdjecie FROM dbo.Konferencja";
                    string query = @"
                        SELECT k.ID, k.Nazwa, k.Data, k.Miejsce, k.Zdjecie,
                               CASE WHEN u.ElementID IS NOT NULL THEN 1 ELSE 0 END AS CzyUlubione
                        FROM dbo.Konferencja k
                        LEFT JOIN dbo.Ulubione u ON k.ID = u.ElementID AND u.TypElementu = 'Konferencja' AND u.UzytkownikID = @UserId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            ConferencesList.Clear();
                            while (reader.Read())
                            {
                                var konferencja = new Konferencja(
                                 ReadInt(reader, 0),
                                 reader.IsDBNull(1) ? "" : reader.GetString(1),
                                 reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                                 reader.IsDBNull(3) ? "" : reader.GetString(3)
                                );
                                konferencja.NazwaPlikuZdjecia = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                konferencja.CzyUlubione = ReadInt(reader, 5) == 1;
                                ConferencesList.Add(konferencja);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd odczytu konferencji: {ex.Message}", "Błąd SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPublicationsFromSql()
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    //  string query = "SELECT ID, Tytul, Rok_Wydania, Typ, Wydawnictwo, PlikPDF, Strony, Zdjecie FROM dbo.Publikacja";
                    string query = @"
                        SELECT p.ID, p.Tytul, p.Rok_Wydania, p.Typ, p.Wydawnictwo, p.PlikPDF, p.Strony, p.Zdjecie,
                               CASE WHEN u.ElementID IS NOT NULL THEN 1 ELSE 0 END AS CzyUlubione
                        FROM dbo.Publikacja p
                        LEFT JOIN dbo.Ulubione u ON p.ID = u.ElementID AND u.TypElementu = 'Publikacja' AND u.UzytkownikID = @UserId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            PublicationsList.Clear();
                            while (reader.Read())
                            {
                                var pub = new Publikacja(
                                    ReadInt(reader, 0),
                                    reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    ReadInt(reader, 2),
                                    reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                    reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                    reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    reader.IsDBNull(6) ? 0 : ReadInt(reader, 6)
                                );
                                pub.NazwaPlikuZdjecia = reader.IsDBNull(7) ? "" : reader.GetString(7);
                                pub.CzyUlubione = ReadInt(reader, 8) == 1;
                                PublicationsList.Add(pub);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd odczytu publikacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==========================================
        // 7. IMPLEMENTACJA KOMEND (CAN EXECUTE / EXECUTE)
        // ==========================================
        private bool CanExecuteAdminAction(object obj) => IsAdmin;

        private bool CanExecuteOpenAddWindow(object obj) => IsAdmin;

        private void ExecuteOpenAddWindow(object obj)
        {
            NewTitle = string.Empty;
            NewPublisher = string.Empty;
            NewPdf = string.Empty;
            NewYear = DateTime.Now.Year;
            NewPages = 1;
            NewType = "Artykuł";
            NewPhoto = string.Empty;

            var addWindow = new View.AddRecordWindow();
            addWindow.DataContext = this;
            addWindow.ShowDialog();
        }

        private bool CanExecuteAddPublication(object obj)
        {
            return IsAdmin && !string.IsNullOrWhiteSpace(NewTitle) && NewPages > 0;
        }

        private void ExecuteAddPublication(object obj)
        {
            if (!IsAdmin) return;

            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO dbo.Publikacja (Tytul, Rok_Wydania, Typ, Wydawnictwo, PlikPDF, Strony, Zdjecie) " +
                                   "VALUES (@Tytul, @Rok, @Typ, @Wydawnictwo, @Pdf, @Strony, @Zdjecie); SELECT SCOPE_IDENTITY();";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Tytul", NewTitle);
                        cmd.Parameters.AddWithValue("@Rok", NewYear);
                        cmd.Parameters.AddWithValue("@Typ", NewType);
                        cmd.Parameters.AddWithValue("@Wydawnictwo", string.IsNullOrWhiteSpace(NewPublisher) ? string.Empty : NewPublisher);
                        cmd.Parameters.AddWithValue("@Pdf", string.IsNullOrWhiteSpace(NewPdf) ? "brak_pliku.pdf" : NewPdf);
                        cmd.Parameters.AddWithValue("@Strony", NewPages);
                        cmd.Parameters.AddWithValue("@Zdjecie", string.IsNullOrWhiteSpace(NewPhoto) ? (object)DBNull.Value : NewPhoto);

                        int insertedId = Convert.ToInt32(cmd.ExecuteScalar());

                        var nowaPublikacja = new Publikacja(
                            insertedId, NewTitle, NewYear, NewType, NewPublisher,
                            string.IsNullOrWhiteSpace(NewPdf) ? "brak_pliku.pdf" : NewPdf, NewPages)
                        {
                            NazwaPlikuZdjecia = NewPhoto
                        };

                        PublicationsList.Add(nowaPublikacja);
                        MessageBox.Show("Dodano poprawnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (obj is Window window) window.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteElement(object obj, string tableName)
        {
            // 1. Sprawdź uprawnienia z komunikatem
            if (!IsAdmin)
            {
                MessageBox.Show("Nie masz uprawnień administratora!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Bezpieczna konwersja ID
            if (obj == null || !int.TryParse(obj.ToString(), out int id))
            {
                MessageBox.Show($"Nieprawidłowe ID elementu: {obj}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć ten element z tabeli {tableName}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    // ROZPOCZYNAMY TRANSAKCJĘ
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Krok A: Usunięcie powiązań z tabeli Ulubione
                            string deleteFavs = "DELETE FROM dbo.Ulubione WHERE ElementID = @Id AND TypElementu = @Typ";
                            using (var cmdFav = new SqlCommand(deleteFavs, conn, transaction)) // Dodano parametr transakcji
                            {
                                cmdFav.Parameters.AddWithValue("@Id", id);
                                cmdFav.Parameters.AddWithValue("@Typ", tableName);
                                cmdFav.ExecuteNonQuery();
                            }

                            // Krok B: Odpięcie klucza obcego, jeśli usuwamy PROJEKT
                            // (Upewnij się, że nazwa tabeli w bazie to "Projekt" czy "Projekty")
                            if (tableName == "Projekt" || tableName == "Projekty")
                            {
                                string odepnijProjekt = "UPDATE dbo.Publikacja SET ProjektID = NULL WHERE ProjektID = @Id";
                                using (var cmdOdepnij = new SqlCommand(odepnijProjekt, conn, transaction))
                                {
                                    cmdOdepnij.Parameters.AddWithValue("@Id", id);
                                    cmdOdepnij.ExecuteNonQuery();
                                }
                            }

                            // Krok C: Właściwe usunięcie rekordu z głównej tabeli
                            string query = $"DELETE FROM dbo.{tableName} WHERE ID = @Id";
                            using (var cmd = new SqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                cmd.ExecuteNonQuery();
                            }

                            // ZATWIERDZENIE TRANSAKCJI - wszystkie 3 kroki się udały
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // Jeśli cokolwiek po drodze wywali błąd (np. krok C), cofamy kroki A i B
                            transaction.Rollback();
                            throw; // Wyrzucamy błąd wyżej, żeby złapał go główny catch na dole
                        }
                    }
                }

                // Usuwanie z kolekcji w interfejsie bez konieczności przeładowywania z bazy
                if (tableName == "Publikacja")
                {
                    var item = PublicationsList.FirstOrDefault(x => x.ID == id);
                    if (item != null) PublicationsList.Remove(item);
                }
                else if (tableName == "Projekt")
                {
                    var item = ProjectsList.FirstOrDefault(x => x.ID == id);
                    if (item != null) ProjectsList.Remove(item);
                }
                else if (tableName == "Konferencja")
                {
                    var item = ConferencesList.FirstOrDefault(x => x.ID == id);
                    if (item != null) ConferencesList.Remove(item);
                }
                LoadFavoritesFromSql(); // Odśwież zakładkę Ulubione po usunięciu

                MessageBox.Show("Element został pomyślnie usunięty.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteToggleFavorite(object obj, string elementType)
        {
            if (!(obj is int elementId)) return;

            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(1) FROM dbo.Ulubione WHERE UzytkownikID = @UserId AND ElementID = @ElementId AND TypElementu = @Type";
                    bool exists;

                    using (var checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        checkCmd.Parameters.AddWithValue("@ElementId", elementId);
                        checkCmd.Parameters.AddWithValue("@Type", elementType);
                        exists = (int)checkCmd.ExecuteScalar() > 0;
                    }

                    if (exists)
                    {
                        string deleteQuery = "DELETE FROM dbo.Ulubione WHERE UzytkownikID = @UserId AND ElementID = @ElementId AND TypElementu = @Type";
                        using (var deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                            deleteCmd.Parameters.AddWithValue("@ElementId", elementId);
                            deleteCmd.Parameters.AddWithValue("@Type", elementType);
                            deleteCmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Usunięto z ulubionych.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO dbo.Ulubione (UzytkownikID, ElementID, TypElementu) VALUES (@UserId, @ElementId, @Type)";
                        using (var insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                            insertCmd.Parameters.AddWithValue("@ElementId", elementId);
                            insertCmd.Parameters.AddWithValue("@Type", elementType);
                            insertCmd.ExecuteNonQuery();
                        }
                      //  MessageBox.Show("Dodano do ulubionych! ⭐", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    // Odwróć wartość w głównej liście (UI zaktualizuje przycisk natychmiast)
                    if (elementType == "Publikacja")
                    {
                        var item = PublicationsList.FirstOrDefault(p => p.ID == elementId);
                        if (item != null) item.CzyUlubione = !exists;
                    }
                    else if (elementType == "Projekt")
                    {
                        var item = ProjectsList.FirstOrDefault(p => p.ID == elementId);
                        if (item != null) item.CzyUlubione = !exists;
                    }
                    else if (elementType == "Konferencja")
                    {
                        var item = ConferencesList.FirstOrDefault(p => p.ID == elementId);
                        if (item != null) item.CzyUlubione = !exists;
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd systemu ulubionych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadFavoritesFromSql();
          //  MessageBox.Show("Zaktualizowano listę ulubionych.");
        }

        private void ExecuteLogout(object obj)
        {
            var loginWindow = new View.LoginWindow();
            loginWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        // ==========================================
        // 8. OBSŁUGA ZDARZEŃ
        // ==========================================
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}