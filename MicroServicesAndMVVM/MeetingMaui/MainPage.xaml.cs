using MeetingCommon.Abstractions;
using MeetingCommon.DataTypes;
using MeetingGrpcClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeetingMaui;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private readonly MeetingServiceAbstract _meetingServiceAbstract;
    private readonly UserDto _currentUser;

    private string _message = "Kek";
    public string Message { get => _message; set => SetPropertyValue(ref _message, value); }

    public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
    {
        new Message(Guid.NewGuid(), "Hello world!", DateTime.UtcNow)
    };

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        _meetingServiceAbstract = new MeetingService();
        _meetingServiceAbstract.ConnectionStateChanged += OnConnectionStateChanged;
        _currentUser = _meetingServiceAbstract.Connect("limeniye_mobile");
    }

    private void OnConnectionStateChanged(object sender, (ConnectionAction Action, MeetingCommon.DataTypes.UserDto User) e)
    {
        _meetingServiceAbstract.MessageService.MessagesChanged += OnMessagesChanged;
        _meetingServiceAbstract.MessageService.ChatSubscribeAsync();
    }

    private void OnMessagesChanged(object sender, Common.EventArgs.NotifyDictionaryChangedEventArgs<Guid, MeetingCommon.DataTypes.MessageDto> e)
    {

    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        var newMessage = new Message(Guid.NewGuid(), Message, null);
        Message = "";
        Messages.Add(newMessage);
        await _meetingServiceAbstract.MessageService.SendMessageAsync(newMessage.Id, _currentUser.Guid, newMessage.Text);
    }

    #region INPC
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (value == null ? field != null : !value.Equals(field))
        {
            field = value;

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
            return true;
        }
        return false;
    }
    #endregion
}

public class Message
{
    public Guid Id { get; }
    public string Text { get; }
    public DateTime? DateTime { get; }

    public Message(Guid id, string text, DateTime? dateTime)
    {
        Id = id;
        Text = text;
        DateTime = dateTime;
    }
}

