/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// OAuth problem report.
    /// </summary>
    [Serializable]
    public class OAuthProblemReport
    {
        /// <summary>
        /// OAuth problem report.
        /// </summary>
        public OAuthProblemReport()
        {
            ParametersRejected = new List<string>();
            ParametersAbsent = new List<string>();
        }

        /// <summary>
        /// OAuth problem report.
        /// </summary>
        /// <param name="parameters">The URI parameters</param>
        public OAuthProblemReport(NameValueCollection parameters)
        {
            Problem = parameters[Parameters.OAuth_Problem];

            ProblemAdvice = parameters[Parameters.OAuth_Problem_Advice];

            ParametersAbsent = parameters.AllKeys.Any(key => key == Parameters.OAuth_Parameters_Absent)
                                ? ParseFormattedParameters(parameters[Parameters.OAuth_Parameters_Absent])
                                : new List<string>();

            ParametersRejected = parameters.AllKeys.Any(key => key == Parameters.OAuth_Parameters_Rejected)
                                    ? ParseFormattedParameters(parameters[Parameters.OAuth_Parameters_Rejected])
                                    : new List<string>();

            if (parameters.AllKeys.Any(key => key == Parameters.OAuth_Acceptable_Timestamps))
            {
                string[] timeStamps = parameters[Parameters.OAuth_Acceptable_Timestamps].Split(new[] { '-' });
                AcceptableTimeStampsFrom = DateTimeUtility.FromEpoch(Convert.ToInt64(timeStamps[0]));
                AcceptableTimeStampsTo = DateTimeUtility.FromEpoch(Convert.ToInt64(timeStamps[1]));
            }

            if (parameters.AllKeys.Any(key => key == Parameters.OAuth_Acceptable_Versions))
            {
                string[] versions = parameters[Parameters.OAuth_Acceptable_Versions].Split(new[] { '-' });
                AcceptableVersionFrom = versions[0];
                AcceptableVersionTo = versions[1];
            }
        }

        /// <summary>
        /// OAuth problem report.
        /// </summary>
        /// <param name="formattedReport">The formatted report.</param>
        public OAuthProblemReport(string formattedReport)
            : this(HttpUtility.ParseQueryString(formattedReport))
        {
        }

        /// <summary>
        /// Gets sets the acceptable version to
        /// </summary>
        public string AcceptableVersionTo { get; set; }

        /// <summary>
        /// Gets sets the acceptable version from
        /// </summary>
        public string AcceptableVersionFrom { get; set; }

        /// <summary>
        /// Gets sets the parameters rejected
        /// </summary>
        public List<string> ParametersRejected { get; set; }

        /// <summary>
        /// Gets sets the parameters absent
        /// </summary>
        public List<string> ParametersAbsent { get; set; }

        /// <summary>
        /// Gets sets the problem advice
        /// </summary>
        public string ProblemAdvice { get; set; }

        /// <summary>
        /// Gets sets the problem
        /// </summary>
        public string Problem { get; set; }

        /// <summary>
        /// Gets sets the acceptable time stamps to
        /// </summary>
        public DateTime? AcceptableTimeStampsTo { get; set; }

        /// <summary>
        /// Gets sets the acceptable time stamps from
        /// </summary>
        public DateTime? AcceptableTimeStampsFrom { get; set; }

        /// <summary>
        /// Validate request parameters absent
        /// </summary>
        /// <param name="parameters">The collection of request parameters</param>
        public void ValidateRequestParametersAbsent(NameValueCollection parameters)
        {
            List<string> maditoryParameters = new List<string>()
            {
                Parameters.OAuth_Callback.ToLower(),
                Parameters.OAuth_Nonce.ToLower(),
                Parameters.OAuth_Consumer_Key.ToLower(),
                Parameters.OAuth_Signature_Method.ToLower(),
                Parameters.OAuth_Timestamp.ToLower(),
                Parameters.OAuth_Signature.ToLower()
            };

            foreach (string parameter in maditoryParameters)
            {
                if (!parameters.AllKeys.Contains(parameter))
                    ParametersAbsent.Add(parameter);
            }
        }

        /// <summary>
        /// Validate request parameters absent
        /// </summary>
        /// <param name="parameters">The collection of request parameters</param>
        public void ValidateResourceParametersAbsent(NameValueCollection parameters)
        {
            List<string> maditoryParameters = new List<string>()
            {
                Parameters.OAuth_Token.ToLower(),
                Parameters.OAuth_Consumer_Key.ToLower()
            };

            foreach (string parameter in maditoryParameters)
            {
                if (!parameters.AllKeys.Contains(parameter))
                    ParametersAbsent.Add(parameter);
            }
        }

        /// <summary>
        /// Validate access parameters absent
        /// </summary>
        /// <param name="parameters">The collection of access parameters</param>
        public void ValidateAccessParametersAbsent(NameValueCollection parameters)
        {
            List<string> maditoryParameters = new List<string>()
            {
                Parameters.OAuth_Token.ToLower(),
                Parameters.OAuth_Nonce.ToLower(),
                Parameters.OAuth_Consumer_Key.ToLower(),
                Parameters.OAuth_Signature_Method.ToLower(),
                Parameters.OAuth_Timestamp.ToLower(),
                Parameters.OAuth_Verifier.ToLower(),
                Parameters.OAuth_Signature.ToLower()
            };

            foreach (string parameter in maditoryParameters)
            {
                if (!parameters.AllKeys.Contains(parameter))
                    ParametersAbsent.Add(parameter);
            }
        }

        /// <summary>
        /// Validate authorise parameters absent
        /// </summary>
        /// <param name="parameters">The collection of access parameters</param>
        public void ValidateAuthoriseParametersAbsent(NameValueCollection parameters)
        {
            List<string> maditoryParameters = new List<string>()
            {
                Parameters.OAuth_Token.ToLower(),
                Parameters.OAuth_Callback.ToLower(),
                Parameters.Company_Unique_User_Identifier.ToLower()
            };

            foreach (string parameter in maditoryParameters)
            {
                if (!parameters.AllKeys.Contains(parameter))
                    ParametersAbsent.Add(parameter);
            }
        }

        /// <summary>
        /// Over ride the to string value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var response = new NameValueCollection();

            if (!string.IsNullOrEmpty(Problem))
            {
                response[Parameters.OAuth_Problem] = Problem;
            }

            if (!string.IsNullOrEmpty(ProblemAdvice))
            {
                response[Parameters.OAuth_Problem_Advice] = ProblemAdvice.Replace("\r\n", "\n").Replace("\r", "\n");
            }

            if (ParametersAbsent.Count > 0)
            {
                response[Parameters.OAuth_Parameters_Absent] = FormatParameterNames(ParametersAbsent);
            }

            if (ParametersRejected.Count > 0)
            {
                response[Parameters.OAuth_Parameters_Rejected] = FormatParameterNames(ParametersRejected);
            }

            if (AcceptableTimeStampsFrom.HasValue && AcceptableTimeStampsTo.HasValue)
            {
                response[Parameters.OAuth_Acceptable_Timestamps] = string.Format("{0}-{1}",
                                                                                 AcceptableTimeStampsFrom.Value.Epoch(),
                                                                                 AcceptableTimeStampsTo.Value.Epoch());
            }

            if (!(string.IsNullOrEmpty(AcceptableVersionFrom) || string.IsNullOrEmpty(AcceptableVersionTo)))
            {
                response[Parameters.OAuth_Acceptable_Versions] = string.Format("{0}-{1}", AcceptableVersionFrom,
                                                                               AcceptableVersionTo);
            }

            return UriUtility.FormatQueryString(response);
        }

        /// <summary>
        /// Format Parameter Names
        /// </summary>
        /// <param name="names">The parameter list.</param>
        /// <returns>The formatted parameter list.</returns>
        static string FormatParameterNames(IEnumerable<string> names)
        {
            var builder = new StringBuilder();

            foreach (string name in names)
            {
                if (builder.Length > 0) builder.Append("&");
                builder.Append(UriUtility.UrlEncode(name));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Parse Formatted Parameters
        /// </summary>
        /// <param name="formattedList">The formatted list.</param>
        /// <returns>The parameter list.</returns>
        static List<string> ParseFormattedParameters(string formattedList)
        {
            return formattedList.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
