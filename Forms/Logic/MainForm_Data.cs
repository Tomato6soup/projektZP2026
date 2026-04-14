using System;
using System.Linq;
using System.Windows.Forms;

namespace BazaPublikacji_app
{
    public partial class MainForm
    {
        // ----------------- ŁADOWANIE DANYCH -----------------
        private void ZaladujDane()
        {
            publikacjeVM.ZaladujPublikacje();
            projektyVM.ZaladujProjekty();
            konferencjeVM.ZaladujKonferencje();
        }

        // ----------------- WYŚWIETLANIE -----------------
        private void WyswietlWszystkieZakladki()
        {
            WyswietlPublikacje();
            WyswietlProjekty();
            WyswietlKonferencje();
            WyswietlUlubione();
        }

       

        private void WyswietlPublikacje()
        {
            pnlPublikacje.Controls.Clear();
            foreach (var pub in publikacjeVM.Publikacje)
                pnlPublikacje.Controls.Add(StworzCardPublikacja(pub));
        }

        private void WyswietlProjekty()
        {
            pnlProjekty.Controls.Clear();
            foreach (var proj in projektyVM.Projekty)
                pnlProjekty.Controls.Add(StworzCardProjekt(proj));
        }

        private void WyswietlKonferencje()
        {
            pnlKonferencje.Controls.Clear();
            foreach (var konf in konferencjeVM.Konferencje)
                pnlKonferencje.Controls.Add(StworzCardKonferencja(konf));
        }

        private void WyswietlUlubione()
        {
            pnlUlubione.Controls.Clear();
            foreach (var pub in publikacjeVM.UlubionePublikacje)
                pnlUlubione.Controls.Add(StworzCardPublikacja(pub));
        }

        private void ZastosujFiltr(string rok, string typ, string wydawnictwo)
        {
            pnlPublikacje.Controls.Clear();
            var lista = publikacjeVM.Publikacje.AsEnumerable();

            if (!string.IsNullOrEmpty(rok) && int.TryParse(rok, out int r)) lista = lista.Where(p => p.Rok_Wydania == r);
            if (!string.IsNullOrEmpty(typ)) lista = lista.Where(p => p.Typ == typ);
            if (!string.IsNullOrEmpty(wydawnictwo)) lista = lista.Where(p => p.Wydawnictwo == wydawnictwo);

            foreach (var pub in lista) pnlPublikacje.Controls.Add(StworzCardPublikacja(pub));
        }


    }
}