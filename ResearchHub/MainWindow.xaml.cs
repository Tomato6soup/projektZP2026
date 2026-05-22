using System.Windows;
using System.Windows.Controls;

namespace ResearchHub
{
    public partial class MainWindow : Window
    {
        // Zmieniamy konstruktor, aby przyjmował też userId
        public MainWindow(string role, int userId)
        {
            InitializeComponent();
            // Przekazujemy userId do ViewModelu
            DataContext = new ViewModel.MainViewModel(role, userId);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Możesz zostawić puste
        }
    }
}