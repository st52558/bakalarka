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
    /// Interaction logic for ViewSponsors.xaml
    /// </summary>
    public partial class ViewSponsors : Window
    {
        private string databaseName;
        Random random = new Random(); // nastavit seed na: měsíc * rok aby se sponzoři a datumy měnily každý měsíc!!

        public ViewSponsors(int formType, string name)
        {
            
            InitializeComponent();
            ShowRightComponents(formType);
            databaseName = name;
        }

        private void ShowRightComponents(int formType)//1=summary,2=add new sponsor
        {
            if (formType == 1) {
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
            }
            else if (formType == 2)
            {
                Sponsor1BonusIncomeLabel.Content = "Bonus při podpisu";
                Sponsor2BonusIncomeLabel.Content = "Bonus při podpisu";
                Sponsor3BonusIncomeLabel.Content = "Bonus při podpisu";
                
                Sponsor1DateOfExpiration.Content = GetExpirationDate().ToShortDateString();
                Sponsor2DateOfExpiration.Content = GetExpirationDate().ToShortDateString();
                Sponsor3DateOfExpiration.Content = GetExpirationDate().ToShortDateString();
            }
            //naplnit sponzory validními hodnotami
        }

        private DateTime GetExpirationDate()
        {
            
            DateTime now = new DateTime(2019, 10, 8);
            return now.AddYears(random.Next(1,5));
        }

        private int CheckNumberOfSponsors()
        {
            // zjistit počet sponsorů
            return 3;
        }

        private void AddNewSponsor(object sender, RoutedEventArgs e)
        {
            ViewSponsors win2 = new ViewSponsors(2, databaseName);
            win2.Show();
        }
    }
}
