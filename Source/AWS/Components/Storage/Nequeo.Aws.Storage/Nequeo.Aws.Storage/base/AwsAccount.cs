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
using System.Threading;

using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Nequeo.Aws.Storage
{
    /// <summary>
    /// AWS account.
    /// </summary>
    public sealed class AwsAccount
    {
        /// <summary>
        /// AWS account.
        /// </summary>
        public AwsAccount()
        {
            _awsDynamoDBServiceUrl = Nequeo.Aws.Storage.Properties.Settings.Default.AwsDynamoDBServiceUrl;
            _awsSimpleDBServiceUrl = Nequeo.Aws.Storage.Properties.Settings.Default.AwsSimpleDBServiceUrl;
            _awsDynamoDBRegionEndPoint = Nequeo.Aws.Storage.Properties.Settings.Default.AwsDynamoDBRegionEndPoint;
            _awsSimpleDBRegionEndPoint = Nequeo.Aws.Storage.Properties.Settings.Default.AwsSimpleDBRegionEndPoint;
        }

        private string _awsDynamoDBServiceUrl = null;
        private string _awsSimpleDBServiceUrl = null;

        private string _awsDynamoDBRegionEndPoint = null;
        private string _awsSimpleDBRegionEndPoint = null;

        /// <summary>
        /// Gets or sets the DynamoDB service URL.
        /// </summary>
        public string AwsDynamoDBServiceUrl
        {
            get { return _awsDynamoDBServiceUrl; }
            set { _awsDynamoDBServiceUrl = value; }
        }

        /// <summary>
        /// Gets or sets the SimpleDB service URL.
        /// </summary>
        public string AwsSimpleDBServiceUrl
        {
            get { return _awsSimpleDBServiceUrl; }
            set { _awsSimpleDBServiceUrl = value; }
        }

        /// <summary>
        /// Gets or sets the DynamoDB region endpoint.
        /// </summary>
        public string AwsDynamoDBRegionEndPoint
        {
            get { return _awsDynamoDBRegionEndPoint; }
            set { _awsDynamoDBRegionEndPoint = value; }
        }

        /// <summary>
        /// Gets or sets the SimpleDB region endpoint.
        /// </summary>
        public string AwsSimpleDBRegionEndPoint
        {
            get { return _awsSimpleDBRegionEndPoint; }
            set { _awsSimpleDBRegionEndPoint = value; }
        }
    }
}
