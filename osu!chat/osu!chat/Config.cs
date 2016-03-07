using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace osu_chat
{
    public static class Config
    {
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static bool RememberLogin { get; set; }
        public static bool RememberPassword { get; set; }

        public static void Save(string filename)
        {
            string[] config = new string[2];
            config[0] = string.Format("Login = {0}", Login);
            config[1] = string.Format("Password = {0}", Password);
            File.WriteAllLines(filename, config);
        }


        public static void Load(string filename)
        {
            if (File.Exists(filename))
            {
                string[] config = File.ReadAllLines(filename);
                for (int i = 0; i < config.Length; i++)
                    if (!string.IsNullOrWhiteSpace(config[i]))
                        if (config[i].StartsWith("Login = "))
                        {
                            Login = config[i].Substring(8);
                            RememberLogin = !string.IsNullOrWhiteSpace(Login);
                        }
                        else if (config[i].StartsWith("Password = "))
                        {
                            Password = config[i].Substring(11);
                            RememberPassword = !string.IsNullOrWhiteSpace(Password);
                        }
            }
        }
    }
}
