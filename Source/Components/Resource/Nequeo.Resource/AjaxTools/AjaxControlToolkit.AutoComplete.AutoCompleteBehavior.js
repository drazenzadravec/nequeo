Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.AutoCompleteBehavior = function(element) {
AjaxControlToolkit.AutoCompleteBehavior.initializeBase(this, [element]);this._servicePath = null;this._serviceMethod = null;this._contextKey = null;this._useContextKey = false;this._minimumPrefixLength = 3;this._completionSetCount = 10;this._completionInterval = 1000;this._completionListElementID = null;this._completionListElement = null;this._textColor = 'windowtext';this._textBackground = 'window';this._popupBehavior = null;this._popupBehaviorHiddenHandler = null;this._onShowJson = null;this._onHideJson = null;this._timer = null;this._cache = null;this._currentPrefix = null;this._selectIndex = -1;this._focusHandler = null;this._blurHandler = null;this._bodyClickHandler = null;this._completionListBlurHandler = null;this._keyDownHandler = null;this._mouseDownHandler = null;this._mouseUpHandler = null;this._mouseOverHandler = null;this._tickHandler = null;this._enableCaching = true;this._flyoutHasFocus = false;this._textBoxHasFocus = false;this._completionListCssClass = null;this._completionListItemCssClass = null;this._highlightedItemCssClass = null;this._delimiterCharacters = null;this._firstRowSelected = false;this._showOnlyCurrentWordInCompletionListItem = false;this._webRequest = null;}
AjaxControlToolkit.AutoCompleteBehavior.prototype = {
initialize: function() {
AjaxControlToolkit.AutoCompleteBehavior.callBaseMethod(this, 'initialize');$common.prepareHiddenElementForATDeviceUpdate();this._popupBehaviorHiddenHandler = Function.createDelegate(this, this._popupHidden);this._tickHandler = Function.createDelegate(this, this._onTimerTick);this._focusHandler = Function.createDelegate(this, this._onGotFocus);this._blurHandler = Function.createDelegate(this, this._onLostFocus);this._keyDownHandler = Function.createDelegate(this, this._onKeyDown);this._mouseDownHandler = Function.createDelegate(this, this._onListMouseDown);this._mouseUpHandler = Function.createDelegate(this, this._onListMouseUp);this._mouseOverHandler = Function.createDelegate(this, this._onListMouseOver);this._completionListBlurHandler = Function.createDelegate(this, this._onCompletionListBlur);this._bodyClickHandler = Function.createDelegate(this, this._onCompletionListBlur);this._timer = new Sys.Timer();this.initializeTimer(this._timer);var element = this.get_element();this.initializeTextBox(element);if(this._completionListElementID !== null)
this._completionListElement = $get(this._completionListElementID);if (this._completionListElement == null ) {
this._completionListElement = document.createElement('ul');this._completionListElement.id = this.get_id() + '_completionListElem';if (Sys.Browser.agent === Sys.Browser.Safari) {
document.body.appendChild(this._completionListElement);} else {
element.parentNode.insertBefore(this._completionListElement, element.nextSibling);}
}
this.initializeCompletionList(this._completionListElement);this._popupBehavior = $create(AjaxControlToolkit.PopupBehavior, 
{ 'id':this.get_id()+'PopupBehavior', 'parentElement':element, "positioningMode": AjaxControlToolkit.PositioningMode.BottomLeft }, null, null, this._completionListElement);this._popupBehavior.add_hidden(this._popupBehaviorHiddenHandler);if (this._onShowJson) {
this._popupBehavior.set_onShow(this._onShowJson);}
if (this._onHideJson) {
this._popupBehavior.set_onHide(this._onHideJson);}
},
dispose: function() {
this._onShowJson = null;this._onHideJson = null;if (this._popupBehavior) {
if (this._popupBehaviorHiddenHandler) {
this._popupBehavior.remove_hidden(this._popupBehaviorHiddenHandler);}
this._popupBehavior.dispose();this._popupBehavior = null;}
if (this._timer) { 
this._timer.dispose();this._timer = null;}
var element = this.get_element();if (element) {
$removeHandler(element, "focus", this._focusHandler);$removeHandler(element, "blur", this._blurHandler);$removeHandler(element, "keydown", this._keyDownHandler);$removeHandler(this._completionListElement, 'blur', this._completionListBlurHandler);$removeHandler(this._completionListElement, 'mousedown', this._mouseDownHandler);$removeHandler(this._completionListElement, 'mouseup', this._mouseUpHandler);$removeHandler(this._completionListElement, 'mouseover', this._mouseOverHandler);}
if (this._bodyClickHandler) {
$removeHandler(document.body, 'click', this._bodyClickHandler);this._bodyClickHandler = null;}
this._popupBehaviorHiddenHandler = null;this._tickHandler = null;this._focusHandler = null;this._blurHandler = null;this._keyDownHandler = null;this._completionListBlurHandler = null;this._mouseDownHandler = null;this._mouseUpHandler = null;this._mouseOverHandler = null;AjaxControlToolkit.AutoCompleteBehavior.callBaseMethod(this, 'dispose');},
initializeTimer: function(timer) {
timer.set_interval(this._completionInterval);timer.add_tick(this._tickHandler);},
initializeTextBox: function(element) {
element.autocomplete = "off";$addHandler(element, "focus", this._focusHandler);$addHandler(element, "blur", this._blurHandler);$addHandler(element, "keydown", this._keyDownHandler);},
initializeCompletionList: function(element) {
if(this._completionListCssClass) {
Sys.UI.DomElement.addCssClass(element, this._completionListCssClass);} else {
var completionListStyle = element.style;completionListStyle.textAlign = 'left';completionListStyle.visibility = 'hidden';completionListStyle.cursor = 'default';completionListStyle.listStyle = 'none';completionListStyle.padding = '0px';completionListStyle.margin = '0px! important';if (Sys.Browser.agent === Sys.Browser.Safari) {
completionListStyle.border = 'solid 1px gray';completionListStyle.backgroundColor = 'white';completionListStyle.color = 'black';} else {
completionListStyle.border = 'solid 1px buttonshadow';completionListStyle.backgroundColor = this._textBackground;completionListStyle.color = this._textColor;}
}
$addHandler(element, "mousedown", this._mouseDownHandler);$addHandler(element, "mouseup", this._mouseUpHandler);$addHandler(element, "mouseover", this._mouseOverHandler);$addHandler(element, "blur", this._completionListBlurHandler);$addHandler(document.body, 'click', this._bodyClickHandler);},
_currentCompletionWord: function() {
var element = this.get_element();var elementValue = element.value;var word = elementValue;if (this.get_isMultiWord()) {
var startIndex = this._getCurrentWordStartIndex();var endIndex = this._getCurrentWordEndIndex(startIndex);if (endIndex <= startIndex) {
word = elementValue.substring(startIndex);} else {
word = elementValue.substring(startIndex, endIndex);}
}
return word;},
_getCursorIndex: function() {
return this.get_element().selectionStart;},
_getCurrentWordStartIndex: function() {
var element = this.get_element();var elementText = element.value.substring(0,this._getCursorIndex());var index = 0;var lastIndex = -1;for (var i = 0;i < this._delimiterCharacters.length;++i) {
var curIndex = elementText.lastIndexOf(this._delimiterCharacters.charAt(i));if (curIndex > lastIndex) {
lastIndex = curIndex;}
} 
index = lastIndex;if (index >= this._getCursorIndex()) {
index = 0;}
return index < 0 ? 0 : index + 1;},
_getCurrentWordEndIndex: function(wordStartIndex) {
var element = this.get_element();var elementText = element.value.substring(wordStartIndex);var index = 0;for (var i = 0;i < this._delimiterCharacters.length;++i) {
var curIndex = elementText.indexOf(this._delimiterCharacters.charAt(i));if (curIndex > 0 && (curIndex < index || index == 0)) {
index = curIndex;}
}
return index <= 0 ? element.value.length : index + wordStartIndex;},
get_isMultiWord : function() {
return (this._delimiterCharacters != null) && (this._delimiterCharacters != '');},
_getTextWithInsertedWord: function(wordToInsert) {
var text = wordToInsert;var replaceIndex = 0;var element = this.get_element();var originalText = element.value;if (this.get_isMultiWord()) {
var startIndex = this._getCurrentWordStartIndex();var endIndex = this._getCurrentWordEndIndex(startIndex);var prefix = '';var suffix = '';if (startIndex > 0) {
prefix = originalText.substring(0, startIndex);}
if (endIndex > startIndex) {
suffix = originalText.substring(endIndex);}
text = prefix + wordToInsert + suffix;}
return text;},
_hideCompletionList: function() {
var eventArgs = new Sys.CancelEventArgs();this.raiseHiding(eventArgs);if (eventArgs.get_cancel()) {
return;}
this.hidePopup();},
showPopup : function() {
this._popupBehavior.show();this.raiseShown(Sys.EventArgs.Empty);},
hidePopup : function() {
if (this._popupBehavior) {
this._popupBehavior.hide();} else {
this._popupHidden();}
},
_popupHidden : function() {
this._completionListElement.innerHTML = '';this._selectIndex = -1;this._flyoutHasFocus = false;this.raiseHidden(Sys.EventArgs.Empty);},
_highlightItem: function(item) {
var children = this._completionListElement.childNodes;for (var i = 0;i < children.length;i++) {
var child = children[i];if (child._highlighted) {
if (this._completionListItemCssClass) {
Sys.UI.DomElement.removeCssClass(child, this._highlightedItemCssClass);Sys.UI.DomElement.addCssClass(child, this._completionListItemCssClass);} else {
if (Sys.Browser.agent === Sys.Browser.Safari) {
child.style.backgroundColor = 'white';child.style.color = 'black';} else {
child.style.backgroundColor = this._textBackground;child.style.color = this._textColor;}
}
this.raiseItemOut(new AjaxControlToolkit.AutoCompleteItemEventArgs(child, child.firstChild.nodeValue, child._value));}
}
if(this._highlightedItemCssClass) {
Sys.UI.DomElement.removeCssClass(item, this._completionListItemCssClass);Sys.UI.DomElement.addCssClass(item, this._highlightedItemCssClass);} else {
if (Sys.Browser.agent === Sys.Browser.Safari) {
item.style.backgroundColor = 'lemonchiffon';} else {
item.style.backgroundColor = 'highlight';item.style.color = 'highlighttext';}
}
item._highlighted = true;this.raiseItemOver(new AjaxControlToolkit.AutoCompleteItemEventArgs(item, item.firstChild.nodeValue, item._value));},
_onCompletionListBlur: function(ev) {
this._hideCompletionList();},
_onListMouseDown: function(ev) {
if (ev.target !== this._completionListElement) {
this._setText(ev.target);this._flyoutHasFocus = false;} else { 
this._flyoutHasFocus = true;}
},
_onListMouseUp: function(ev) {
this.get_element().focus();},
_onListMouseOver: function(ev) {
var item = ev.target;if(item !== this._completionListElement) {
var children = this._completionListElement.childNodes;for (var i = 0;i < children.length;++i) {
if (item === children[i]) {
this._highlightItem(item);this._selectIndex = i;break;} 
}
}
},
_onGotFocus: function(ev) {
this._textBoxHasFocus = true;if (this._flyoutHasFocus) {
this._hideCompletionList();}
if ((this._minimumPrefixLength == 0) && (!this.get_element().value)) {
this._timer.set_enabled(true);}
},
_onKeyDown: function(ev) {
this._timer.set_enabled(false);var k = ev.keyCode ? ev.keyCode : ev.rawEvent.keyCode;if (k === Sys.UI.Key.esc) {
this._hideCompletionList();ev.preventDefault();}
else if (k === Sys.UI.Key.up) {
if (this._selectIndex > 0) {
this._selectIndex--;this._handleScroll(this._completionListElement.childNodes[this._selectIndex], this._selectIndex);this._highlightItem(this._completionListElement.childNodes[this._selectIndex]);ev.stopPropagation();ev.preventDefault();}
}
else if (k === Sys.UI.Key.down) {
if (this._selectIndex < (this._completionListElement.childNodes.length - 1)) {
this._selectIndex++;this._handleScroll(this._completionListElement.childNodes[this._selectIndex], this._selectIndex);this._highlightItem(this._completionListElement.childNodes[this._selectIndex]);ev.stopPropagation();ev.preventDefault();}
}
else if (k === Sys.UI.Key.enter) {
if (this._selectIndex !== -1) {
this._setText(this._completionListElement.childNodes[this._selectIndex]);ev.preventDefault();} else {
this.hidePopup();}
}
else if (k === Sys.UI.Key.tab) {
if (this._selectIndex !== -1) {
this._setText(this._completionListElement.childNodes[this._selectIndex]);}
}
else {
this._timer.set_enabled(true);}
},
_handleScroll : function(element, index) {
var flyout = this._completionListElement;var elemBounds = $common.getBounds(element);var numItems = this._completionListElement.childNodes.length;if (((elemBounds.height * index) - (flyout.clientHeight + flyout.scrollTop)) >= 0) {
flyout.scrollTop += (((elemBounds.height * index) - (flyout.clientHeight + flyout.scrollTop)) + elemBounds.height);}
if (((elemBounds.height * (numItems - (index + 1))) - (flyout.scrollHeight - flyout.scrollTop)) >= 0) {
flyout.scrollTop -= (((elemBounds.height * (numItems - (index + 1))) - (flyout.scrollHeight - flyout.scrollTop)) + elemBounds.height);} 
if (flyout.scrollTop % elemBounds.height !== 0) { 
if (((elemBounds.height * (index + 1)) - (flyout.clientHeight + flyout.scrollTop)) >= 0) { 
flyout.scrollTop -= (flyout.scrollTop % elemBounds.height);} else { 
flyout.scrollTop += (elemBounds.height - (flyout.scrollTop % elemBounds.height));}
} 
},
_handleFlyoutFocus : function() {
if(!this._textBoxHasFocus) { 
if (!this._flyoutHasFocus) {
if (this._webRequest) {
this._webRequest.get_executor().abort();this._webRequest = null;}
this._hideCompletionList();} else {
}
}
}, 
_onLostFocus: function() {
this._textBoxHasFocus = false;this._timer.set_enabled(false);window.setTimeout(Function.createDelegate(this, this._handleFlyoutFocus), 500);}, 
_onMethodComplete: function(result, context) {
this._webRequest = null;this._update(context, result,  true);},
_onMethodFailed: function(err, response, context) {
this._webRequest = null;},
_onTimerTick: function(sender, eventArgs) {
this._timer.set_enabled(false);if (this._servicePath && this._serviceMethod) {
var text = this._currentCompletionWord();if (text.trim().length < this._minimumPrefixLength) {
this._currentPrefix = null;this._update('', null,  false);return;}
if ((this._currentPrefix !== text) || ((text == "") && (this._minimumPrefixLength == 0))) {
this._currentPrefix = text;if ((text != "") && this._cache && this._cache[text]) {
this._update(text, this._cache[text],  false);return;}
var eventArgs = new Sys.CancelEventArgs();this.raisePopulating(eventArgs);if (eventArgs.get_cancel()) {
return;}
var params = { prefixText : this._currentPrefix, count: this._completionSetCount };if (this._useContextKey) {
params.contextKey = this._contextKey;}
if (this._webRequest) {
this._webRequest.get_executor().abort();this._webRequest = null;}
this._webRequest = Sys.Net.WebServiceProxy.invoke(this.get_servicePath(), this.get_serviceMethod(), false, params,
Function.createDelegate(this, this._onMethodComplete),
Function.createDelegate(this, this._onMethodFailed),
text);$common.updateFormToRefreshATDeviceBuffer();}
}
},
_setText: function(item) {
var text = (item && item.firstChild) ? item.firstChild.nodeValue : null;this._timer.set_enabled(false);var element = this.get_element();var control = element.control;var newText = this._showOnlyCurrentWordInCompletionListItem ? this._getTextWithInsertedWord(text) : text;if (control && control.set_text) {
control.set_text(newText);} else {
element.value = newText;}
$common.tryFireEvent(element, "change");this.raiseItemSelected(new AjaxControlToolkit.AutoCompleteItemEventArgs(item, text, item ? item._value : null));this._currentPrefix = this._currentCompletionWord();this._hideCompletionList();},
_update: function(prefixText, completionItems, cacheResults) {
if (cacheResults && this.get_enableCaching()) {
if (!this._cache) {
this._cache = {};}
this._cache[prefixText] = completionItems;}
if ((!this._textBoxHasFocus) || (prefixText != this._currentCompletionWord())) {
this._hideCompletionList();return;} 
if (completionItems && completionItems.length) {
this._completionListElement.innerHTML = '';this._selectIndex = -1;var _firstChild = null;var text = null;var value = null;for (var i = 0;i < completionItems.length;i++) {
var itemElement = null;if (this._completionListElementID) { 
itemElement = document.createElement('div');} else {
itemElement = document.createElement('li');}
if( _firstChild == null ){
_firstChild = itemElement;}
try {
var pair = Sys.Serialization.JavaScriptSerializer.deserialize('(' + completionItems[i] + ')');if (pair && pair.First) {
text = pair.First;value = pair.Second;} else {
text = pair;value = pair;} 
} catch (ex) {
text = completionItems[i];value = completionItems[i];}
var optionText = this._showOnlyCurrentWordInCompletionListItem ? text : this._getTextWithInsertedWord(text);itemElement.appendChild(document.createTextNode(optionText));itemElement._value = value;itemElement.__item = '';if (this._completionListItemCssClass) {
Sys.UI.DomElement.addCssClass(itemElement, this._completionListItemCssClass);} else {
var itemElementStyle = itemElement.style;itemElementStyle.padding = '0px';itemElementStyle.textAlign = 'left';itemElementStyle.textOverflow = 'ellipsis';if (Sys.Browser.agent === Sys.Browser.Safari) {
itemElementStyle.backgroundColor = 'white';itemElementStyle.color = 'black';} else {
itemElementStyle.backgroundColor = this._textBackground;itemElementStyle.color = this._textColor;}
}
this._completionListElement.appendChild(itemElement);}
var elementBounds = $common.getBounds(this.get_element());this._completionListElement.style.width = Math.max(1, elementBounds.width - 2) + 'px';this._completionListElement.scrollTop = 0;this.raisePopulated(Sys.EventArgs.Empty);var eventArgs = new Sys.CancelEventArgs();this.raiseShowing(eventArgs);if (!eventArgs.get_cancel()) {
this.showPopup();if (this._firstRowSelected && (_firstChild != null)) {
this._highlightItem( _firstChild );this._selectIndex = 0;}
} 
} else {
this._hideCompletionList();}
},
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
get_completionInterval: function() {
return this._completionInterval;},
set_completionInterval: function(value) {
if (this._completionInterval != value) {
this._completionInterval = value;this.raisePropertyChanged('completionInterval');}
},
get_completionList: function() {
return this._completionListElement;},
set_completionList: function(value) {
if (this._completionListElement != value) {
this._completionListElement = value;this.raisePropertyChanged('completionList');}
},
get_completionSetCount: function() {
return this._completionSetCount;},
set_completionSetCount: function(value) {
if (this._completionSetCount != value) {
this._completionSetCount = value;this.raisePropertyChanged('completionSetCount');}
},
get_minimumPrefixLength: function() {
return this._minimumPrefixLength;},
set_minimumPrefixLength: function(value) {
if (this._minimumPrefixLength != value) {
this._minimumPrefixLength = value;this.raisePropertyChanged('minimumPrefixLength');}
},
get_serviceMethod: function() {
return this._serviceMethod;},
set_serviceMethod: function(value) {
if (this._serviceMethod != value) {
this._serviceMethod = value;this.raisePropertyChanged('serviceMethod');}
},
get_servicePath: function() {
return this._servicePath;},
set_servicePath: function(value) {
if (this._servicePath != value) {
this._servicePath = value;this.raisePropertyChanged('servicePath');}
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
get_enableCaching: function() {
return this._enableCaching;},
set_enableCaching: function(value) {
if (this._enableCaching != value) {
this._enableCaching = value;this.raisePropertyChanged('enableCaching');}
},
get_completionListElementID: function() {
return this._completionListElementID;},
set_completionListElementID: function(value) {
if (this._completionListElementID != value) {
this._completionListElementID = value;this.raisePropertyChanged('completionListElementID');}
}, 
get_completionListCssClass : function() {
return this._completionListCssClass;},
set_completionListCssClass : function(value) {
if (this._completionListCssClass != value) {
this._completionListCssClass = value;this.raisePropertyChanged('completionListCssClass');}
}, 
get_completionListItemCssClass : function() {
return this._completionListItemCssClass;},
set_completionListItemCssClass : function(value) {
if (this._completionListItemCssClass != value) {
this._completionListItemCssClass = value;this.raisePropertyChanged('completionListItemCssClass');}
},
get_highlightedItemCssClass : function() {
return this._highlightedItemCssClass;},
set_highlightedItemCssClass : function(value) {
if(this._highlightedItemCssClass != value) {
this._highlightedItemCssClass = value;this.raisePropertyChanged('highlightedItemCssClass');}
},
get_delimiterCharacters: function() {
return this._delimiterCharacters;},
set_delimiterCharacters: function(value) {
if (this._delimiterCharacters != value) {
this._delimiterCharacters = value;this.raisePropertyChanged('delimiterCharacters');}
},
get_firstRowSelected:function() {
return this._firstRowSelected;},
set_firstRowSelected:function(value) {
if(this._firstRowSelected != value) {
this._firstRowSelected = value;this.raisePropertyChanged('firstRowSelected');}
},
get_showOnlyCurrentWordInCompletionListItem:function() {
return this._showOnlyCurrentWordInCompletionListItem;},
set_showOnlyCurrentWordInCompletionListItem:function(value) {
if(this._showOnlyCurrentWordInCompletionListItem != value) {
this._showOnlyCurrentWordInCompletionListItem = value;this.raisePropertyChanged('showOnlyCurrentWordInCompletionListItem');}
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
},
add_showing : function(handler) {
this.get_events().addHandler('showing', handler);},
remove_showing : function(handler) {
this.get_events().removeHandler('showing', handler);},
raiseShowing : function(eventArgs) {
var handler = this.get_events().getHandler('showing');if (handler) {
handler(this, eventArgs);}
},
add_shown : function(handler) {
this.get_events().addHandler('shown', handler);},
remove_shown : function(handler) {
this.get_events().removeHandler('shown', handler);},
raiseShown : function(eventArgs) {
var handler = this.get_events().getHandler('shown');if (handler) {
handler(this, eventArgs);}
},
add_hiding : function(handler) {
this.get_events().addHandler('hiding', handler);},
remove_hiding : function(handler) {
this.get_events().removeHandler('hiding', handler);},
raiseHiding : function(eventArgs) {
var handler = this.get_events().getHandler('hiding');if (handler) {
handler(this, eventArgs);}
},
add_hidden : function(handler) {
this.get_events().addHandler('hidden', handler);},
remove_hidden : function(handler) {
this.get_events().removeHandler('hidden', handler);},
raiseHidden : function(eventArgs) {
var handler = this.get_events().getHandler('hidden');if (handler) {
handler(this, eventArgs);}
},
add_itemSelected : function(handler) {
this.get_events().addHandler('itemSelected', handler);},
remove_itemSelected : function(handler) {
this.get_events().removeHandler('itemSelected', handler);},
raiseItemSelected : function(eventArgs) {
var handler = this.get_events().getHandler('itemSelected');if (handler) {
handler(this, eventArgs);}
},
add_itemOver : function(handler) {
this.get_events().addHandler('itemOver', handler);},
remove_itemOver : function(handler) {
this.get_events().removeHandler('itemOver', handler);},
raiseItemOver : function(eventArgs) {
var handler = this.get_events().getHandler('itemOver');if (handler) {
handler(this, eventArgs);}
},
add_itemOut : function(handler) {
this.get_events().addHandler('itemOut', handler);},
remove_itemOut : function(handler) {
this.get_events().removeHandler('itemOut', handler);},
raiseItemOut : function(eventArgs) {
var handler = this.get_events().getHandler('itemOut');if (handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit.AutoCompleteBehavior.registerClass('AjaxControlToolkit.AutoCompleteBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.AutoCompleteBehavior.descriptor = {
properties: [ {name: 'completionInterval', type: Number},
{name: 'completionList', isDomElement: true},
{name: 'completionListElementID', type: String},
{name: 'completionSetCount', type: Number},
{name: 'minimumPrefixLength', type: Number},
{name: 'serviceMethod', type: String},
{name: 'servicePath', type: String},
{name: 'enableCaching', type: Boolean},
{name: 'showOnlyCurrentWordInCompletionListItem', type: Boolean} ]
}
AjaxControlToolkit.AutoCompleteItemEventArgs = function(item, text, value) {
AjaxControlToolkit.AutoCompleteItemEventArgs.initializeBase(this);this._item = item;this._text = text;this._value = (value !== undefined) ? value : null;}
AjaxControlToolkit.AutoCompleteItemEventArgs.prototype = {
get_item : function() {
return this._item;},
set_item : function(value) {
this._item = value;},
get_text : function() {
return this._text;},
set_text : function(value) {
this._text = value;},
get_value : function() {
return this._value;},
set_value : function(value) {
this._value = value;}
}
AjaxControlToolkit.AutoCompleteItemEventArgs.registerClass('AjaxControlToolkit.AutoCompleteItemEventArgs', Sys.EventArgs);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
