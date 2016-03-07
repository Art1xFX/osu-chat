using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace osu_chat.Controls
{
    public class FriendItem : Control
    {
        public FriendItem() : base()
        {
            
        }
        
        static FriendItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FriendItem), new FrameworkPropertyMetadata(typeof(FriendItem)));
        }
        

        public bool IsOnline
        {
            get
            {
                return (bool)GetValue(IsOnlineProperty);
            }
            set
            {
                SetValue(IsOnlineProperty, value);
            }
        }

        public static readonly DependencyProperty IsOnlineProperty = DependencyProperty.Register("IsOnline", typeof(bool), typeof(FriendItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public string Nickname
        {
            get
            {
                return (string)GetValue(NicknameProperty);
            }
            set
            {
                SetValue(NicknameProperty, value);
            }
        }

        public static readonly DependencyProperty NicknameProperty = DependencyProperty.Register("Nickname", typeof(string), typeof(FriendItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

               
    }
}
