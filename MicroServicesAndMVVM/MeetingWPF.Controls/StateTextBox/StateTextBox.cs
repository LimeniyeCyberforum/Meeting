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

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string),
                typeof(StateTextBox), new FrameworkPropertyMetadata(string.Empty));

        #endregion

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

        #region DependencyProperty : PlaceholderFontSize

        public double PlaceholderFontSize
        {
            get => (double)GetValue(PlaceholderFontSizeProperty);
            set => SetValue(PlaceholderFontSizeProperty, value);
        }

        public static readonly DependencyProperty PlaceholderFontSizeProperty =
            DependencyProperty.Register(nameof(PlaceholderFontSize), typeof(double),
                typeof(StateTextBox), new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.Inherits, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    ((StateTextBox)s)?.OnPlaceholderFontSizeChanged((double)e.OldValue, (double)e.NewValue);
                }));

        #endregion

        #region DependencyProperty : PlaceholderFontFamily

        public FontFamily PlaceholderFontFamily
        {
            get => (FontFamily)GetValue(PlaceholderFontFamilyProperty);
            set => SetValue(PlaceholderFontFamilyProperty, value);
        }

        public static readonly DependencyProperty PlaceholderFontFamilyProperty =
            DependencyProperty.Register(nameof(PlaceholderFontFamily), typeof(FontFamily),
                typeof(StateTextBox), new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.Inherits, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    ((StateTextBox)s)?.OnFontFamilyChanged(e.OldValue as FontFamily, e.NewValue as FontFamily);
                }));

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
