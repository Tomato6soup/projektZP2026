// Plik: Model/Konferencja.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ResearchHub.Model
{
    public class Konferencja: INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public DateTime Data { get; set; }
        public string Miejsce { get; set; }
        // W pliku Publikacja.cs (oraz analogicznie w Projekt i Konferencja)
        public string NazwaPlikuZdjecia { get; set; } = string.Empty;
        private bool _czyUlubione;
        public bool CzyUlubione { get => _czyUlubione; set { _czyUlubione = value; OnPropertyChanged(); } }
        // 3. To zdarzenie jest niezbędne, aby interfejs działał
        public event PropertyChangedEventHandler PropertyChanged;

        // 4. Metoda pomocnicza (dobra praktyka, znacznie upraszcza kod)
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public string SciezkaZdjecia
        {
            get
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string folderPath = System.IO.Path.Combine(basePath, "Zdjecia");

                if (string.IsNullOrWhiteSpace(NazwaPlikuZdjecia))
                    return System.IO.Path.Combine(folderPath, "Konferencja.png");

                string fullPath = System.IO.Path.Combine(folderPath, NazwaPlikuZdjecia);

                if (System.IO.File.Exists(fullPath))
                    return fullPath;
                else
                    return System.IO.Path.Combine(folderPath, "Konferencja.png");
            }
        }
        public Konferencja(int id, string nazwa, DateTime data, string miejsce)
        {
            ID = id;
            Nazwa = nazwa;
            Data = data;
            Miejsce = miejsce;
        }
    }
}
