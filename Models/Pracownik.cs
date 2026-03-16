using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaPublikacji_app.Models
{
    public class Pracownik
    {
        public int ID { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;
        public string Stanowisko { get; set; } = string.Empty;
        public string Wydzial { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
