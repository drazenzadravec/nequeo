/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Data.Provider
{
    /// <summary>
    /// Global store provider
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// Load provider data from the store.
        /// </summary>
        void Load();

        /// <summary>
        /// Commit provider data to the store.
        /// </summary>
        void Commit();
    }
}
