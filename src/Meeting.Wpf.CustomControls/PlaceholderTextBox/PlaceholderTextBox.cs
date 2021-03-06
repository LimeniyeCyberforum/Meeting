using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Meeting.Wpf.CustomControls
{
    public class PlaceholderTextBox : TextBox
    {
        static PlaceholderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));
        }

        #region DependencyProperty : Placeholder

        public object Placeholder
        {
            get => GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(object), 
                typeof(PlaceholderTextBox), new PropertyMetadata(default));

        #endregion

        #region DependencyProperty : PlaceholderFontSize

        public double PlaceholderFontSize
        {
            get => (double)GetValue(PlaceholderFontSizeProperty);
            set => SetValue(PlaceholderFontSizeProperty, value);
        }

        public static readonly DependencyProperty PlaceholderFontSizeProperty =
            DependencyProperty.Register(nameof(PlaceholderFontSize), typeof(double),
                typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.Inherits, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    ((PlaceholderTextBox)s)?.OnPlaceholderFontSizeChanged((double)e.OldValue, (double)e.NewValue);
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
                typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), FrameworkPropertyMetadataOptions.Inherits, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    ((PlaceholderTextBox)s)?.OnPlaceholderFontFamilyChanged(e.OldValue as FontFamily, e.NewValue as FontFamily);
                }));

        #endregion

        #region DependencyProperty : IsTextEmpty

        public bool IsTextEmpty
        {
            get => (bool)GetValue(IsTextEmptyProperty);
            private set => SetValue(IsTextEmptyPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsTextEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsTextEmpty), typeof(bool), 
                typeof(PlaceholderTextBox), new PropertyMetadata(true));

        public static readonly DependencyProperty IsTextEmptyProperty = IsTextEmptyPropertyKey.DependencyProperty;

        #endregion

        #region DependencyProperty : PlaceholderOpacity

        public double PlaceholderOpacity
        {
            get => (double)GetValue(PlaceholderOpacityProperty);
            set => SetValue(PlaceholderOpacityProperty, value);
        }

        public static readonly DependencyProperty PlaceholderOpacityProperty =
            DependencyProperty.Register(nameof(PlaceholderOpacity), typeof(double),
                typeof(PlaceholderTextBox), new PropertyMetadata(default));

        #endregion

        #region DependencyProperty : PlaceholderHorizontalAlignment

        public HorizontalAlignment PlaceholderHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(PlaceholderHorizontalAlignmentProperty);
            set => SetValue(PlaceholderHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty PlaceholderHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(PlaceholderHorizontalAlignment), typeof(HorizontalAlignment),
                typeof(PlaceholderTextBox), new PropertyMetadata(default));

        #endregion

        #region DependencyProperty : PlaceholderHorizontalAlignment

        public VerticalAlignment PlaceholderVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(PlaceholderVerticalAlignmentProperty);
            set => SetValue(PlaceholderVerticalAlignmentProperty, value);
        }

        public static readonly DependencyProperty PlaceholderVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(PlaceholderVerticalAlignment), typeof(VerticalAlignment),
                typeof(PlaceholderTextBox), new PropertyMetadata(default));

        #endregion

        protected virtual void OnPlaceholderFontSizeChanged(double oldValue, double newValue)
        {
        }

        protected virtual void OnPlaceholderFontFamilyChanged(FontFamily? fontFamily1, FontFamily? fontFamily2)
        {
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            IsTextEmpty = string.IsNullOrWhiteSpace(Text);
        }
    }
}
