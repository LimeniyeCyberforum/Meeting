using GrpsServer.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.ComponentModel.Composition;
using System.Net;

namespace GrpsServer.Services
{
    [Export(typeof(IService))]
    internal class ChatGrpcServer : IService
    {
        [Import]
        private Logger logger = null;

        [Import]
        private MeetingService m_service = null;

        private WebApplication m_app;

        public void Start()
        {
            // See
            // https://docs.microsoft.com/en-us/aspnet/core/grpc/aspnetcore
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints

            var builder = WebApplication.CreateBuilder();

            // Add services to the container.
            builder.Services.AddGrpc((options) =>
            {
                // See https://docs.microsoft.com/en-us/aspnet/core/grpc/interceptors#server-interceptors
                options.Interceptors.Add<IpAddressAuthenticator>();
            });

            // See https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes
            builder.Services.AddSingleton(m_service);
            builder.Services.AddSingleton(new IpAddressAuthenticator());

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Listen(IPAddress.Any, 7129, (listenOptions) =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;

                    // HTTPS is recommended
                    //listenOptions.UseHttps(@"C:\localhost_server.pfx", "password");
                });
            });

            m_app = builder.Build();

            m_app.MapGrpcService<MeetingService>();

            m_app.RunAsync();
            logger.Info("Started.");
        }
    }
}
