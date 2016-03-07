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
    public class ChatMessage : Control
    {
        public ChatMessage() : base()
        {
            Time = string.Format("{0:D2}:{1:D2}", DateTime.Now.Hour, DateTime.Now.Minute);

        }
        
        static ChatMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatMessage), new FrameworkPropertyMetadata(typeof(ChatMessage)));
        }

        public ImageSource Avatar
        {
            get
            {
                return (ImageSource)GetValue(AvatarProperty);
            }
            set
            {
                SetValue(AvatarProperty, value);
            }
        }

        public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register("Avatar", typeof(ImageSource), typeof(ChatMessage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));



        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ChatMessage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public string Time
        {
            get
            {
                return (string)GetValue(TimeProperty);
            }
            set
            {
                SetValue(TimeProperty, value);
            }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(string), typeof(ChatMessage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


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

        public static readonly DependencyProperty NicknameProperty = DependencyProperty.Register("Nickname", typeof(string), typeof(ChatMessage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Brush
        {
            get
            {
                return (Brush)GetValue(BrushProperty);
            }
            set
            {
                SetValue(BrushProperty, value);
            }
        }

        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(Brush), typeof(ChatMessage), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
                
    }
}
