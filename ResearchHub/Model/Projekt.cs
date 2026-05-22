// Plik: Model/Projekt.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResearchHub.Model
{
    public class Projekt: INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Tytul { get; set; }
        public string Opis { get; set; }
        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }
        // W pliku Publikacja.cs (oraz analogicznie w Projekt i Konferencja)
        public string NazwaPlikuZdjecia { get; set; } = string.Empty;
        // 3. To zdarzenie jest niezbędne, aby interfejs działał
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _czyUlubione;
        public bool CzyUlubione { get => _czyUlubione; set { _czyUlubione = value; OnPropertyChanged(); } }

        // 4. Metoda pomocnicza (dobra praktyka, znacznie upraszcza kod)
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool _czyDolaczono;
        public bool CzyDolaczono
        {
            get => _czyDolaczono;
            set
            {
                _czyDolaczono = value;
                OnPropertyChanged(); // Teraz to zadziała!
            }
        }
        public string SciezkaZdjecia
        {
            get
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string folderPath = System.IO.Path.Combine(basePath, "Zdjecia");

                if (string.IsNullOrWhiteSpace(NazwaPlikuZdjecia))
                    return System.IO.Path.Combine(folderPath, "Projekt.png");

                string fullPath = System.IO.Path.Combine(folderPath, NazwaPlikuZdjecia);

                if (System.IO.File.Exists(fullPath))
                    return fullPath;
                else
                    return System.IO.Path.Combine(folderPath, "Projekt.png");
            }
        }

        public Projekt(int id, string tytul, string opis, DateTime dataRozpoczecia, DateTime dataZakonczenia)
        {
            ID = id;
            Tytul = tytul;
            Opis = opis;
            DataRozpoczecia = dataRozpoczecia;
            DataZakonczenia = dataZakonczenia;
        }
    }
}
