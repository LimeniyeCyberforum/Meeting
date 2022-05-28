using Common;
using System;
using System.Windows;

namespace MeetingWPF.Controls.StateTextBox
{
    public enum Status
    {
        Empty,
        Success,
        Fail
    }

    public class StateEmptiedEventArgs : EventArgs { }
    public class StateSuccessedEventArgs : EventArgs { }
    public class StateFailedEventArgs : EventArgs { }

    public class StateTextBox : System.Windows.Controls.TextBox
    {
        #region DependencyProperty : Placeholder
        public static DependencyProperty PlaceholderProperty { get; } =
            DependencyProperty.Register("Placeholder", typeof(string), 
                typeof(StateTextBox), new FrameworkPropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
        #endregion

        #region DependencyProperty : Status
        public static DependencyProperty StatusProperty { get; } =
            DependencyProperty.Register("Status", typeof(Status),
                typeof(StateTextBox), new FrameworkPropertyMetadata(Status.Empty, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    (s as StateTextBox)?.OnStatusPropertyChanged(e);
                }));

        public Status Status
        {
            get => (Status)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }
        #endregion

        public event TypedEventHandler<StateTextBox, StateEmptiedEventArgs>? Emptied;

        public event TypedEventHandler<StateTextBox, StateSuccessedEventArgs>? Successed;

        public event TypedEventHandler<StateTextBox, StateFailedEventArgs>? Failed;

        static StateTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StateTextBox),
             new FrameworkPropertyMetadata(typeof(StateTextBox)));
        }

        private void OnStatusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void RaiseStatusEmptiedEvent(StateTextBox container)
        {
            this.Emptied?.Invoke(this, new StateEmptiedEventArgs());
        }

        protected void RaiseStatusSuccessedEvent(StateTextBox container)
        {
            this.Successed?.Invoke(this, new StateSuccessedEventArgs());
        }

        protected void RaiseStatusFailedEvent(StateTextBox container)
        {
            this.Successed?.Invoke(this, new StateSuccessedEventArgs());
        }
    }
}
