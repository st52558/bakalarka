using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro Training.xaml
    /// </summary>
    public partial class Training : Window
    {
        List<string> trainingTypes = new List<string>();
        
        public Training()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            trainingTypes.Add("Volno");
            trainingTypes.Add("Individuální");
            trainingTypes.Add("Analýza");
            trainingTypes.Add("Cvičný zápas");
            SetAllComboboxes();

        }

        private void SetAllComboboxes()
        {
            for (int i = 0; i < trainingTypes.Count; i++)
            {
                MondayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                MondayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                MondayEveningCB.Items.Add(trainingTypes.ElementAt(i));
                TuesdayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                TuesdayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                TuesdayEveningCB.Items.Add(trainingTypes.ElementAt(i));
                WednesdayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                WednesdayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                WednesdayEveningCB.Items.Add(trainingTypes.ElementAt(i));
                ThursdayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                ThursdayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                ThursdayEveningCB.Items.Add(trainingTypes.ElementAt(i));
                FridayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                FridayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                FridayEveningCB.Items.Add(trainingTypes.ElementAt(i));
                SaturdayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                SaturdayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                SaturdayEveningCB.Items.Add(trainingTypes.ElementAt(i));
                SundayMorningCB.Items.Add(trainingTypes.ElementAt(i));
                SundayAfternoonCB.Items.Add(trainingTypes.ElementAt(i));
                SundayEveningCB.Items.Add(trainingTypes.ElementAt(i));
            }
            
        }

        private void SetBackgrounds(ComboBox combo, Grid grid)
        {
            switch (combo.SelectedIndex)
            {
                case 0:
                    grid.Background = new SolidColorBrush(Color.FromRgb(200, 30, 30));
                    break;
                case 1:
                    grid.Background = new SolidColorBrush(Color.FromRgb(200, 200, 30));
                    break;
                case 2:
                    grid.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                    break;
                case 3:
                    grid.Background = new SolidColorBrush(Color.FromRgb(30, 30, 200));
                    break;
                default:
                    grid.Background = new SolidColorBrush(Color.FromRgb(30, 30, 200));
                    break;
            }
            
        }

        private void SaveTraining(object sender, RoutedEventArgs e)
        {

        }

        private void TrainingChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MondayMorningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a,b);
        }

        

        private void MondayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void MondayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void SundayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void SundayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void SundayMorningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void SaturdayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void SaturdayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void SaturdayMorningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void FridayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void FridayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void FridayMorningCBTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void ThursdayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void ThursdayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void ThursdayMorningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void WednesdayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void WednesdayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void WednesdayMorningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void TuesdayEveningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void TuesdayAfternoonTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }

        private void TuesdayMorningTrainingChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a = (ComboBox)e.Source;
            Grid b = (Grid)a.Parent;
            SetBackgrounds(a, b);
        }
    }
}
