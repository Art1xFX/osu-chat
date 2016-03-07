
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace osu_chat
{
    public class ChatUser
    {
        public ImageSource Avatar { get; set; }
        public bool IsSupporter { get; set; }
        public string Nickname { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string server = "irc.ppy.sh";
        public const string html_supporter = "<div class=\"profileSupporter\">";
        public const string html_avatar = "<div class=\"avatar-holder\"><img src=\"";

        public static List<ChatUser> cachedUsers = new List<ChatUser>();

        public static string[] channels;


        public static ChatUser ThisUser;

        public static async Task<ChatUser> GetChatUser(string Nickname)
        {
            var user = await Osu.Api.GetUserAsync(App.ApiKey, Nickname);
            var html = (HttpWebRequest)WebRequest.Create(string.Format("https://osu.ppy.sh/u/{0}", user.UserId));
            
            using (var stream = (await html.GetResponseAsync()).GetResponseStream())
            {
                string str = await new StreamReader(stream).ReadToEndAsync();

                bool isSupporter = false;
                string avatar = null;

                int j = 0;
                for (int i = 0; i < str.Length - html_avatar.Length; i++)
                {
                    if (str.Substring(i, html_avatar.Length) == html_avatar)
                    {
                        j = i + html_avatar.Length;
                        while (str[j] != '"')
                        {
                            avatar += str[j];
                            j++;
                        }
                    }
                    else if (str.Substring(i, html_supporter.Length) == html_supporter)
                        isSupporter = true;
                    
                }

                ImageSource Avatar = null;
                //try
                //{
                if (avatar != null)
                {
                    Avatar = new BitmapImage(new Uri(string.Format("https:{0}", avatar)));
                    if (Avatar.CanFreeze)
                        Avatar.Freeze();
                }
                //}
                //catch { }

                return new ChatUser()
                {
                    Nickname = user.Username,
                    Avatar = Avatar,
                    IsSupporter = isSupporter
                };
            }
        }
        
        public MainWindow(Irc.IrcClient client)
        {
            InitializeComponent();
            c = client;
            c.PrivateMessage += C_PrivateMessage;


            if (Directory.Exists(string.Format("cfg/", c.Nick)))
                UserConfig.Load(string.Format("cfg/{0}.cfg", c.Nick));
            else
                Directory.CreateDirectory("cfg/");
            if((!File.Exists(string.Format("cfg/{0}.cfg", c.Nick))) || channels == null)
                UserConfig.Channels = channels = new string[] { "#osu", "#lobby" };

            App.FriendList = new UserCollection(UserConfig.FriendList);
            App.IgnoreList = new UserCollection(UserConfig.IgnoreList);
            App.HighlightedWords = new UserCollection(UserConfig.HighlightedWords);

                foreach (var channel in UserConfig.Channels)
                    tabControl.Items.Add(new TabItem()
                    {
                        Header = channel,
                        Content = new Controls.ChatPage(c, channel)
                    });
                channels = UserConfig.Channels;
            

            App.FriendList.UserAdded += FriendList_Changed;
            App.FriendList.UserRemoved += FriendList_Changed;
        }

        private void C_PrivateMessage(string User, string Message)
        {
            bool founded = false;
            foreach (TabItem item in tabControl.Items)
                if (item.Header.ToString() == User)
                    founded = true;
            if (!founded)
                tabControl.Items.Add(new TabItem()
                {
                    Header = User,
                    Content = new Controls.ChatPage(c, User)
                });
        }

        private void FriendList_Changed(object sender, string e)
        {
            UserConfig.FriendList = App.FriendList.ToArray();
            UserConfig.Save(string.Format("cfg/{0}.cfg", c.Nick));
        }

        public static List<string> ignorelist = new List<string>();

        Irc.IrcClient c;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThisUser = await GetChatUser(c.Nick);
            nick.Text = ThisUser.Nickname;
            image.Source = ThisUser.Avatar;
        }

        private void Ellipse_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SettingsWindow s = new SettingsWindow();
            s.Show();
        }
    }
}
