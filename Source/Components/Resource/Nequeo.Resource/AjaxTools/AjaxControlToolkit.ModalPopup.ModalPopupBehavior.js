Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.ModalPopupRepositionMode = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.ModalPopupRepositionMode.prototype = {
None : 0,
RepositionOnWindowResize : 1,
RepositionOnWindowScroll : 2,
RepositionOnWindowResizeAndScroll : 3
}
AjaxControlToolkit.ModalPopupRepositionMode.registerEnum('AjaxControlToolkit.ModalPopupRepositionMode');AjaxControlToolkit.ModalPopupBehavior = function(element) {
AjaxControlToolkit.ModalPopupBehavior.initializeBase(this, [element]);this._PopupControlID = null;this._PopupDragHandleControlID = null;this._BackgroundCssClass = null;this._DropShadow = false;this._Drag = false;this._OkControlID = null;this._CancelControlID = null;this._OnOkScript = null;this._OnCancelScript = null;this._xCoordinate = -1;this._yCoordinate = -1;this._repositionMode = AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll;this._backgroundElement = null;this._foregroundElement = null;this._relativeOrAbsoluteParentElement = null;this._popupElement = null;this._dragHandleElement = null;this._showHandler = null;this._okHandler = null;this._cancelHandler = null;this._scrollHandler = null;this._resizeHandler = null;this._windowHandlersAttached = false;this._dropShadowBehavior = null;this._dragBehavior = null;this._isIE6 = false;this._saveTabIndexes = new Array();this._saveDesableSelect = new Array();this._tagWithTabIndex = new Array('A','AREA','BUTTON','INPUT','OBJECT','SELECT','TEXTAREA','IFRAME');}
AjaxControlToolkit.ModalPopupBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.ModalPopupBehavior.callBaseMethod(this, 'initialize');this._isIE6 = (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7);if(this._PopupDragHandleControlID)
this._dragHandleElement = $get(this._PopupDragHandleControlID);this._popupElement = $get(this._PopupControlID);if(this._DropShadow)
{
this._foregroundElement = document.createElement('div');this._foregroundElement.id = this.get_id() + '_foregroundElement';this._popupElement.parentNode.appendChild(this._foregroundElement);this._foregroundElement.appendChild(this._popupElement);}
else
{
this._foregroundElement = this._popupElement;}
this._backgroundElement = document.createElement('div');this._backgroundElement.id = this.get_id() + '_backgroundElement';this._backgroundElement.style.display = 'none';this._backgroundElement.style.position = 'fixed';this._backgroundElement.style.left = '0px';this._backgroundElement.style.top = '0px';this._backgroundElement.style.zIndex = 10000;if (this._BackgroundCssClass) {
this._backgroundElement.className = this._BackgroundCssClass;}
this._foregroundElement.parentNode.appendChild(this._backgroundElement);this._foregroundElement.style.display = 'none';this._foregroundElement.style.position = 'fixed';this._foregroundElement.style.zIndex = $common.getCurrentStyle(this._backgroundElement, 'zIndex', this._backgroundElement.style.zIndex) + 1;this._showHandler = Function.createDelegate(this, this._onShow);$addHandler(this.get_element(), 'click', this._showHandler);if (this._OkControlID) {
this._okHandler = Function.createDelegate(this, this._onOk);$addHandler($get(this._OkControlID), 'click', this._okHandler);}
if (this._CancelControlID) {
this._cancelHandler = Function.createDelegate(this, this._onCancel);$addHandler($get(this._CancelControlID), 'click', this._cancelHandler);}
this._scrollHandler = Function.createDelegate(this, this._onLayout);this._resizeHandler = Function.createDelegate(this, this._onLayout);this.registerPartialUpdateEvents();},
dispose : function() {
this._hideImplementation();if (this._foregroundElement && this._foregroundElement.parentNode) {
this._foregroundElement.parentNode.removeChild(this._backgroundElement);if(this._DropShadow) {
this._foregroundElement.parentNode.appendChild(this._popupElement);this._foregroundElement.parentNode.removeChild(this._foregroundElement);}
}
this._scrollHandler = null;this._resizeHandler = null;if (this._cancelHandler && $get(this._CancelControlID)) {
$removeHandler($get(this._CancelControlID), 'click', this._cancelHandler);this._cancelHandler = null;}
if (this._okHandler && $get(this._OkControlID)) {
$removeHandler($get(this._OkControlID), 'click', this._okHandler);this._okHandler = null;}
if (this._showHandler) {
$removeHandler(this.get_element(), 'click', this._showHandler);this._showHandler = null;}
AjaxControlToolkit.ModalPopupBehavior.callBaseMethod(this, 'dispose');},
_attachPopup : function() {
if (this._DropShadow && !this._dropShadowBehavior) {
this._dropShadowBehavior = $create(AjaxControlToolkit.DropShadowBehavior, {}, null, null, this._popupElement);}
if (this._dragHandleElement && !this._dragBehavior) {
this._dragBehavior = $create(AjaxControlToolkit.FloatingBehavior, {"handle" : this._dragHandleElement}, null, null, this._foregroundElement);} 
$addHandler(window, 'resize', this._resizeHandler);$addHandler(window, 'scroll', this._scrollHandler);this._windowHandlersAttached = true;},
_detachPopup : function() {
if (this._windowHandlersAttached) {
if (this._scrollHandler) {
$removeHandler(window, 'scroll', this._scrollHandler);}
if (this._resizeHandler) {
$removeHandler(window, 'resize', this._resizeHandler);}
this._windowHandlersAttached = false;}
if (this._dragBehavior) {
this._dragBehavior.dispose();this._dragBehavior = null;} 
if (this._dropShadowBehavior) {
this._dropShadowBehavior.dispose();this._dropShadowBehavior = null;}
},
_onShow : function(e) {
if (!this.get_element().disabled) {
this.show();e.preventDefault();return false;}
},
_onOk : function(e) {
var element = $get(this._OkControlID);if (element && !element.disabled) {
if (this.hide() && this._OnOkScript) {
window.setTimeout(this._OnOkScript, 0);}
e.preventDefault();return false;}
},
_onCancel : function(e) {
var element = $get(this._CancelControlID);if (element && !element.disabled) {
if (this.hide() && this._OnCancelScript) {
window.setTimeout(this._OnCancelScript, 0);}
e.preventDefault();return false;}
},
_onLayout : function(e) {
var positioning = this.get_repositionMode();if (((positioning === AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowScroll) ||
(positioning === AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll)) && (e.type === 'scroll')) {
this._layout();} else if (((positioning === AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowResize) ||
(positioning === AjaxControlToolkit.ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll)) && (e.type === 'resize')) {
this._layout();} else {
this._layoutBackgroundElement();}
},
show : function() {
var eventArgs = new Sys.CancelEventArgs();this.raiseShowing(eventArgs);if (eventArgs.get_cancel()) {
return;}
this.populate();this._attachPopup();this._backgroundElement.style.display = '';this._foregroundElement.style.display = '';this._popupElement.style.display = '';if (this._isIE6) {
this._foregroundElement.style.position = 'absolute';this._backgroundElement.style.position = 'absolute';var tempRelativeOrAbsoluteParent = this._foregroundElement.parentNode;while (tempRelativeOrAbsoluteParent && (tempRelativeOrAbsoluteParent != document.documentElement)) {
if((tempRelativeOrAbsoluteParent.style.position != 'relative') && (tempRelativeOrAbsoluteParent.style.position != 'absolute')) {
tempRelativeOrAbsoluteParent = tempRelativeOrAbsoluteParent.parentNode;} else {
this._relativeOrAbsoluteParentElement = tempRelativeOrAbsoluteParent;break;}
} 
} 
this.disableTab();this._layout();this._layout();this.raiseShown(Sys.EventArgs.Empty);},
disableTab : function() {
var i = 0;var tagElements;var tagElementsInPopUp = new Array();Array.clear(this._saveTabIndexes);for (var j = 0;j < this._tagWithTabIndex.length;j++) {
tagElements = this._foregroundElement.getElementsByTagName(this._tagWithTabIndex[j]);for (var k = 0 ;k < tagElements.length;k++) {
tagElementsInPopUp[i] = tagElements[k];i++;}
}
i = 0;for (var j = 0;j < this._tagWithTabIndex.length;j++) {
tagElements = document.getElementsByTagName(this._tagWithTabIndex[j]);for (var k = 0 ;k < tagElements.length;k++) {
if (Array.indexOf(tagElementsInPopUp, tagElements[k]) == -1) {
this._saveTabIndexes[i] = {tag: tagElements[k], index: tagElements[k].tabIndex};tagElements[k].tabIndex="-1";i++;}
}
}
i = 0;if ((Sys.Browser.agent === Sys.Browser.InternetExplorer) && (Sys.Browser.version < 7)) {
var tagSelectInPopUp = new Array();for (var j = 0;j < this._tagWithTabIndex.length;j++) {
tagElements = this._foregroundElement.getElementsByTagName('SELECT');for (var k = 0 ;k < tagElements.length;k++) {
tagSelectInPopUp[i] = tagElements[k];i++;}
}
i = 0;Array.clear(this._saveDesableSelect);tagElements = document.getElementsByTagName('SELECT');for (var k = 0 ;k < tagElements.length;k++) {
if (Array.indexOf(tagSelectInPopUp, tagElements[k]) == -1) {
this._saveDesableSelect[i] = {tag: tagElements[k], visib: $common.getCurrentStyle(tagElements[k], 'visibility')} ;tagElements[k].style.visibility = 'hidden';i++;}
}
}
},
restoreTab : function() {
for (var i = 0;i < this._saveTabIndexes.length;i++) {
this._saveTabIndexes[i].tag.tabIndex = this._saveTabIndexes[i].index;}
Array.clear(this._saveTabIndexes);if ((Sys.Browser.agent === Sys.Browser.InternetExplorer) && (Sys.Browser.version < 7)) {
for (var k = 0 ;k < this._saveDesableSelect.length;k++) {
this._saveDesableSelect[k].tag.style.visibility = this._saveDesableSelect[k].visib;}
Array.clear(this._saveDesableSelect);}
},
hide : function() {
var eventArgs = new Sys.CancelEventArgs();this.raiseHiding(eventArgs);if (eventArgs.get_cancel()) {
return false;}
this._hideImplementation();this.raiseHidden(Sys.EventArgs.Empty);return true;},
_hideImplementation : function() {
this._backgroundElement.style.display = 'none';this._foregroundElement.style.display = 'none';this.restoreTab();this._detachPopup();},
_layout : function() {
var scrollLeft = (document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft);var scrollTop = (document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop);var clientBounds = $common.getClientBounds();var clientWidth = clientBounds.width;var clientHeight = clientBounds.height;this._layoutBackgroundElement();var xCoord = 0;var yCoord = 0;if(this._xCoordinate < 0) {
var foregroundelementwidth = this._foregroundElement.offsetWidth? this._foregroundElement.offsetWidth: this._foregroundElement.scrollWidth;xCoord = ((clientWidth-foregroundelementwidth)/2);if (this._foregroundElement.style.position == 'absolute') {
xCoord += scrollLeft;}
this._foregroundElement.style.left = xCoord + 'px';} else {
if(this._isIE6) {
this._foregroundElement.style.left = (this._xCoordinate + scrollLeft) + 'px';xCoord = this._xCoordinate + scrollLeft;}
else {
this._foregroundElement.style.left = this._xCoordinate + 'px';xCoord = this._xCoordinate;}
}
if(this._yCoordinate < 0) {
var foregroundelementheight = this._foregroundElement.offsetHeight? this._foregroundElement.offsetHeight: this._foregroundElement.scrollHeight;yCoord = ((clientHeight-foregroundelementheight)/2);if (this._foregroundElement.style.position == 'absolute') {
yCoord += scrollTop;}
this._foregroundElement.style.top = yCoord + 'px';} else {
if(this._isIE6) {
this._foregroundElement.style.top = (this._yCoordinate + scrollTop) + 'px';yCoord = this._yCoordinate + scrollTop;}
else {
this._foregroundElement.style.top = this._yCoordinate + 'px';yCoord = this._yCoordinate;}
}
this._layoutForegroundElement(xCoord, yCoord);if (this._dropShadowBehavior) {
this._dropShadowBehavior.setShadow();window.setTimeout(Function.createDelegate(this, this._fixupDropShadowBehavior), 0);}
this._layoutBackgroundElement();},
_layoutForegroundElement : function(xCoord, yCoord) {
if (this._isIE6 && this._relativeOrAbsoluteParentElement) {
var foregroundLocation = $common.getLocation(this._foregroundElement);var relativeParentLocation = $common.getLocation(this._relativeOrAbsoluteParentElement);var getLocationXCoord = foregroundLocation.x;if (getLocationXCoord != xCoord) {
this._foregroundElement.style.left = (xCoord - relativeParentLocation.x) + 'px';} 
var getLocationYCoord = foregroundLocation.y;if (getLocationYCoord != yCoord) {
this._foregroundElement.style.top = (yCoord - relativeParentLocation.y) + 'px';} 
}
},
_layoutBackgroundElement : function() {
if(this._isIE6) { 
var backgroundLocation = $common.getLocation(this._backgroundElement);var backgroundXCoord = backgroundLocation.x;if (backgroundXCoord != 0) {
this._backgroundElement.style.left = (-backgroundXCoord) + 'px';} 
var backgroundYCoord = backgroundLocation.y;if (backgroundYCoord != 0) {
this._backgroundElement.style.top = (-backgroundYCoord) + 'px';} 
}
var clientBounds = $common.getClientBounds();var clientWidth = clientBounds.width;var clientHeight = clientBounds.height;this._backgroundElement.style.width = Math.max(Math.max(document.documentElement.scrollWidth, document.body.scrollWidth), clientWidth)+'px';this._backgroundElement.style.height = Math.max(Math.max(document.documentElement.scrollHeight, document.body.scrollHeight), clientHeight)+'px';},
_fixupDropShadowBehavior : function() {
if (this._dropShadowBehavior) {
this._dropShadowBehavior.setShadow();}
},
_partialUpdateEndRequest : function(sender, endRequestEventArgs) {
AjaxControlToolkit.ModalPopupBehavior.callBaseMethod(this, '_partialUpdateEndRequest', [sender, endRequestEventArgs]);if (this.get_element()) {
var action = endRequestEventArgs.get_dataItems()[this.get_element().id];if ("show" == action) {
this.show();} else if ("hide" == action) {
this.hide();}
}
this._layout();},
_onPopulated : function(sender, eventArgs) {
AjaxControlToolkit.ModalPopupBehavior.callBaseMethod(this, '_onPopulated', [sender, eventArgs]);this._layout();},
get_PopupControlID : function() {
return this._PopupControlID;},
set_PopupControlID : function(value) {
if (this._PopupControlID != value) {
this._PopupControlID = value;this.raisePropertyChanged('PopupControlID');}
},
get_X: function() {
return this._xCoordinate;},
set_X: function(value) {
if (this._xCoordinate != value) {
this._xCoordinate = value;this.raisePropertyChanged('X');}
},
get_Y: function() {
return this._yCoordinate;},
set_Y: function(value) {
if (this._yCoordinate != value) {
this._yCoordinate = value;this.raisePropertyChanged('Y');}
},
get_PopupDragHandleControlID : function() {
return this._PopupDragHandleControlID;},
set_PopupDragHandleControlID : function(value) {
if (this._PopupDragHandleControlID != value) {
this._PopupDragHandleControlID = value;this.raisePropertyChanged('PopupDragHandleControlID');}
},
get_BackgroundCssClass : function() {
return this._BackgroundCssClass;},
set_BackgroundCssClass : function(value) {
if (this._BackgroundCssClass != value) {
this._BackgroundCssClass = value;this.raisePropertyChanged('BackgroundCssClass');}
},
get_DropShadow : function() {
return this._DropShadow;},
set_DropShadow : function(value) {
if (this._DropShadow != value) {
this._DropShadow = value;this.raisePropertyChanged('DropShadow');}
},
get_Drag : function() {
return this._Drag;},
set_Drag : function(value) {
if (this._Drag != value) {
this._Drag = value;this.raisePropertyChanged('Drag');}
},
get_OkControlID : function() {
return this._OkControlID;},
set_OkControlID : function(value) {
if (this._OkControlID != value) {
this._OkControlID = value;this.raisePropertyChanged('OkControlID');}
},
get_CancelControlID : function() {
return this._CancelControlID;},
set_CancelControlID : function(value) {
if (this._CancelControlID != value) {
this._CancelControlID = value;this.raisePropertyChanged('CancelControlID');}
},
get_OnOkScript : function() {
return this._OnOkScript;},
set_OnOkScript : function(value) {
if (this._OnOkScript != value) {
this._OnOkScript = value;this.raisePropertyChanged('OnOkScript');}
},
get_OnCancelScript : function() {
return this._OnCancelScript;},
set_OnCancelScript : function(value) {
if (this._OnCancelScript != value) {
this._OnCancelScript = value;this.raisePropertyChanged('OnCancelScript');}
},
get_repositionMode : function() {
return this._repositionMode;},
set_repositionMode : function(value) {
if (this._repositionMode !== value) {
this._repositionMode = value;this.raisePropertyChanged('RepositionMode');}
},
add_showing : function(handler) {
this.get_events().addHandler('showing', handler);},
remove_showing : function(handler) {
this.get_events().removeHandler('showing', handler);},
raiseShowing : function(eventArgs) {
var handler = this.get_events().getHandler('showing');if (handler) {
handler(this, eventArgs);}
},
add_shown : function(handler) {
this.get_events().addHandler('shown', handler);},
remove_shown : function(handler) {
this.get_events().removeHandler('shown', handler);},
raiseShown : function(eventArgs) {
var handler = this.get_events().getHandler('shown');if (handler) {
handler(this, eventArgs);}
},
add_hiding : function(handler) {
this.get_events().addHandler('hiding', handler);},
remove_hiding : function(handler) {
this.get_events().removeHandler('hiding', handler);},
raiseHiding : function(eventArgs) {
var handler = this.get_events().getHandler('hiding');if (handler) {
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
AjaxControlToolkit.ModalPopupBehavior.registerClass('AjaxControlToolkit.ModalPopupBehavior', AjaxControlToolkit.DynamicPopulateBehaviorBase);AjaxControlToolkit.ModalPopupBehavior.invokeViaServer = function(behaviorID, show) {
var behavior = $find(behaviorID);if (behavior) {
if (show) {
behavior.show();} else {
behavior.hide();}
}
}

if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
