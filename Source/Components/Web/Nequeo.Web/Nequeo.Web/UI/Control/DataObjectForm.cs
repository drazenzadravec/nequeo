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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Compilation;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

using Nequeo.ComponentModel;
using Nequeo.Web.UI.Control.Design;
using Nequeo.Handler;
using Nequeo.Web.Conversion;
using Nequeo.Data;

namespace Nequeo.Web.UI.Control
{
    /// <summary>
    /// Data object form generator web control
    /// </summary>
    [ToolboxBitmap(typeof(DataObjectForm))]
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:DataObjectForm runat=\"server\"></{0}:DataObjectForm>")]
    [Description("Nequeo data object form web control")]
    [DisplayName("Data Object Form")]
    [Designer(typeof(DataObjectFormDesigner))]
    public class DataObjectForm : WebControl, INamingContainer, IPostBackDataHandler, IPostBackEventHandler
    {
        #region Private Fields
        private LinkButton _updateButton = new LinkButton();
        private LinkButton _insertButton = new LinkButton();
        private LinkButton _deleteButton = new LinkButton();

        private bool _allowUpdate = true;
        private bool _allowInsert = true;
        private bool _allowDelete = true;

        private bool _includeRequiredValidation = true;
        private bool _includeTypeExpressValidation = true;

        private ConnectionTypeModel _connectionTypeModel = null;
        private DataObjectColumnCollection _columns = null;
        private ITemplate _itemTemplate = null;

        private Object _dataObjectData = null;
        private string _scriptManagerID = string.Empty;

        private Style _captionStyle = new Style();
        private Style _textBoxStyle = new Style();
        private Style _textBoxPrimaryKeyStyle = new Style();
        private Style _requiredFieldValidationStyle = new Style();
        private Style _regularExpressionValidationStyle = new Style();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets sets, connection informations and data obejct type.
        /// </summary>
        [Category("Data")]
        [DefaultValue(null)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Editor(typeof(ConnectionTypeDropDownEditor), typeof(UITypeEditor))]
        [Description("Connection informations and data obejct type.")]
        public ConnectionTypeModel ConnectionTypeModel
        {
            get { return _connectionTypeModel; }
            set { _connectionTypeModel = value; }
        }

        /// <summary>
        /// Gets sets, the collection of data object type columns.
        /// </summary>
        [Category("Data")]
        [DefaultValue(null)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Editor(typeof(DataObjectColumnListModalEditor), typeof(UITypeEditor))]
        [Description("The collection of data object type columns.")]
        public DataObjectColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new DataObjectColumnCollection(ConnectionTypeModel.DataObjectTypeName);
                    if (IsTrackingViewState)
                        ((IStateManager)_columns).TrackViewState();
                }
                return _columns;
            }
        }

        /// <summary>
        /// Gets sets, the custom content for the data row in a 
        /// control when the control is in read-only mode.
        /// </summary>
        [TemplateContainer(typeof(DataObjectForm), BindingDirection.TwoWay)]
        [Browsable(false)]
        [DefaultValue("")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("the custom content for the data row in a control when the control is in read-only mode.")]
        public ITemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set { _itemTemplate = value;  }
        }

        /// <summary>
        /// Gets sets, allow data updates.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Allow data updates.")]
        public bool AllowUpdate
        {
            get { return _allowUpdate; }
            set { _allowUpdate = value; }
        }

        /// <summary>
        /// Gets sets, allow data insertion.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Allow data insertion.")]
        public bool AllowInsert
        {
            get { return _allowInsert; }
            set { _allowInsert = value; }
        }

        /// <summary>
        /// Gets sets, allow data deletion.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Allow data deletion.")]
        public bool AllowDelete
        {
            get { return _allowDelete; }
            set { _allowDelete = value; }
        }

        /// <summary>
        /// Gets sets, the script manager id, used for ajax post back.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("The script manager id, used for ajax post back.")]
        public string ScriptManagerID
        {
            get { return _scriptManagerID; }
            set { _scriptManagerID = value; }
        }

        /// <summary>
        /// Gets sets, should the control include required field validation controls.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Should the control include required field validation controls.")]
        public bool IncludeRequiredValidation
        {
            get { return _includeRequiredValidation; }
            set { _includeRequiredValidation = value; }
        }

        /// <summary>
        /// Gets sets, should the control include data type member regular expression validation controls.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Should the control include data type member regular expression validation controls.")]
        public bool IncludeTypeExpressValidation
        {
            get { return _includeTypeExpressValidation; }
            set { _includeTypeExpressValidation = value; }
        }

        /// <summary>
        /// Gets, the Style for the Required Field Validator Control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Style")]
        [Description("The Style for the Required Field Validator Control.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        public Style RequiredFieldValidationStyle
        {
            get { return _requiredFieldValidationStyle; }
        }

        /// <summary>
        /// Gets, the Style for the Regular Expression Validator Control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Style")]
        [Description("The Style for the Regular Expression Validator Control.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        public Style RegularExpressionValidationStyle
        {
            get { return _regularExpressionValidationStyle; }
        }

        /// <summary>
        /// Gets, the Style for the Caption Control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Style")]
        [Description("The Style for the Caption Control.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        public Style CaptionStyle
        {
            get { return _captionStyle; }
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
        [NotifyParentProperty(true)]
        public Style TextBoxStyle
        {
            get { return _textBoxStyle; }
        }

        /// <summary>
        /// Gets, The Style for the Primary Key Text Box Control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [Category("Style")]
        [Description("The Style for the Primary Key Text Box Control.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        public Style PrimaryKeyTextBoxStyle
        {
            get { return _textBoxPrimaryKeyStyle; }
        }

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
        /// Gets, the collection of child controls.
        /// </summary>
        [Browsable(false)]
        public override ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }

        /// <summary>
        /// Gets, the data object.
        /// </summary>
        [Browsable(false)]
        public Object DataObjectData
        {
            get { return _dataObjectData; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// General error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> Error;

        /// <summary>
        /// Internal selection error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> SelectError;

        /// <summary>
        /// Internal insertion error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> InsertError;

        /// <summary>
        /// Internal update error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> UpdateError;

        /// <summary>
        /// Internal deletion error event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> DeleteError;

        /// <summary>
        /// Internal insertion complete event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<int> InsertComplete;

        /// <summary>
        /// Internal update complete event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<int> UpdateComplete;

        /// <summary>
        /// Internal deletion complete event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<int> DeleteComplete;

        /// <summary>
        /// User insertion redirection event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Object> InsertRedirect;

        /// <summary>
        /// User update redirection event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Object> UpdateRedirect;

        /// <summary>
        /// User deletion redirection event handler.
        /// </summary>
        public event Nequeo.Threading.EventHandler<Object> DeleteRedirect;

        #endregion

        #region Virtual Event Methods
        /// <summary>
        /// General error.
        /// </summary>
        /// <param name="e">Error message arguments.</param>
        protected virtual void OnError(ErrorMessageArgs e)
        {
            if (Error != null)
                Error(this, e);
        }

        /// <summary>
        /// Internal insert error.
        /// </summary>
        /// <param name="e">Error message arguments</param>
        protected virtual void OnInsertError(ErrorMessageArgs e)
        {
            if (InsertError != null)
                InsertError(this, e);
        }

        /// <summary>
        /// Internal update error.
        /// </summary>
        /// <param name="e">Error message arguments</param>
        protected virtual void OnUpdateError(ErrorMessageArgs e)
        {
            if (UpdateError != null)
                UpdateError(this, e);
        }

        /// <summary>
        /// Internal delete error.
        /// </summary>
        /// <param name="e">Error message arguments</param>
        protected virtual void OnDeleteError(ErrorMessageArgs e)
        {
            if (DeleteError != null)
                DeleteError(this, e);
        }

        /// <summary>
        /// Internal select error.
        /// </summary>
        /// <param name="e">Error message arguments</param>
        protected virtual void OnSelectError(ErrorMessageArgs e)
        {
            if (SelectError != null)
                SelectError(this, e);
        }

        /// <summary>
        /// Internal insert complete.
        /// </summary>
        /// <param name="rowsAffected">The number of rows affected.</param>
        protected virtual void OnInsertComplete(int rowsAffected)
        {
            if (InsertComplete != null)
                InsertComplete(this, rowsAffected);
        }

        /// <summary>
        /// Internal update complete.
        /// </summary>
        /// <param name="rowsAffected">The number of rows affected.</param>
        protected virtual void OnUpdateComplete(int rowsAffected)
        {
            if (UpdateComplete != null)
                UpdateComplete(this, rowsAffected);
        }

        /// <summary>
        /// Internal delete complete.
        /// </summary>
        /// <param name="rowsAffected">The number of rows affected.</param>
        protected virtual void OnDeleteComplete(int rowsAffected)
        {
            if (DeleteComplete != null)
                DeleteComplete(this, rowsAffected);
        }

        /// <summary>
        /// User insert process redirection.
        /// </summary>
        /// <param name="dataObjectType">The data object type containing the data.</param>
        protected virtual void OnInsertRedirect(Object dataObjectType)
        {
            if (InsertRedirect != null)
                InsertRedirect(this, dataObjectType);
        }

        /// <summary>
        /// User update process redirection.
        /// </summary>
        /// <param name="dataObjectType">The data object type containing the data.</param>
        protected virtual void OnUpdateRedirect(Object dataObjectType)
        {
            if (UpdateRedirect != null)
                UpdateRedirect(this, dataObjectType);
        }

        /// <summary>
        /// User delete process redirection.
        /// </summary>
        /// <param name="dataObjectType">The data object type containing the data.</param>
        protected virtual void OnDeleteRedirect(Object dataObjectType)
        {
            if (DeleteRedirect != null)
                DeleteRedirect(this, dataObjectType);
        }
        #endregion

        #region Create Control Methods
        /// <summary>
        /// Register internal web controls within an Ajax script manager.
        /// </summary>
        public void RegisterForAjaxPostback()
        {
            if (!String.IsNullOrEmpty(_scriptManagerID))
            {
                AjaxManager.RegisterPostBackControl(_scriptManagerID, _updateButton);
                AjaxManager.RegisterPostBackControl(_scriptManagerID, _insertButton);
                AjaxManager.RegisterPostBackControl(_scriptManagerID, _deleteButton);
            }
        }

        /// <summary>
        /// Create the child controls on the data object form control
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                // Remove all the child controls in the collection.
                Controls.Clear();

                // Create a new type instance of the data object type
                // and get the collection of data object information.
                Type dataObjectType = BuildManager.GetType(_connectionTypeModel.DataObjectTypeName, true, true);
                List<DataObjectInfo> properties = GetDataObjectInfo(dataObjectType);

                // If the data obejct type contains properties
                if (properties.Count > 0)
                {
                    // For each property found in the data object type.
                    foreach (DataObjectInfo item in properties)
                    {
                        // Create the web control according
                        // to the data object property type.
                        System.Web.UI.WebControls.WebControl ctrl = WebControlType.CreateWebControl(item.Member.PropertyType);
                        ctrl.ID = item.Member.Name;
                        ctrl.ToolTip = item.Caption;

                        // If the property is a primary key.
                        if (item.IsPrimaryKey)
                        {
                            ctrl.Width = PrimaryKeyTextBoxStyle.Width;
                            ctrl.Height = PrimaryKeyTextBoxStyle.Height;
                            ctrl.BackColor = PrimaryKeyTextBoxStyle.BackColor;
                            ctrl.BorderColor = PrimaryKeyTextBoxStyle.BorderColor;
                            ctrl.BorderStyle = PrimaryKeyTextBoxStyle.BorderStyle;
                            ctrl.ForeColor = PrimaryKeyTextBoxStyle.ForeColor;
                            ctrl.Font.Bold = PrimaryKeyTextBoxStyle.Font.Bold;
                            ctrl.Font.Italic = PrimaryKeyTextBoxStyle.Font.Italic;
                            ctrl.Font.Size = PrimaryKeyTextBoxStyle.Font.Size;
                            ctrl.Font.Name = PrimaryKeyTextBoxStyle.Font.Name;
                        }
                        else
                        {
                            ctrl.Width = TextBoxStyle.Width;
                            ctrl.Height = TextBoxStyle.Height;
                            ctrl.BackColor = TextBoxStyle.BackColor;
                            ctrl.BorderColor = TextBoxStyle.BorderColor;
                            ctrl.BorderStyle = TextBoxStyle.BorderStyle;
                            ctrl.ForeColor = TextBoxStyle.ForeColor;
                            ctrl.Font.Bold = TextBoxStyle.Font.Bold;
                            ctrl.Font.Italic = TextBoxStyle.Font.Italic;
                            ctrl.Font.Size = TextBoxStyle.Font.Size;
                            ctrl.Font.Name = TextBoxStyle.Font.Name;
                        }

                        // If data is being passed for the data object type.
                        if (_dataObjectData != null)
                        {
                            // Get the porperty info and get the value for the property.
                            PropertyInfo propertyInfo = _dataObjectData.GetType().GetProperty(item.Member.Name);
                            object value = propertyInfo.GetValue(_dataObjectData, null);

                            // Assign the web control default value.
                            if (value != null)
                                WebControlType.AssignWebControlValue(ctrl, value, ctrl.GetType());
                        }

                        // Add the control to the collection.
                        Controls.Add(ctrl);

                        // Add a calendar control if it is a date time.
                        if (item.IsDateTime)
                            Controls.Add(WebControlType.CreateCalendarControl(ctrl));

                        // Is the property nullable.
                        if (!item.IsNullable)
                        {
                            // Create the required field validator according
                            // to the data object property type is nullbale.
                            BaseValidator valItem = WebControlType.CreateWebControlRequiredFieldValidator(item.Member.PropertyType);
                            if (valItem != null)
                            {
                                valItem.ID = "validator_Req_" + item.Member.Name;
                                valItem.ControlToValidate = item.Member.Name;
                                valItem.Width = RequiredFieldValidationStyle.Width;
                                valItem.Height = RequiredFieldValidationStyle.Height;
                                valItem.BackColor = RequiredFieldValidationStyle.BackColor;
                                valItem.BorderColor = RequiredFieldValidationStyle.BorderColor;
                                valItem.BorderStyle = RequiredFieldValidationStyle.BorderStyle;
                                valItem.ForeColor = RequiredFieldValidationStyle.ForeColor;
                                valItem.Font.Bold = RequiredFieldValidationStyle.Font.Bold;
                                valItem.Font.Italic = RequiredFieldValidationStyle.Font.Italic;
                                valItem.Font.Size = RequiredFieldValidationStyle.Font.Size;
                                valItem.Font.Name = RequiredFieldValidationStyle.Font.Name;
                                Controls.Add(valItem);
                            }
                        }

                        if (_includeTypeExpressValidation)
                        {
                            // Create the regular expression validator according
                            // to the data object property type.
                            BaseValidator valItemReg = WebControlType.CreateWebControlRegularExpressionValidator(item.Member.PropertyType);
                            if (valItemReg != null)
                            {
                                valItemReg.ID = "validator_Reg_" + item.Member.Name;
                                valItemReg.ControlToValidate = item.Member.Name;
                                valItemReg.Width = RegularExpressionValidationStyle.Width;
                                valItemReg.Height = RegularExpressionValidationStyle.Height;
                                valItemReg.BackColor = RegularExpressionValidationStyle.BackColor;
                                valItemReg.BorderColor = RegularExpressionValidationStyle.BorderColor;
                                valItemReg.BorderStyle = RegularExpressionValidationStyle.BorderStyle;
                                valItemReg.ForeColor = RegularExpressionValidationStyle.ForeColor;
                                valItemReg.Font.Bold = RegularExpressionValidationStyle.Font.Bold;
                                valItemReg.Font.Italic = RegularExpressionValidationStyle.Font.Italic;
                                valItemReg.Font.Size = RegularExpressionValidationStyle.Font.Size;
                                valItemReg.Font.Name = RegularExpressionValidationStyle.Font.Name;
                                Controls.Add(valItemReg);
                            }
                        }
                    }

                    // If updates are allowed then create the
                    // link button control.
                    if (_allowUpdate)
                    {
                        _updateButton.Click += new System.EventHandler(OnUpdateButtonClick);
                        _updateButton.ID = "UpdateButton";
                        _updateButton.Text = "Update";
                        _updateButton.Visible = true;
                        _updateButton.CausesValidation = true;
                        Controls.Add(_updateButton);
                    }

                    // If inserts are allowed then create the
                    // link button control.
                    if (_allowInsert)
                    {
                        _insertButton.Click += new System.EventHandler(OnInsertButtonClick);
                        _insertButton.ID = "InsertButton";
                        _insertButton.Text = "Insert";
                        _insertButton.Visible = true;
                        _insertButton.CausesValidation = true;
                        Controls.Add(_insertButton);
                    }

                    // If deletes are allowed then create the
                    // link button control.
                    if (_allowDelete)
                    {
                        _deleteButton.Click += new System.EventHandler(OnDeleteButtonClick);
                        _deleteButton.ID = "DeleteButton";
                        _deleteButton.Text = "Delete";
                        _deleteButton.Visible = true;
                        _deleteButton.CausesValidation = true;
                        Controls.Add(_deleteButton);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// When implemented by a class, instructs the server control to track changes
        /// to its view state.
        /// </summary>
        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (_columns != null) ((IStateManager)_columns).TrackViewState();
        }

        /// <summary>
        /// When implemented by a class, loads the server control's previously saved
        /// view state to the control.
        /// </summary>
        /// <returns>An System.Object that contains the saved view state values for the control.</returns>
        protected override object SaveViewState()
        {
            object[] states = new object[2];
            states[0] = base.SaveViewState();
            states[1] = (_columns == null ? null : ((IStateManager)_columns).SaveViewState());
            
            return null;
        }

        /// <summary>
        /// When implemented by a class, saves the changes to a server control's view
        /// state to an System.Object.
        /// </summary>
        /// <param name="savedState">The System.Object that contains the view state changes.</param>
        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
            {
                base.LoadViewState(null);
                return;
            }

            object[] states = (object[])savedState;
            base.LoadViewState(states[0]);
            EnsureChildControls();

            if (states[1] != null) ((IStateManager)Columns).LoadViewState(states[1]);
        }

        ///// <summary>
        ///// Handle events raised by children by overriding OnBubbleEvent.
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="args"></param>
        ///// <returns></returns>
        //protected override bool OnBubbleEvent(object source, EventArgs args)
        //{
        //    return base.OnBubbleEvent(source, args);
        //}
        #endregion

        #region Data Object Operation Methods
        /// <summary>
        /// Applies the user passed data object type data.
        /// </summary>
        /// <param name="dataObjectData">The data object type containing the data to display.</param>
        public void SelectedData(Object dataObjectData)
        {
            // If the data object type equals the passed data object type
            // else throw an exception.
            if (dataObjectData.GetType() == BuildManager.GetType(_connectionTypeModel.DataObjectTypeName, true, true))
                _dataObjectData = dataObjectData;
            else
                if (SelectError != null)
                    OnSelectError(new ErrorMessageArgs("The data object type: '" +
                                    dataObjectData.GetType().FullName +
                                    "' is not of type: '" + _connectionTypeModel.DataObjectTypeName + "'"));
        }

        /// <summary>
        /// Applies the user passed data object type data.
        /// </summary>
        /// <param name="dataObjectData">The enumerable data object type containing the data to display.</param>
        /// <param name="index">The index in the enumerable data object to select.</param>
        public void SelectedData(Object dataObjectData, int index)
        {
            int i = 0;
            bool found = false;

            // Cast the data object type as an enumerable object,
            // get the enumerator.
            System.Collections.IEnumerable items = (System.Collections.IEnumerable)dataObjectData;
            System.Collections.IEnumerator dataObjects = items.GetEnumerator();

            // Iterate through the collection.
            while (dataObjects.MoveNext())
            {
                // If the current index equals the
                // selected index then return
                // the data object type.
                if (i == index)
                {
                    SelectedData(dataObjects.Current);
                    found = true;
                    break;
                }
                i++;
            }
            dataObjects.Reset();

            // Make sure the correct data object type in the
            // collection has been selected.
            if (!found)
                OnSelectError(new ErrorMessageArgs("Index is out side the bounds of the array."));
        }

        /// <summary>
        /// Update button click, update data object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateButtonClick(object sender, EventArgs e)
        {
            try
            {
                // Assign the data object with post back data.
                Type dataObjectType = AssignDataObjectType();

                // If the user is to update the data, then
                // pass the returned data object to the user.
                if (UpdateRedirect != null)
                {
                    OnUpdateRedirect(_dataObjectData);
                }
                else
                {
                    // Get type that will perform the update.
                    Type listGenericType = typeof(UpdateDataGenericBase<>);

                    // Create the generic type parameters
                    // and create the genric type.
                    Type[] typeArgs = { dataObjectType };
                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);
                    Type dataAccessProviderType = BuildManager.GetType(_connectionTypeModel.DataAccessProvider, true, true);

                    // Create an instance of the data access provider
                    Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                    // Add the genric tyoe contructor parameters
                    // and create the generic type instance.
                    object[] parameters = new object[] { _connectionTypeModel.DatabaseConnection, 
                        _connectionTypeModel.ConnectionType, _connectionTypeModel.ConnectionDataType, dataAccess };
                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                    // Get the current object.
                    Object[] args = new Object[] 
                    { 
                        _dataObjectData 
                    };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("UpdateItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);

                    // The update completed sucessfully.
                    OnUpdateComplete((int)ret);
                }
            }
            catch (Exception ex)
            {
                OnUpdateError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Insert button click, insert data object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInsertButtonClick(object sender, EventArgs e)
        {
            try
            {
                // Assign the data object with post back data.
                Type dataObjectType = AssignDataObjectType();

                // If the user is to insert the data, then
                // pass the returned data object to the user.
                if (InsertRedirect != null)
                {
                    OnInsertRedirect(_dataObjectData);
                }
                else
                {
                    // Get type that will perform the insert.
                    Type listGenericType = typeof(InsertDataGenericBase<>);

                    // Create the generic type parameters
                    // and create the genric type.
                    Type[] typeArgs = { dataObjectType };
                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);
                    Type dataAccessProviderType = BuildManager.GetType(_connectionTypeModel.DataAccessProvider, true, true);

                    // Create an instance of the data access provider
                    Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                    // Add the genric tyoe contructor parameters
                    // and create the generic type instance.
                    object[] parameters = new object[] { _connectionTypeModel.DatabaseConnection, 
                        _connectionTypeModel.ConnectionType, _connectionTypeModel.ConnectionDataType, dataAccess };
                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                    // Get the current object.
                    Object[] args = new Object[] 
                    { 
                        _dataObjectData 
                    };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("InsertItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);

                    // The insert completed sucessfully.
                    OnInsertComplete((int)ret);
                }
            }
            catch (Exception ex)
            {
                OnInsertError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Delete button click, delete data object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            try
            {
                // Assign the data object with post back data.
                Type dataObjectType = AssignDataObjectType();

                // If the user is to delete the data, then
                // pass the returned data object to the user.
                if (DeleteRedirect != null)
                {
                    OnDeleteRedirect(_dataObjectData);
                }
                else
                {
                    // Get type that will perform the insert.
                    Type listGenericType = typeof(DeleteDataGenericBase<>);

                    // Create the generic type parameters
                    // and create the genric type.
                    Type[] typeArgs = { dataObjectType };
                    Type listGenericTypeConstructor = listGenericType.MakeGenericType(typeArgs);
                    Type dataAccessProviderType = BuildManager.GetType(_connectionTypeModel.DataAccessProvider, true, true);

                    // Create an instance of the data access provider
                    Nequeo.Data.DataType.IDataAccess dataAccess = ((Nequeo.Data.DataType.IDataAccess)Activator.CreateInstance(dataAccessProviderType));

                    // Add the genric tyoe contructor parameters
                    // and create the generic type instance.
                    object[] parameters = new object[] { _connectionTypeModel.DatabaseConnection, 
                        _connectionTypeModel.ConnectionType, _connectionTypeModel.ConnectionDataType, dataAccess };
                    object listGeneric = Activator.CreateInstance(listGenericTypeConstructor, parameters);

                    // Get the current object.
                    Object[] args = new Object[] 
                    { 
                        _dataObjectData 
                    };

                    // Add the current data row to the
                    // business object collection.
                    object ret = listGeneric.GetType().InvokeMember("DeleteItem",
                        BindingFlags.DeclaredOnly | BindingFlags.Public |
                        BindingFlags.Instance | BindingFlags.InvokeMethod,
                        null, listGeneric, args);

                    // The delete completed sucessfully.
                    OnDeleteComplete((int)ret);
                }
            }
            catch (Exception ex)
            {
                OnDeleteError(new ErrorMessageArgs(ex.Message));
            }
        }

        /// <summary>
        /// Assign the data object with post back data.
        /// </summary>
        /// <returns>The type instance of the data object.</returns>
        private Type AssignDataObjectType()
        {
            // Create a new type instance of the data object type
            // and get the collection of data object information.
            Type dataObjectType = BuildManager.GetType(_connectionTypeModel.DataObjectTypeName, true, true);
            List<DataObjectInfo> properties = GetDataObjectInfo(dataObjectType);

            // If the data object type is null then
            // create a new instance of the data type.
            if (_dataObjectData == null)
                _dataObjectData = Activator.CreateInstance(dataObjectType);

            // For each child control.
            foreach (System.Web.UI.Control control in Controls)
            {
                // Find the property in the type and the control id.
                IEnumerable<DataObjectInfo> items = properties.Where(u => u.Member.Name == control.ID);
                if (items != null)
                {
                    // If the property has been found.
                    if (items.Count() > 0)
                    {
                        // Find the first property found, get the current control
                        // default value. Set the property value to the default
                        // web control value.
                        DataObjectInfo data = items.First();
                        object value = WebControlType.WebControlConvertType(control, data.Member.PropertyType);
                        data.Member.SetValue(_dataObjectData, value, null);
                    }
                }
            }
            return dataObjectType;
        }
        #endregion

        #region Control Contents Render Methods
        /// <summary>
        /// Render the control contents to the current page.
        /// </summary>
        /// <param name="output">The html text writer.</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            try
            {
                // Adds HTML attributes and styles that need to be rendered to the specified
                // System.Web.UI.HtmlTextWriterTag. This method is used primarily by control
                // developers.
                AddAttributesToRender(output);

                // If the DataObjectForm control contains child controls.
                if (Controls.Count > 0)
                {
                    // Is end tag
                    bool isEndControl = true;

                    // Add the table attributes.
                    output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    output.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "4");
                    output.RenderBeginTag(HtmlTextWriterTag.Table);

                    // For each child control created
                    foreach (System.Web.UI.Control control in Controls)
                    {
                        // If the control is the required validator
                        if (control.ID.StartsWith("validator_Req_"))
                        {
                            // Add the tag elements
                            RequiredFieldValidationStyle.AddAttributesToRender(output);
                            output.RenderBeginTag(HtmlTextWriterTag.Td);
                            output.Write("*");

                            // Only add the control if it is required.
                            if (_includeRequiredValidation)
                                control.RenderControl(output);

                            output.RenderEndTag();

                            isEndControl = true;
                        }
                        // If the control is the regular expression validator.
                        else if (control.ID.StartsWith("validator_Reg_"))
                        {
                            // Add the tag elements
                            RegularExpressionValidationStyle.AddAttributesToRender(output);
                            output.RenderBeginTag(HtmlTextWriterTag.Td);
                            control.RenderControl(output);
                            output.RenderEndTag();

                            isEndControl = true;
                        }
                        // Render the calendar control.
                        else if (control.ID.EndsWith("_Calendar"))
                        {
                            // Add the calendar control
                            output.RenderBeginTag(HtmlTextWriterTag.Td);
                            control.RenderControl(output);
                            output.RenderEndTag();

                            isEndControl = true;
                        }
                        else
                        {
                            // If the control is not a button
                            // then the control is a field control.
                            if (!control.ID.Contains("Button"))
                            {
                                // If the end (TR) tag should be applied.
                                if (!isEndControl)
                                    output.RenderEndTag();

                                isEndControl = false;

                                // Add the tag elements
                                output.RenderBeginTag(HtmlTextWriterTag.Tr);
                                CaptionStyle.AddAttributesToRender(output);
                                output.RenderBeginTag(HtmlTextWriterTag.Td);
                                output.Write(((WebControl)control).ToolTip);
                                output.RenderEndTag();
                                output.RenderBeginTag(HtmlTextWriterTag.Td);
                                control.RenderControl(output);
                                output.RenderEndTag();
                            }
                        }
                    }

                    // End the field table tag
                    output.RenderEndTag();

                    // Add the button table attributes.
                    output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    output.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "4");
                    output.RenderBeginTag(HtmlTextWriterTag.Table);
                    output.RenderBeginTag(HtmlTextWriterTag.Tr);

                    // For each child control created
                    foreach (System.Web.UI.Control control in Controls)
                    {
                        // If the control is a button
                        // then the control is a field control.
                        if (control.ID.Contains("Button"))
                        {
                            // If the buuton control is the update button.
                            if (control.ID == "UpdateButton")
                            {
                                // If update is allowed
                                if (_allowUpdate)
                                {
                                    // Add the tag elements
                                    output.RenderBeginTag(HtmlTextWriterTag.Td);
                                    control.RenderControl(output);
                                    output.RenderEndTag();
                                }
                            }

                            // If the buuton control is the delete button.
                            if (control.ID == "DeleteButton")
                            {
                                // If delete is allowed
                                if (_allowDelete)
                                {
                                    // Add the tag elements
                                    output.RenderBeginTag(HtmlTextWriterTag.Td);
                                    control.RenderControl(output);
                                    output.RenderEndTag();
                                }
                            }

                            // If the buuton control is the insert button.
                            if (control.ID == "InsertButton")
                            {
                                // If insert is allowed
                                if (_allowInsert)
                                {
                                    // Add the tag elements
                                    output.RenderBeginTag(HtmlTextWriterTag.Td);
                                    control.RenderControl(output);
                                    output.RenderEndTag();
                                }
                            }
                        }
                    }

                    // End the field table tag.
                    output.RenderEndTag();
                    output.RenderEndTag();
                }
                else
                    output.Write("Control not rendered at design time.");
            }
            catch (Exception ex)
            {
                OnError(new ErrorMessageArgs(ex.Message));
            }
        }
        #endregion

        #region Interface Implementation Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="postDataKey"></param>
        /// <param name="postCollection"></param>
        /// <returns></returns>
        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RaisePostDataChangedEvent()
        {
        }

        /// <summary>
        /// Raised when a client side page event occurs.
        /// </summary>
        /// <param name="eventArgument">Post back event argument.</param>
        public void RaisePostBackEvent(string eventArgument)
        {
        }
        #endregion

        #region Private General Methods
        /// <summary>
        /// Get all table column properties for the data entity type.
        /// </summary>
        /// <param name="dataObjectType">The current data object type</param>
        /// <returns>The collection of data object properties</returns>
        private List<DataObjectInfo> GetDataObjectInfo(Type dataObjectType)
        {
            // Create a new property collection.
            List<DataObjectInfo> column = new List<DataObjectInfo>();
            List<DataObjectInfo> fields = null;

            // For each property member in the current type.
            foreach (PropertyInfo member in dataObjectType.GetProperties())
            {
                // For each attribute on each property
                // in the type.
                foreach (object attribute in member.GetCustomAttributes(true))
                {
                    // If the attribute is the
                    // linq column attribute.
                    if (attribute is Nequeo.Data.Custom.DataColumnAttribute)
                    {
                        Nequeo.Data.Custom.DataColumnAttribute item =
                            (Nequeo.Data.Custom.DataColumnAttribute)attribute;

                        DataObjectInfo info = new DataObjectInfo();
                        info.Member = member;
                        info.IsNullable = item.IsNullable;
                        info.IsPrimaryKey = item.IsPrimaryKey;
                        info.IsAutoGenerated = item.IsAutoGenerated;
                        info.Caption = member.Name;
                        info.IsDateTime = (member.PropertyType == typeof(System.DateTime) ? true : false);
                        column.Add(info);
                    }
                }
            }

            // If bound columns have been selected.
            if(Columns.Count > 0)
            {
                fields = new List<DataObjectInfo>();
                for(int i = 0; i < Columns.Count; i++)
                {
                    // Get the current bound field.
                    System.Web.UI.WebControls.BoundField field = (System.Web.UI.WebControls.BoundField)Columns[i];
                    DataObjectInfo info = null;
                    try
                    {
                        // Find the first bound field item
                        // in the data object type collection.
                        info = column.First(u => u.Member.Name.ToLower() == field.DataField.ToLower());
                    }
                    catch { }

                    // If the item has been found add it
                    // to the fields collection.
                    if (info != null)
                    {
                        info.Caption = field.HeaderText;
                        fields.Add(info);
                    }
                }

                // Return the collection of
                // columns properties.
                return fields;
            }
            else
                // Return the collection of
                // columns properties.
                return column;
        }
        #endregion

        #region Data Object Info Type
        /// <summary>
        /// The data object information type, stores information for each
        /// data object property for the current data object type.
        /// </summary>
        internal class DataObjectInfo
        {
            private PropertyInfo _member;
            private Boolean _isNullable;
            private Boolean _isPrimaryKey;
            private Boolean _isAutoGenerated;
            private Boolean _isDateTime;
            private String _caption;

            /// <summary>
            /// Gets sets, the current property information.
            /// </summary>
            internal PropertyInfo Member
            {
                get { return _member; }
                set { _member = value; }
            }

            /// <summary>
            /// Gets sets, the caption for the member.
            /// </summary>
            internal String Caption
            {
                get { return _caption; }
                set { _caption = value; }
            }

            /// <summary>
            /// Gets sets, is the data object property nullable.
            /// </summary>
            internal Boolean IsNullable
            {
                get { return _isNullable; }
                set { _isNullable = value; }
            }

            /// <summary>
            /// Gets sets, is the data object property a primary key.
            /// </summary>
            internal Boolean IsPrimaryKey
            {
                get { return _isPrimaryKey; }
                set { _isPrimaryKey = value; }
            }

            /// <summary>
            /// Gets sets, is the data object property a auto generated.
            /// </summary>
            internal Boolean IsAutoGenerated
            {
                get { return _isAutoGenerated; }
                set { _isAutoGenerated = value; }
            }

            /// <summary>
            /// Gets sets, is the data object property a datetime type.
            /// </summary>
            internal Boolean IsDateTime
            {
                get { return _isDateTime; }
                set { _isDateTime = value; }
            }
        }
        #endregion
    }
}
