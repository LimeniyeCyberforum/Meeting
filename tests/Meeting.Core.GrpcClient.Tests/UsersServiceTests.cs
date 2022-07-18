using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Meeting.Core.GrpcClient.Tests.Models;
using Meeting.Core.GrpcClient.Tests.Utils;
using MeetingProtobuf.Protos;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UsersClient = MeetingProtobuf.Protos.Users.UsersClient;


namespace Meeting.Core.GrpcClient.Tests
{
    public class UsersServiceTests
    {
        [Fact]
        public async Task UsersSubscribeAsyncTest()
        {
            //Arrange
            var usersMockClient = new Mock<UsersClient>();

            List<UserOnlineStatusResponse> userOnlineStatusResponses = 
                new List<UserOnlineStatusResponse>
                {
                    new UserOnlineStatusResponse { User = new User { Name = "Elliot", UserGuid = Guid.NewGuid().ToString() }, Action = MeetingProtobuf.Protos.Action.Added },
                    new UserOnlineStatusResponse { User = new User { Name = "Ollie", UserGuid = Guid.NewGuid().ToString() }, Action = MeetingProtobuf.Protos.Action.Added }
                };

            var reader = new AsyncStreamReader<UserOnlineStatusResponse>(userOnlineStatusResponses);

            var fakeCall = CallHelpers.CreateAsyncServerStreamingCall(reader);

            usersMockClient.Setup(m => m.UsersSubscribe(new Empty(), null, null, CancellationToken.None))
                .Returns(fakeCall);

            // Act
            UsersService service = new UsersService(usersMockClient.Object);
            
            await service.UsersSubscribeAsync();

            // Assert
            Assert.True(service.Users.Count > 0);

            service.Dispose();
        }
    }
}
