Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.CascadingDropDownSelectionChangedEventArgs = function(oldValue, newValue) {
AjaxControlToolkit.CascadingDropDownSelectionChangedEventArgs.initializeBase(this);this._oldValue = oldValue;this._newValue = newValue;}
AjaxControlToolkit.CascadingDropDownSelectionChangedEventArgs.prototype = {
get_oldValue : function() {
return this._oldValue;},
get_newValue : function() {
return this._newValue;}
}
AjaxControlToolkit.CascadingDropDownSelectionChangedEventArgs.registerClass('AjaxControlToolkit.CascadingDropDownSelectionChangedEventArgs', Sys.EventArgs);AjaxControlToolkit.CascadingDropDownBehavior = function(e) {
AjaxControlToolkit.CascadingDropDownBehavior.initializeBase(this, [e]);this._parentControlID = null;this._category = null;this._promptText = null;this._loadingText = null;this._promptValue = null;this._emptyValue = null;this._emptyText = null;this._servicePath = null;this._serviceMethod = null;this._contextKey = null;this._useContextKey = false;this._parentElement = null;this._changeHandler = null;this._parentChangeHandler = null;this._lastParentValues = null;this._selectedValue = null;}
AjaxControlToolkit.CascadingDropDownBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.CascadingDropDownBehavior.callBaseMethod(this, 'initialize');$common.prepareHiddenElementForATDeviceUpdate();var e = this.get_element();this._clearItems();e.CascadingDropDownCategory = this._category;this._changeHandler = Function.createDelegate(this, this._onChange);$addHandler(e, "change",this._changeHandler);if (this._parentControlID) {
this._parentElement = $get(this._parentControlID);Sys.Debug.assert(this._parentElement != null, String.format(AjaxControlToolkit.Resources.CascadingDropDown_NoParentElement, this._parentControlID));if (this._parentElement) {
e.CascadingDropDownParentControlID = this._parentControlID;this._parentChangeHandler = Function.createDelegate(this, this._onParentChange);$addHandler(this._parentElement, "change", this._parentChangeHandler);if (!this._parentElement.childDropDown) {
this._parentElement.childDropDown = new Array();}
this._parentElement.childDropDown.push(this);}
}
this._onParentChange(null, true);},
dispose : function() {
var e = this.get_element();if (this._changeHandler) { 
$removeHandler(e, "change", this._changeHandler);this._changeHandler = null;}
if (this._parentChangeHandler) {
if (this._parentElement) { 
$removeHandler(this._parentElement, "change", this._parentChangeHandler);}
this._parentChangeHandler = null;}
AjaxControlToolkit.CascadingDropDownBehavior.callBaseMethod(this, 'dispose');},
_clearItems : function() {
var e = this.get_element();while (0 < e.options.length) {
e.remove(0);}
},
_isPopulated : function() {
var items = this.get_element().options.length;if (this._promptText) {
return items > 1;} else {
return items > 0;}
},
_setOptions : function(list, inInit, gettingList) {
if (!this.get_isInitialized()) {
return;}
var e = this.get_element();this._clearItems();var headerText;var headerValue = "";if (gettingList && this._loadingText) {
headerText = this._loadingText;if ( this._selectedValue) {
headerValue = this._selectedValue;}
} else if (!gettingList && list && (0 == list.length) && (null != this._emptyText)) {
headerText = this._emptyText;if (this._emptyValue) {
headerValue = this._emptyValue;}
} else if (this._promptText) {
headerText = this._promptText;if (this._promptValue) {
headerValue = this._promptValue;}
}
if (headerText) {
var optionElement = new Option(headerText, headerValue);e.options[e.options.length] = optionElement;}
var selectedValueOption = null;var defaultIndex = -1;if (list) {
for (i = 0 ;i < list.length ;i++) {
var listItemName = list[i].name;var listItemValue = list[i].value;if (list[i].isDefaultValue) {
defaultIndex = i;if (this._promptText) {
defaultIndex++;}
}
var optionElement = new Option(listItemName, listItemValue);if (listItemValue == this._selectedValue) {
selectedValueOption = optionElement;}
e.options[e.options.length] = optionElement;}
if (selectedValueOption) {
selectedValueOption.selected = true;}
}
if (selectedValueOption) {
this.set_SelectedValue(e.options[e.selectedIndex].value, e.options[e.selectedIndex].text);} else if (!selectedValueOption && defaultIndex != -1) {
e.options[defaultIndex].selected = true;this.set_SelectedValue(e.options[defaultIndex].value, e.options[defaultIndex].text);} else if (!inInit && !selectedValueOption && !gettingList && !this._promptText && (e.options.length > 0)) {
this.set_SelectedValue(e.options[0].value, e.options[0].text);} else if (!inInit && !selectedValueOption && !gettingList) {
this.set_SelectedValue('', '');}
if (e.childDropDown && !gettingList) {
for(i = 0;i < e.childDropDown.length;i++) {
e.childDropDown[i]._onParentChange();}
}
else {
if (list && (Sys.Browser.agent !== Sys.Browser.Safari) && (Sys.Browser.agent !== Sys.Browser.Opera)) {
if (document.createEvent) {
var onchangeEvent = document.createEvent('HTMLEvents');onchangeEvent.initEvent('change', true, false);this.get_element().dispatchEvent(onchangeEvent);} else if( document.createEventObject ) {
this.get_element().fireEvent('onchange');}
}
}
if (this._loadingText || this._promptText || this._emptyText) {
e.disabled = !list || (0 == list.length);}
this.raisePopulated(Sys.EventArgs.Empty);},
_onChange : function() {
if (!this._isPopulated()) {
return;}
var e = this.get_element();if ((-1 != e.selectedIndex) && !(this._promptText && (0 == e.selectedIndex))) {
this.set_SelectedValue(e.options[e.selectedIndex].value, e.options[e.selectedIndex].text);} else {
this.set_SelectedValue('', '');}
},
_onParentChange : function(evt, inInit) {
var e = this.get_element();var knownCategoryValues = '';var parentControlID = this._parentControlID;while (parentControlID) {
var parentElement = $get(parentControlID);if (parentElement && (-1 != parentElement.selectedIndex)){
var selectedValue = parentElement.options[parentElement.selectedIndex].value;if (selectedValue && selectedValue != "") {
knownCategoryValues = parentElement.CascadingDropDownCategory + ':' + selectedValue + ';' + knownCategoryValues;parentControlID = parentElement.CascadingDropDownParentControlID;continue;}
} 
break;}
if (knownCategoryValues != '' && this._lastParentValues == knownCategoryValues) {
return;}
this._lastParentValues = knownCategoryValues;if (knownCategoryValues == '' && this._parentControlID) {
this._setOptions(null, inInit);return;}
this._setOptions(null, inInit, true);if (this._servicePath && this._serviceMethod) {
var eventArgs = new Sys.CancelEventArgs();this.raisePopulating(eventArgs);if (eventArgs.get_cancel()) {
return;}
var params = { knownCategoryValues:knownCategoryValues, category:this._category };if (this._useContextKey) {
params.contextKey = this._contextKey;}
Sys.Net.WebServiceProxy.invoke(this._servicePath, this._serviceMethod, false, params,
Function.createDelegate(this, this._onMethodComplete), Function.createDelegate(this, this._onMethodError));$common.updateFormToRefreshATDeviceBuffer();}
},
_onMethodComplete : function(result, userContext, methodName) {
this._setOptions(result);},
_onMethodError : function(webServiceError, userContext, methodName) {
if (webServiceError.get_timedOut()) {
this._setOptions( [ this._makeNameValueObject(AjaxControlToolkit.Resources.CascadingDropDown_MethodTimeout) ] );} else {
this._setOptions( [ this._makeNameValueObject(String.format(AjaxControlToolkit.Resources.CascadingDropDown_MethodError, webServiceError.get_statusCode())) ] );}
},
_makeNameValueObject : function(message) {
return { 'name': message, 'value': message };},
get_ParentControlID : function() {
return this._parentControlID;},
set_ParentControlID : function(value) {
if (this._parentControlID != value) {
this._parentControlID = value;this.raisePropertyChanged('ParentControlID');}
},
get_Category : function() {
return this._category;},
set_Category : function(value) {
if (this._category != value) {
this._category = value;this.raisePropertyChanged('Category');}
},
get_PromptText : function() {
return this._promptText;},
set_PromptText : function(value) {
if (this._promptText != value) {
this._promptText = value;this.raisePropertyChanged('PromptText');}
},
get_PromptValue : function() {
return this._promptValue;},
set_PromptValue : function(value) {
if (this._promptValue != value) {
this._promptValue = value;this.raisePropertyChanged('PromptValue');}
},
get_EmptyText : function() {
return this._emptyText;},
set_EmptyText : function(value) {
if (this._emptyText != value) {
this._emptyText = value;this.raisePropertyChanged('EmptyText');}
},
get_EmptyValue : function() {
return this._emptyValue;},
set_EmptyValue : function(value) {
if (this._emptyValue != value) {
this._emptyValue = value;this.raisePropertyChanged('EmptyValue');}
},
get_LoadingText : function() {
return this._loadingText;},
set_LoadingText : function(value) {
if (this._loadingText != value) {
this._loadingText = value;this.raisePropertyChanged('LoadingText');}
},
get_SelectedValue : function() {
return this._selectedValue;},
set_SelectedValue : function(value, text) {
if (this._selectedValue != value) {
if (!text) {
var i = value.indexOf(':::');if (-1 != i) {
text = value.slice(i + 3);value = value.slice(0, i);}
}
var oldValue = this._selectedValue;this._selectedValue = value;this.raisePropertyChanged('SelectedValue');this.raiseSelectionChanged(new AjaxControlToolkit.CascadingDropDownSelectionChangedEventArgs(oldValue, value));}
AjaxControlToolkit.CascadingDropDownBehavior.callBaseMethod(this, 'set_ClientState', [ this._selectedValue+':::'+text ]);},
get_ServicePath : function() {
return this._servicePath;},
set_ServicePath : function(value) {
if (this._servicePath != value) {
this._servicePath = value;this.raisePropertyChanged('ServicePath');}
},
get_ServiceMethod : function() {
return this._serviceMethod;},
set_ServiceMethod : function(value) {
if (this._serviceMethod != value) {
this._serviceMethod = value;this.raisePropertyChanged('ServiceMethod');}
},
get_contextKey : function() {
return this._contextKey;},
set_contextKey : function(value) {
if (this._contextKey != value) {
this._contextKey = value;this.set_useContextKey(true);this.raisePropertyChanged('contextKey');}
},
get_useContextKey : function() {
return this._useContextKey;},
set_useContextKey : function(value) {
if (this._useContextKey != value) {
this._useContextKey = value;this.raisePropertyChanged('useContextKey');}
},
add_selectionChanged : function(handler) {
this.get_events().addHandler('selectionChanged', handler);},
remove_selectionChanged : function(handler) {
this.get_events().removeHandler('selectionChanged', handler);},
raiseSelectionChanged : function(eventArgs) {
var handler = this.get_events().getHandler('selectionChanged');if (handler) {
handler(this, eventArgs);}
},
add_populating : function(handler) {
this.get_events().addHandler('populating', handler);},
remove_populating : function(handler) {
this.get_events().removeHandler('populating', handler);},
raisePopulating : function(eventArgs) {
var handler = this.get_events().getHandler('populating');if (handler) {
handler(this, eventArgs);}
},
add_populated : function(handler) {
this.get_events().addHandler('populated', handler);},
remove_populated : function(handler) {
this.get_events().removeHandler('populated', handler);},
raisePopulated : function(eventArgs) {
var handler = this.get_events().getHandler('populated');if (handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit.CascadingDropDownBehavior.registerClass('AjaxControlToolkit.CascadingDropDownBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
