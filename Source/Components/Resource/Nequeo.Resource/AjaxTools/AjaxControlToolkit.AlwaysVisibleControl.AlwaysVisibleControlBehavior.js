Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.HorizontalSide = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.HorizontalSide.prototype = {
Left : 0,
Center : 1,
Right : 2
}
AjaxControlToolkit.HorizontalSide.registerEnum("AjaxControlToolkit.HorizontalSide", false);AjaxControlToolkit.VerticalSide = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.VerticalSide.prototype = {
Top : 0,
Middle : 1,
Bottom : 2
}
AjaxControlToolkit.VerticalSide.registerEnum("AjaxControlToolkit.VerticalSide", false);AjaxControlToolkit.AlwaysVisibleControlBehavior = function(element) {
AjaxControlToolkit.AlwaysVisibleControlBehavior.initializeBase(this, [element]);this._horizontalOffset = 0;this._horizontalSide = AjaxControlToolkit.HorizontalSide.Left;this._verticalOffset = 0;this._verticalSide = AjaxControlToolkit.VerticalSide.Top;this._scrollEffectDuration = .1;this._repositionHandler = null;this._animate = false;this._animation = null;}
AjaxControlToolkit.AlwaysVisibleControlBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.AlwaysVisibleControlBehavior.callBaseMethod(this, 'initialize');var element = this.get_element();if (!element) throw Error.invalidOperation(AjaxControlToolkit.Resources.AlwaysVisible_ElementRequired);this._repositionHandler = Function.createDelegate(this, this._reposition);if (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7) {
this._animate = true;}
if (this._animate) {
this._animation = new AjaxControlToolkit.Animation.MoveAnimation(
element, this._scrollEffectDuration, 25, 0, 0, false, 'px');element.style.position = 'absolute';} else {
element.style.position = 'fixed';}
$addHandler(window, 'resize', this._repositionHandler);if (this._animate) {
$addHandler(window, 'scroll', this._repositionHandler);}
this._reposition();},
dispose : function() {
if (this._repositionHandler) {
if (this._animate) {
$removeHandler(window, 'scroll', this._repositionHandler);}
$removeHandler(window, 'resize', this._repositionHandler);this._repositionHandler = null;}
if (this._animation) {
this._animation.dispose();this._animation = null;}
AjaxControlToolkit.AlwaysVisibleControlBehavior.callBaseMethod(this, 'dispose');},
_reposition : function(eventObject) {
var element = this.get_element();if (!element) return;this.raiseRepositioning(Sys.EventArgs.Empty);var x = 0;var y = 0;if (this._animate) {
if (document.documentElement && document.documentElement.scrollTop) {
x = document.documentElement.scrollLeft;y = document.documentElement.scrollTop;} else {
x = document.body.scrollLeft;y = document.body.scrollTop;}
}
var clientBounds = $common.getClientBounds();var width = clientBounds.width;var height = clientBounds.height;switch (this._horizontalSide) {
case AjaxControlToolkit.HorizontalSide.Center :
x = Math.max(0, Math.floor(x + width / 2.0 - element.offsetWidth / 2.0 - this._horizontalOffset));break;case AjaxControlToolkit.HorizontalSide.Right :
x = Math.max(0, x + width - element.offsetWidth - this._horizontalOffset);break;case AjaxControlToolkit.HorizontalSide.Left :
default :
x += this._horizontalOffset;break;} 
switch (this._verticalSide) {
case AjaxControlToolkit.VerticalSide.Middle :
y = Math.max(0, Math.floor(y + height / 2.0 - element.offsetHeight / 2.0 - this._verticalOffset));break;case AjaxControlToolkit.VerticalSide.Bottom :
y = Math.max(0, y + height - element.offsetHeight - this._verticalOffset);break;case AjaxControlToolkit.VerticalSide.Top :
default :
y += this._verticalOffset;break;}
if (this._animate && this._animation) {
this._animation.stop();this._animation.set_horizontal(x);this._animation.set_vertical(y);this._animation.play();} else {
element.style.left = x + 'px';element.style.top = y + 'px';}
this.raiseRepositioned(Sys.EventArgs.Empty);},
get_HorizontalOffset : function() {
return this._horizontalOffset;},
set_HorizontalOffset : function(value) {
if (this._horizontalOffset != value) {
this._horizontalOffset = value;this._reposition();this.raisePropertyChanged('HorizontalOffset');}
},
get_HorizontalSide : function() {
return this._horizontalSide;},
set_HorizontalSide : function(value) {
if (this._horizontalSide != value) {
this._horizontalSide = value;this._reposition();this.raisePropertyChanged('HorizontalSide');}
},
get_VerticalOffset : function() {
return this._verticalOffset;},
set_VerticalOffset : function(value) {
if (this._verticalOffset != value) {
this._verticalOffset = value;this._reposition();this.raisePropertyChanged('VerticalOffset');}
},
get_VerticalSide : function() {
return this._verticalSide;},
set_VerticalSide : function(value) {
if (this._verticalSide != value) {
this._verticalSide = value;this._reposition();this.raisePropertyChanged('VerticalSide');}
},
get_ScrollEffectDuration : function() {
return this._scrollEffectDuration;},
set_ScrollEffectDuration : function(value) {
if (this._scrollEffectDuration != value) {
this._scrollEffectDuration = value;if (this._animation) {
this._animation.set_duration(value);}
this.raisePropertyChanged('ScrollEffectDuration');}
},
get_useAnimation : function() {
return this._animate;},
set_useAnimation : function(value) {
value |= (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version < 7);if (this._animate != value) {
this._animate = value;this.raisePropertyChanged('useAnimation');}
},
add_repositioning : function(handler) {
this.get_events().addHandler('repositioning', handler);},
remove_repositioning : function(handler) {
this.get_events().removeHandler('repositioning', handler);},
raiseRepositioning : function(eventArgs) {
var handler = this.get_events().getHandler('repositioning');if (handler) {
handler(this, eventArgs);}
},
add_repositioned : function(handler) {
this.get_events().addHandler('repositioned', handler);},
remove_repositioned : function(handler) {
this.get_events().removeHandler('repositioned', handler);},
raiseRepositioned : function(eventArgs) {
var handler = this.get_events().getHandler('repositioned');if (handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit.AlwaysVisibleControlBehavior.registerClass('AjaxControlToolkit.AlwaysVisibleControlBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
