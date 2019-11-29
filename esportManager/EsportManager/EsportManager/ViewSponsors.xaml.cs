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

namespace EsportManager
{
    /// <summary>
    /// Interaction logic for ViewSponsors.xaml
    /// </summary>
    public partial class ViewSponsors : Window
    {
        public ViewSponsors FormParent { get; set; }
         
        string databaseName;
        int idTeam;
        string date;
        int[] availableSponsorsIndexes = new int[3];
        Random random = new Random(); // nastavit seed na: měsíc * rok aby se sponzoři a datumy měnily každý měsíc!!

        public ViewSponsors(int formType, string name)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            databaseName = name;
            InitializeComponent();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_team, date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                idTeam = reader.GetInt32(0);
                date = reader.GetString(1);
                reader.Close();
            }
            ShowRightComponents(formType);
        }

        private void ShowRightComponents(int formType)//1=summary,2=add new sponsor
        {
            if (formType == 1)
            {
                Sponsor1SignContract.Visibility = Visibility.Hidden;
                Sponsor2SignContract.Visibility = Visibility.Hidden;
                Sponsor3SignContract.Visibility = Visibility.Hidden;
                int numberOfSponsors = CheckNumberOfSponsors();
                if (numberOfSponsors < 3)
                {
                    AddSponsor3.Visibility = Visibility.Visible;
                    Sponsor3BonusIncome.Visibility = Visibility.Hidden;
                    Sponsor3BonusIncomeLabel.Visibility = Visibility.Hidden;
                    Sponsor3CancelContract.Visibility = Visibility.Hidden;
                    Sponsor3DateOfExpiration.Visibility = Visibility.Hidden;
                    Sponsor3DateOfExpirationLabel.Visibility = Visibility.Hidden;
                    Sponsor3MonthlyIncome.Visibility = Visibility.Hidden;
                    Sponsor3MonthlyIncomeLabel.Visibility = Visibility.Hidden;
                    Sponsor3Name.Visibility = Visibility.Hidden;
                    Sponsor3RenewContract.Visibility = Visibility.Hidden;
                }
                if (numberOfSponsors < 2)
                {
                    AddSponsor2.Visibility = Visibility.Visible;
                    Sponsor2BonusIncome.Visibility = Visibility.Hidden;
                    Sponsor2BonusIncomeLabel.Visibility = Visibility.Hidden;
                    Sponsor2CancelContract.Visibility = Visibility.Hidden;
                    Sponsor2DateOfExpiration.Visibility = Visibility.Hidden;
                    Sponsor2DateOfExpirationLabel.Visibility = Visibility.Hidden;
                    Sponsor2MonthlyIncome.Visibility = Visibility.Hidden;
                    Sponsor2MonthlyIncomeLabel.Visibility = Visibility.Hidden;
                    Sponsor2Name.Visibility = Visibility.Hidden;
                    Sponsor2RenewContract.Visibility = Visibility.Hidden;
                }
                if (numberOfSponsors < 1)
                {
                    AddSponsor1.Visibility = Visibility.Visible;
                    Sponsor1BonusIncome.Visibility = Visibility.Hidden;
                    Sponsor1BonusIncomeLabel.Visibility = Visibility.Hidden;
                    Sponsor1CancelContract.Visibility = Visibility.Hidden;
                    Sponsor1DateOfExpiration.Visibility = Visibility.Hidden;
                    Sponsor1DateOfExpirationLabel.Visibility = Visibility.Hidden;
                    Sponsor1MonthlyIncome.Visibility = Visibility.Hidden;
                    Sponsor1MonthlyIncomeLabel.Visibility = Visibility.Hidden;
                    Sponsor1Name.Visibility = Visibility.Hidden;
                    Sponsor1RenewContract.Visibility = Visibility.Hidden;
                }
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select sponsor2.id_sponsor, sponsor2.name, monthly_payment, renew_bonus, success_payment, teamxsponsor.expiration_date from sponsor2 join teamxsponsor on sponsor2.id_sponsor=teamxsponsor.id_sponsor where teamxsponsor.id_team=" + idTeam + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    int sponsorsCount = 0;
                    while (reader.Read())
                    {
                        if (sponsorsCount == 2)
                        {
                            availableSponsorsIndexes[2] = reader.GetInt32(0);
                            Sponsor3Name.Content = reader.GetString(1);
                            Sponsor3MonthlyIncome.Content = reader.GetInt32(2) + "$";
                            Sponsor3BonusIncome.Content = reader.GetInt32(3) + "$";
                            Console.WriteLine(reader.GetDouble(4));
                            Sponsor3DateOfExpiration.Content = reader.GetString(5);
                            int yearExpire = int.Parse(reader.GetString(5).Remove(4, 6));
                            int monthExpire = int.Parse(reader.GetString(5).Remove(7, 3).Remove(0, 5));
                            int yearCurrent = int.Parse(date.Remove(4, 6));
                            int monthCurrent = int.Parse(date.Remove(7, 3).Remove(0, 5));
                            if (yearExpire != yearCurrent || monthCurrent != monthExpire)
                            {
                                Sponsor3RenewContract.IsEnabled = false;
                            }
                        }
                        if (sponsorsCount == 1)
                        {
                            availableSponsorsIndexes[1] = reader.GetInt32(0);
                            Sponsor2Name.Content = reader.GetString(1);
                            Sponsor2MonthlyIncome.Content = reader.GetInt32(2) + "$";
                            Sponsor2BonusIncome.Content = reader.GetInt32(3) + "$";
                            Console.WriteLine(reader.GetDouble(4));
                            Sponsor2DateOfExpiration.Content = reader.GetString(5);
                            int yearExpire = int.Parse(reader.GetString(5).Remove(4, 6));
                            int monthExpire = int.Parse(reader.GetString(5).Remove(7, 3).Remove(0, 5));
                            int yearCurrent = int.Parse(date.Remove(4, 6));
                            int monthCurrent = int.Parse(date.Remove(7, 3).Remove(0, 5));
                            if (yearExpire != yearCurrent || monthCurrent != monthExpire)
                            {
                                Sponsor2RenewContract.IsEnabled = false;
                            }
                            sponsorsCount++;
                        }
                        if (sponsorsCount == 0)
                        {
                            availableSponsorsIndexes[0] = reader.GetInt32(0);
                            Sponsor1Name.Content = reader.GetString(1);
                            Sponsor1MonthlyIncome.Content = reader.GetInt32(2) + "$";
                            Sponsor1BonusIncome.Content = reader.GetInt32(3) + "$";
                            Console.WriteLine(reader.GetDouble(4));
                            Sponsor1DateOfExpiration.Content = reader.GetString(5);
                            int yearExpire = int.Parse(reader.GetString(5).Remove(4, 6));
                            int monthExpire = int.Parse(reader.GetString(5).Remove(7, 3).Remove(0, 5));
                            int yearCurrent = int.Parse(date.Remove(4, 6));
                            int monthCurrent = int.Parse(date.Remove(7, 3).Remove(0, 5));
                            if (yearExpire != yearCurrent || monthCurrent != monthExpire)
                            {
                                Sponsor1RenewContract.IsEnabled = false;
                            }
                            sponsorsCount++;
                        }
                    }
                    reader.Close();
                }
            }
            else if (formType == 2)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select count(*) from sponsor2 join team on team.reputation>=sponsor2.min_team_strength where id_team=" + idTeam + ";", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int possibleSponsorCount = reader.GetInt32(0);
                    reader.Close();
                    int[] chosenSponsors = new int[3];
                    do
                    {
                        chosenSponsors[0] = random.Next(0, possibleSponsorCount);
                        chosenSponsors[1] = random.Next(0, possibleSponsorCount);
                        chosenSponsors[2] = random.Next(0, possibleSponsorCount);
                    } while (chosenSponsors.Distinct().Count() != chosenSponsors.Length);
                    Array.Sort(chosenSponsors);


                    command = new SQLiteCommand("select id_sponsor, sponsor2.name, monthly_payment, renew_bonus, success_payment from sponsor2 join team on team.reputation>=sponsor2.min_team_strength where id_team=" + idTeam + ";", conn);
                    reader = command.ExecuteReader();
                    int commandCount = 0;
                    while (reader.Read())
                    {
                        if (commandCount == chosenSponsors[0])
                        {
                            availableSponsorsIndexes[0] = reader.GetInt32(0);
                            Sponsor1Name.Content = reader.GetString(1);
                            Sponsor1MonthlyIncome.Content = reader.GetInt32(2) + "$";
                            Sponsor1BonusIncome.Content = reader.GetInt32(3) + "$";
                            Console.WriteLine(reader.GetDouble(4));
                        }
                        if (commandCount == chosenSponsors[1])
                        {
                            availableSponsorsIndexes[1] = reader.GetInt32(0);
                            Sponsor2Name.Content = reader.GetString(1);
                            Sponsor2MonthlyIncome.Content = reader.GetInt32(2) + "$";
                            Sponsor2BonusIncome.Content = reader.GetInt32(3) + "$";
                            Console.WriteLine(reader.GetDouble(4));
                        }
                        if (commandCount == chosenSponsors[2])
                        {
                            availableSponsorsIndexes[2] = reader.GetInt32(0);
                            Sponsor3Name.Content = reader.GetString(1);
                            Sponsor3MonthlyIncome.Content = reader.GetInt32(2) + "$";
                            Sponsor3BonusIncome.Content = reader.GetInt32(3) + "$";
                            Console.WriteLine(reader.GetDouble(4));
                        }
                        commandCount++;
                    }
                    reader.Close();
                }
                Sponsor1CancelContract.Visibility = Visibility.Hidden;
                Sponsor2CancelContract.Visibility = Visibility.Hidden;
                Sponsor3CancelContract.Visibility = Visibility.Hidden;

                Sponsor1BonusIncomeLabel.Content = "Bonus při podpisu";
                Sponsor2BonusIncomeLabel.Content = "Bonus při podpisu";
                Sponsor3BonusIncomeLabel.Content = "Bonus při podpisu";

                Sponsor1DateOfExpiration.Content = GetExpirationDate();
                Sponsor2DateOfExpiration.Content = GetExpirationDate();
                Sponsor3DateOfExpiration.Content = GetExpirationDate();
            }
            //naplnit sponzory validními hodnotami
        }

        private string GetExpirationDate()
        {
            int year = int.Parse(date.Remove(4, 6));
            year += random.Next(1, 5);
            string yearString = year.ToString() + date.Remove(0, 4);
            return yearString;
        }

        private int CheckNumberOfSponsors()
        {
            // zjistit počet sponsorů
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select count(*) from teamxsponsor where id_team=" + idTeam + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                int count = reader.GetInt32(0);
                reader.Close();
                return count;
            }
            
        }

        private void AddNewSponsor(object sender, RoutedEventArgs e)
        {
            ViewSponsors win2 = new ViewSponsors(2, databaseName);
            win2.FormParent = this;
            win2.Show();
        }

        private void SignContract1(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("insert into teamxsponsor(id_team, id_sponsor, expiration_date) values (" + idTeam + "," + availableSponsorsIndexes[0] + ",'" + Sponsor1DateOfExpiration.Content + "');", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update team set budget=budget+" + Sponsor1BonusIncome.Content.ToString().Substring(0, Sponsor1BonusIncome.Content.ToString().Length - 1) + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            this.Close();
        }

        private void SignContract2(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("insert into teamxsponsor(id_team, id_sponsor, expiration_date) values (" + idTeam + "," + availableSponsorsIndexes[1] + ",'" + Sponsor2DateOfExpiration.Content + "');", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update team set budget=budget+" + Sponsor2BonusIncome.Content.ToString().Substring(0, Sponsor2BonusIncome.Content.ToString().Length - 1) + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            this.Close();
        }

        private void SignContract3(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("insert into teamxsponsor(id_team, id_sponsor, expiration_date) values (" + idTeam + "," + availableSponsorsIndexes[2] + ",'" + Sponsor3DateOfExpiration.Content + "');", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update team set budget=budget+" + Sponsor3BonusIncome.Content.ToString().Substring(0, Sponsor3BonusIncome.Content.ToString().Length-1) + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            this.Close();
        }

        private void CancelContract1(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                if (!Sponsor1RenewContract.IsEnabled)
                {
                    int cancelFee = int.Parse(Sponsor1BonusIncome.Content.ToString().Substring(0, Sponsor1BonusIncome.Content.ToString().Length - 1));
                    MessageBoxResult result = MessageBox.Show("Vážně se chcete rozvázat smlouvu s firmou " + Sponsor1Name.Content + "? Bude Vás to stát " + cancelFee + "$.", "Chystáte se rozvázat smlouvu", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    command = new SQLiteCommand("update team set budget=budget-" + cancelFee + " where id_team=" + idTeam + ";", conn);
                    command.ExecuteReader();
                }
                command = new SQLiteCommand("delete from teamxsponsor where id_team=" + idTeam + " and id_sponsor=" + availableSponsorsIndexes[0] + ";", conn);
                command.ExecuteReader();
            }
            this.ShowRightComponents(1);
        }

        private void CancelContract2(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                if (!Sponsor2RenewContract.IsEnabled)
                {
                    int cancelFee = int.Parse(Sponsor2BonusIncome.Content.ToString().Substring(0, Sponsor2BonusIncome.Content.ToString().Length - 1));
                    MessageBoxResult result = MessageBox.Show("Vážně se chcete rozvázat smlouvu s firmou " + Sponsor2Name.Content + "? Bude Vás to stát " + cancelFee + "$.", "Chystáte se rozvázat smlouvu", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    command = new SQLiteCommand("update team set budget=budget-" + cancelFee + " where id_team=" + idTeam + ";", conn);
                    command.ExecuteReader();
                }
                command = new SQLiteCommand("delete from teamxsponsor where id_team=" + idTeam + " and id_sponsor=" + availableSponsorsIndexes[1] + ";", conn);
                command.ExecuteReader();
            }
            this.ShowRightComponents(1);
        }

        private void CancelContract3(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                if (!Sponsor3RenewContract.IsEnabled)
                {
                    int cancelFee = int.Parse(Sponsor3BonusIncome.Content.ToString().Substring(0, Sponsor3BonusIncome.Content.ToString().Length - 1));
                    MessageBoxResult result = MessageBox.Show("Vážně se chcete rozvázat smlouvu s firmou " + Sponsor3Name.Content + "? Bude Vás to stát " + cancelFee + "$.", "Chystáte se rozvázat smlouvu", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    command = new SQLiteCommand("update team set budget=budget-" + cancelFee + " where id_team=" + idTeam + ";", conn);
                    command.ExecuteReader();
                }
                command = new SQLiteCommand("delete from teamxsponsor where id_team=" + idTeam + " and id_sponsor=" + availableSponsorsIndexes[2] + ";", conn);
                command.ExecuteReader();
            }
            this.ShowRightComponents(1);
        }

        private void RenewSponsor2(object sender, RoutedEventArgs e)
        {
            string date = Sponsor2DateOfExpiration.Content.ToString();
            int year = int.Parse(date.Remove(4, 6)) + random.Next(1, 4);
            string yearString = year.ToString() + date.Remove(0, 4);
            MessageBoxResult result = MessageBox.Show(Sponsor2Name.Content + " s vámi obnoví smlouvu do roku " + year + ".\nSouhlasíte s podmínkami?", "Chystáte se obnovit smlouvu", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update teamxsponsor set expiration_date='" + yearString + "' where id_team=" + idTeam + " and id_sponsor=" + availableSponsorsIndexes[1] + ";", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update team set budget=budget+" + Sponsor2BonusIncome.Content.ToString().Substring(0, Sponsor2BonusIncome.Content.ToString().Length - 1) + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            this.ShowRightComponents(1);
        }

        private void RenewSponsor1(object sender, RoutedEventArgs e)
        {
            string date = Sponsor1DateOfExpiration.Content.ToString();
            int year = int.Parse(date.Remove(4, 6)) + random.Next(1, 4);
            string yearString = year.ToString() + date.Remove(0, 4);
            MessageBoxResult result = MessageBox.Show(Sponsor1Name.Content + " s vámi obnoví smlouvu do roku " + year + ".\nSouhlasíte s podmínkami?", "Chystáte se obnovit smlouvu", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update teamxsponsor set expiration_date='" + yearString + "' where id_team=" + idTeam + " and id_sponsor=" + availableSponsorsIndexes[0] + ";", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update team set budget=budget+" + Sponsor1BonusIncome.Content.ToString().Substring(0, Sponsor1BonusIncome.Content.ToString().Length - 1) + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            this.ShowRightComponents(1);
        }

        private void RenewSponsor3(object sender, RoutedEventArgs e)
        {
            string date = Sponsor3DateOfExpiration.Content.ToString();
            int year = int.Parse(date.Remove(4, 6)) + random.Next(1, 4);
            string yearString = year.ToString() + date.Remove(0, 4);
            MessageBoxResult result = MessageBox.Show(Sponsor3Name.Content + " s vámi obnoví smlouvu do roku " + year + ".\nSouhlasíte s podmínkami?", "Chystáte se obnovit smlouvu", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + databaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update teamxsponsor set expiration_date='" + yearString + "' where id_team=" + idTeam + " and id_sponsor=" + availableSponsorsIndexes[2] + ";", conn);
                command.ExecuteReader();
                command = new SQLiteCommand("update team set budget=budget+" + Sponsor3BonusIncome.Content.ToString().Substring(0, Sponsor3BonusIncome.Content.ToString().Length - 1) + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            this.ShowRightComponents(1);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (FormParent == null)
            {
                ShowRightComponents(1);
            }
            
        }
    }
}
