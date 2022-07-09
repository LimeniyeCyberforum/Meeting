using Meeting.Core.Common;
using Meeting.Core.GrpcClient;
using Meeting.Wpf.ViewModels;
using Prism.Ioc;

namespace Meeting.Wpf
{
    public static class Startup
    {
        public static void ConfigurationServices(this IContainerRegistry services)
        {
            services.RegisterSingleton<IMeetingService>(() => MeetingService.Instance);
            services.Add<MeetingWindow, MeetingViewModel>();
        }

        private static void Add<TView, TViewModel>(this IContainerRegistry containerRegistry, string? name = null)
        {
            containerRegistry.RegisterForNavigation<TView, TViewModel>(name);
        }
    }
}
