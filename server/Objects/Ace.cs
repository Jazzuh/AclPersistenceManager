using AclManager.Server.Helpers;
using Newtonsoft.Json;
using Console = AclManager.Server.Helpers.Console;

namespace AclManager.Server.Objects
{
    /// <summary>
    /// Represents an access control entry
    /// </summary>
    internal class Ace
    {
        /// <summary>
        /// The ID of this entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The principal which this ace has been assigned to
        /// </summary>
        public string Principal { get; set; }

        /// <summary>
        /// The ace object string which will be assigned to the <see cref="Principal"/>
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// The allow type of this <see cref="Ace"/>
        /// </summary>
        public string AllowType { get; set; }

        /// <summary>
        /// Indicates if this <see cref="Ace"/> can be added to the access control list
        /// </summary>
        [JsonIgnore]
        public bool CanBeAdded { get; set; } = true;

        /// <summary>
        /// Indicates if this <see cref="Ace"/> can be removed from the access control list
        /// </summary>
        [JsonIgnore]
        public bool CanBeRemoved { get; set; }

        public Ace()
        {

        }

        public Ace(string principal, string aceObject, string allowType)
        {
            Principal = principal;
            Object = aceObject;
            AllowType = allowType;
        }

        /// <summary>
        /// Adds this <see cref="Ace"/> to the access control list
        /// </summary>
        public void Add()
        {
            CanBeAdded = false;

            Console.WriteDebug($"Adding ace: {ToString()}");

            AccessControl.AddAce(Principal, Object, AllowType.ToLower());
        }

        /// <summary>
        /// Removes this <see cref="Ace"/> from the access control list
        /// </summary>
        public void Remove()
        {
            CanBeRemoved = false;

            Console.WriteDebug($"Removing ace: {ToString()}");

            AccessControl.RemoveAce(Principal, Object, AllowType.ToLower());
        }

        public bool Equals(Ace ace)
        {
            if (ReferenceEquals(ace, null))
            {
                return false;
            }

            return (Id > 0 && Id == ace.Id) || (Principal == ace.Principal && Object == ace.Object && AllowType == ace.AllowType);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && Equals((Ace)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Ace left, Ace right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }
        public static bool operator !=(Ace left, Ace right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{Principal} {Object} {AllowType} ({Id})";
        }
    }
}
