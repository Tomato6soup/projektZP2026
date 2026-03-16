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
        }
    }
}