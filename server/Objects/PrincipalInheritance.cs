using AclManager.Server.Helpers;
using Newtonsoft.Json;
using Console = AclManager.Server.Helpers.Console;

namespace AclManager.Server.Objects
{
    /// <summary>
    /// Represents a principal inheritance entry
    /// </summary>
    internal class PrincipalInheritance
    {
        /// <summary>
        /// The ID of this entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The child principal which will inherit from the <see cref="Parent"/>
        /// </summary>
        public string Child { get; set; }

        /// <summary>
        /// The parent principal which the <see cref="Child"/> will inherit from
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Indicates if this <see cref="PrincipalInheritance"/> can be added to the access control list
        /// </summary>
        [JsonIgnore]
        public bool CanBeAdded { get; set; } = true;

        /// <summary>
        /// Indicates if this <see cref="PrincipalInheritance"/> can be removed from the access control list
        /// </summary>
        [JsonIgnore]
        public bool CanBeRemoved { get; set; }

        public PrincipalInheritance()
        {

        }

        public PrincipalInheritance(string child, string parent)
        {
            Child = child;
            Parent = parent;
        }

        /// <summary>
        /// Adds this <see cref="PrincipalInheritance"/> entry to the access control list
        /// </summary>
        public void Add()
        {
            CanBeAdded = false;

            Console.WriteDebug($"Adding principal: {ToString()}");

            AccessControl.AddPrincipal(Child, Parent);
        }

        /// <summary>
        /// Removes this <see cref="PrincipalInheritance"/> entry to the access control list
        /// </summary>
        public void Remove()
        {
            CanBeRemoved = false;

            Console.WriteDebug($"Removing principal: {ToString()}");

            AccessControl.RemovePrincipal(Child, Parent);
        }

        public bool Equals(PrincipalInheritance principal)
        {
            if (ReferenceEquals(principal, null))
            {
                return false;
            }

            return (Id > 0 && Id == principal.Id) || (Child == principal.Child && Parent == principal.Parent);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && Equals((PrincipalInheritance)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(PrincipalInheritance left, PrincipalInheritance right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }
        public static bool operator !=(PrincipalInheritance left, PrincipalInheritance right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{Child}<-{Parent} ({Id})";
        }
    }
}
