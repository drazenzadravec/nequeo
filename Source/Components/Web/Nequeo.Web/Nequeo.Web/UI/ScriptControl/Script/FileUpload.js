/// <summary>
/// Register the client control namespace.
/// </summary>
Type.registerNamespace("Nequeo.Web.UI.ScriptControl");

/// <summary>
/// Initialise the class prototype
/// </summary>
Nequeo.Web.UI.ScriptControl.FileUploadControl = function(element) {

    /// <summary>
    /// Initialize base class.
    /// </summary>
    Nequeo.Web.UI.ScriptControl.FileUploadControl.initializeBase(this, [element]);
    
    // Inner variables
    this._TargetControlID = null;
    this._ResponseControlID = null;
    this._ErrorControlID = null;
    this._FileExtensionFilter = null;
    this._UrlAction = null;
    this._CssClass = "uploadButton";

    this._elementTragetDiv = null;
}

/// <summary>
/// Create the class prototype for the file upload client control.
/// </summary>
Nequeo.Web.UI.ScriptControl.FileUploadControl.prototype = {
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
    /// Get the ResponseControlID.
    /// </summary>
    get_responseControlID: function() {
        return this._ResponseControlID;
    },

    /// <summary>
    /// Set the ResponseControlID.
    /// </summary>
    set_responseControlID: function(value) {
        this._ResponseControlID = value;
    },

    /// <summary>
    /// Get the ErrorControlID.
    /// </summary>
    get_errorControlID: function() {
        return this._ErrorControlID;
    },

    /// <summary>
    /// Set the ErrorControlID.
    /// </summary>
    set_errorControlID: function(value) {
        this._ErrorControlID = value;
    },

    /// <summary>
    /// Get the FileExtensionFilter.
    /// </summary>
    get_fileExtensionFilter: function() {
        return this._FileExtensionFilter;
    },

    /// <summary>
    /// Set the FileExtensionFilter.
    /// </summary>
    set_fileExtensionFilter: function(value) {
        this._FileExtensionFilter = value;
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
        Sys.Debug.trace(" from inside FileUploadInstance");

        // Call the init function.
        if (this._TargetControlID != null) {
            // get the current target control
            var elementTarget = $get(this._TargetControlID);

            // assign the css class.
            this._elementTragetDiv = elementTarget;
            this._elementTragetDiv.className = this._CssClass;

            // Initalise the upload.
            this._fileUploadInit();
        }

        // Call the base initialiser.
        Nequeo.Web.UI.ScriptControl.FileUploadControl.callBaseMethod(this, 'initialize');
    },

    /// <summary>
    /// Dispose of the class.
    /// </summary>
    dispose: function() {
        // Get the current base element object
        var element = this.get_element();

        // Call the base disposer.
        Nequeo.Web.UI.ScriptControl.FileUploadControl.callBaseMethod(this, "dispose");
    },

    /// <summary>
    /// Uploads the file to the URL.
    /// </summary>
    _fileUploadInit: function() {
        // Varables.
        var filter = null;
        var button = null;
        var urlAction = null;
        var fileExtensionFilter = null;
        var errorControl = null;
        var responseControl = null;
        var divInnerText = this._elementTragetDiv.innerHTML;

        // If a target control id has been specified.
        if (this.get_targetControlID() != null) {
            var targetValue = "#" + this.get_targetControlID();
            button = $(targetValue);
        } else {
            alert("Target control has not been set.");
            return;
        }

        // If a URL action has been specified.
        if (this.get_urlAction() != null) {
            urlAction = this.get_urlAction();
        } else {
            alert("Target URL action has not been set.");
            return;
        }

        // If a filter file has been specified.
        if (this.get_fileExtensionFilter() != null) {
            filter = eval('/^(' + this.get_fileExtensionFilter() + ')$/');
            fileExtensionFilter = this.get_fileExtensionFilter();
        }

        // If the error control id has been set.
        if (this.get_errorControlID() != null) {
            errorControl = $get(this.get_errorControlID());
        }

        // If the response control id has been set.
        if (this.get_responseControlID() != null) {
            responseControl = $get(this.get_responseControlID());
        }

        // Start the upload instance
        this._fileUpload(button, urlAction, responseControl, errorControl, fileExtensionFilter, filter, divInnerText);
    },

    /// <summary>
    /// Uploads the file to the URL.
    /// </summary>
    /// <param name="button">The upload trigger reference.</param>
    /// <param name="urlAction">The upload action url.</param>
    /// <param name="responseControl">The response message control.</param>
    /// <param name="errorControl">The error message control.</param>
    /// <param name="fileExtensionFilter">The file extension filter.</param>
    /// <param name="filter">The file extension regular expression filter.</param>
    /// <param name="divInnerText">The target control element for text rendering.</param>
    _fileUpload: function(button, urlAction, responseControl, errorControl, fileExtensionFilter, filter, divInnerText) {

        $(document).ready(function() {
            // Varables.
            var interval;
            var originalUploadText = divInnerText;

            // Create a new ajax upload instance.
            var fileUploadInt =
            new AjaxUpload(button, {
                action: urlAction,
                onSubmit: function(file, ext) {

                    // If a extension filter exists
                    if (filter != null) {
                        // Validated the file extension with
                        // the filter.
                        if (ext && filter.test(ext)) {
                            // Continue upload.
                        } else {
                            // extension is not allowed and cancel upload.
                            if (errorControl != null) {
                                // Write the error.
                                errorControl.innerHTML = 'Error: only {' + fileExtensionFilter + '} file extensions are allowed.<br/>';
                            } else {
                                alert('Error: only {' + fileExtensionFilter + '} file extensions are allowed.');
                            }
                            return false;
                        }
                    }

                    // change button text, when user selects file
                    button.text('Uploading');

                    // If you want to allow uploading only 1 file at time,
                    // you can disable upload button
                    this.disable();

                    // Uploading -> Uploading. -> Uploading...
                    interval = window.setInterval(function() {
                        var text = button.text();
                        // Pad with . if too long.
                        if (text.length < 13) {
                            button.text(text + '.');
                        } else {
                            button.text('Uploading');
                        }
                    }, 200);
                },
                onComplete: function(file, response) {
                    button.text(originalUploadText);

                    // Clear the interval.
                    window.clearInterval(interval);

                    // enable upload button
                    this.enable();

                    // If there is a response.
                    if (response) {
                        // If a response control has been set write the response.
                        if (responseControl != null) {
                            // Write the response.
                            responseControl.innerHTML = response.replace("successfully", "successfully<br/>") + "<br/>";
                        } else {
                            alert('Response: ' + response);
                        }
                    } else {
                        alert('File: ' + file + ' has been uploaded.');
                    }
                }
            });
        });
    }
}

/// <summary>
/// Register the client control class created above.
/// </summary>
Nequeo.Web.UI.ScriptControl.FileUploadControl.registerClass("Nequeo.Web.UI.ScriptControl.FileUploadControl", Sys.UI.Control);

/// <summary>
/// Added to satisfy new notifyScriptLoaded() requirement
/// </summary>
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();