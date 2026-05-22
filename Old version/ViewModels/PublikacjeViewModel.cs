using System.ComponentModel;
using BazaPublikacji_app.Models;
using Microsoft.Data.SqlClient;

public class PublikacjeViewModel : BaseViewModel
{
    public BindingList<Publikacja> Publikacje { get; set; } = new();
    public BindingList<Publikacja> UlubionePublikacje { get; set; } = new();

    public void ZaladujPublikacje()
    {
        string connStr = @"Server=DESKTOP-QSNU0EE\SQLEXPRESS;
Database=BazaPublikacjiUBB;
Trusted_Connection=True;
TrustServerCertificate=True;";
        using SqlConnection conn = new(connStr);
        conn.Open();

        string query = "SELECT ID, Tytul, Rok_Wydania, Typ, Wydawnictwo, PlikPDF, Strony FROM Publikacja";
        using SqlCommand cmd = new(query, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        Publikacje.Clear();
        while (reader.Read())
        {
            Publikacje.Add(new Publikacja
            {
                ID = reader.GetInt32(0),
                Tytul = reader.GetString(1),
                Rok_Wydania = reader.GetInt32(2),
                Typ = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Wydawnictwo = reader.IsDBNull(4) ? "" : reader.GetString(4),
                PlikPDF = reader.IsDBNull(5) ? "" : reader.GetString(5),
                Strony = reader.IsDBNull(6) ? "" : reader.GetString(6)
            });
        }
    }
}