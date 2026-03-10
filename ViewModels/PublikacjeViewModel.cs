using System.ComponentModel;
using BazaPublikacji_app.Models;
using Microsoft.Data.SqlClient;

public class PublikacjeViewModel : BaseViewModel
{
    public BindingList<Publikacja> Publikacje { get; set; } = new();

    public void ZaladujPublikacje()
    {
        string connStr = "Data Source=.\\SQLEXPRESS;Initial Catalog=TwojaBazaDanych;Integrated Security=True";
        using SqlConnection conn = new(connStr);
        conn.Open();

        string query = "SELECT ID, Tytul, Rok_Wydania FROM Publikacja";
        using SqlCommand cmd = new(query, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        Publikacje.Clear();
        while (reader.Read())
        {
            Publikacje.Add(new Publikacja
            {
                ID = reader.GetInt32(0),
                Tytul = reader.GetString(1),
                RokWydania = reader.GetInt32(2)
            });
        }
    }
}