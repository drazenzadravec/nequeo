Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.PopupControlBehavior = function(element) {
AjaxControlToolkit.PopupControlBehavior.initializeBase(this, [element]);this._popupControlID = null;this._commitProperty = null;this._commitScript = null;this._position = null;this._offsetX = 0;this._offsetY = 0;this._extenderControlID = null;this._popupElement = null;this._popupBehavior = null;this._popupVisible = false;this._focusHandler = null;this._popupKeyDownHandler = null;this._popupClickHandler = null;this._bodyClickHandler = null;this._onShowJson = null;this._onHideJson = null;}
AjaxControlToolkit.PopupControlBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.PopupControlBehavior.callBaseMethod(this, 'initialize');var e = this.get_element();this._popupElement = $get(this._popupControlID);this._popupBehavior = $create(AjaxControlToolkit.PopupBehavior, { 'id':this.get_id()+'PopupBehavior', 'parentElement':e }, null, null, this._popupElement);if (this._onShowJson) {
this._popupBehavior.set_onShow(this._onShowJson);}
if (this._onHideJson) {
this._popupBehavior.set_onHide(this._onHideJson);}
this._focusHandler = Function.createDelegate(this, this._onFocus);this._popupClickHandler = Function.createDelegate(this, this._onPopupClick);this._bodyClickHandler = Function.createDelegate(this, this._onBodyClick);this._popupKeyDownHandler = Function.createDelegate(this, this._onPopupKeyDown);$addHandler(e, 'focus', this._focusHandler);$addHandler(e, 'click', this._focusHandler);$addHandler(document.body, 'click', this._bodyClickHandler);$addHandler(this._popupElement, 'click', this._popupClickHandler);$addHandler(this._popupElement, 'keydown', this._popupKeyDownHandler);this.registerPartialUpdateEvents();if(AjaxControlToolkit.PopupControlBehavior.__VisiblePopup && (this.get_id() == AjaxControlToolkit.PopupControlBehavior.__VisiblePopup.get_id())) {
this._onFocus(null);}
},
dispose : function() {
var e = this.get_element();this._onShowJson = null;this._onHideJson = null;if (this._popupBehavior) {
this._popupBehavior.dispose();this._popupBehavior = null;}
if (this._focusHandler) {
$removeHandler(e, 'focus', this._focusHandler);$removeHandler(e, 'click', this._focusHandler);this._focusHandler = null;}
if (this._bodyClickHandler) {
$removeHandler(document.body, 'click', this._bodyClickHandler);this._bodyClickHandler = null;}
if (this._popupClickHandler) {
$removeHandler(this._popupElement, 'click', this._popupClickHandler);this._popupClickHandler = null;}
if (this._popupKeyDownHandler) {
$removeHandler(this._popupElement, 'keydown', this._popupKeyDownHandler);this._popupKeyDownHandler = null;}
AjaxControlToolkit.PopupControlBehavior.callBaseMethod(this, 'dispose');},
showPopup : function() {
var old = AjaxControlToolkit.PopupControlBehavior.__VisiblePopup;if (old && old._popupBehavior) {
old.hidePopup();}
AjaxControlToolkit.PopupControlBehavior.callBaseMethod(this, 'populate');this._popupBehavior.set_x(this._getLeftOffset());this._popupBehavior.set_y(this._getTopOffset());this._popupBehavior.show();this._popupVisible = true;AjaxControlToolkit.PopupControlBehavior.__VisiblePopup = this;},
hidePopup : function() {
this._popupBehavior.hide();this._popupVisible = false;AjaxControlToolkit.PopupControlBehavior.__VisiblePopup = null;},
_onFocus : function(e) {
if (!this._popupVisible) {
this.showPopup();}
if (e) {
e.stopPropagation();}
},
_onPopupKeyDown : function(e) {
if (this._popupVisible && e.keyCode == 27 ) {
this.get_element().focus();}
},
_onPopupClick : function(e) {
e.stopPropagation();},
_onBodyClick : function() {
if (this._popupVisible) {
this.hidePopup();}
},
_close : function(result) {
var e = this.get_element();if (null != result) {
if ('$$CANCEL$$' != result) {
if (this._commitProperty) {
e[this._commitProperty] = result;} else if ('text' == e.type) {
e.value = result;} else {
Sys.Debug.assert(false, String.format(AjaxControlToolkit.Resources.PopupControl_NoDefaultProperty, e.id, e.type));}
if (this._commitScript) {
eval(this._commitScript);}
}
this.hidePopup();}
},
_partialUpdateEndRequest : function(sender, endRequestEventArgs) {
AjaxControlToolkit.PopupControlBehavior.callBaseMethod(this, '_partialUpdateEndRequest', [sender, endRequestEventArgs]);if (this.get_element()) {
var result = endRequestEventArgs.get_dataItems()[this.get_element().id];if ((undefined === result) &&
AjaxControlToolkit.PopupControlBehavior.__VisiblePopup &&
(this.get_id() == AjaxControlToolkit.PopupControlBehavior.__VisiblePopup.get_id())) {
result = endRequestEventArgs.get_dataItems()["_PopupControl_Proxy_ID_"];}
if (undefined !== result) {
this._close(result);}
}
},
_onPopulated : function(sender, eventArgs) {
AjaxControlToolkit.PopupControlBehavior.callBaseMethod(this, '_onPopulated', [sender, eventArgs]);if (this._popupVisible) {
this._popupBehavior.show();}
},
_getLeftOffset : function() {
if (AjaxControlToolkit.PopupControlPopupPosition.Left == this._position) {
return (-1 * this.get_element().offsetWidth) + this._offsetX;} else if (AjaxControlToolkit.PopupControlPopupPosition.Right == this._position) {
return this.get_element().offsetWidth + this._offsetX;} else {
return this._offsetX;}
},
_getTopOffset : function() {
var yoffSet;if(AjaxControlToolkit.PopupControlPopupPosition.Top == this._position) {
yoffSet = (-1 * this.get_element().offsetHeight) + this._offsetY;} else if (AjaxControlToolkit.PopupControlPopupPosition.Bottom == this._position) {
yoffSet = this.get_element().offsetHeight + this._offsetY;} else {
yoffSet = this._offsetY;}
return yoffSet;},
get_onShow : function() {
return this._popupBehavior ? this._popupBehavior.get_onShow() : this._onShowJson;},
set_onShow : function(value) {
if (this._popupBehavior) {
this._popupBehavior.set_onShow(value)
} else {
this._onShowJson = value;}
this.raisePropertyChanged('onShow');},
get_onShowBehavior : function() {
return this._popupBehavior ? this._popupBehavior.get_onShowBehavior() : null;},
onShow : function() {
if (this._popupBehavior) {
this._popupBehavior.onShow();}
},
get_onHide : function() {
return this._popupBehavior ? this._popupBehavior.get_onHide() : this._onHideJson;},
set_onHide : function(value) {
if (this._popupBehavior) {
this._popupBehavior.set_onHide(value)
} else {
this._onHideJson = value;}
this.raisePropertyChanged('onHide');},
get_onHideBehavior : function() {
return this._popupBehavior ? this._popupBehavior.get_onHideBehavior() : null;},
onHide : function() {
if (this._popupBehavior) {
this._popupBehavior.onHide();}
},
get_PopupControlID : function() {
return this._popupControlID;},
set_PopupControlID : function(value) {
if (this._popupControlID != value) { 
this._popupControlID = value;this.raisePropertyChanged('PopupControlID');}
},
get_CommitProperty : function() {
return this._commitProperty;},
set_CommitProperty : function(value) {
if (this._commitProperty != value) {
this._commitProperty = value;this.raisePropertyChanged('CommitProperty');}
},
get_CommitScript : function() {
return this._commitScript;},
set_CommitScript : function(value) {
if (this._commitScript != value) {
this._commitScript = value;this.raisePropertyChanged('CommitScript');}
},
get_Position : function() {
return this._position;},
set_Position : function(value) {
if (this._position != value) {
this._position = value;this.raisePropertyChanged('Position');}
},
get_ExtenderControlID : function() {
return this._extenderControlID;},
set_ExtenderControlID : function(value) {
if (this._extenderControlID != value) {
this._extenderControlID = value;this.raisePropertyChanged('ExtenderControlID');}
},
get_OffsetX : function() {
return this._offsetX;},
set_OffsetX : function(value) {
if (this._offsetX != value) {
this._offsetX = value;this.raisePropertyChanged('OffsetX');}
},
get_OffsetY : function() {
return this._offsetY;},
set_OffsetY : function(value) {
if (this._offsetY != value) {
this._offsetY = value;this.raisePropertyChanged('OffsetY');}
},
get_PopupVisible : function() {
return this._popupVisible;},
add_showing : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.add_showing(handler);}
},
remove_showing : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.remove_showing(handler);}
},
raiseShowing : function(eventArgs) {
if (this._popupBehavior) {
this._popupBehavior.raiseShowing(eventArgs);}
},
add_shown : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.add_shown(handler);}
},
remove_shown : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.remove_shown(handler);}
},
raiseShown : function(eventArgs) {
if (this._popupBehavior) {
this._popupBehavior.raiseShown(eventArgs);}
}, 
add_hiding : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.add_hiding(handler);}
},
remove_hiding : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.remove_hiding(handler);}
},
raiseHiding : function(eventArgs) {
if (this._popupBehavior) {
this._popupBehavior.raiseHiding(eventArgs);}
},
add_hidden : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.add_hidden(handler);}
},
remove_hidden : function(handler) {
if (this._popupBehavior) {
this._popupBehavior.remove_hidden(handler);}
},
raiseHidden : function(eventArgs) {
if (this._popupBehavior) {
this._popupBehavior.raiseHidden(eventArgs);}
}
}
AjaxControlToolkit.PopupControlBehavior.registerClass('AjaxControlToolkit.PopupControlBehavior', AjaxControlToolkit.DynamicPopulateBehaviorBase);AjaxControlToolkit.PopupControlBehavior.__VisiblePopup = null;AjaxControlToolkit.PopupControlPopupPosition = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.PopupControlPopupPosition.prototype = {
Center : 0,
Top : 1,
Left : 2,
Bottom : 3,
Right : 4
}
AjaxControlToolkit.PopupControlPopupPosition.registerEnum("AjaxControlToolkit.PopupControlPopupPosition", false);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
