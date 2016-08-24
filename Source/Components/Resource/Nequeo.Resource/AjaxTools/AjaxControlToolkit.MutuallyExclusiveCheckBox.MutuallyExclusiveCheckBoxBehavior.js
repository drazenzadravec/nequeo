Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior = function(element) {
AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.initializeBase(this, [element]);this._key = "";this._clickHandler = Function.createDelegate(this, this._onclick);}
AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.callBaseMethod(this, 'initialize');$addHandler(this.get_element(), "click", this._clickHandler);},
dispose : function() {
if (this._key) {
var keys = AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.Keys;var ar = keys[this._key];Array.remove(ar, this);this._key = null;}
if (this._clickHandler) {
$removeHandler(this.get_element(), "click", this._clickHandler);this._clickHandler = null;}
AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.callBaseMethod(this, 'dispose');},
get_Key : function() {
return this._key;},
set_Key : function(value) {
var keys = AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.Keys;if(value != this._key) {
if(this._key) {
var ar = keys[this._key];Array.remove(ar, this._key);}
this._key = value;if(value) {
var ar = keys[this._key];if(ar == null) {
ar = keys[this._key] = [];}
Array.add(ar, this);}
}
},
_onclick : function() {
var element = this.get_element();var keys = AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.Keys;if(this._key && element.checked) {
var ar = keys[this._key];var t = this;Array.forEach(ar, function(b) {
if(b != t) {
b.get_element().checked = false;$common.tryFireEvent(b.get_element(), "change");}
});}
this.raiseChecked(new AjaxControlToolkit.MutuallyExclusiveCheckBoxEventArgs(element, this._key));},
add_checked : function(handler) {
this.get_events().addHandler('checked', handler);},
remove_checked : function(handler) {
this.get_events().removeHandler('checked', handler);},
raiseChecked : function(eventArgs) {
var handler = this.get_events().getHandler('checked');if (handler) {
handler(this, eventArgs);}
}
} 
AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.registerClass('AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.MutuallyExclusiveCheckBoxBehavior.Keys = {};AjaxControlToolkit.MutuallyExclusiveCheckBoxEventArgs = function(checkbox, key) {
AjaxControlToolkit.MutuallyExclusiveCheckBoxEventArgs.initializeBase(this);this._key = key;this._checkbox = checkbox;}
AjaxControlToolkit.MutuallyExclusiveCheckBoxEventArgs.prototype = {
get_checkbox : function() {
return this._checkbox;},
get_key : function() {
return this._key;}
}
AjaxControlToolkit.MutuallyExclusiveCheckBoxEventArgs.registerClass('AjaxControlToolkit.MutuallyExclusiveCheckBoxEventArgs', Sys.EventArgs);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
