Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.PopupBehavior = function(element) {
AjaxControlToolkit.PopupBehavior.initializeBase(this, [element]);this._x = 0;this._y = 0;this._positioningMode = AjaxControlToolkit.PositioningMode.Absolute;this._parentElement = null;this._parentElementID = null;this._moveHandler = null;this._firstPopup = true;this._originalParent = null;this._visible = false;this._onShow = null;this._onShowEndedHandler = null;this._onHide = null;this._onHideEndedHandler = null;}
AjaxControlToolkit.PopupBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.PopupBehavior.callBaseMethod(this, 'initialize');this._hidePopup();this.get_element().style.position = "absolute";this._onShowEndedHandler = Function.createDelegate(this, this._onShowEnded);this._onHideEndedHandler = Function.createDelegate(this, this._onHideEnded);},
dispose : function() {
var element = this.get_element();if (element) {
if (this._visible) {
this.hide();}
if (this._originalParent) {
element.parentNode.removeChild(element);this._originalParent.appendChild(element);this._originalParent = null;}
element._hideWindowedElementsIFrame = null;}
this._parentElement = null;if (this._onShow && this._onShow.get_animation() && this._onShowEndedHandler) {
this._onShow.get_animation().remove_ended(this._onShowEndedHandler);}
this._onShowEndedHandler = null;this._onShow = null;if (this._onHide && this._onHide.get_animation() && this._onHideEndedHandler) {
this._onHide.get_animation().remove_ended(this._onHideEndedHandler);}
this._onHideEndedHandler = null;this._onHide = null;AjaxControlToolkit.PopupBehavior.callBaseMethod(this, 'dispose');},
show : function() {
if (this._visible) {
return;}
var eventArgs = new Sys.CancelEventArgs();this.raiseShowing(eventArgs);if (eventArgs.get_cancel()) {
return;}
this._visible = true;var element = this.get_element();$common.setVisible(element, true);this.setupPopup();if (this._onShow) {
$common.setVisible(element, false);this.onShow();} else {
this.raiseShown(Sys.EventArgs.Empty);}
},
hide : function() {
if (!this._visible) {
return;}
var eventArgs = new Sys.CancelEventArgs();this.raiseHiding(eventArgs);if (eventArgs.get_cancel()) {
return;}
this._visible = false;if (this._onHide) {
this.onHide();} else {
this._hidePopup();this._hideCleanup();}
},
getBounds : function() {
var element = this.get_element();var offsetParent = element.offsetParent || document.documentElement;var diff;var parentBounds;if (this._parentElement) {
parentBounds = $common.getBounds(this._parentElement);var offsetParentLocation = $common.getLocation(offsetParent);diff = {x: parentBounds.x - offsetParentLocation.x, y:parentBounds.y - offsetParentLocation.y};} else {
parentBounds = $common.getBounds(offsetParent);diff = {x:0, y:0};}
var width = element.offsetWidth - (element.clientLeft ? element.clientLeft * 2 : 0);var height = element.offsetHeight - (element.clientTop ? element.clientTop * 2 : 0);if (this._firstpopup) {
element.style.width = width + "px";this._firstpopup = false;}
var position;switch (this._positioningMode) {
case AjaxControlToolkit.PositioningMode.Center:
position = {
x: Math.round(parentBounds.width / 2 - width / 2),
y: Math.round(parentBounds.height / 2 - height / 2)
};break;case AjaxControlToolkit.PositioningMode.BottomLeft:
position = {
x: 0,
y: parentBounds.height
};break;case AjaxControlToolkit.PositioningMode.BottomRight:
position = {
x: parentBounds.width - width,
y: parentBounds.height
};break;case AjaxControlToolkit.PositioningMode.TopLeft:
position = {
x: 0,
y: -element.offsetHeight
};break;case AjaxControlToolkit.PositioningMode.TopRight:
position = {
x: parentBounds.width - width,
y: -element.offsetHeight
};break;case AjaxControlToolkit.PositioningMode.Right:
position = {
x: parentBounds.width,
y: 0
};break;case AjaxControlToolkit.PositioningMode.Left:
position = {
x: -element.offsetWidth,
y: 0
};break;default:
position = {x: 0, y: 0};}
position.x += this._x + diff.x;position.y += this._y + diff.y;return new Sys.UI.Bounds(position.x, position.y, width, height);},
adjustPopupPosition : function(bounds) {
var element = this.get_element();if (!bounds) {
bounds = this.getBounds();}
var newPosition = $common.getBounds(element);var updateNeeded = false;if (newPosition.x < 0) {
bounds.x -= newPosition.x;updateNeeded = true;}
if (newPosition.y < 0) {
bounds.y -= newPosition.y;updateNeeded = true;}
if (updateNeeded) {
$common.setLocation(element, bounds);}
},
addBackgroundIFrame : function() {
var element = this.get_element();if ((Sys.Browser.agent === Sys.Browser.InternetExplorer) && (Sys.Browser.version < 7)) {
var childFrame = element._hideWindowedElementsIFrame;if (!childFrame) {
childFrame = document.createElement("iframe");childFrame.src = "javascript:'<html></html>';";childFrame.style.position = "absolute";childFrame.style.display = "none";childFrame.scrolling = "no";childFrame.frameBorder = "0";childFrame.tabIndex = "-1";childFrame.style.filter = "progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)";element.parentNode.insertBefore(childFrame, element);element._hideWindowedElementsIFrame = childFrame;this._moveHandler = Function.createDelegate(this, this._onMove);Sys.UI.DomEvent.addHandler(element, "move", this._moveHandler);}
$common.setBounds(childFrame, $common.getBounds(element));childFrame.style.display = element.style.display;if (element.currentStyle && element.currentStyle.zIndex) {
childFrame.style.zIndex = element.currentStyle.zIndex;} else if (element.style.zIndex) {
childFrame.style.zIndex = element.style.zIndex;}
}
},
setupPopup : function() {
var element = this.get_element();var bounds = this.getBounds();$common.setLocation(element, bounds);this.adjustPopupPosition(bounds);element.zIndex = 1000;this.addBackgroundIFrame();},
_hidePopup : function() {
var element = this.get_element();$common.setVisible(element, false);if (element.originalWidth) {
element.style.width = element.originalWidth + "px";element.originalWidth = null;}
},
_hideCleanup : function() {
var element = this.get_element();if (this._moveHandler) {
Sys.UI.DomEvent.removeHandler(element, "move", this._moveHandler);this._moveHandler = null;}
if (Sys.Browser.agent === Sys.Browser.InternetExplorer) {
var childFrame = element._hideWindowedElementsIFrame;if (childFrame) {
childFrame.style.display = "none";}
}
this.raiseHidden(Sys.EventArgs.Empty);},
_onMove : function() {
var element = this.get_element();if (element._hideWindowedElementsIFrame) {
element.parentNode.insertBefore(element._hideWindowedElementsIFrame, element);element._hideWindowedElementsIFrame.style.top = element.style.top;element._hideWindowedElementsIFrame.style.left = element.style.left;}
},
get_onShow : function() {
return this._onShow ? this._onShow.get_json() : null;},
set_onShow : function(value) {
if (!this._onShow) {
this._onShow = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onShow.initialize();}
this._onShow.set_json(value);var animation = this._onShow.get_animation();if (animation) {
animation.add_ended(this._onShowEndedHandler);}
this.raisePropertyChanged('onShow');},
get_onShowBehavior : function() {
return this._onShow;},
onShow : function() {
if (this._onShow) {
if (this._onHide) {
this._onHide.quit();}
this._onShow.play();}
},
_onShowEnded : function() {
this.adjustPopupPosition();this.addBackgroundIFrame();this.raiseShown(Sys.EventArgs.Empty);},
get_onHide : function() {
return this._onHide ? this._onHide.get_json() : null;},
set_onHide : function(value) {
if (!this._onHide) {
this._onHide = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onHide.initialize();}
this._onHide.set_json(value);var animation = this._onHide.get_animation();if (animation) {
animation.add_ended(this._onHideEndedHandler);}
this.raisePropertyChanged('onHide');},
get_onHideBehavior : function() {
return this._onHide;},
onHide : function() {
if (this._onHide) {
if (this._onShow) {
this._onShow.quit();}
this._onHide.play();}
},
_onHideEnded : function() {
this._hideCleanup();},
get_parentElement : function() {
if (!this._parentElement && this._parentElementID) {
this.set_parentElement($get(this._parentElementID));Sys.Debug.assert(this._parentElement != null, String.format(AjaxControlToolkit.Resources.PopupExtender_NoParentElement, this._parentElementID));} 
return this._parentElement;},
set_parentElement : function(element) {
this._parentElement = element;this.raisePropertyChanged('parentElement');},
get_parentElementID : function() {
if (this._parentElement) {
return this._parentElement.id
}
return this._parentElementID;},
set_parentElementID : function(elementID) {
this._parentElementID = elementID;if (this.get_isInitialized()) {
this.set_parentElement($get(elementID));}
},
get_positioningMode : function() {
return this._positioningMode;},
set_positioningMode : function(mode) {
this._positioningMode = mode;this.raisePropertyChanged('positioningMode');},
get_x : function() {
return this._x;},
set_x : function(value) {
if (value != this._x) {
this._x = value;if (this._visible) {
this.setupPopup();}
this.raisePropertyChanged('x');}
},
get_y : function() {
return this._y;},
set_y : function(value) {
if (value != this._y) {
this._y = value;if (this._visible) {
this.setupPopup();}
this.raisePropertyChanged('y');}
},
get_visible : function() {
return this._visible;},
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
AjaxControlToolkit.PopupBehavior.registerClass('AjaxControlToolkit.PopupBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.PositioningMode = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.PositioningMode.prototype = {
Absolute: 0,
Center: 1,
BottomLeft: 2,
BottomRight: 3,
TopLeft: 4,
TopRight: 5,
Right: 6,
Left: 7
}
AjaxControlToolkit.PositioningMode.registerEnum('AjaxControlToolkit.PositioningMode');
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
