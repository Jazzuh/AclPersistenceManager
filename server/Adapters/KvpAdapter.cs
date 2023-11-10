using System.Collections.Generic;
using System.Threading.Tasks;
using AclManager.Server.Enumerators;
using AclManager.Server.Objects;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;
using Console = AclManager.Server.Helpers.Console;

/*
 * Kvp schema:
 *
 *  acl:object:<id>: [id, principal, aceObject, allowType]
 *  acl:inheritance:<id>: [id, childPrincipal, parentPrincipal]
 *
 *  object_next_id: <id>
 *  inheritance_next_id: <id>
 */

namespace AclManager.Server.Adapters
{
    internal class KvpAdapter : IDatabaseAdapter
    {
        public Task<List<Ace>> GetAllAces()
        {
            Console.WriteDebug("Getting ace objects");
            
            var aces = new List<Ace>();
            var objectKeys = new KeyEnumerator("acl:object:");

            foreach (var key in objectKeys)
            {
                var ace = JsonConvert.DeserializeObject<Ace>(GetResourceKvpString(key));

                aces.Add(ace);
            }

            Console.WriteDebug("Done getting ace objects");

            return Task.FromResult(aces);
        }

        public Task<List<PrincipalInheritance>> GetAllPrincipals()
        {
            Console.WriteDebug("Getting principal inheritance");

            var principals = new List<PrincipalInheritance>();
            var objectKeys = new KeyEnumerator("acl:inheritance:");

            foreach (var key in objectKeys)
            {
                var principal = JsonConvert.DeserializeObject<PrincipalInheritance>(GetResourceKvpString(key));

                principals.Add(principal);
            }

            Console.WriteDebug("Done getting principal inheritance");

            return Task.FromResult(principals);
        }

        public Task SaveAces(IList<Ace> aces)
        {
            Console.WriteDebug($"Saving {aces.Count} aces");

            foreach (var ace in aces)
            {
                if (ace is not { Id: 0 })
                {
                    continue;
                }

                var objectId = getNextObjectId();
                ace.Id = objectId;

                Console.WriteDebug($"Saving ace: {ace}");

                var key = $"acl:object:{objectId}";
                var value = JsonConvert.SerializeObject(ace);

                SetResourceKvpNoSync(key, value);
            }

            FlushResourceKvp();

            Console.WriteDebug($"Done saving {aces.Count} aces");

            return Task.FromResult(0);
        }

        public Task DeleteAces(IList<Ace> aces)
        {
            Console.WriteDebug($"Deleting {aces.Count} aces");

            foreach (var ace in aces)
            {
                if (ace is not { Id: > 0 })
                {
                    continue;
                }

                Console.WriteDebug($"Deleting ace: {ace}");

                DeleteResourceKvpNoSync($"acl:object:{ace.Id}");
            }

            FlushResourceKvp();

            Console.WriteDebug($"Done deleting {aces.Count} aces");

            return Task.FromResult(0);
        }

        public Task SavePrincipals(IList<PrincipalInheritance> principals)
        {
            Console.WriteDebug($"Saving {principals.Count} principals");

            foreach (var principal in principals)
            {
                if (principal is not { Id: 0 })
                {
                    continue;
                }

                var inheritanceId = getNextInheritanceId();
                principal.Id = inheritanceId;

                Console.WriteDebug($"Saving principal: {principal}");

                var key = $"acl:inheritance:{inheritanceId}";
                var value = JsonConvert.SerializeObject(principal);

                SetResourceKvpNoSync(key, value);
            }

            FlushResourceKvp();

            Console.WriteDebug($"Done saving {principals.Count} principals");

            return Task.FromResult(0);
        }

        public Task DeletePrincipals(IList<PrincipalInheritance> principals)
        {
            Console.WriteDebug($"Deleting {principals.Count} principals");

            foreach (var principal in principals)
            {
                if (principal is not { Id: > 0 })
                {
                    continue;
                }

                Console.WriteDebug($"Deleting principal: {principal}");

                DeleteResourceKvpNoSync($"acl:inheritance:{principal.Id}");
            }

            FlushResourceKvp();

            Console.WriteDebug($"Done deleting {principals.Count} principals");

            return Task.FromResult(0);
        }


        private int getNextObjectId()
        {
            return getNextId("object_next_id");
        }

        private int getNextInheritanceId()
        {
            return getNextId("inheritance_next_id");
        }

        private int getNextId(string idType)
        {
            var nextId = GetResourceKvpInt(idType);
            nextId += 1;

            SetResourceKvpIntNoSync(idType, nextId);

            return nextId;
        }
    }
}
