using System;
using System.Collections.Generic;
using System.Text;

namespace inventory.Presentation
{
    public class MenuOption
    {
        public string Text { get; }
        public Action Action { get; }

        public MenuOption(string text, Action action)
        {
            Text = text;
            Action = action;
        }
    }
}
