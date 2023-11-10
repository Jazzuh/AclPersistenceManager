using System.Collections.Generic;
using System.Threading.Tasks;
using AclManager.Server.Objects;

namespace AclManager.Server.Adapters
{
    internal interface IDatabaseAdapter
    {
        /// <summary>
        /// Gets all <see cref="Ace"/>s from the database
        /// </summary>
        /// <returns>A task that can be awaited to get all of the available aces</returns>
        public Task<List<Ace>> GetAllAces();

        /// <summary>
        /// Gets all <see cref="PrincipalInheritance"/>s from the database
        /// </summary>
        /// <returns>A task that can be awaited to get all of the available principals</returns>
        public Task<List<PrincipalInheritance>> GetAllPrincipals();

        /// <summary>
        /// Saves the specified <see cref="Ace"/>s to the database
        /// </summary>
        /// <param name="aces">The aces to save to the database</param>
        /// <returns>An awaitable task</returns>
        public Task SaveAces(IList<Ace> aces);

        /// <summary>
        /// Deletes the specified <see cref="Ace"/>s to the database
        /// </summary>
        /// <param name="aces">The aces to deletes from the database</param>
        /// <returns>An awaitable task</returns>
        public Task DeleteAces(IList<Ace> aces);

        /// <summary>
        /// Saves the specified <see cref="PrincipalInheritance"/>s to the database
        /// </summary>
        /// <param name="principals">The principals to save to the database</param>
        /// <returns>An awaitable task</returns>
        public Task SavePrincipals(IList<PrincipalInheritance> principals);

        /// <summary>
        /// Deletes the specified <see cref="PrincipalInheritance"/>s to the database
        /// </summary>
        /// <param name="principals">The principals to delete from the database</param>
        /// <returns>An awaitable task</returns>
        public Task DeletePrincipals(IList<PrincipalInheritance> principals);
    }
}
