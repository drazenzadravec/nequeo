Type.registerNamespace('AjaxControlToolkit.Animation');AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior = function(element) {
AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior.initializeBase(this, [element]);this._onUpdating = new AjaxControlToolkit.Animation.GenericAnimationBehavior(element);this._onUpdated = new AjaxControlToolkit.Animation.GenericAnimationBehavior(element);this._postBackPending = null;this._pageLoadedHandler = null;}
AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior.prototype = { 
initialize : function() {
AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior.callBaseMethod(this, 'initialize');var element = this.get_element();var parentDiv = document.createElement(element.tagName);element.parentNode.insertBefore(parentDiv, element);parentDiv.appendChild(element);Array.remove(element._behaviors, this);Array.remove(element._behaviors, this._onUpdating);Array.remove(element._behaviors, this._onUpdated);if (parentDiv._behaviors) {
Array.add(parentDiv._behaviors, this);Array.add(parentDiv._behaviors, this._onUpdating);Array.add(parentDiv._behaviors, this._onUpdated);} else {
parentDiv._behaviors = [this, this._onUpdating, this._onUpdated];}
this._element = this._onUpdating._element = this._onUpdated._element = parentDiv;this._onUpdating.initialize();this._onUpdated.initialize();this.registerPartialUpdateEvents();this._pageLoadedHandler = Function.createDelegate(this, this._pageLoaded);this._pageRequestManager.add_pageLoaded(this._pageLoadedHandler);},
dispose : function() {
if (this._pageRequestManager && this._pageLoadedHandler) {
this._pageRequestManager.remove_pageLoaded(this._pageLoadedHandler);this._pageLoadedHandler = null;}
AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior.callBaseMethod(this, 'dispose');},
_partialUpdateBeginRequest : function(sender, beginRequestEventArgs) {
AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior.callBaseMethod(this, '_partialUpdateBeginRequest', [sender, beginRequestEventArgs]);if (!this._postBackPending) {
this._postBackPending = true;this._onUpdated.quit();this._onUpdating.play();}
},
_pageLoaded : function(sender, args) {
if (this._postBackPending) {
this._postBackPending = false;var element = this.get_element();var panels = args.get_panelsUpdated();for (var i = 0;i < panels.length;i++) {
if (panels[i].parentNode == element) {
this._onUpdating.quit();this._onUpdated.play();break;}
}
}
},
get_OnUpdating : function() {
return this._onUpdating.get_json();},
set_OnUpdating : function(value) {
this._onUpdating.set_json(value);this.raisePropertyChanged('OnUpdating');},
get_OnUpdatingBehavior : function() {
return this._onUpdating;},
get_OnUpdated : function() {
return this._onUpdated.get_json();},
set_OnUpdated : function(value) {
this._onUpdated.set_json(value);this.raisePropertyChanged('OnUpdated');},
get_OnUpdatedBehavior : function() {
return this._onUpdated;}
}
AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior.registerClass('AjaxControlToolkit.Animation.UpdatePanelAnimationBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
