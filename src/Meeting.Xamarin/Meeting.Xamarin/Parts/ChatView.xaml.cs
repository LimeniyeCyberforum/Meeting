using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Meeting.Xamarin.Parts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatView : Grid
    {
        public ChatView()
        {
            InitializeComponent();
        }
    }
}