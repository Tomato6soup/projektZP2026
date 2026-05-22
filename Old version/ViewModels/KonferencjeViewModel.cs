using System.ComponentModel;
using Microsoft.Data.SqlClient;

public class KonferencjeViewModel : BaseViewModel
{
    public BindingList<Konferencja> Konferencje { get; set; } = new();

    public void ZaladujKonferencje()
    {
        string connStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=BazaPublikacjiUBB;Integrated Security=True;TrustServerCertificate=True;";
        using var conn = new SqlConnection(connStr);
        conn.Open();

        string query = "SELECT ID, Nazwa, Data, Miejsce FROM Konferencja";
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        Konferencje.Clear();
        while (reader.Read())
        {
            Konferencje.Add(new Konferencja
            {
                ID = reader.GetInt32(0),
                Nazwa = reader.GetString(1),
                Data = reader.GetDateTime(2),
                Miejsce = reader.GetString(3)
            });
        }
    }
}
