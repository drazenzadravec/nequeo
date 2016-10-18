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
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

using Nequeo.Data.DataType;
using Nequeo.Data;
using Nequeo.Data.Linq;
using Nequeo.Data.Control;
using Nequeo.Data.Custom;
using Nequeo.Data.LinqToSql;
using Nequeo.Data.DataSet;
using Nequeo.Data.Edm;
using Nequeo.Net.ServiceModel.Common;
using Nequeo.Data.TypeExtenders;
using Nequeo.Data.Extension;

namespace Nequeo.DataAccess.NequeoCompany.Edm.Model
{
    /// <summary>
    /// The User model object class.
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Validate the user
        /// </summary>
        /// <param name="loginUserName">The login username</param>
        /// <param name="loginPassword">The login password</param>
        /// <returns>The validated user; else null</returns>
        public virtual Edm.User Validate(string loginUserName, string loginPassword)
        {
            Edm.User user = null;

            try
            {
                // Encode password.
                Nequeo.Security.Configuration.Reader reader = new Security.Configuration.Reader();
                Nequeo.Cryptography.IPasswordEncryption encoder = reader.GetEncoder();

                // Encode the password.
                string encodedPassword = encoder.Encode(loginPassword, encoder.PasswordFormat);

                // Get the user for the credentials.
                user = Select.SelectIQueryableItems(
                    u => 
                        (u.LoginUserName == loginUserName) &&
                        (u.LoginPassword == encodedPassword)).First();
            }
            catch { user = null; }

            // Return the user.
            return user;
        }
    }
}
