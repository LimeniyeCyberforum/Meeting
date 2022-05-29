using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace MeetingWPF.Controls
{
    public class StateTextBox : PlaceholderTextBox.PlaceholderTextBox
    {
        public enum StatusEnum
        {
            Empty,
            Success,
            Fail
        }

        public class StateEmptiedEventArgs : EventArgs { }
        public class StateSuccessedEventArgs : EventArgs { }
        public class StateFailedEventArgs : EventArgs { }

        #region DependencyProperty : Status

        public StatusEnum Status
        {
            get => (StatusEnum)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(StatusEnum),
                typeof(StateTextBox), new FrameworkPropertyMetadata(StatusEnum.Empty, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    (s as StateTextBox)?.OnStatusPropertyChanged(e);
                }));

        #endregion

        #region DependencyProperty : CornerRadius

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius),
                typeof(StateTextBox), new PropertyMetadata(default));

        #endregion

        public event TypedEventHandler<StateTextBox, StateEmptiedEventArgs>? Emptied;

        public event TypedEventHandler<StateTextBox, StateSuccessedEventArgs>? Successed;

        public event TypedEventHandler<StateTextBox, StateFailedEventArgs>? Failed;

        static StateTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StateTextBox),
                new FrameworkPropertyMetadata(typeof(StateTextBox)));
        }

        protected virtual void OnStatusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
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
            this.Failed?.Invoke(this, new StateFailedEventArgs());
        }
    }
}
