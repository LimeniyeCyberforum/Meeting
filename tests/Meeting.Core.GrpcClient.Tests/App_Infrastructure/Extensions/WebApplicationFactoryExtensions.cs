using Meeting.Core.GrpcClient.Tests.App_Infrastructure.DelegatingHandlers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using System.Net;
using System.Net.Http;

namespace Meeting.Core.GrpcClient.Tests.App_Infrastructure.Extensions
{
    public static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateClientForGrpc<TEntryPoint>(this WebApplicationFactory<TEntryPoint> webApplicationFactory) where TEntryPoint : class
        {
            return webApplicationFactory.CreateDefaultClient(new DelegatingHandler[]
            {
                new CookieContainerHandler(),
                new OverrideResponseHttpVersionHandler(HttpVersion.Version20)
            });
        }
    }
}
