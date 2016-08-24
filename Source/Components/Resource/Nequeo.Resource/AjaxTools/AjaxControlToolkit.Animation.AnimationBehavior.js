Type.registerNamespace('AjaxControlToolkit.Animation');AjaxControlToolkit.Animation.AnimationBehavior = function(element) {
AjaxControlToolkit.Animation.AnimationBehavior.initializeBase(this, [element]);this._onLoad = null;this._onClick = null;this._onMouseOver = null;this._onMouseOut = null;this._onHoverOver = null;this._onHoverOut = null;this._onClickHandler = null;this._onMouseOverHandler = null;this._onMouseOutHandler = null;}
AjaxControlToolkit.Animation.AnimationBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.Animation.AnimationBehavior.callBaseMethod(this, 'initialize');var element = this.get_element();if (element) {
this._onClickHandler = Function.createDelegate(this, this.OnClick);$addHandler(element, 'click', this._onClickHandler);this._onMouseOverHandler = Function.createDelegate(this, this.OnMouseOver);$addHandler(element, 'mouseover', this._onMouseOverHandler);this._onMouseOutHandler = Function.createDelegate(this, this.OnMouseOut);$addHandler(element, 'mouseout', this._onMouseOutHandler);}
},
dispose : function() {
var element = this.get_element();if (element) {
if (this._onClickHandler) {
$removeHandler(element, 'click', this._onClickHandler);this._onClickHandler = null;}
if (this._onMouseOverHandler) {
$removeHandler(element, 'mouseover', this._onMouseOverHandler);this._onMouseOverHandler = null;}
if (this._onMouseOutHandler) {
$removeHandler(element, 'mouseout', this._onMouseOutHandler);this._onMouseOutHandler = null;}
}
this._onLoad = null;this._onClick = null;this._onMouseOver = null;this._onMouseOut = null;this._onHoverOver = null;this._onHoverOut = null;AjaxControlToolkit.Animation.AnimationBehavior.callBaseMethod(this, 'dispose');},
get_OnLoad : function() {
return this._onLoad ? this._onLoad.get_json() : null;},
set_OnLoad : function(value) {
if (!this._onLoad) {
this._onLoad = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onLoad.initialize();}
this._onLoad.set_json(value);this.raisePropertyChanged('OnLoad');this._onLoad.play();},
get_OnLoadBehavior : function() {
return this._onLoad;},
get_OnClick : function() {
return this._onClick ? this._onClick.get_json() : null;},
set_OnClick : function(value) {
if (!this._onClick) {
this._onClick = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onClick.initialize();}
this._onClick.set_json(value);this.raisePropertyChanged('OnClick');},
get_OnClickBehavior : function() {
return this._onClick;},
OnClick : function() {
if (this._onClick) {
this._onClick.play();}
},
get_OnMouseOver : function() {
return this._onMouseOver ? this._onMouseOver.get_json() : null;},
set_OnMouseOver : function(value) {
if (!this._onMouseOver) {
this._onMouseOver = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onMouseOver.initialize();}
this._onMouseOver.set_json(value);this.raisePropertyChanged('OnMouseOver');},
get_OnMouseOverBehavior : function() {
return this._onMouseOver;},
OnMouseOver : function() {
if (this._onMouseOver) {
this._onMouseOver.play();}
if (this._onHoverOver) {
if (this._onHoverOut) {
this._onHoverOut.quit();}
this._onHoverOver.play();}
},
get_OnMouseOut : function() {
return this._onMouseOut ? this._onMouseOut.get_json() : null;},
set_OnMouseOut : function(value) {
if (!this._onMouseOut) {
this._onMouseOut = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onMouseOut.initialize();}
this._onMouseOut.set_json(value);this.raisePropertyChanged('OnMouseOut');},
get_OnMouseOutBehavior : function() {
return this._onMouseOut;},
OnMouseOut : function() {
if (this._onMouseOut) {
this._onMouseOut.play();}
if (this._onHoverOut) {
if (this._onHoverOver) {
this._onHoverOver.quit();}
this._onHoverOut.play();}
},
get_OnHoverOver : function() {
return this._onHoverOver ? this._onHoverOver.get_json() : null;},
set_OnHoverOver : function(value) {
if (!this._onHoverOver) {
this._onHoverOver = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onHoverOver.initialize();}
this._onHoverOver.set_json(value);this.raisePropertyChanged('OnHoverOver');},
get_OnHoverOverBehavior : function() {
return this._onHoverOver;},
get_OnHoverOut : function() {
return this._onHoverOut ? this._onHoverOut.get_json() : null;},
set_OnHoverOut : function(value) {
if (!this._onHoverOut) {
this._onHoverOut = new AjaxControlToolkit.Animation.GenericAnimationBehavior(this.get_element());this._onHoverOut.initialize();}
this._onHoverOut.set_json(value);this.raisePropertyChanged('OnHoverOut');},
get_OnHoverOutBehavior : function() {
return this._onHoverOut;}
}
AjaxControlToolkit.Animation.AnimationBehavior.registerClass('AjaxControlToolkit.Animation.AnimationBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.Animation.GenericAnimationBehavior = function(element) {
AjaxControlToolkit.Animation.GenericAnimationBehavior.initializeBase(this, [element]);this._json = null;this._animation = null;}
AjaxControlToolkit.Animation.GenericAnimationBehavior.prototype = {
dispose : function() {
this.disposeAnimation();AjaxControlToolkit.Animation.GenericAnimationBehavior.callBaseMethod(this, 'dispose');},
disposeAnimation : function() {
if (this._animation) {
this._animation.dispose();}
this._animation = null;},
play : function() {
if (this._animation && !this._animation.get_isPlaying()) {
this.stop();this._animation.play();}
},
stop : function() {
if (this._animation) {
if (this._animation.get_isPlaying()) {
this._animation.stop(true);}
}
},
quit : function() {
if (this._animation) {
if (this._animation.get_isPlaying()) {
this._animation.stop(false);}
}
},
get_json : function() {
return this._json;},
set_json : function(value) {
if (this._json != value) {
this._json = value;this.raisePropertyChanged('json');this.disposeAnimation();var element = this.get_element();if (element) {
this._animation = AjaxControlToolkit.Animation.buildAnimation(this._json, element);if (this._animation) {
this._animation.initialize();}
this.raisePropertyChanged('animation');}
}
},
get_animation : function() {
return this._animation;}
}
AjaxControlToolkit.Animation.GenericAnimationBehavior.registerClass('AjaxControlToolkit.Animation.GenericAnimationBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
