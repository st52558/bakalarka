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
        List<Tournament> tournamentsForSection1 = new List<Tournament>();
        List<Tournament> tournamentsForSection2 = new List<Tournament>();
        List<Tournament> tournamentsForSection3 = new List<Tournament>();
        List<Tournament> tournamentsForSection4 = new List<Tournament>();
        List<Tournament> tournamentsForSection5 = new List<Tournament>();
        List<Tournament> tournamentsForSection6 = new List<Tournament>();
        public TournamentsParticipating()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            SetAllLists();
        }

        private void SetAllLists()
        {
            tournamentsForSection1.Add(new Tournament(1, "WRLDS", "1.1.2011", "31.1.2011", 2000));
            tournamentsForSection2.Add(new Tournament(1, "WRLDS", "1.1.2011", "31.1.2011", 2000));
            tournamentsForSection3.Add(new Tournament(1, "WRLDS", "1.1.2011", "31.1.2011", 2000));
            tournamentsForSection4.Add(new Tournament(1, "WRLDS", "1.1.2011", "31.1.2011", 2000));
            tournamentsForSection5.Add(new Tournament(1, "WRLDS", "1.1.2011", "31.1.2011", 2000));
            tournamentsForSection6.Add(new Tournament(1, "WRLDS", "1.1.2011", "31.1.2011", 2000));
            AddAllListsToTournamentListBoxes();
        }

        private void AddAllListsToTournamentListBoxes()
        {
            Section1Tournaments.Items.Clear();
            Section2Tournaments.Items.Clear();
            Section3Tournaments.Items.Clear();
            Section4Tournaments.Items.Clear();
            Section5Tournaments.Items.Clear();
            Section6Tournaments.Items.Clear();
            for (int i = 0; i < tournamentsForSection1.Count; i++)
            {
                Section1Tournaments.Items.Add(tournamentsForSection1.ElementAt(i).Name + ", " + tournamentsForSection1.ElementAt(i).DateFrom + " - " + tournamentsForSection1.ElementAt(i).DateTo);
            }
            for (int i = 0; i < tournamentsForSection2.Count; i++)
            {
                Section2Tournaments.Items.Add(tournamentsForSection2.ElementAt(i).Shortcut + ", " + tournamentsForSection2.ElementAt(i).DateFrom + " - " + tournamentsForSection2.ElementAt(i).DateTo);
            }
            for (int i = 0; i < tournamentsForSection3.Count; i++)
            {
                Section3Tournaments.Items.Add(tournamentsForSection3.ElementAt(i).Shortcut + ", " + tournamentsForSection3.ElementAt(i).DateFrom + " - " + tournamentsForSection3.ElementAt(i).DateTo);
            }
            for (int i = 0; i < tournamentsForSection4.Count; i++)
            {
                Section4Tournaments.Items.Add(tournamentsForSection4.ElementAt(i).Shortcut + ", " + tournamentsForSection4.ElementAt(i).DateFrom + " - " + tournamentsForSection4.ElementAt(i).DateTo);
            }
            for (int i = 0; i < tournamentsForSection5.Count; i++)
            {
                Section5Tournaments.Items.Add(tournamentsForSection5.ElementAt(i).Shortcut + ", " + tournamentsForSection5.ElementAt(i).DateFrom + " - " + tournamentsForSection5.ElementAt(i).DateTo);
            }
            for (int i = 0; i < tournamentsForSection6.Count; i++)
            {
                Section6Tournaments.Items.Add(tournamentsForSection6.ElementAt(i).Shortcut + ", " + tournamentsForSection6.ElementAt(i).DateFrom + " - " + tournamentsForSection6.ElementAt(i).DateTo);
            }
        }

        private void CancelChosenTournament(object sender, RoutedEventArgs e)
        {
            if (Section1Tournaments.SelectedIndex == -1 && Section2Tournaments.SelectedIndex == -1 && Section3Tournaments.SelectedIndex == -1 && Section4Tournaments.SelectedIndex == -1 && Section5Tournaments.SelectedIndex == -1 && Section6Tournaments.SelectedIndex == -1)
            {
                return;
            }
            int cancelFee = 1000;
            MessageBoxResult result = MessageBox.Show("Vážně se chcete odhlásit z turnaje? Bude Vás to stát " + cancelFee + "€.", "Chystáte se odhlásit z turnaje", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            if (Section1Tournaments.SelectedIndex != -1)
            {
                tournamentsForSection1.RemoveAt(Section1Tournaments.SelectedIndex);
            }
            if (Section2Tournaments.SelectedIndex != -1)
            {
                tournamentsForSection2.RemoveAt(Section2Tournaments.SelectedIndex);
            }
            if (Section3Tournaments.SelectedIndex != -1)
            {
                tournamentsForSection3.RemoveAt(Section3Tournaments.SelectedIndex);
            }
            if (Section4Tournaments.SelectedIndex != -1)
            {
                tournamentsForSection4.RemoveAt(Section4Tournaments.SelectedIndex);
            }
            if (Section5Tournaments.SelectedIndex != -1)
            {
                tournamentsForSection5.RemoveAt(Section5Tournaments.SelectedIndex);
            }
            if (Section6Tournaments.SelectedIndex != -1)
            {
                tournamentsForSection6.RemoveAt(Section6Tournaments.SelectedIndex);
            }
            AddAllListsToTournamentListBoxes();
        }
    }
}
