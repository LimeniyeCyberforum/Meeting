using GrpcCommon;
using MeetingCommon.DataTypes;
using System;

namespace MeetingGrpcClient
{
    internal static class TypeExtensions
    {
        public static MessageDto ToMessageDto(this MessageFromLobby messageFromLobby)
        {
            if (!Guid.TryParse(messageFromLobby.MessageGuid, out Guid messageGuid))
                throw new ArgumentException($"Lobby MessageGuid {messageFromLobby.MessageGuid} is not Guid.");

            if (!Guid.TryParse(messageFromLobby.UserGuid, out Guid userGuid))
                throw new ArgumentException($"Lobby UserGuid {messageFromLobby.UserGuid} is not Guid.");

            return new MessageDto(messageGuid, userGuid, messageFromLobby.Message, messageFromLobby.Username, messageFromLobby.Time.ToDateTime());
        }
    }
}
