using System.Windows;
using System.Windows.Controls;

namespace MeetingWPF.Controls.PlaceholderTextBox
{
    public class PlaceholderTextBox : TextBox
    {
        static PlaceholderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));
        }

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register(nameof(PlaceHolder), typeof(string), 
                typeof(PlaceholderTextBox), new PropertyMetadata(string.Empty));

        public bool IsTextEmpty
        {
            get => (bool)GetValue(IsTextEmptyProperty);
            private set => SetValue(IsTextEmptyPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsTextEmptyPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsTextEmpty), typeof(bool), typeof(PlaceholderTextBox), new PropertyMetadata(true));

        public static readonly DependencyProperty IsTextEmptyProperty = IsTextEmptyPropertyKey.DependencyProperty;


        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            IsTextEmpty = string.IsNullOrWhiteSpace(Text);
        }
    }
}
