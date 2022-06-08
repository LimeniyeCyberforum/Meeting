using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Meeting.Xamarin.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeetingPage : ContentPage
    {
        public MeetingPage()
        {
            InitializeComponent();
            ResourcesHelper.SetAcrylic(true);
        }
    }
}