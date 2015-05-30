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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Drawing.Design;

using Nequeo.Handler;
using Nequeo.Web.UI.Control.Design;
using Nequeo.Invention;

namespace Nequeo.Web.UI.Control
{
    /// <summary>
    /// Captcha image web control.
    /// </summary>
    [ToolboxBitmap(typeof(Captcha))]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Captcha runat=\"server\" CaptchaHeight=\"100px\" CaptchaWidth=\"300px\"></{0}:Captcha>")]
    [Description("Nequeo captcha web control")]
    [DisplayName("Captcha Web Control")]
    [Designer(typeof(CaptchaDesigner))]
    public class Captcha : WebControl, INamingContainer, IPostBackDataHandler
    {
        #region Private Constants
        private const int EXPIRATION_DEFAULT = 120;
        private const int LENGTH_DEFAULT = 6;
        private const string RENDERURL_DEFAULT = "captcha.aspx";
        internal const string KEY = "captcha";
        #endregion

        #region Private Fields
        private System.Web.UI.WebControls.Image _image;

        private Color _backGroundColor = Color.Transparent;
        private Unit _captchaHeight = Unit.Pixel(100);
        private Unit _captchaWidth = Unit.Pixel(300);

        private Style _errorStyle = new Style();
        private Style _textBoxStyle = new Style();

        private string _backGroundImage = string.Empty;
      
        private int _captchaLength = LENGTH_DEFAULT;
        private string _captchaText = string.Empty;
        private string _errorMessage = "Invalid Captcha";
        
        private int _expiration = EXPIRATION_DEFAULT;
        private bool _isValid = false;
        private string _renderUrl = RENDERURL_DEFAULT;
        private string _instructions = "Enter the code shown above:";
        private string _userText = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, the view state text value.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }
            set { ViewState["Text"] = value; }
        }

        /// <summary>
        /// Gets sets, the Background Color to use for the Captcha Image.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Appearance")]
        [Description("The Background Color to use for the Captcha Image.")]
        public Color BackGroundColor
        {
            get { return _backGroundColor; }
            set { _backGroundColor = value; }
        }

        /// <summary>
        /// Gets sets, a Background Image to use for the Captcha Image.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("A Background Image to use for the Captcha Image.")]
        [Editor(typeof(System.Web.UI.Design.ImageUrlEditor), typeof(UITypeEditor))]
        [UrlProperty()]
        public string BackGroundImage
        {
            get { return _backGroundImage; }
            set { _backGroundImage = value; }
        }

        /// <summary>
        /// Gets sets, the Height of Captcha Image.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Appearance")]
        [Description("The Height of Captcha Image.")]
        public Unit CaptchaHeight
        {
            get { return _captchaHeight; }
            set { _captchaHeight = value; }
        }

        /// <summary>
        /// Gets sets, the Width of Captcha Image.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Appearance")]
        [Description("The Width of Captcha Image.")]
        public Unit CaptchaWidth
        {
            get { return _captchaWidth; }
            set { _captchaWidth = value; }
        }

        /// <summary>
        /// Gets sets, the Number of Captcha characters used in the CAPTCHA text
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(6)]
        [Category("Appearance")]
        [Description("The Number of Captcha characters used in the CAPTCHA text")]
        public int CaptchaLength
        {
            get { return _captchaLength; }
            set { _captchaLength = value; }
        }

        /// <summary>
        /// Gets sets, the Error Message to display if invalid.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("Invalid Captcha")]
        [Category("Behavior")]
        [Description("The Error Message to display if invalid.")]
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        /// <summary>
        /// Gets, the Style for the Error Message Control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Style")]
        [Description("The Style for the Error Message Control.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style ErrorStyle
        {
            get { return _errorStyle; }
        }

        /// <summary>
        /// Gets sets, the duration of time (seconds) a user has before the challenge expires.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(120)]
        [Category("Behavior")]
        [Description("The duration of time (seconds) a user has before the challenge expires.")]
        public int Expiration
        {
            get { return _expiration; }
            set { _expiration = value; }
        }

        /// <summary>
        /// Gets, True if the user was CAPTCHA validated after a postback.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(120)]
        [Category("Validation")]
        [Description("True if the user was CAPTCHA validated after a postback.")]
        [Browsable(false)]
        public bool IsValid
        {
            get { return _isValid; }
        }

        /// <summary>
        /// Gets sets, the URL used to render the image to the client.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("captcha.aspx")]
        [Category("Behavior")]
        [Description("The URL used to render the image to the client.")]
        public string RenderUrl
        {
            get { return _renderUrl; }
            set { _renderUrl = value; }
        }

        /// <summary>
        /// Gets sets, instructional text displayed next to CAPTCHA image.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("Enter the code shown above:")]
        [Category("Captcha")]
        [Description("Instructional text displayed next to CAPTCHA image.")]
        public string Instructions
        {
            get { return _instructions; }
            set { _instructions = value; }
        }

        /// <summary>
        /// Gets, the Style for the Text Box Control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Style")]
        [Description("The Style for the Text Box Control.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public Style TextBoxStyle
        {
            get { return _textBoxStyle; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// User validation event handler.
        /// </summary>
        public event ServerValidateEventHandler UserValidated;

        /// <summary>
        /// General error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> Error;

        #endregion

        #region Virtual Event Methods
        /// <summary>
        /// User validated.
        /// </summary>
        /// <param name="e">Validation arguments</param>
        protected virtual void OnUserValidated(ServerValidateEventArgs e)
        {
            if (UserValidated != null)
                UserValidated(this, e);
        }

        /// <summary>
        /// General error.
        /// </summary>
        /// <param name="e">Error message arguments.</param>
        protected virtual void OnError(ErrorMessageArgs e)
        {
            if (Error != null)
                Error(this, e);
        }
        #endregion

        #region Create Control Methods
        /// <summary>
        /// Creates the child controls.
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                base.CreateChildControls();

                // If the height and width have been set.
                if (CaptchaWidth.IsEmpty || CaptchaWidth.Type != UnitType.Pixel ||
                        CaptchaHeight.IsEmpty || CaptchaHeight.Type != UnitType.Pixel)
                {
                    throw new InvalidOperationException("Must specify size of control in pixels.");
                }

                // Create the image control.
                _image = new System.Web.UI.WebControls.Image();
                _image.BorderColor = this.BorderColor;
                _image.BorderStyle = this.BorderStyle;
                _image.BorderWidth = this.BorderWidth;
                _image.ToolTip = this.ToolTip;
                _image.EnableViewState = false;
                Controls.Add(_image);
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Loads the previously saved Viewstate
        /// </summary>
        /// <param name="savedState">The saved state.</param>
        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                // Load State from the array of objects that was saved at SaveViewState.
                object[] states = (object[])savedState;

                // Load the ViewState of the Base Control
                if (states[0] != null)
                    base.LoadViewState(states[0]);

                // Load the CAPTCHA Text from the ViewState
                if (states[1] != null)
                    _captchaText = states[1].ToString();
            }
        }

        /// <summary>
        /// Save the controls Viewstate
        /// </summary>
        /// <returns>The array of view states.</returns>
        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[2];

            allStates[0] = baseState;
            if (String.IsNullOrEmpty(_captchaText))
                _captchaText = GetNextCaptcha();

            allStates[1] = _captchaText;
            return allStates;
        }
        #endregion

        #region Control Contents Render Methods
        /// <summary>
        /// Runs just before the control is to be rendered
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            // Generate Random Challenge Text
            _captchaText = GetNextCaptcha();

            // Enable Viewstate Encryption
            Page.RegisterRequiresViewStateEncryption();

            // Call Base Class method
            base.OnPreRender(e);
        }

        /// <summary>
        /// Render the control
        /// </summary>
        /// <param name="output">An Html Text Writer.</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            try
            {
                ControlStyle.AddAttributesToRender(output);
                output.RenderBeginTag(HtmlTextWriterTag.Div);

                // Render image <img> Tag
                output.AddAttribute(HtmlTextWriterAttribute.Src, GetUrl());
                output.AddAttribute(HtmlTextWriterAttribute.Border, "0");

                if (ToolTip.Length > 0)
                    output.AddAttribute(HtmlTextWriterAttribute.Alt, ToolTip);
                else
                    output.AddAttribute(HtmlTextWriterAttribute.Alt, "CaptchaAlt.Text");

                output.RenderBeginTag(HtmlTextWriterTag.Img);
                output.RenderEndTag();

                // Render Help Text
                if (_instructions.Length > 0)
                {
                    output.RenderBeginTag(HtmlTextWriterTag.Div);
                    output.Write(_instructions);
                    output.RenderEndTag();
                }

                // Render text box <input> Tag
                TextBoxStyle.AddAttributesToRender(output);
                output.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                output.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + Width.ToString());
                output.AddAttribute(HtmlTextWriterAttribute.Maxlength, _captchaText.Length.ToString());
                output.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);

                if (AccessKey.Length > 0)
                    output.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey);

                if (!Enabled)
                    output.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

                if (TabIndex > 0)
                    output.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString());

                if (_userText.ToLower() == _captchaText.ToLower())
                    output.AddAttribute(HtmlTextWriterAttribute.Value, _userText);
                else
                    output.AddAttribute(HtmlTextWriterAttribute.Value, "");

                output.RenderBeginTag(HtmlTextWriterTag.Input);
                output.RenderEndTag();

                // Write an error message if the user entered the wrong
                // text for the post back operation.
                if (!IsValid && Page.IsPostBack && !String.IsNullOrEmpty(_userText))
                {
                    ErrorStyle.AddAttributesToRender(output);
                    output.RenderBeginTag(HtmlTextWriterTag.Div);
                    output.Write(ErrorMessage);
                    output.RenderEndTag();
                }

                // Render </div>
                output.RenderEndTag();
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Builds the url for the Handler.
        /// </summary>
        /// <returns>The encoded url.</returns>
        private string GetUrl()
        {
            string url = ResolveUrl(RenderUrl);
            url += "?" + KEY + "=" + WebManager.Encrypt(EncodeTicket(), DateTime.Now.AddSeconds(Expiration));
            return url;
        }

        /// <summary>
        /// Encodes the querystring to pass to the Handler.
        /// </summary>
        /// <returns>The encoded ticket.</returns>
        private string EncodeTicket()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CaptchaWidth.Value.ToString());
            sb.Append(WebManager._separator + CaptchaHeight.Value.ToString());
            sb.Append(WebManager._separator + _captchaText);
            sb.Append(WebManager._separator + BackGroundImage);
            return sb.ToString();
        }

        /// <summary>
        /// Validates the posted back data
        /// </summary>
        /// <param name="userData">The user entered data.</param>
        /// <returns>True if valid else false.</returns>
        private bool Validate(string userData)
        {
            // Determine if the user text is the same as the generated text.
            if (String.Compare(userData, _captchaText, false, CultureInfo.InvariantCulture) == 0)
                _isValid = true;
            else
                _isValid = false;

            // Send a message back to the user.
            OnUserValidated(new ServerValidateEventArgs(_captchaText, _isValid));
            return _isValid;
        }

        /// <summary>
        /// Gets the next Captcha
        /// </summary>
        /// <returns>the new captcha text.</returns>
        protected virtual string GetNextCaptcha()
        {
            PasswordStandardGenerator captchaKey = new PasswordStandardGenerator();
            return captchaKey.Random(_captchaLength);
        }
        #endregion

        #region IPost back data handler members
        /// <summary>
        /// When implemented by a class, processes postback data 
        /// for an ASP.NET server control.
        /// </summary>
        /// <param name="postDataKey">The key identifier for the control.</param>
        /// <param name="postCollection">The collection of all incoming name values.</param>
        /// <returns>true if the server control's state changes as a result of the postback; otherwise, false.</returns>
        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            _userText = postCollection[postDataKey];
            bool ret = Validate(_userText);

            if (!_isValid && !String.IsNullOrEmpty(_userText))
                // Generate Random Challenge Text.
                _captchaText = GetNextCaptcha();

            return false;
        }

        /// <summary>
        /// RaisePostDataChangedEvent runs when the PostBackData has changed.
        /// </summary>
        public void RaisePostDataChangedEvent()
        {
        }
        #endregion
    }
}
