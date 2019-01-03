using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Accelerider.Windows.Controls
{
    public enum NotificationPlacement
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class NotificationConfigure
    {
        public string Key { get; set; }

        public ImageSource Icon { get; set; }

        public string Message { get; set; }

        public string Description { get; set; }

        public NotificationPlacement Placement { get; set; }

        public TimeSpan Duration { get; set; }

        public List<(string Text, ICommand Command)> Buttons { get; set; }

        public Action OnClick { get; set; }

        public Action OnClose { get; set; }
    }
}
