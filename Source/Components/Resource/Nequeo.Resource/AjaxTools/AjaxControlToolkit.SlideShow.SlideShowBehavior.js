Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.SlideShowBehavior = function(element) {
AjaxControlToolkit.SlideShowBehavior.initializeBase(this, [element]);this._nextButtonID = null;this._previousButtonID = null;this._imageDescriptionLabelID = null;this._imageTitleLabelID = null;this._playButtonID = null;this._playButtonValue = '||>';this._stopButtonValue = '[]';this._slideShowServicePath = null;this._slideShowServiceMethod = null;this._contextKey = null;this._useContextKey = false;this._playInterval = 3000;this._tickHandler = null;this._loop = false;this._autoPlay = false;this._inPlayMode = false;this._elementImage = null;this._bNext = null;this._bPrevious = null;this._currentIndex = -1;this._currentValue = null;this._imageDescriptionLabel = null;this._imageTitleLabel = null;this._bPlay = null;this._slides = null;this._timer = null;this._currentImageElement = null;this._images = null;this._cachedImageIndex = -1;this._clickNextHandler = null;this._clickPreviousHandler = null;this._clickPlayHandler = null;this._tickHandler = null;this._imageLoadedHandler = null;}
AjaxControlToolkit.SlideShowBehavior.prototype = { 
initialize : function() {
AjaxControlToolkit.SlideShowBehavior.callBaseMethod(this, 'initialize');var e = this.get_element();this._elementImage = e;this._currentImageElement = document.createElement('IMG');this._currentImageElement.style.display = 'none';document.body.appendChild(this._currentImageElement);var _divContent = document.createElement('DIV');e.parentNode.insertBefore(_divContent, e);e.parentNode.removeChild(e);_divContent.appendChild(e);_divContent.align = 'center';this.controlsSetup();if (this._bNext) {
this._clickNextHandler = Function.createDelegate(this, this._onClickNext);$addHandler(this._bNext, 'click', this._clickNextHandler);}
if (this._bPrevious) {
this._clickPreviousHandler = Function.createDelegate(this, this._onClickPrevious);$addHandler(this._bPrevious, 'click', this._clickPreviousHandler);} 
if (this._bPlay) {
this._clickPlayHandler = Function.createDelegate(this, this._onClickPlay);$addHandler(this._bPlay, 'click', this._clickPlayHandler);} 
this._imageLoadedHandler = Function.createDelegate(this, this._onImageLoaded);$addHandler(this._currentImageElement, 'load', this._imageLoadedHandler);this._slideShowInit();},
dispose : function() {
if (this._clickNextHandler) {
$removeHandler(this._bNext, 'click', this._clickNextHandler);this._clickNextHandler = null;}
if (this._clickPreviousHandler) {
$removeHandler(this._bPrevious, 'click', this._clickPreviousHandler);this._clickPreviousHandler = null;}
if (this._clickPlayHandler) {
$removeHandler(this._bPlay, 'click', this._clickPlayHandler);this._clickPlayHandler = null;} 
if (this._imageLoadedHandler) {
$removeHandler(this._currentImageElement, 'load', this._imageLoadedHandler);this._imageLoadedHandler = null;}
if (this._timer) {
this._timer.dispose();this._timer = null;}
AjaxControlToolkit.SlideShowBehavior.callBaseMethod(this, 'dispose');},
add_slideChanged : function(handler) {
this.get_events().addHandler('slideChanged', handler);},
remove_slideChanged : function(handler) {
this.get_events().removeHandler('slideChanged', handler);},
raiseSlideChanged : function(eventArgs) {
var handler = this.get_events().getHandler('slideChanged');if (handler) {
if (!eventArgs) {
eventArgs = Sys.EventArgs.Empty;}
handler(this, eventArgs);}
},
add_slideChanging : function(handler) {
this.get_events().addHandler('slideChanging', handler);},
remove_slideChanging : function(handler) {
this.get_events().removeHandler('slideChanging', handler);},
raiseSlideChanging : function(previousSlide, newSlide) {
var handler = this.get_events().getHandler('slideChanging');if (handler) {
var eventArgs = new AjaxControlToolkit.SlideShowEventArgs(previousSlide, newSlide, this._currentIndex);handler(this, eventArgs);return eventArgs.get_cancel();}
return false;},
get_contextKey : function() {
return this._contextKey;},
set_contextKey : function(value) {
if (this._contextKey != value) {
this._contextKey = value;this.set_useContextKey(true);if(this._elementImage) {
this._slideShowInit();}
this.raisePropertyChanged('contextKey');}
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
this._enableCaching = value;},
get_completionListElementID: function() {
return this._completionListElementID;},
set_completionListElementID: function(value) {
this._completionListElementID = value;}, 
get_completionListCssClass : function() {
this._completionListCssClass;},
set_completionListCssClass : function(value) {
if(this._completionListCssClass != value) {
this._completionListCssClass = value;this.raisePropertyChanged('completionListCssClass');}
}, 
get_completionListItemCssClass : function() {
this._completionListItemCssClass;},
set_completionListItemCssClass : function(value) {
if(this._completionListItemCssClass != value) {
this._completionListItemCssClass = value;this.raisePropertyChanged('completionListItemCssClass');}
}, 
get_highlightedItemCssClass : function() {
this._highlightedItemCssClass;},
set_highlightedItemCssClass : function(value) {
if(this._highlightedItemCssClass != value) { 
this._highlightedItemCssClass = value;this.raisePropertyChanged('highlightedItemCssClass');}
}, 
get_delimiterCharacters: function() {
return this._delimiterCharacters;},
set_delimiterCharacters: function(value) {
this._delimiterCharacters = value;},
controlsSetup : function() {
if (this._previousButtonID) {
this._bPrevious = document.getElementById(this._previousButtonID);}
if (this._imageDescriptionLabelID) {
this._imageDescriptionLabel = document.getElementById(this._imageDescriptionLabelID);}
if (this._imageTitleLabelID) {
this._imageTitleLabel = document.getElementById(this._imageTitleLabelID);}
if (this._nextButtonID) {
this._bNext = document.getElementById(this._nextButtonID);}
if (this._playButtonID) {
this._bPlay = document.getElementById(this._playButtonID);this._bPlay.value = this._playButtonValue;}
},
resetButtons : function() {
if (!this._loop) {
if (this._slides.length <= this._currentIndex + 1 ) {
if (this._bNext) this._bNext.disabled = true;if (this._bPlay) this._bPlay.disabled = true;if (this._bPrevious) this._bPrevious.disabled = false;this._inPlayMode = false;if (this._timer) {
this._timer.set_enabled(false);}
if (this._bPlay) this._bPlay.value = this._playButtonValue;} else {
if (this._bNext) this._bNext.disabled = false;if (this._bPlay) this._bPlay.disabled = false;}
if (this._currentIndex <= 0) {
if (this._bPrevious) this._bPrevious.disabled = true;} else {
if (this._bPrevious) this._bPrevious.disabled = false;}
}
else {
if (this._slides.length == 0) {
if (this._bPrevious) this._bPrevious.disabled = true;if (this._bNext) this._bNext.disabled = true;if (this._bPlay) this._bPlay.disabled = true;}
} 
if (this._inPlayMode) {
this._timer.set_enabled(false);this._timer.set_enabled(true);}
},
resetSlideShowButtonState : function() {
if (this._inPlayMode) {
if (this._bPlay) this._bPlay.value = this._stopButtonValue;}
else {
this.resetButtons();if (this._bPlay) this._bPlay.value = this._playButtonValue;}
},
setCurrentImage : function() {
if (this._slides[this._currentIndex]) {
this._currentImageElement.src = this._slides[this._currentIndex].ImagePath;} else {
this._currentImageElement.src = '';}
if(Sys.Browser.agent == Sys.Browser.Opera) { 
this._onImageLoaded(true);}
},
updateImage : function(value) {
if (value) {
if (this.raiseSlideChanging(this._currentValue, value)) {
return;}
this._currentValue = value;this._elementImage.src = value.ImagePath;this._elementImage.alt = value.Name;if (this._imageDescriptionLabel) {
this._imageDescriptionLabel.innerHTML = value.Description ? value.Description : "";}
if (this._imageTitleLabel) {
this._imageTitleLabel.innerHTML = value.Name ? value.Name : "";}
this.raiseSlideChanged(value);this.resetButtons();}
},
get_imageDescriptionLabelID : function() {
return this._imageDescriptionLabelID;}, 
set_imageDescriptionLabelID : function(value) {
if (this._imageDescriptionLabelID != value) {
this._imageDescriptionLabelID = value;this.raisePropertyChanged('imageDescriptionLabelID');}
},
get_imageTitleLabelID : function() {
return this._imageTitleLabelID;},
set_imageTitleLabelID : function(value) {
if (this._imageTitleLabelID != value) {
this._imageTitleLabelID = value;this.raisePropertyChanged('imageTitleLabelID');}
},
get_nextButtonID : function() {
return this._nextButtonID;},
set_nextButtonID : function(value) {
if (this._nextButtonID != value) {
this._nextButtonID = value;this.raisePropertyChanged('nextButtonID');}
},
get_playButtonID : function() {
return this._playButtonID;},
set_playButtonID : function(value) {
if (this._playButtonID != value) {
this._playButtonID = value;this.raisePropertyChanged('playButtonID');}
},
get_playButtonText : function() {
return this._playButtonValue;},
set_playButtonText : function(value) {
if (this._playButtonValue != value) {
this._playButtonValue = value;this.raisePropertyChanged('playButtonText');}
}, 
get_stopButtonText : function() {
return this._stopButtonValue;},
set_stopButtonText : function(value) {
if (this._stopButtonValue != value) {
this._stopButtonValue = value;this.raisePropertyChanged('stopButtonText');}
},
get_playInterval : function() {
return this._playInterval;},
set_playInterval : function(value) {
if (this._playInterval != value) {
this._playInterval = value;this.raisePropertyChanged('playInterval');}
},
get_previousButtonID : function() {
return this._previousButtonID;},
set_previousButtonID : function(value) {
if (this._previousButtonID != value) {
this._previousButtonID = value;this.raisePropertyChanged('previousButtonID');}
},
get_slideShowServicePath : function() {
return this._slideShowServicePath;},
set_slideShowServicePath : function(value) {
if (this._slideShowServicePath != value) {
this._slideShowServicePath = value;this.raisePropertyChanged('slideShowServicePath');}
},
get_slideShowServiceMethod : function() {
return this._slideShowServiceMethod;},
set_slideShowServiceMethod : function(value) {
if (this._slideShowServiceMethod != value) {
this._slideShowServiceMethod = value;this.raisePropertyChanged('slideShowServiceMethod');}
},
get_loop : function() {
return this._loop;},
set_loop : function(value) {
if (this._loop != value) {
this._loop = value;this.raisePropertyChanged('loop');}
},
get_autoPlay : function() {
return this._autoPlay;},
set_autoPlay : function(value) {
if (this._autoPlay != value) {
this._autoPlay = value;this.raisePropertyChanged('autoPlay');}
},
_onClickNext : function(e) {
e.preventDefault();e.stopPropagation();this._clickNext();}, 
_onImageLoaded : function(e) {
this.updateImage(this._slides[this._currentIndex]);this.resetButtons();this._cacheImages();},
_clickNext : function() {
if (this._slides) {
if ((this._currentIndex + 1) < this._slides.length) {
++this._currentIndex;} else if (this._loop) {
this._currentIndex = 0;} else {
return false;}
this.setCurrentImage();return true;}
return false;},
_onClickPrevious : function(e) {
e.preventDefault();e.stopPropagation();this._clickPrevious();}, 
_clickPrevious : function() {
if (this._slides) {
if ((this._currentIndex - 1) >= 0) {
--this._currentIndex;}
else if (this._loop) {
this._currentIndex = this._slides.length - 1;} else {
return false;}
this.setCurrentImage();return true;}
return false;},
_onClickPlay : function(e) {
e.preventDefault();e.stopPropagation();this._play();}, 
_play : function() {
if (this._inPlayMode) {
this._inPlayMode = false;this._timer.set_enabled(false);this.resetSlideShowButtonState();} else {
this._inPlayMode = true;if (!this._timer) {
this._timer = new Sys.Timer();this._timer.set_interval(this._playInterval);this._tickHandler = Function.createDelegate(this, this._onPlay);this._timer.add_tick(this._tickHandler);}
this.resetSlideShowButtonState();this._timer.set_enabled(true);}
},
_onPlay : function(e) {
if (this._slides) {
if ((this._currentIndex + 1) < this._slides.length) {
++this._currentIndex;this.setCurrentImage();return true;} else if (this._loop) {
this._currentIndex = 0;this.setCurrentImage();return true;} else {
this._inPlayMode = false;this.resetSlideShowButtonState();}
}
return false;},
_slideShowInit : function() {
this._currentIndex = -1;this._cachedImageIndex = -1;this._inPlayMode = false;this._currentValue = null;this._images = null;var params = null;if (this._useContextKey) {
params = { contextKey : this._contextKey };}
Sys.Net.WebServiceProxy.invoke(
this._slideShowServicePath, 
this._slideShowServiceMethod, 
false,
params,
Function.createDelegate(this, this._initSlides), 
null, 
null);},
_initSlides : function(sender, eventArgs) {
this._slides = sender;if (this._slides) {
this._images = new Array();this._clickNext();if (this._autoPlay) {
this._play();}
}
},
_cacheImages : function() {
if ((this._currentIndex) % 3 == 0) {
var oldCachedImageIndex = this._cachedImageIndex;for (var i = this._cachedImageIndex + 1;i < this._slides.length;i++) {
if (this._slides[i]) {
this._images[i] = new Image();this._images[i].src = this._slides[i].ImagePath;this._cachedImageIndex = i;if((oldCachedImageIndex + 4) <= i ) {
break;}
}
}
}
}
}
AjaxControlToolkit.SlideShowBehavior.registerClass('AjaxControlToolkit.SlideShowBehavior', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.SlideShowEventArgs = function(previousSlide, nextSlide, slideIndex) {
AjaxControlToolkit.SlideShowEventArgs.initializeBase(this);this._previousSlide = previousSlide;this._nextSlide = nextSlide;this._slideIndex = slideIndex;}
AjaxControlToolkit.SlideShowEventArgs.prototype = {
get_previousSlide : function() {
return this._previousSlide;},
get_nextSlide : function() {
return this._nextSlide;},
get_slideIndex : function () {
return this._slideIndex;}
}
AjaxControlToolkit.SlideShowEventArgs.registerClass('AjaxControlToolkit.SlideShowEventArgs', Sys.CancelEventArgs);
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
