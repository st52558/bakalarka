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

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadGameClick(object sender, RoutedEventArgs e)
        {
            LoadGame win2 = new LoadGame();
            win2.MainWindow = this;
            win2.Show();
        }

        private void Quit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Manual win2 = new Manual();
            win2.Show();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame win2 = new NewGame();
            win2.Mainwindow = this;
            win2.Show();
        }
    }
}
