using System;
using System.Collections.Generic;
using System.Text;

namespace ResearchHub.ViewModel
{
    public static class SessionManager
    {
        public static int CurrentUserId { get; set; } = 1;

        // Dodajemy przechowywanie roli (domyślnie ustawione do testów)
        public static string CurrentRole { get; set; } = "Student";
    }
}
