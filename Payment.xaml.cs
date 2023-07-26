using NewApp;
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

namespace BusinessByJob
{
    /// <summary>
    /// Логика взаимодействия для Payment.xaml
    /// </summary>
    public partial class Payment : Window
    {

        Random rnd = new Random();

        double money;
        double totalPrice;
        string requisites;

        public Payment()
        {
            InitializeComponent();
            money = rnd.Next(0, 10000);
            Money.Text += money + " руб.";
            requisites = RandomString(8);
            Requisites.Text += requisites;
        }

        public void SetPrice(double price)
        {
            TotalPrice.Text += string.Format("{0:0.00}", price) + " руб.";
            totalPrice = price;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (money > totalPrice)
            {
                this.DialogResult = true;
                WindowApp.order.requisites = requisites;
                MessageBox.Show("Оплата прошла успешно!");
                this.Close();
            }
            else
            {
                this.DialogResult = false;
                MessageBox.Show("Недостаточно средств!");
                this.Close();
            }
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
