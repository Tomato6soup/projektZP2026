public class Publikacja
{
    public int ID { get; set; }
    public string Tytul { get; set; } = string.Empty;
    public int Rok_Wydania { get; set; }
    public string Typ { get; set; } = string.Empty;
    public string Wydawnictwo { get; set; } = string.Empty;
    public string PlikPDF { get; set; } = string.Empty;
    public string Strony { get; set; } = string.Empty;
    public int PracownikID { get; set; }
    public int ProjektID { get; set; }
    public int StronaID { get; set; }
}
