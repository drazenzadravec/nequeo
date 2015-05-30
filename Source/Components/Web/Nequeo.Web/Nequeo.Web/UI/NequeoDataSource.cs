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
using System.Data;
using System.Security.Permissions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;

using Nequeo.ComponentModel;
using Nequeo.Handler;
using Nequeo.Web.UI.Design;

namespace Nequeo.Web.UI
{
    /// <summary>
    /// The common nequeo web data source provider.
    /// </summary>
    [ToolboxBitmap(typeof(NequeoDataSource))]
    [Designer(typeof(NequeoDataSourceDesigner))]
    [ToolboxData("<{0}:NequeoDataSource runat=\"server\"></{0}:NequeoDataSource>")]
    [DefaultProperty("Text")]
    [Description("Nequeo Web Data Source Binder")]
    [DisplayName("Data Source Binder")]
    public class NequeoDataSource : ObjectDataSource
    {
        #region Nequeo Data Source
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NequeoDataSource() : base() { }

        private ObjectDataSourceView _view = null;
        private string _orderByClause = string.Empty;
        private string _whereClause = string.Empty;
        private string _defaultViewName = "NequeoBinding";
        private ConnectionTypeModel _connectionTypeModel = null;
       
        /// <summary>
        /// Gets sets, the Sql order by clause.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Data")]
        [Description("The order by sql cluase. This is manditory.")]
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        /// <summary>
        /// Gets sets, the Sql where clause
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Data")]
        [Description("The where sql clause.")]
        public string WhereClause
        {
            get { return _whereClause; }
            set { _whereClause = value; }
        }

        /// <summary>
        /// Gets sets, connection informations and data obejct type.
        /// </summary>
        [Category("Data")]
        [DefaultValue(null)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Editor(typeof(ConnectionTypeDropDownEditor), typeof(UITypeEditor))]
        [Description("Connection informations and data obejct type.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public ConnectionTypeModel ConnectionTypeModel
        {
            get { return _connectionTypeModel; }
            set { _connectionTypeModel = value; }
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
        /// Trigger when a database operation error occures.
        /// </summary>
        public event Nequeo.Threading.EventHandler<ErrorMessageArgs> ExecutionError;

        /// <summary>
        /// Triggered when an execution error occures.
        /// </summary>
        /// <param name="e">The error message arguments.</param>
        protected virtual void OnError(ErrorMessageArgs e)
        {
            if (ExecutionError != null)
                ExecutionError(this, e);
        }

        /// <summary>
        /// Triggered when an execution error occures.
        /// </summary>
        /// <param name="e">The error message arguments.</param>
        internal void OnErrorEx(ErrorMessageArgs e)
        {
            OnError(e);
        }

        /// <summary>
        /// Gets a view by name, the runtime data source view.
        /// </summary>
        /// <param name="viewName">The run time view</param>
        /// <returns>The data source view.</returns>
        protected override DataSourceView GetView(string viewName)
        {
            // This data source only allows one view
            if (viewName != _defaultViewName)
                return null;
            else if (_view == null)
            {
                _view = new NequeoDataSourceView(this,
                    _defaultViewName, HttpContext.Current);
            }

            return _view;
        }

        /// <summary>
        /// Gets a list of view names for this class
        /// </summary>
        /// <returns>The collection of runtime views.</returns>
        protected override ICollection GetViewNames()
        {
            ArrayList ar = new ArrayList(1);
            ar.Add(_defaultViewName);
            return ar as ICollection;
        }
        #endregion
    }
}