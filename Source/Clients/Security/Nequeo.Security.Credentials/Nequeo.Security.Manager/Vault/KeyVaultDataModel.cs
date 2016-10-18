/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
	Permission is hereby granted, free of charge, to any person
	obtaining a copy of this software and associated documentation
	files (the "Software"), to deal in the Software without
	restriction, including without limitation the rights to use,
	copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the
	Software is furnished to do so, subject to the following
	conditions:

	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
	OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
	WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
	OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Security.Manager.Vault
{
    /// <summary>
    /// The key vault type that is requested.
    /// </summary>
    public enum KeyVaultType
    {
        /// <summary>
        /// Process all requests for cryptography internally, does not expose any keys.
        /// </summary>
        Cryptography = 0,
        /// <summary>
        /// Process all requests by sending keys to requests for external processing, exposes keys.
        /// </summary>
        ActiveStore = 1,
    }

    /// <summary>
    /// Key vault data model.
    /// </summary>
    public class KeyVaultDataModel
    {
        /// <summary>
        /// Gets or sets cryptography key data models.
        /// </summary>
        public CryptographyKeyVaultDataModel[] Cryptography { get; set; }

        /// <summary>
        /// Gets or sets active store key data models.
        /// </summary>
        public ActiveStoreKeyVaultDataModel[] ActiveStore { get; set; }
    }

    /// <summary>
    /// Cryptography key store, keys are never exposed publically, all requests for cryptography are processed internally.
    /// </summary>
    public class CryptographyKeyVaultDataModel
    {
        /// <summary>
        /// Gets or sets the key name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the stored key.
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sete the stored key type.
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// Gets or sets the type of security key that is stored.
        /// </summary>
        public Nequeo.Cryptography.SecurityType SecurityType { get; set; }

        /// <summary>
        /// Gets or sets the cryptograph cypher to use.
        /// </summary>
        public Nequeo.Cryptography.CypherType CypherType { get; set; }

        /// <summary>
        /// Gets or sets the cryptograph key ID; used specifically for CypherType.PGP.
        /// </summary>
        public long KeyID { get; set; }
    }

    /// <summary>
    /// Active storekey store, keys are exposed publically, all requests for keys are send to the client for processing.
    /// </summary>
    public class ActiveStoreKeyVaultDataModel
    {
        /// <summary>
        /// Gets or sets the key name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the stored key.
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sete the stored key type.
        /// </summary>
        public string KeyType { get; set; }
    }
}
