/* ***********************************
* Created by KoBE at TechLifeForum
* http://tech.reboot.pro
*************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
namespace Irc
{
    /// <summary>
    /// IRC Client class written at http://tech.reboot.pro
    /// </summary>
    public class IrcClient
    {
        #region Variables
        private string _server = "";
        private int _port = 6667;
        private string _ServerPass = "";
        private string _nick = "osurcTest";
        private string _altNick = "";
        private TcpClient irc;
        private NetworkStream stream;
        private string inputLine;
        private StreamReader reader;
        private StreamWriter writer;
        private AsyncOperation op;

        #endregion

        #region Constructors
        public IrcClient(string Server, int Port)
        {
            op = AsyncOperationManager.CreateOperation(null);
            _server = Server;
            _port = Port;
        }
        public IrcClient(string Server) : this(Server,6667)
        {
            op = AsyncOperationManager.CreateOperation(null);
            _server = Server;
            _port = 6667;
        }
        #endregion

        #region Properties
        public string Server
        {
            get { return _server; }
        }
        public int Port
        {
            get { return _port; }
        }
        public string ServerPass
        {
            get { return _ServerPass; }
            set { _ServerPass = value; }
        }
        public string Nick
        {
            get { return _nick; }
            set { _nick = value; }
        }
        public string AltNick
        {
            get { return _altNick; }
            set { _altNick = value; }
        }
        public bool Connected
        {
            get
            {
                if (irc != null)
                    if (irc.Connected)
                        return true;
                return false;
            }
        }
        #endregion

        #region Events

        public event UpdateUserListEventDelegate UpdateUsers;
        public event UserJoinedEventDelegate UserJoined;
        public event UserLeftEventDelegate UserLeft;
        public event UserNickChangeEventDelegate UserNickChange;

        public event ChannelMesssageEventDelegate ChannelMessage;
        public event NoticeMessageEventDelegate NoticeMessage;
        public event PrivateMessageEventDelegate PrivateMessage;
        public event ServerMessageEventDelegate ServerMessage;

        public event NickTakenEventDelegate NickTaken;

        public event ConnectedEventDelegate OnConnect;
        //public event DisconnectedEventDelegate Disconnected;

        public event ExceptionThrownEventDelegate ExceptionThrown;

        private void Fire_UpdateUsers(oUserList o)
        {
            //
            // op.Post(x => Fire_UpdateUsers((oUserList)x), new oUserList(channel,users));
            //

            if (UpdateUsers != null) UpdateUsers(o.Channel, o.UserList);
        }
        private void Fire_UserJoined(oUserJoined o)
        {
            //
            // op.Post(x => Fire_UserJoined((oUserJoined)x), new oUserJoined(channel,user));
            //

            if (UserJoined != null) UserJoined(o.Channel, o.User);
        }
        private void Fire_UserLeft(oUserLeft o)
        {
            //
            // op.Post(x => Fire_UserLeft((oUserLeft)x), new oUserLeft(channel,user));
            //

            if (UserLeft != null) UserLeft(o.Channel, o.User);
        }
        private void Fire_NickChanged(oUserNickChanged o)
        {
            //
            // op.Post(x => Fire_NickChanged((oUserNickChanged)x), new oUserNickChanged(old,new));
            //

            if (UserNickChange != null) UserNickChange(o.Old, o.New);
        }
        private void Fire_ChannelMessage(oChannelMessage o)
        {
            //
            // op.Post(x => Fire_ChannelMessage((oChannelMessage)x), new oChannelMessage(channel,user,message));
            //

            if (ChannelMessage != null) ChannelMessage(o.Channel, o.From, o.Message);
        }
        private void Fire_NoticeMessage(oNoticeMessage o)
        {
            //
            // op.Post(x => Fire_NoticeMessage((oNoticeMessage)x), new oNoticeMessage(user,message));
            //

            if (NoticeMessage != null) NoticeMessage(o.From, o.Message);
        }
        private void Fire_PrivateMessage(oPrivateMessage o)
        {
            //
            // op.Post(x => Fire_PrivateMessage((oPrivateMessage)x), new oPrivateMessage(user,message));
            //

            if (PrivateMessage != null) PrivateMessage(o.From, o.Message);
        }
        private void Fire_ServerMesssage(string s)
        {
            //
            // op.Post(x => Fire_ServerMesssage((string)x), message);
            //

            if (ServerMessage != null) ServerMessage(s);
        }
        private void Fire_NickTaken(string s)
        {
            //
            // op.Post(x => Fire_NickTaken((string)x), nick);
            //

            if (NickTaken != null) NickTaken(s);
        }
        private void Fire_Connected()
        {
            //
            // op.Post((x)=>Fire_Connected(),null);
            //
            if (OnConnect != null) OnConnect();
        }
        private void Fire_ExceptionThrown(Exception ex)
        {
            //
            // op.Post(x => OnExceptionThrown((Exception)x),e); <- code to throw the exception
            //

            if (ExceptionThrown != null) ExceptionThrown(ex);
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Connect to the IRC server
        /// </summary>
        public void Connect()
        {
            Thread t = new Thread(DoConnect);
            t.IsBackground = true;
            t.Start();
            //DoConnect();
        }
        private void DoConnect()
        {
            try
            {
                irc = new TcpClient(_server, _port);
                stream = irc.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                if (!string.IsNullOrEmpty(_ServerPass))
                    Send("PASS " + _ServerPass);

                Send("NICK " + _nick);
                Send("USER " + _nick + " 0 * :" + _nick);

                Listen();
            }
            catch (Exception ex)
            {
                op.Post(x => Fire_ExceptionThrown((Exception)x),ex); 
            }
        }
        /// <summary>
        /// Disconnect from the IRC server
        /// </summary>
        public void Disconnect()
        {
            if (irc != null)
            {
                if (irc.Connected)
                {
                    Send("quit");
                }
                irc = null;
                //if (Disconnected != null)  // TODO: shouldn't need this event,
                //    Disconnected();        //       it's only fired her
            }
        }
        /// <summary>
        /// Sends the JOIN command to the server
        /// </summary>
        /// <param name="Channel">Channel to join</param>
        public void JoinChannel(string Channel)
        {
            if (irc != null && irc.Connected)
            {
                Send("JOIN " + Channel);
            }
        }
        /// <summary>
        /// Sends the PART command for a given channel
        /// </summary>
        /// <param name="Channel">Channel to leave</param>
        public void PartChannel(string Channel)
        {
            Send("PART " + Channel);
        }
        /// <summary>
        /// Send a notice to a user
        /// </summary>
        /// <param name="Nick">User to send the notice to</param>
        /// <param name="message">The message to send</param>
        public void SendNotice(string Nick, string message)
        {
            Send("NOTICE " + Nick + " :" + message);
        }
        /// <summary>
        /// Send a message to the channel
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(string Channel, string Message)
        {
            Send("PRIVMSG " + Channel + " :" + Message);
        }
        /// <summary>
        /// Send RAW IRC commands
        /// </summary>
        /// <param name="message"></param>
        public void SendRAW(string message)
        {
            Send(message);
        }
        #endregion

        #region PrivateMethods
        private void Listen()
        {

            while ((inputLine = reader.ReadLine()) != null)
            {
                ParseData(inputLine);
            }
        }
        private void ParseData(string data)
        {
            string[] ircData = data.Split(' ');
            if (data.Length > 4)
            {
                if (data.Substring(0, 4) == "PING")
                {
                    Send("PONG " + ircData[1]);
                    return;
                }

            }
            switch (ircData[1])
            {
                case "001":
                    Send("MODE " + _nick + " +B");
                    op.Post((x) => Fire_Connected(), null);
                    break;
                case "353":
                    op.Post(x => Fire_UpdateUsers((oUserList)x), new oUserList(ircData[4], JoinArray(ircData, 5).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
                    break;
                case "433":
                    op.Post(x => Fire_NickTaken((string)x), ircData[3]);
                    if (ircData[3] == _altNick)
                    {
                        Random rand = new Random();
                        string randomNick = "Guest" + rand.Next(0, 9) + rand.Next(0, 9) + rand.Next(0, 9);
                        Send("NICK " + randomNick);
                        Send("USER " + randomNick + " 0 * :" + randomNick);
                        _nick = randomNick;
                    }
                    else
                    {
                        Send("NICK " + _altNick);
                        Send("USER " + _altNick + " 0 * :" + _altNick);
                        _nick = _altNick;
                    }
                    break;
                case "JOIN":
                    op.Post(x => Fire_UserJoined((oUserJoined)x), new oUserJoined(ircData[2], ircData[0].Substring(1, ircData[0].IndexOf("!") - 1)));
                    break;
                case "NICK":
                    op.Post(x => Fire_NickChanged((oUserNickChanged)x), new oUserNickChanged(ircData[0].Substring(1, ircData[0].IndexOf("!") - 1), JoinArray(ircData, 3)));
                    break;
                case "NOTICE":
                    if (ircData[0].Contains("!"))
                    {
                        op.Post(x => Fire_NoticeMessage((oNoticeMessage)x), new oNoticeMessage(ircData[0].Substring(1, ircData[0].IndexOf('!') - 1), JoinArray(ircData, 3)));
                    }
                    else
                    {
                        op.Post(x => Fire_NoticeMessage((oNoticeMessage)x), new oNoticeMessage(_server, JoinArray(ircData, 3)));
                    }
                    break;
                case "PRIVMSG":
                    if (ircData[2].ToLower() == _nick.ToLower())
                    {
                        op.Post(x => Fire_PrivateMessage((oPrivateMessage)x), new oPrivateMessage(ircData[0].Substring(1, ircData[0].IndexOf('!') - 1), JoinArray(ircData, 3)));
                    }
                    else
                    {
                        op.Post(x => Fire_ChannelMessage((oChannelMessage)x), new oChannelMessage(ircData[2], ircData[0].Substring(1, ircData[0].IndexOf('!') - 1), JoinArray(ircData, 3)));
                    }
                    break;
                case "PART":
                case "QUIT":
                    op.Post(x => Fire_UserLeft((oUserLeft)x), new oUserLeft(ircData[2], ircData[0].Substring(1, data.IndexOf("!") - 1)));
                    Send("NAMES " + ircData[2]);
                    break;
                default:
                    if (ircData.Length > 3)
                        op.Post(x => Fire_ServerMesssage((string)x), JoinArray(ircData, 3));
                    break;
            }

        }
        private string StripMessage(string message)
        {
            foreach (Match m in new Regex((char)3 + @"(?:\d{1,2}(?:,\d{1,2})?)?").Matches(message))
                message = message.Replace(m.Value, "");
            if (message == "")
                return "";
            else if (message.Substring(0, 1) == ":" && message.Length > 2)
                return message.Substring(1, message.Length - 1);
            else
                return message;
        }
        private string JoinArray(string[] strArray, int startIndex)
        {
            return StripMessage(String.Join(" ", strArray, startIndex, strArray.Length - startIndex));
        }
        private void Send(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
        #endregion

        #region Structs

        public struct oUserList
        {
            public string Channel;
            public string[] UserList;
            public oUserList(string Channel, string[] UserList)
            {
                this.Channel = Channel;
                this.UserList = UserList;
            }
        }
        public struct oUserJoined
        {
            public string Channel;
            public string User;
            public oUserJoined(string Channel, string User)
            {
                this.Channel = Channel;
                this.User = User;
            }
        }
        public struct oUserLeft
        {
            public string Channel;
            public string User;
            public oUserLeft(string Channel, string User)
            {
                this.Channel = Channel;
                this.User = User;
            }
        }

        public struct oChannelMessage
        {
            public string Channel;
            public string From;
            public string Message;
            public oChannelMessage(string Channel, string From, string Message)
            {
                this.Channel = Channel;
                this.From = From;
                this.Message = Message;
            }
        }
        public struct oNoticeMessage
        {
            public string From;
            public string Message;
            public oNoticeMessage(string From, string Message)
            {
                this.From = From;
                this.Message = Message;
            }
        }
        public struct oPrivateMessage
        {
            public string From;
            public string Message;
            public oPrivateMessage(string From, string Message)
            {
                this.From = From;
                this.Message = Message;
            }
        }
        public struct oUserNickChanged
        {
            public string Old;
            public string New;
            public oUserNickChanged(string Old, string New)
            {
                this.Old = Old;
                this.New = New;
            }
        }

        #endregion
    }

}