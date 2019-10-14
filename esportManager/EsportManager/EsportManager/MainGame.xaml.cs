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
        public string DatabaseName { get; set; }
        List<TeamSectionBasic> sectionsList = new List<TeamSectionBasic>();
        public MainGame()
        {
            InitializeComponent();
            
            AddSectionsToList();
            SetTabs();
        }

        private void AddSectionsToList()
        {
            // už tady se bude pomocí dotazu zjišťovat jestli jde o B tým (následně se tam to B za to přidá)
            sectionsList.Add(new TeamSectionBasic(1,2, "League of Legends"));
            sectionsList.Add(new TeamSectionBasic(3,1, "Counter Strike"));
            sectionsList.Add(new TeamSectionBasic(6,3, "League of Legends"));
        }

        private void SetTabs()
        {
            int numberOfSections = GetNumberOfSections();
            while (numberOfSections < SectionTabs.Items.Count)
            {
                SectionTabs.Items.RemoveAt(numberOfSections);
            }
            switch (numberOfSections)
            {
                case 1:
                    SetTabsDesign(Section1, sectionsList.ElementAt(0));
                    break;
                case 2:
                    SetTabsDesign(Section1, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, sectionsList.ElementAt(1));
                    break;
                case 3:
                    SetTabsDesign(Section1, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, sectionsList.ElementAt(2));
                    break;
                case 4:
                    SetTabsDesign(Section1, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, sectionsList.ElementAt(3));
                    break;
                case 5:
                    SetTabsDesign(Section1, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, sectionsList.ElementAt(3));
                    SetTabsDesign(Section5, sectionsList.ElementAt(4));
                    break;
                case 6:
                    SetTabsDesign(Section1, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, sectionsList.ElementAt(3));
                    SetTabsDesign(Section5, sectionsList.ElementAt(4));
                    SetTabsDesign(Section6, sectionsList.ElementAt(5));
                    break;
                default:
                    break;
            }

        }

        private void SetTabsDesign(TabItem tab, TeamSectionBasic teamSectionBasic)
        {
            Brush brush = Brushes.Black;
            
            if (teamSectionBasic.sectionID % 4 == 0) { brush = Brushes.Brown; }
            if (teamSectionBasic.sectionID % 4 == 1) { brush = Brushes.Cyan; }
            if (teamSectionBasic.sectionID % 4 == 2) { brush = Brushes.Salmon; }
            if (teamSectionBasic.sectionID % 4 == 3) { brush = Brushes.Snow; }

            tab.Background = brush;
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
