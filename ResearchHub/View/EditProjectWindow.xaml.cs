using Microsoft.Data.SqlClient;
using ResearchHub.Model;
using ResearchHub.Database;
using System;
using System.Windows;

namespace ResearchHub.View
{
    public partial class EditProjectWindow : Window
    {
        private Projekt _projekt;
        private DbConnection _db;

        public EditProjectWindow(Projekt projekt, DbConnection db)
        {
            InitializeComponent();
            _projekt = projekt;
            _db = db;

            // Wypełnianie danych
            txtNazwa.Text = projekt.Tytul;
            txtOpis.Text = projekt.Opis;
            dpStart.SelectedDate = projekt.DataRozpoczecia;
            dpKoniec.SelectedDate = projekt.DataZakonczenia;
        }

        private void btnZapisz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    string query = @"UPDATE dbo.Projekt 
                                     SET Tytul = @Nazwa, Opis = @Opis, 
                                         DataRozpoczecia = @Start, DataZakonczenia = @Koniec 
                                     WHERE ID = @Id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _projekt.ID);
                        cmd.Parameters.AddWithValue("@Nazwa", txtNazwa.Text);
                        cmd.Parameters.AddWithValue("@Opis", txtOpis.Text);

                        // Zapisywanie dat (jeśli kalendarz jest pusty, zapiszemy NULL)
                        cmd.Parameters.AddWithValue("@Start", dpStart.SelectedDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Koniec", dpKoniec.SelectedDate ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Zaktualizowano projekt!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAnuluj_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}