/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel;

namespace Nequeo.DataAccess.NequeoCompany.Data.Extended
{
    /// <summary>
    /// User type list Observable Collection
    /// </summary>
    public partial class UserTypeList : Nequeo.Collections.Observable<Data.UserType>
    {
    }
}
