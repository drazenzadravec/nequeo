Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.PagingBulletedListBehavior = function(element) {
AjaxControlToolkit.PagingBulletedListBehavior.initializeBase(this, [element]);this._indexSizeValue = 1;this._separatorValue = ' - ';this._heightValue = null;this._maxItemPerPage = null;this._clientSortValue = false;this._selectIndexCssClassValue = null;this._unselectIndexCssClassValue = null;this._tabValue = new Array();this._tabValueObject = new Array();this._tabIndex = new Array();this._divContent = null;this._divContentIndex = null;this._divContentUl = null;this._prevIndexSelected = null;this._indexSelected = 0;this._clickIndex = null;}
AjaxControlToolkit.PagingBulletedListBehavior.prototype = {
initialize : function() {
AjaxControlToolkit.PagingBulletedListBehavior.callBaseMethod(this, 'initialize');var clientState = this.get_ClientState();if (clientState){
var stateItems = clientState.split(";");if (stateItems.length) {
this._indexSelected = stateItems[0];if (stateItems[1] == "null")
this._indexSizeValue = null;else
this._indexSizeValue = stateItems[1];if (stateItems[2] == "null")
this._maxItemPerPage = null;else
this._maxItemPerPage = stateItems[2];if (stateItems[3] == "true"){
this._clientSortValue = true;}else{
this._clientSortValue = false;} 
}
} 
var e = this.get_element();this._divContent = document.createElement('div');e.parentNode.insertBefore(this._divContent, e);var liElements = e.childNodes;this._clickIndex = Function.createDelegate(this, this._onIndexClick);var inner;var index;this._divContentIndex = document.createElement('DIV');this._divContentIndex.style.marginBottom = '5px';this._divContent.appendChild(this._divContentIndex);for (var i = 0 ;i < liElements.length;i++) {
if (liElements[i].nodeName == 'LI') {
if ((liElements[i].firstChild) && (liElements[i].firstChild.innerHTML)) {
inner = liElements[i].firstChild.innerHTML;} else {
inner = liElements[i].innerHTML;}
this._tabValueObject[this._tabValueObject.length] = {text : inner, obj : liElements[i], index : i};}
}
if(this._clientSortValue) {
this._tabValueObject.sort(this.liElementSortText);}
this._generateIndexAndTabForView();this._removeChilds(e.childNodes);this._divContentUl = document.createElement('DIV');this._changeHeightDivContent();this._divContentUl.appendChild(e);this._divContent.appendChild(this._divContentUl);this._updateIndexAndView(this._indexSelected);},
_changeHeightDivContent : function() {
if (this._heightValue) {
this._divContentUl.style.overflow = 'scroll';this._divContentUl.style.height = (this._heightValue) + 'px';} else {
this._divContentUl.style.overflow = '';this._divContentUl.style.height = '';}
},
_createAHrefIndex : function(indexText, indexNumber) {
var spanSeparator;var aIndex;aIndex = document.createElement('a');aIndex.href = '';Sys.UI.DomElement.addCssClass(aIndex, this._unselectIndexCssClassValue);aIndex.innerHTML = indexText;aIndex.tag = indexNumber;$addHandler(aIndex, 'click',this._clickIndex);this._tabIndex[this._tabIndex.length] = aIndex;this._divContentIndex.appendChild(aIndex);spanSeparator = document.createElement('SPAN');spanSeparator.innerHTML = '\uFEFF' + this._separatorValue + '\uFEFF';this._divContentIndex.appendChild(spanSeparator);return spanSeparator;},
liElementSortText : function(x, y) {
if (x.text.toLowerCase() == y.text.toLowerCase()) {
return 0;} else {
if (x.text.toLowerCase() < y.text.toLowerCase()) {
return -1;} else {
return 1;}
}
},
liElementSortIndex : function(x, y) {
return x.index - y.index;},
_generateIndexAndTabForView : function() {
this._deleteTabIndexAndTabValue();this._tabValue = new Array();this._tabIndex = new Array();var lastSpanSeparator;this._removeChilds(this._divContentIndex.childNodes);if(this._maxItemPerPage) {
if (this._maxItemPerPage > 0) {
var j = -1;for(var i = 0;i < this._tabValueObject.length;i++) {
if((i % this._maxItemPerPage) == 0) {
j++;index = this._tabValueObject[i].text;this._tabValue[j] = new Array();lastSpanSeparator = this._createAHrefIndex(index, j);}
this._tabValue[j][this._tabValue[j].length] = this._tabValueObject[i].obj;}
}
} else {
if (this._indexSizeValue > 0) {
var currentIndex = '';var j = -1;for(var i = 0;i < this._tabValueObject.length;i++) {
index = this._tabValueObject[i].text.substr(0, this._indexSizeValue).toUpperCase();if (currentIndex != index) {
j++;this._tabValue[j] = new Array();lastSpanSeparator = this._createAHrefIndex(index, j);currentIndex = index;}
this._tabValue[j][this._tabValue[j].length] = this._tabValueObject[i].obj;}
}
}
if (lastSpanSeparator) {
this._divContentIndex.removeChild(lastSpanSeparator);}
},
_deleteTabIndexAndTabValue : function() {
if (this._clickIndex) {
for(var i = 0;i < this._tabIndex.length;i++) {
var aIndex = this._tabIndex[i];if(aIndex) {
$removeHandler(aIndex, 'click', this._clickIndex);}
}
this._changeHandler = null;}
delete this._tabIndex;for(var i = 0;i < this._tabValue.length;i++) {
delete this._tabValue[i];}
delete this._tabValue;},
dispose : function() {
this._deleteTabIndexAndTabValue();delete this._tabValueObject;AjaxControlToolkit.PagingBulletedListBehavior.callBaseMethod(this, 'dispose');},
_removeChilds : function(eChilds) {
for(var i = 0;eChilds.length;i++) {
eChilds[0].parentNode.removeChild(eChilds[0]);}
},
_renderHtml : function(index) {
var e = this.get_element();this._removeChilds(e.childNodes);for(var i = 0;i<this._tabValue[index].length;i++) {
e.appendChild(this._tabValue[index][i]);}
this._divContentUl.scrollTop = 0;},
_selectIndex : function(index) {
if (this._tabIndex.length > 0) {
Sys.UI.DomElement.removeCssClass(this._tabIndex[index], this._unselectIndexCssClassValue);Sys.UI.DomElement.addCssClass(this._tabIndex[index], this._selectIndexCssClassValue);this._prevIndexSelected = this._tabIndex[index];this.raiseIndexChanged(this._tabIndex[index]);}
},
_onIndexClick : function(evt) {
var e = this.get_element();var aIndex = evt.target;Sys.UI.DomElement.removeCssClass(this._prevIndexSelected, this._selectIndexCssClassValue);Sys.UI.DomElement.addCssClass(this._prevIndexSelected, this._unselectIndexCssClassValue);Sys.UI.DomElement.removeCssClass(aIndex, this._unselectIndexCssClassValue);Sys.UI.DomElement.addCssClass(aIndex, this._selectIndexCssClassValue);this._prevIndexSelected = aIndex;this._renderHtml(aIndex.tag);this.raiseIndexChanged(aIndex);evt.preventDefault();},
add_indexChanged : function(handler) {
this.get_events().addHandler('indexChanged', handler);},
remove_indexChanged : function(handler) {
this.get_events().removeHandler('indexChanged', handler);},
raiseIndexChanged : function(eventArgs) {
this._indexSelected = eventArgs.tag;var handler = this.get_events().getHandler('indexChanged');if (handler) {
if (!eventArgs) {
eventArgs = Sys.EventArgs.Empty;}
handler(this, eventArgs);}
this.set_ClientState(eventArgs.tag+";"+this.get_IndexSize()+";"+this.get_MaxItemPerPage()+";"+this.get_ClientSort());},
get_tabIndex : function() {
return this._tabIndex;},
get_tabValue : function() {
return this._tabValue;},
_updateIndexAndView : function(index) {
this._generateIndexAndTabForView()
if (this._tabIndex.length > 0) {
if (index < this._tabIndex.length) {
this._renderHtml(this._tabIndex[index].tag);this._selectIndex(index);} else {
this._renderHtml(this._tabIndex[0].tag);this._selectIndex(0);}
}
},
get_Height : function() {
return this._heightValue;},
set_Height : function(value) {
if (this._heightValue != value) {
this._heightValue = value;if (this.get_isInitialized()) {
this._changeHeightDivContent();}
this.raisePropertyChanged('Height');}
},
get_IndexSize : function() {
return this._indexSizeValue;},
set_IndexSize : function(value) {
if (this._indexSizeValue != value) {
this.set_ClientState("0;"+value+";"+this.get_MaxItemPerPage()+";"+this.get_ClientSort());this._indexSizeValue = value;if (this.get_isInitialized()) {
this._updateIndexAndView(0);}
this.raisePropertyChanged('IndexSize');}
},
get_MaxItemPerPage : function() {
return this._maxItemPerPage;},
set_MaxItemPerPage : function(value) {
if(this._maxItemPerPage != value) {
this.set_ClientState("0;"+this.get_IndexSize()+";"+value+";"+this.get_ClientSort());this._maxItemPerPage = value;if (this.get_isInitialized()) {
this._updateIndexAndView(0);}
this.raisePropertyChanged('MaxItemPerPage');}
},
get_Separator : function() {
return this._separatorValue;},
set_Separator : function(value) {
if (this._separatorValue != value) {
if (value) {
this._separatorValue = value;} else {
this._separatorValue = '';} 
if (this.get_isInitialized()) {
this._updateIndexAndView(0);}
this.raisePropertyChanged('Separator');}
},
get_ClientSort : function() {
return this._clientSortValue;},
set_ClientSort : function(value) {
if (this._clientSortValue != value) {
this.set_ClientState("0;"+this.get_IndexSize()+";"+this.get_MaxItemPerPage()+";"+value);this._clientSortValue = value;if (this.get_isInitialized()) {
if (this._clientSortValue)
this._tabValueObject.sort(this.liElementSortText);else
this._tabValueObject.sort(this.liElementSortIndex);this._updateIndexAndView(0);} 
this.raisePropertyChanged('ClientSort');}
},
get_SelectIndexCssClass : function() {
return this._selectIndexCssClassValue;},
set_SelectIndexCssClass : function(value) {
if (this._selectIndexCssClassValue != value) {
this._selectIndexCssClassValue = value;this.raisePropertyChanged('SelectIndexCssClass');}
},
get_UnselectIndexCssClass : function() {
return this._unselectIndexCssClassValue;},
set_UnselectIndexCssClass : function(value) {
if (this._unselectIndexCssClassValue != value) {
this._unselectIndexCssClassValue = value;this.raisePropertyChanged('UnselectIndexCssClass');}
}
}
AjaxControlToolkit.PagingBulletedListBehavior.registerClass('AjaxControlToolkit.PagingBulletedListBehavior', AjaxControlToolkit.BehaviorBase);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
