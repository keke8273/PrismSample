using System;
using System.Windows;
using Microsoft.Practices.Prism.Commands;

namespace QBR.Infrastructure.Commands
{
    public class WindowCloseCommandBehavior : CommandBehaviorBase<Window>
    {
        public WindowCloseCommandBehavior(Window window) : base(window)
        {
            if (window == null) throw new ArgumentNullException("window");
            window.Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            ExecuteCommand();
        }
    }
}
