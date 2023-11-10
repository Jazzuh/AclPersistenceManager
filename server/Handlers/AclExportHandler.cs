using System.Collections.Generic;
using AclManager.Server.Enums;
using AclManager.Server.Helpers;
using AclManager.Server.Objects;
using static CitizenFX.Core.Native.API;
using Console = AclManager.Server.Helpers.Console;

namespace AclManager.Server.Handlers
{
    internal class AclExportHandler
    {
        private AclHandler _aclHandler;
        
        public AclExportHandler(AclHandler aclHandler)
        {
            _aclHandler = aclHandler;
        }

        public bool HasLoaded()
        {
            return _aclHandler.HasLoaded;
        }

        public void AddAceExport(string principal, string aceObject, string type)
        {
            AddAcesExport(new ()
            {
                new List<object> { principal, aceObject, type }
            });
        }

        public async void AddAcesExport(List<object> aces)
        {
            var acesToAdd = new List<Ace>();
            var resource = GetInvokingResource();

            foreach (var aceList in aces)
            {
                if (aceList is not List<object> aceData)
                {
                    continue;
                }

                if (aceData.Count != 3)
                {
                    continue;
                }

                var principal = aceData[0].ToString();
                var aceObject = aceData[1].ToString();
                var allowType = aceData[2].ToString();

                if (!AccessControl.CanGrantAce(aceObject) && !AccessControl.CanGrantAce($"resource.{resource}", aceObject))
                {
                    Console.SendReply(ReplyType.Warning, $"You can not grant the permission '{aceObject}'");
                    return;
                }

                acesToAdd.Add(new (principal, aceObject, allowType));
            }

            await _aclHandler.AddAces(acesToAdd);
        }

        public void RemoveAceExport(string principal, string aceObject, string type)
        {
            RemoveAcesExport(new()
            {
                new List<object> { principal, aceObject, type }
            });
        }

        public async void RemoveAcesExport(List<object> aces)
        {
            var acesToRemove = new List<Ace>();
            var resource = GetInvokingResource();

            foreach (var aceList in aces)
            {
                if (aceList is not List<object> aceData)
                {
                    continue;
                }

                if (aceData.Count != 3)
                {
                    continue;
                }

                var principal = aceData[0].ToString();
                var aceObject = aceData[1].ToString();
                var allowType = aceData[2].ToString();

                if (!AccessControl.CanRevokeAce(aceObject) && !AccessControl.CanRevokeAce($"resource.{resource}", aceObject))
                {
                    Console.SendReply(ReplyType.Warning, $"You can not revoke the permission '{aceObject}'");
                    return;
                }

                var ace = _aclHandler.GetAce(principal, aceObject, allowType);

                if (ace == null)
                {
                    continue;
                }

                acesToRemove.Add(ace);
            }

            await _aclHandler.RemoveAces(acesToRemove);
        }

        public void AddPrincipalExport(string child, string parent)
        {
            AddPrincipalsExport(new()
            {
                new List<object> { child, parent }
            });
        }

        public async void AddPrincipalsExport(List<object> principals)
        {
            var principalsToAdd = new List<PrincipalInheritance>();
            var resource = GetInvokingResource();

            foreach (var principalList in principals)
            {
                if (principalList is not List<object> principalData)
                {
                    continue;
                }

                if (principalData.Count != 2)
                {
                    continue;
                }

                var child = principalData[0].ToString();
                var parent = principalData[1].ToString();

                if (!AccessControl.CanInheritFromPrincipal(child) && !AccessControl.CanInheritFromPrincipal($"resource.{resource}", child))
                {
                    Console.SendReply(ReplyType.Warning, $"You can not grant the principal '{child}'");
                    return;
                }

                principalsToAdd.Add(new (child, parent));
            }

            await _aclHandler.AddPrincipals(principalsToAdd);
        }

        public void RemovePrincipalExport(string child, string parent)
        {
            RemovePrincipalsExport(new()
            {
                new List<object> { child, parent }
            });
        }

        public async void RemovePrincipalsExport(List<object> principals)
        {
            var principalsToRemove = new List<PrincipalInheritance>();
            var resource = GetInvokingResource();

            foreach (var principalList in principals)
            {
                if (principalList is not List<object> principalData)
                {
                    continue;
                }

                if (principalData.Count != 2)
                {
                    continue;
                }

                var child = principalData[0].ToString();
                var parent = principalData[1].ToString();

                if (!AccessControl.CanDisinheritFromPrincipal(child) && !AccessControl.CanDisinheritFromPrincipal($"resource.{resource}", child))
                {
                    Console.SendReply(ReplyType.Warning, $"You can not revoke the principal '{child}'");
                    return;
                }

                var principal = _aclHandler.GetPrincipal(child, parent);

                if (principal == null)
                {
                    continue;
                }

                principalsToRemove.Add(principal);
            }

            await _aclHandler.RemovePrincipals(principalsToRemove);
        }
    }
}
