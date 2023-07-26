using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Schema;
using System.Windows.Automation.Peers;
using BusinessByJob;
using ProtoBuf;
using System.Timers;
using System.Security.Cryptography.X509Certificates;
using static NewApp.UserInfo;
using System.Windows.Media.Effects;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Diagnostics;

namespace NewApp
{
    public partial class WindowApp : Window
    {

        public static Order order;

        public WindowApp()
        {
            try
            {
                InitializeComponent();
                Timer timer = new Timer();
                timer = new Timer(2000);
                timer.Elapsed += UpdateChatControl;
                timer.AutoReset = true;
                timer.Enabled = true;
                if (Server.user.persona != "Пользователь")
                {
                    Product.Visibility = Visibility.Visible;
                    Orders.Visibility = Visibility.Visible;
                    LoadNomenclatureTypes();
                    LoadNomenclature();
                    LoadProducts();
                    LoadOrders();
                    timer.Elapsed += UpdateProductControl;
                }
                LoadData();
                LoadProfile();
                LoadChat();
                LoadUsers();
                LoadChatRooms();
                LoadAvailableProducts();
                LoadMyOrderList();
                if (Server.user.permission == "Admin")
                {
                    Administration.Visibility = Visibility.Visible;
                    LoadUsersForAdmin();
                }
                Timer timer2 = new Timer(2000);
                timer2.Elapsed += UpdateOrder;
                timer2.AutoReset = true;
                timer2.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void UpdateChat(object sender, MouseButtonEventArgs e)
        {
            LoadChat();
        }

        public void UpdateProfile(object sender, MouseButtonEventArgs e)
        {
            LoadProfile();
        }

        public void UpdateOrder(object sender, ElapsedEventArgs arg)
        {
            LoadMyOrderList();
        }

        public void LoadData()
        {
            if (Server.user.fullname != null)
            {
                textboxSurname.Text = Server.user.fullname[0];
                textboxName.Text = Server.user.fullname[1];
                textboxSecondName.Text = Server.user.fullname[2];
            }
            LabelPermission.Content += Server.user.permission;
            LabelPersona.Content += Server.user.persona;
            if (Server.user.img != null)
                ImgUser.Source = ByteArrayToImage(Server.user.img);
            LabelRegisterDate.Content += Server.user.registerDate.ToString("d MMMM yyyy HH:mm");
        }

        public void LoadChatRooms()
        {
            MainWindow.server.SendServer($"Procedure,LoadChatRooms", true);
            if (Server.user.chats.Count > Chats.Items.Count)
            {
                for (int i = Chats.Items.Count; i < Server.user.chats.Count; i++)
                {
                    StackPanel panel = new StackPanel();
                    panel.Margin = new Thickness(5);
                    panel.DataContext = Server.user.chats[i];
                    panel.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    if (Server.user.chats[i].nameUser != null)
                        name.Content = Server.user.chats[i].nameUser[1] + " " + Server.user.chats[i].nameUser[0];
                    else
                        name.Content = Server.user.chats[i].loginUser;
                    name.FontSize = 18;
                    ImageBrush brush = new ImageBrush();
                    brush.Stretch = Stretch.UniformToFill;
                    if (Server.user.chats[i].imgUser != null)
                        brush.ImageSource = ByteArrayToImage(Server.user.chats[i].imgUser);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = brush;
                    ellipse.Height = 40;
                    ellipse.Width = 40;
                    ellipse.Margin = new Thickness(0, 0, 5, 0);
                    panel.Children.Add(ellipse);
                    panel.Children.Add(name);
                    if (Server.user.chats[i].check == false)
                    {
                        Ellipse ellipse2 = new Ellipse();
                        ellipse2.Fill = new SolidColorBrush(Colors.Gray);
                        ellipse2.Height = 20;
                        ellipse2.Width = 20;
                        ellipse2.VerticalAlignment = VerticalAlignment.Center;
                        ellipse2.Margin = new Thickness(0, 0, 5, 0);
                        panel.Children.Add(ellipse2);
                    }
                    Chats.Items.Insert(0, panel);
                }
            }
        }

        public void LoadChat()
        {
            ListFavourites.Items.Clear();
            MainWindow.server.SendServer($"Procedure,LoadChat", true);
            foreach (Favourites item in Server.user.favourites)
            {
                if (comboBoxFilter.SelectedIndex == 0)
                {
                    StackPanel panel = new StackPanel();
                    panel.DataContext = item;
                    ImageBrush brush = new ImageBrush();
                    panel.Orientation = Orientation.Horizontal;
                    brush.Stretch = Stretch.UniformToFill;
                    if (item.img != null)
                        brush.ImageSource = ByteArrayToImage(item.img);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = brush;
                    ellipse.Height = 50;
                    ellipse.Width = 50;
                    ellipse.Margin = new Thickness(5);
                    Label status = new Label();
                    if (item.status == true)
                    {
                        status.Content = "Онлайн";
                        status.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        status.Content = "Оффлайн";
                        status.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    Label label = new Label();
                    if (item.fullname != null)
                        label.Content = item.fullname[1] + " " + item.fullname[0];
                    else
                        label.Content = item.login;
                    Label label2 = new Label();
                    label2.Content = item.persona;
                    if (item.persona != "Пользователь")
                        label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                    else
                        label2.Foreground = new SolidColorBrush(Colors.Gray);
                    panel.Children.Add(ellipse);
                    panel.Children.Add(label);
                    panel.Children.Add(label2);
                    panel.Children.Add(status);
                    ListFavourites.Items.Add(panel);
                }
                else if (item.status == true)
                {
                    StackPanel panel = new StackPanel();
                    panel.DataContext = item.id.ToString();
                    ImageBrush brush = new ImageBrush();
                    panel.Orientation = Orientation.Horizontal;
                    brush.Stretch = Stretch.UniformToFill;
                    if (item.img != null)
                        brush.ImageSource = ByteArrayToImage(item.img);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = brush;
                    ellipse.Height = 50;
                    ellipse.Width = 50;
                    ellipse.Margin = new Thickness(5);
                    Label status = new Label();
                    status.Content = "Онлайн";
                    status.Foreground = new SolidColorBrush(Colors.Green);
                    Label label = new Label();
                    if (item.fullname != null)
                        label.Content = item.fullname[1] + " " + item.fullname[0];
                    else
                        label.Content = item.login;
                    Label label2 = new Label();
                    label2.Content = item.persona;
                    if (item.persona != "Пользователь")
                        label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                    else
                        label2.Foreground = new SolidColorBrush(Colors.Gray);
                    panel.Children.Add(ellipse);
                    panel.Children.Add(label);
                    panel.Children.Add(label2);
                    panel.Children.Add(status);
                    ListFavourites.Items.Add(panel);
                }
            }
        }

        public void LoadUsers()
        {
            ListUsers.Items.Clear();
            MainWindow.server.SendServer($"Procedure,LoadUsers", true);
            foreach (Users.User item in Server.users.users)
            {
                bool check = false;
                foreach (Favourites item2 in Server.user.favourites)
                {
                    if (item.id == item2.id)
                    {
                        check = true;
                        break;
                    }
                }
                if (check == true)
                {
                    continue;
                }
                StackPanel stackPanel = new StackPanel();
                stackPanel.DataContext = item.id.ToString();
                ImageBrush brush = new ImageBrush();
                stackPanel.Orientation = Orientation.Horizontal;
                brush.Stretch = Stretch.UniformToFill;
                if (item.img != null)
                    brush.ImageSource = ByteArrayToImage(item.img);
                else
                    brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = brush;
                ellipse.Height = 50;
                ellipse.Width = 50;
                ellipse.Margin = new Thickness(5);
                Label label = new Label();
                Label label2 = new Label();
                label2.Content = item.persona;
                if (item.persona != "Пользователь")
                {
                    label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                }
                else
                    label2.Foreground = new SolidColorBrush(Colors.Gray);
                if (item.fullname != null)
                    label.Content = item.fullname[1] + " " + item.fullname[0];
                else
                    label.Content = item.login;
                stackPanel.Children.Add(ellipse);
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(label2);
                ListUsers.Items.Add(stackPanel);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListFavourites == null)
                return;
            ListFavourites.Items.Clear();
            MainWindow.server.SendServer($"Procedure,LoadChat", true);
            foreach(Favourites item in Server.user.favourites)
            {
                if (item.login.Contains(FindFavourites.Text))
                {
                    if (comboBoxFilter.SelectedIndex == 0)
                    {
                        StackPanel panel = new StackPanel();
                        panel.DataContext = item.id.ToString();
                        ImageBrush brush = new ImageBrush();
                        panel.Orientation = Orientation.Horizontal;
                        brush.Stretch = Stretch.UniformToFill;
                        if (item.img != null)
                            brush.ImageSource = ByteArrayToImage(item.img);
                        else
                            brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = brush;
                        ellipse.Height = 50;
                        ellipse.Width = 50;
                        ellipse.Margin = new Thickness(5);
                        Label status = new Label();
                        if (item.status == true)
                        {
                            status.Content = "Онлайн";
                            status.Foreground = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            status.Content = "Оффлайн";
                            status.Foreground = new SolidColorBrush(Colors.Gray);
                        }
                        Label label = new Label();
                        if (item.fullname != null)
                            label.Content = item.fullname[1] + " " + item.fullname[0];
                        else
                            label.Content = item.login;
                        Label label2 = new Label();
                        label2.Content = item.persona;
                        if (item.persona != "Пользователь")
                            label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                        else
                            label2.Foreground = new SolidColorBrush(Colors.Gray);
                        panel.Children.Add(ellipse);
                        panel.Children.Add(label);
                        panel.Children.Add(label2);
                        panel.Children.Add(status);
                        ListFavourites.Items.Add(panel);
                    }
                    else if (item.status == true)
                    {
                        StackPanel panel = new StackPanel();
                        panel.DataContext = item.id.ToString();
                        ImageBrush brush = new ImageBrush();
                        panel.Orientation = Orientation.Horizontal;
                        brush.Stretch = Stretch.UniformToFill;
                        if (item.img != null)
                            brush.ImageSource = ByteArrayToImage(item.img);
                        else
                            brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = brush;
                        ellipse.Height = 50;
                        ellipse.Width = 50;
                        ellipse.Margin = new Thickness(5);
                        Label status = new Label();
                        status.Content = "Онлайн";
                        status.Foreground = new SolidColorBrush(Colors.Green);
                        Label label = new Label();
                        if (item.fullname != null)
                            label.Content = item.fullname[1] + " " + item.fullname[0];
                        else
                            label.Content = item.login;
                        Label label2 = new Label();
                        label2.Content = item.persona;
                        if (item.persona != "Пользователь")
                            label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                        else
                            label2.Foreground = new SolidColorBrush(Colors.Gray);
                        panel.Children.Add(ellipse);
                        panel.Children.Add(label);
                        panel.Children.Add(label2);
                        panel.Children.Add(status);
                        ListFavourites.Items.Add(panel);
                    }
                }
            }
        }

        private void buttonEditUserData_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(textboxName.Text, "^[А-Яа-я]+$"))
            {
                MessageBox.Show("Можно использовать только русские буквы в имени.");
                return;
            }
            if (!Regex.IsMatch(textboxSecondName.Text, "^[А-Яа-я]+$"))
            {
                MessageBox.Show("Можно использовать только русские буквы в отчестве.");
                return;
            }
            if (!Regex.IsMatch(textboxSurname.Text, "^[А-Яа-я]+$"))
            {
                MessageBox.Show("Можно использовать только русские буквы в фамилии.");
                return;
            }
            if (textboxName.Text.Length < 1)
            {
                MessageBox.Show("Имя не может быть меньше 1 символа.");
                return;
            }
            if (textboxSecondName.Text.Length < 1)
            {
                MessageBox.Show("Отчество не может быть меньше 1 символа.");
                return;
            }
            if (textboxSurname.Text.Length < 1)
            {
                MessageBox.Show("Фамилия не может быть меньше 1 символа.");
                return;
            }
            for (int i = 0; i < textboxSurname.Text.Length; i++)
            {
                if (Regex.IsMatch(textboxSurname.Text[i].ToString(), @"[ -@\[-`\{-~\№]$"))
                {
                    MessageBox.Show("Фамилия может состоять только из букв.");
                    return;
                }
            }
            for (int i = 0; i < textboxName.Text.Length; i++)
            {
                if (Regex.IsMatch(textboxName.Text[i].ToString(), @"[ -@\[-`\{-~\№]$"))
                {
                    MessageBox.Show("Имя может состоять только из букв.");
                    return;
                }
            }
            for (int i = 0; i < textboxSecondName.Text.Length; i++)
            {
                if (Regex.IsMatch(textboxSecondName.Text[i].ToString(), @"[ -@\[-`\{-~\№]$"))
                {
                    MessageBox.Show("Отчество может состоять только из букв.");
                    return;
                }
            }
            if (Server.user.fullname != null && textboxName.Text == Server.user.fullname[1] && textboxSurname.Text == Server.user.fullname[0] && textboxSecondName.Text == Server.user.fullname[2])
            {
                MessageBox.Show("ФИО осталось прежним.");
                return;
            }
            TextInfo t = CultureInfo.CurrentCulture.TextInfo;
            Server.user.fullname = (t.ToTitleCase(textboxSurname.Text.ToLower()) + "," + t.ToTitleCase(textboxName.Text.ToLower()) + "," + t.ToTitleCase(textboxSecondName.Text.ToLower())).Split(',');
            MainWindow.server.SendServer($"Procedure,ChangeData,{Server.user.fullname[0] + " " + Server.user.fullname[1] + " " + Server.user.fullname[2]}", false);
            textboxSurname.Text = Server.user.fullname[0];
            textboxName.Text = Server.user.fullname[1];
            textboxSecondName.Text = Server.user.fullname[2];
            MessageBox.Show("ФИО изменено.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ButtonChangePass_Click(object sender, RoutedEventArgs e)
        {
            if (NewPass.Password.Length < 5)
            {
                MessageBox.Show("Пароль не может быть меньше 5 символов.");
                return;
            }
            for (int i = 0; i < NewPass.Password.Length; i++)
            {
                if (Regex.IsMatch(NewPass.Password[i].ToString(), @"[ -/\:-@\[-`\{-~\№]$"))
                {
                    MessageBox.Show("Пароль может состоять только из букв и цифр.");
                    return;
                }
            }
            if (NewPass.Password != NewPass2.Password)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }
            string result = MainWindow.server.SendServer($"Procedure,ChangePass,{NewPass.Password}", true);
            switch (result)
            {
                case "Пароль изменен.":
                    MessageBox.Show(result);
                    break;
                case "Новый пароль совпадает со старым.":
                    MessageBox.Show(result);
                    break;
            }
        }

        private void buttonEditImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 1024000)
                    MessageBox.Show("Размер файла слишком большой. Максимальный размер изображения составляет 1 мегабайт.");
                else
                {
                    byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                    ImgUser.Source = ByteArrayToImage(bytes);
                    Server.user.img = bytes;
                    MainWindow.server.SendServer($"Procedure,ChangeImg", false);
                    MainWindow.server.SendServer(bytes.Length, false);
                    MainWindow.server.SendServer(bytes, false);
                    LoadProfile();
                }
            }
        }

        public BitmapImage ByteArrayToImage(byte[] array) /*Преобразовывает массив байтов в изображение*/
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(array);
            image.EndInit();
            return image;
        }

        public byte[] ImageToByteArray(Image img) /*Преобразовывает изображение в массив байтов*/
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)img.Source));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
                return data;
            }
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PanelImgs.Items.Count > 9)
            {
                MessageBox.Show("Максимум 10 изображений.");
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 1024000)
                    MessageBox.Show("Размер файла слишком большой. Максимальный размер изображения составляет 1 мегабайт.");
                else
                {
                    byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                    Image image = new Image { Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute)) };
                    image.Stretch = Stretch.Uniform;
                    image.MaxHeight = 200;
                    image.MaxWidth = 200;
                    PanelImgs.Items.Add(image);
                }
            }
        }

        private void buttonAddNote_Click(object sender, RoutedEventArgs e)
        {
            Note note = new Note();
            note.imgs = new List<byte[]>();
            if (!String.IsNullOrEmpty(NoteBox.Text))
            {
                note.text = NoteBox.Text;
            }
            if (PanelImgs.Items.Count > 0)
            {
                foreach (Image item in PanelImgs.Items)
                {
                    note.imgs.Add(ImageToByteArray(item));
                }
            }
            if (!String.IsNullOrEmpty(NoteBox.Text) || PanelImgs.Items.Count > 0)
            {
                note.idAuthor = Server.user.id;
                var streamObj = new MemoryStream();
                Serializer.Serialize(streamObj, note);
                byte[] data = streamObj.ToArray();
                MainWindow.server.SendServer($"Procedure,AddNote", false);
                MainWindow.server.SendServer(data.Length, false);
                MainWindow.server.SendServer(data, false);
                NoteBox.Text = "";
                PanelImgs.Items.Clear();
                LoadProfile();
            }
        }

        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            PanelImgs.Items.Remove(PanelImgs.SelectedItem);
        }

        public void LoadProfile()
        {
            GridNotes.Children.Clear();
            MainWindow.server.SendServer($"Procedure,LoadProfile", true);
            SolidColorBrush solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222225"));
            Grid global = new Grid();
            int j = 0;
            foreach (Notes note in Server.user.notes)
            {
                borderNotes.CornerRadius = new CornerRadius(5, 5, 0, 0);
                global.RowDefinitions.Add(new RowDefinition());
                Grid grid = new Grid();
                grid.DataContext = note;
                Grid panelAuthor = new Grid();
                panelAuthor.HorizontalAlignment = HorizontalAlignment.Stretch;
                panelAuthor.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                panelAuthor.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                panelAuthor.RowDefinitions.Add(new RowDefinition());
                panelAuthor.RowDefinitions.Add(new RowDefinition());
                ImageBrush brush = new ImageBrush();
                brush.Stretch = Stretch.UniformToFill;
                if (Server.user.img != null)
                    brush.ImageSource = ByteArrayToImage(Server.user.img);
                else
                    brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = brush;
                ellipse.Height = 50;
                ellipse.Width = 50;
                ellipse.Margin = new Thickness(0, 0, 5, 0);
                TextBox fullname = new TextBox();
                TextBox date = new TextBox();
                fullname.Margin = new Thickness(5, 0, 0, 5);
                date.Margin = new Thickness(5, 5, 0, 0);
                date.Text = note.createDate.ToString("d MMMM yyyy");
                date.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#727272"));
                date.FontSize = 12;
                if (Server.user.fullname != null)
                    fullname.Text = Server.user.fullname[1] + ' ' + Server.user.fullname[0];
                else
                    fullname.Text = Server.user.login;
                fullname.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5d5dff"));
                fullname.IsReadOnly = true;
                fullname.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                date.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                date.IsReadOnly = true;
                Grid.SetColumn(ellipse, 0);
                Grid.SetRowSpan(ellipse, 2);
                Grid.SetColumn(fullname, 1);
                Grid.SetColumn(date, 1);
                Grid.SetRow(date, 1);
                ellipse.VerticalAlignment = VerticalAlignment.Center;
                ellipse.HorizontalAlignment = HorizontalAlignment.Left;
                fullname.VerticalAlignment = VerticalAlignment.Bottom;
                fullname.HorizontalAlignment = HorizontalAlignment.Left;
                date.VerticalAlignment = VerticalAlignment.Top;
                date.HorizontalAlignment = HorizontalAlignment.Left;

                Grid optionPanel = new Grid();
                optionPanel.VerticalAlignment = VerticalAlignment.Top;
                optionPanel.HorizontalAlignment = HorizontalAlignment.Right;
                optionPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                optionPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                optionPanel.Margin = new Thickness(10);
                grid.Children.Add(optionPanel);

                Border borderOptions = new Border();
                borderOptions.IsEnabled = false;
                optionPanel.DataContext = borderOptions;
                optionPanel.MouseLeave += OptionsHide;
                borderOptions.Effect = new DropShadowEffect { BlurRadius = 30, Opacity = 0.5, ShadowDepth = 0, Direction = 0};
                borderOptions.Opacity = 0;
                Grid gridOptions = new Grid();
                Label optionDelete = new Label();
                optionDelete.Content = "Удалить запись";
                optionDelete.Cursor = Cursors.Hand;
                optionDelete.DataContext = note;
                optionDelete.FontSize = 16;
                optionDelete.HorizontalAlignment = HorizontalAlignment.Stretch;
                optionDelete.FontWeight = FontWeights.Light;
                optionDelete.MouseLeftButtonUp += LabelOptionDeleteClick;
                gridOptions.Margin = new Thickness(0,5,0,5);
                gridOptions.Children.Add(optionDelete);
                optionDelete.MouseEnter += LabelOptionEnter;
                optionDelete.MouseLeave += LabelOptionLeave;
                borderOptions.Child = gridOptions;
                borderOptions.CornerRadius = new CornerRadius(5);
                borderOptions.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#222225"));
                borderOptions.HorizontalAlignment = HorizontalAlignment.Right;
                borderOptions.VerticalAlignment = VerticalAlignment.Top;
                Grid.SetRow(borderOptions, 1);
                optionPanel.Children.Add(borderOptions);

                Grid grid1 = new Grid();
                grid1.VerticalAlignment = VerticalAlignment.Top;
                grid1.HorizontalAlignment = HorizontalAlignment.Right;
                grid1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                Image options = new Image();
                options.Source = ByteArrayToImage(BusinessByJob.Properties.Resources.Options);
                options.Stretch = Stretch.UniformToFill;
                options.Cursor = Cursors.Hand;
                options.MouseEnter += OptionsShow;
                options.Width = 20;
                options.Margin = new Thickness(10,10,10,15);
                options.DataContext = borderOptions;
                grid1.Children.Add(options);
                Grid.SetRow(grid1, 0);
                optionPanel.Children.Add(grid1);

                panelAuthor.Children.Add(ellipse);
                panelAuthor.Children.Add(fullname);
                panelAuthor.Children.Add(date);
                panelAuthor.Margin = new Thickness(10, 10, 10, 5);
                if (!String.IsNullOrEmpty(note.text))
                {
                    TextBox text = new TextBox();
                    text.Foreground = new SolidColorBrush(Colors.White);
                    text.Text = note.text;
                    text.IsReadOnly = true;
                    text.VerticalContentAlignment = VerticalAlignment.Top;
                    text.HorizontalContentAlignment = HorizontalAlignment.Left;
                    text.TextWrapping = TextWrapping.Wrap;
                    text.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                    text.VerticalAlignment = VerticalAlignment.Top;
                    text.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetRow(text, 1);
                    text.Margin = new Thickness(10, 5, 10, 5);
                    grid.Children.Add(text);
                }
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(panelAuthor, 0);
                grid.Children.Add(panelAuthor);
                Grid imgs = new Grid();
                imgs.Margin = new Thickness(0, 0, 5, 0);
                int i = 0;
                foreach (var noteImg in note.imgs)
                {
                    imgs.ColumnDefinitions.Add(new ColumnDefinition());
                    Image image = new Image();
                    image.Source = ByteArrayToImage(noteImg);
                    image.Stretch = Stretch.UniformToFill;
                    image.Margin = new Thickness(0, 10, 5, 10);
                    image.Width = 300;
                    Grid.SetColumn(image, i);
                    imgs.Children.Add(image);
                    i++;
                }
                ScrollViewer scrollViewer = new ScrollViewer();
                scrollViewer.Content = imgs;
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.Margin = new Thickness(10, 0, 10, 0);
                Grid.SetRow(scrollViewer, 2);
                grid.Children.Add(scrollViewer);
                grid.VerticalAlignment = VerticalAlignment.Top;
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;

                Grid gridAction = new Grid();
                gridAction.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridAction.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                Grid gridLike = new Grid();
                gridLike.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridLike.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                Image like = new Image();
                if (note.like == true)
                    like.Source = ByteArrayToImage(BusinessByJob.Properties.Resources.Like);
                else
                    like.Source = ByteArrayToImage(BusinessByJob.Properties.Resources.Like2);
                like.Stretch = Stretch.Uniform;
                like.VerticalAlignment = VerticalAlignment.Center;
                like.HorizontalAlignment = HorizontalAlignment.Left;
                like.Height = 20;
                like.Width = 20;
                like.Margin = new Thickness(10,5,5,5);
                Grid.SetColumn(gridLike, 0);
                gridLike.Children.Add(like);
                TextBox boxLike = new TextBox();
                boxLike.IsReadOnly = true;
                boxLike.FontSize = 16;
                boxLike.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                boxLike.Margin = new Thickness(0,5,10,5);
                boxLike.HorizontalAlignment = HorizontalAlignment.Left;
                boxLike.VerticalAlignment = VerticalAlignment.Center;
                boxLike.Text = note.likes.ToString();
                boxLike.Foreground = new SolidColorBrush(Colors.White);
                Grid.SetColumn(boxLike, 1);
                gridLike.Children.Add(boxLike);
                Grid.SetColumn(gridLike, 0);
                gridAction.Children.Add(gridLike);
                Border borderAction = new Border();
                borderAction.Child = gridAction;
                borderAction.CornerRadius = new CornerRadius(10);
                borderAction.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30282f"));
                borderAction.Margin = new Thickness(10,5,5,10);
                borderAction.HorizontalAlignment = HorizontalAlignment.Left;
                borderAction.VerticalAlignment = VerticalAlignment.Center;
                borderAction.Cursor = Cursors.Hand;
                borderAction.MouseLeftButtonDown += BorderAction_MouseLeftButtonUpAddLike;
                borderAction.Uid = j.ToString();
                Grid.SetColumn(borderAction, 0);
                Grid gridActions = new Grid();
                Grid.SetRow(gridActions, 3);
                gridActions.Margin = new Thickness(0,5,0,0);
                gridActions.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridActions.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridActions.Children.Add(borderAction);
                grid.Children.Add(gridActions);

                Border borderComm = new Border();
                borderComm.CornerRadius = new CornerRadius(10);
                borderComm.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30282f"));
                borderComm.Margin = new Thickness(5, 5, 5, 10);
                borderComm.HorizontalAlignment = HorizontalAlignment.Left;
                borderComm.VerticalAlignment = VerticalAlignment.Center;
                borderComm.Cursor = Cursors.Hand;
                borderComm.MouseLeftButtonDown += BorderAction_MouseLeftButtonUpComments;
                borderComm.Uid = j.ToString();
                Grid gridComment = new Grid();
                borderComm.Child = gridComment;
                gridComment.ColumnDefinitions.Add(new ColumnDefinition());
                gridComment.ColumnDefinitions.Add(new ColumnDefinition());
                Image comm = new Image();
                comm.Source = ByteArrayToImage(BusinessByJob.Properties.Resources.Comment);
                comm.Margin = new Thickness(10, 5, 5, 5);
                comm.Stretch = Stretch.Uniform;
                comm.Height = 20;
                comm.Width = 20;
                comm.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(comm, 0);
                gridComment.Children.Add(comm);
                TextBox boxComm = new TextBox();
                boxComm.IsReadOnly = true;
                boxComm.FontSize = 16;
                boxComm.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                boxComm.Margin = new Thickness(0, 5, 10, 5);
                boxComm.HorizontalAlignment = HorizontalAlignment.Left;
                boxComm.VerticalAlignment = VerticalAlignment.Center;
                boxComm.Text = note.comments.Count.ToString();
                boxComm.Foreground = new SolidColorBrush(Colors.White);
                Grid.SetColumn(boxComm, 1);
                gridComment.Children.Add(boxComm);
                Grid.SetColumn(borderComm, 1);
                gridActions.Children.Add(borderComm);

                Border border = new Border();
                border.Background = solidColorBrush;
                if (j == 0)
                    border.CornerRadius = new CornerRadius(0,0,5,5);
                else
                    border.CornerRadius = new CornerRadius(5);
                border.Child = grid;
                border.Margin = new Thickness(0,0,0,10);
                Grid.SetRow(border, j);
                global.Children.Add(border);
                j++;
            }
            GridNotes.Children.Add(global);
        }

        private void BorderAction_MouseLeftButtonUpAddLike(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            if (Server.user.notes[int.Parse(border.Uid)].like == true)
                MainWindow.server.SendServer($"Procedure,RemoveLike,{Server.user.notes[int.Parse(border.Uid)].id}", false);
            else
                MainWindow.server.SendServer($"Procedure,AddLike,{Server.user.notes[int.Parse(border.Uid)].id}", false);
            LoadProfile();
        }
        private void BorderAction_MouseLeftButtonUpComments(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void OptionsShow(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            Border border = (Border)img.DataContext;
            border.Opacity = 100;
            border.IsEnabled = true;
        }

        private void OptionsHide(object sender, MouseEventArgs e)
        {
            Grid grid = (Grid)sender;
            Border border = (Border)grid.DataContext;
            border.Opacity = 0;
            border.IsEnabled = false;
        }

        private void LabelOptionEnter(object sender, RoutedEventArgs e)
        {
            Label label = (Label)sender;
            label.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#655d64"));
        }

        private void LabelOptionLeave(object sender, RoutedEventArgs e)
        {
            Label label = (Label)sender;
            label.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
        }

        private void LabelOptionDeleteClick(object sender, RoutedEventArgs e)
        {
            Label label = (Label)sender;
            Notes note = (Notes)label.DataContext;
            MainWindow.server.SendServer($"Procedure,DeleteNote,{note.id}", false);
            LoadProfile();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = (StackPanel)ListUsers.SelectedItem;
            MainWindow.server.SendServer($"Procedure,AddFavourites,{panel.DataContext}", false);
            LoadChat();
            LoadUsers();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            StackPanel panel = (StackPanel)ListFavourites.SelectedItem;
            MainWindow.server.SendServer($"Procedure,DeleteFavourites,{panel.DataContext}", false);
            LoadChat();
            LoadUsers();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            StackPanel panel = (StackPanel)ListFavourites.SelectedItem;
            Favourites user = (Favourites)panel.DataContext;
            foreach (Chat item in Server.user.chats)
            {
                if (item.idUser == user.id)
                {
                    return;
                }
            }
            MainWindow.server.SendServer($"Procedure,AddChat,{user.id}", false);
            LoadChatRooms();
        }

        private void Chats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListMessages.Items.Clear();
            ChatWish.Visibility = Visibility.Hidden;
            Border1.Visibility = Visibility.Visible;
            Border2.Visibility = Visibility.Visible;
            StackPanel panel = (StackPanel)Chats.SelectedItem;
            Chat chat1 = (Chat)panel.DataContext;
            if (panel.Children.Count > 2)
                panel.Children.RemoveAt(2);
            MainWindow.server.SendServer($"Procedure,LoadMessages,{chat1.id}", true);
            Chat chat = Server.user.chats.Find(x => x.id == chat1.id);
            foreach (Chat.ChatMessages item in chat.chatMessages)
            {
                Border border = new Border();
                border.CornerRadius = new CornerRadius(5);
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30282f"));
                border.Margin = new Thickness(10);
                border.VerticalAlignment = VerticalAlignment.Stretch;
                ImageBrush brush = new ImageBrush();
                brush.Stretch = Stretch.UniformToFill;
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = brush;
                ellipse.Height = 40;
                ellipse.Width = 40;
                ellipse.VerticalAlignment = VerticalAlignment.Top;
                ellipse.Margin = new Thickness(5);
                Grid.SetRowSpan(ellipse, 2);
                TextBox userInfo = new TextBox();
                TextBox message = new TextBox();
                message.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                userInfo.TextWrapping = TextWrapping.Wrap;
                userInfo.Margin = new Thickness(0, 5, 5, 0);
                userInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                userInfo.IsReadOnly = true;
                if (item.idAuthor == Server.user.id)
                {
                    if (Server.user.fullname != null)
                        userInfo.Text = Server.user.fullname[1];
                    else
                        userInfo.Text = Server.user.login;
                    if (Server.user.img != null)
                        brush.ImageSource = ByteArrayToImage(Server.user.img);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                }
                else
                {
                    if (chat.nameUser != null)
                        userInfo.Text = chat.nameUser[1];
                    else
                        userInfo.Text = chat.loginUser;
                    if (chat.imgUser != null)
                        brush.ImageSource = ByteArrayToImage(chat.imgUser);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                }
                userInfo.TextAlignment = TextAlignment.Left;
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                Grid.SetRow(message, 1);
                Grid.SetColumn(message, 1);
                message.VerticalContentAlignment = VerticalAlignment.Stretch;
                message.HorizontalContentAlignment = HorizontalAlignment.Left;
                message.TextWrapping = TextWrapping.Wrap;
                message.Margin = new Thickness(0, 0, 5, 5);
                message.IsReadOnly = true;
                message.Text = item.text;
                message.TextAlignment = TextAlignment.Left;
                userInfo.HorizontalAlignment = HorizontalAlignment.Left;
                userInfo.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(userInfo, 0);
                Grid.SetColumn(userInfo, 1);
                grid.Children.Add(ellipse);
                grid.Children.Add(message);
                grid.Children.Add(userInfo);
                grid.Margin = new Thickness(5);
                message.HorizontalAlignment = HorizontalAlignment.Left;
                message.VerticalAlignment = VerticalAlignment.Stretch;
                userInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7aa1ff"));
                border.Child = grid;
                ListMessages.Items.Add(border);
            }
            ScrolMessages.ScrollToEnd();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SendMessage();
        }

        public void SendMessage()
        {
            if (!String.IsNullOrEmpty(MessageText.Text) && !String.IsNullOrWhiteSpace(MessageText.Text))
            {
                StackPanel panel = (StackPanel)Chats.SelectedItem;
                Chat chat1 = (Chat)panel.DataContext;
                MainWindow.server.SendServer($"Procedure,AddMessage,{chat1.id},{MessageText.Text}", false);
                Border border = new Border();
                border.CornerRadius = new CornerRadius(5);
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30282f"));
                border.Margin = new Thickness(10);
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.VerticalAlignment = VerticalAlignment.Stretch;
                ImageBrush brush = new ImageBrush();
                brush.Stretch = Stretch.UniformToFill;
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = brush;
                ellipse.Height = 40;
                ellipse.Width = 40;
                ellipse.VerticalAlignment = VerticalAlignment.Top;
                ellipse.Margin = new Thickness(5);
                Grid.SetRowSpan(ellipse, 2);
                TextBox userInfo = new TextBox();
                userInfo.TextWrapping = TextWrapping.Wrap;
                userInfo.Margin = new Thickness(0, 5, 5, 0);
                userInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                userInfo.IsReadOnly = true;
                if (Server.user.fullname != null)
                    userInfo.Text = Server.user.fullname[1];
                else
                    userInfo.Text = Server.user.login;
                if (Server.user.img != null)
                    brush.ImageSource = ByteArrayToImage(Server.user.img);
                else
                    brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                userInfo.TextAlignment = TextAlignment.Left;
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                TextBox message = new TextBox();
                Grid.SetRow(message, 1);
                Grid.SetColumn(message, 1);
                message.VerticalContentAlignment = VerticalAlignment.Stretch;
                message.HorizontalContentAlignment = HorizontalAlignment.Left;
                message.TextWrapping = TextWrapping.Wrap;
                message.Margin = new Thickness(0, 0, 5, 5);
                message.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                message.IsReadOnly = true;
                message.Text = MessageText.Text;
                message.TextAlignment = TextAlignment.Left;
                userInfo.HorizontalAlignment = HorizontalAlignment.Left;
                userInfo.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(userInfo, 0);
                Grid.SetColumn(userInfo, 1);
                grid.Children.Add(ellipse);
                grid.Children.Add(message);
                grid.Children.Add(userInfo);
                grid.Margin = new Thickness(5);
                message.HorizontalAlignment = HorizontalAlignment.Left;
                message.VerticalAlignment = VerticalAlignment.Stretch;
                userInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7aa1ff"));
                border.Child = grid;
                ListMessages.Items.Add(border);
                Chat chat = Server.user.chats.Find(x => x.id == chat1.id);
                chat.chatMessages.Add(new Chat.ChatMessages { idAuthor = Server.user.id, text = MessageText.Text });
                MessageText.Text = "";
                ScrolMessages.ScrollToEnd();
            }
        }

        public void UpdateChatControl(object sender, ElapsedEventArgs arg)
        {
            NewMessages();
            NewChats();
        }

        public void UpdateProductControl(object sender, ElapsedEventArgs arg)
        {
            NewNomenclatureType();
            LoadOrders();
            Application.Current.Dispatcher.Invoke(() =>
            {
                int index = ProductsList.SelectedIndex;
                LoadProducts();
                ProductsList.SelectedIndex = index;
            });
        }

        public void NewMessages()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Chats.SelectedIndex != -1)
                {
                    StackPanel panel = (StackPanel)Chats.SelectedItem;
                    Chat chat1 = (Chat)panel.DataContext;
                    Chat chat = Server.user.chats.Find(x => x.id == chat1.id);
                    if (ListMessages.Items.Count < chat.chatMessages.Count)
                    {
                        for (int i = ListMessages.Items.Count; i < chat.chatMessages.Count; i++)
                        {
                            Border border = new Border();
                            border.CornerRadius = new CornerRadius(5);
                            border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30282f"));
                            border.Margin = new Thickness(10);
                            border.HorizontalAlignment = HorizontalAlignment.Stretch;
                            border.VerticalAlignment = VerticalAlignment.Stretch;
                            ImageBrush brush = new ImageBrush();
                            brush.Stretch = Stretch.UniformToFill;
                            Ellipse ellipse = new Ellipse();
                            ellipse.Fill = brush;
                            ellipse.Height = 40;
                            ellipse.Width = 40;
                            ellipse.VerticalAlignment = VerticalAlignment.Top;
                            ellipse.Margin = new Thickness(5);
                            Grid.SetRowSpan(ellipse, 2);
                            TextBox userInfo = new TextBox();
                            userInfo.TextWrapping = TextWrapping.Wrap;
                            userInfo.Margin = new Thickness(0, 5, 5, 0);
                            userInfo.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                            userInfo.IsReadOnly = true;
                            if (chat.nameUser != null)
                                userInfo.Text = chat.nameUser[1];
                            else
                                userInfo.Text = chat.loginUser;
                            if (chat.imgUser != null)
                                brush.ImageSource = ByteArrayToImage(chat.imgUser);
                            else
                                brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                            userInfo.TextAlignment = TextAlignment.Left;
                            Grid grid = new Grid();
                            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            grid.HorizontalAlignment = HorizontalAlignment.Left;
                            grid.VerticalAlignment = VerticalAlignment.Stretch;
                            TextBox message = new TextBox();
                            Grid.SetRow(message, 1);
                            Grid.SetColumn(message, 1);
                            message.VerticalContentAlignment = VerticalAlignment.Stretch;
                            message.HorizontalContentAlignment = HorizontalAlignment.Left;
                            message.TextWrapping = TextWrapping.Wrap;
                            message.Margin = new Thickness(0, 0, 5, 5);
                            message.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000"));
                            message.IsReadOnly = true;
                            message.Text = chat.chatMessages[i].text;
                            message.TextAlignment = TextAlignment.Left;
                            userInfo.HorizontalAlignment = HorizontalAlignment.Left;
                            userInfo.VerticalAlignment = VerticalAlignment.Center;
                            Grid.SetRow(userInfo, 0);
                            Grid.SetColumn(userInfo, 1);
                            grid.Children.Add(ellipse);
                            grid.Children.Add(message);
                            grid.Children.Add(userInfo);
                            grid.Margin = new Thickness(5);
                            message.HorizontalAlignment = HorizontalAlignment.Left;
                            message.VerticalAlignment = VerticalAlignment.Stretch;
                            userInfo.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7aa1ff"));
                            border.Child = grid;
                            ListMessages.Items.Add(border);
                        }
                        ScrolMessages.ScrollToEnd();
                    }
                }
            });
        }

        public void NewChats()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (int i = Chats.Items.Count; i < Server.user.chats.Count; i++)
                {
                    StackPanel panel = new StackPanel();
                    panel.Margin = new Thickness(5);
                    panel.DataContext = Server.user.chats[i];
                    panel.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    if (Server.user.chats[i].nameUser != null)
                        name.Content = Server.user.chats[i].nameUser[1] + " " + Server.user.chats[i].nameUser[0];
                    else
                        name.Content = Server.user.chats[i].loginUser;
                    name.FontSize = 18;
                    ImageBrush brush = new ImageBrush();
                    brush.Stretch = Stretch.UniformToFill;
                    if (Server.user.chats[i].imgUser != null)
                        brush.ImageSource = ByteArrayToImage(Server.user.chats[i].imgUser);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = brush;
                    ellipse.Height = 40;
                    ellipse.Width = 40;
                    ellipse.Margin = new Thickness(0, 0, 5, 0);
                    panel.Children.Add(ellipse);
                    panel.Children.Add(name);
                    Chats.Items.Insert(0, panel);
                }
            });
        }

        public void NewNomenclatureType()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (int i = NomenclatureTypes.Items.Count; i < Server.products.types.Count; i++)
                {
                    ListViewItem listViewItem = new ListViewItem { Content = Server.products.types[i] };
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Удалить";
                    menuItem.Click += MenuItem_RemoveType;
                    menuItem.DataContext = Server.products.types[i];
                    ContextMenu cm = new ContextMenu();
                    cm.Items.Add(menuItem);
                    listViewItem.ContextMenu = cm;
                    NomenclatureTypes.Items.Add(listViewItem);
                    ListTypes.Items.Add(Server.products.types[i].name);
                }
            });
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            StackPanel panel = (StackPanel)ListUsers.SelectedItem;
            int id = int.Parse((string)panel.DataContext);
            foreach (Chat item in Server.user.chats)
            {
                if (item.idUser == id)
                {
                    return;
                }
            }
            MainWindow.server.SendServer($"Procedure,AddChat,{id}", false);
            LoadChatRooms();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListUsers.Items.Clear();
            foreach (Users.User item in Server.users.users)
            {
                if (item.login.Contains(FindUsers.Text))
                {
                    bool check = false;
                    foreach (Favourites item2 in Server.user.favourites)
                    {
                        if (item.id == item2.id)
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check == true)
                    {
                        continue;
                    }
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.DataContext = item.id.ToString();
                    ImageBrush brush = new ImageBrush();
                    stackPanel.Orientation = Orientation.Horizontal;
                    brush.Stretch = Stretch.UniformToFill;
                    if (item.img != null)
                        brush.ImageSource = ByteArrayToImage(item.img);
                    else
                        brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                    Ellipse ellipse = new Ellipse();
                    ellipse.Fill = brush;
                    ellipse.Height = 50;
                    ellipse.Width = 50;
                    ellipse.Margin = new Thickness(5);
                    Label label = new Label();
                    Label label2 = new Label();
                    label2.Content = item.persona;
                    if (item.persona != "Пользователь")
                    {
                        label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                    }
                    else
                        label2.Foreground = new SolidColorBrush(Colors.Gray);
                    if (item.fullname != null)
                        label.Content = item.fullname[1] + " " + item.fullname[0];
                    else
                        label.Content = item.login;
                    stackPanel.Children.Add(ellipse);
                    stackPanel.Children.Add(label);
                    stackPanel.Children.Add(label2);
                    ListUsers.Items.Add(stackPanel);
                }
            }
        }

        private void FindFavourites_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListFavourites.Items.Clear();
            foreach (Favourites item in Server.user.favourites)
            {
                if (item.login.Contains(FindFavourites.Text))
                {
                    if (comboBoxFilter.SelectedIndex == 0)
                    {
                        StackPanel panel = new StackPanel();
                        panel.DataContext = item;
                        ImageBrush brush = new ImageBrush();
                        panel.Orientation = Orientation.Horizontal;
                        brush.Stretch = Stretch.UniformToFill;
                        if (item.img != null)
                            brush.ImageSource = ByteArrayToImage(item.img);
                        else
                            brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = brush;
                        ellipse.Height = 50;
                        ellipse.Width = 50;
                        ellipse.Margin = new Thickness(5);
                        Label status = new Label();
                        if (item.status == true)
                        {
                            status.Content = "Онлайн";
                            status.Foreground = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            status.Content = "Оффлайн";
                            status.Foreground = new SolidColorBrush(Colors.Gray);
                        }
                        Label label = new Label();
                        if (item.fullname != null)
                            label.Content = item.fullname[1] + " " + item.fullname[0];
                        else
                            label.Content = item.login;
                        Label label2 = new Label();
                        label2.Content = item.persona;
                        if (item.persona != "Пользователь")
                            label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                        else
                            label2.Foreground = new SolidColorBrush(Colors.Gray);
                        panel.Children.Add(ellipse);
                        panel.Children.Add(label);
                        panel.Children.Add(label2);
                        panel.Children.Add(status);
                        ListFavourites.Items.Add(panel);
                    }
                    else if (item.status == true)
                    {
                        StackPanel panel = new StackPanel();
                        panel.DataContext = item.id.ToString();
                        ImageBrush brush = new ImageBrush();
                        panel.Orientation = Orientation.Horizontal;
                        brush.Stretch = Stretch.UniformToFill;
                        if (item.img != null)
                            brush.ImageSource = ByteArrayToImage(item.img);
                        else
                            brush.ImageSource = ByteArrayToImage(BusinessByJob.Properties.Resources.default_user_icon);
                        Ellipse ellipse = new Ellipse();
                        ellipse.Fill = brush;
                        ellipse.Height = 50;
                        ellipse.Width = 50;
                        ellipse.Margin = new Thickness(5);
                        Label status = new Label();
                        status.Content = "Онлайн";
                        status.Foreground = new SolidColorBrush(Colors.Green);
                        Label label = new Label();
                        if (item.fullname != null)
                            label.Content = item.fullname[1] + " " + item.fullname[0];
                        else
                            label.Content = item.login;
                        Label label2 = new Label();
                        label2.Content = item.persona;
                        if (item.persona != "Пользователь")
                            label2.Foreground = new SolidColorBrush(Colors.BlueViolet);
                        else
                            label2.Foreground = new SolidColorBrush(Colors.Gray);
                        panel.Children.Add(ellipse);
                        panel.Children.Add(label);
                        panel.Children.Add(label2);
                        panel.Children.Add(status);
                        ListFavourites.Items.Add(panel);
                    }
                }
            }
        }

        /*Номенклатура*/
        public void LoadNomenclatureTypes()
        {
            NomenclatureTypes.Items.Clear();
            ListTypes.Items.Clear();
            MainWindow.server.SendServer($"Procedure,LoadNomenclatureTypes", true);
            foreach (var item in Server.products.types)
            {
                ListViewItem listViewItem = new ListViewItem { Content = item };
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Удалить";
                menuItem.Click += MenuItem_RemoveType;
                menuItem.DataContext = item;
                ContextMenu cm = new ContextMenu();
                cm.Items.Add(menuItem);
                listViewItem.ContextMenu = cm;
                NomenclatureTypes.Items.Add(listViewItem);
                ListTypes.Items.Add(item.name);
            }
        }

        private void MenuItem_RemoveType(object sender, EventArgs e)
        {
            ProductsAvailable.NomenclatureType type = (ProductsAvailable.NomenclatureType)((MenuItem)sender).DataContext;
            MainWindow.server.SendServer($"Procedure,RemoveNomenclatureTypes,{type.id}", false);
            NomenclatureTypes.Items.RemoveAt(NomenclatureTypes.SelectedIndex);
            Server.products.types.Remove(type);
            LoadNomenclatureTypes();
        }

        private void MenuItem_RemovePermission(object sender, EventArgs e)
        {
            string permissionName = (string)((MenuItem)sender).Tag;
            MainWindow.server.SendServer($"Procedure,RemovePermission,{permissionName}", false);
            PermissionList.Items.RemoveAt(PermissionList.SelectedIndex);
            Server.permissions.Remove(permissionName);
            LoadUsersForAdmin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TypeName.Text) && !Regex.IsMatch(TypeName.Text, @"[ -@\[-`\{-~\№]$"))
            {
                MainWindow.server.SendServer($"Procedure,AddNomenclatureTypes,{TypeName.Text}", false);
                TypeName.Text = string.Empty;
                LoadNomenclatureTypes();
            }
        }

        public void LoadNomenclature()
        {
            Nomenclature.Items.Clear();
            ListNomenclature.Items.Clear();
            MainWindow.server.SendServer($"Procedure,LoadNomenclature", true);
            foreach (var item in Server.products.nomenclatures)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Удалить";
                menuItem.Click += MenuItem_RemoveNomenclature;
                menuItem.DataContext = item;
                ContextMenu cm = new ContextMenu();
                cm.Items.Add(menuItem);
                ListViewItem listViewItem = new ListViewItem { Content = item };
                listViewItem.ContextMenu = cm;
                Nomenclature.Items.Add(listViewItem);
                ListNomenclature.Items.Add(item.name);
            }
        }

        private void MenuItem_RemoveNomenclature(object sender, EventArgs e)
        {
            ProductsAvailable.Nomenclature nomenclature = (ProductsAvailable.Nomenclature)((MenuItem)sender).DataContext;
            MainWindow.server.SendServer($"Procedure,RemoveNomenclature,{nomenclature.id}", false);
            Nomenclature.Items.RemoveAt(Nomenclature.SelectedIndex);
            LoadNomenclature();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameNomenclature.Text) && !Regex.IsMatch(NameNomenclature.Text, @"[ -@\[-`\{-~\№]$") && ListTypes.SelectedIndex != -1 && !string.IsNullOrWhiteSpace(articulName.Text) && !string.IsNullOrWhiteSpace(priceNomenclature.Text))
            {
                MainWindow.server.SendServer($"Procedure,AddNomenclature,{NameNomenclature.Text},{ListTypes.Text},{articulName.Text},{priceNomenclature.Text}", false);
                MainWindow.server.SendServer(double.Parse(priceNomenclature.Text), false);
                if (ImageNomenclature.Source != null)
                {
                    MainWindow.server.SendServer($"ImgYes", false);
                    byte[] imgData = ImageToByteArray(ImageNomenclature);
                    MainWindow.server.SendServer(imgData.Length, false);
                    MainWindow.server.SendServer(imgData, false);
                }
                else
                {
                    MainWindow.server.SendServer($"ImgNo", false);
                }
                NameNomenclature.Text = string.Empty;
                articulName.Text = string.Empty;
                priceNomenclature.Text = string.Empty;
                ImageNomenclature.Source = null;
                LoadNomenclature();
            }
        }

        public void LoadProducts()//Загрузка продукции
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ProductsList.Items.Clear();
                MainWindow.server.SendServer($"Procedure,LoadProducts", true);
                foreach (var item in Server.products.products)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Удалить";
                    menuItem.Click += MenuItem_RemoveProduct;
                    menuItem.DataContext = item;
                    ContextMenu cm = new ContextMenu();
                    cm.Items.Add(menuItem);
                    ListViewItem listViewItem = new ListViewItem { Content = item };
                    listViewItem.ContextMenu = cm;
                    ProductsList.Items.Add(listViewItem);
                }
            });
        }

        private void priceNomenclature_PreviewTextInput(object sender, TextCompositionEventArgs e)//Проверка ввода цены продукции
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        public bool IsTextAllowed(string text)
        {
            return !Regex.IsMatch(text, "[^0-9,]");//Проверка на нужный регистр
        }

        private void countProduct_PreviewTextInput(object sender, TextCompositionEventArgs e)//Проверка ввода количества продукции
        {
            e.Handled = !IsTextAllowedProduct(e.Text);
        }

        public bool IsTextAllowedProduct(string text)//Проверка на нужный регистр
        {
            return !Regex.IsMatch(text, "[^0-9-]");
        }

        private void MenuItem_RemoveProduct(object sender, EventArgs e)//Удаление продукции
        {
            ProductsAvailable.Product product = (ProductsAvailable.Product)((MenuItem)sender).DataContext;
            MainWindow.server.SendServer($"Procedure,RemoveProduct,{product.id}", false);
            ProductsList.Items.RemoveAt(ProductsList.SelectedIndex);
        }

        private void AddProduct(object sender, RoutedEventArgs e)//Добавление продукции
        {
            if (ListNomenclature.SelectedIndex != -1 && !string.IsNullOrWhiteSpace(ProductCount.Text))
            {
                foreach (var item in Server.products.products)
                {
                    if (ListNomenclature.Text == item.name)
                    {
                        item.count += int.Parse(ProductCount.Text);
                        MainWindow.server.SendServer($"Procedure,UpdateProduct,{item.id},{item.count}", false);
                        ProductCount.Text = string.Empty;
                        LoadProducts();
                        return;
                    }
                }
                MainWindow.server.SendServer($"Procedure,AddProduct,{ListNomenclature.Text},{ProductCount.Text}", false);
                ProductCount.Text = string.Empty;
                LoadProducts();
            }
        }

        public void LoadOrders()//Загрузка заказов
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OrderList.Items.Clear();
                MainWindow.server.SendServer($"Procedure,LoadOrders", true);
                foreach (var item in Server.orders)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Отменить";
                    menuItem.Click += MenuItem_CancelOrder;
                    menuItem.Tag = item;
                    ContextMenu cm = new ContextMenu();
                    cm.Items.Add(menuItem);

                    MenuItem menuItem2 = new MenuItem();
                    menuItem2.Header = "Следующий этап";
                    menuItem2.Click += MenuItem_NextStep;
                    menuItem2.Tag = item;
                    cm.Items.Add(menuItem2);

                    ListViewItem listViewItem = new ListViewItem { Content = item, ContextMenu = cm };
                    listViewItem.Tag = item;
                    OrderList.Items.Add(listViewItem);
                }
            });
        }

        public void MenuItem_CancelOrder(object sender, EventArgs e)
        {
            Order orderSelected = (Order)((MenuItem)sender).Tag;
            if (orderSelected.status == "Отменён" || orderSelected.status == "Завершён")
                return;
            orderSelected.status = "Отменён";
            MainWindow.server.SendServer($"Procedure,ChangeOrderStatus,{orderSelected.id},{orderSelected.status}", false);
            LoadOrders();
        }

        public void MenuItem_NextStep(object sender, EventArgs e)
        {
            Order orderSelected = (Order)((MenuItem)sender).Tag;
            if (orderSelected.status == "Отменён")
                return;
            if (orderSelected.status == "Ожидание")
                orderSelected.status = "Готовится";
            else if (orderSelected.status == "Готовится")
                orderSelected.status = "Готов";
            else if (orderSelected.status == "Готов")
                orderSelected.status = "Завершён";
            MainWindow.server.SendServer($"Procedure,ChangeOrderStatus,{orderSelected.id},{orderSelected.status}", false);
            LoadOrders();
        }

        public void LoadUsersForAdmin()
        {
            UsersList.Items.Clear();
            foreach (var item in Server.users.users)
            {
                if (item.login != Server.user.login)
                {
                    ListViewItem listViewItem = new ListViewItem { Content = item };
                    listViewItem.Tag = item;
                    UsersList.Items.Add(listViewItem);
                }
            }

            PersonaList.Items.Clear();
            PermissionList.Items.Clear();
            ComboListPermissions.Items.Clear();
            ComboListPersons.Items.Clear();
            MainWindow.server.SendServer($"Procedure,LoadInfoForAdmin", true);
            foreach (var item in Server.permissions)
            {
                if (item != "User" && item != "Admin")
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Удалить";
                    menuItem.Click += MenuItem_RemovePermission;
                    menuItem.Tag = item;
                    ContextMenu cm = new ContextMenu();
                    cm.Items.Add(menuItem);
                    PermissionList.Items.Add(new ListViewItem { Content = item, ContextMenu = cm });
                }
                ComboListPermissions.Items.Add(item);
            }
            foreach (var item in Server.persons)
            {
                if (item != "Пользователь" && item != "Администратор")
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Удалить";
                    menuItem.Click += MenuItem_RemovePerson;
                    menuItem.Tag = item;
                    ContextMenu cm = new ContextMenu();
                    cm.Items.Add(menuItem);
                    PersonaList.Items.Add(new ListViewItem { Content = item, ContextMenu = cm });
                }
                ComboListPersons.Items.Add(item);
            }
        }

        private void MenuItem_RemovePerson(object sender, EventArgs e)
        {
            string personName = (string)((MenuItem)sender).Tag;
            MainWindow.server.SendServer($"Procedure,RemovePerson,{personName}", false);
            PersonaList.Items.RemoveAt(PersonaList.SelectedIndex);
            Server.permissions.Remove(personName);
            LoadUsersForAdmin();
        }

        private void AddOrder(object sender, RoutedEventArgs e)//Добавление заказа
        {
            if (Server.user.fullname == null)
            {
                MessageBox.Show("Заполните ФИО на вкладке профиля для возможности совершать заказы!");
                return;
            }
            order = new Order();
            foreach (var item in Server.products.products)
            {
                if (item.slider.Value > 0)
                {
                    Order.Product product = new Order.Product { id = item.id, name = item.name, count = (int)item.slider.Value, type = item.type, articul = item.articul, price = item.price };
                    product.buyCount = (int)item.slider.Maximum - (int)item.slider.Value;
                    order.products.Add(product);
                    order.totalPrice += product.price * product.count;
                    item.slider.Value = 0;
                }
            }
            if (order.products.Count > 0)
            {
                Payment payment = new Payment();
                payment.SetPrice(order.totalPrice);
                payment.ShowDialog();
                if (payment.DialogResult == true)
                {
                    order.fullname = Server.user.fullname;
                    var streamObj = new MemoryStream();
                    Serializer.Serialize(streamObj, order);
                    byte[] data = streamObj.ToArray();
                    MainWindow.server.SendServer($"Procedure,CreateOrder", false);
                    MainWindow.server.SendServer(data.Length, false);
                    MainWindow.server.SendServer(data, false);
                    LoadAvailableProducts();
                    LoadOrders();
                    LoadProducts();
                }
            }
            LoadMyOrderList();
        }

        public void LoadAvailableProducts()//Загрузка доступной продукции
        {
            ChooseProducts.Children.Clear();
            MainWindow.server.SendServer($"Procedure,LoadProducts", true);
            int i = 0;
            Grid booderGrid = new Grid();
            foreach (var item in Server.products.products)
            {
                if (item.count > 0)
                {
                    item.slider = new Slider();
                    item.slider.Value = 0;
                    item.slider.Minimum = 0;
                    item.slider.Maximum = item.count;
                    item.slider.AutoToolTipPlacement = AutoToolTipPlacement.TopLeft;
                    item.slider.AutoToolTipPrecision = 0;
                    item.slider.TickPlacement = TickPlacement.BottomRight;
                    item.slider.ValueChanged += ChangeCount;
                    item.slider.IsSnapToTickEnabled = true;
                    StackPanel panel = new StackPanel();
                    TextBlock textBlock = new TextBlock { Text = item.name + " - " + item.type + " - " + item.Price };
                    TextBlock selectCount = new TextBlock { Text = item.slider.Value.ToString() };
                    item.slider.DataContext = selectCount;
                    item.slider.Tag = item.price;
                    panel.Children.Add(textBlock);
                    panel.Children.Add(item.slider);
                    panel.Children.Add(selectCount);
                    if (item.img != null)
                    {
                        Image img = new Image();
                        img.Source = ByteArrayToImage(item.img);
                        img.Stretch = Stretch.Uniform;
                        img.Height = 100;
                        img.Width = 200;
                        panel.Children.Add(img);
                    }
                    panel.Margin = new Thickness(10);
                    Border border = new Border();
                    border.Background = new SolidColorBrush(Colors.Gray);
                    border.CornerRadius = new CornerRadius(10);
                    border.Child = panel;
                    border.Margin = new Thickness(5);
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    booderGrid.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(border, i);
                    booderGrid.Children.Add(border);
                    i++;
                }
            }
            ChooseProducts.Children.Add(booderGrid);
        }

        public void ChangeCount(object sender, EventArgs e)
        {
            double price = 0;
            TextBlock textBlock = (TextBlock)((Slider)sender).DataContext;
            textBlock.Text = ((Slider)sender).Value.ToString();
            foreach(var item in Server.products.products)
            {
                price += item.slider.Value * item.price;
            }
            TotalPrice.Content = "Стоимость: " + string.Format("{0:0.00}", price) + " руб.";
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://bikers-pizza.ru/") { UseShellExecute = true });
        }

        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.google.com/maps/place/Bikers+Pizza/@58.0100287,56.239347,17.25z/data=!4m6!3m5!1s0x43e8c72143d3e611:0x1e0898878b0164c!8m2!3d58.0101072!4d56.2405059!16s%2Fg%2F11h_g14mm5?hl=RU&entry=ttu") { UseShellExecute = true });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(namePermission.Text))
            {
                MainWindow.server.SendServer($"Procedure,AddPermission,{namePermission.Text}", false);
                namePermission.Text = string.Empty;
                LoadUsersForAdmin();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(namePerson.Text))
            {
                MainWindow.server.SendServer($"Procedure,AddPerson,{namePerson.Text}", false);
                namePerson.Text = string.Empty;
                LoadUsersForAdmin();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UserNameForPermission.Text) && ComboListPermissions.SelectedIndex != -1)
            {
                foreach (var item in Server.users.users)
                {
                    if (UserNameForPermission.Text == item.login && item.permission != ComboListPermissions.Text)
                    {
                        MainWindow.server.SendServer($"Procedure,ChangePermission,{item.login},{ComboListPermissions.Text}", false);
                        item.permission = ComboListPermissions.Text;
                        UserNameForPermission.Text = string.Empty;
                        ComboListPermissions.SelectedIndex = -1;
                        LoadUsersForAdmin();
                        return;
                    }
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UserNameForPerson.Text) && ComboListPersons.SelectedIndex != -1)
            {
                foreach (var item in Server.users.users)
                {
                    if (UserNameForPerson.Text == item.login && item.persona != ComboListPersons.Text)
                    {
                        MainWindow.server.SendServer($"Procedure,ChangePerson,{item.login},{ComboListPersons.Text}", false);
                        item.persona = ComboListPersons.Text;
                        UserNameForPerson.Text = string.Empty;
                        ComboListPersons.SelectedIndex = -1;
                        LoadUsersForAdmin();
                        return;
                    }
                }
            }
        }

        private void AddImageForNomenclature(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 1024000)
                    MessageBox.Show("Размер файла слишком большой. Максимальный размер изображения составляет 1 мегабайт.");
                else
                {
                    byte[] bytes = File.ReadAllBytes(openFileDialog.FileName);
                    ImageNomenclature.Width = 200;
                    ImageNomenclature.Height = 200;
                    ImageNomenclature.Source = ByteArrayToImage(bytes);
                }
            }
        }

        public void ShowNomenclatureImg(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            ShowImg showImg = new ShowImg();
            showImg.Owner = this;
            showImg.AddImgShow(img, img.PointToScreen(new Point(0, 0)));
            showImg.Show();
        }

        public void LoadMyOrderList()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MyOrderList.Items.Clear();
                MainWindow.server.SendServer($"Procedure,LoadMyOrders", true);
                foreach (var item in Server.myOrders)
                {
                    ListViewItem listViewItem = new ListViewItem { Content = item };
                    MyOrderList.Items.Add(listViewItem);
                }
            });
        }
    }
}
