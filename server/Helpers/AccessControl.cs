using static CitizenFX.Core.Native.API;

namespace AclManager.Server.Helpers
{
    internal static class AccessControl
    {
        /// <summary>
        /// Executes the add_ace command
        /// </summary>
        /// <param name="principal">The principal to add the access control entry to</param>
        /// <param name="aceObject">The access control entry being added to the principal</param>
        /// <param name="allowType">The allow type of the entry</param>
        public static void AddAce(string principal, string aceObject, string allowType)
        {
            ExecuteCommand($"add_ace \"{principal}\" \"{aceObject}\" \"{allowType}\"");
        }

        /// <summary>
        /// Executes the remove_ace command
        /// </summary>
        /// <param name="principal">The principal to remove the access control entry to</param>
        /// <param name="aceObject">The access control entry being removed to the principal</param>
        /// <param name="allowType">The allow type of the entry</param>
        public static void RemoveAce(string principal, string aceObject, string allowType)
        {
            ExecuteCommand($"remove_ace \"{principal}\" \"{aceObject}\" \"{allowType}\"");
        }
        
        /// <summary>
        /// Executes the add_principal command
        /// </summary>
        /// <param name="child">The child principal</param>
        /// <param name="parent">The parent principal</param>
        public static void AddPrincipal(string child, string parent)
        {
            ExecuteCommand($"add_principal \"{child}\" \"{parent}\"");
        }

        /// <summary>
        /// Executes the remove_principal command
        /// </summary>
        /// <param name="child">The child principal</param>
        /// <param name="parent">The parent principal</param>
        public static void RemovePrincipal(string child, string parent)
        {
            ExecuteCommand($"remove_principal \"{child}\" \"{parent}\"");
        }

        /// <summary>
        /// Checks if an ace can be granted in the current context
        /// </summary>
        /// <param name="ace">The ace object to grant</param>
        /// <returns>If the ace can be granted in the current context</returns>
        public static bool CanGrantAce(string ace)
        {
            return IsAceAllowed($"acl.grant.{ace}");
        }

        /// <summary>
        /// Checks if a principal can grant the specified ace 
        /// </summary>
        /// <param name="principal">The principal to check</param>
        /// <param name="ace">The ace object to grant</param>
        /// <returns>If the principal can grant the ace</returns>
        public static bool CanGrantAce(string principal, string ace)
        {
            return IsPrincipalAceAllowed(principal, $"acl.grant.{ace}");
        }

        /// <summary>
        /// Checks if an ace can be revoked in the current context
        /// </summary>
        /// <param name="ace">The ace object to revoke</param>
        /// <returns>If the ace can be revoked in the current context</returns>
        public static bool CanRevokeAce(string ace)
        {
            return IsAceAllowed($"acl.revoke.{ace}");
        }

        /// <summary>
        /// Checks if a principal can revoke the specified ace 
        /// </summary>
        /// <param name="principal">The principal to check</param>
        /// <param name="ace">The ace object to revoke</param>
        /// <returns>If the principal can revoke the ace</returns>
        public static bool CanRevokeAce(string principal, string ace)
        {
            return IsPrincipalAceAllowed(principal, $"acl.revoke.{ace}");
        }

        /// <summary>
        /// Checks if the current context has permission to allow principals to inherit from the specified principal
        /// </summary>
        /// <param name="principal">The principal to inherit</param>
        /// <returns>If the principal can be inherited from in the current context</returns>
        public static bool CanInheritFromPrincipal(string principal)
        {
            return IsAceAllowed($"acl.inherit.{principal}");
        }

        /// <summary>
        /// Checks if principal has permission to allow principals to inherit from the specified principal
        /// </summary>
        /// <param name="principal">The principal that will be checked for permissions</param>
        /// <param name="principalToInherit">The principal to inherit</param>
        /// <returns>If the principal can be inherited from in the current context</returns>
        public static bool CanInheritFromPrincipal(string principal, string principalToInherit)
        {
            return IsPrincipalAceAllowed(principal, $"acl.inherit.{principalToInherit}");
        }

        /// <summary>
        /// Checks if the current context has permission to allow principals to disinherit from the specified principal
        /// </summary>
        /// <param name="principal">The principal to disinherit</param>
        /// <returns>If the principal can be inherited from in the current context</returns>
        public static bool CanDisinheritFromPrincipal(string principal)
        {
            return IsAceAllowed($"acl.disinherit.{principal}");
        }

        /// <summary>
        /// Checks if principal has permission to allow principals to disinherit from the specified principal
        /// </summary>
        /// <param name="principal">The principal that will be checked for permissions</param>
        /// <param name="principalToInherit">The principal to disinherit</param>
        /// <returns>If the principal can be disinherited from by the target principal</returns>
        public static bool CanDisinheritFromPrincipal(string principal, string principalToInherit)
        {
            return IsPrincipalAceAllowed(principal, $"acl.disinherit.{principalToInherit}");
        }
    }
}
