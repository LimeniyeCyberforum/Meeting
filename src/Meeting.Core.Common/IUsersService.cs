using Utils.EventArgs;
using Meeting.Core.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Core.Common
{
    public interface IUsersService : IDisposable
    {
        IReadOnlyDictionary<Guid, UserDto> Users { get; }

        event EventHandler<NotifyDictionaryChangedEventArgs<Guid, UserDto>> UsersChanged;

        Task UsersSubscribeAsync();

        void UsersUnsubscribe();
    }
}
