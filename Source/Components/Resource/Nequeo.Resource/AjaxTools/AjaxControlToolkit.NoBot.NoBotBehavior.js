Type.registerNamespace("AjaxControlToolkit");AjaxControlToolkit.NoBotBehavior = function(element) {
AjaxControlToolkit.NoBotBehavior.initializeBase(this, [element]);this._ChallengeScript = "";}
AjaxControlToolkit.NoBotBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.NoBotBehavior.callBaseMethod(this, "initialize");var response = eval(this._ChallengeScript);AjaxControlToolkit.NoBotBehavior.callBaseMethod(this, "set_ClientState", [response]);},
dispose : function() {
AjaxControlToolkit.NoBotBehavior.callBaseMethod(this, "dispose");},
get_ChallengeScript : function() {
return this._ChallengeScript;},
set_ChallengeScript : function(value) {
if (this._ChallengeScript != value) { 
this._ChallengeScript = value;this.raisePropertyChanged('ChallengeScript');}
}
}
AjaxControlToolkit.NoBotBehavior.registerClass("AjaxControlToolkit.NoBotBehavior", AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
