using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {
        public string DatabaseName { get; set; }
        public MainGame()
        {
            InitializeComponent();
            
        }

        private void GoToNextDay(object sender, RoutedEventArgs e)
        {
           
        }

        private void RegisteredTournamentClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(DatabaseName);
        }
    }
}
