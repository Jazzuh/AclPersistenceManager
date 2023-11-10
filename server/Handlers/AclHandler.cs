using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AclManager.Server.Adapters;
using AclManager.Server.Objects;
using CitizenFX.Core;
using Console = AclManager.Server.Helpers.Console;

namespace AclManager.Server.Handlers
{
    internal class AclHandler
    {
        /// <summary>
        /// Gets all the currently cached <see cref="Ace"/>s
        /// </summary>
        public IReadOnlyList<Ace> Aces => _aces.ToList();

        /// <summary>
        /// Gets all the currently cached <see cref="PrincipalInheritance"/>s
        /// </summary>
        public IReadOnlyList<PrincipalInheritance> Principals => _principals.ToList();

        /// <summary>
        /// Gets or sets if this ACL has been loaded from the database
        /// </summary>
        public bool HasLoaded { get; private set; }

        private IDatabaseAdapter _dbAdapter;

        private List<Ace> _aces;
        private List<PrincipalInheritance> _principals;

        public AclHandler(IDatabaseAdapter dbAdapter)
        {
            _dbAdapter = dbAdapter;

            _aces = new ();
            _principals = new ();
        }

        /// <summary>
        /// Gets a cached access control entry, if it exists
        /// </summary>
        /// <param name="principal">The principal to add the access control entry to</param>
        /// <param name="aceObject">The access control entry being added to the principal</param>
        /// <param name="allowType">The allow type of the entry</param>
        /// <returns>The stored <see cref="Ace"/>, or <see langword="null"/></returns>
        public Ace GetAce(string principal, string aceObject, string allowType)
        {
            if (string.IsNullOrEmpty(principal) || string.IsNullOrEmpty(aceObject) || string.IsNullOrEmpty(allowType))
            {
                return null;
            }

            return _aces.FirstOrDefault(ace => ace.Principal == principal && ace.Object == aceObject && ace.AllowType == allowType);
        }

        /// <summary>
        /// Checks if the specified <see cref="Ace"/> exists in the ACL
        /// </summary>
        /// <param name="ace">The <see cref="Ace"/> to check</param>
        /// <returns>If the <see cref="Ace"/> exists in the ACL</returns>
        public bool DoesAceExist(Ace ace)
        {
            return _aces.Contains(ace);
        }

        /// <summary>
        /// Gets a cached principal inheritance entry, if it exists
        /// </summary>
        /// <param name="child">The child principal</param>
        /// <param name="parent">The parent principal</param>
        /// <returns>The stored <see cref="PrincipalInheritance"/>, or <see langword="null"/></returns>
        public PrincipalInheritance GetPrincipal(string child, string parent)
        {
            if (string.IsNullOrEmpty(child) || string.IsNullOrEmpty(parent))
            {
                return null;
            }

            return _principals.FirstOrDefault(principal => principal.Parent == parent && principal.Child == child);
        }

        /// <summary>
        /// Checks if the specified <see cref="PrincipalInheritance"/> exists in the ACL
        /// </summary>
        /// <param name="principal">The <see cref="PrincipalInheritance"/> to check</param>
        /// <returns>If the <see cref="PrincipalInheritance"/> exists in the ACL</returns>
        public bool DoesPrincipalExist(PrincipalInheritance principal)
        {
            return _principals.Contains(principal);
        }

        /// <summary>
        /// Adds an <see cref="Ace"/> into the database and ACL
        /// </summary>
        /// <param name="ace">The <see cref="Ace"/> to add</param>
        /// <returns>An awaitable task</returns>
        public Task AddAce(Ace ace)
        {
            return AddAces(new() { ace });
        }

        /// <summary>
        /// Adds a list of <see cref="Ace"/>s into the database and ACL
        /// </summary>
        /// <param name="aces">The list of <see cref="Ace"/>s to add</param>
        /// <returns>An awaitable task</returns>
        public async Task AddAces(List<Ace> aces)
        {
            while (!HasLoaded)
            {
                await BaseScript.Delay(0);
            }

            var acesToAdd = new List<Ace>();

            foreach (var ace in aces)
            {
                if (DoesAceExist(ace))
                {
                    Console.WriteDebug($"Ace: {ace}; already exists. Skipping");
                    continue;
                }

                acesToAdd.Add(ace);
            }

            _aces.AddRange(acesToAdd);

            await _dbAdapter.SaveAces(acesToAdd);

            await BaseScript.Delay(0);

            ReloadAcl();
        }

        /// <summary>
        /// Removes an <see cref="Ace"/> from the database and ACL
        /// </summary>
        /// <param name="ace">The <see cref="Ace"/> to remove</param>
        /// <returns>An awaitable task</returns>
        public Task RemoveAce(Ace ace)
        {
            return RemoveAces(new() { ace });
        }

        /// <summary>
        /// Removes a list of <see cref="Ace"/>s from the database and ACL
        /// </summary>
        /// <param name="aces">The <see cref="Ace"/>s to remove</param>
        /// <returns>An awaitable task</returns>
        public async Task RemoveAces(List<Ace> aces)
        {
            while (!HasLoaded)
            {
                await BaseScript.Delay(0);
            }

            var acesToRemove = new List<Ace>();

            foreach (var ace in aces)
            {
                if (!DoesAceExist(ace))
                {
                    Console.WriteDebug($"Ace: {ace}; already exists. Skipping");
                    continue;
                }

                ace.CanBeRemoved = true;

                acesToRemove.Add(ace);
            }

            await _dbAdapter.DeleteAces(acesToRemove);

            await BaseScript.Delay(0);

            ReloadAcl();
        }

        /// <summary>
        /// Adds a <see cref="PrincipalInheritance"/> to the database and ACL
        /// </summary>
        /// <param name="principal">The <see cref="PrincipalInheritance"/> to add</param>
        /// <returns>An awaitable task</returns>
        public Task AddPrincipal(PrincipalInheritance principal)
        {
            return AddPrincipals(new() { principal });
        }

        /// <summary>
        /// Adds a list of <see cref="PrincipalInheritance"/>s to the database and ACL
        /// </summary>
        /// <param name="principals">The <see cref="PrincipalInheritance"/>s to add</param>
        /// <returns>An awaitable task</returns>
        public async Task AddPrincipals(List<PrincipalInheritance> principals)
        {
            while (!HasLoaded)
            {
                await BaseScript.Delay(0);
            }

            var principalsToAdd = new List<PrincipalInheritance>();

            foreach (var principal in principals)
            {
                if (DoesPrincipalExist(principal))
                {
                    Console.WriteDebug($"Principal: {principal}; already exists. Skipping");
                    continue;
                }

                principalsToAdd.Add(principal);
            }

            _principals.AddRange(principalsToAdd);

            await _dbAdapter.SavePrincipals(principalsToAdd);

            await BaseScript.Delay(0);

            ReloadAcl();
        }

        /// <summary>
        /// Removes a <see cref="PrincipalInheritance"/> from the database and ACL
        /// </summary>
        /// <param name="principal">The <see cref="PrincipalInheritance"/> to remove</param>
        /// <returns>An awaitable task</returns>
        public Task RemovePrincipal(PrincipalInheritance principal)
        {
            return RemovePrincipals(new() { principal });
        }

        /// <summary>
        /// Removes a list of <see cref="PrincipalInheritance"/>s from the database and ACL
        /// </summary>
        /// <param name="principals">The <see cref="PrincipalInheritance"/> to remove</param>
        /// <returns>An awaitable task</returns>
        public async Task RemovePrincipals(List<PrincipalInheritance> principals)
        {
            while (!HasLoaded)
            {
                await BaseScript.Delay(0);
            }

            var principalsToRemove = new List<PrincipalInheritance>();

            foreach (var principal in principals)
            {
                if (!DoesPrincipalExist(principal))
                {
                    Console.WriteDebug($"Principal: {principal}; already exists. Skipping");
                    continue;
                }

                principal.CanBeRemoved = true;

                principalsToRemove.Add(principal);
            }

            await _dbAdapter.DeletePrincipals(principalsToRemove);

            await BaseScript.Delay(0);

            ReloadAcl();
        }

        /// <summary>
        /// Loads the ACEs and principal inheritances from the database into the ACL
        /// </summary>
        /// <returns></returns>
        public async Task LoadAcl()
        {
            if (HasLoaded)
            {
                return;
            }

            HasLoaded = true;

            Console.WriteDebug("Loading ACL");

            _aces = await _dbAdapter.GetAllAces();
            _principals = await _dbAdapter.GetAllPrincipals();

            await BaseScript.Delay(0);

            ReloadAcl();

            BaseScript.TriggerEvent("acl:onLoaded");
    
            Console.WriteDebug("Done loading ACL");
        }

        /// <summary>
        /// Reloads the ACEs and principal inheritances into the ACL
        /// </summary>
        public void ReloadAcl()
        {
            Console.WriteDebug("Reloading ACL");

            foreach (var ace in Aces)
            {
                if (ace.CanBeAdded)
                {
                    ace.Add();
                }
                else if (ace.CanBeRemoved)
                {
                    ace.Remove();

                    _aces.Remove(ace);
                }
            }

            foreach (var principal in Principals)
            {
                if (principal.CanBeAdded)
                {
                    principal.Add();
                }
                else if (principal.CanBeRemoved)
                {
                    principal.Remove();

                    _principals.Remove(principal);
                }
            }

            BaseScript.TriggerEvent("acl:onReloaded");

            Console.WriteDebug("Done reloading ACL");
        }

        /// <summary>
        /// Removes all added ACEs and principal inheritances from the ACL
        /// </summary>
        public void UnloadAcl()
        {
            Console.WriteDebug("Unloading ACL");

            foreach (var ace in _aces)
            {
                ace.Remove();
            }

            foreach (var principal in _principals)
            {
                principal.Remove();
            }

            BaseScript.TriggerEvent("acl:onUnloaded");

            Console.WriteDebug("Done unloading ACL");
        }
    }
}
