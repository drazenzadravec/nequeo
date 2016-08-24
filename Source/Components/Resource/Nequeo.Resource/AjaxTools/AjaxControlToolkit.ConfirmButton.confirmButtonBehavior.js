Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.ConfirmButtonHiddenEventArgs = function(confirmed) {
AjaxControlToolkit.ConfirmButtonHiddenEventArgs.initializeBase(this);this._confirmed = confirmed;}
AjaxControlToolkit.ConfirmButtonHiddenEventArgs.prototype = {
get_confirmed : function() {
return this._confirmed;}
}
AjaxControlToolkit.ConfirmButtonHiddenEventArgs.registerClass('AjaxControlToolkit.ConfirmButtonHiddenEventArgs', Sys.EventArgs);AjaxControlToolkit.ConfirmButtonBehavior = function(element) {
AjaxControlToolkit.ConfirmButtonBehavior.initializeBase(this, [element]);this._ConfirmTextValue = null;this._OnClientCancelValue = null;this._ConfirmOnFormSubmit = false;this._displayModalPopupID = null;this._postBackScript = null;this._clickHandler = null;this._oldScript = null;}
AjaxControlToolkit.ConfirmButtonBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.ConfirmButtonBehavior.callBaseMethod(this, 'initialize');var element = this.get_element();this._clickHandler = Function.createDelegate(this, this._onClick);$addHandler(element, "click", this._clickHandler);this._oldScript = element.getAttribute("onclick");if (this._oldScript) { 
element.setAttribute("onclick", null);}
if (this._ConfirmOnFormSubmit && (typeof(WebForm_OnSubmit) == 'function') && !AjaxControlToolkit.ConfirmButtonBehavior._originalWebForm_OnSubmit) {
if (AjaxControlToolkit.TextBoxWatermarkBehavior && AjaxControlToolkit.TextBoxWatermarkBehavior._originalWebForm_OnSubmit) {
AjaxControlToolkit.ConfirmButtonBehavior._originalWebForm_OnSubmit = AjaxControlToolkit.TextBoxWatermarkBehavior._originalWebForm_OnSubmit;AjaxControlToolkit.TextBoxWatermarkBehavior._originalWebForm_OnSubmit = AjaxControlToolkit.ConfirmButtonBehavior.WebForm_OnSubmit;} else {
AjaxControlToolkit.ConfirmButtonBehavior._originalWebForm_OnSubmit = WebForm_OnSubmit;WebForm_OnSubmit = AjaxControlToolkit.ConfirmButtonBehavior.WebForm_OnSubmit;}
}
},
dispose : function() {
if (this._clickHandler) {
$removeHandler(this.get_element(), "click", this._clickHandler);this._clickHandler = null;}
if (this._oldScript) {
this.get_element().setAttribute("onclick", this._oldScript);this._oldScript = null;}
AjaxControlToolkit.ConfirmButtonBehavior.callBaseMethod(this, 'dispose');},
_onClick : function(e) {
if (this.get_element() && !this.get_element().disabled) {
if (this._ConfirmOnFormSubmit) {
AjaxControlToolkit.ConfirmButtonBehavior._clickedBehavior = this;} else {
if (!this._displayConfirmDialog()) {
e.preventDefault();return false;}
else if (this._oldScript) {
if (String.isInstanceOfType(this._oldScript)) {
eval(this._oldScript);}
else if (typeof(this._oldScript) == 'function'){
this._oldScript();} 
}
}
}
},
_displayConfirmDialog : function() {
var eventArgs = new Sys.CancelEventArgs();this.raiseShowing(eventArgs);if (eventArgs.get_cancel()) {
return;}
if(this._displayModalPopupID) {
var mpe = $find(this._displayModalPopupID);if (!mpe) {
throw Error.argument('displayModalPopupID', String.format(AjaxControlToolkit.Resources.CollapsiblePanel_NoControlID, this._displayModalPopupID));}
mpe.set_OnOkScript("$find('"+this.get_id()+"')._handleConfirmDialogCompletion(true);");mpe.set_OnCancelScript("$find('"+this.get_id()+"')._handleConfirmDialogCompletion(false);");mpe.show();return false;} else {
var result = window.confirm(this._ConfirmTextValue);this._handleConfirmDialogCompletion(result);return result;}
},
_handleConfirmDialogCompletion : function(result) {
this.raiseHidden(new AjaxControlToolkit.ConfirmButtonHiddenEventArgs(result));if (result) {
if (this._postBackScript ) {
eval(this._postBackScript);}
} else {
if (this._OnClientCancelValue) {
window[this._OnClientCancelValue]();}
}
},
get_OnClientCancel : function (){
return this._OnClientCancelValue;},
set_OnClientCancel : function (value) {
if (this._OnClientCancelValue != value) {
this._OnClientCancelValue = value;this.raisePropertyChanged('OnClientCancel');}
},
get_ConfirmText : function() {
return this._ConfirmTextValue;},
set_ConfirmText : function(value) {
if (this._ConfirmTextValue != value) {
this._ConfirmTextValue = value;this.raisePropertyChanged('ConfirmText');}
},
get_ConfirmOnFormSubmit : function() {
return this._ConfirmOnFormSubmit;},
set_ConfirmOnFormSubmit : function(value) {
if (this._ConfirmOnFormSubmit != value) {
this._ConfirmOnFormSubmit = value;this.raisePropertyChanged('ConfirmOnFormSubmit');}
},
get_displayModalPopupID : function() {
return this._displayModalPopupID;},
set_displayModalPopupID : function(value) {
if (this._displayModalPopupID != value) {
this._displayModalPopupID = value;this.raisePropertyChanged('displayModalPopupID');}
},
get_postBackScript : function() {
return this._postBackScript;},
set_postBackScript : function(value) {
if (this._postBackScript != value) {
this._postBackScript = value;this.raisePropertyChanged('postBackScript');}
},
add_showing : function(handler) {
this.get_events().addHandler('showing', handler);},
remove_showing : function(handler) {
this.get_events().removeHandler('showing', handler);},
raiseShowing : function(eventArgs) {
var handler = this.get_events().getHandler('showing');if (handler) {
handler(this, eventArgs);}
},
add_hidden : function(handler) {
this.get_events().addHandler('hidden', handler);},
remove_hidden : function(handler) {
this.get_events().removeHandler('hidden', handler);},
raiseHidden : function(eventArgs) {
var handler = this.get_events().getHandler('hidden');if (handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit.ConfirmButtonBehavior.registerClass('AjaxControlToolkit.ConfirmButtonBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.ConfirmButtonBehavior.WebForm_OnSubmit = function() {
var result = AjaxControlToolkit.ConfirmButtonBehavior._originalWebForm_OnSubmit();if (result && AjaxControlToolkit.ConfirmButtonBehavior._clickedBehavior) {
result = AjaxControlToolkit.ConfirmButtonBehavior._clickedBehavior._displayConfirmDialog();}
AjaxControlToolkit.ConfirmButtonBehavior._clickedBehavior = null;return result;}

if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
