using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.DataTypes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UsersClient = MeetingGrpc.Protos.Users.UsersClient;

namespace Meeting.Business.GrpcClient
{
    public class UsersService : UsersServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly UsersClient _usersClient;

        public UsersService(UsersClient usersClient)
            : base()
        {
            _usersClient = usersClient;
        }

        public override Task UsersSubscribeAsync()
        {
            var call = _usersClient.UsersSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    UserDto user = new UserDto(Guid.Parse(x.User.UserGuid), x.User.Name);

                    switch (x.Action)
                    {
                        case MeetingGrpc.Protos.Action.Added:
                            SmartRaiseUsersChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Added, user);
                            break;
                        case MeetingGrpc.Protos.Action.Removed:
                            SmartRaiseUsersChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Removed, user);
                            break;
                        case MeetingGrpc.Protos.Action.Changed:
                            SmartRaiseUsersChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Changed, user);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }, chatCancelationToken.Token);
        }

        public override void UsersUnsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
