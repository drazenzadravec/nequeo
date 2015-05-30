/// <summary>
/// Register the client control namespace.
/// </summary>
Type.registerNamespace("Nequeo.Web.UI.ScriptControl");

/// <summary>
/// Initialise the class prototype
/// </summary>
Nequeo.Web.UI.ScriptControl.ItemListClientControl = function(element) {
    /// <summary>
    /// Initialize base class.
    /// </summary>
    Nequeo.Web.UI.ScriptControl.ItemListClientControl.initializeBase(this, [element]);

    // Inner variables
    this._itemtopDiv = null;
    this._itemlistDiv = null;
    
    // WebRequest object returned from WebServiceProxy.invoke
    this._webRequest = null; 

    this._ItemTitle = null;
    this._ItemTitleCssClass = null;
    this._ItemListCssClass = null;

    this._TargetControlID = null;
    this._ListTargetControlID = null;
    this._TitleTargetControlID = null;
    
    this._ServicePath = null;
    this._ServiceMethod = null;

    this._clickItemHandler = null;
    this._propertyChanged = null;
}

/// <summary>
/// Create the class prototype for the item client control.
/// </summary>
Nequeo.Web.UI.ScriptControl.ItemListClientControl.prototype =
{
    /*** Properties *****************************************/

    /// <summary>
    /// Get the TargetControlID.
    /// </summary>
    get_TargetControlID: function() {
        return this._TargetControlID;
    },

    /// <summary>
    /// Set the TargetControlID.
    /// </summary>
    set_TargetControlID: function(value) {
        // If the service path has changed
        if (this._TargetControlID != value) {
            // Set the new target control id and
            // raise as property changed event
            this._TargetControlID = value;
            this.raisePropertyChanged('TargetControlID');
        }
    },

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
    /// Get the ItemTitle.
    /// </summary>
    get_ItemTitle: function() {
        return this._ItemTitle;
    },

    /// <summary>
    /// Set the ItemTitle.
    /// </summary>
    set_ItemTitle: function(value) {
        this._ItemTitle = value;
    },

    /// <summary>
    /// Get the ItemTitleCssClass.
    /// </summary>
    get_ItemTitleCssClass: function() {
        return this._ItemTitleCssClass;
    },

    /// <summary>
    /// Set the ItemTitleCssClass.
    /// </summary>
    set_ItemTitleCssClass: function(value) {
        this._ItemTitleCssClass = value;
    },

    /// <summary>
    /// Get the ItemListCssClass.
    /// </summary>
    get_ItemListCssClass: function() {
        return this._ItemListCssClass;
    },

    /// <summary>
    /// Set the ItemListCssClass.
    /// </summary>
    set_ItemListCssClass: function(value) {
        this._ItemListCssClass = value;
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

        if (result != null) {
            // For each item in the array, create a
            // new line in the temp html.
            while (typeof (result[index++]) != 'undefined')
                tempHTML += result[index - 1] + "<br/>";
        }

        // Assign the item list DIV tag inner html
        // with the item collection returned.
        this._itemlistDiv.innerHTML = tempHTML;
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
        // level css class and the item title in the inner html.
        this._itemtopDiv = elementTitle;
        this._itemtopDiv.className = this._ItemTitleCssClass;
        this._itemtopDiv.innerHTML = this.get_ItemTitle();

        // Create a new click event handler when
        // user clicks on the top level DIV.
        this._clickItemHandler = Function.createDelegate(this, this._onClick);
        $addHandler(this._itemtopDiv, "click", this._clickItemHandler);

        // Create a new list level DIV and assign
        // the css class to the DIV tag
        this._itemlistDiv = elementList
        this._itemlistDiv.className = this._ItemListCssClass;

        // Create a new property changed event handler
        // when service data changes.
        this._propertyChanged = Function.createDelegate(this, this._onPropertyChanged);
        this.add_propertyChanged(this._propertyChanged);

        // Call the base initialiser.
        Nequeo.Web.UI.ScriptControl.ItemListClientControl.callBaseMethod(this, 'initialize');
    },

    /// <summary>
    /// Dispose of the class.
    /// </summary>
    dispose: function() {
        // Get the current base element object (target element this control extendes)
        var element = this.get_element();

        // Remove the property changed event handler
        this.remove_propertyChanged(this._propertyChanged);

        // Remove the handler.
        if (element) {
            $removeHandler(this._itemtopDiv, "click", this._clickItemHandler);
        }

        // Clean up resources.
        this._clickItemHandler = null;
        this._propertyChanged = null;

        // Call the base disposer.
        Nequeo.Web.UI.ScriptControl.ItemListClientControl.callBaseMethod(this, "dispose");
    },

    /// <summary>
    /// Method to call the web service asynchronously to get the list of data.
    /// </summary>
    _invokeWebService: function() {

        // Get the value in the target control.
        var text = this._currentCompletionWord();

        // Create the service parameters and optionally add the context parameter
        // (thereby determining which method signature we're expecting...)
        var params = { textData: text };

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
                                                        params,
                                                        Function.createDelegate(this, this._onMethodComplete),
                                                        Function.createDelegate(this, this._onMethodError));
    },

    /// <summary>
    /// Method to return the target control value.
    /// </summary>
    _currentCompletionWord: function() {
        // Get the current base element object
        var element = this.get_element();

        // Get the target control value.
        var elementValue = element.value;
        var word = elementValue;

        // Return the value in the target control text.
        return word;
    },

    /*** Events *****************************************/

    /// <summary>
    /// When the top level DIV tag is clicked
    /// </summary>
    _onClick: function(evt) {
        // Start the debug tracer.
        Sys.Debug.trace(" from inside _onClick");

        // Get the list DIV tag reference.
        var itemdom = this._itemlistDiv;

        // If the list item is not displayed.
        if (itemdom.style.display == "none" || itemdom.style.display == "") {
            // Invoke the web service method to
            // return the list data and make
            // the lis DIV tag visible to
            // display the list data returned.
            this._invokeWebService();
            itemdom.style.display = "list-item";
        }
        else {
            // Set the display style to none (hidden).
            itemdom.style.display = "none";
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
Nequeo.Web.UI.ScriptControl.ItemListClientControl.registerClass("Nequeo.Web.UI.ScriptControl.ItemListClientControl", Sys.UI.Control);

/// <summary>
/// Added to satisfy new notifyScriptLoaded() requirement
/// </summary>
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();