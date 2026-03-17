using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaPublikacji_app.Models
{
    public class Uzytkownik
    {
        public int ID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Haslo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rola { get; set; } = string.Empty; //  "Administrator" lub "Student"
    }

}
