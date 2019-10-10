using System;
using System.Windows;

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

        private void ShowRegisteredTournaments(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(DatabaseName);
        }

        private void ShowSponsors(object sender, RoutedEventArgs e)
        {
            ViewSponsors win2 = new ViewSponsors(1, DatabaseName);
            win2.Show();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowManual(object sender, RoutedEventArgs e)
        {
            Manual win2 = new Manual();
            win2.Show();
        }

        private void ShowFreeEmployees(object sender, RoutedEventArgs e)
        {

        }

        private void ShowPowerRanking(object sender, RoutedEventArgs e)
        {

        }

        private void ShowTeams(object sender, RoutedEventArgs e)
        {

        }
    }
}
