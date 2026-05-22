using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace BazaPublikacji_app
{
    public partial class MainForm
    {
        // ----------------- WYSZUKIWARKA -----------------
        private void TxtSzukaj_TextChanged(object sender, EventArgs e)
        {
            string filtr = txtSzukaj.Text.ToLower();

            pnlPublikacje.Controls.Clear();
            foreach (var pub in publikacjeVM.Publikacje.Where(p => p.Tytul.ToLower().Contains(filtr)))
                pnlPublikacje.Controls.Add(StworzCardPublikacja(pub));

            pnlProjekty.Controls.Clear();
            foreach (var proj in projektyVM.Projekty.Where(p => p.Tytul.ToLower().Contains(filtr)))
                pnlProjekty.Controls.Add(StworzCardProjekt(proj));

            pnlKonferencje.Controls.Clear();
            foreach (var k in konferencjeVM.Konferencje.Where(k => k.Nazwa.ToLower().Contains(filtr)))
                pnlKonferencje.Controls.Add(StworzCardKonferencja(k));
        }

        // ----------------- FILTR -----------------
        private void BtnFiltruj_Click(object sender, EventArgs e)
        {
            // Tworzenie formularza filtrów
            Form filtrForm = new Form
            {
                Text = "Filtr publikacji",
                Size = new Size(350, 220),
                StartPosition = FormStartPosition.CenterParent
            };

            Label lblRok = new Label { Text = "Rok:", Location = new Point(20, 20) };
            ComboBox cmbRok = new ComboBox { Location = new Point(120, 20), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblTyp = new Label { Text = "Typ:", Location = new Point(20, 60) };
            ComboBox cmbTyp = new ComboBox { Location = new Point(120, 60), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblWydawnictwo = new Label { Text = "Wydawnictwo:", Location = new Point(20, 100) };
            ComboBox cmbWydawnictwo = new ComboBox { Location = new Point(120, 100), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT Rok_Wydania FROM Publikacja ORDER BY Rok_Wydania", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) cmbRok.Items.Add(reader["Rok_Wydania"].ToString());
                reader.Close();

                cmd.CommandText = "SELECT DISTINCT Typ FROM Publikacja ORDER BY Typ";
                reader = cmd.ExecuteReader();
                while (reader.Read()) cmbTyp.Items.Add(reader["Typ"].ToString());
                reader.Close();

                cmd.CommandText = "SELECT DISTINCT Wydawnictwo FROM Publikacja ORDER BY Wydawnictwo";
                reader = cmd.ExecuteReader();
                while (reader.Read()) cmbWydawnictwo.Items.Add(reader["Wydawnictwo"].ToString());
            }

            Button btnZastosuj = new Button { Text = "Zastosuj", Location = new Point(120, 150), Width = 100 };
            btnZastosuj.Click += (s, ev) =>
            {
                ZastosujFiltr(cmbRok.SelectedItem?.ToString(), cmbTyp.SelectedItem?.ToString(), cmbWydawnictwo.SelectedItem?.ToString());
                filtrForm.Close();
            };

            filtrForm.Controls.AddRange(new Control[] { lblRok, cmbRok, lblTyp, cmbTyp, lblWydawnictwo, cmbWydawnictwo, btnZastosuj });
            filtrForm.ShowDialog();
        }

        // ----------------- DODAWANIE PUBLIKACJI -----------------
        private void BtnDodajPublikacje_Click(object sender, EventArgs e)
        {
            var dlg = new Form
            {
                Text = "Dodaj publikację",
                Size = new Size(420, 340),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblTytul = new Label { Text = "Tytuł:", Location = new Point(20, 20), AutoSize = true };
            var txtTytul = new TextBox { Location = new Point(140, 18), Width = 240 };

            var lblRok = new Label { Text = "Rok wydania:", Location = new Point(20, 60), AutoSize = true };
            var txtRok = new TextBox { Location = new Point(140, 58), Width = 240 };

            var lblTyp = new Label { Text = "Typ:", Location = new Point(20, 100), AutoSize = true };
            var txtTyp = new TextBox { Location = new Point(140, 98), Width = 240 };

            var lblWydawnictwo = new Label { Text = "Wydawnictwo:", Location = new Point(20, 140), AutoSize = true };
            var txtWydawnictwo = new TextBox { Location = new Point(140, 138), Width = 240 };

            var lblPlik = new Label { Text = "Plik PDF:", Location = new Point(20, 180), AutoSize = true };
            var txtPlik = new TextBox { Location = new Point(140, 178), Width = 200, ReadOnly = true };
            var btnBrowse = new Button { Text = "Wybierz...", Location = new Point(350, 176), Size = new Size(30, 24) };

            var lblStrony = new Label { Text = "Strony:", Location = new Point(20, 220), AutoSize = true };
            var txtStrony = new TextBox { Location = new Point(140, 218), Width = 240 };

            var btnOk = new Button { Text = "Dodaj", Location = new Point(140, 260), Width = 100 };
            var btnCancel = new Button { Text = "Anuluj", Location = new Point(280, 260), Width = 100 };

            btnBrowse.Click += (s, ev) =>
            {
                using (var ofd = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*", Title = "Wybierz plik PDF" })
                {
                    if (ofd.ShowDialog(this) == DialogResult.OK) txtPlik.Text = ofd.FileName;
                }
            };

            btnCancel.Click += (s, ev) => dlg.Close();

            btnOk.Click += (s, ev) =>
            {
                string tytul = txtTytul.Text.Trim();
                if (string.IsNullOrEmpty(tytul))
                {
                    MessageBox.Show("Tytuł jest wymagany.", "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int? rokVal = null;
                if (int.TryParse(txtRok.Text.Trim(), out int rokParsed)) rokVal = rokParsed;

                int? stronyVal = null;
                if (int.TryParse(txtStrony.Text.Trim(), out int stronyParsed)) stronyVal = stronyParsed;

                string typ = string.IsNullOrWhiteSpace(txtTyp.Text) ? null : txtTyp.Text.Trim();
                string wydawnictwo = string.IsNullOrWhiteSpace(txtWydawnictwo.Text) ? null : txtWydawnictwo.Text.Trim();
                string plikPdf = string.IsNullOrWhiteSpace(txtPlik.Text) ? null : txtPlik.Text.Trim();

                string correlationId = Guid.NewGuid().ToString();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        string sql = "INSERT INTO Publikacja (Tytul, Rok_Wydania, Typ, Wydawnictwo, PlikPDF, Strony) VALUES (@Tytul, @Rok, @Typ, @Wydawnictwo, @PlikPDF, @Strony)";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Tytul", (object)tytul);
                            cmd.Parameters.AddWithValue("@Rok", (object)rokVal ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Typ", (object)typ ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Wydawnictwo", (object)wydawnictwo ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@PlikPDF", (object)plikPdf ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Strony", (object)stronyVal ?? DBNull.Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Odświeżenie widoku w bezpieczny sposób
                    publikacjeVM.ZaladujPublikacje();
                    WyswietlPublikacje();

                    MessageBox.Show($"Publikacja została dodana.\r\nId śledzenia: {correlationId}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dlg.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas dodawania publikacji.\r\nId śledzenia: {correlationId}\r\n{ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            dlg.Controls.AddRange(new Control[] { lblTytul, txtTytul, lblRok, txtRok, lblTyp, txtTyp, lblWydawnictwo, txtWydawnictwo, lblPlik, txtPlik, btnBrowse, lblStrony, txtStrony, btnOk, btnCancel });
            dlg.ShowDialog(this);
        }


    }
}