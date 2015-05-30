/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security;
using System.Net;
using System.Security.Authentication;
using System.Security.Permissions;

namespace Nequeo.Security
{
    /// <summary>
    /// File access permission.
    /// </summary>
    [Serializable()] 
    public sealed class FileAccessPermission : CodeAccessPermission, System.Security.IPermission, IUnrestrictedPermission, ISecurityEncodable, ICloneable
    {
        /// <summary>
        /// File access permission.
        /// </summary>
        /// <param name="permission">Permission type.</param>
        public FileAccessPermission(PermissionType permission)
        {
            _permission = permission;
            _specifiedAsUnrestricted = false;
        }

        private Boolean _specifiedAsUnrestricted = false;
        private PermissionType _permission = PermissionType.None;
        private PermissionType _filePermission =
            PermissionType.Append | PermissionType.Close | PermissionType.Copy | PermissionType.Create | PermissionType.Rename | PermissionType.None |
            PermissionType.Delete | PermissionType.Open | PermissionType.Read | PermissionType.Write | PermissionType.Unrestricted;

        /// <summary>
        /// Gets the list of all file permissions.
        /// </summary>
        public PermissionType FilePermissions
        {
            get { return _filePermission; }
        }

        /// <summary>
        /// Gets or sets the permissions set.
        /// </summary>
        public PermissionType Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }

        /// <summary>
        /// Creates and returns an identical copy of the current permission object.
        /// </summary>
        /// <returns>A copy of the current permission object.</returns>
        public override System.Security.IPermission Copy()
        {
            return (System.Security.IPermission)Clone();
        }

        /// <summary>
        /// Reconstructs a security object with a
        /// specified state from an XML encoding.
        /// </summary>
        /// <param name="elem">The XML encoding to use to reconstruct the security object.</param>
        public override void FromXml(SecurityElement elem)
        {
            _specifiedAsUnrestricted = false;
            _permission = 0;

            // If XML indicates an unrestricted permission, make this permission unrestricted.
            String s = (String)elem.Attributes["Unrestricted"];
            if (s != null)
            {
                _specifiedAsUnrestricted = Convert.ToBoolean(s);
                if (_specifiedAsUnrestricted)
                    _permission = PermissionType.Unrestricted;
            }

            // If XML indicates a restricted permission, parse the flags. 
            if (!_specifiedAsUnrestricted)
            {
                s = (String)elem.Attributes["Flags"];
                if (s != null)
                {
                    _permission = (PermissionType)Enum.Parse(typeof(PermissionType), s);
                }
            }
        }

        /// <summary>
        /// Creates and returns a permission that is the intersection 
        /// of the current permission and the specified permission.
        /// </summary>
        /// <param name="target">A permission to intersect with the current permission. 
        /// It must be of the same type as the current permission.</param>
        /// <returns>A new permission that represents the intersection of the current permission
        /// and the specified permission. This new permission is null if the intersectionis empty.</returns>
        public override System.Security.IPermission Intersect(System.Security.IPermission target)
        {
            // If 'target' is null, return null. 
            if (target == null) return null;

            // Both objects must be the same type.
            FileAccessPermission filePerm = VerifyTypeMatch(target);

            // If 'this' and 'target' are unrestricted, return a new unrestricted permission. 
            if (_specifiedAsUnrestricted && filePerm._specifiedAsUnrestricted)
                return Clone(true, PermissionType.Unrestricted);

            // Calculate the intersected permissions. If there are none, return null.
            PermissionType val = (PermissionType)Math.Min((Int32)_permission, (Int32)filePerm._permission);
            if (val == 0) return null;

            // Get the intersect.
            val = new PermissionSource(_permission).Intersect(filePerm._permission);

            // Return a new object with the intersected permission value. 
            return Clone(false, val);
        }

        /// <summary>
        /// Determines whether the current permission
        /// is a subset of the specified permission.
        /// </summary>
        /// <param name="target">A permission that is to be tested for the subset relationship. 
        /// This permission must be of the same type as the current permission.</param>
        /// <returns>True if the current permission is a subset of the specified permission; otherwise, false.</returns>
        public override bool IsSubsetOf(System.Security.IPermission target)
        {
            // If 'target' is null and this permission allows nothing, return true. 
            if (target == null) return _permission == 0;

            // Both objects must be the same type.
            FileAccessPermission filePerm = VerifyTypeMatch(target);

            // Return true if the permissions of 'this' is a subset of 'target'. 
            return new PermissionSource(_permission).IsSubsetOf(filePerm._permission);
        }

        /// <summary>
        /// Creates a permission that is the union of the current permission and the specified permission.
        /// </summary>
        /// <param name="target">A permission to combine with the current permission. It must be of the same
        /// type as the current permission.</param>
        /// <returns>A new permission that represents the union of the current permission and
        /// the specified permission.</returns>
        public override System.Security.IPermission Union(System.Security.IPermission target)
        {
            // If 'target' is null, then return a copy of 'this'. 
            if (target == null) return Copy();

            // Both objects must be the same type.
            FileAccessPermission filePerm = VerifyTypeMatch(target);

            // If 'this' or 'target' are unrestricted, return a new unrestricted permission. 
            if (_specifiedAsUnrestricted || filePerm._specifiedAsUnrestricted)
                return Clone(true, PermissionType.Unrestricted);

            // Return a new object with the calculated, unioned permission value. 
            return Clone(false, new PermissionSource(_permission).Union(filePerm._permission));
        }

        /// <summary>
        /// Creates an XML encoding of the security object and its current state.
        /// </summary>
        /// <returns>An XML encoding of the security object, including any state information.</returns>
        public override SecurityElement ToXml()
        {
            // These first three lines create an element with the required format.
            SecurityElement e = new SecurityElement("IPermission");

            // Replace the double quotation marks with single quotation marks
            // to remain XML compliant when the culture is not neutral.
            e.AddAttribute("class", GetType().AssemblyQualifiedName.Replace('\"', '\''));
            e.AddAttribute("version", "1");

            if (!_specifiedAsUnrestricted)
                e.AddAttribute("Flags", Enum.Format(typeof(PermissionType), _permission, "G"));
            else
                e.AddAttribute("Unrestricted", "true");
            return e;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Returns a value indicating whether unrestricted access to the resource protected by the permission is allowed.
        /// </summary>
        /// <returns>True if unrestricted use of the resource protected by the permission is allowed; otherwise, false.</returns>
        public bool IsUnrestricted()
        {
            // This means that the object is unrestricted at runtime. 
            return _permission == PermissionType.Unrestricted;
        }

        /// <summary>
        /// Creates and returns a string representation of the current permission object.
        /// </summary>
        /// <returns>A string representation of the current permission object.</returns>
        public override string ToString()
        {
            return ToXml().ToString();
        }

        /// <summary>
        /// Verify the type.
        /// </summary>
        /// <param name="target">The current target.</param>
        /// <returns>The file access permission is a match; else throw exception.</returns>
        private FileAccessPermission VerifyTypeMatch(System.Security.IPermission target)
        {
            if (GetType() != target.GetType())
                throw new ArgumentException(String.Format("target must be of the {0} type", GetType().FullName));

            return (FileAccessPermission)target;
        }

        /// <summary>
        /// Clone the file access permission.
        /// </summary>
        /// <param name="specifiedAsUnrestricted">True if unrestricted; else restricted.</param>
        /// <param name="flags">The permission flags.</param>
        /// <returns>The file access permission clone.</returns>
        private FileAccessPermission Clone(Boolean specifiedAsUnrestricted, PermissionType flags)
        {
            FileAccessPermission filePerm = (FileAccessPermission)Clone();
            filePerm._specifiedAsUnrestricted = specifiedAsUnrestricted;
            filePerm._permission = specifiedAsUnrestricted ? PermissionType.Unrestricted : _permission;
            return filePerm;
        }
    }
}
