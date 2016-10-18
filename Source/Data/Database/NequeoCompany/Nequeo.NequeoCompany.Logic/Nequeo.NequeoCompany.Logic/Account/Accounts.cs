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

namespace Nequeo.Logic.NequeoCompany.Account
{
    /// <summary>
    /// Complete data Accounts logic control
    /// </summary>
    public partial interface IAccounts
    {
        /// <summary>
        /// Gets the account data extension implementation.
        /// </summary>
        DataAccountExtender Extension { get; }

        /// <summary>
        /// Gets the account transactions data extension implementation.
        /// </summary>
        DataTransactionExtender Extension1 { get; }

        /// <summary>
        /// Gets the account type data extension implementation.
        /// </summary>
        DataAccountTypeExtender Extension2 { get; }

        /// <summary>
        /// Gets the data member data extension implementation.
        /// </summary>
        DataDataMemberExtender Extension3 { get; }

        /// <summary>
        /// Gets the data payment type data extension implementation.
        /// </summary>
        DataPaymentTypeExtender Extension4 { get; }

        /// <summary>
        /// Gets the data transaction type data extension implementation.
        /// </summary>
        DataTransactionTypeExtender Extension5 { get; }

        /// <summary>
        /// Gets the account report extension implementation.
        /// </summary>
        ReportAccountExtender ReportExtension { get; }
    }

    /// <summary>
    /// Complete data accounts logic control
    /// </summary>
    public partial class Accounts : DataAccountExtender,
        Data.Control.IExtension<
            DataAccountExtender,
            DataTransactionExtender,
            DataAccountTypeExtender,
            DataDataMemberExtender,
            DataPaymentTypeExtender,
            DataTransactionTypeExtender>,
        Report.Common.IReportExtension<ReportAccountExtender>
    {
        /// <summary>
        /// Gets the account data extension implementation.
        /// </summary>
        public virtual DataAccountExtender Extension
        {
            get { return new DataAccountExtender(); }
        }

        /// <summary>
        /// Gets the account transactions data extension implementation.
        /// </summary>
        public virtual DataTransactionExtender Extension1
        {
            get { return new DataTransactionExtender(); }
        }

        /// <summary>
        /// Gets the account type data extension implementation.
        /// </summary>
        public virtual DataAccountTypeExtender Extension2
        {
            get { return new DataAccountTypeExtender(); }
        }

        /// <summary>
        /// Gets the data member data extension implementation.
        /// </summary>
        public virtual DataDataMemberExtender Extension3
        {
            get { return new DataDataMemberExtender(); }
        }

        /// <summary>
        /// Gets the data payment type data extension implementation.
        /// </summary>
        public virtual DataPaymentTypeExtender Extension4
        {
            get { return new DataPaymentTypeExtender(); }
        }

        /// <summary>
        /// Gets the data transaction type data extension implementation.
        /// </summary>
        public virtual DataTransactionTypeExtender Extension5
        {
            get { return new DataTransactionTypeExtender(); }
        }

        /// <summary>
        /// Gets the account report extension implementation.
        /// </summary>
        public virtual ReportAccountExtender ReportExtension
        {
            get { return new ReportAccountExtender(); }
        }
    }
}
