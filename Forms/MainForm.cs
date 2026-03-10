using System;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private readonly PublikacjeViewModel viewModel = new();
    private DataGridView dgvPublikacje;

    public MainForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        dgvPublikacje = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
        Controls.Add(dgvPublikacje);
        Load += MainForm_Load;

        Text = "Publikacje";
        Width = 800;
        Height = 600;
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        viewModel.ZaladujPublikacje();
        dgvPublikacje.DataSource = viewModel.Publikacje;
    }
}
