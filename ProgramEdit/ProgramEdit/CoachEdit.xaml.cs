using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace ProgramEdit
{
    /// <summary>
    /// Interaction logic for CoachEdit.xaml
    /// </summary>
    public partial class CoachEdit : Window
    {
        string database;
        List<Coach> coaches;
        List<Section> sections;
        List<Team> teams;
        public CoachEdit(string databaseName)
        {
            database = databaseName;
            coaches = new List<Coach>();
            sections = new List<Section>();
            teams = new List<Team>();
            InitializeComponent();
            AddData(0);
        }

        private void AddData(int a)
        {
            coaches.Clear();
            sections.Clear();
            teams.Clear();
            CoachesList.Items.Clear();
            SectionsCB.Items.Clear();
            TeamsCB.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + database + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_coach, coach.name, surname, nick, contractEnd, training, coach.id_section, section.name, coach.id_team, team.name from coach join section on coach.id_section=section.id_section join team on team.id_team=coach.id_team order by nick;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    coaches.Add(new Coach(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetString(7), reader.GetInt32(8), reader.GetString(9)));
                }
                reader.Close();
                command = new SQLiteCommand("select id_section, name from section", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sections.Add(new Section(reader.GetInt32(0), reader.GetString(1)));
                }
                reader.Close();
                command = new SQLiteCommand("select id_team, name from team", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    teams.Add(new Team(reader.GetInt32(0), reader.GetString(1)));
                }
                reader.Close();
            }
            for (int i = 0; i < coaches.Count; i++)
            {
                CoachesList.Items.Add(coaches.ElementAt(i).Nick);
            }
            for (int i = 0; i < sections.Count; i++)
            {
                SectionsCB.Items.Add(sections.ElementAt(i).Name);
            }
            for (int i = 0; i < teams.Count; i++)
            {
                TeamsCB.Items.Add(teams.ElementAt(i).Name);
            }
            if (a != -1)
            {
                CoachesList.SelectedIndex = a;
            } 
            else if (CoachesList.Items.Count > 0)
            {
                CoachesList.SelectedIndex = 0;
            }
        }

        private void CoachesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoachesList.SelectedIndex > -1)
            {
                Nick.Text = coaches.ElementAt(CoachesList.SelectedIndex).Nick;
                Name.Text = coaches.ElementAt(CoachesList.SelectedIndex).Name;
                Surname.Text = coaches.ElementAt(CoachesList.SelectedIndex).Surname;
                Training.Text = coaches.ElementAt(CoachesList.SelectedIndex).Training.ToString();
                ContractEnd.SelectedDate = new DateTime(int.Parse(coaches.ElementAt(CoachesList.SelectedIndex).ContractEnd.Substring(0, 4)), int.Parse(coaches.ElementAt(CoachesList.SelectedIndex).ContractEnd.Substring(5, 2)), int.Parse(coaches.ElementAt(CoachesList.SelectedIndex).ContractEnd.Substring(8, 2)));
                for (int i = 0; i < sections.Count; i++)
                {
                    if (sections.ElementAt(i).IdSection == coaches.ElementAt(CoachesList.SelectedIndex).IdSection)
                    {
                        SectionsCB.SelectedIndex = i;
                    }
                }
                for (int i = 0; i < teams.Count; i++)
                {
                    if (teams.ElementAt(i).IdTeam == coaches.ElementAt(CoachesList.SelectedIndex).IdTeam)
                    {
                        TeamsCB.SelectedIndex = i;
                    }
                }
                Saved.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateCoach_Click(object sender, RoutedEventArgs e)
        {
            string dayString = ContractEnd.SelectedDate.Value.Day.ToString();
            if (dayString.Length == 1)
            {
                dayString = "0" + dayString;
            }
            string monthString = ContractEnd.SelectedDate.Value.Month.ToString();
            if (monthString.Length == 1)
            {
                monthString = "0" + monthString;
            }
            string contract = ContractEnd.SelectedDate.Value.Year + "-" + monthString + "-" + dayString;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + database + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update coach set name='" + Name.Text + "', surname='" + Surname.Text  + "', nick='" + Nick.Text + "', contractEnd='" + contract +"', training=" + Training.Text + ", id_section=" + sections.ElementAt(SectionsCB.SelectedIndex).IdSection +", id_team=" + teams.ElementAt(TeamsCB.SelectedIndex).IdTeam +" where id_coach=" + coaches.ElementAt(CoachesList.SelectedIndex).IdCoach + ";", conn);
                command.ExecuteReader();
            }
            Saved.Visibility = Visibility.Visible;
            AddData(CoachesList.SelectedIndex);
        }

        private void AddNewCoach_Click(object sender, RoutedEventArgs e)
        {
            string dayString = ContractEnd.SelectedDate.Value.Day.ToString();
            if (dayString.Length == 1)
            {
                dayString = "0" + dayString;
            }
            string monthString = ContractEnd.SelectedDate.Value.Month.ToString();
            if (monthString.Length == 1)
            {
                monthString = "0" + monthString;
            }
            string contract = ContractEnd.SelectedDate.Value.Year + "-" + monthString + "-" + dayString;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + database + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("insert into coach (name,surname,nick,contractEnd,training,id_section,id_team) values ('" + Name.Text + "','" + Surname.Text + "','" + Nick.Text + "','" + contract + "'," + Training.Text + "," + sections.ElementAt(SectionsCB.SelectedIndex).IdSection + "," + teams.ElementAt(TeamsCB.SelectedIndex).IdTeam + ");", conn);
                command.ExecuteReader();
            }
            Saved.Visibility = Visibility.Visible;
            AddData(CoachesList.SelectedIndex);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + database + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("delete from coach where id_coach=" + coaches.ElementAt(CoachesList.SelectedIndex).IdCoach + ";", conn);
                command.ExecuteReader();
            }
            AddData(0);
        }
    }
}
