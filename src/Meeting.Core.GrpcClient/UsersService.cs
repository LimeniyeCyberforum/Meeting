using Framework.EventArgs;
using Framework.Extensions;
using Google.Protobuf.WellKnownTypes;
using Meeting.Core.Common;
using Meeting.Core.Common.DataTypes;
using Meeting.Core.GrpcClient.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UsersClient = MeetingProtobuf.Protos.Users.UsersClient;

namespace Meeting.Core.GrpcClient
{
    internal sealed partial class UsersService : IUsersService
    {
        private readonly Dictionary<Guid, UserDto> users = new Dictionary<Guid, UserDto>();

        private int messagesChangedSyncNumber = 0;

        private bool disposed = false;

        private CancellationTokenSource _usersSubscribeCancellationToken;

        private readonly UsersClient _usersClient;

        public IReadOnlyDictionary<Guid, UserDto> Users { get; }

        public event EventHandler<NotifyDictionaryChangedEventArgs<Guid, UserDto>> UsersChanged;

        public UsersService(UsersClient usersClient)
        {
            _usersClient = usersClient;
            Users = new ReadOnlyDictionary<Guid, UserDto>(users);
        }

        public Task UsersSubscribeAsync()
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
                    var user = new UserDto(Guid.Parse(x.User.UserGuid), x.User.Name);

                    switch (x.Action)
                    {
                        case MeetingProtobuf.Protos.Action.Added:
                            users.AddAndShout(this, UsersChanged, user.Guid, user, ref messagesChangedSyncNumber);
                            break;
                        case MeetingProtobuf.Protos.Action.Removed:
                            users.RemoveAndShout(this, UsersChanged, user.Guid, ref messagesChangedSyncNumber);
                            break;
                        case MeetingProtobuf.Protos.Action.Changed:
                            users.SetAndShout(this, UsersChanged, user.Guid, user, ref messagesChangedSyncNumber);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }, _usersSubscribeCancellationToken.Token);
        }

        public void UsersUnsubscribe()
        {
            _usersSubscribeCancellationToken.Cancel();
            _usersSubscribeCancellationToken.Dispose();
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
                UsersUnsubscribe();
            }
            disposed = true;
        }
    }
}
