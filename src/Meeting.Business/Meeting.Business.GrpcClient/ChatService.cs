﻿using Framework.EventArgs;
using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.DataTypes;
using MeetingGrpc.Protos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatClient = MeetingGrpc.Protos.Chat.ChatClient;

namespace Meeting.Business.GrpcClient
{
    public class ChatService : ChatServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly ChatClient _client;

        public ChatService(ChatClient client)
            : base()
        {
            _client = client;
        }

        public override Task ChatSubscribeAsync()
        {
            var call = _client.MessagesSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    MessageDto message = new(Guid.Parse(x.LobbyMessage.MessageGuid), Guid.Parse(x.LobbyMessage.UserGuid), x.LobbyMessage.Username, x.LobbyMessage.MessageGuid, x.LobbyMessage.Time.ToDateTime());
                    switch (x.Action)
                    {
                        case MeetingGrpc.Protos.Action.Added:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Added, message);
                            break;
                        case MeetingGrpc.Protos.Action.Removed:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Removed, message);
                            break;
                        case MeetingGrpc.Protos.Action.Changed:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Changed, message);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }, chatCancelationToken.Token);
        }

        public override Task ChatUnsubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(Guid messageGuid, string message)
        {
            _client.SendMessage(new MessageRequest()
            {
                MessageGuid = messageGuid.ToString(),
                Message = message
            });
        }

        public override async Task SendMessageAsync(Guid messageGuid, string message)
        {
            await _client.SendMessageAsync(new MessageRequest() 
            {
                MessageGuid = messageGuid.ToString(),
                Message = message
            });
        }

        //private AsyncDuplexStreamingCall<MessageRequest, MessageReplay> _call;
        //private AsyncDuplexStreamingCall<CameraCaptureTest, CameraCaptureTest> _call2;
        //private GrpsServer.Chat.MessangerClient _client;

        //public override void SendMessage(Guid guid, string username, string message)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task SendMessageAsync(Guid guid, string username, string message)
        //{
        //    var response = await _client.SendMessageAsync(new MessageRequest() { Username = username, Message = message });
        //}

        //public override async Task SendCameraCaptureAsync(MemoryStream stream)
        //{
        //    try
        //    {
        //        await _call2.RequestStream.WriteAsync(new CameraCaptureTest() { CaptureFrame = ByteString.FromStream(stream) });
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    //await _call.RequestStream.WriteAsync(new MessageRequest() { Username = "limeniye", Message = "fasf" });
        //}

        //private void StreamComplete()
        //{
        //    _call.RequestStream.CompleteAsync();
        //}



        //private async void InitializeStream()
        //{
        //    var httpHandler = new HttpClientHandler();
        //    httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        //    using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
        //    _client = new GrpsServer.Chat.MessangerClient(channel);
        //    _call = _client.MessageStream();
        //    _call2 = _client.CameraCaptureStream();

        //    await Task.Run(async () =>
        //    {
        //        await foreach (var response in _call2.ResponseStream.ReadAllAsync())
        //        {
        //            RaiseCameraCaptureChanged(response.CaptureFrame.ToByteArray());
        //            //System.Diagnostics.Debug.WriteLine(response.CaptureFrame);
        //        }

        //        //await foreach (var response in _call.ResponseStream.ReadAllAsync())
        //        //{
        //        //    RaiseMessagesChangedEvent(Framework.EventArgs.NotifyDictionaryChangedAction.Added, new MessageDto(Guid.NewGuid(), response.Message, response.Username, response.Time.ToDateTime()));
        //        //}
        //    });
        //}
    }
}