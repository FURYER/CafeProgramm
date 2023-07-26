using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace NewApp
{
    public partial class MainWindow : Window
    {
        public static Server server = new Server();

        public MainWindow()
        {
            InitializeComponent();
            if (!server.GetClient.Connected)
            {
                new Thread(() => server.OpenConnection()).Start();
                server.handle.WaitOne();
            }
        }

        private void Join_Click(object sender, RoutedEventArgs e)
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
            string result = server.SendServer($"Procedure,CheckUser,{Login.Text},{Password.Password}", true);
            switch (result)
            {
                case "Успешная авторизация.":
                    WindowApp window = new WindowApp();
                    Hide();
                    window.Show();
                    break;
                case "Неправильное имя пользователя.":
                    MessageBox.Show(result);
                    break;
                case "Неправильный пароль.":
                    MessageBox.Show(result);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowRegister window = new WindowRegister();
            window.Show();
            Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
