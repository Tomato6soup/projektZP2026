using System.ComponentModel;
using Microsoft.Data.SqlClient;

public class ProjektyViewModel : BaseViewModel
{
    public BindingList<Projekt> Projekty { get; set; } = new();

    public void ZaladujProjekty()
    {
        string connStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=BazaPublikacjiUBB;Integrated Security=True;TrustServerCertificate=True;";
        using var conn = new SqlConnection(connStr);
        conn.Open();

        string query = "SELECT ID, Tytul, Opis, DataRozpoczecia, DataZakonczenia FROM Projekt";
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        Projekty.Clear();
        while (reader.Read())
        {
            Projekty.Add(new Projekt
            {
                ID = reader.GetInt32(0),
                Tytul = reader.GetString(1),
                Opis = reader.GetString(2),
                DataRozpoczecia = reader.GetDateTime(3),
                DataZakonczenia = reader.GetDateTime(4),

            });
        }
    }
}
