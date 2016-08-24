Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.RatingBehavior = function(element) {
AjaxControlToolkit.RatingBehavior.initializeBase(this, [element]);this._starCssClass = null;this._filledStarCssClass = null;this._emptyStarCssClass = null;this._waitingStarCssClass = null;this._readOnly = false;this._ratingValue = 0;this._currentRating = 0;this._maxRatingValue = 5;this._tag = "";this._ratingDirection = 0;this._stars = null;this._callbackID = null;this._mouseOutHandler = Function.createDelegate(this, this._onMouseOut);this._starClickHandler = Function.createDelegate(this, this._onStarClick);this._starMouseOverHandler = Function.createDelegate(this, this._onStarMouseOver);this._keyDownHandler = Function.createDelegate(this, this._onKeyDownBack);this._autoPostBack = false;}
AjaxControlToolkit.RatingBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.RatingBehavior.callBaseMethod(this, 'initialize');var elt = this.get_element();this._stars = [];for (var i = 1;i <= this._maxRatingValue;i++) {
starElement = $get(elt.id + '_Star_' + i);starElement.value = i;Array.add(this._stars, starElement);$addHandler(starElement, 'click', this._starClickHandler);$addHandler(starElement, 'mouseover', this._starMouseOverHandler);}
$addHandler(elt, 'mouseout', this._mouseOutHandler);$addHandler(elt, "keydown", this._keyDownHandler);this._update();},
dispose : function() {
var elt = this.get_element();if (this._stars) {
for (var i = 0;i < this._stars.length;i++) {
var starElement = this._stars[i];$removeHandler(starElement, 'click', this._starClickHandler);$removeHandler(starElement, 'mouseover', this._starMouseOverHandler);}
this._stars = null;}
$removeHandler(elt, 'mouseout', this._mouseOutHandler);$removeHandler(elt, "keydown", this._keyDownHandler);AjaxControlToolkit.RatingBehavior.callBaseMethod(this, 'dispose');},
_onError : function(message, context) {
alert(String.format(AjaxControlToolkit.Resources.Rating_CallbackError, message));},
_receiveServerData : function(arg, context) {
context._waitingMode(false);context.raiseEndClientCallback(arg);},
_onMouseOut : function(e) {
if (this._readOnly) {
return;}
this._currentRating = this._ratingValue;this._update();this.raiseMouseOut(this._currentRating);},
_onStarClick : function(e) {
if (this._readOnly) {
return;}
if (this._ratingValue != this._currentRating) {
this.set_Rating(this._currentRating);}
},
_onStarMouseOver : function(e) {
if (this._readOnly) {
return;}
if (this._ratingDirection == 0) {
this._currentRating = e.target.value;} else {
this._currentRating = this._maxRatingValue + 1 - e.target.value;}
this._update();this.raiseMouseOver(this._currentRating);},
_onKeyDownBack : function(ev){
if (this._readOnly) {
return;}
var k = ev.keyCode ? ev.keyCode : ev.rawEvent.keyCode;if ( (k == Sys.UI.Key.right) || (k == Sys.UI.Key.up) ) {
this._currentRating = Math.min(this._currentRating + 1, this._maxRatingValue);this.set_Rating(this._currentRating);ev.preventDefault();ev.stopPropagation();} else if ( (k == Sys.UI.Key.left) || (k == Sys.UI.Key.down) ) {
this._currentRating = Math.max(this._currentRating - 1, 1);this.set_Rating(this._currentRating);ev.preventDefault();ev.stopPropagation();}
},
_waitingMode : function(activated) {
for (var i = 0;i < this._maxRatingValue;i++) {
var starElement;if (this._ratingDirection == 0) {
starElement = this._stars[i];} else {
starElement = this._stars[this._maxRatingValue - i - 1];}
if (this._currentRating > i) {
if (activated)
{
Sys.UI.DomElement.removeCssClass(starElement, this._filledStarCssClass);Sys.UI.DomElement.addCssClass(starElement, this._waitingStarCssClass);} else {
Sys.UI.DomElement.removeCssClass(starElement, this._waitingStarCssClass);Sys.UI.DomElement.addCssClass(starElement, this._filledStarCssClass);}
} else {
Sys.UI.DomElement.removeCssClass(starElement, this._waitingStarCssClass);Sys.UI.DomElement.removeCssClass(starElement, this._filledStarCssClass);Sys.UI.DomElement.addCssClass(starElement, this._emptyStarCssClass);}
}
},
_update : function() {
var elt = this.get_element();$get(elt.id + "_A").title = this._currentRating;for (var i = 0;i < this._maxRatingValue;i++) {
var starElement;if (this._ratingDirection == 0) {
starElement = this._stars[i];} else {
starElement = this._stars[this._maxRatingValue - i - 1];}
if (this._currentRating > i) {
Sys.UI.DomElement.removeCssClass(starElement, this._emptyStarCssClass);Sys.UI.DomElement.addCssClass(starElement, this._filledStarCssClass);}
else {
Sys.UI.DomElement.removeCssClass(starElement, this._filledStarCssClass);Sys.UI.DomElement.addCssClass(starElement, this._emptyStarCssClass);}
}
},
add_Rated : function(handler) {
this.get_events().addHandler("Rated", handler);},
remove_Rated : function(handler) {
this.get_events().removeHandler("Rated", handler);},
raiseRated : function(rating) {
var handler = this.get_events().getHandler("Rated");if (handler) {
handler(this, new AjaxControlToolkit.RatingEventArgs(rating));}
},
add_MouseOver : function(handler) {
this.get_events().addHandler("MouseOver", handler);},
remove_MouseOver : function(handler) {
this.get_events().removeHandler("MouseOver", handler);},
raiseMouseOver : function(rating_tmp) {
var handler = this.get_events().getHandler("MouseOver");if (handler) {
handler(this, new AjaxControlToolkit.RatingEventArgs(rating_tmp));}
},
add_MouseOut : function(handler) {
this.get_events().addHandler("MouseOut", handler);},
remove_MouseOut : function(handler) {
this.get_events().removeHandler("MouseOut", handler);},
raiseMouseOut : function(rating_old) {
var handler = this.get_events().getHandler("MouseOut");if (handler) {
handler(this, new AjaxControlToolkit.RatingEventArgs(rating_old));}
},
add_EndClientCallback : function(handler) {
this.get_events().addHandler("EndClientCallback", handler);},
remove_EndClientCallback : function(handler) {
this.get_events().removeHandler("EndClientCallback", handler);},
raiseEndClientCallback : function(result) {
var handler = this.get_events().getHandler("EndClientCallback");if (handler) {
handler(this, new AjaxControlToolkit.RatingCallbackResultEventArgs(result));}
},
get_AutoPostBack : function() {
return this._autoPostBack;},
set_AutoPostBack : function(value) {
this._autoPostBack = value;},
get_Stars : function() {
return this._stars;},
get_Tag : function() {
return this._tag;},
set_Tag : function(value) {
if (this._tag != value) {
this._tag = value;this.raisePropertyChanged('Tag');}
},
get_CallbackID : function() {
return this._callbackID;},
set_CallbackID : function(value) {
this._callbackID = value;},
get_RatingDirection : function() {
return this._ratingDirection;},
set_RatingDirection : function(value) {
if (this._ratingDirection != value) {
this._ratingDirection = value;if (this.get_isInitialized()) {
this._update();}
this.raisePropertyChanged('RatingDirection');}
},
get_EmptyStarCssClass : function() {
return this._emptyStarCssClass;},
set_EmptyStarCssClass : function(value) {
if (this._emptyStarCssClass != value) {
this._emptyStarCssClass = value;this.raisePropertyChanged('EmptyStarCssClass');}
},
get_FilledStarCssClass : function() {
return this._filledStarCssClass;},
set_FilledStarCssClass : function(value) {
if (this._filledStarCssClass != value) {
this._filledStarCssClass = value;this.raisePropertyChanged('FilledStarCssClass');}
},
get_WaitingStarCssClass : function() {
return this._waitingStarCssClass;},
set_WaitingStarCssClass : function(value) {
if (this._waitingStarCssClass != value) {
this._waitingStarCssClass = value;this.raisePropertyChanged('WaitingStarCssClass');}
},
get_Rating : function() {
this._ratingValue = AjaxControlToolkit.RatingBehavior.callBaseMethod(this, 'get_ClientState');if (this._ratingValue == '') 
this._ratingValue = null;return this._ratingValue;},
set_Rating : function(value) {
if (this._ratingValue != value) {
this._ratingValue = value;this._currentRating = value;if (this.get_isInitialized()) {
if ((value < 0) || (value > this._maxRatingValue)) {
return;}
this._update();AjaxControlToolkit.RatingBehavior.callBaseMethod(this, 'set_ClientState', [ this._ratingValue ]);this.raisePropertyChanged('Rating');this.raiseRated(this._currentRating);this._waitingMode(true);var args = this._currentRating + ";" + this._tag;var id = this._callbackID;if (this._autoPostBack) { 
__doPostBack(id, args);}
else {
WebForm_DoCallback(id, args, this._receiveServerData, this, this._onError, true)
} 
}
}
},
get_MaxRating : function() {
return this._maxRatingValue;},
set_MaxRating : function(value) {
if (this._maxRatingValue != value) {
this._maxRatingValue = value;this.raisePropertyChanged('MaxRating');}
},
get_ReadOnly : function() {
return this._readOnly;},
set_ReadOnly : function(value) {
if (this._readOnly != value) {
this._readOnly = value;this.raisePropertyChanged('ReadOnly');}
},
get_StarCssClass : function() {
return this._starCssClass;},
set_StarCssClass : function(value) {
if (this._starCssClass != value) {
this._starCssClass = value;this.raisePropertyChanged('StarCssClass');}
}
}
AjaxControlToolkit.RatingBehavior.registerClass('AjaxControlToolkit.RatingBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.RatingEventArgs = function(rating) {
AjaxControlToolkit.RatingEventArgs.initializeBase(this);this._rating = rating;}
AjaxControlToolkit.RatingEventArgs.prototype = {
get_Rating : function() {
return this._rating;}
}
AjaxControlToolkit.RatingEventArgs.registerClass('AjaxControlToolkit.RatingEventArgs', Sys.EventArgs);AjaxControlToolkit.RatingCallbackResultEventArgs = function(result) {
AjaxControlToolkit.RatingCallbackResultEventArgs.initializeBase(this);this._result = result;}
AjaxControlToolkit.RatingCallbackResultEventArgs.prototype = {
get_CallbackResult : function() {
return this._result;}
}
AjaxControlToolkit.RatingCallbackResultEventArgs.registerClass('AjaxControlToolkit.RatingCallbackResultEventArgs', Sys.EventArgs);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
