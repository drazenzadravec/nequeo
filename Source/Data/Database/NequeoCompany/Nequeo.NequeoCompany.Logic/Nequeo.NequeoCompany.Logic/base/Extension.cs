/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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

using Nequeo.Data.Control;
using Nequeo.DataAccess.NequeoCompany;

namespace Nequeo.Logic.NequeoCompany
{
    /// <summary>
    /// Complete data reference logic control
    /// </summary>
    public class Extender : 
        Data.Control.IExtension<
            Logic.NequeoCompany.Company.ICompanies,
            Logic.NequeoCompany.Account.IAccounts,
            Logic.NequeoCompany.Customer.ICustomers,
            Logic.NequeoCompany.Employee.IEmployees,
            Logic.NequeoCompany.Product.IProducts,
            Logic.NequeoCompany.Vendor.IVendors,
            Logic.NequeoCompany.Asset.IAssets>
    {
        /// <summary>
        /// Gets the company logic extension implementation.
        /// </summary>
        public virtual Company.ICompanies Extension
        {
            get { return new Company.Companies(); }
        }

        /// <summary>
        /// Gets the accounts logic extension implementation.
        /// </summary>
        public virtual Account.IAccounts Extension1
        {
            get { return new Account.Accounts(); }
        }

        /// <summary>
        /// Gets the customers logic extension implementation.
        /// </summary>
        public virtual Customer.ICustomers Extension2
        {
            get { return new Customer.Customers(); }
        }

        /// <summary>
        /// Gets the employees logic extension implementation.
        /// </summary>
        public virtual Employee.IEmployees Extension3
        {
            get { return new Employee.Employees(); }
        }

        /// <summary>
        /// Gets the products logic extension implementation.
        /// </summary>
        public virtual Product.IProducts Extension4
        {
            get { return new Product.Products(); }
        }

        /// <summary>
        /// Gets the vendors logic extension implementation.
        /// </summary>
        public virtual Vendor.IVendors Extension5
        {
            get { return new Vendor.Vendors(); }
        }

        /// <summary>
        /// Gets the assets logic extension implementation.
        /// </summary>
        public virtual Asset.IAssets Extension6
        {
            get { return new Asset.Assets(); }
        }
    }
}
