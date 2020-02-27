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
using System.Windows.Forms;
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
        public List<string> sqlStatements = new List<string>();
        List<Nation> nationList = new List<Nation>();
        List<City> cityList = new List<City>();
        List<TeamSection> teamSectionList = new List<TeamSection>();
        int selectedTeamId;
        string filePath;

        public TeamEditor()
        {
            InitializeComponent();
            AddCountriesToCB();
            ButtonsChanging();
        }

        private void ButtonsChanging()
        {
            int sections = GetMaxSections();
            if (SectionLB.Items.Count >= sections)
            {
                AddSection.IsEnabled = false;
            } else
            {
                AddSection.IsEnabled = true;
            }

            if (SectionLB.SelectedIndex > -1)
            {
                DeleteSection.IsEnabled = true;
            } else
            {
                DeleteSection.IsEnabled = false;
            }

            if (SponsorsLB.Items.Count >= 5)
            {
                AddSponsor.IsEnabled = false;
            }
            else
            {
                AddSponsor.IsEnabled = true;
            }

            if (SponsorsLB.SelectedIndex > -1)
            {
                DeleteSponsor.IsEnabled = true;
            }
            else
            {
                DeleteSponsor.IsEnabled = false;
            }
            if (TeamLogo!=null && !string.IsNullOrWhiteSpace(NameBox.Text))
            {
                Save.IsEnabled = true;
            } else
            {
                Save.IsEnabled = false;
            }
        }

        private int GetMaxSections()
        {
            int sections;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select count(*) from section", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                sections = reader.GetInt32(0) * 2;
                reader.Close();
            }
            return sections;
        }

        public TeamEditor(int chosenTeam)
        {
            selectedTeamId = chosenTeam;
            InitializeComponent();
            AddCountriesToCB();
            AddTeamProperties(chosenTeam);
            GetAllSectionsToListBox();
            ButtonsChanging();
        }

        private void AddCitiesToCB()
        {
            ResidencyCB.Items.Clear();
            cityList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select id_mesto,nazev,id_stat_fk from mesto where id_stat_fk=" + nationList.ElementAt(NationalityCB.SelectedIndex).IdNation, conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    cityList.Add(new City(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
                }
                for (int i = 0; i < cityList.Count; i++)
                {
                    ResidencyCB.Items.Add(cityList.ElementAt(i).Name);
                }
                reader.Close();
            }
        }

        public void GetAllSectionsToListBox()
        {
            SectionLB.Items.Clear();
            teamSectionList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select teamxsection.id_teamxsection,shortcut from teamxsection join section on teamxsection.id_section=section.id_section where teamxsection.id_team=" + selectedTeamId, conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    teamSectionList.Add(new TeamSection(reader.GetInt32(0), reader.GetString(1), selectedTeamId));
                    Console.WriteLine(reader.GetInt32(0) + " " + reader.GetString(1) + " " + selectedTeamId);
                }
                for (int i = 0; i < teamSectionList.Count; i++)
                {
                    SectionLB.Items.Add(teamSectionList.ElementAt(i).Section);
                }
                reader.Close();
            }
            ButtonsChanging();
        }

        private void AddCountriesToCB()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select id_stat,jmeno from stat", conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    nationList.Add(new Nation(reader.GetInt32(0), reader.GetString(1)));
                }
                for (int i = 0; i < nationList.Count; i++)
                {
                    NationalityCB.Items.Add(nationList.ElementAt(i).Name);
                }
                reader.Close();
            }
        }

        private void AddTeamProperties(int chosenTeam)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select name,id_city_fk,logo, shortcut from v_team_edit where id_team =" + chosenTeam, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                AddLogo((byte[])reader[2]);
                SetDefaultCountryAndCity(reader.GetInt32(1));
                NameBox.Text = reader.GetString(0);
                ShortcutBox.Text = reader.GetString(3);
            }
        }

        private void AddLogo(byte[] data)
        {
            BitmapImage imageSource = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                    }
                    
                
                TeamLogo.Source = imageSource;


            }
        

        private void SetDefaultCountryAndCity(int cityID)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("select id_stat_fk from mesto where id_mesto=" + cityID, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                
                for (int j = 0; j < nationList.Count; j++)
                {

                    if (nationList.ElementAt(j).IdNation == reader.GetInt32(0))
                    {
                        NationalityCB.SelectedIndex = j;
                    }
                    
                }
                AddCitiesToCB();
            
                for (int i = 0; i < cityList.Count; i++)
                    {
                    if (cityList.ElementAt(i).CityId == cityID)
                        {
                        ResidencyCB.SelectedIndex = i;
                        break;
                    }
                }
            
            
            
                reader.Close();
            }
        }

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            AddSection win2 = new AddSection(selectedTeamId, GetNotAvailableSections());
            win2.Show(this);
        }

        private int[] GetNotAvailableSections()
        {
            int[] notAvailableSections;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();
                //zjistit počet nepoužitelných sekcí
                SQLiteCommand command = new SQLiteCommand("SELECT count(*) FROM (SELECT ID_SECTION,count(ID_SECTION) FROM TEAMXSECTION WHERE ID_TEAM = " + selectedTeamId + " GROUP BY ID_SECTION) WHERE `count(ID_SECTION)`>=2", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                notAvailableSections = new int[reader.GetInt32(0)];
                reader.Close();
                //přidat je do pole
                command.CommandText = "SELECT ID_SECTION FROM (SELECT ID_SECTION,count(ID_SECTION) FROM TEAMXSECTION WHERE ID_TEAM = " + selectedTeamId + " GROUP BY ID_SECTION) WHERE `count(ID_SECTION)`>=2";
                reader = command.ExecuteReader();
                int counter = 0;
                while (reader.Read())
                {
                    notAvailableSections[counter] = reader.GetInt32(0);
                    counter++;
                }
                reader.Close();
            }
            
            return notAvailableSections;
        }

        private void DeleteSectionClick(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                conn.Open();

                SQLiteCommand command = new SQLiteCommand("delete from tymxsekce where id_tym_sekce=" + teamSectionList.ElementAt(SectionLB.SelectedIndex).ID, conn);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    teamSectionList.Add(new TeamSection(reader.GetInt32(0), reader.GetString(1), selectedTeamId));
                    Console.WriteLine(reader.GetInt32(0) + " " + reader.GetString(1) + " " + selectedTeamId);
                }
                for (int i = 0; i < teamSectionList.Count; i++)
                {
                    SectionLB.Items.Add(teamSectionList.ElementAt(i).Section);
                }
                reader.Close();
            }
            GetAllSectionsToListBox();
        }

        private void SponsorsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonsChanging();
        }

        private void SectionLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonsChanging();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            //UDĚLAT UKLÁDÁNÍ OBÁRZKŮ DO DATABÁZE


            /*using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\test.db;"))
            {
                

                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] logoByte = br.ReadBytes((int)fs.Length);
                BinaryWriter writer = new BinaryWriter(new MemoryStream());
                writer.Write(logoByte);
                SQLiteBlob logoBlob = new SQLiteBlob(writer);
                conn.Open();
               // Console.WriteLine("update tym set nazev='" + NameBox.Text + "', id_stat_fk=" + nationList.ElementAt(NationalityCB.SelectedIndex).IdNation + ", logo=" + logoByte + " where id_tym=" + selectedTeamId);
               // SQLiteCommand command = new SQLiteCommand("update tym set nazev='" + NameBox.Text + "', id_stat_fk=" + nationList.ElementAt(NationalityCB.SelectedIndex).IdNation + ", logo=" + logoByte + " where id_tym=" + selectedTeamId, conn);
                SQLiteDataReader reader = command.ExecuteReader();
            }*/

            this.Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NameChange(object sender, TextChangedEventArgs e)
        {
            ButtonsChanging();
        }

        private void NewLogoClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files(.jpg,.png,.jpeg) | *.jpg;*.png;*.jpeg";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.FileName;
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = dialog.OpenFile();
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                TeamLogo.Source = imageSource;
            }
        }
        
        private void NationChange(object sender, SelectionChangedEventArgs e)
        {
            AddCitiesToCB();
        }

        
    }
}
