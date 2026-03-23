using BazaPublikacji_app.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BazaPublikacji_app
{
    public partial class MainForm : Form
    {
        // ----------------- MODELE -----------------
        private readonly PublikacjeViewModel publikacjeVM = new();
        private readonly ProjektyViewModel projektyVM = new();
        private readonly KonferencjeViewModel konferencjeVM = new();
        private readonly string connString = "Server=.\\SQLEXPRESS;Database=BazaPublikacjiUBB;Trusted_Connection=True;TrustServerCertificate=True;";

        // ----------------- KONTROLKI -----------------
        private Panel panelBoczny;
        private Panel panelGorny;
        private TabControl zakladki;
        private Panel pnlDetails;
        private TextBox txtDetails;

        private FlowLayoutPanel pnlUlubione;
        private FlowLayoutPanel pnlPublikacje;
        private FlowLayoutPanel pnlProjekty;
        private FlowLayoutPanel pnlKonferencje;

        private TextBox txtSzukaj;
        private Button btnFiltruj;
        private Button btnTemat;

        private Label lblPowitanie;

        // ----------------- STAN UŻYTKOWNIKA -----------------
        private string zalogowanyUzytkownik;
        private string rolaUzytkownika;
        private bool darkTheme = true;

        // ----------------- KONSTRUKTOR -----------------
        public MainForm(string login, string rola)
        {
            zalogowanyUzytkownik = login;
            rolaUzytkownika = rola;

            InitializeComponent();
            ZaladujDane();
            WyswietlWszystkieZakladki();
            WyswietlPowitanie();
        }

        // ----------------- INICJALIZACJA FORMULARZA -----------------
        private void InitializeComponent()
        {
            Text = "📚 Baza Publikacji";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;

            // ---------- PANEL BOCZNY ----------
            panelBoczny = new Panel
            {
                Dock = DockStyle.Left,
                Width = 180,
                BackColor = Color.FromArgb(40, 40, 60)
            };
            Controls.Add(panelBoczny);

            string[] menuItems = { "Strona główna", "Projekty", "Publikacje", "Konferencje" };
            foreach (var item in menuItems)
            {
                var btn = new Button
                {
                    Text = item,
                    Dock = DockStyle.Top,
                    Height = 50,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => zakladki.SelectedTab = zakladki.TabPages[item];
                panelBoczny.Controls.Add(btn);
                btn.BringToFront();
            }

            // ---------- PANEL GÓRNY ----------
            panelGorny = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(60, 60, 90)
            };
            Controls.Add(panelGorny);

            // --- Wyszukiwarka ---
            txtSzukaj = new TextBox
            {
                PlaceholderText = "Szukaj...",
                Location = new Point(200, 15),
                Width = 300
            };
            txtSzukaj.TextChanged += TxtSzukaj_TextChanged;
            panelGorny.Controls.Add(txtSzukaj);

            // --- Przycisk filtr ---
            btnFiltruj = new Button
            {
                Text = "Filtruj",
                Location = new Point(520, 12),
                Width = 100,
                Height = 30
            };
            btnFiltruj.Click += BtnFiltruj_Click;
            panelGorny.Controls.Add(btnFiltruj);

            // --- Przycisk zmiany tematu ---
            btnTemat = new Button
            {
                Text = "🌙",
                Location = new Point(640, 12),
                Width = 50,
                Height = 30
            };
            btnTemat.Click += (s, e) => PrzelaczTeme();
            panelGorny.Controls.Add(btnTemat);

            // --- Panel szczegółów po prawej stronie ---
            pnlDetails = new Panel
            {
                Dock = DockStyle.Right,
                Width = 300,
                BackColor = Color.WhiteSmoke,
                Visible = false
            };
            txtDetails = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10)
            };
            pnlDetails.Controls.Add(txtDetails);

            // --- Przycisk zamknięcia panelu szczegółów ---
            Button btnClose = new Button
            {
                Text = "✖",
                Size = new Size(25, 25),
                Location = new Point(pnlDetails.Width - 35, 5)
            };
            btnClose.Click += (s, e) => pnlDetails.Visible = false;
            pnlDetails.Controls.Add(btnClose);
            btnClose.BringToFront();

            Controls.Add(pnlDetails);
            pnlDetails.BringToFront();

            // ---------- TABCONTROL ----------
            zakladki = new TabControl
            {
                Dock = DockStyle.Fill,
                Alignment = TabAlignment.Top
            };
            Controls.Add(zakladki);
            zakladki.BringToFront();

            var zakStronaGlowna = new TabPage("Strona główna") { Name = "Strona główna" };
            var zakProjekty = new TabPage("Projekty") { Name = "Projekty" };
            var zakPublikacje = new TabPage("Publikacje") { Name = "Publikacje" };
            var zakKonferencje = new TabPage("Konferencje") { Name = "Konferencje" };
            zakladki.TabPages.AddRange(new[] { zakStronaGlowna, zakProjekty, zakPublikacje, zakKonferencje });

            // ---------- STRONA GŁÓWNA ----------
            lblPowitanie = new Label
            {
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            zakStronaGlowna.Controls.Add(lblPowitanie);

            var lblOpis = new Label
            {
                Text = "W tej bazie znajdziesz publikacje, projekty i konferencje.\n" +
                       "Możesz je przeglądać, filtrować i dodawać do ulubionych.\n" +
                       "Używaj wyszukiwarki i filtrów, aby szybko znaleźć potrzebne informacje.",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(20, 60),
                AutoSize = true
            };
            zakStronaGlowna.Controls.Add(lblOpis);

            var lblUlubione = new Label
            {
                Text = "Ulubione publikacje:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 120),
                AutoSize = true
            };
            zakStronaGlowna.Controls.Add(lblUlubione);

            pnlUlubione = new FlowLayoutPanel
            {
                Location = new Point(20, 160),
                Size = new Size(740, 400),
                AutoScroll = true
            };
            zakStronaGlowna.Controls.Add(pnlUlubione);
            // ---------- PANEL KARTY ----------
            pnlPublikacje = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true };
            zakPublikacje.Controls.Add(pnlPublikacje);

            pnlProjekty = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true };
            zakProjekty.Controls.Add(pnlProjekty);

            pnlKonferencje = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true };
            zakKonferencje.Controls.Add(pnlKonferencje);

            // ---------- PRZYCISK DODAWANIA PUBLIKACJI DLA ADMIN ----------
            if (rolaUzytkownika == "Administrator")
            {
                var btnDodajPublikacje = new Button
                {
                    Text = "Dodaj publikację",
                    Location = new Point(panelGorny.Width - 160, 15),
                    Width = 150,
                    Height = 30,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnDodajPublikacje.Click += BtnDodajPublikacje_Click;
                panelGorny.Controls.Add(btnDodajPublikacje);
            }

            // ---------- TEMA DOMYŚLNA ----------
            darkTheme = true;
            UstawTeme();
        }

        // ----------------- ŁADOWANIE DANYCH -----------------
        private void ZaladujDane()
        {
            publikacjeVM.ZaladujPublikacje();
            projektyVM.ZaladujProjekty();
            konferencjeVM.ZaladujKonferencje();
        }

        // ----------------- WYŚWIETLANIE -----------------
        private void WyswietlWszystkieZakladki()
        {
            WyswietlPublikacje();
            WyswietlProjekty();
            WyswietlKonferencje();
            WyswietlUlubione();
        }

        private void WyswietlPowitanie()
        {
            lblPowitanie.Text = $"Witaj, {zalogowanyUzytkownik}!";
        }

        private void WyswietlPublikacje()
        {
            pnlPublikacje.Controls.Clear();
            foreach (var pub in publikacjeVM.Publikacje)
                pnlPublikacje.Controls.Add(StworzCardPublikacja(pub));
        }

        private void WyswietlProjekty()
        {
            pnlProjekty.Controls.Clear();
            foreach (var proj in projektyVM.Projekty)
                pnlProjekty.Controls.Add(StworzCardProjekt(proj));
        }

        private void WyswietlKonferencje()
        {
            pnlKonferencje.Controls.Clear();
            foreach (var konf in konferencjeVM.Konferencje)
                pnlKonferencje.Controls.Add(StworzCardKonferencja(konf));
        }

        private void WyswietlUlubione()
        {
            pnlUlubione.Controls.Clear();
            foreach (var pub in publikacjeVM.UlubionePublikacje)
                pnlUlubione.Controls.Add(StworzCardPublikacja(pub));
        }

        // ----------------- TEMAT -----------------
        private void PrzelaczTeme()
        {
            darkTheme = !darkTheme;
            UstawTeme();
        }

        private void UstawTeme()
        {
            // Kolory
            Color backMain = darkTheme ? Color.FromArgb(40, 40, 60) : Color.White;
            Color backTop = darkTheme ? Color.FromArgb(60, 60, 90) : Color.LightGray;
            Color backSide = darkTheme ? Color.FromArgb(30, 30, 50) : Color.LightGray;
            Color backCard = darkTheme ? Color.FromArgb(50, 50, 70) : Color.White;
            Color foreColor = darkTheme ? Color.White : Color.Black;
            Color hoverCard = darkTheme ? Color.FromArgb(180, 150, 255) : Color.FromArgb(240, 240, 255);
            Color btnCardColor = darkTheme ? Color.FromArgb(70, 70, 90) : Color.White;

            // Kolory formy i paneli
            this.BackColor = backMain;
            panelGorny.BackColor = backTop;
            panelBoczny.BackColor = backSide;

            foreach (Control c in panelGorny.Controls)
            {
                c.BackColor = darkTheme ? Color.FromArgb(80, 80, 100) : Color.White;
                c.ForeColor = foreColor;
            }

            foreach (Control c in panelBoczny.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = backSide;
                    btn.ForeColor = foreColor;
                }
            }

            // Panel kart
            UpdateCardsColors(pnlPublikacje, backCard, btnCardColor, foreColor, hoverCard);
            UpdateCardsColors(pnlProjekty, backCard, btnCardColor, foreColor, hoverCard);
            UpdateCardsColors(pnlKonferencje, backCard, btnCardColor, foreColor, hoverCard);
            UpdateCardsColors(pnlUlubione, backCard, btnCardColor, foreColor, hoverCard);

            foreach (TabPage tab in zakladki.TabPages)
            {
                tab.BackColor = backMain;
                foreach (Control c in tab.Controls)
                    if (c is Label lbl) lbl.ForeColor = foreColor;
            }
        }

        private void UpdateCardsColors(FlowLayoutPanel panel, Color backCard, Color btnCardColor, Color foreColor, Color hoverCard)
        {
            foreach (Control c in panel.Controls)
            {
                if (c is Panel card)
                {
                    card.BackColor = backCard;

                    card.MouseEnter -= Card_MouseEnter;
                    card.MouseLeave -= Card_MouseLeave;
                    card.MouseEnter += Card_MouseEnter;
                    card.MouseLeave += Card_MouseLeave;

                    foreach (Control child in card.Controls)
                    {
                        if (child is Button btn)
                        {
                            btn.BackColor = btnCardColor;
                            btn.ForeColor = foreColor;
                        }
                        else child.ForeColor = foreColor;
                    }

                    void Card_MouseEnter(object s, EventArgs e) => card.BackColor = hoverCard;
                    void Card_MouseLeave(object s, EventArgs e) => card.BackColor = backCard;
                }
            }
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

 private void ZastosujFiltr(string rok, string typ, string wydawnictwo)
 {
     pnlPublikacje.Controls.Clear();
     var lista = publikacjeVM.Publikacje.AsEnumerable();

     if (!string.IsNullOrEmpty(rok) && int.TryParse(rok, out int r)) lista = lista.Where(p => p.Rok_Wydania == r);
     if (!string.IsNullOrEmpty(typ)) lista = lista.Where(p => p.Typ == typ);
     if (!string.IsNullOrEmpty(wydawnictwo)) lista = lista.Where(p => p.Wydawnictwo == wydawnictwo);

     foreach (var pub in lista) pnlPublikacje.Controls.Add(StworzCardPublikacja(pub));
 }

        }
    }
}
