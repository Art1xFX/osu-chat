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

namespace osu_chat
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Text = string.Join(" ", App.FriendList);
            textBox1_Copy.Text = string.Join(" ", App.IgnoreList);
            textBox1_Copy1.Text = string.Join(" ", App.HighlightedWords);
            textBox1_Copy2.Text = string.Join(" ", MainWindow.channels);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //UserConfig.Write(string.Format("cfg/{0}.cfg", MainWindow.ThisUser.Nickname), "FriendList", textBox1.Text);
            //UserConfig.Write(string.Format("cfg/{0}.cfg", MainWindow.ThisUser.Nickname), "IgnoreList", textBox1_Copy.Text);
            //UserConfig.Write(string.Format("cfg/{0}.cfg", MainWindow.ThisUser.Nickname), "HighlightedWords", textBox1_Copy1.Text);
            //UserConfig.Write(string.Format("cfg/{0}.cfg", MainWindow.ThisUser.Nickname), "ChatChannels", textBox1_Copy2.Text);


            UserConfig.FriendList = textBox1.Text.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
            App.FriendList.AddRange(UserConfig.FriendList);
            UserConfig.IgnoreList = textBox1_Copy.Text.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
            App.IgnoreList = new UserCollection(UserConfig.IgnoreList);
            UserConfig.HighlightedWords = textBox1_Copy1.Text.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
            App.HighlightedWords = new UserCollection(UserConfig.HighlightedWords);
            MainWindow.channels = UserConfig.Channels = textBox1_Copy2.Text.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
        }
    }
}
