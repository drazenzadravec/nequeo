Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.DeferredOperation = function(delay, context, callback) {
this._delay = delay;this._context = context;this._callback = callback;this._completeCallback = null;this._errorCallback = null;this._timer = null;this._callArgs = null;this._isComplete = false;this._completedSynchronously = false;this._asyncResult = null;this._exception = null;this._throwExceptions = true;this._oncomplete$delegate = Function.createDelegate(this, this._oncomplete);this.post = Function.createDelegate(this, this.post);}
AjaxControlToolkit.DeferredOperation.prototype = {
get_isPending : function() { 
return (this._timer != null);},
get_isComplete : function() { 
return this._isComplete;},
get_completedSynchronously : function() {
return this._completedSynchronously;},
get_exception : function() {
return this._exception;},
get_throwExceptions : function() {
return this._throwExceptions;}, 
set_throwExceptions : function(value) {
this._throwExceptions = value;},
get_delay : function() { 
return this._delay;},
set_delay : function(value) { 
this._delay = value;},
post : function(args) {
var ar = [];for (var i = 0;i < arguments.length;i++) {
ar[i] = arguments[i];}
this.beginPost(ar, null, null);},
beginPost : function(args, completeCallback, errorCallback) {
this.cancel();this._callArgs = Array.clone(args || []);this._completeCallback = completeCallback;this._errorCallback = errorCallback;if (this._delay == -1) { 
try {
this._oncomplete();} finally {
this._completedSynchronously = true;}
} else { 
this._timer = setTimeout(this._oncomplete$delegate, this._delay);}
}, 
cancel : function() {
if (this._timer) {
clearTimeout(this._timer);this._timer = null;}
this._callArgs = null;this._isComplete = false;this._asyncResult = null;this._completeCallback = null;this._errorCallback = null;this._exception = null;this._completedSynchronously = false;},
call : function(args) {
var ar = [];for (var i = 0;i < arguments.length;i++) {
ar[i] = arguments[i];}
this.cancel();this._callArgs = ar;this._completeCallback = null;this._errorCallback = null;try {
this._oncomplete();} finally {
this._completedSynchronously = true;}
if (this._exception) {
throw this._exception;}
return this._asyncResult;},
complete : function() {
if (this._timer) {
try {
this._oncomplete();} finally {
this._completedSynchronously = true;}
return this._asyncResult;} else if (this._isComplete) {
return this._asyncResult;}
}, 
_oncomplete : function() {
var args = this._callArgs;var completeCallback = this._completeCallback;var errorCallback = this._errorCallback;this.cancel();try {
if (args) {
this._asyncResult = this._callback.apply(this._context, args);} else {
this._asyncResult = this._callback.call(this._context);}
this._isComplete = true;this._completedSynchronously = false;if (completeCallback) {
completeCallback(this);}
} catch (e) {
this._isComplete = true;this._completedSynchronously = false;this._exception = e;if (errorCallback) {
if (errorCallback(this)) {
return;}
} 
if (this._throwExceptions) {
throw e;}
}
}
}
AjaxControlToolkit.DeferredOperation.registerClass("AjaxControlToolkit.DeferredOperation");
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
