Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.DropShadowBehavior = function(element) {
AjaxControlToolkit.DropShadowBehavior.initializeBase(this, [element]);this._opacity = 1.0;this._width = 5;this._shadowDiv = null;this._trackPosition = null;this._trackPositionDelay = 50;this._timer = null;this._tickHandler = null;this._roundedBehavior = null;this._shadowRoundedBehavior = null;this._rounded = false;this._radius = 5;this._lastX = null;this._lastY = null;this._lastW = null;this._lastH = null;}
AjaxControlToolkit.DropShadowBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.DropShadowBehavior.callBaseMethod(this, 'initialize');var e = this.get_element();if ($common.getCurrentStyle(e, 'position', e.style.position) != "absolute") {
e.style.position = "relative";}
if (this._rounded) {
this.setupRounded();}
if (this._trackPosition) {
this.startTimer();}
this.setShadow();},
dispose : function() {
this.stopTimer();this.disposeShadowDiv();AjaxControlToolkit.DropShadowBehavior.callBaseMethod(this, 'dispose');},
buildShadowDiv : function() {
var e = this.get_element();if (!this.get_isInitialized() || !e || !this._width) return;var div = document.createElement("DIV");div.style.backgroundColor = "black";div.style.position= "absolute";if (e.id) {
div.id = e.id + "_DropShadow";}
this._shadowDiv = div;e.parentNode.appendChild(div);if (this._rounded ) {
this._shadowDiv.style.height = Math.max(0, e.offsetHeight - (2*this._radius)) + "px";if (!this._shadowRoundedBehavior) {
this._shadowRoundedBehavior = $create(AjaxControlToolkit.RoundedCornersBehavior, {"Radius": this._radius}, null, null, this._shadowDiv);} else {
this._shadowRoundedBehavior.set_Radius(this._radius);}
} else if (this._shadowRoundedBehavior) {
this._shadowRoundedBehavior.set_Radius(0);}
if (this._opacity != 1.0) {
this.setupOpacity();}
this.setShadow(false, true);this.updateZIndex();},
disposeShadowDiv : function() {
if (this._shadowDiv) {
if (this._shadowDiv.parentNode) {
this._shadowDiv.parentNode.removeChild(this._shadowDiv);} 
this._shadowDiv = null;}
if (this._shadowRoundedBehavior) {
this._shadowRoundedBehavior.dispose();this._shadowRoundedBehavior = null;}
},
onTimerTick : function() {
this.setShadow();},
startTimer : function() {
if (!this._timer) {
if (!this._tickHandler) {
this._tickHandler = Function.createDelegate(this, this.onTimerTick);}
this._timer = new Sys.Timer();this._timer.set_interval(this._trackPositionDelay);this._timer.add_tick(this._tickHandler);this._timer.set_enabled(true);}
},
stopTimer : function() {
if (this._timer) {
this._timer.remove_tick(this._tickHandler);this._timer.set_enabled(false);this._timer.dispose();this._timer = null;}
},
setShadow : function(force, norecurse) {
var e = this.get_element();if (!this.get_isInitialized() || !e || (!this._width && !force)) return;var existingShadow = this._shadowDiv;if (!existingShadow) {
this.buildShadowDiv();}
var location = $common.getLocation(e);if (force || this._lastX != location.x || this._lastY != location.y || !existingShadow) {
this._lastX = location.x;this._lastY = location.y;var w = this.get_Width();if((e.parentNode.style.position == "absolute") || (e.parentNode.style.position == "fixed") )
{
location.x = w;location.y = w;}
else if (e.parentNode.style.position == "relative")
{
location.x = w;var paddingTop = e.parentNode.style.paddingTop;paddingTop = paddingTop.replace("px", "");var intPaddingTop = 0;intPaddingTop = parseInt(paddingTop);location.y = w + intPaddingTop;}
else
{
location.x += w;location.y += w;}
$common.setLocation(this._shadowDiv, location);}
var h = e.offsetHeight;var w = e.offsetWidth;if (force || h != this._lastH || w != this._lastW || !existingShadow) {
this._lastW = w;this._lastH = h;if (!this._rounded || !existingShadow || norecurse) {
this._shadowDiv.style.width = w + "px";this._shadowDiv.style.height = h + "px";} else {
this.disposeShadowDiv();this.setShadow();}
}
if (this._shadowDiv) {
this._shadowDiv.style.visibility = $common.getCurrentStyle(e, 'visibility');}
},
setupOpacity : function() {
if (this.get_isInitialized() && this._shadowDiv) {
$common.setElementOpacity(this._shadowDiv, this._opacity);}
},
setupRounded : function() {
if (!this._roundedBehavior && this._rounded) {
this._roundedBehavior = $create(AjaxControlToolkit.RoundedCornersBehavior, null, null, null, this.get_element());}
if (this._roundedBehavior) {
this._roundedBehavior.set_Radius(this._rounded ? this._radius : 0);}
},
updateZIndex : function() {
if (!this._shadowDiv) return;var e = this.get_element();var targetZIndex = e.style.zIndex;var shadowZIndex = this._shadowDiv.style.zIndex;if (shadowZIndex && targetZIndex && targetZIndex > shadowZIndex) {
return;} else {
targetZIndex = Math.max(2, targetZIndex);shadowZIndex = targetZIndex - 1;}
e.style.zIndex = targetZIndex;this._shadowDiv.style.zIndex = shadowZIndex;},
updateRoundedCorners : function() {
if (this.get_isInitialized()) {
this.setupRounded();this.disposeShadowDiv();this.setShadow();}
},
get_Opacity : function() {
return this._opacity;},
set_Opacity : function(value) {
if (this._opacity != value) {
this._opacity = value;this.setupOpacity();this.raisePropertyChanged('Opacity');}
},
get_Rounded : function() {
return this._rounded;},
set_Rounded : function(value) {
if (value != this._rounded) {
this._rounded = value;this.updateRoundedCorners();this.raisePropertyChanged('Rounded');}
},
get_Radius : function() {
return this._radius;},
set_Radius : function(value) {
if (value != this._radius) {
this._radius = value;this.updateRoundedCorners();this.raisePropertyChanged('Radius');}
},
get_Width : function() {
return this._width;},
set_Width : function(value) {
if (value != this._width) {
this._width = value;if (this._shadowDiv) {
$common.setVisible(this._shadowDiv, value > 0);}
this.setShadow(true);this.raisePropertyChanged('Width');}
},
get_TrackPositionDelay : function() {
return this._trackPositionDelay;},
set_TrackPositionDelay : function(value) {
if (value != this._trackPositionDelay) {
this._trackPositionDelay = value;if (this._trackPosition) {
this.stopTimer();this.startTimer();}
this.raisePropertyChanged('TrackPositionDelay');}
},
get_TrackPosition : function() {
return this._trackPosition;},
set_TrackPosition : function(value) {
if (value != this._trackPosition) {
this._trackPosition = value;if (this.get_element()) {
if (value) {
this.startTimer();} else {
this.stopTimer();}
}
this.raisePropertyChanged('TrackPosition');}
}
}
AjaxControlToolkit.DropShadowBehavior.registerClass('AjaxControlToolkit.DropShadowBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
