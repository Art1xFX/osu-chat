using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace osu_chat.Controls
{
    /// <summary>
    /// Логика взаимодействия для ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Grid
    {
        public string channel;
        public Irc.IrcClient ircClient;

        public string Channel => channel;



        public ChatPage(Irc.IrcClient ircClient, string channel)
        {
            InitializeComponent();


            if (channel.StartsWith("#"))
            {
                foreach (string Friend in App.FriendList)
                {
                    int newFriendIndex = friendList.Children.Add(new FriendItem() { Nickname = Friend });
                    ((FriendItem)friendList.Children[newFriendIndex]).MouseDown += ChatPage_MouseDown;
                }

                this.ircClient = ircClient;
                this.channel = channel;

                this.ircClient.JoinChannel(channel);
                this.ircClient.ChannelMessage += IrcClient_ChannelMessage;

                this.ircClient.UserJoined += IrcClient_UserJoined;
                this.ircClient.UserLeft += IrcClient_UserLeft;
                this.ircClient.UpdateUsers += IrcClient_UpdateUsers;

            }
            else
            {
                ircClient.PrivateMessage += IrcClient_PrivateMessage;
                col.Width = new GridLength(0);
                gridSplitter.Visibility = Visibility.Collapsed;
            }
            App.FriendList.UserAdded += FriendList_UserAdded;
            App.FriendList.UserRemoved += FriendList_UserRemoved;

        }

        private async void IrcClient_PrivateMessage(string User, string Message)
        {
            if (Channel == User)
            {
                ChatUser ChatUser = null;

                var cachedUser = MainWindow.cachedUsers.Where(u => u.Nickname == User);
                if (cachedUser.Count() > 0)
                    ChatUser = cachedUser.First();
                else
                {
                    ChatUser = await MainWindow.GetChatUser(User);

                    if (App.MaxCachedUsers <= MainWindow.cachedUsers.Count)
                        MainWindow.cachedUsers.RemoveAt(0);
                    MainWindow.cachedUsers.Add(ChatUser);
                }

                var newMessage = new ChatMessage()
                {
                    Nickname = User,
                    Message = Message,
                    Avatar = ChatUser.Avatar
                };

                if (App.IgnoreList.Exists(User))
                    return;
                else if (App.FriendList.Exists(User))
                    newMessage.BorderBrush = App.FriendBrush;
                else if (ChatUser.IsSupporter)
                    newMessage.BorderBrush = App.SupporterBrush;

                foreach (var word in Message.Split(' ', '.', ',', ':', ';', '!', '?', '&'))
                    if (App.HighlightedWords.Exists(word))
                        newMessage.BorderBrush = App.HighlightedBrush;

                newMessage.ContextMenu = new App.userConextMenu(newMessage);
               
                if (App.MaxMessages <= Chat.Children.Count)
                    Chat.Children.RemoveAt(0);
                Chat.Children.Add(newMessage);
                
                ((ScrollViewer)Chat.Parent).ScrollToEnd();

            }
        }

        private void OrderChildren()
        {
            //    List<FriendItem> items = new List<FriendItem>(friendList.Children.OfType<FriendItem>());
            //    items = (from f in items orderby f.IsOnline orderby f.Nickname select f).ToList();

            //var items = friendList.Children.OfType<FriendItem>().OrderBy(f => f.IsOnline).ThenBy(f => f.Nickname);
            var items = friendList.Children.OfType<FriendItem>().OrderBy(f => f.Nickname);

            friendList.Children.Clear();
            foreach (var item in items)
                friendList.Children.Add(item);
        }


        private void FriendList_UserRemoved(object sender, string e)
        {
            var friend = from f in friendList.Children.OfType<FriendItem>() where f.Nickname == e select f;
            if (friend.Count() > 0)
                friendList.Children.Remove(friend.First());
            
        }

        private void FriendList_UserAdded(object sender, string e)
        {

            int newFriendIndex = friendList.Children.Add(new FriendItem() { Nickname = e });
            ((FriendItem)friendList.Children[newFriendIndex]).MouseDown += ChatPage_MouseDown;
            if (UserList.Exists(e))
                ((FriendItem)friendList.Children[newFriendIndex]).IsOnline = true;
        }

        private void ChatPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var tabControl = ((TabControl)((TabItem)this.Parent).Parent);
                bool founded = false;
                foreach (TabItem item in tabControl.Items)
                    if (item.Header.ToString() == ((FriendItem)sender).Nickname)
                        founded = true;
                if (!founded)
                    tabControl.Items.Add(new TabItem()
                    {
                        Header = ((FriendItem)sender).Nickname,
                        Content = new ChatPage(ircClient, ((FriendItem)sender).Nickname)
                    });
            }
        }

        private void IrcClient_UserLeft(string Channel, string User)
        {
            UserList.Remove(User);
            if (Channel == channel)
                if (App.FriendList.Exists(User))
                    ((IEnumerable<FriendItem>)friendList.Children).Where(f => f.Nickname == User).First().IsOnline = false;
        }

        private void IrcClient_UserJoined(string Channel, string User)
        {
            UserList.Add(User);
            if (Channel == channel)
                if (App.FriendList.Exists(User))
                    ((IEnumerable<FriendItem>)friendList.Children).Where(f => f.Nickname == User).First().IsOnline = true;
        }

        //string[] userlist;

        UserCollection UserList = new UserCollection();


        private void IrcClient_UpdateUsers(string Channel, string[] userlist)
        {
            if (Channel == channel)
            {
                this.UserList.AddRange(userlist);
                for (int i = 0; i < userlist.Length; i++)
                {
                    if (App.FriendList.Exists(userlist[i]))
                    {
                        var friend = from f in friendList.Children.OfType<FriendItem>() where f.Nickname == userlist[i] select f;
                        if (friend.Count() > 0)
                            friend.First().IsOnline = true;
                    }
                }
            }
        }

        private async void IrcClient_ChannelMessage(string Channel, string User, string Message)
        {
            // dobaviti umnii scroll
            if (Channel == channel)
            {
                ChatUser ChatUser = null;

                var cachedUser = MainWindow.cachedUsers.Where(u => u.Nickname == User);
                if (cachedUser.Count() > 0)
                    ChatUser = cachedUser.First();
                else
                {
                    ChatUser = await MainWindow.GetChatUser(User);

                    if (App.MaxCachedUsers <= MainWindow.cachedUsers.Count)
                        MainWindow.cachedUsers.RemoveAt(0);
                    MainWindow.cachedUsers.Add(ChatUser);
                }
                
                var newMessage = new ChatMessage()
                {
                    Nickname = User,
                    Message = Message,
                    Avatar = ChatUser.Avatar
                };

                if (App.IgnoreList.Exists(User))
                    return;
                else if (App.FriendList.Exists(User))
                    newMessage.BorderBrush = App.FriendBrush;
                else if (ChatUser.IsSupporter)
                    newMessage.BorderBrush = App.SupporterBrush;

                foreach (var word in Message.Split(' ', '.', ',', ':', ';', '!', '?', '&'))
                    if (App.HighlightedWords.Exists(word))
                        newMessage.BorderBrush = App.HighlightedBrush;
                
                newMessage.ContextMenu = new App.userConextMenu(newMessage);
                newMessage.MouseDown += NewMessage_MouseDown;


                if (App.MaxMessages <= Chat.Children.Count)
                    Chat.Children.RemoveAt(0);
                Chat.Children.Add(newMessage);




                ((ScrollViewer)Chat.Parent).ScrollToEnd();
                
            }
        }

        private void NewMessage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                textBox.Text = string.Format("{0}: ", ((ChatMessage)sender).Nickname);
                textBox.Focus();
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter && textBox.Text != null && textBox.Text != string.Empty)
            {

                var newMessage = new ChatMessage()
                {
                    Nickname = MainWindow.ThisUser.Nickname,
                    Avatar = MainWindow.ThisUser.Avatar,
                    Message = textBox.Text

                };
                newMessage.Style = (Style)Application.Current.Resources["myMessage"];

                if (MainWindow.ThisUser.IsSupporter)
                    newMessage.BorderBrush = App.SupporterBrush;

                Chat.Children.Add(newMessage);

                if (Channel.StartsWith("#"))
                {

                    ircClient.SendMessage(channel, textBox.Text);
                }
                else
                {
                    ircClient.SendNotice(channel, textBox.Text);
                }
                textBox.Text = string.Empty;
            }
        }



        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textBox.Text == "Type your message here!")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.White;
            }
        }


        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Type your message here!";
                textBox.Foreground = Brushes.WhiteSmoke;
            }
        }
    }
}
