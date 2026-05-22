using Microsoft.Data.SqlClient;
using ResearchHub.Model;
using ResearchHub.Database; // Dodaj ten using, aby widzieć klasę DbConnection
using System;
using System.Windows;

namespace ResearchHub.View
{
    public partial class EditPublicationWindow : Window
    {
        private Publikacja _publikacja;
        private DbConnection _db; // Zmiana z SqlConnection na Twoją klasę DbConnection


        // Konstruktor przyjmuje teraz prawidłowy typ z MainViewModel
        public EditPublicationWindow(Publikacja pub, DbConnection db)
        {
            InitializeComponent();
            _publikacja = pub;
            _db = db;

            // Wypełnienie TextBoxów obecnymi danymi
            txtTytul.Text = pub.Tytul;
            txtRok.Text = pub.RokWydania.ToString();
            txtWydawnictwo.Text = pub.Wydawnictwo;
            txtTyp.Text = pub.Typ;
            txtPlik.Text = pub.PlikPdf;
            txtStrony.Text = pub.Strony.ToString();
            txtZdjecie.Text = pub.SciezkaZdjecia; // lub jakkolwiek nazywa się to pole w Twojej klasie Publikacja
        }

        private void btnZapisz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_db == null)
                    throw new InvalidOperationException("Menedżer bazy danych jest nullem.");

                // Pobieramy połączenie z Twojego menedżera (używając klauzuli using, która sama zamknie połączenie)
                using (SqlConnection conn = _db.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                        conn.Open();

                    string query = @"UPDATE dbo.Publikacja 
                                     SET Tytul = @Tytul, 
                                         Rok_Wydania = @Rok, 
                                         Wydawnictwo = @Wydawnictwo,
                                         Typ = @Typ,
                                         PlikPDF = @PlikPDF,
                                         Strony = @Strony,
                                         Zdjecie = @Zdjecie
                                     WHERE ID = @Id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _publikacja.ID);
                        cmd.Parameters.AddWithValue("@Tytul", txtTytul.Text);

                        if (int.TryParse(txtRok.Text, out int rok))
                            cmd.Parameters.AddWithValue("@Rok", rok);
                        else
                            cmd.Parameters.AddWithValue("@Rok", DBNull.Value);

                        cmd.Parameters.AddWithValue("@Wydawnictwo", txtWydawnictwo.Text);
                        cmd.Parameters.AddWithValue("@Typ", txtTyp.Text);
                        cmd.Parameters.AddWithValue("@PlikPDF", txtPlik.Text);
                        cmd.Parameters.AddWithValue("@Strony", txtStrony.Text);
                        // Bezpieczne podejście: jeśli nic nie wybrano, zapisujemy w bazie NULL
                        if (string.IsNullOrWhiteSpace(txtZdjecie.Text))
                            cmd.Parameters.AddWithValue("@Zdjecie", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Zdjecie", txtZdjecie.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Zaktualizowano pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnWybierzZdjecie_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Zmień zdjęcie",
                Filter = "Pliki obrazów (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Wrzucamy wybraną ścieżkę do TextBoxa (tylko do odczytu), żeby użytkownik widział, co wybrał
                txtZdjecie.Text = openFileDialog.FileName;

                // Jeśli do bazy zapisujesz samą nazwę pliku, użyj:
                // txtZdjecie.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
            }
        }
        private void btnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}