Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.ToggleButtonBehavior = function(element) {
AjaxControlToolkit.ToggleButtonBehavior.initializeBase(this, [element]);this._idDecoration = '_ToggleButton';this._ImageWidth = null;this._ImageHeight = null;this._UncheckedImageUrl = null;this._CheckedImageUrl = null;this._DisabledUncheckedImageUrl = null;this._DisabledCheckedImageUrl = null;this._CheckedImageOverUrl = null;this._UncheckedImageOverUrl = null;this._UncheckedImageAlternateText = null;this._CheckedImageAlternateText = null;this._CheckedImageOverAlternateText = null;this._UncheckedImageOverAlternateText = null;this._decoyElement = null;this._decoyElementClickHandler = null;this._checkChangedHandler = null;this._divContent = null;this._clickHandler = null;this._decoyElementMouseOverHandler = null;this._decoyElementMouseOutHandler = null;}
AjaxControlToolkit.ToggleButtonBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.ToggleButtonBehavior.callBaseMethod(this, 'initialize');var e = this.get_element();this._divContent = document.createElement('div');this._divContent.style.position = 'relative';this._decoyElement = document.createElement('a');e.parentNode.insertBefore(this._divContent, e);this._decoyElement.id = e.id + this._idDecoration;this._decoyElement.href = '';this._divContent.appendChild(this._decoyElement);e.style.visibility = 'hidden';var decoyElementStyle = this._decoyElement.style;decoyElementStyle.position = 'absolute';decoyElementStyle.left = '0px';decoyElementStyle.top = '0px';decoyElementStyle.width = this._ImageWidth + 'px';decoyElementStyle.height = this._ImageHeight + 'px';decoyElementStyle.fontSize = this._ImageHeight + 'px';decoyElementStyle.backgroundRepeat = 'no-repeat';this._onClick();this._clickHandler = Function.createDelegate(this, this._onClick);this._checkChangedHandler = Function.createDelegate(this, this._onClick);this._decoyElementClickHandler = Function.createDelegate(this, this._onDecoyElementClick);this._decoyElementMouseOverHandler = Function.createDelegate(this, this._onDecoyElementMouseOver);this._decoyElementMouseOutHandler = Function.createDelegate(this, this._onDecoyElementMouseOut);$addHandler(e, "click", this._clickHandler);$addHandler(e, "change", this._checkChangedHandler);$addHandler(this._decoyElement, "click", this._decoyElementClickHandler);$addHandler(this._decoyElement, "mouseover", this._decoyElementMouseOverHandler);$addHandler(this._decoyElement, "mouseout", this._decoyElementMouseOutHandler);if (Sys.Browser.agent === Sys.Browser.InternetExplorer) {
var labels = this._divContent.parentNode.getElementsByTagName('label');for (i = 0 ;i < labels.length ;i++) {
if (e.id == labels[i].htmlFor) {
labels[i].htmlFor = e.id + this._idDecoration;}
}
}
},
dispose : function() {
if (this._decoyElementClickHandler) {
$removeHandler(this._decoyElement, "click", this._decoyElementClickHandler);this._decoyElementClickHandler = null;}
if(this._checkChangedHandler) {
$removeHandler(this.get_element(), "change", this._checkChangedHandler);this._checkChangedHandler = null;}
if (this._clickHandler) {
$removeHandler(this.get_element(), "click", this._clickHandler);this._clickHandler = null;}
if(this._decoyElementMouseOverHandler) {
$removeHandler(this._decoyElement, "mouseover", this._decoyElementMouseOverHandler);this._decoyElementMouseOverHandler = null;}
if(this._decoyElementMouseOutHandler) {
$removeHandler(this._decoyElement, "mouseout", this._decoyElementMouseOutHandler);this._decoyElementMouseOutHandler = null;} 
AjaxControlToolkit.ToggleButtonBehavior.callBaseMethod(this, 'dispose');},
_onClick : function() {
if(this.get_element().checked) {
this._decoyElement.style.backgroundImage = 'url(' + (this.get_element().disabled ? this.get_DisabledCheckedImageUrl() : this._CheckedImageUrl) + ')';if (this._CheckedImageAlternateText) {
this._decoyElement.title = this._CheckedImageAlternateText;}
} else {
this._decoyElement.style.backgroundImage = 'url(' + (this.get_element().disabled ? this.get_DisabledUncheckedImageUrl() : this._UncheckedImageUrl) + ')';if (this._UncheckedImageAlternateText) {
this._decoyElement.title = this._UncheckedImageAlternateText;}
}
},
_onDecoyElementClick : function(e) {
this.get_element().click();e.preventDefault();return false;},
_onDecoyElementMouseOver : function(e) {
var e = this.get_element();if(e && !e.disabled) {
if (e.checked && this._CheckedImageOverUrl) {
this._decoyElement.style.backgroundImage = 'url(' + this._CheckedImageOverUrl + ')';if (this._CheckedImageOverAlternateText) {
this._decoyElement.title = this._CheckedImageOverAlternateText;} 
} else if (!e.checked && this._UncheckedImageOverUrl) {
this._decoyElement.style.backgroundImage = 'url(' + this._UncheckedImageOverUrl + ')';if (this._UncheckedImageOverAlternateText) {
this._decoyElement.title = this._UncheckedImageOverAlternateText;} 
}
} 
},
_onDecoyElementMouseOut : function(e) {
this._onClick();}, 
get_ImageWidth : function() {
return this._ImageWidth;},
set_ImageWidth : function(value) {
if (this._ImageWidth != value) {
this._ImageWidth = value;this.raisePropertyChanged('ImageWidth');}
},
get_ImageHeight : function() {
return this._ImageHeight;},
set_ImageHeight : function(value) {
if (this._ImageHeight != value) {
this._ImageHeight = value;this.raisePropertyChanged('ImageHeight');}
},
get_UncheckedImageUrl : function() {
return this._UncheckedImageUrl;},
set_UncheckedImageUrl : function(value) {
if (this._UncheckedImageUrl != value) {
this._UncheckedImageUrl = value;this.raisePropertyChanged('UncheckedImageUrl');}
},
get_CheckedImageUrl : function() {
return this._CheckedImageUrl;},
set_CheckedImageUrl : function(value) {
if (this._CheckedImageUrl != value) {
this._CheckedImageUrl = value;this.raisePropertyChanged('CheckedImageUrl');}
},
get_DisabledUncheckedImageUrl : function() {
return (this._DisabledUncheckedImageUrl != undefined) ?
this._DisabledUncheckedImageUrl : this._UncheckedImageUrl;},
set_DisabledUncheckedImageUrl : function(value) {
if (this._DisabledUncheckedImageUrl != value) {
this._DisabledUncheckedImageUrl = value;this.raisePropertyChanged('DisabledUncheckedImageUrl');}
},
get_DisabledCheckedImageUrl : function() {
return (this._DisabledUncheckedImageUrl != undefined) ?
this._DisabledCheckedImageUrl : this._CheckedImageUrl;},
set_DisabledCheckedImageUrl : function(value) {
if (this._DisabledCheckedImageUrl != value) {
this._DisabledCheckedImageUrl = value;this.raisePropertyChanged('DisabledCheckedImageUrl');}
},
get_CheckedImageOverUrl : function() {
return this._CheckedImageOverUrl;},
set_CheckedImageOverUrl : function(value) {
if (this._CheckedImageOverUrl != value) {
this._CheckedImageOverUrl = value;this.raisePropertyChanged('CheckedImageOverUrl');}
}, 
get_UncheckedImageOverUrl : function() {
return this._UncheckedImageOverUrl;},
set_UncheckedImageOverUrl : function(value) {
if (this._UncheckedImageOverUrl != value) {
this._UncheckedImageOverUrl = value;this.raisePropertyChanged('UncheckedImageOverUrl');}
}, 
get_UncheckedImageAlternateText : function() {
return this._UncheckedImageAlternateText;},
set_UncheckedImageAlternateText : function(value) {
if (this._UncheckedImageAlternateText != value) {
this._UncheckedImageAlternateText = value;this.raisePropertyChanged('UncheckedImageAlternateText');}
},
get_CheckedImageAlternateText : function() {
return this._CheckedImageAlternateText;},
set_CheckedImageAlternateText : function(value) {
if (this._CheckedImageAlternateText != value) {
this._CheckedImageAlternateText = value;this.raisePropertyChanged('CheckedImageAlternateText');}
},
get_CheckedImageOverAlternateText : function() {
return this._CheckedImageOverAlternateText;},
set_CheckedImageOverAlternateText : function(value) {
if (this._CheckedImageOverAlternateText != value) {
this._CheckedImageOverAlternateText = value;this.raisePropertyChanged('CheckedImageOverAlternateText');}
},
get_UncheckedImageOverAlternateText : function() {
return this._UncheckedImageOverAlternateText;},
set_UncheckedImageOverAlternateText : function(value) {
if (this._UncheckedImageOverAlternateText != value) {
this._UncheckedImageOverAlternateText = value;this.raisePropertyChanged('UncheckedImageOverAlternateText');}
} 
}
AjaxControlToolkit.ToggleButtonBehavior.registerClass('AjaxControlToolkit.ToggleButtonBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
