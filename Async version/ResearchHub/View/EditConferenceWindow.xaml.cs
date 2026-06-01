using Microsoft.Data.SqlClient;
using ResearchHub.Model;
using ResearchHub.Database;
using System;
using System.Windows;

namespace ResearchHub.View
{
    public partial class EditConferenceWindow : Window
    {
        private Konferencja _konferencja;
        private DbConnection _db;

        public EditConferenceWindow(Konferencja konf, DbConnection db)
        {
            InitializeComponent();
            _konferencja = konf;
            _db = db;

            txtNazwa.Text = konf.Nazwa;
            dpData.SelectedDate = konf.Data;
            txtMiejsce.Text = konf.Miejsce;
        }

        private void btnZapisz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = _db.GetConnection())
                {
                    if (conn.State != System.Data.ConnectionState.Open) conn.Open();

                    string query = @"UPDATE dbo.Konferencja 
                                     SET Nazwa = @Nazwa, Data = @Data, Miejsce = @Miejsce 
                                     WHERE ID = @Id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", _konferencja.ID);
                        cmd.Parameters.AddWithValue("@Nazwa", txtNazwa.Text);
                        cmd.Parameters.AddWithValue("@Data", dpData.SelectedDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Miejsce", txtMiejsce.Text);

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Zaktualizowano konferencję!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
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