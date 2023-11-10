using System.Collections.Generic;
using System.Threading.Tasks;
using AclManager.Server.Adapters;
using AclManager.Server.Objects;

namespace AclManager.Server.Extensions
{
    internal static class DatabaseAdapterExtensions
    {
        /// <summary>
        /// Saves the specified <see cref="Ace"/> to the database
        /// </summary>
        /// <param name="adapter">The database adapter</param>
        /// <param name="ace">The ace to save to the database</param>
        /// <returns>An awaitable task</returns>
        public static Task SaveAce(this IDatabaseAdapter adapter, Ace ace)
        {
            return adapter.SaveAces(new List<Ace> { ace });
        }

        /// <summary>
        /// Deletes the specified <see cref="Ace"/> to the database
        /// </summary>
        /// <param name="adapter">The database adapter</param>
        /// <param name="ace">The ace to deletes from the database</param>
        /// <returns>An awaitable task</returns>
        public static Task DeleteAce(this IDatabaseAdapter adapter, Ace ace)
        {
            return adapter.DeleteAces(new List<Ace> { ace });
        }

        /// <summary>
        /// Saves the specified <see cref="PrincipalInheritance"/> to the database
        /// </summary>
        /// <param name="adapter">The database adapter</param>
        /// <param name="principal">The principal to save to the database</param>
        /// <returns>An awaitable task</returns>
        public static Task SavePrincipal(this IDatabaseAdapter adapter, PrincipalInheritance principal)
        {
            return adapter.SavePrincipals(new List<PrincipalInheritance> { principal });
        }

        /// <summary>
        /// Deletes the specified <see cref="PrincipalInheritance"/> to the database
        /// </summary>
        /// <param name="adapter">The database adapter</param>
        /// <param name="principal">The principal to delete from the database</param>
        /// <returns>An awaitable task</returns>
        public static Task DeletePrincipal(this IDatabaseAdapter adapter, PrincipalInheritance principal)
        {
            return adapter.DeletePrincipals(new List<PrincipalInheritance> { principal });
        }
    }
}
