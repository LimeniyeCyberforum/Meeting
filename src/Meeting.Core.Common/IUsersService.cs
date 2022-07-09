using Framework.EventArgs;
using Meeting.Core.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Core.Common
{
    public interface IUsersService : IDisposable
    {
        public IReadOnlyDictionary<Guid, UserDto> Users { get; }

        public event EventHandler<NotifyDictionaryChangedEventArgs<Guid, UserDto>> UsersChanged;

        Task UsersSubscribeAsync();

        void UsersUnsubscribe();
    }
}
