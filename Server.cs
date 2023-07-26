using BusinessByJob;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Timer = System.Timers.Timer;

namespace NewApp
{
    public class Server
    {
        TcpClient client = new TcpClient();
        Timer timer = new Timer();
        public static UserInfo user = new UserInfo();
        public static Users users = new Users();
        public static ProductsAvailable products = new ProductsAvailable();
        public static List<Order> orders = new List<Order>();
        public static List<string> permissions = new List<string>();
        public static List<string> persons = new List<string>();
        public static List<Order> myOrders = new List<Order>();
        public string check = "";
        byte[] checkConnection = new byte[] { 0 };
        NetworkStream? stream;
        BinaryReader? binaryReader;
        BinaryWriter? binaryWriter;
        public EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public TcpClient GetClient
        {
            get { return client; }
        }

        public async void OpenConnection()
        {
            try
            {
                string[] address = BusinessByJob.Properties.Resources.ServerAddress.Split(',');
                await client.ConnectAsync(address[0], int.Parse(address[1]));
                handle.Set();
                new Thread(async () => await ProcessServerAsync()).Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось установить подключение.\nОшибка: " + ex.Message);
                Environment.Exit(0);
            }
        }

        Task ProcessServerAsync()
        {
            stream = client.GetStream();
            binaryReader = new BinaryReader(stream);
            binaryWriter = new BinaryWriter(stream);
            timer = new Timer(3000);
            timer.Elapsed += CheckServerConnection;
            timer.AutoReset = true;
            timer.Enabled = true;
            while (true)
            {
                binaryWriter.Flush();
                try
                {
                    var request = binaryReader.ReadString();
                    string[] dataString = request.Split(',');
                    if (dataString[0] == "Procedure")
                    {
                        if (dataString[1] == "CheckUser")
                        {
                            if (dataString[2] == "Success")
                            {
                                var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                                user = Serializer.Deserialize<UserInfo>(stream);
                                check = "Успешная авторизация.";
                                handle.Set();
                                continue;
                            }
                            if (dataString[2] == "ErrorLogin")
                            {
                                check = "Неправильное имя пользователя.";
                                handle.Set();
                                continue;
                            }
                            if (dataString[2] == "ErrorPassword")
                            {
                                check = "Неправильный пароль.";
                                handle.Set();
                                continue;
                            }
                        }
                        if (dataString[1] == "RegisterUser")
                        {
                            if (dataString[2] == "Success")
                            {
                                check = "Успешная регистрация.";
                                handle.Set();
                                continue;
                            }
                            if (dataString[2] == "ErrorLogin")
                            {
                                check = "Имя пользователя уже занято.";
                                handle.Set();
                                continue;
                            }
                        }
                        if (dataString[1] == "ChangePass")
                        {
                            if (dataString[2] == "Success")
                            {
                                check = "Пароль изменен.";
                                handle.Set();
                                continue;
                            }
                            if (dataString[2] == "ErrorPassword")
                            {
                                check = "Новый пароль совпадает со старым.";
                                handle.Set();
                                continue;
                            }
                        }
                        if (dataString[1] == "LoadProfile")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            user.notes = Serializer.Deserialize<List<UserInfo.Notes>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "Disconnect")
                        {
                            MessageBox.Show("На аккаунт зашёл кто то другой.");
                            Environment.Exit(0);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadChat")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            user.favourites = Serializer.Deserialize<List<UserInfo.Favourites>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadUsers")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            users = Serializer.Deserialize<Users>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadChatRooms")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            user.chats = Serializer.Deserialize<List<UserInfo.Chat>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadMessages")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            user.chats = Serializer.Deserialize<List<UserInfo.Chat>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadNomenclatureTypes")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            products.types = Serializer.Deserialize<List<ProductsAvailable.NomenclatureType>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadNomenclature")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            products.nomenclatures = Serializer.Deserialize<List<ProductsAvailable.Nomenclature>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadProducts")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            products.products = Serializer.Deserialize<List<ProductsAvailable.Product>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadOrders")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            orders = Serializer.Deserialize<List<Order>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadInfoForAdmin")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            permissions = Serializer.Deserialize<List<string>>(stream);
                            stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            persons = Serializer.Deserialize<List<string>>(stream);
                            handle.Set();
                            continue;
                        }
                        if (dataString[1] == "LoadMyOrders")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            myOrders = Serializer.Deserialize<List<Order>>(stream);
                            handle.Set();
                            continue;
                        }
                    }
                    if (dataString[0] == "Module")
                    {
                        if (dataString[1] == "SendMessage")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            UserInfo.Chat.ChatMessages message = Serializer.Deserialize<UserInfo.Chat.ChatMessages>(stream);
                            var idChat = binaryReader.ReadInt32();
                            foreach (UserInfo.Chat item in user.chats)
                            {
                                if (item.id == idChat)
                                {
                                    item.chatMessages.Add(message);
                                    break;
                                }
                            }
                            continue;
                        }
                        if (dataString[1] == "NewChat")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            UserInfo.Chat chat = Serializer.Deserialize<UserInfo.Chat>(stream);
                            user.chats.Add(chat);
                            continue;
                        }
                        if (dataString[1] == "NewNomenclatureType")
                        {
                            var stream = new MemoryStream(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                            ProductsAvailable.NomenclatureType type = Serializer.Deserialize<ProductsAvailable.NomenclatureType>(stream);
                            products.types.Add(type);
                            continue;
                        }
                    }
                }
                catch
                {
                    if (!timer.Enabled)
                        return Task.CompletedTask;
                }
            }
        }

        void CheckServerConnection(object? source, ElapsedEventArgs e)
        {
            try
            {
                binaryWriter?.Write(checkConnection);
                binaryWriter?.Flush();
            }
            catch (Exception ex)
            {
                client.Close();
                timer.Enabled = false;
                MessageBox.Show("Соединение с сервером потеряно.\nОшибка: " + ex.Message);
                Environment.Exit(0);
            }
        }

        public string SendServer(string data, bool checking)
        {
            try
            {
                binaryWriter?.Write(data);
                if (checking)
                    handle.WaitOne();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось передать данные.\nОшибка: " + ex.Message);
                Environment.Exit(0);
            }
            return check;
        }

        public void SendServer(int data, bool checking)
        {
            try
            {
                binaryWriter?.Write(data);
                if (checking)
                    handle.WaitOne();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось передать данные.\nОшибка: " + ex.Message);
                Environment.Exit(0);
            }
        }

        public void SendServer(double data, bool checking)
        {
            try
            {
                binaryWriter?.Write(data);
                if (checking)
                    handle.WaitOne();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось передать данные.\nОшибка: " + ex.Message);
                Environment.Exit(0);
            }
        }

        public void SendServer(byte[] data, bool checking)
        {
            try
            {
                binaryWriter?.Write(data);
                if (checking)
                    handle.WaitOne();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось передать данные.\nОшибка: " + ex.Message);
                Environment.Exit(0);
            }
        }
    }
}