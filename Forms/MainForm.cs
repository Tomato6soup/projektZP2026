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
            
        }
    }
}
