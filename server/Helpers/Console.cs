using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using AclManager.Server.Enums;
using CfxUtils.Logging;
using Debug = CitizenFX.Core.Debug;

namespace AclManager.Server.Helpers
{
    internal static class Console
    {
        private static Dictionary<ReplyType, string> _replyColors = new()
        {
            {ReplyType.Error, LogColors.Red },
            {ReplyType.Warning, LogColors.Yellow },
            {ReplyType.Success, LogColors.Green },
        };

        [Conditional("DEBUG")]
        public static void WriteDebug(string message, [CallerMemberName] string callingMember = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var locationString = $"{fileName.Split('\\').Last().Replace(".cs", "")}/{callingMember}:{lineNumber}";

            Debug.WriteLine($"{locationString}: {message}");
        }

        public static void SendReply(ReplyType type, string message) 
        {
            Debug.WriteLine($"{_replyColors[type]}{type}:^7 {message}");
        }
    }
}
