using System.Collections.Generic;
using AclManager.Server.Enums;
using AclManager.Server.Extensions;
using AclManager.Server.Helpers;
using CitizenFX.Core;

namespace AclManager.Server.Handlers
{
    internal class AclCommandHandler
    {
        private readonly AclHandler _aclHandler;

        public AclCommandHandler(AclHandler aclHandler)
        {
            _aclHandler = aclHandler;
        }

        public void HandleCommand(Player source, string command, List<string> args)
        {
            switch (command.ToLower())
            {
                case "grant":
                    AddAce(source, args);
                    break;
                case "revoke":
                    RemoveAce(source, args);
                    break;
                case "inherit":
                    AddPrincipal(source, args);
                    break;
                case "disinherit":
                    RemovePrincipal(source, args);
                    break;
                default:
                    source.SendReply(ReplyType.Error, $"Invalid command: {command}");
                    break;
            }
        }

        private async void AddAce(Player source, List<string> args)
        {
            if (args.Count < 3)
            {
                source.SendReply(ReplyType.Error, "Usage: acl grant <principal> <object> <type>");
                return;
            }

            var principal = args[0];
            var aceObject = args[1];
            var type = args[2];

            if (string.IsNullOrEmpty(principal))
            {
                source.SendReply(ReplyType.Error, "No principal specified");
                return;
            }

            if (string.IsNullOrEmpty(aceObject))
            {
                source.SendReply(ReplyType.Error, "No object specified");
                return;
            }

            if (!AccessControl.CanGrantAce(aceObject))
            {
                source.SendReply(ReplyType.Warning, $"You can not grant the permission '{aceObject}'");
                return;
            }

            if (type != "allow" && type != "deny")
            {
                source.SendReply(ReplyType.Error, "Type must be allow or deny");
                return;
            }

            await _aclHandler.AddAce(new (principal, aceObject, type));

            source.SendReply(ReplyType.Success, "Saved access control entry");
        }

        private async void RemoveAce(Player source, List<string> args)
        {
            if (args.Count < 3)
            {
                source.SendReply(ReplyType.Error, "Usage: acl revoke <principal> <object> <type>");
                return;
            }

            var principal = args[0];
            var aceObject = args[1];
            var type = args[2];

            if (string.IsNullOrEmpty(principal))
            {
                source.SendReply(ReplyType.Error, "No principal specified");
                return;
            }

            if (string.IsNullOrEmpty(aceObject))
            {
                source.SendReply(ReplyType.Error, "No object specified");
                return;
            }

            if (!AccessControl.CanRevokeAce(aceObject))
            {
                source.SendReply(ReplyType.Warning, $"You can not revoke the permission '{aceObject}'");
                return;
            }

            if (type != "allow" && type != "deny")
            {
                source.SendReply(ReplyType.Error, "Type must be allow or deny");
                return;
            }

            await _aclHandler.RemoveAce(new (principal, aceObject, type));

            source.SendReply(ReplyType.Success, "Removed access control entry");
        }

        private async void AddPrincipal(Player source, List<string> args)
        {
            if (args.Count < 2)
            {
                source.SendReply(ReplyType.Error, "Usage: acl inherit <child> <parent>");
                return;
            }

            var child = args[0];
            var parent = args[1];

            if (string.IsNullOrEmpty(child))
            {
                source.SendReply(ReplyType.Error, "No child principal specified");
                return;
            }

            if (string.IsNullOrEmpty(parent))
            {
                source.SendReply(ReplyType.Error, "No parent principal specified");
                return;
            }

            if (!AccessControl.CanInheritFromPrincipal(parent))
            {
                source.SendReply(ReplyType.Warning, $"You can not grant the principal '{parent}'");
                return;
            }

            await _aclHandler.AddPrincipal(new (child, parent));

            source.SendReply(ReplyType.Success, "Saved principal inheritance entry");
        }

        private async void RemovePrincipal(Player source, List<string> args)
        {
            if (args.Count < 2)
            {
                source.SendReply(ReplyType.Error, "Usage: acl disinherit <child> <parent>");
                return;
            }

            var child = args[0];
            var parent = args[1];

            if (string.IsNullOrEmpty(child))
            {
                source.SendReply(ReplyType.Error, "No child principal specified");
                return;
            }

            if (string.IsNullOrEmpty(parent))
            {
                source.SendReply(ReplyType.Error, "No parent principal specified");
                return;
            }

            if (!AccessControl.CanDisinheritFromPrincipal(parent))
            {
                source.SendReply(ReplyType.Warning, $"You can not revoke the principal '{parent}'");
                return;
            }

            await _aclHandler.RemovePrincipal(new (child, parent));

            source.SendReply(ReplyType.Success, "Removed principal inheritance entry");
        }
    }
}
