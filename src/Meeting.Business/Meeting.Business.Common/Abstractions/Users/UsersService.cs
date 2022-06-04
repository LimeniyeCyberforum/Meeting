using Framework.EventArgs;
using Framework.Extensions;
using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Meeting.Business.Common.Abstractions.Users
{
    public class UsersService
    {
        protected readonly Dictionary<Guid, UserDto> users = new Dictionary<Guid, UserDto>();
        private int messagesChangedSyncNumber = 0;

        public IReadOnlyDictionary<Guid, UserDto> Users { get; }

        public UsersService()
        {
            Users = new ReadOnlyDictionary<Guid, UserDto>(users);
        }

        public event EventHandler<NotifyDictionaryChangedEventArgs<Guid, UserDto>> UsersChanged;


        /// <summary>
        /// The method automaticty changes the dictionary from action and raises notification
        /// </summary>
        protected void SmartRaiseUsersChangedEvent(NotifyDictionaryChangedAction action, UserDto newUser = null, UserDto oldUser = null)
        {
            switch (action)
            {
                case NotifyDictionaryChangedAction.Added:
                    users.AddAndShout(this, UsersChanged, newUser.Guid, newUser, ref messagesChangedSyncNumber);
                    break;
                case NotifyDictionaryChangedAction.Removed:
                    users.RemoveAndShout(this, UsersChanged, newUser.Guid, ref messagesChangedSyncNumber);
                    break;
                case NotifyDictionaryChangedAction.Changed:
                    users.SetAndShout(this, UsersChanged, newUser.Guid, newUser, ref messagesChangedSyncNumber);
                    break;
                case NotifyDictionaryChangedAction.Cleared:
                    users.ClearAndShout(this, UsersChanged, ref messagesChangedSyncNumber);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The method automaticty finding changes and raises notifications
        /// </summary>
        protected void SmartRaiseUsersChangedEvent(IDictionary<Guid, UserDto> newElements)
        {
            users.NewElementsHandler(this, newElements, UsersChanged, ref messagesChangedSyncNumber);
        }
    }
}
