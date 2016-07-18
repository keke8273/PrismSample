using System.Windows;
using System.Windows.Input;

namespace QBR.Infrastructure.Commands
{
    public static class Closed
    {
        private static readonly DependencyProperty ClosedCommandBehaviorProperty = DependencyProperty.RegisterAttached(
            "ClosedCommandBehavior",
            typeof (WindowCloseCommandBehavior),
            typeof (Closed),
            null);

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(Closed),
            new PropertyMetadata(OnSetCommandCallback));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof (object),
            typeof (Closed),
            new PropertyMetadata(OnSetCommandParameterCallback));


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for window")]
        public static void SetCommand(Window window, ICommand command)
        {
            if (window == null) throw new System.ArgumentNullException("window");
            window.SetValue(CommandProperty, command);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for window")]
        public static ICommand GetCommand(Window window)
        {
            if (window == null) throw new System.ArgumentNullException("window");
            return window.GetValue(CommandProperty) as ICommand;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for window")]
        public static void SetCommandParameter(Window window, object parameter)
        {
            if (window == null) throw new System.ArgumentNullException("window");
            window.SetValue(CommandParameterProperty, parameter);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for window")]
        public static object GetCommandParameter(Window window)
        {
            if (window == null) throw new System.ArgumentNullException("window");
            return window.GetValue(CommandParameterProperty);
        }

        private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window != null)
            {
                var behavior = GetOrCreateBehavior(window);
                behavior.Command = e.NewValue as ICommand;
            }
        }

        private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window != null)
            {
                var behavior = GetOrCreateBehavior(window);
                behavior.CommandParameter = e.NewValue;
            }
        }

        private static WindowCloseCommandBehavior GetOrCreateBehavior(Window window)
        {
            var behavior = window.GetValue(ClosedCommandBehaviorProperty) as WindowCloseCommandBehavior;
            if (behavior == null)
            {
                behavior = new WindowCloseCommandBehavior(window);
                window.SetValue(ClosedCommandBehaviorProperty, behavior);
            }

            return behavior;
        }
    }
}
