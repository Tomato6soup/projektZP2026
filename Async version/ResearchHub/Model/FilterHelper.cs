using System;
using System.Collections.Generic;
using System.Text;

namespace ResearchHub.Model
{
    public static class FilterHelper
    {
        public static bool FilterPublications(object obj, string searchText, string typeFilter)
        {
            if (obj is Publikacja pub)
            {
                bool matchesSearch = string.IsNullOrWhiteSpace(searchText) ||
                    (pub.Tytul != null && pub.Tytul.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (pub.Wydawnictwo != null && pub.Wydawnictwo.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                bool matchesType = string.IsNullOrWhiteSpace(typeFilter) ||
                                   typeFilter.Equals("Wszystkie", StringComparison.OrdinalIgnoreCase) ||
                                   typeFilter.Equals("Wszystkie typy", StringComparison.OrdinalIgnoreCase) ||
                                   (pub.Typ != null && pub.Typ.Equals(typeFilter, StringComparison.OrdinalIgnoreCase));

                return matchesSearch && matchesType;
            }
            return false;
        }

        public static bool FilterProjects(object obj, string searchText)
        {
            if (obj is Projekt projekt)
            {
                return string.IsNullOrWhiteSpace(searchText) ||
                       (projekt.Tytul != null && projekt.Tytul.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        public static bool FilterConferences(object obj, string searchText)
        {
            if (obj is Konferencja konf)
            {
                return string.IsNullOrWhiteSpace(searchText) ||
                       (konf.Nazwa != null && konf.Nazwa.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
    }
}
