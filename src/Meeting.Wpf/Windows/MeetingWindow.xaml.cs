using System;
using System.Windows;
using System.Windows.Controls;

namespace Meeting.Wpf
{
    public partial class MeetingWindow : Window
    {
        public MeetingWindow()
        {
            InitializeComponent();
            ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            var exitButton = (Button)Template.FindName("ExitButton", this);
            if (exitButton != null) 
                exitButton.Click += OnExitButtonClicked;
        }

        private void OnExitButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}