using System.Threading.Tasks;
using AutoFixture;
using Grpc.Core;
using Meeting.Core.GrpcClient.App_Infrastructure.Tests.Factories;
using MeetingGrpc.Repositories.LocalServices;
using MeetingGrpc.Server.Repositories;
using MeetingGrpc.Server.Repositories.LocalServices;
using MeetingGrpc.Server.Services;
using MeetingProtobuf.Protos;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Meeting.Core.GrpcClient.Tests.Unit
{
    public sealed class MeetingServiceUnitTests
    {
        private readonly Fixture _fixture;

        public MeetingServiceUnitTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task True_If_Authorization_Success()
        {
            // Users service
            // var usersLoggerMock = new Moq.Mock<ILogger<UsersService>>();
            var localUsersServiceLoggerMock = new Moq.Mock<ILogger<LocalUsersService>>();
            var usersRepository = new UsersRepository();
            var localUsersService = new LocalUsersService(localUsersServiceLoggerMock.Object, usersRepository);
            //var usersService = new UsersService(usersLoggerMock.Object, localUsersService);

            // CaptureFrames service
            var captureFramesRepository = new FrameCaptureStatesRepository();
            var localCaptureFramesService = new LocalCaptureFramesService(captureFramesRepository);


            // Arrange
            var loggerMock = new Moq.Mock<ILogger<AuthorizationService>>();
            var contextMock = new Moq.Mock<ServerCallContext>();
            var service = new AuthorizationService(loggerMock.Object, ConfigurationFactory.GetConfiguration(), localUsersService, localCaptureFramesService);
            var request = new ConnectRequest() { Username = "Иван" };
            // Act
            var result = await service.Connect(request, contextMock.Object);
            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
