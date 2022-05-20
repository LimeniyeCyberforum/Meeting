using MeetingCommon.Abstractions.Messanger;
using MeetingGrpcClient;
using Microsoft.Extensions.DependencyInjection;

namespace WPFView
{
    public static class IocService
    {
        private static ServiceProvider _serviceProvider;

        public static ServiceProvider ServiceProvider => _serviceProvider;

        public static void Initialize()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<MessageServiceAbstract, MessageService>()
                .BuildServiceProvider();
        }
    }
}
