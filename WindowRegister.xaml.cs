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
using System.Text.RegularExpressions;

namespace NewApp
{
    public partial class WindowRegister : Window
    {
        public WindowRegister()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Length < 3)
            {
                MessageBox.Show("Имя пользователя не может быть меньше 3 символов.");
                return;
            }
            if (Password.Password.Length < 5)
            {
                MessageBox.Show("Пароль не может быть меньше 5 символов.");
                return;
            }
            for (int i = 0; i < Login.Text.Length; i++)
            {
                if (Regex.IsMatch(Login.Text[i].ToString(), @"[ -/\:-@\[-`\{-~\№]$"))
                {
                    MessageBox.Show("Имя пользователя может состоять только из букв и цифр.");
                    return;
                }
            }
            for (int i = 0; i < Password.Password.Length; i++)
            {
                if (Regex.IsMatch(Password.Password[i].ToString(), @"[ -/\:-@\[-`\{-~\№]$"))
                {
                    MessageBox.Show("Пароль может состоять только из букв и цифр.");
                    return;
                }
            }
            string result = MainWindow.server.SendServer($"Procedure,RegisterUser,{Login.Text},{Password.Password}", true);
            switch (result)
            {
                case "Успешная регистрация.":
                    MessageBox.Show(result);
                    MainWindow window = new MainWindow();
                    Hide();
                    window.Show();
                    break;
                case "Имя пользователя уже занято.":
                    MessageBox.Show(result);
                    break;
            }
        }
        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            Hide();
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.server.GetClient.Close();
        }

        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
