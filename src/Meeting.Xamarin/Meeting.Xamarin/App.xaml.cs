using Meeting.Business.GrpcClient;
using Meeting.Xamarin.Pages;
using Xamarin.Forms;

namespace Meeting.Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MeetingPage()
            {
                BindingContext = new MeetingViewModel(new MeetingService())
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
