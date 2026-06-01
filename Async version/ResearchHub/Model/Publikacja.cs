using Microsoft.Data.SqlClient;
using ResearchHub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace ResearchHub.Model
{
    public class Publikacja : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Tytul { get; set; }
        public int RokWydania { get; set; }
        public string Typ { get; set; }
        public string Wydawnictwo { get; set; }
        public string PlikPdf { get; set; }
        public int Strony { get; set; }
        // W pliku Publikacja.cs (oraz analogicznie w Projekt i Konferencja)
        public string NazwaPlikuZdjecia { get; set; } = string.Empty;
        public event PropertyChangedEventHandler PropertyChanged;
        // 4. Metoda pomocnicza (dobra praktyka, znacznie upraszcza kod)
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public bool IsFavorite { get; set; } // Pole pomocnicze dla UI
        private bool _czyUlubione;
        public bool CzyUlubione
        {
            get => _czyUlubione;
            set
            {
                _czyUlubione = value;
                OnPropertyChanged(); // Twoja metoda powiadamiająca widok
            }
        }

        public Publikacja(int id, string tytul, int rokWydania, string typ, string wydawnictwo, string plikPdf, int strony)
        {
            ID = id;
            Tytul = tytul;
            RokWydania = rokWydania;
            Typ = typ;
            Wydawnictwo = wydawnictwo;
            PlikPdf = plikPdf;
            Strony = strony;
            IsFavorite = false;
        }
        public string SciezkaZdjecia
        {
            get
            {
                // Pobieramy ścieżkę, w której aktualnie uruchomiona jest aplikacja
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string folderPath = System.IO.Path.Combine(basePath, "Zdjecia");

                // Jeśli w bazie nie ma nazwy pliku, zwracamy zdjęcie domyślne
                if (string.IsNullOrWhiteSpace(NazwaPlikuZdjecia))
                {
                    return System.IO.Path.Combine(folderPath, "Book_cover.png");
                }

                string fullPath = System.IO.Path.Combine(folderPath, NazwaPlikuZdjecia);

                // Zabezpieczenie: sprawdzamy, czy plik faktycznie istnieje na dysku
                if (System.IO.File.Exists(fullPath))
                {
                    return fullPath;
                }
                else
                {
                    return System.IO.Path.Combine(folderPath, "Book_cover.png");
                }
            }
        }
    }
}



