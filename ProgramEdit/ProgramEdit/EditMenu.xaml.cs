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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProgramEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EditMenu : Window
    {
        string database;
        public EditMenu(string databaseName)
        {
            database = databaseName;
            InitializeComponent();
        }

        private void Coach_Click(object sender, RoutedEventArgs e)
        {
            CoachEdit win2 = new CoachEdit(database);
            win2.ShowDialog();
        }

        private void Sponsor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Team_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Section_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Player_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Tournament_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
