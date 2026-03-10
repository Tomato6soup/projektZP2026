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

    public bool Zaloguj()
    {
        string connStr = "Data Source=.\\SQLEXPRESS;Initial Catalog=TwojaBazaDanych;Integrated Security=True";
        using SqlConnection conn = new(connStr);
        conn.Open();

        string query = "SELECT COUNT(*) FROM Uzytkownik WHERE Login = @login AND Haslo = @haslo";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@login", Login);
        cmd.Parameters.AddWithValue("@haslo", Haslo);

        int count = (int)cmd.ExecuteScalar();
        return count > 0;
    }
}