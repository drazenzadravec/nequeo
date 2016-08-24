Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.BoxCorners = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.BoxCorners.prototype = {
None : 0x00,
TopLeft : 0x01,
TopRight : 0x02,
BottomRight : 0x04,
BottomLeft : 0x08,
Top : 0x01 | 0x02,
Right : 0x02 | 0x04,
Bottom : 0x04 | 0x08,
Left : 0x08 | 0x01,
All : 0x01 | 0x02 | 0x04 | 0x08
}
AjaxControlToolkit.BoxCorners.registerEnum("AjaxControlToolkit.BoxCorners", true);AjaxControlToolkit.RoundedCornersBehavior = function(element) {
AjaxControlToolkit.RoundedCornersBehavior.initializeBase(this, [element]);this._corners = AjaxControlToolkit.BoxCorners.All;this._radius = 5;this._color = null;this._parentDiv = null;this._originalStyle = null;this._borderColor = null;this._isDirty = true;}
AjaxControlToolkit.RoundedCornersBehavior.prototype = {
initialize: function() {
AjaxControlToolkit.RoundedCornersBehavior.callBaseMethod(this, 'initialize');this.update();},
dispose: function() {
this.disposeParentDiv();AjaxControlToolkit.RoundedCornersBehavior.callBaseMethod(this, 'dispose');},
update: function() {
var e = this.get_element();if (!e || !this._isDirty || this.get_isUpdating()) return;this.disposeParentDiv();var color = this.getBackgroundColor();var originalWidth = e.offsetWidth;var newParent = e.cloneNode(false);this.moveChildren(e, newParent);this._originalStyle = e.style.cssText;e.style.backgroundColor = "transparent";e.style.verticalAlign = "top";e.style.padding = "0";e.style.overflow = "";e.style.className = "";if (e.style.height && e.style.height != "auto") {
e.style.height = parseInt($common.getCurrentStyle(e, 'height')) + (this._radius * 2) + "px";} else {
if (!e.style.width && (0 < originalWidth)) {
e.style.width = originalWidth + "px";}
}
newParent.style.position = "";newParent.style.border = "";newParent.style.margin = "";newParent.style.width = "100%";newParent.id = "";newParent.removeAttribute("control");if (this._borderColor) {
newParent.style.borderTopStyle = "none";newParent.style.borderBottomStyle = "none";newParent.style.borderLeftStyle = "solid";newParent.style.borderRightStyle = "solid";newParent.style.borderLeftColor = this._borderColor;newParent.style.borderRightColor = this._borderColor;newParent.style.borderLeftWidth = "1px";newParent.style.borderRightWidth = "1px";if (this._radius == 0) {
newParent.style.borderTopStyle = "solid";newParent.style.borderBottomStyle = "solid";newParent.style.borderTopColor = this._borderColor;newParent.style.borderBottomColor = this._borderColor;newParent.style.borderTopWidth = "1px";newParent.style.borderBottomWidth = "1px";}
} else {
newParent.style.borderTopStyle = "none";newParent.style.borderBottomStyle = "none";newParent.style.borderLeftStyle = "none";newParent.style.borderRightStyle = "none";}
var lastDiv = null;var radius = this._radius;var lines = this._radius;var lastDelta = 0;for (var i = lines;i > 0;i--) {
var angle = Math.acos(i / radius);var delta = radius - Math.round(Math.sin(angle) * radius);var newDiv = document.createElement("DIV");newDiv.__roundedDiv = true;newDiv.style.backgroundColor = color;newDiv.style.marginLeft = delta + "px";newDiv.style.marginRight = (delta - (this._borderColor ? 2 : 0)) + "px";newDiv.style.height = "1px";newDiv.style.fontSize = "1px";newDiv.style.overflow = "hidden";if (this._borderColor) {
newDiv.style.borderLeftStyle = "solid";newDiv.style.borderRightStyle = "solid";newDiv.style.borderLeftColor = this._borderColor;newDiv.style.borderRightColor = this._borderColor;var offset = Math.max(0, lastDelta - delta - 1);newDiv.style.borderLeftWidth = (offset + 1) + "px";newDiv.style.borderRightWidth = (offset + 1) + "px";if (i == lines) {
newDiv.__roundedDivNoBorder = true;newDiv.style.backgroundColor = this._borderColor;}
}
e.insertBefore(newDiv, lastDiv);var topDiv = newDiv;newDiv = newDiv.cloneNode(true);newDiv.__roundedDiv = true;e.insertBefore(newDiv, lastDiv);var bottomDiv = newDiv;lastDiv = newDiv;lastDelta = delta;if (!this.isCornerSet(AjaxControlToolkit.BoxCorners.TopLeft)) {
topDiv.style.marginLeft = "0";if (this._borderColor) {
topDiv.style.borderLeftWidth = "1px";}
}
if (!this.isCornerSet(AjaxControlToolkit.BoxCorners.TopRight)) {
topDiv.style.marginRight = "0";if (this._borderColor) {
topDiv.style.borderRightWidth = "1px";topDiv.style.marginRight = "-2px";}
}
if (!this.isCornerSet(AjaxControlToolkit.BoxCorners.BottomLeft)) {
bottomDiv.style.marginLeft = "0";if (this._borderColor) {
bottomDiv.style.borderLeftWidth = "1px";}
}
if (!this.isCornerSet(AjaxControlToolkit.BoxCorners.BottomRight)) {
bottomDiv.style.marginRight = "0";if (this._borderColor) {
bottomDiv.style.borderRightWidth = "1px";bottomDiv.style.marginRight = "-2px";}
}
}
e.insertBefore(newParent, lastDiv);this._parentDiv = newParent;this._isDirty = false;},
disposeParentDiv: function() {
if (this._parentDiv) {
var e = this.get_element();var children = e.childNodes;for (var i = children.length - 1;i >= 0;i--) {
var child = children[i];if (child) {
if (child == this._parentDiv) {
this.moveChildren(child, e);}
try {
e.removeChild(child);} catch (e) {
}
}
}
if (this._originalStyle) {
e.style.cssText = this._originalStyle;this._originalStyle = null;}
this._parentDiv = null;}
},
getBackgroundColor: function() {
if (this._color) {
return this._color;}
return $common.getCurrentStyle(this.get_element(), 'backgroundColor');},
moveChildren: function(src, dest) {
var moveCount = 0;while (src.hasChildNodes()) {
var child = src.childNodes[0];child = src.removeChild(child);dest.appendChild(child);moveCount++;}
return moveCount;},
isCornerSet: function(corner) {
return (this._corners & corner) != AjaxControlToolkit.BoxCorners.None;},
setCorner: function(corner, value) {
if (value) {
this.set_Corners(this._corners | corner);} else {
this.set_Corners(this._corners & ~corner);}
},
get_Color: function() {
return this._color;},
set_Color: function(value) {
if (value != this._color) {
this._color = value;this._isDirty = true;this.update();this.raisePropertyChanged('Color');}
},
get_Radius: function() {
return this._radius;},
set_Radius: function(value) {
if (value != this._radius) {
this._radius = value;this._isDirty = true;this.update();this.raisePropertyChanged('Radius');}
},
get_Corners: function() {
return this._corners;},
set_Corners: function(value) {
if (value != this._corners) {
this._corners = value;this._isDirty = true;this.update();this.raisePropertyChanged("Corners");}
},
get_BorderColor: function() {
return this._borderColor;},
set_BorderColor: function(value) {
if (value != this._borderColor) {
this._borderColor = value;this._isDirty = true;this.update();this.raisePropertyChanged("BorderColor");}
}
}
AjaxControlToolkit.RoundedCornersBehavior.registerClass('AjaxControlToolkit.RoundedCornersBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
