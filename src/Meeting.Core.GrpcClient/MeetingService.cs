using System;
using Grpc.Core;
using Grpc.Net.Client;

using System.Net.Http;
using Xamarin.Essentials;
using Grpc.Net.Client.Web;
using Meeting.Core.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Meeting.Core.GrpcClient.Tests")]
namespace Meeting.Core.GrpcClient
{
    public sealed partial class MeetingService : IMeetingService
    {
        private readonly MetadataRepository metadataRepository = new MetadataRepository();
        private bool disposed = false;

        public IAuthorizationService Authorization { get; private set; }

        public IChatService Chat { get; private set; }

        public ICaptureFramesService CaptureFrames { get; private set; }

        public IUsersService Users { get; private set; }

        private string GetServerAddress()
        {
            var address = "https://3.72.127.66:5010";
#if UseLocalConnect
            address = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:5010" : "https://localhost:5010";
#endif
            return address;
        }

        private GrpcChannel GetGrpcChannel()
        {
            string address = GetServerAddress();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            HttpClient httpClient = null;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);
                httpClient = new HttpClient(grpcWebHandler);
            }
            else
            {
                httpClient = new(httpClientHandler);
            }

            return GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpClient = httpClient,
                Credentials = ChannelCredentials.SecureSsl
            });
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Chat.Dispose();
                CaptureFrames.Dispose();
                Users.Dispose();
            }
            disposed = true;
        }
    }
}
