using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Testing;
using MeetingProtobuf.Protos;
using Moq;
using Xunit;
using static MeetingProtobuf.Protos.Authorization;

namespace Meeting.Core.GrpcClient.Tests
{
    public sealed class MeetingServiceTests
    {
        [Fact]
        public void Connection_Test()
        {
            // Arrange
            var mockClient = new Mock<AuthorizationClient>();


            // Use a factory method provided by Grpc.Core.Testing.TestCalls to create an instance of a call.
            var fakeCall = TestCalls.AsyncUnaryCall(Task.FromResult(new ConnectResponse()),
                Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { });

            mockClient.Setup(m => m.ConnectAsync(It.IsAny<ConnectRequest>(), null, null, CancellationToken.None))
                      .Returns(fakeCall);


            var test = mockClient.Object.ConnectAsync(new ConnectRequest() { Username = "limpopo" });

            //// Assert
            Assert.Same(fakeCall, test);

            //var worker = new Worker(mockClient.Object, mockRepository.Object);

            //// Act
            //await worker.StartAsync(CancellationToken.None);

            //// Assert
            //mockRepository.Verify(v => v.SaveGreeting("Test"));
        }
    }
}
