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
    public partial class AddConferenceWindow : Window
    {
        // Ta właściwość przetrzyma wpisane przez użytkownika dane
        public Konferencja EdytowanaKonferencja { get; set; }

        public AddConferenceWindow()
        {
            InitializeComponent();

            // Inicjujemy z pustymi danymi i dzisiejszą datą
            EdytowanaKonferencja = new Konferencja(0, string.Empty, DateTime.Now, string.Empty);

            DataContext = this;
        }

        private void BtnWybierzZdjecie_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Wybierz grafikę konferencji",
                Filter = "Pliki obrazów (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                EdytowanaKonferencja.NazwaPlikuZdjecia = openFileDialog.FileName;

                // Odświeżenie interfejsu, żeby TextBox ze ścieżką zaktualizował tekst
                DataContext = null;
                DataContext = this;
            }
        }

        private void BtnZapisz_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EdytowanaKonferencja.Nazwa) || string.IsNullOrWhiteSpace(EdytowanaKonferencja.Miejsce))
            {
                MessageBox.Show("Nazwa i miejsce są wymagane!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true; // Zamyka okno i daje sygnał "Sukces" do ViewModelu
        }

        private void BtnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Anulowanie
        }
    }
}
