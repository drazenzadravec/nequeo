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

namespace Nequeo.Security
{
    /// <summary>
    /// Code access permission.
    /// </summary>
    public class CodeAccess
    {
        /// <summary>
        /// Code access permission.
        /// </summary>
        /// <param name="permission">Defines the underlying structure of all code access permissions.</param>
        /// <param name="source">Permission source provider.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public CodeAccess(CodeAccessPermission permission, Nequeo.Security.IPermission source)
        {
            if (permission == null) throw new ArgumentNullException("permission");
            if (source == null) throw new ArgumentNullException("source");

            _permission = permission;
            _source = source;
        }

        private CodeAccessPermission _permission = null;
        private Nequeo.Security.IPermission _source = null;

        /// <summary>
        /// Gets the permission source.
        /// </summary>
        public virtual Nequeo.Security.IPermission Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets the code access permissions.
        /// </summary>
        public virtual CodeAccessPermission Permission
        {
            get { return _permission; }
        }

        /// <summary>
        /// Grant access to the resource (from the source 'Access' member).
        /// </summary>
        /// <returns>True if access is granted; else false.</returns>
        public bool Grant()
        {
            return _source.Access();
        }

        /// <summary>
        /// Forces a System.Security.SecurityException at run time if all callers higher
        /// in the call stack have not been granted the permission specified by the current
        /// instance.
        /// </summary>
        /// <exception cref="System.Security.SecurityException">A caller higher in the call 
        /// stack does not have the permission specified by the current instance</exception>
        public void Demand()
        {
            _permission.Demand();
        }
    }

    /// <summary>
    /// Permission source provider.
    /// </summary>
    public class PermissionSource : Nequeo.Security.IPermission
    {
        /// <summary>
        /// Permission source provider.
        /// </summary>
        public PermissionSource() { }

        /// <summary>
        /// Permission source provider.
        /// </summary>
        /// <param name="permission">Permission type.</param>
        public PermissionSource(PermissionType permission)
        {
            _permission = permission;
        }

        private PermissionType _permission = PermissionType.None;

        /// <summary>
        /// Gets or sets the permission type.
        /// </summary>
        public virtual PermissionType Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }
        
        /// <summary>
        /// Is permission to access resources granted.
        /// </summary>
        /// <returns>True if granted; else false.</returns>
        public virtual bool Access()
        {
            // If none permission then no access allowed.
            return _permission == PermissionType.None ? false : true;
        }

        /// <summary>
        /// Has permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True if has permission; else false.</returns>
        public virtual bool HasPermission(PermissionType permission)
        {
            return _permission.HasFlag(permission);
        }

        /// <summary>
        /// Has not got permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True if has not got permission; else false.</returns>
        public virtual bool NoPermission(PermissionType permission)
        {
            if (!_permission.HasFlag(permission))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates and returns a permission that is the intersection 
        /// of the current permission and the specified permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>The resulting permission.</returns>
        public virtual PermissionType Intersect(PermissionType permission)
        {
            PermissionType perm;
            List<string> permList = new List<string>();

            // Get the permission lists.
            string[] permPass = permission.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] permCurrent = _permission.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // For each permission.
            foreach (string item in permCurrent)
            {
                // Find the permission.
                IEnumerable<string> results = permPass.Where(u => u.Trim().Equals(item.Trim()));
                if(results != null && results.Count() > 0)
                {
                    // Add to the collection.
                    permList.Add(item.Trim()); 
                }
            }

            // If permission exists.
            if (permList.Count > 0)
            {
                // Get the intersect.
                string permInterset = string.Join(",", permList.ToArray());
                perm = (PermissionType)Enum.Parse(typeof(PermissionType), permInterset);
            }
            else
                perm = PermissionType.None;

            // Return the permission.
            return perm;
        }

        /// <summary>
        /// Determines whether the current permission
        /// is a subset of the specified permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True if the current permission is a subset of the specified permission; otherwise, false.</returns>
        public virtual bool IsSubsetOf(PermissionType permission)
        {
            int found = 0;
            
            // Get the permission lists.
            string[] permPass = permission.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] permCurrent = _permission.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int totalCount = permPass.Length;
            
            // For each permission.
            foreach (string item in permPass)
            {
                // Find the permission.
                IEnumerable<string> results = permCurrent.Where(u => u.Trim().Equals(item.Trim()));
                if (results != null && results.Count() > 0)
                    found++;
            }

            // If all have been found in permission.
            if (found == totalCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a permission that is the union of the current permission and the specified permission.
        /// </summary>
        /// <param name="permission">A permission to combine with the current permission. It must be of the same
        /// type as the current permission.</param>
        /// <returns>A new permission that represents the union of the current permission and
        /// the specified permission.</returns>
        public virtual PermissionType Union(PermissionType permission)
        {
            PermissionType perm;
            List<string> permList = new List<string>();

            // Get the permission lists.
            string[] permPass = permission.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] permCurrent = _permission.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // For each permission.
            foreach (string item in permCurrent)
            {
                // Add to the collection.
                permList.Add(item.Trim());
            }

            // For each permission.
            foreach (string item in permPass)
            {
                if (!permList.Contains(item.Trim()))
                    // Add to the collection.
                    permList.Add(item.Trim());
            }

            // If permission exists.
            if (permList.Count > 0)
            {
                // Get the intersect.
                string permInterset = string.Join(",", permList.ToArray());
                perm = (PermissionType)Enum.Parse(typeof(PermissionType), permInterset);
            }
            else
                perm = PermissionType.None;

            // Return the permission.
            return perm;
        }
    }

    /// <summary>
    /// Permission provider.
    /// </summary>
    public interface IPermission
    {
        /// <summary>
        /// Gets or sets the permission type.
        /// </summary>
        PermissionType Permission { get; set; }

        /// <summary>
        /// Is permission to access resources granted.
        /// </summary>
        /// <returns>True if granted; else false.</returns>
        bool Access();

        /// <summary>
        /// Has permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True if has permission; else false.</returns>
        bool HasPermission(PermissionType permission);

        /// <summary>
        /// Has not got permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True if has not permission; else false.</returns>
        bool NoPermission(PermissionType permission);

        /// <summary>
        /// Creates and returns a permission that is the intersection 
        /// of the current permission and the specified permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>The resulting permission.</returns>
        PermissionType Intersect(PermissionType permission);
        
        /// <summary>
        /// Determines whether the current permission
        /// is a subset of the specified permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>True if the current permission is a subset of the specified permission; otherwise, false.</returns>
        bool IsSubsetOf(PermissionType permission);

        /// <summary>
        /// Creates a permission that is the union of the current permission and the specified permission.
        /// </summary>
        /// <param name="permission">A permission to combine with the current permission. It must be of the same
        /// type as the current permission.</param>
        /// <returns>A new permission that represents the union of the current permission and
        /// the specified permission.</returns>
        PermissionType Union(PermissionType permission);
    }

    /// <summary>
    /// Permission type.
    /// </summary>
    [Flags]
    public enum PermissionType : long
    {
        /// <summary>
        /// Permission denied.
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow read.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Allow write.
        /// </summary>
        Write = 2,
        /// <summary>
        /// Allow viewing.
        /// </summary>
        View = 4,
        /// <summary>
        /// Allow modifying.
        /// </summary>
        Modify = 8,
        /// <summary>
        /// Allow adding.
        /// </summary>
        Add = 16,
        /// <summary>
        /// Allow deleting.
        /// </summary>
        Delete = 32,
        /// <summary>
        /// Allow downloading.
        /// </summary>
        Download = 64,
        /// <summary>
        /// Allow uploading.
        /// </summary>
        Upload = 128,
        /// <summary>
        /// Allow list content.
        /// </summary>
        List = 256,
        /// <summary>
        /// Allow execution.
        /// </summary>
        Execute = 512,
        /// <summary>
        /// Allow full control.
        /// </summary>
        Full = 1024,
        /// <summary>
        /// Allow limited access.
        /// </summary>
        Limited = 2048,
        /// <summary>
        /// Allow creation.
        /// </summary>
        Create = 4096,
        /// <summary>
        /// Allow open.
        /// </summary>
        Open = 8192,
        /// <summary>
        /// Allow close.
        /// </summary>
        Close = 16384,
        /// <summary>
        /// Allow moving.
        /// </summary>
        Move = 32768,
        /// <summary>
        /// Allow renaming.
        /// </summary>
        Rename = 65536,
        /// <summary>
        /// Allow copying.
        /// </summary>
        Copy = 131072,
        /// <summary>
        /// Allow design.
        /// </summary>
        Design = 262144,
        /// <summary>
        /// Allow contribute.
        /// </summary>
        Contribute = 524288,
        /// <summary>
        /// Allow manage.
        /// </summary>
        Manage = 1048576,
        /// <summary>
        /// Allow approve.
        /// </summary>
        Approve = 2097152,
        /// <summary>
        /// Allow apply.
        /// </summary>
        Apply = 4194304,
        /// <summary>
        /// Allow use.
        /// </summary>
        Use = 8388608,
        /// <summary>
        /// Allow browse.
        /// </summary>
        Browse = 16777216,
        /// <summary>
        /// Allow update.
        /// </summary>
        Update = 33554432,
        /// <summary>
        /// Allow remove.
        /// </summary>
        Remove = 67108864,
        /// <summary>
        /// Allow select.
        /// </summary>
        Select = 134217728,
        /// <summary>
        /// Allow get.
        /// </summary>
        Get = 268435456,
        /// <summary>
        /// Allow set.
        /// </summary>
        Set = 536870912,
        /// <summary>
        /// Allow override.
        /// </summary>
        Override = 1073741824,
        /// <summary>
        /// Allow send.
        /// </summary>
        Send = 2147483648,
        /// <summary>
        /// Allow receive.
        /// </summary>
        Receive = 4294967296,
        /// <summary>
        /// Allow insert.
        /// </summary>
        Insert = 8589934592,
        /// <summary>
        /// Allow administration.
        /// </summary>
        Administer = 17179869184,
        /// <summary>
        /// Allow full access.
        /// </summary>
        Unrestricted = 34359738368,
        /// <summary>
        /// Allow appending to resources.
        /// </summary>
        Append = 68719476736,
        /// <summary>
        /// Allow accept action.
        /// </summary>
        Accept = 137438953472,
        /// <summary>
        /// Allow connect action.
        /// </summary>
        Connect = 274877906944,
        /// <summary>
        /// Provides full access to all printers.
        /// </summary>
        AllPrinting = 549755813888,
        /// <summary>
        /// Provides printing programmatically to the default printer, along with safe printing through semirestricted dialog box. DefaultPrinting is a subset of AllPrinting.
        /// </summary>
        DefaultPrinting = 1099511627776,
        /// <summary>
        /// Prevents access to printers. NoPrinting is a subset of SafePrinting.
        /// </summary>
        NoPrinting = 2199023255552,
        /// <summary>
        /// Provides printing only from a restricted dialog box. SafePrinting is a subset of DefaultPrinting.
        /// </summary>
        SafePrinting = 4398046511104,
        /// <summary>
        /// Connection to any port.
        /// </summary>
        ConnectToUnrestrictedPort = 8796093022208,
        /// <summary>
        /// Ping access to network information.
        /// </summary>
        Ping = 17592186044416,
        /// <summary>
        /// Can look at the queues that are available and read the messages.
        /// </summary>
        Peek = 35184372088832,
        /// <summary>
        /// The ability to encrypt data.
        /// </summary>
        ProtectData = 70368744177664,
        /// <summary>
        /// The ability to encrypt memory.
        /// </summary>
        ProtectMemory = 140737488355328,
        /// <summary>
        /// The ability to unencrypt data.
        /// </summary>
        UnprotectData = 281474976710656,
        /// <summary>
        /// The ability to unencrypt memory.
        /// </summary>
        UnprotectMemory = 562949953421312,
        /// <summary>
        /// Ability to save files through the File dialog boxes.
        /// </summary>
        Save = 1125899906842624,
        /// <summary>
        /// The trusted sites zone is used for content located on Web sites 
        /// considered more reputable or trustworthy than other sites on the Internet.
        /// Users can use this zone to assign a higher trust level to these sites to 
        /// minimize the number of authentication requests. The URLs of these trusted 
        /// Web sites need to be mapped into this zone by the user.
        /// </summary>
        Trusted = 2251799813685248,
        /// <summary>
        /// Decrypt a key container. Decryption is a privileged operation because it uses the private key.
        /// </summary>
        Decrypt = 4503599627370496,
        /// <summary>
        /// Encrypt a key container. Decryption is a privileged operation because it uses the public key.
        /// </summary>
        Encrypt = 9007199254740992,
        /// <summary>
        /// Export a key from a key container. The ability to export a key is potentially harmful because it removes the exclusivity of the key.
        /// </summary>
        Export = 18014398509481984,
        /// <summary>
        /// Import a key into a key container. The ability to import a key can be as harmful as the ability to delete a 
        /// container because importing a key into a named key container replaces the existing key.
        /// </summary>
        Import = 36028797018963968,
        /// <summary>
        /// Sign a file using a key. The ability to sign a file is potentially harmful because it can allow a user to sign a file using another user's key.
        /// </summary>
        Sign = 72057594037927936,
        /// <summary>
        /// The restricted sites zone is used for Web sites with content that could cause, or could have caused, 
        /// problems when downloaded. The URLs of these untrusted Web sites need to be mapped into this zone by the user.
        /// </summary>
        Untrusted = 144115188075855872,
        /// <summary>
        /// Ability to store data.
        /// </summary>
        Store = 288230376151711744,
    }
}
