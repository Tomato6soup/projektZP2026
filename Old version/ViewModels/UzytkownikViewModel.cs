using BazaPublikacji_app.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

public class UzytkownikViewModel : BaseViewModel
{
    public BindingList<Uzytkownik> Uzytkownicy { get; set; } = new();

    public void ZaladujUzytkownikow()
    {
        string connStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=BazaPublikacjiUBB;Integrated Security=True;TrustServerCertificate=True;";
        using SqlConnection conn = new(connStr);
        conn.Open();

        string query = "SELECT Login, Haslo, Email, Rola FROM Uzytkownik";
        using SqlCommand cmd = new(query, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        Uzytkownicy.Clear();
        while (reader.Read())
        {
            Uzytkownicy.Add(new Uzytkownik
            {
                Login = reader.GetString(0),
                Haslo = reader.GetString(1),
                Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Rola = reader.IsDBNull(3) ? "" : reader.GetString(3)
            });
        }
    }

    public bool Zaloguj(string login, string haslo, out string rola)
    {
        rola = "";
        string connStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=BazaPublikacjiUBB;Integrated Security=True;TrustServerCertificate=True;";
        using var conn = new SqlConnection(connStr);
        conn.Open();

        string query = "SELECT Rola FROM Uzytkownik WHERE Login=@login AND Haslo=@haslo";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@login", login);
        cmd.Parameters.AddWithValue("@haslo", haslo);

        var result = cmd.ExecuteScalar();
        if (result != null)
        {
            rola = result.ToString();
            return true;
        }
        return false;
    }
}
