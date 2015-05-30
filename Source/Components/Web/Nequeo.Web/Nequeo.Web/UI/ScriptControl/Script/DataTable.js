/// <summary>
/// Register the client control namespace.
/// </summary>
Type.registerNamespace("Nequeo.Web.UI.ScriptControl");

/// <summary>
/// Initialise the class prototype
/// </summary>
Nequeo.Web.UI.ScriptControl.DataTableServiceControl = function(element) {

    /// <summary>
    /// Initialize base class.
    /// </summary>
    Nequeo.Web.UI.ScriptControl.DataTableServiceControl.initializeBase(this, [element]);

    // Inner variables
    this._TargetControlID = null;
    this._ConnectionStringExtensionName = null;
    this._UrlAction = null;
    this._CssClass = "display";
    
    this._elementTragetDiv = null;
}

/// <summary>
/// Create the class prototype for the data table service client control.
/// </summary>
Nequeo.Web.UI.ScriptControl.DataTableServiceControl.prototype = {
    /*** Properties *****************************************/

    /// <summary>
    /// Get the MenuTitle.
    /// </summary>
    get_targetControlID: function() {
        return this._TargetControlID;
    },

    /// <summary>
    /// Set the TargetControlID.
    /// </summary>
    set_targetControlID: function(value) {
        this._TargetControlID = value;
    },

    /// <summary>
    /// Get the ConnectionStringExtensionName.
    /// </summary>
    get_connectionStringExtensionName: function() {
        return this._ConnectionStringExtensionName;
    },

    /// <summary>
    /// Set the ConnectionStringExtensionName.
    /// </summary>
    set_connectionStringExtensionName: function(value) {
        this._ConnectionStringExtensionName = value;
    },

    /// <summary>
    /// Get the UrlAction.
    /// </summary>
    get_urlAction: function() {
        return this._UrlAction;
    },

    /// <summary>
    /// Set the UrlAction.
    /// </summary>
    set_urlAction: function(value) {
        this._UrlAction = value;
    },

    /// <summary>
    /// Get the CssClass.
    /// </summary>
    get_cssClass: function() {
        return this._CssClass;
    },

    /// <summary>
    /// Set the CssClass.
    /// </summary>
    set_cssClass: function(value) {
        this._CssClass = value;
    },

    /*** Methods *****************************************/

    /// <summary>
    /// Initialise the class.
    /// </summary>
    initialize: function() {
        // Start the debug tracer
        Sys.Debug.trace(" from inside DataTableInstance");

        // Call the init function.
        if (this._TargetControlID != null) {
            // get the current target control
            var elementTarget = $get(this._TargetControlID);

            // assign the css class.
            this._elementTragetDiv = elementTarget;
            this._elementTragetDiv.className = this._CssClass;

            // Initalise the data table.
            this._dataTableInit();
        }

        // Call the base initialiser.
        Nequeo.Web.UI.ScriptControl.DataTableServiceControl.callBaseMethod(this, 'initialize');
    },

    /// <summary>
    /// Dispose of the class.
    /// </summary>
    dispose: function() {
        // Get the current base element object
        var element = this.get_element();

        // Call the base disposer.
        Nequeo.Web.UI.ScriptControl.DataTableServiceControl.callBaseMethod(this, "dispose");
    },

    /// <summary>
    /// Initialise the data table object.
    /// </summary>
    _dataTableInit: function() {
        // Varables.
        var button = null;
        var urlAction = null;
        var extensionName = null;

        // If a target control id has been specified.
        if (this.get_targetControlID() != null) {
            var targetValue = "#" + this.get_targetControlID();
            button = $(targetValue);
        } else {
            alert("Target control has not been set.");
            return;
        }

        // If a extension name action has been specified.
        if (this.get_connectionStringExtensionName() != null) {
            extensionName = this.get_connectionStringExtensionName();
        } else {
            alert("Target connection string extension name has not been set.");
            return;
        }

        // If a URL action has been specified.
        if (this.get_urlAction() != null) {
            urlAction = this.get_urlAction();
        } else {
            alert("Target URL action has not been set.");
            return;
        }

        // Start the datatable instance
        this._dataTable(button, extensionName, urlAction);
    },

    /// <summary>
    /// Start the data table listener.
    /// </summary>
    /// <param name="button">The datatable trigger reference.</param>
    /// <param name="extensionName">The connection string extension name.</param>
    /// <param name="urlAction">The ajax action url.</param>
    _dataTable: function(button, extensionName, urlAction) {

        $(document).ready(function() {
            $(button).dataTable({
                "bProcessing": true,
                "bServerSide": true,
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "sAjaxSource": urlAction,
                "fnServerData": function(sSource, aoData, fnCallback) {
                    aoData.push({ "name": "extensionName", "value": extensionName });

                    $.ajax({
                        "dataType": 'json',
                        "contentType": 'application/json',
                        "type": "GET",
                        "url": sSource,
                        "data": aoData,
                        "success": function(msg) {
                            var json = $.evalJSON(msg.d);
                            fnCallback(json);
                        }
                    });
                }
            });
        });
    }
}

/// <summary>
/// Register the client control class created above.
/// </summary>
Nequeo.Web.UI.ScriptControl.DataTableServiceControl.registerClass("Nequeo.Web.UI.ScriptControl.DataTableServiceControl", Sys.UI.Control);

/// <summary>
/// Added to satisfy new notifyScriptLoaded() requirement
/// </summary>
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();