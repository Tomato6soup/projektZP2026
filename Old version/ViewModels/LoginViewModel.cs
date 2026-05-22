using Microsoft.Data.SqlClient;

public class LoginViewModel : BaseViewModel
{


    private string _login = string.Empty;
    public string Login
    {
        get => _login;
        set { _login = value; OnPropertyChanged(); }
    }

    private string _haslo = string.Empty;
    public string Haslo
    {
        get => _haslo;
        set { _haslo = value; OnPropertyChanged(); }
    }

    public string Rola { get; private set; } = string.Empty;

    public bool Zaloguj()
    {
        string connStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=BazaPublikacjiUBB;Integrated Security=True;TrustServerCertificate=True;";
        using var conn = new SqlConnection(connStr);
        conn.Open();

        string query = "SELECT Rola FROM Uzytkownik WHERE Login = @login AND Haslo = @haslo";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@login", Login);
        cmd.Parameters.AddWithValue("@haslo", Haslo);

        var result = cmd.ExecuteScalar();
        if (result != null)
        {
            Rola = result.ToString();
            return true;
        }
        return false;
    }
}
