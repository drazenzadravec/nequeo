Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.ListSearchBehavior = function(element) {
AjaxControlToolkit.ListSearchBehavior.initializeBase(this, [element]);this._promptCssClass = null;this._promptText = AjaxControlToolkit.Resources.ListSearch_DefaultPrompt;this._offsetX = 0;this._offsetY = 0;this._promptPosition = AjaxControlToolkit.ListSearchPromptPosition.Top;this._raiseImmediateOnChange = false;this._queryPattern = AjaxControlToolkit.ListSearchQueryPattern.StartsWith;this._isSorted = false;this._popupBehavior = null;this._onShowJson = null;this._onHideJson = null;this._originalIndex = 0;this._newIndex = -1;this._showingPromptText = false;this._searchText = '';this._ellipsis = String.fromCharCode(0x2026);this._binarySearch = false;this._applicationLoadDelegate = null;this._focusIndex = 0;this._queryTimeout = 0;this._timer = null;this._matchFound = false;this._focusHandler = null;this._blurHandler = null;this._keyDownHandler = null;this._keyUpHandler = null;this._keyPressHandler = null;}
AjaxControlToolkit.ListSearchBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.ListSearchBehavior.callBaseMethod(this, 'initialize');var element = this.get_element();if(element && element.tagName === 'SELECT') {
this._focusHandler = Function.createDelegate(this, this._onFocus);this._blurHandler = Function.createDelegate(this, this._onBlur);this._keyDownHandler = Function.createDelegate(this, this._onKeyDown);this._keyUpHandler = Function.createDelegate(this, this._onKeyUp);this._keyPressHandler = Function.createDelegate(this, this._onKeyPress);$addHandler(element, "focus", this._focusHandler);$addHandler(element, "blur", this._blurHandler);$addHandler(element, "keydown", this._keyDownHandler);$addHandler(element, "keyup", this._keyUpHandler);$addHandler(element, "keypress", this._keyPressHandler);this._applicationLoadDelegate = Function.createDelegate(this, this._onApplicationLoad);Sys.Application.add_load(this._applicationLoadDelegate);}
},
dispose : function() {
var element = this.get_element();$removeHandler(element, "keypress", this._keyPressHandler);$removeHandler(element, "keyup", this._keyUpHandler);$removeHandler(element, "keydown", this._keyDownHandler);$removeHandler(element, "blur", this._blurHandler);$removeHandler(element, "focus", this._focusHandler);this._onShowJson = null;this._onHideJson = null;this._disposePopupBehavior();if(this._applicationLoadDelegate) {
Sys.Application.remove_load(this._applicationLoadDelegate);this._applicationLoadDelegate = null;}
if(this._timer) {
this._stopTimer();} 
AjaxControlToolkit.ListSearchBehavior.callBaseMethod(this, 'dispose');},
_onApplicationLoad : function(sender, applicationLoadEventArgs) {
var hasInitialFocus = false;var clientState = AjaxControlToolkit.ListSearchBehavior.callBaseMethod(this, 'get_ClientState');if (clientState != null && clientState != "") {
hasInitialFocus = (clientState === "Focused");AjaxControlToolkit.ListSearchBehavior.callBaseMethod(this, 'set_ClientState', null);}
if(hasInitialFocus) {
this._handleFocus();}
},
_checkIfSorted : function(options) {
if (this._isSorted) {
return true;} else {
var previousOptionValue = null;var optionsLength = options.length;for(var index = 0;index < optionsLength;index++) {
var optionValue = options[index].text.toLowerCase();if(previousOptionValue && this._compareStrings(optionValue, previousOptionValue) < 0) {
return false;}
previousOptionValue = optionValue;}
return true;}
},
_onFocus : function(e) {
this._handleFocus();},
_handleFocus : function() {
var element = this.get_element();this._focusIndex = element.selectedIndex;if(!this._promptDiv) {
this._promptDiv = document.createElement('div');this._promptDiv.id = element.id + '_promptDiv';this._promptDiv.innerHTML = this._promptText && this._promptText.length > 0 ? this._promptText : AjaxControlToolkit.Resources.ListSearch_DefaultPrompt;this._showingPromptText = true;if(this._promptCssClass) {
this._promptDiv.className = this._promptCssClass;}
element.parentNode.insertBefore(this._promptDiv, element.nextSibling);this._promptDiv.style.overflow = 'hidden';this._promptDiv.style.height = this._promptDiv.offsetHeight + 'px';this._promptDiv.style.width = element.offsetWidth + 'px';}
if(!this._popupBehavior) {
this._popupBehavior = $create(AjaxControlToolkit.PopupBehavior, { parentElement : element }, {}, {}, this._promptDiv);}
if (this._promptPosition && this._promptPosition == AjaxControlToolkit.ListSearchPromptPosition.Bottom) {
this._popupBehavior.set_positioningMode(AjaxControlToolkit.PositioningMode.BottomLeft);} else {
this._popupBehavior.set_positioningMode(AjaxControlToolkit.PositioningMode.TopLeft);}
if (this._onShowJson) {
this._popupBehavior.set_onShow(this._onShowJson);}
if (this._onHideJson) {
this._popupBehavior.set_onHide(this._onHideJson);}
this._popupBehavior.show();this._updatePromptDiv(this._promptText);},
_onBlur : function() {
this._disposePopupBehavior();var promptDiv = this._promptDiv;var element = this.get_element();if(promptDiv) {
this._promptDiv = null;element.parentNode.removeChild(promptDiv);}
if(!this._raiseImmediateOnChange && this._focusIndex != element.selectedIndex) {
this._raiseOnChange(element);} 
},
_disposePopupBehavior : function() {
if (this._popupBehavior) {
this._popupBehavior.dispose();this._popupBehavior = null;}
},
_onKeyDown : function(e) {
var element = this.get_element();var promptDiv = this._promptDiv;if(!element || !promptDiv) {
return;}
this._originalIndex = element.selectedIndex;if(this._showingPromptText) {
promptDiv.innerHTML = '';this._searchText = '';this._showingPromptText = false;this._binarySearch = this._checkIfSorted(element.options);} 
if(e.keyCode == Sys.UI.Key.backspace) {
e.preventDefault();e.stopPropagation();this._removeCharacterFromPromptDiv();this._searchForTypedText(element);if(!this._searchText || this._searchText.length == 0) {
this._stopTimer();}
} else if(e.keyCode == Sys.UI.Key.esc) {
e.preventDefault();e.stopPropagation();promptDiv.innerHTML = '';this._searchText = '';this._searchForTypedText(element);this._stopTimer();} else if(e.keyCode == Sys.UI.Key.enter && !this._raiseImmediateOnChange && this._focusIndex != element.selectedIndex) {
this._focusIndex = element.selectedIndex;this._raiseOnChange(element);} 
},
_onKeyUp : function(e) {
var element = this.get_element();var promptDiv = this._promptDiv;if(!element || !promptDiv) {
return;}
if(this._newIndex == -1 || !element || !promptDiv || promptDiv.innerHTML == '') {
this._newIndex = -1;return;}
element.selectedIndex = this._newIndex;this._newIndex = -1;},
_onKeyPress : function(e) {
var element = this.get_element();var promptDiv = this._promptDiv;if(!element || !promptDiv) {
return;}
if(!this._isNormalChar(e)) {
if(e.charCode == Sys.UI.Key.backspace) {
e.preventDefault();e.stopPropagation();if(this._searchText && this._searchText.length == 0) {
this._stopTimer();} 
}
return;}
e.preventDefault();e.stopPropagation();this._addCharacterToPromptDiv(e.charCode);this._searchForTypedText(element);this._stopTimer();if(this._searchText && this._searchText.length != 0) {
this._startTimer();} 
},
_isNormalChar : function(e) {
if (Sys.Browser.agent == Sys.Browser.Firefox && e.rawEvent.keyCode) {
return false;}
if (Sys.Browser.agent == Sys.Browser.Opera && e.rawEvent.which == 0) {
return false;}
if (e.charCode && (e.charCode < Sys.UI.Key.space || e.charCode > 6000)) {
return false;}
return true;},
_updatePromptDiv : function(newText) {
var promptDiv = this._promptDiv;if(!promptDiv || !this.get_element()) {
return;}
var text = typeof(newText) === 'undefined' ? this._searchText : newText;var textNode = promptDiv.firstChild;if(!textNode) {
textNode = document.createTextNode(text);promptDiv.appendChild(textNode);} else {
textNode.nodeValue = text;}
if(promptDiv.scrollWidth <= promptDiv.offsetWidth && promptDiv.scrollHeight <= promptDiv.offsetHeight) {
return;}
for(var maxFit = text.length - 1;maxFit > 0 && (promptDiv.scrollWidth > promptDiv.offsetWidth || promptDiv.scrollHeight > promptDiv.offsetHeight);maxFit--) {
textNode.nodeValue = this._ellipsis + text.substring(text.length - maxFit, text.length);}
},
_addCharacterToPromptDiv : function (charCode) {
this._searchText += String.fromCharCode(charCode);this._updatePromptDiv();},
_removeCharacterFromPromptDiv : function () {
if(this._searchText && this._searchText != '') {
this._searchText = this._searchText.substring(0, this._searchText.length - 1);this._updatePromptDiv();}
},
_searchForTypedText : function(element) {
var searchText = this._searchText;var options = element.options;var text = searchText ? searchText.toLowerCase() : "";this._matchFound = false;if(text.length == 0) { 
if(options.length > 0) {
element.selectedIndex = 0;this._newIndex = 0;}
} else {
var selectedIndex = -1;if(this._binarySearch && (this._queryPattern == AjaxControlToolkit.ListSearchQueryPattern.StartsWith)) {
selectedIndex = this._doBinarySearch(options, text, 0, options.length - 1);} else {
selectedIndex = this._doLinearSearch(options, text, 0, options.length - 1);}
if(selectedIndex == -1) {
this._newIndex = this._originalIndex;} else { 
element.selectedIndex = selectedIndex;this._newIndex = selectedIndex;this._matchFound = true;}
}
if(this._raiseImmediateOnChange && this._originalIndex != element.selectedIndex) {
this._raiseOnChange(element);}
},
_raiseOnChange : function(element) {
if (document.createEvent) {
var onchangeEvent = document.createEvent('HTMLEvents');onchangeEvent.initEvent('change', true, false);element.dispatchEvent(onchangeEvent);} else if( document.createEventObject ) {
element.fireEvent('onchange');}
},
_compareStrings : function(strA, strB) {
return ((strA == strB) ? 0 : ((strA < strB) ? -1 : 1))
},
_doBinarySearch : function(options, value, left, right) {
while (left <= right) {
var mid = Math.floor((left+right)/2);var option = options[mid].text.toLowerCase().substring(0, value.length);var compareResult = this._compareStrings(value, option);if (compareResult > 0) {
left = mid+1
} else if(compareResult < 0) {
right = mid-1;} else {
while(mid > 0 && options[mid - 1].text.toLowerCase().startsWith(value)) {
mid--;}
return mid;}
}
return -1;},
_doLinearSearch : function(options, value, left, right) {
if (this._queryPattern == AjaxControlToolkit.ListSearchQueryPattern.Contains) {
for(var i = left;i <= right;i++) {
if(options[i].text.toLowerCase().indexOf(value) >= 0) {
return i;}
}
} else if (this._queryPattern == AjaxControlToolkit.ListSearchQueryPattern.StartsWith) {
for(var i = left;i <= right;i++) {
if(options[i].text.toLowerCase().startsWith(value)) {
return i;}
}
}
return -1;},
_onTimerTick : function() {
this._stopTimer();if (!this._matchFound) {
this._searchText = '';this._updatePromptDiv();}
},
_startTimer : function() {
if (this._queryTimeout > 0) {
this._timer = window.setTimeout(Function.createDelegate(this, this._onTimerTick), this._queryTimeout);}
},
_stopTimer : function() {
if(this._timer != null) {
window.clearTimeout(this._timer);}
this._timer = null;},
get_onShow : function() {
return this._popupBehavior ? this._popupBehavior.get_onShow() : this._onShowJson;},
set_onShow : function(value) {
if (this._popupBehavior) {
this._popupBehavior.set_onShow(value)
} else {
this._onShowJson = value;}
this.raisePropertyChanged('onShow');},
get_onShowBehavior : function() {
return this._popupBehavior ? this._popupBehavior.get_onShowBehavior() : null;},
onShow : function() {
if (this._popupBehavior) {
this._popupBehavior.onShow();}
},
get_onHide : function() {
return this._popupBehavior ? this._popupBehavior.get_onHide() : this._onHideJson;},
set_onHide : function(value) {
if (this._popupBehavior) {
this._popupBehavior.set_onHide(value)
} else {
this._onHideJson = value;}
this.raisePropertyChanged('onHide');},
get_onHideBehavior : function() {
return this._popupBehavior ? this._popupBehavior.get_onHideBehavior() : null;},
onHide : function() {
if (this._popupBehavior) {
this._popupBehavior.onHide();}
},
get_promptText : function() {
return this._promptText;}, 
set_promptText : function(value) {
if (this._promptText != value) {
this._promptText = value;this.raisePropertyChanged('promptText');}
}, 
get_promptCssClass : function() {
return this._promptCssClass;}, 
set_promptCssClass : function(value) {
if (this._promptCssClass != value) {
this._promptCssClass = value;this.raisePropertyChanged('promptCssClass');}
},
get_promptPosition : function() {
return this._promptPosition;},
set_promptPosition : function(value) {
if (this._promptPosition != value) {
this._promptPosition = value;this.raisePropertyChanged('promptPosition');}
},
get_raiseImmediateOnChange : function() {
return this._raiseImmediateOnChange;},
set_raiseImmediateOnChange : function(value) {
if (this._raiseImmediateOnChange != value) {
this._raiseImmediateOnChange = value;this.raisePropertyChanged('raiseImmediateOnChange');}
},
get_queryTimeout : function() {
return this._queryTimeout;},
set_queryTimeout : function(value) {
if (this._queryTimeout != value) {
this._queryTimeout = value;this.raisePropertyChanged('queryTimeout');}
},
get_isSorted : function() {
return this._isSorted;},
set_isSorted : function(value) {
if (this._isSorted != value) {
this._isSorted = value;this.raisePropertyChanged('isSorted');}
},
get_queryPattern : function() {
return this._queryPattern;},
set_queryPattern : function(value) {
if (this._queryPattern != value) {
this._queryPattern = value;this.raisePropertyChanged('queryPattern');}
} 
}
AjaxControlToolkit.ListSearchBehavior.registerClass('AjaxControlToolkit.ListSearchBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.ListSearchPromptPosition = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.ListSearchPromptPosition.prototype = {
Top: 0,
Bottom: 1
}
AjaxControlToolkit.ListSearchPromptPosition.registerEnum('AjaxControlToolkit.ListSearchPromptPosition');AjaxControlToolkit.ListSearchQueryPattern = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.ListSearchQueryPattern.prototype = {
StartsWith: 0,
Contains: 1
}
AjaxControlToolkit.ListSearchQueryPattern.registerEnum('AjaxControlToolkit.ListSearchQueryPattern');
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
