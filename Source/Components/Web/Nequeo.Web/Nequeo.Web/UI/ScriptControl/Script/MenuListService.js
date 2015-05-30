/// <summary>
/// Register the client control namespace.
/// </summary>
Type.registerNamespace("Nequeo.Web.UI.ScriptControl");

/// <summary>
/// Initialise the class prototype
/// </summary>
Nequeo.Web.UI.ScriptControl.MenuListClientControl = function(element) 
{
    /// <summary>
    /// Initialize base class.
    /// </summary>
    Nequeo.Web.UI.ScriptControl.MenuListClientControl.initializeBase(this, [element]);

    // Inner variables
    this._menutopDiv = null;
    this._menulistDiv = null;

    // WebRequest object returned from WebServiceProxy.invoke
    this._webRequest = null; 

    this._MenuTitle = null;
    this._MenuTitleCssClass = null;
    this._MenuListCssClass = null;
    
    this._ListTargetControlID = null;
    this._TitleTargetControlID = null;

    this._ServicePath = null;
    this._ServiceMethod = null;

    this._clickMenuHandler = null;
    this._propertyChanged = null;
}

/// <summary>
/// Create the class prototype for the menu client control.
/// </summary>
Nequeo.Web.UI.ScriptControl.MenuListClientControl.prototype =
{
    /*** Properties *****************************************/

    /// <summary>
    /// Get the ListTargetControlID.
    /// </summary>
    get_ListTargetControlID: function() {
        return this._ListTargetControlID;
    },

    /// <summary>
    /// Set the ListTargetControlID.
    /// </summary>
    set_ListTargetControlID: function(value) {
        // If the service path has changed
        if (this._ListTargetControlID != value) {
            // Set the new target control id and
            // raise as property changed event
            this._ListTargetControlID = value;
            this.raisePropertyChanged('ListTargetControlID');
        }
    },

    /// <summary>
    /// Get the TitleTargetControlID.
    /// </summary>
    get_TitleTargetControlID: function() {
        return this._TitleTargetControlID;
    },

    /// <summary>
    /// Set the TitleTargetControlID.
    /// </summary>
    set_TitleTargetControlID: function(value) {
        // If the service path has changed
        if (this._TitleTargetControlID != value) {
            // Set the new target control id and
            // raise as property changed event
            this._TitleTargetControlID = value;
            this.raisePropertyChanged('TitleTargetControlID');
        }
    },
    
    /// <summary>
    /// Get the MenuTitle.
    /// </summary>
    get_MenuTitle: function() {
        return this._MenuTitle;
    },

    /// <summary>
    /// Set the MenuTitle.
    /// </summary>
    set_MenuTitle: function(value) {
        this._MenuTitle = value;
    },

    /// <summary>
    /// Get the MenuTitleCssClass.
    /// </summary>
    get_MenuTitleCssClass: function() {
        return this._MenuTitleCssClass;
    },

    /// <summary>
    /// Set the MenuTitleCssClass.
    /// </summary>
    set_MenuTitleCssClass: function(value) {
        this._MenuTitleCssClass = value;
    },

    /// <summary>
    /// Get the MenuListCssClass.
    /// </summary>
    get_MenuListCssClass: function() {
        return this._MenuListCssClass;
    },

    /// <summary>
    /// Set the MenuListCssClass.
    /// </summary>
    set_MenuListCssClass: function(value) {
        this._MenuListCssClass = value;
    },

    /// <summary>
    /// Get the ServicePath.
    /// </summary>
    get_ServicePath: function() {
        return this._ServicePath;
    },

    /// <summary>
    /// Set the ServicePath.
    /// </summary>
    set_ServicePath: function(value) {
        // If the service path has changed
        if (this._ServicePath != value) {
            // Set the new service path and
            // raise as property changed event
            this._ServicePath = value;
            this.raisePropertyChanged('ServicePath');
        }
    },

    /// <summary>
    /// Get the ServiceMethod.
    /// </summary>
    get_ServiceMethod: function() {
        return this._ServiceMethod;
    },

    /// <summary>
    /// Set the ServiceMethod.
    /// </summary>
    set_ServiceMethod: function(value) {
        // If the service method has changed
        if (this._ServiceMethod != value) {
            // Set the new service method and
            // raise as property changed event
            this._ServiceMethod = value;
            this.raisePropertyChanged('ServiceMethod');
        }
    },

    /// <summary>
    /// Set the data as its return from the service.
    /// </summary>
    set_data: function(result) {
        var index = 0;
        var tempHTML = "";

        if (result != null) 
        {
            // For each item in the array, create a
            // new line in the temp html.
            while (typeof (result[index++]) != 'undefined')
                tempHTML += result[index - 1] + "<br/>";
        }

        // Assign the menu list DIV tag inner html
        // with the item collection returned.
        this._menulistDiv.innerHTML = tempHTML;
    },

    /*** Methods *****************************************/

    /// <summary>
    /// Initialise the class.
    /// </summary>
    initialize: function() {
        // Start the debug tracer
        Sys.Debug.trace(" from inside set_ServiceMethod");
        
        // Get the current base element object
        var elementList = $get(this._ListTargetControlID);
        var elementTitle = $get(this._TitleTargetControlID);

        // Create a new top level DIV tag and assign the top
        // level css class and the menu title in the inner html.
        this._menutopDiv = elementTitle;
        this._menutopDiv.className = this._MenuTitleCssClass;
        this._menutopDiv.innerHTML = this.get_MenuTitle();

        // Create a new click event handler when
        // user clicks on the top level DIV.
        this._clickMenuHandler = Function.createDelegate(this, this._onClick);
        $addHandler(this._menutopDiv, "click", this._clickMenuHandler);

        // Create a new list level DIV and assign
        // the css class to the DIV tag
        this._menulistDiv = elementList;
        this._menulistDiv.className = this._MenuListCssClass;

        // Create a new property changed event handler
        // when service data changes.
        this._propertyChanged = Function.createDelegate(this, this._onPropertyChanged);
        this.add_propertyChanged(this._propertyChanged);

        // Call the base initialiser.
        Nequeo.Web.UI.ScriptControl.MenuListClientControl.callBaseMethod(this, 'initialize');
    },

    /// <summary>
    /// Dispose of the class.
    /// </summary>
    dispose: function() {
        // Get the current base element object
        var element = this.get_element();

        // Remove the property changed event handler
        this.remove_propertyChanged(this._propertyChanged);

        // Remove the handler.
        if (element) {
            $removeHandler(this._menutopDiv, "click", this._clickMenuHandler);
        }

        // Clean up resources.
        this._clickMenuHandler = null;
        this._propertyChanged = null;

        // Call the base disposer.
        Nequeo.Web.UI.ScriptControl.MenuListClientControl.callBaseMethod(this, "dispose");
    },

    /// <summary>
    /// Method to call the web service asynchronously to get the list of data.
    /// </summary>
    _invokeWebService: function() {

        if (this._webRequest) {
            // abort the previous web service call if we 
            // are issuing a new one and the previous one is 
            // active.
            this._webRequest.get_executor().abort();
            this._webRequest = null;
        }
        
        // Call the DOM web service proxy method
        // to send any data to the web service.
        this._webRequest = Sys.Net.WebServiceProxy.invoke(this._ServicePath, this._ServiceMethod, false,
                                                    {},
                                                    Function.createDelegate(this, this._onMethodComplete),
                                                    Function.createDelegate(this, this._onMethodError));
    },

    /*** Events *****************************************/

    /// <summary>
    /// When the top level DIV tag is clicked
    /// </summary>
    _onClick: function(evt) {
        // Start the debug tracer.
        Sys.Debug.trace(" from inside _onClick");

        // Get the list DIV tag reference.
        var menudom = this._menulistDiv;

        // If the list menu is not displayed.
        if (menudom.style.display == "none" || menudom.style.display == "") {
            // Invoke the web service method to
            // return the list data and make
            // the lis DIV tag visible to
            // display the list data returned.
            this._invokeWebService();
            menudom.style.display = "list-item";
        }
        else {
            // Set the display style to none (hidden).
            menudom.style.display = "none";
        }

    },

    /// <summary>
    /// When the web serice returns the data this method is invoked.
    /// </summary>
    _onMethodComplete: function(result, userContext, methodName) {
        // Bind returned data
        this.set_data(result);
    },

    /// <summary>
    /// When an error occurs in calling the web service.
    /// </summary>
    _onMethodError: function(webServiceError, userContext, methodName) {
        // Call failed
        if (webServiceError.get_timedOut()) {
            // Display a dialog.
            alert("Web Service call timed out.");
        }
        else {
            // Display a dialog.
            alert("Error calling Web Service: " + webServiceError.get_statusCode() + " " + webServiceError.get_message());
        }
    },

    /// <summary>
    /// When the property changes event.
    /// </summary>
    _onPropertyChanged: function(sender, args) {
        // Get the current property name that has been changed
        var propname = args.get_propertyName();
        if (propname == "ServicePath" || propname === "ServiceMethod") {
            // Call the web service invoker method.
            this._invokeWebService();
        }
    }
}

/// <summary>
/// Register the client control class created above.
/// </summary>
Nequeo.Web.UI.ScriptControl.MenuListClientControl.registerClass("Nequeo.Web.UI.ScriptControl.MenuListClientControl", Sys.UI.Control);

/// <summary>
/// Added to satisfy new notifyScriptLoaded() requirement
/// </summary>
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();