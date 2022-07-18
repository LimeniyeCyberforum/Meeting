using System.Threading;
using Moq;
using Xunit;
using static MeetingProtobuf.Protos.Authorization;

namespace Meeting.Tests
{
    //public sealed class MeetingServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory<>>
    //{
    //    [Fact]
    //    public void Connection_Test()
    //    {
    //         Arrange
    //        var mockCall = CallHelpers.CreateAsyncUnaryCall(new ConnectResponse() { IsSuccess = true });
    //        var mockClient = new Mock<AuthorizationClient>();
    //        mockClient.Setup(m => m.ConnectAsync(It.IsAny<ConnectRequest>(), null, null, CancellationToken.None))
    //                  .Returns(mockCall);

    //         Act
    //        var test = mockClient.Object.ConnectAsync(new ConnectRequest() { Username = "limpopo" });

    //         Assert
    //        Assert.Same(mockCall, test);
    //    }
    //}
}
