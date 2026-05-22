using System;
using System.Drawing;
using System.Windows.Forms;
using BazaPublikacji_app.Models;

namespace BazaPublikacji_app
{
    public partial class MainForm
    {
        // ----------------- KARTY -----------------
        private Panel StworzCardPublikacja(Publikacja pub)
        {
            var card = new Panel
            {
                Width = 220,
                Height = 300,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            PictureBox pic = new PictureBox
            {
                Image = Image.FromFile("C:\\Users\\Поліна\\Documents\\BazaPublikacji_app\\BazaPublikacji_app\\App_Data\\Book_Cover.jfif"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(10, 10),
                Size = new Size(200, 150)
            };
            card.Controls.Add(pic);

            var lblTytul = new Label
            {
                Text = pub.Tytul,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 170),
                Width = 200
            };
            card.Controls.Add(lblTytul);

            var lblRok = new Label
            {
                Text = $"Rok: {pub.Rok_Wydania}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 200)
            };
            card.Controls.Add(lblRok);

            // --- Przycisk szczegółów ---
            var btnSzczegoly = new Button { Text = "Szczegóły", Location = new Point(10, 230), Width = 200 };
            ToolTip tt = new ToolTip();
            tt.SetToolTip(btnSzczegoly, "Kliknij, aby zobaczyć szczegóły publikacji");
            btnSzczegoly.Click += (s, e) =>
            {
                txtDetails.Text =
                    $"Tytuł: {pub.Tytul}\r\n" +
                    $"Rok wydania: {pub.Rok_Wydania}\r\n" +
                    $"Typ: {pub.Typ}\r\n" +
                    $"Wydawnictwo: {pub.Wydawnictwo}\r\n" +
                    $"Plik PDF: {pub.PlikPDF}\r\n" +
                    $"Strony: {pub.Strony}";

                pnlDetails.Visible = true;
            };
            card.Controls.Add(btnSzczegoly);

            // --- Przycisk ulubione ---
            var btnUlubione = new Button
            {
                Text = publikacjeVM.UlubionePublikacje.Contains(pub) ? "❤" : "♡",
                Location = new Point(170, 260),
                Width = 40,
                Height = 30
            };
            tt.SetToolTip(btnUlubione, "Dodaj lub usuń z ulubionych");
            btnUlubione.Click += (s, e) =>
            {
                if (publikacjeVM.UlubionePublikacje.Contains(pub))
                    publikacjeVM.UlubionePublikacje.Remove(pub);
                else
                    publikacjeVM.UlubionePublikacje.Add(pub);

                btnUlubione.Text = publikacjeVM.UlubionePublikacje.Contains(pub) ? "❤" : "♡";
                WyswietlUlubione();
            };
            card.Controls.Add(btnUlubione);

            return card;
        }

        private Panel StworzCardProjekt(Projekt proj)
        {
            var card = new Panel
            {
                Width = 220,
                Height = 180,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblTytul = new Label
            {
                Text = proj.Tytul,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                Width = 200
            };
            card.Controls.Add(lblTytul);

            var btnSzczegoly = new Button { Text = "Szczegóły", Location = new Point(10, 120), Width = 200 };
            btnSzczegoly.Click += (s, e) =>
            {
                txtDetails.Text =
                    $"Tytuł: {proj.Tytul}\r\n" +
                    $"Opis: {proj.Opis}\r\n" +
                    $"Data rozpoczęcia: {proj.DataRozpoczecia.ToShortDateString()}\r\n" +
                    $"Data zakończenia: {proj.DataZakonczenia.ToShortDateString()}";

                pnlDetails.Visible = true;
            };
            card.Controls.Add(btnSzczegoly);

            return card;
        }

        private Panel StworzCardKonferencja(Konferencja k)
        {
            var card = new Panel
            {
                Width = 220,
                Height = 150,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblNazwa = new Label
            {
                Text = k.Nazwa,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                Width = 200
            };
            card.Controls.Add(lblNazwa);

            var btnSzczegoly = new Button { Text = "Szczegóły", Location = new Point(10, 90), Width = 200 };
            btnSzczegoly.Click += (s, e) =>
            {
                txtDetails.Text =
                    $"Nazwa: {k.Nazwa}\r\n" +
                    $"Data: {k.Data.ToShortDateString()}\r\n" +
                    $"Miejsce: {k.Miejsce}";

                pnlDetails.Visible = true;
            };
            card.Controls.Add(btnSzczegoly);

            return card;
        }

    }
}