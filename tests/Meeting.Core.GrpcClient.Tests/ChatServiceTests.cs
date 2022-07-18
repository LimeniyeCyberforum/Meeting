using Google.Protobuf.WellKnownTypes;
using Meeting.Core.GrpcClient.Tests.Models;
using Meeting.Core.GrpcClient.Tests.Utils;
using MeetingProtobuf.Protos;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static MeetingProtobuf.Protos.Chat;

namespace Meeting.Core.GrpcClient.Tests
{
    public class ChatServiceTests
    {
        [Fact]
        public async Task ChatSubscribeAsyncTest()
        {
            //Arrange
            var chatMockClient = new Mock<ChatClient>();

            List<LobbyMessageResponse> lobbyMessagesResponse =
                new List<LobbyMessageResponse>
                {
                    new LobbyMessageResponse
                    {
                        Action = MeetingProtobuf.Protos.Action.Added,
                        LobbyMessage = new LobbyMessage
                        {
                            Message = "HelloWorld!",
                            MessageGuid = Guid.NewGuid().ToString(),
                            Time = DateTime.UtcNow.ToTimestamp(),
                            UserGuid = Guid.NewGuid().ToString(),
                            Username = "Elliot"
                        }
                    }
                };

            var reader = new AsyncStreamReader<LobbyMessageResponse>(lobbyMessagesResponse);

            var fakeCall = CallHelpers.CreateAsyncServerStreamingCall(reader);


            chatMockClient.Setup(m => m.MessagesSubscribe(new Empty(), null, null, CancellationToken.None))
                .Returns(fakeCall);

            // Act
            ChatService service = new ChatService(chatMockClient.Object);

            await service.ChatSubscribeAsync();

            // Assert
            Assert.True(service.Messages.Count > 0);

            service.Dispose();
        }
    }
}
