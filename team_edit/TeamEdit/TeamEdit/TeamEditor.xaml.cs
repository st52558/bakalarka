using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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

namespace TeamEdit
{
    /// <summary>
    /// Interaction logic for TeamEditor.xaml
    /// </summary>
    public partial class TeamEditor : Window
    {
        public TeamEditor()
        {
            InitializeComponent();
        }

        public TeamEditor(string chosenTeam)
        {
            InitializeComponent();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                BitmapImage imageSource = new BitmapImage();
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select logo from tym_basic where nazev ='" + chosenTeam + "'", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    
                    byte[] data = (byte[])reader[0];

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                    }
                }
                TeamLogo.Source = imageSource;
            }
        }
    }
}
