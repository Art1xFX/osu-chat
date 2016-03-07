using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_chat
{
    public static class UserConfig
    {
        public static void Load(string filename)
        {
            if (File.Exists(filename))
                foreach (var str in File.ReadAllLines(filename))
                    if (str.StartsWith("ChatChannels = "))
                        Channels = str.Substring(15).Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
                    else if (str.StartsWith("FriendList = "))
                        FriendList = str.Substring(13).Split(' ').Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();
                    else if(str.StartsWith("IgnoreList = "))
                        IgnoreList = str.Substring(13).Split(' ').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                    else if(str.StartsWith("HighlightedWords = "))
                        HighlightedWords = str.Substring(19).Split(' ').Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
        }

        public static void Save(string filename)
        {
            string cc = null;
            string f = null;
            string i = null;
            string hw = null;

            if (Channels != null)
                cc = string.Format("ChatChannels = {0}", string.Join(" ", Channels));
            else
                cc = "ChatChannels = ";
            if (FriendList != null)
                f = string.Format("FriendList = {0}", string.Join(" ", FriendList));
            else
                f = "ChatChannels = ";
            if (IgnoreList != null)
                i = string.Format("IgnoreList = {0}", string.Join(" ", IgnoreList));
            else
                i = "ChatChannels = ";
            if (HighlightedWords != null)
                hw = string.Format("HighlightedWords = {0}", string.Join(" ", HighlightedWords));
            else
                hw = "ChatChannels = ";

            File.WriteAllLines(filename,
            new string[]
            {
                cc,f,i,hw
            });
        }
             
        
        public static string[] Channels
        {
            get; set;
        }
        public static string[] HighlightedWords { get; set; }

        public static string[] FriendList { get;  set; }

        public static string[] IgnoreList { get; set; }


    }
}
