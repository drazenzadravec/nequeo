Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.AutoSize = function() {
throw Error.invalidOperation();}
AjaxControlToolkit.AutoSize.prototype = {
None : 0,
Fill : 1,
Limit : 2
}
AjaxControlToolkit.AutoSize.registerEnum("AjaxControlToolkit.AutoSize", false);AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs = function(oldIndex, selectedIndex) {
AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs.initializeBase(this);this._oldIndex = oldIndex;this._selectedIndex = selectedIndex;}
AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs.prototype = {
get_oldIndex : function() {
return this._oldIndex;},
set_oldIndex : function(value) {
this._oldIndex = value;},
get_selectedIndex : function() {
return this._selectedIndex;},
set_selectedIndex : function(value) {
this._selectedIndex = value;}
}
AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs.registerClass('AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs', Sys.CancelEventArgs);AjaxControlToolkit.AccordionBehavior = function(element) {
AjaxControlToolkit.AccordionBehavior.initializeBase(this, [element]);this._selectedIndex = 0;this._panes = [];this._fadeTransitions = false;this._duration = 0.25;this._framesPerSecond = 30;this._autoSize = AjaxControlToolkit.AutoSize.None;this._requireOpenedPane = true;this._suppressHeaderPostbacks = false;this._headersSize = 0;this._headerClickHandler = null;this._headerCssClass = '';this._headerSelectedCssClass = '';this._contentCssClass = '';this._resizeHandler = null;}
AjaxControlToolkit.AccordionBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.AccordionBehavior.callBaseMethod(this, 'initialize');this._headerClickHandler = Function.createDelegate(this, this._onHeaderClick);var state = this.get_ClientState();if (state !== null && state !== '') {
this._changeSelectedIndex(parseInt(state), false, true);}
var nodes = this.get_element().childNodes;var index = { };for (index.value = 0;index.value < nodes.length;index.value++) {
var header = this._getNextDiv(nodes, index);if (!header) {
break;}
var content = this._getNextDiv(nodes, index);if (content) {
this.addPane(header, content);index.value--;}
}
if (this._requireOpenedPane && !this.get_Pane() && this._panes.length > 0) {
this._changeSelectedIndex(0, false, true);}
this._initializeLayout();},
_getNextDiv : function(nodes, index) {
var div = null;while (index.value < nodes.length && (div = nodes[index.value++])) {
if (div.tagName && (div.tagName.toLowerCase() === 'div')) {
break;}
}
return div;},
addPane : function(header, content) {
var pane = { };pane.animation = null;pane.header = header;header._index = this._panes.length;$addHandler(header, "click", this._headerClickHandler);var accordion = this.get_element();var wrapper = document.createElement('div');accordion.insertBefore(wrapper, content);wrapper.appendChild(content);wrapper._original = content;pane.content = wrapper;wrapper.style.border = '';wrapper.style.margin = '';wrapper.style.padding = '';Array.add(this._panes, pane);this._initializePane(header._index);content.style.display = 'block';return pane;},
_getAnimation : function(pane) {
var animation = pane.animation;if (!animation) {
var length = null;var fade = null;if (!this._fadeTransitions) {
animation = length = new AjaxControlToolkit.Animation.LengthAnimation(pane.content, this._duration, this._framesPerSecond, "style", "height", 0, 0, "px");} else {
length = new AjaxControlToolkit.Animation.LengthAnimation(null, null, null, "style", "height", 0, 0, "px");fade = new AjaxControlToolkit.Animation.FadeAnimation(null, null, null, AjaxControlToolkit.Animation.FadeEffect.FadeOut, 0, 1, false);animation = new AjaxControlToolkit.Animation.ParallelAnimation(pane.content, this._duration, this._framesPerSecond, [fade, length]);}
pane.animation = animation;animation._length = length;animation._fade = fade;animation._pane = pane;animation._opening = true;animation._behavior = this;animation._ended = Function.createDelegate(pane.animation, this._onAnimationFinished);animation.add_ended(pane.animation._ended);animation.initialize();}
return animation;},
_onAnimationFinished : function() {
this._behavior._endPaneChange(this._pane, this._opening);},
_initializeLayout : function() {
for (var i = 0;i < this._panes.length;i++) {
var animation = this._panes[i].animation;if (animation && animation.get_isPlaying()) {
animation.stop();}
}
var accordion = this.get_element();this._initialHeight = accordion.offsetHeight;var style = accordion.style;if (this._autoSize === AjaxControlToolkit.AutoSize.None) {
this._disposeResizeHandler();var isIE7 = (Sys.Browser.agent == Sys.Browser.InternetExplorer && Sys.Browser.version >= 7)
if (!isIE7 || (isIE7 && style.height && style.height.length > 0)) {
style.height = 'auto';}
if (!isIE7 || (isIE7 && style.overflow && style.overflow.length > 0)) { 
style.overflow = 'auto';}
} else {
this._addResizeHandler();style.height = accordion.offsetHeight + 'px';style.overflow = 'hidden';}
for (var i = 0;i < this._panes.length;i++) {
this._initializePane(i);}
this._resizeSelectedPane();},
_initializePane : function(index) {
var pane = this.get_Pane(index);if (!pane) {
return;}
var wrapper = pane.content;var original = wrapper._original;var opened = (index === this._selectedIndex);wrapper.style.height = (opened || (this._autoSize === AjaxControlToolkit.AutoSize.Fill)) ? 'auto' : '0px';wrapper.style.overflow = opened ? 'auto' : 'hidden';wrapper.style.display = opened ? 'block' : 'none';original.style.height = 'auto';original.style.maxHeight = '';original.style.overflow = opened ? 'auto' : 'hidden';var opacity = (opened || !this._fadeTransitions) ? 1 : 0;if (this._autoSize === AjaxControlToolkit.AutoSize.Fill) {
if ($common.getElementOpacity(original) != opacity) {
$common.setElementOpacity(original, opacity);}
if ($common.getElementOpacity(wrapper) != 1) {
$common.setElementOpacity(wrapper, 1);}
} else {
if ($common.getElementOpacity(wrapper) != opacity) {
$common.setElementOpacity(wrapper, opacity);}
if ($common.getElementOpacity(original) != 1) {
$common.setElementOpacity(original, 1);}
}
},
_addResizeHandler : function() {
if (!this._resizeHandler) {
this._resizeHandler = Function.createDelegate(this, this._resizeSelectedPane);$addHandler(window, "resize", this._resizeHandler);}
},
dispose : function() {
this._disposeResizeHandler();this._disposeAnimations();for (var i = this._panes.length - 1;i >= 0;i--) {
var pane = this._panes[i];if (pane) {
if (pane.header) {
pane.header._index = null;$removeHandler(pane.header, "click", this._headerClickHandler);pane.header = null;}
if (pane.content) {
pane.content._original = null;pane.content = null;}
this._panes[i] = null;delete this._panes[i];}
}
this._panes = null;this._headerClickHandler = null;AjaxControlToolkit.AccordionBehavior.callBaseMethod(this, 'dispose');},
_disposeResizeHandler : function() {
if (this._resizeHandler) {
$removeHandler(window, "resize", this._resizeHandler);this._resizeHandler = null;}
},
_disposeAnimations : function() {
for (var i = 0;i < this._panes.length;i++) {
var animation = this._panes[i].animation;if (animation) {
if (animation.get_isPlaying()) {
animation.stop();}
if (animation._ended) {
animation.remove_ended(animation._ended);animation._ended = null;}
animation.dispose();animation._length = null;animation._fade = null;animation._pane = null;animation._opening = null;animation._behavior = null;this._panes[i].animation = null;}
}
},
_resizeSelectedPane : function() {
var pane = this.get_Pane();if (!pane) {
return;}
this._headersSize = this._getHeadersSize().height;var original = pane.content._original;switch (this._autoSize) {
case AjaxControlToolkit.AutoSize.None :
original.style.height = 'auto';original.style.maxHeight = '';break;case AjaxControlToolkit.AutoSize.Limit :
var remaining = this._getRemainingHeight(false);original.style.height = 'auto';original.style.maxHeight = remaining + 'px';break;case AjaxControlToolkit.AutoSize.Fill :
var remaining = this._getRemainingHeight(true);original.style.height = remaining + 'px';original.style.maxHeight = '';break;} 
},
_onHeaderClick : function(evt) {
var header = evt.target;var accordion = this.get_element();while (header && (header.parentNode !== accordion)) {
header = header.parentNode;}
evt.stopPropagation();if (this._suppressHeaderPostbacks) {
evt.preventDefault();}
var index = header._index;if ((index === this._selectedIndex) && !this._requireOpenedPane) {
index = -1;}
this._changeSelectedIndex(index, true);},
_changeSelectedIndex : function(index, animate, force) {
var lastIndex = this._selectedIndex;var currentPane=this.get_Pane(index);var lastPane=this.get_Pane(lastIndex);if (!force && (currentPane == lastPane)) {
return;}
var eventArgs = new AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs(lastIndex, index);this.raiseSelectedIndexChanging(eventArgs);if (eventArgs.get_cancel()) {
return;}
if(lastPane)
{
lastPane.header.className = this._headerCssClass;}
if(currentPane)
{
currentPane.header.className = (this._headerSelectedCssClass == '') ? 
this._headerCssClass : this._headerSelectedCssClass;}
this._selectedIndex = index;this.set_ClientState(this._selectedIndex);if (animate) {
this._changePanes(lastIndex);}
this.raiseSelectedIndexChanged(new AjaxControlToolkit.AccordionSelectedIndexChangeEventArgs(lastIndex, index));this.raisePropertyChanged('SelectedIndex');},
_changePanes : function(lastIndex) {
if (!this.get_isInitialized()) {
return;}
var open = null;var close = null;for (var i = 0;i < this._panes.length;i++) {
var pane = this._panes[i];var animation = this._getAnimation(pane);if (animation.get_isPlaying()) {
animation.stop();}
if (i == this._selectedIndex) {
animation._opening = true;open = animation;} else if (i == lastIndex) {
animation._opening = false;close = animation;} else {
continue;}
this._startPaneChange(pane, animation._opening);if (this._fadeTransitions) {
animation._fade.set_effect(animation._opening ? AjaxControlToolkit.Animation.FadeEffect.FadeIn : AjaxControlToolkit.Animation.FadeEffect.FadeOut );}
if (this._autoSize === AjaxControlToolkit.AutoSize.Fill) {
animation.set_target(pane.content._original);animation._length.set_startValue($common.getContentSize(pane.content._original).height);animation._length.set_endValue(animation._opening ? this._getRemainingHeight(true) : 0);} else {
animation.set_target(pane.content);animation._length.set_startValue(pane.content.offsetHeight);animation._length.set_endValue(animation._opening ? this._getRemainingHeight(false) : 0);}
}
if (close) {
close.play();}
if (open) {
open.play();}
},
_startPaneChange : function(pane, opening) {
var wrapper = pane.content;var original = wrapper._original;if (opening) {
wrapper.style.display = 'block';} else {
wrapper.style.overflow = 'hidden';original.style.overflow = 'hidden';if (this._autoSize === AjaxControlToolkit.AutoSize.Limit) {
wrapper.style.height = this._getTotalSize(original).height + 'px';original.style.maxHeight = '';}
}
},
_endPaneChange : function(pane, opening) {
var wrapper = pane.content;var original = wrapper._original;if (opening) {
if (this._autoSize === AjaxControlToolkit.AutoSize.Limit) {
var remaining = this._getRemainingHeight(true);original.style.maxHeight = remaining + 'px';}
original.style.overflow = 'auto';wrapper.style.height = 'auto';wrapper.style.overflow = 'auto';} else {
wrapper.style.display = 'none';}
},
_getHeadersSize : function() {
var total = { width: 0, height: 0 };for (var i = 0;i < this._panes.length;i++) {
var size = this._getTotalSize(this._panes[i].header);total.width = Math.max(total.width, size.width);total.height += size.height;}
return total;},
_getRemainingHeight : function(includeGutter) {
var height = 0;var pane = this.get_Pane();if (this._autoSize === AjaxControlToolkit.AutoSize.None) {
if (pane) { 
height = this._getTotalSize(pane.content._original).height;}
} else {
height = this._headersSize;if (includeGutter && pane) {
height += this._getGutterSize(pane.content._original).height;}
var accordion = this.get_element();height = Math.max(accordion.offsetHeight - height, 0);if (pane && (this._autoSize === AjaxControlToolkit.AutoSize.Limit)) {
var required = this._getTotalSize(pane.content._original).height;if (required > 0) {
height = Math.min(height, required);}
}
}
return height;},
_getTotalSize : function(element) {
var size = $common.getSize(element);var box = $common.getMarginBox(element);size.width += box.horizontal;size.height += box.vertical;return size;},
_getGutterSize : function(element) {
var gutter = { width: 0, height: 0 };try {
var box = $common.getPaddingBox(element);gutter.width += box.horizontal;gutter.height += box.vertical;} catch(ex) { }
try {
var box = $common.getBorderBox(element);gutter.width += box.horizontal;gutter.height += box.vertical;} catch(ex) { }
var box = $common.getMarginBox(element);gutter.width += box.horizontal;gutter.height += box.vertical;return gutter;},
add_selectedIndexChanging : function(handler) {
this.get_events().addHandler('selectedIndexChanging', handler);},
remove_selectedIndexChanging : function(handler) {
this.get_events().removeHandler('selectedIndexChanging', handler);},
raiseSelectedIndexChanging : function(eventArgs) {
var handler = this.get_events().getHandler('selectedIndexChanging');if (handler) {
handler(this, eventArgs);}
},
add_selectedIndexChanged : function(handler) {
this.get_events().addHandler('selectedIndexChanged', handler);},
remove_selectedIndexChanged : function(handler) {
this.get_events().removeHandler('selectedIndexChanged', handler);},
raiseSelectedIndexChanged : function(eventArgs) {
var handler = this.get_events().getHandler('selectedIndexChanged');if (handler) {
handler(this, eventArgs);}
},
get_Pane : function(index) {
if (index === undefined || index === null) {
index = this._selectedIndex;}
return (this._panes && index >= 0 && index < this._panes.length) ? this._panes[index] : null;}, 
get_Count : function() {
return this._panes ? this._panes.length : 0;},
get_TransitionDuration : function() {
return this._duration * 1000;},
set_TransitionDuration : function(value) {
if (this._duration != (value / 1000)) {
this._duration = value / 1000;for (var i = 0;i < this._panes.length;i++) {
var animation = this._panes[i].animation;if (animation) {
animation.set_duration(this._duration);}
}
this.raisePropertyChanged('TransitionDuration');}
},
get_FramesPerSecond : function() {
return this._framesPerSecond;},
set_FramesPerSecond : function(value) {
if (this._framesPerSecond != value) {
this._framesPerSecond = value;for (var i = 0;i < this._panes.length;i++) {
var animation = this._panes[i].animation;if (animation) {
animation.set_fps(this._framesPerSecond);}
}
this.raisePropertyChanged('FramesPerSecond');}
},
get_FadeTransitions : function() {
return this._fadeTransitions;},
set_FadeTransitions : function(value) {
if (this._fadeTransitions != value) {
this._fadeTransitions = value;this._disposeAnimations();if (!this._fadeTransitions) {
for (var i = 0;i < this._panes.length;i++) {
if ($common.getElementOpacity(this._panes[i].content) != 1) {
$common.setElementOpacity(this._panes[i].content, 1);}
if ($common.getElementOpacity(this._panes[i].content._original) != 1) {
$common.setElementOpacity(this._panes[i].content._original, 1);}
}
}
this.raisePropertyChanged('FadeTransitions');}
},
get_HeaderCssClass: function() {
return this._headerCssClass;},
set_HeaderCssClass: function(value) {
this._headerCssClass = value;this.raisePropertyChanged('HeaderCssClass');},
get_HeaderSelectedCssClass: function() {
return this._headerSelectedCssClass;},
set_HeaderSelectedCssClass: function(value) {
this._headerSelectedCssClass = value;this.raisePropertyChanged('HeaderSelectedCssClass');}, 
get_ContentCssClass: function() {
return this._contentCssClass;},
set_ContentCssClass: function(value) {
this._contentCssClass = value;this.raisePropertyChanged('ContentCssClass');}, 
get_AutoSize : function() {
return this._autoSize;},
set_AutoSize : function(value) {
if (Sys.Browser.agent === Sys.Browser.InternetExplorer && value === AjaxControlToolkit.AutoSize.Limit) {
value = AjaxControlToolkit.AutoSize.Fill;}
if (this._autoSize != value) {
this._autoSize = value;this._initializeLayout();this.raisePropertyChanged('AutoSize');}
},
get_SelectedIndex : function() {
return this._selectedIndex;},
set_SelectedIndex : function(value) {
this._changeSelectedIndex(value, true);},
get_requireOpenedPane : function() {
return this._requireOpenedPane;},
set_requireOpenedPane : function(value) {
if (this._requireOpenedPane != value) {
this._requireOpenedPane = value;this.raisePropertyChanged('requireOpenedPane');}
},
get_suppressHeaderPostbacks : function() {
return this._suppressHeaderPostbacks;},
set_suppressHeaderPostbacks : function(value) {
if (this._suppressHeaderPostbacks != value) {
this._suppressHeaderPostbacks = value;this.raisePropertyChanged('suppressHeaderPostbacks');}
}
}
AjaxControlToolkit.AccordionBehavior.registerClass('AjaxControlToolkit.AccordionBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
