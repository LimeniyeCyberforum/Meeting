using Meeting.Business.Common.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;

namespace Meeting.Wpf.ViewModels
{
    public class ConnectViewModel : ReactiveObject
    {
        private readonly IMeetingAuthorization _meetingAuthorization;

        [Reactive]
        public string? Name { get; set; }

        [ObservableAsProperty]
        public bool? IsValidName { get; set; }

        #region JoinCommand

        public ReactiveCommand<Unit, Unit> JoinCommand => 
            ReactiveCommand.CreateFromTask(OnJoinExecute, 
                this.WhenAnyValue(x => x.Name)
                .Select(x => !string.IsNullOrWhiteSpace(Name) && IsValidName == true));

        private async Task OnJoinExecute() =>
            await _meetingAuthorization.JoinToLobbyAsync(Name);

        #endregion

        public ConnectViewModel(IMeetingAuthorization meetingAuthorization)
        {
            _meetingAuthorization = meetingAuthorization;

            this.WhenAnyValue(x => x.Name)
                .Select(x => string.IsNullOrWhiteSpace(Name) ? null : (bool?)!_meetingAuthorization.IsNameExists(Name))
                .ToPropertyEx(this, x => x.IsValidName);
        }
    }
}
