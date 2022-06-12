using Meeting.Business.Common.Abstractions;
using Meeting.Xamarin.Pages;
using Sharpnado.MaterialFrame;
using Xamarin.Forms;

namespace Meeting.Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Initializer.Initialize(true, true);

            MainPage = new MeetingPage()
            {
                BindingContext = new MeetingViewModel(DependencyService.Get<IMeetingService>())
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
