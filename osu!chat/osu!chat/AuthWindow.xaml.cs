using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void textBlock1_KeyDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://osu.ppy.sh/p/irc");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Config.Load("settings.ini");
            if (Config.RememberLogin)
            {
                checkBox.IsChecked = true;
                textBox.Text = Config.Login;
            }
            if (Config.RememberPassword)
            {
                checkBox_Copy.IsChecked = true;
                passwordBox.Password = Config.Password;
            }
        }

        Irc.IrcClient c;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            gr.IsEnabled = false;
            gr.Opacity = 0.6d;

            c = new Irc.IrcClient("irc.ppy.sh", 6667);
            c.ExceptionThrown += C_ExceptionThrown;


            c.Nick = textBox.Text;
            c.ServerPass = passwordBox.Password;
            c.Connect();
            c.OnConnect += C_OnConnect;
            
           
        }

        private void C_ExceptionThrown(Exception ex)
        {
            gr.IsEnabled = true;
            gr.Opacity = 1.0d;
            MessageBox.Show(ex.Message);
        }

        private void C_OnConnect()
        {
            MainWindow mainWindow = new MainWindow(c);
            mainWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (checkBox.IsChecked.Value)
                Config.Login = textBox.Text;
            else
                Config.Login = null;
            if (checkBox_Copy.IsChecked.Value)
                Config.Password = passwordBox.Password;
            else
                Config.Password = null;
            Config.Save("settings.ini");
        }
    }
}
