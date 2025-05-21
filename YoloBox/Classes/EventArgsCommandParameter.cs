using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace YoloBox.Classes
{
    public class EventArgsCommandParameter : Freezable
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventArgsCommandParameter));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventArgsCommandParameter));

        public static readonly DependencyProperty EventArgsProperty =
            DependencyProperty.Register("EventArgs", typeof(object), typeof(EventArgsCommandParameter));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public object EventArgs
        {
            get => GetValue(EventArgsProperty);
            set => SetValue(EventArgsProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new EventArgsCommandParameter();
        }
    }

    public class PassEventArgsToCommand : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PassEventArgsToCommand));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            if (Command != null && AssociatedObject is FrameworkElement fe)
            {
                var eventArgs = parameter as RoutedEventArgs;
                Command.Execute(eventArgs);
            }
        }
    }
}
