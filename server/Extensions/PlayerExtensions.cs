using System.Collections.Generic;
using AclManager.Server.Enums;
using AclManager.Server.Helpers;
using CfxUtils.Logging;
using CfxUtils.Wrappers.Chat;
using CitizenFX.Core;

namespace AclManager.Server.Extensions
{
    internal static class PlayerExtensions
    {
        private static Dictionary<ReplyType, string> _replyColors = new()
        {
            {ReplyType.Error, LogColors.Red },
            {ReplyType.Warning,  LogColors.Yellow },
            {ReplyType.Success, LogColors.Green },
        };

        public static void SendReply(this Player source, ReplyType type, string message)
        {
            if (int.Parse(source.Handle) > 0)
            {
                source.AddMessage(new ChatMessage
                {
                    Author = type.ToString(),
                    Message = message,
                    Template = $"<span style=\"padding: 0px 8px; background-color: {_replyColors[type]}; font-size: 100%; border-radius: 12px;\">{{0}}</span> {{1}}"
                });
            }
            else
            {
                Console.SendReply(type, message);
            }
        }
    }

    
}
