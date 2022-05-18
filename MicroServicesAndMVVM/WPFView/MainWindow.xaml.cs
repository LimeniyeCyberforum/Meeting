using System.Windows;
using System.Windows.Controls;

namespace WPFView
{
    public class MessageItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnerMessage { get; set; }
        public DataTemplate AnotherMessage { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is bool isOwner)
            {
                return SelectTemplateCore(isOwner);
            }

            throw new System.NotImplementedException($"Uknown DataTemplate parameter. \nType: {nameof(MessageItemDataTemplateSelector)}\nParametr: {item}");
        }

        private DataTemplate SelectTemplateCore(bool isOwner)
        {
            if (isOwner)
            {
                return OwnerMessage;
            }
            else
            {
                return AnotherMessage;
            }
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();
        }
    }
}
