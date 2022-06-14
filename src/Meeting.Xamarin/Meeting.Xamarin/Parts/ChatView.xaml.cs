using Meeting.Business.Common.DataTypes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Meeting.Xamarin.Parts
{
    public class MessageItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnerMessage { get; set; }
        public DataTemplate AnotherMessage { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is null)
                throw new System.NullReferenceException("Element is null. [SelectTemplate]");

            return SelectTemplateCore(item);
        }

        private DataTemplate SelectTemplateCore(object item)
        {
            if (item is OwnerMessageDto)
            {
                return OwnerMessage;
            }
            else if (item is MessageDto)
            {
                return AnotherMessage;
            }

            throw new System.NotImplementedException($"Uknown DataTemplate parameter. \nType: {nameof(MessageItemDataTemplateSelector)}\nParametr: {item}");
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatView : Grid
    {
        public ChatView()
        {
            InitializeComponent();
        }
    }
}