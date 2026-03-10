using System;
using System.Windows.Forms;

public partial class LoginForm : Form
{
    private readonly LoginViewModel viewModel = new();
    private TextBox txtLogin;
    private TextBox txtHaslo;
    private Button btnLogin;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        txtLogin = new TextBox { Left = 50, Top = 20, Width = 200, PlaceholderText = "Login" };
        txtHaslo = new TextBox { Left = 50, Top = 60, Width = 200, UseSystemPasswordChar = true, PlaceholderText = "Hasło" };
        btnLogin = new Button { Left = 100, Top = 100, Width = 100, Text = "Zaloguj" };
        btnLogin.Click += btnLogin_Click;

        Controls.Add(txtLogin);
        Controls.Add(txtHaslo);
        Controls.Add(btnLogin);

        Text = "Logowanie";
        Width = 320;
        Height = 200;
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
        viewModel.Login = txtLogin.Text;
        viewModel.Haslo = txtHaslo.Text;

        if (viewModel.Zaloguj())
        {
            Hide();
            var mainForm = new MainForm();
            mainForm.Show();
        }
        else
        {
            MessageBox.Show("Błędny login lub hasło.");
        }
    }
}