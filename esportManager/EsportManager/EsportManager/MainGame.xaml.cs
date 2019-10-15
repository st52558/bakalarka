using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {
        class TournamentsDataGrid
        {
            public string Turnaj { get; set; }
            public string Od { get; set; }
            public string Do { get; set; }
            public string Místo { get; set; }
        }

        class PlayersDataGrid
        {
            public string Nick { get; set; }
            public string Jmeno { get; set; }
            public string Prijmeni { get; set; }
            public string Pozice { get; set; }
        }
        public string DatabaseName { get; set; }
        List<TeamSectionBasic> sectionsList = new List<TeamSectionBasic>();
        public MainGame()
        {
            InitializeComponent();
            AddSectionsToList();
            SetTabs();
            AddAllTournaments();
            AddAllPlayers();
            ChangePropertiesOfNextActionButton();
        }

        private void ChangePropertiesOfNextActionButton()
        {
            /* hledání, co se vlastně bude dít.. možnosti:
               1 - nic se neděje - a - další den
                                 - b - konec roku
               2 - je na řadě zápas, takže button posune na okno se zápasem
               3 - končí sponzorská smlouva, vyjede okno, jestli chceme přesunout na smlouvy
               4 - končí hráčovi smlouva, vyjede okno, jestli chceme přesunout na přehled hráčů
               5 - končí zaměstnanci smlouva, vyjede okno, jestli chceme přesunout na 
            */
        }

        private void AddAllPlayers()
        {
            List<PlayersDataGrid> players = new List<PlayersDataGrid>();
            players.Add(new PlayersDataGrid() { Nick = "Gunny", Jmeno = "František", Prijmeni = "Soldán", Pozice = "Jungle" });
            players.Add(new PlayersDataGrid() { Nick = "BlackStar", Jmeno = "Mykola", Prijmeni = "Klebus", Pozice = "Support" });
            Section1PlayersList.ItemsSource = players;
            Section2PlayersList.ItemsSource = players;
            Section3PlayersList.ItemsSource = players;
            Section4PlayersList.ItemsSource = players;
            Section5PlayersList.ItemsSource = players;
            Section6PlayersList.ItemsSource = players;
        }

        private void AddAllTournaments()
        {
            List<TournamentsDataGrid> tournaments = new List<TournamentsDataGrid>();
            tournaments.Add(new TournamentsDataGrid() { Turnaj = "LOLEC turnaj", Od = "1. 1. 2011", Do = "31. 1. 2011", Místo = "Praha" });
            tournaments.Add(new TournamentsDataGrid() { Turnaj = "LOLEC turnaj 2", Od = "1. 2. 2011", Do = "31. 2. 2011", Místo = "Brno" });
            Section1TournamentsList.ItemsSource = tournaments;
            Section2TournamentsList.ItemsSource = tournaments;
            Section3TournamentsList.ItemsSource = tournaments;
            Section4TournamentsList.ItemsSource = tournaments;
            Section5TournamentsList.ItemsSource = tournaments;
            Section6TournamentsList.ItemsSource = tournaments;
        }

        private void AddSectionsToList()
        {
            // už tady se bude pomocí dotazu zjišťovat jestli jde o B tým (následně se tam to B za to přidá)
            sectionsList.Add(new TeamSectionBasic(1, 2, "League of Legends"));
            sectionsList.Add(new TeamSectionBasic(3, 1, "Counter Strike"));
            sectionsList.Add(new TeamSectionBasic(6, 3, "League of Legends"));
            sectionsList.Add(new TeamSectionBasic(1, 2, "League of Legends"));
            //sectionsList.Add(new TeamSectionBasic(3, 1, "Counter Strike"));
        }

        private void SetTabs()
        {
            int numberOfSections = GetNumberOfSections() + 1;
            while (numberOfSections < SectionTabs.Items.Count)
            {
                SectionTabs.Items.RemoveAt(numberOfSections);
            }
            switch (numberOfSections)
            {
                case 2:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabAsNewSection(Section2, Section2Body);
                    break;
                case 3:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabAsNewSection(Section3, Section3Body);
                    break;
                case 4:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabAsNewSection(Section4, Section4Body);
                    break;
                case 5:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3));
                    SetTabAsNewSection(Section5, Section5Body);
                    break;
                case 6:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3));
                    SetTabsDesign(Section5, Section5Body, sectionsList.ElementAt(4));
                    SetTabAsNewSection(Section6, Section6Body);
                    break;
                case 7:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3));
                    SetTabsDesign(Section5, Section5Body, sectionsList.ElementAt(4));
                    SetTabsDesign(Section6, Section6Body, sectionsList.ElementAt(5));
                    break;
                default:
                    break;
            }

        }

        private void SetTabAsNewSection(TabItem tab, Grid body)
        {
            
            tab.Header = "+";
            body.Children.Clear();
            Button addSectionButton = new Button();
            addSectionButton.Name = "addSection";
            addSectionButton.Click += this.AddNewSection;
            body.Children.Add(addSectionButton);
        }

        private void AddNewSection(object sender, RoutedEventArgs e)
        {
            // vyjede form s přidáním sekce
        }

        private void SetTabsDesign(TabItem tab, Grid body, TeamSectionBasic teamSectionBasic)
        {
            Brush brush = Brushes.Black;
            if (teamSectionBasic.sectionID % 4 == 0) { brush = Brushes.Brown;}
            if (teamSectionBasic.sectionID % 4 == 1) { brush = Brushes.Cyan; }
            if (teamSectionBasic.sectionID % 4 == 2) { brush = Brushes.Salmon; }
            if (teamSectionBasic.sectionID % 4 == 3) { brush = Brushes.Snow; }
            tab.Background = brush;
            body.Background = brush;
            tab.Header = teamSectionBasic.sectionName;
        }

        private void GoToNextDay(object sender, RoutedEventArgs e)
        {
            
        }

        private int GetNumberOfSections()
        {
            return sectionsList.Count;
        }

        private void ShowRegisteredTournaments(object sender, RoutedEventArgs e)
        {
            TournamentsParticipating win2 = new TournamentsParticipating();
            win2.Show();
        }

        private void ShowSponsors(object sender, RoutedEventArgs e)
        {
            ViewSponsors win2 = new ViewSponsors(1, DatabaseName);
            win2.Show();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
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
            TeamSearch win2 = new TeamSearch();
            win2.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
