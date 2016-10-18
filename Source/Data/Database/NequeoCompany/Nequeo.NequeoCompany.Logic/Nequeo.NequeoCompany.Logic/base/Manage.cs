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
using System.Threading.Tasks;

using Nequeo.Logic.NequeoCompany.User;
using Nequeo.Logic.NequeoCompany.Account;
using Nequeo.Logic.NequeoCompany.Vendor;
using Nequeo.Logic.NequeoCompany.Product;
using Nequeo.Logic.NequeoCompany.Employee;
using Nequeo.Logic.NequeoCompany.Customer;
using Nequeo.Logic.NequeoCompany.Company;
using Nequeo.Logic.NequeoCompany.Asset;

namespace Nequeo.Logic.NequeoCompany
{
    /// <summary>
    /// Complete data Manage logic control
    /// </summary>
    public partial interface IManage
    {
        /// <summary>
        /// Gets the current type instance.
        /// </summary>
        IManage Current { get; }

        /// <summary>
        /// Gets the user type data extension implementation.
        /// </summary>
        DataUserTypeExtender UserType { get; }

        /// <summary>
        /// Gets the user data extension implementation.
        /// </summary>
        DataUserExtender User { get; }

        /// <summary>
        /// Gets the transaction type data extension implementation.
        /// </summary>
        DataTransactionTypeExtender TransactionType { get; }

        /// <summary>
        /// Gets the state data extension implementation.
        /// </summary>
        DataStateExtender State { get; }

        /// <summary>
        /// Gets the purchase type data extension implementation.
        /// </summary>
        DataPurchaseTypeExtender PurchaseType { get; }

        /// <summary>
        /// Gets the Product Sub Category data extension implementation.
        /// </summary>
        DataProductSubCategoryExtender ProductSubCategory { get; }

        /// <summary>
        /// Gets the Product Category data extension implementation.
        /// </summary>
        DataProductCategoryExtender ProductCategory { get; }

        /// <summary>
        /// Gets the Product Status data extension implementation.
        /// </summary>
        DataProductStatusExtender ProductStatus { get; }

        /// <summary>
        /// Gets the Payment Type data extension implementation.
        /// </summary>
        DataPaymentTypeExtender PaymentType { get; }

        /// <summary>
        /// Gets the Pay Interval Type data extension implementation.
        /// </summary>
        DataPayIntervalTypeExtender PayIntervalType { get; }

        /// <summary>
        /// Gets the Income Type data extension implementation.
        /// </summary>
        DataIncomeTypeExtender IncomeType { get; }

        /// <summary>
        /// Gets the GST Income Type data extension implementation.
        /// </summary>
        DataGstIncomeTypeExtender GstIncomeType { get; }

        /// <summary>
        /// Gets the Generic data extension implementation.
        /// </summary>
        DataGenericDataExtender GenericData { get; }

        /// <summary>
        /// Gets the Expense Type data extension implementation.
        /// </summary>
        DataExpenseTypeExtender ExpenseType { get; }

        /// <summary>
        /// Gets the Data Member Tables data extension implementation.
        /// </summary>
        DataDataMemberTablesExtender DataMemberTables { get; }

        /// <summary>
        /// Gets the Data Member data extension implementation.
        /// </summary>
        DataDataMemberExtender DataMember { get; }

        /// <summary>
        /// Gets the Asset Category data extension implementation.
        /// </summary>
        DataAssetCategoryExtender AssetCategory { get; }

        /// <summary>
        /// Gets the Account Type data extension implementation.
        /// </summary>
        DataAccountTypeExtender AccountType { get; }
    }

    /// <summary>
    /// The Manage transaction data extension extender.
    /// </summary>
    public partial class Manage : IManage
    {
        /// <summary>
        /// Gets the current type instance.
        /// </summary>
        public IManage Current
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the user type data extension implementation.
        /// </summary>
        public virtual DataUserTypeExtender UserType
        {
            get { return new DataUserTypeExtender(); }
        }

        /// <summary>
        /// Gets the user data extension implementation.
        /// </summary>
        public virtual DataUserExtender User
        {
            get { return new DataUserExtender(); }
        }

        /// <summary>
        /// Gets the transaction type data extension implementation.
        /// </summary>
        public virtual DataTransactionTypeExtender TransactionType
        {
            get { return new DataTransactionTypeExtender(); }
        }

        /// <summary>
        /// Gets the state data extension implementation.
        /// </summary>
        public virtual DataStateExtender State
        {
            get { return new DataStateExtender(); }
        }

        /// <summary>
        /// Gets the purchase type data extension implementation.
        /// </summary>
        public virtual DataPurchaseTypeExtender PurchaseType
        {
            get { return new DataPurchaseTypeExtender(); }
        }

        /// <summary>
        /// Gets the Product Sub Category data extension implementation.
        /// </summary>
        public virtual DataProductSubCategoryExtender ProductSubCategory
        {
            get { return new DataProductSubCategoryExtender(); }
        }

        /// <summary>
        /// Gets the Product Category data extension implementation.
        /// </summary>
        public virtual DataProductCategoryExtender ProductCategory
        {
            get { return new DataProductCategoryExtender(); }
        }

        /// <summary>
        /// Gets the Product Status data extension implementation.
        /// </summary>
        public virtual DataProductStatusExtender ProductStatus
        {
            get { return new DataProductStatusExtender(); }
        }

        /// <summary>
        /// Gets the Payment Type data extension implementation.
        /// </summary>
        public virtual DataPaymentTypeExtender PaymentType
        {
            get { return new DataPaymentTypeExtender(); }
        }

        /// <summary>
        /// Gets the Pay Interval Type data extension implementation.
        /// </summary>
        public virtual DataPayIntervalTypeExtender PayIntervalType
        {
            get { return new DataPayIntervalTypeExtender(); }
        }

        /// <summary>
        /// Gets the Income Type data extension implementation.
        /// </summary>
        public virtual DataIncomeTypeExtender IncomeType
        {
            get { return new DataIncomeTypeExtender(); }
        }

        /// <summary>
        /// Gets the GST Income Type data extension implementation.
        /// </summary>
        public virtual DataGstIncomeTypeExtender GstIncomeType
        {
            get { return new DataGstIncomeTypeExtender(); }
        }

        /// <summary>
        /// Gets the Generic data extension implementation.
        /// </summary>
        public virtual DataGenericDataExtender GenericData
        {
            get { return new DataGenericDataExtender(); }
        }

        /// <summary>
        /// Gets the Expense Type data extension implementation.
        /// </summary>
        public virtual DataExpenseTypeExtender ExpenseType
        {
            get { return new DataExpenseTypeExtender(); }
        }

        /// <summary>
        /// Gets the Data Member Tables data extension implementation.
        /// </summary>
        public virtual DataDataMemberTablesExtender DataMemberTables
        {
            get { return new DataDataMemberTablesExtender(); }
        }

        /// <summary>
        /// Gets the Data Member data extension implementation.
        /// </summary>
        public virtual DataDataMemberExtender DataMember
        {
            get { return new DataDataMemberExtender(); }
        }

        /// <summary>
        /// Gets the Asset Category data extension implementation.
        /// </summary>
        public virtual DataAssetCategoryExtender AssetCategory
        {
            get { return new DataAssetCategoryExtender(); }
        }

        /// <summary>
        /// Gets the Account Type data extension implementation.
        /// </summary>
        public virtual DataAccountTypeExtender AccountType
        {
            get { return new DataAccountTypeExtender(); }
        }
    }
}
