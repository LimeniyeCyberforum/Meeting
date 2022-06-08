using Meeting.Business.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Xamarin;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Meeting.Xamarin.Parts
{
    public class ConnectViewModel : BaseInpc
    {
        private readonly IMeetingAuthorization _meetingAuthorization;

        private string _name;
        private bool? _isValidName = null;

        public string Name { get => _name; set => Set(ref _name, value); }
        public bool? IsValidName { get => _isValidName; set => Set(ref _isValidName, value); }

        #region JoinCommand
        private AsyncCommand _joinCommand;
        public AsyncCommand JoinCommand => _joinCommand ?? (
            _joinCommand = new AsyncCommand(OnJoinExecute, CanJoinExecute));

        private async Task OnJoinExecute()
        {
            await _meetingAuthorization.JoinToLobbyAsync(Name);
        }

        private bool CanJoinExecute()
        {
            return !(string.IsNullOrWhiteSpace(Name) || IsValidName == false);
        }
        #endregion

        public ConnectViewModel(IMeetingAuthorization meetingAuthorization)
        {
            _meetingAuthorization = meetingAuthorization;
            ProtectedPropertyChanged += OnProtectedPropertyChanged;
        }

        private void OnProtectedPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (string.Equals(propertyName, nameof(Name)))
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    IsValidName = null;
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvokeOnMainThread(() =>
                        IsValidName = !_meetingAuthorization.IsNameExists(Name));
                }
            }
        }
    }
}
