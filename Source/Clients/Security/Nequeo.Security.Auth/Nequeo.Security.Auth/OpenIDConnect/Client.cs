/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Thinktecture.IdentityModel;
using Thinktecture.IdentityModel.Client;

namespace Nequeo.Security.Auth.OpenIDConnect
{
    /// <summary>
    /// OpenID Connect authorisation client.
    /// </summary>
    public sealed class Client : IDisposable
    {
        /// <summary>
        /// OpenID Connect authorisation client.
        /// </summary>
        public Client() { }

        /// <summary>
        /// OpenID Connect authorisation client.
        /// </summary>
        /// <param name="baseAddress">The base address of the authority.</param>
        public Client(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        private string _baseAddress = null;
        private string _baseServiceAddress = null;

        private string _authorizeEndpoint = "/connect/authorize";
        private string _tokenEndpoint = "/connect/token";
        private string _userInfoEndpoint = "/connect/userinfo";
        private string _identityTokenValidationEndpoint = "/connect/identitytokenvalidation";
        private string _tokenRevocationEndpoint = "/connect/revocation";

        private string _discoveryWebKeysEndpoint = ".well-known/jwks";
        private string _discoveryConfigurationEndpoint = ".well-known/openid-configuration";

        private string _accessTokenValidationEndpoint = "connect/accessTokenValidation";
        private string _consentEndpoint = "connect/consent";
        private string _switchUserEndpoint = "connect/switch";

        /// <summary>
        /// Gets or sets the discovery web keys of the authority.
        /// </summary>
        public string DiscoveryWebKeysEndpoint
        {
            get { return _discoveryWebKeysEndpoint; }
            set { _discoveryWebKeysEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the discovery configuration of the authority.
        /// </summary>
        public string DiscoveryConfigurationEndpoint
        {
            get { return _discoveryConfigurationEndpoint; }
            set { _discoveryConfigurationEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the access token validation of the authority.
        /// </summary>
        public string AccessTokenValidationEndpoint
        {
            get { return _accessTokenValidationEndpoint; }
            set { _accessTokenValidationEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the consent of the authority.
        /// </summary>
        public string ConsentEndpoint
        {
            get { return _consentEndpoint; }
            set { _consentEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the switch user of the authority.
        /// </summary>
        public string SwitchUserEndpoint
        {
            get { return _switchUserEndpoint; }
            set { _switchUserEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the base address of the authority.
        /// </summary>
        public string BaseAddress
        {
            get { return _baseAddress; }
            set { _baseAddress = value; }
        }

        /// <summary>
        /// Gets or sets the base service address of the authority.
        /// </summary>
        public string BaseServiceAddress
        {
            get { return _baseServiceAddress; }
            set { _baseServiceAddress = value; }
        }

        /// <summary>
        /// Gets or sets the authorize endpoint of the authority.
        /// </summary>
        public string AuthorizeEndpoint
        {
            get { return _authorizeEndpoint; }
            set { _authorizeEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the token endpoint of the authority.
        /// </summary>
        public string TokenEndpoint
        {
            get { return _tokenEndpoint; }
            set { _tokenEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the user info endpoint of the authority.
        /// </summary>
        public string UserInfoEndpoint
        {
            get { return _userInfoEndpoint; }
            set { _userInfoEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the token identity token validation of the authority.
        /// </summary>
        public string IdentityTokenValidationEndpoint
        {
            get { return _identityTokenValidationEndpoint; }
            set { _identityTokenValidationEndpoint = value; }
        }

        /// <summary>
        /// Gets or sets the token revocatio endpoint of the authority.
        /// </summary>
        public string TokenRevocationEndpoint
        {
            get { return _tokenRevocationEndpoint; }
            set { _tokenRevocationEndpoint = value; }
        }

        /// <summary>
        /// Create authorise url.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="responseTypes">The response types <see cref="SignOn.GetIDTokenResponseTypes"/>.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="redirectUri">The redirect url.</param>
        /// <param name="state">The state.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="loginHint">The login hint.</param>
        /// <param name="acrValues">The acr values (idp:Google b c).</param>
        /// <param name="responseMode">The response mode (form_post).</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <returns>The authosise URL.</returns>
        public string CreateAuthoriseUrl(
            string clientId,
            string[] responseTypes,
            string[] scopes = null,
            string redirectUri = null,
            string state = "random_state",
            string nonce = "random_nonce",
            string loginHint = null,
            string acrValues = null,
            string responseMode = null,
            Dictionary<string, string> additionalValues = null)
        {
            // Create a request.
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _authorizeEndpoint.Trim('/')));
            return request.CreateAuthorizeUrl(clientId, String.Join(" ", responseTypes), (scopes == null ? null : String.Join(" ", scopes)),
                redirectUri, state, nonce, loginHint, acrValues, responseMode, additionalValues);
        }

        /// <summary>
        /// Create code flow url.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="redirectUri">The redirect url.</param>
        /// <param name="state">The state.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="loginHint">The login hint.</param>
        /// <param name="acrValues">The acr values (idp:Google b c).</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <returns>The code flow URL.</returns>
        public string CreateCodeFlowUrl(
            string clientId,
            string[] scopes = null, 
            string redirectUri = null,
            string state = "random_state",
            string nonce = "random_nonce",
            string loginHint = null, 
            string acrValues = null, 
            Dictionary<string, string> additionalValues = null)
        {
            // Create a request.
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _authorizeEndpoint.Trim('/')));
            return request.CreateCodeFlowUrl(clientId, (scopes == null ? null : String.Join(" ", scopes)),
                redirectUri, state, nonce, loginHint, acrValues, additionalValues);
        }

        /// <summary>
        /// Create implicit flow url.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="redirectUri">The redirect url.</param>
        /// <param name="state">The state.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="loginHint">The login hint.</param>
        /// <param name="acrValues">The acr values (idp:Google b c).</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <returns>The implicit flow URL.</returns>
        public string CreateImplicitFlowUrl(
            string clientId,
            string[] scopes = null, 
            string redirectUri = null,
            string state = "random_state",
            string nonce = "random_nonce",
            string loginHint = null, 
            string acrValues = null, 
            Dictionary<string, string> additionalValues = null)
        {
            // Create a request.
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _authorizeEndpoint.Trim('/')));
            return request.CreateImplicitFlowUrl(clientId, (scopes == null ? null : String.Join(" ", scopes)),
                redirectUri, state, nonce, loginHint, acrValues, additionalValues);
        }

        /// <summary>
        /// Get the authorisation response.
        /// </summary>
        /// <param name="completeUri">The complete uri string returned after a request has been made.</param>
        /// <returns>The authosisation response.</returns>
        public AuthoriseResponse GetAuthoriseResponse(string completeUri)
        {
            return new AuthoriseResponse(completeUri);
        }

        /// <summary>
        /// Get the json user information.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="accessToken">The access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task; the type; else null.</returns>
        public async Task<T> GetUserInfo<T>(string accessToken = null, Action<string> responseStatus = null, 
            CancellationToken cancellationToken = default(CancellationToken)) where T : class, new()
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(_baseAddress.TrimEnd('/') + "/" + _userInfoEndpoint.Trim('/'));

                // If a access token exists.
                if (!String.IsNullOrEmpty(accessToken))
                {
                    // Add the bearer access token.
                    httpClient.SetBearerToken(accessToken);
                }

                // Get the response.
                HttpResponseMessage response = await httpClient.GetAsync("", cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the json user information.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns></returns>
        public async Task<object> GetUserInfo(string accessToken = null, Action<string> responseStatus = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(_baseAddress.TrimEnd('/') + "/" + _userInfoEndpoint.Trim('/'));

                // If a access token exists.
                if (!String.IsNullOrEmpty(accessToken))
                {
                    // Add the bearer access token.
                    httpClient.SetBearerToken(accessToken);
                }

                // Get the response.
                HttpResponseMessage response = await httpClient.GetAsync("", cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the user information.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task; the type; else null.</returns>
        public async Task<string> GetUserInfoEx(string accessToken = null, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(_baseAddress.TrimEnd('/') + "/" + _userInfoEndpoint.Trim('/'));

                // If a access token exists.
                if (!String.IsNullOrEmpty(accessToken))
                {
                    // Add the bearer access token.
                    httpClient.SetBearerToken(accessToken);
                }

                // Get the response.
                HttpResponseMessage response = await httpClient.GetAsync("", cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the json service information.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="authoriseResponse">The response access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task; the type; else null.</returns>
        public async Task<T> GetServiceInfo<T>(AuthoriseResponse authoriseResponse = null, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : class, new()
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(_baseServiceAddress.TrimEnd('/') + "/");

                // If a access token exists.
                if (authoriseResponse != null && authoriseResponse.Values.ContainsKey(SignOn.AccessTokenValue))
                {
                    // Add the bearer access token.
                    httpClient.SetBearerToken(authoriseResponse.AccessToken);
                }

                // Get the response.
                HttpResponseMessage response = await httpClient.GetAsync(SignOn.IdentityValue, cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the json service information.
        /// </summary>
        /// <param name="authoriseResponse">The response access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns></returns>
        public async Task<object> GetServiceInfo(AuthoriseResponse authoriseResponse = null, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(_baseServiceAddress.TrimEnd('/') + "/");

                // If a access token exists.
                if (authoriseResponse != null && authoriseResponse.Values.ContainsKey(SignOn.AccessTokenValue))
                {
                    // Add the bearer access token.
                    httpClient.SetBearerToken(authoriseResponse.AccessToken);
                }

                // Get the response.
                HttpResponseMessage response = await httpClient.GetAsync(SignOn.IdentityValue, cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the service information.
        /// </summary>
        /// <param name="responseStatus">The response access token.</param>
        /// <param name="authoriseResponse">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task; the type; else null.</returns>
        public async Task<string> GetServiceInfoEx(AuthoriseResponse authoriseResponse = null, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the base address.
                httpClient.BaseAddress = new Uri(_baseServiceAddress.TrimEnd('/') + "/");

                // If a access token exists.
                if (authoriseResponse != null && authoriseResponse.Values.ContainsKey(SignOn.AccessTokenValue))
                {
                    // Add the bearer access token.
                    httpClient.SetBearerToken(authoriseResponse.AccessToken);
                }

                // Get the response.
                HttpResponseMessage response = await httpClient.GetAsync(SignOn.IdentityValue, cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }


        /// <summary>
        /// Get json validation ID token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="authoriseResponse">The response access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task; the type; else null.</returns>
        public async Task<T> ValidateIdToken<T>(string clientId, AuthoriseResponse authoriseResponse, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : class, new()
        {
            // If a access token exists.
            if (authoriseResponse != null && authoriseResponse.Values.ContainsKey(SignOn.IDTokenResponseType))
            {
                // Create a http client.
                using (HttpClient httpClient = new HttpClient())
                {
                    // Get the response.
                    HttpResponseMessage response = await httpClient.GetAsync(
                        _baseAddress.TrimEnd('/') + "/" + _identityTokenValidationEndpoint.Trim('/') + 
                        "?" + SignOn.AccessTokenResponseType + "=" + authoriseResponse.Values[SignOn.IDTokenResponseType] + "&" + SignOn.LoginScope + "=" + clientId, cancellationToken);

                    // If OK.
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Send the response status.
                        responseStatus?.Invoke(response.StatusCode.ToString());

                        // Get the json response.
                        var json = await response.Content.ReadAsStringAsync();
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        // Send the response status.
                        responseStatus?.Invoke(response.StatusCode.ToString());

                        // Return nothing.
                        return null;
                    }
                }
            }
            else
            {
                // Send the response status.
                responseStatus?.Invoke("ID Token Not Found");

                // Return nothing.
                return null;
            }
        }

        /// <summary>
        /// Get json validation ID token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="authoriseResponse">The response access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns></returns>
        public async Task<object> ValidateIdToken(string clientId, AuthoriseResponse authoriseResponse, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // If a access token exists.
            if (authoriseResponse != null && authoriseResponse.Values.ContainsKey(SignOn.IDTokenResponseType))
            {
                // Create a http client.
                using (HttpClient httpClient = new HttpClient())
                {
                    // Get the response.
                    HttpResponseMessage response = await httpClient.GetAsync(
                        _baseAddress.TrimEnd('/') + "/" + _identityTokenValidationEndpoint.Trim('/') +
                        "?" + SignOn.AccessTokenResponseType + "=" + authoriseResponse.Values[SignOn.IDTokenResponseType] + "&" + SignOn.LoginScope + "=" + clientId, cancellationToken);

                    // If OK.
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Send the response status.
                        responseStatus?.Invoke(response.StatusCode.ToString());

                        // Get the json response.
                        var json = await response.Content.ReadAsStringAsync();
                        return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    }
                    else
                    {
                        // Send the response status.
                        responseStatus?.Invoke(response.StatusCode.ToString());

                        // Return nothing.
                        return null;
                    }
                }
            }
            else
            {
                // Send the response status.
                responseStatus?.Invoke("ID Token Not Found");

                // Return nothing.
                return null;
            }
        }

        /// <summary>
        /// Get validation ID token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="authoriseResponse">The response access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task; the type; else null.</returns>
        public async Task<string> ValidateIdTokenEx(string clientId, AuthoriseResponse authoriseResponse, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // If a access token exists.
            if (authoriseResponse != null && authoriseResponse.Values.ContainsKey(SignOn.IDTokenResponseType))
            {
                // Create a http client.
                using (HttpClient httpClient = new HttpClient())
                {
                    // Get the response.
                    HttpResponseMessage response = await httpClient.GetAsync(
                        _baseAddress.TrimEnd('/') + "/" + _identityTokenValidationEndpoint.Trim('/') +
                        "?" + SignOn.AccessTokenResponseType + "=" + authoriseResponse.Values[SignOn.IDTokenResponseType] + "&" + SignOn.LoginScope + "=" + clientId, cancellationToken);

                    // If OK.
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Send the response status.
                        responseStatus?.Invoke(response.StatusCode.ToString());

                        // Get the json response.
                        var data = await response.Content.ReadAsStringAsync();
                        return data;
                    }
                    else
                    {
                        // Send the response status.
                        responseStatus?.Invoke(response.StatusCode.ToString());

                        // Return nothing.
                        return null;
                    }
                }
            }
            else
            {
                // Send the response status.
                responseStatus?.Invoke("ID Token Not Found");

                // Return nothing.
                return null;
            }
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="code">The access code.</param>
        /// <param name="redirectUri">The redirect url.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestToken(string clientId, string clientSecret, string code, string redirectUri, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestAuthorizationCodeAsync(code, redirectUri, additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="code">The access code.</param>
        /// <param name="redirectUri">The redirect url.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestToken<T>(string clientId, string clientSecret, string code, string redirectUri, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestAuthorizationCodeAsync(code, redirectUri, additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestToken(string clientId, string clientSecret, string userName, string password, string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestResourceOwnerPasswordAsync(userName, password, (scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestToken<T>(string clientId, string clientSecret, string userName, string password, string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestResourceOwnerPasswordAsync(userName, password, (scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestToken(X509Certificate2 clientCertificate, string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the client certificate.
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(clientCertificate);

            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), handler);
            TokenResponse response = await request.RequestClientCredentialsAsync((scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientCertificate">The client certificate.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestToken<T>(X509Certificate2 clientCertificate, string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the client certificate.
            var handler = new WebRequestHandler();
            handler.ClientCertificates.Add(clientCertificate);

            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), handler);
            TokenResponse response = await request.RequestClientCredentialsAsync((scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestToken(string clientId, string clientSecret, string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestClientCredentialsAsync((scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestToken<T>(string clientId, string clientSecret, string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestClientCredentialsAsync((scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="style">The authentication style.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestToken(string clientId, string clientSecret, AuthenticationStyle style, 
            string[] scopes = null, Dictionary<string, string> additionalValues = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret, AuthenticationStyleHelper.GetAuthenticationStyle(style));
            TokenResponse response = await request.RequestClientCredentialsAsync((scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="style">The authentication style.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestToken<T>(string clientId, string clientSecret, AuthenticationStyle style,
            string[] scopes = null, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret, AuthenticationStyleHelper.GetAuthenticationStyle(style));
            TokenResponse response = await request.RequestClientCredentialsAsync((scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RefreshToken(string clientId, string clientSecret, string refreshToken, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestRefreshTokenAsync(refreshToken, additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RefreshToken<T>(string clientId, string clientSecret, string refreshToken, Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestRefreshTokenAsync(refreshToken, additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Revoke access token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RevokeAccessToken<T>(string clientId, string clientSecret, string accessToken, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : class, new()
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Add the basic auth access token.
                httpClient.SetBasicAuthentication(clientId, clientSecret);

                // Post body data.
                var postBody = new Dictionary<string, string>
                {
                    { SignOn.AccessTokenResponseType, accessToken },
                    { SignOn.TokenTypeHintValue, SignOn.AccessTokenValue }
                };

                // Get the response.
                HttpResponseMessage response = await httpClient.PostAsync(
                    _baseAddress.TrimEnd('/') + "/" + _tokenRevocationEndpoint.Trim('/'), new FormUrlEncodedContent(postBody), cancellationToken);

                // If OK.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Revoke access token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<object> RevokeAccessToken(string clientId, string clientSecret, string accessToken, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Add the basic auth access token.
                httpClient.SetBasicAuthentication(clientId, clientSecret);

                // Post body data.
                var postBody = new Dictionary<string, string>
                {
                    { SignOn.AccessTokenResponseType, accessToken },
                    { SignOn.TokenTypeHintValue, SignOn.AccessTokenValue }
                };

                // Get the response.
                HttpResponseMessage response = await httpClient.PostAsync(
                    _baseAddress.TrimEnd('/') + "/" + _tokenRevocationEndpoint.Trim('/'), new FormUrlEncodedContent(postBody), cancellationToken);

                // If OK.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Revoke access token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RevokeAccessTokenEx(string clientId, string clientSecret, string accessToken, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Add the basic auth access token.
                httpClient.SetBasicAuthentication(clientId, clientSecret);

                // Post body data.
                var postBody = new Dictionary<string, string>
                {
                    { SignOn.AccessTokenResponseType, accessToken },
                    { SignOn.TokenTypeHintValue, SignOn.AccessTokenValue }
                };

                // Get the response.
                HttpResponseMessage response = await httpClient.PostAsync(
                    _baseAddress.TrimEnd('/') + "/" + _tokenRevocationEndpoint.Trim('/'), new FormUrlEncodedContent(postBody), cancellationToken);

                // If OK.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Revoke refresh token.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RevokeRefreshToken<T>(string clientId, string clientSecret, string refreshToken, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken)) where T : class, new()
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Add the basic auth access token.
                httpClient.SetBasicAuthentication(clientId, clientSecret);

                // Post body data.
                var postBody = new Dictionary<string, string>
                {
                    { SignOn.AccessTokenResponseType, refreshToken },
                    { SignOn.TokenTypeHintValue, SignOn.RefreshTokenValue }
                };

                // Get the response.
                HttpResponseMessage response = await httpClient.PostAsync(
                    _baseAddress.TrimEnd('/') + "/" + _tokenRevocationEndpoint.Trim('/'), new FormUrlEncodedContent(postBody), cancellationToken);

                // If OK.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Revoke refresh token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<object> RevokeRefreshToken(string clientId, string clientSecret, string refreshToken, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Add the basic auth access token.
                httpClient.SetBasicAuthentication(clientId, clientSecret);

                // Post body data.
                var postBody = new Dictionary<string, string>
                {
                    { SignOn.AccessTokenResponseType, refreshToken },
                    { SignOn.TokenTypeHintValue, SignOn.RefreshTokenValue }
                };

                // Get the response.
                HttpResponseMessage response = await httpClient.PostAsync(
                    _baseAddress.TrimEnd('/') + "/" + _tokenRevocationEndpoint.Trim('/'), new FormUrlEncodedContent(postBody), cancellationToken);

                // If OK.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var json = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Revoke refresh token.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="responseStatus">The response status callback.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RevokeRefreshTokenEx(string clientId, string clientSecret, string refreshToken, Action<string> responseStatus = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create a http client.
            using (HttpClient httpClient = new HttpClient())
            {
                // Add the basic auth access token.
                httpClient.SetBasicAuthentication(clientId, clientSecret);

                // Post body data.
                var postBody = new Dictionary<string, string>
                {
                    { SignOn.AccessTokenResponseType, refreshToken },
                    { SignOn.TokenTypeHintValue, SignOn.RefreshTokenValue }
                };

                // Get the response.
                HttpResponseMessage response = await httpClient.PostAsync(
                    _baseAddress.TrimEnd('/') + "/" + _tokenRevocationEndpoint.Trim('/'), new FormUrlEncodedContent(postBody), cancellationToken);

                // If OK.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Get the json response.
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    // Send the response status.
                    responseStatus?.Invoke(response.StatusCode.ToString());

                    // Return nothing.
                    return null;
                }
            }
        }

        /// <summary>
        /// Request token custom.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestTokenCustom(string clientId, string clientSecret, string[] scopes = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Add values
            var additionalValues = new Dictionary<string, string>
                {
                    { SignOn.CustomCredentialValue,String.Join(" ", SignOn.GetCustomCredentialGrants()) }
                };

            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestCustomGrantAsync(SignOn.CustomGrantType, (scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token custom.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestTokenCustom<T>(string clientId, string clientSecret, string[] scopes = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Add values
            var additionalValues = new Dictionary<string, string>
                {
                    { SignOn.CustomCredentialValue, String.Join(" ", SignOn.GetCustomCredentialGrants()) }
                };

            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestCustomGrantAsync(SignOn.CustomGrantType, (scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        /// <summary>
        /// Request token custom.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<string> RequestTokenCustom(string clientId, string clientSecret, string userName, string password, string[] scopes = null,
            Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestResourceOwnerPasswordAsync(userName, password, (scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToString();
        }

        /// <summary>
        /// Request token custom.
        /// </summary>
        /// <typeparam name="T">The type model.</typeparam>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="scopes">The scopes for this request.</param>
        /// <param name="additionalValues">Additional values.</param>
        /// <param name="cancellationToken">Async cancellation token.</param>
        /// <returns>The async task.</returns>
        public async Task<T> RequestTokenCustom<T>(string clientId, string clientSecret, string userName, string password, string[] scopes = null, 
            Dictionary<string, string> additionalValues = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new OAuth2Client(new Uri(_baseAddress.TrimEnd('/') + "/" + _tokenEndpoint.Trim('/')), clientId, clientSecret);
            TokenResponse response = await request.RequestResourceOwnerPasswordAsync(userName, password, (scopes == null ? null : String.Join(" ", scopes)), additionalValues, cancellationToken);
            return response.Json.ToObject<T>();
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.  If disposing
        /// equals true, the method has been called directly or indirectly by a user's
        /// code. Managed and unmanaged resources can be disposed.  If disposing equals
        /// false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can
        /// be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Client()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
