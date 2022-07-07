using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.DataTypes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UsersClient = MeetingProtobuf.Protos.Users.UsersClient;

namespace Meeting.Business.GrpcClient
{
    public class UsersService : UsersServiceAbstract
    {
        private CancellationTokenSource _usersSubscribeCancellationToken;

        private readonly UsersClient _usersClient;

        public UsersService(UsersClient usersClient)
            : base()
        {
            _usersClient = usersClient;
        }

        public override Task UsersSubscribeAsync()
        {
            if (_usersSubscribeCancellationToken is not null && !_usersSubscribeCancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            var call = _usersClient.UsersSubscribe(new Empty());
            _usersSubscribeCancellationToken = new CancellationTokenSource();

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    UserDto user = new UserDto(Guid.Parse(x.User.UserGuid), x.User.Name);

                    switch (x.Action)
                    {
                        case MeetingProtobuf.Protos.Action.Added:
                            SmartRaiseUsersChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Added, user);
                            break;
                        case MeetingProtobuf.Protos.Action.Removed:
                            SmartRaiseUsersChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Removed, user);
                            break;
                        case MeetingProtobuf.Protos.Action.Changed:
                            SmartRaiseUsersChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Changed, user);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }, _usersSubscribeCancellationToken.Token);
        }

        public override void UsersUnsubscribe()
        {
            _usersSubscribeCancellationToken.Cancel();
            _usersSubscribeCancellationToken.Dispose();
        }
    }
}
