using Common;
using System;
using System.Windows;
using System.Windows.Media;

namespace MeetingWPF.Controls
{
    public class StateTextBox : System.Windows.Controls.TextBox
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
            DependencyProperty.Register("Status", typeof(StatusEnum),
                typeof(StateTextBox), new FrameworkPropertyMetadata(StatusEnum.Empty, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    (s as StateTextBox)?.OnStatusPropertyChanged(e);
                }));

        public StatusEnum Status
        {
            get => (StatusEnum)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        #endregion

        #region DependencyProperty : CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                typeof(StateTextBox), new PropertyMetadata(default));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        #region DependencyProperty : PlaceholderFontSize

        public static DependencyProperty PlaceholderFontSizeProperty { get; } =
            DependencyProperty.Register("PlaceholderFontSize", typeof(double),
                typeof(StateTextBox), new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.Inherits, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    ((StateTextBox)s)?.OnPlaceholderFontSizeChanged((double)e.OldValue, (double)e.NewValue);
                }));

        public double PlaceholderFontSize
        {
            get => (double)GetValue(PlaceholderFontSizeProperty);
            set => SetValue(PlaceholderFontSizeProperty, value);
        }

        #endregion

        #region DependencyProperty : PlaceholderFontFamily

        public static DependencyProperty PlaceholderFontFamilyProperty { get; } =
            DependencyProperty.Register("PlaceholderFontFamily", typeof(FontFamily),
                typeof(StateTextBox), new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.Inherits, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((StateTextBox)s)?.OnFontFamilyChanged(e.OldValue as FontFamily, e.NewValue as FontFamily);
        }));

        public FontFamily PlaceholderFontFamily
        {
            get
            {
                return (FontFamily)GetValue(PlaceholderFontFamilyProperty);
            }
            set
            {
                SetValue(PlaceholderFontFamilyProperty, value);
            }
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

        protected virtual void OnStatusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPlaceholderFontSizeChanged(double oldValue, double newValue)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnFontFamilyChanged(FontFamily? fontFamily1, FontFamily? fontFamily2)
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
            this.Failed?.Invoke(this, new StateFailedEventArgs());
        }
    }
}
