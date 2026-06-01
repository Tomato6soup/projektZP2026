using Microsoft.Win32;
using ResearchHub.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResearchHub.View
{
    /// <summary>
    /// Interaction logic for AddEditProjectWindow.xaml
    /// </summary>
    public partial class AddEditProjectWindow : Window
    {
        // Właściwość, do której bindowany jest formularz w XAML
        public Projekt EdytowanyProjekt { get; set; }

        // KONSTRUKTOR 1: Wywoływany przy TWORZENIU NOWEGO projektu
        public AddEditProjectWindow()
        {
            InitializeComponent();

            // Przekazujemy domyślne wartości do wymaganego konstruktora klasy Projekt (C# 9.0)
            EdytowanyProjekt = new(0, string.Empty, string.Empty, DateTime.Now, DateTime.Now.AddMonths(1))
            {
                CzyRekrutacjaOtwarta = true
            };

            DataContext = this;
        }

        // KONSTRUKTOR 2: Wywoływany przy EDYCJI ISTNIEJĄCEGO projektu
        public AddEditProjectWindow(Projekt projektDoEdycji)
        {
            InitializeComponent();

            // Przypisujemy przekazany projekt do właściwości
            EdytowanyProjekt = projektDoEdycji;

            DataContext = this;
            Title = "Edycja Projektu"; // Zmieniamy tytuł okna
        }

        private void BtnWybierzZdjecie_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Wybierz zdjęcie projektu",
                Filter = "Pliki obrazów (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Aktualizujemy właściwość. W pełnej wersji aplikacji wypadałoby plik skopiować
                // do jakiegoś folderu wewnątrz projektu, np. "Assets/Images/"
                EdytowanyProjekt.NazwaPlikuZdjecia = openFileDialog.FileName;

                // Odświeżenie interfejsu (wymuszenie, by XAML zauważył zmianę nazwy pliku)
                // Najprościej w code-behind bez pełnego MVVM:
                DataContext = null;
                DataContext = this;
            }
        }

        private void BtnZapisz_Click(object sender, RoutedEventArgs e)
        {
            // Podstawowa walidacja
            if (string.IsNullOrWhiteSpace(EdytowanyProjekt.Tytul))
            {
                MessageBox.Show("Podaj tytuł projektu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ustawienie DialogResult na true automatycznie zamyka okno 
            // i daje sygnał do Głównego Okna, że użytkownik kliknął "Zapisz"
            DialogResult = true;
        }

        private void BtnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            // Ustawienie DialogResult na false oznacza anulowanie akcji
            DialogResult = false;
        }
    }
}
