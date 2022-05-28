using System.Windows;
using System.Windows.Controls;

namespace MeetingWPF.Chat
{
    public class MessageItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnerMessage { get; set; }
        public DataTemplate AnotherMessage { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is null)
                throw new System.NullReferenceException("Element is null. [SelectTemplate]");

            return SelectTemplateCore(item);
        }

        private DataTemplate SelectTemplateCore(object item)
        {
            if (item is OwnMessage)
            {
                return OwnerMessage;
            }
            else if(item is Message)
            {
                return AnotherMessage;
            }

            throw new System.NotImplementedException($"Uknown DataTemplate parameter. \nType: {nameof(MessageItemDataTemplateSelector)}\nParametr: {item}");
        }
    }
}
