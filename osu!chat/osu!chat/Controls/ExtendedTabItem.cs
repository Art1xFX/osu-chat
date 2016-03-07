using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace osu_chat.Controls
{
    public class ExtendedTabItem : TabItem
    {
        static ExtendedTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTabItem), new FrameworkPropertyMetadata(typeof(ExtendedTabItem)));
            //_dType = DependencyObjectType.FromSystemTypeInternal(typeof(Button));
        }

        public ExtendedTabItem() : base()
        {
        }

        public void OnButtonClick()
        {
            
        }

    }
}
