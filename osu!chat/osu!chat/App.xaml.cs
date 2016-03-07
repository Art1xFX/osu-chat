using osu_chat.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace osu_chat
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserCollection FriendList = new UserCollection();
        public static UserCollection IgnoreList = new UserCollection();
        public static UserCollection HighlightedWords = new UserCollection();
        public static int MaxMessages = 50;
        public static int MaxCachedUsers = 100;



#error Your ApiKey from https://osu.ppy.sh/p/api
        public const string ApiKey = "";


        public class userConextMenu : ContextMenu
        {
            public userConextMenu(ChatMessage parent) : base()
            {

                this.Items.Add(new MenuItem() { Header = "Open profile" });

                if (!FriendList.Exists(parent.Nickname))
                {
                    this.Items.Add(new MenuItem() { Header = "Add to friend list" });
                    ((MenuItem)Items[1]).Click += (o, e) =>
                    {

                        App.FriendList.Add(parent.Nickname);

                        //osu_chat.MainWindow.friends.Add(await osu_chat.MainWindow.GetChatUser(parent.Nickname));
                    };
                }
                else
                {
                    this.Items.Add(new MenuItem() { Header = "Remove from friend list" });
                    ((MenuItem)Items[1]).Click += (o, e) =>
                    {
                        App.FriendList.Remove(parent.Nickname);

                        //osu_chat.MainWindow.friends.Remove(osu_chat.MainWindow.friends.Where(u => u.Nickname == parent.Nickname).First());
                    };
                }

                if (!IgnoreList.Exists(parent.Nickname))
                {
                    this.Items.Add(new MenuItem() { Header = "Add to ignore list" });
                    ((MenuItem)Items[2]).Click += (o, e) =>
                    {
                        IgnoreList.Add(parent.Nickname);

                        //osu_chat.MainWindow.ignoredUser.Add(await osu_chat.MainWindow.GetChatUser(parent.Nickname));
                    };
                }
                else
                {
                    this.Items.Add(new MenuItem() { Header = "Remove from ignore list" });
                    ((MenuItem)Items[2]).Click += (o, e) =>
                    {
                        IgnoreList.Remove(parent.Nickname);
                    };
                }


                ((MenuItem)Items[0]).Click += async (o, e) =>
                {
                    // zameniti na user id
                    Process.Start(string.Format("http://osu.ppy.sh/u/{0}", (await Osu.Api.GetUserAsync(ApiKey, parent.Nickname)).UserId));
                };

            }
        }
        
        public static SolidColorBrush UserBrush = new SolidColorBrush(Color.FromRgb(255, 240, 154));
        public static SolidColorBrush SupporterBrush = new SolidColorBrush(Color.FromRgb(255, 167, 34));
        public static SolidColorBrush FriendBrush = new SolidColorBrush(Color.FromRgb(255, 102, 170));
        public static SolidColorBrush HighlightedBrush = new SolidColorBrush(Color.FromRgb(136, 179, 0));

        static App()
        {
            UserBrush.Freeze();
            SupporterBrush.Freeze();
            FriendBrush.Freeze();
            HighlightedBrush.Freeze();
        }
    }
}
