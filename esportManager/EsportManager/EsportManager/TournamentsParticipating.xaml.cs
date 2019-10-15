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
    /// Interakční logika pro TournamentsParticipating.xaml
    /// </summary>
    public partial class TournamentsParticipating : Window
    {
        public TournamentsParticipating()
        {
            InitializeComponent();
            Section1Tournaments.Items.Add("asdf");
            Section2Tournaments.Items.Add("asdf");
            Section3Tournaments.Items.Add("asdf");
            Section4Tournaments.Items.Add("asdf");
            Section5Tournaments.Items.Add("asdf");
            Section6Tournaments.Items.Add("asdf");
        }

        private void CancelTournament_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
