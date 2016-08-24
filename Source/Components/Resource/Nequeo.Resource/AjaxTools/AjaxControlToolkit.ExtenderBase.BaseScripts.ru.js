Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.BehaviorBase = function(element) {
AjaxControlToolkit.BehaviorBase.initializeBase(this,[element]);this._clientStateFieldID = null;this._pageRequestManager = null;this._partialUpdateBeginRequestHandler = null;this._partialUpdateEndRequestHandler = null;}
AjaxControlToolkit.BehaviorBase.prototype = {
initialize : function() {
AjaxControlToolkit.BehaviorBase.callBaseMethod(this, 'initialize');},
dispose : function() {
AjaxControlToolkit.BehaviorBase.callBaseMethod(this, 'dispose');if (this._pageRequestManager) {
if (this._partialUpdateBeginRequestHandler) {
this._pageRequestManager.remove_beginRequest(this._partialUpdateBeginRequestHandler);this._partialUpdateBeginRequestHandler = null;}
if (this._partialUpdateEndRequestHandler) {
this._pageRequestManager.remove_endRequest(this._partialUpdateEndRequestHandler);this._partialUpdateEndRequestHandler = null;}
this._pageRequestManager = null;}
},
get_ClientStateFieldID : function() {
return this._clientStateFieldID;},
set_ClientStateFieldID : function(value) {
if (this._clientStateFieldID != value) {
this._clientStateFieldID = value;this.raisePropertyChanged('ClientStateFieldID');}
},
get_ClientState : function() {
if (this._clientStateFieldID) {
var input = document.getElementById(this._clientStateFieldID);if (input) {
return input.value;}
}
return null;},
set_ClientState : function(value) {
if (this._clientStateFieldID) {
var input = document.getElementById(this._clientStateFieldID);if (input) {
input.value = value;}
}
},
registerPartialUpdateEvents : function() {
if (Sys && Sys.WebForms && Sys.WebForms.PageRequestManager){
this._pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();if (this._pageRequestManager) {
this._partialUpdateBeginRequestHandler = Function.createDelegate(this, this._partialUpdateBeginRequest);this._pageRequestManager.add_beginRequest(this._partialUpdateBeginRequestHandler);this._partialUpdateEndRequestHandler = Function.createDelegate(this, this._partialUpdateEndRequest);this._pageRequestManager.add_endRequest(this._partialUpdateEndRequestHandler);}
}
},
_partialUpdateBeginRequest : function(sender, beginRequestEventArgs) {
},
_partialUpdateEndRequest : function(sender, endRequestEventArgs) {
}
}
AjaxControlToolkit.BehaviorBase.registerClass('AjaxControlToolkit.BehaviorBase', Sys.UI.Behavior);AjaxControlToolkit.DynamicPopulateBehaviorBase = function(element) {
AjaxControlToolkit.DynamicPopulateBehaviorBase.initializeBase(this, [element]);this._DynamicControlID = null;this._DynamicContextKey = null;this._DynamicServicePath = null;this._DynamicServiceMethod = null;this._cacheDynamicResults = false;this._dynamicPopulateBehavior = null;this._populatingHandler = null;this._populatedHandler = null;}
AjaxControlToolkit.DynamicPopulateBehaviorBase.prototype = {
initialize : function() {
AjaxControlToolkit.DynamicPopulateBehaviorBase.callBaseMethod(this, 'initialize');this._populatingHandler = Function.createDelegate(this, this._onPopulating);this._populatedHandler = Function.createDelegate(this, this._onPopulated);},
dispose : function() {
if (this._populatedHandler) {
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.remove_populated(this._populatedHandler);}
this._populatedHandler = null;}
if (this._populatingHandler) {
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.remove_populating(this._populatingHandler);}
this._populatingHandler = null;}
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.dispose();this._dynamicPopulateBehavior = null;}
AjaxControlToolkit.DynamicPopulateBehaviorBase.callBaseMethod(this, 'dispose');},
populate : function(contextKeyOverride) {
if (this._dynamicPopulateBehavior && (this._dynamicPopulateBehavior.get_element() != $get(this._DynamicControlID))) {
this._dynamicPopulateBehavior.dispose();this._dynamicPopulateBehavior = null;}
if (!this._dynamicPopulateBehavior && this._DynamicControlID && this._DynamicServiceMethod) {
this._dynamicPopulateBehavior = $create(AjaxControlToolkit.DynamicPopulateBehavior,
{
"id" : this.get_id() + "_DynamicPopulateBehavior",
"ContextKey" : this._DynamicContextKey,
"ServicePath" : this._DynamicServicePath,
"ServiceMethod" : this._DynamicServiceMethod,
"cacheDynamicResults" : this._cacheDynamicResults
}, null, null, $get(this._DynamicControlID));this._dynamicPopulateBehavior.add_populating(this._populatingHandler);this._dynamicPopulateBehavior.add_populated(this._populatedHandler);}
if (this._dynamicPopulateBehavior) {
this._dynamicPopulateBehavior.populate(contextKeyOverride ? contextKeyOverride : this._DynamicContextKey);}
},
_onPopulating : function(sender, eventArgs) {
this.raisePopulating(eventArgs);},
_onPopulated : function(sender, eventArgs) {
this.raisePopulated(eventArgs);},
get_dynamicControlID : function() {
return this._DynamicControlID;},
get_DynamicControlID : this.get_dynamicControlID,
set_dynamicControlID : function(value) {
if (this._DynamicControlID != value) {
this._DynamicControlID = value;this.raisePropertyChanged('dynamicControlID');this.raisePropertyChanged('DynamicControlID');}
},
set_DynamicControlID : this.set_dynamicControlID,
get_dynamicContextKey : function() {
return this._DynamicContextKey;},
get_DynamicContextKey : this.get_dynamicContextKey,
set_dynamicContextKey : function(value) {
if (this._DynamicContextKey != value) {
this._DynamicContextKey = value;this.raisePropertyChanged('dynamicContextKey');this.raisePropertyChanged('DynamicContextKey');}
},
set_DynamicContextKey : this.set_dynamicContextKey,
get_dynamicServicePath : function() {
return this._DynamicServicePath;},
get_DynamicServicePath : this.get_dynamicServicePath,
set_dynamicServicePath : function(value) {
if (this._DynamicServicePath != value) {
this._DynamicServicePath = value;this.raisePropertyChanged('dynamicServicePath');this.raisePropertyChanged('DynamicServicePath');}
},
set_DynamicServicePath : this.set_dynamicServicePath,
get_dynamicServiceMethod : function() {
return this._DynamicServiceMethod;},
get_DynamicServiceMethod : this.get_dynamicServiceMethod,
set_dynamicServiceMethod : function(value) {
if (this._DynamicServiceMethod != value) {
this._DynamicServiceMethod = value;this.raisePropertyChanged('dynamicServiceMethod');this.raisePropertyChanged('DynamicServiceMethod');}
},
set_DynamicServiceMethod : this.set_dynamicServiceMethod,
get_cacheDynamicResults : function() {
return this._cacheDynamicResults;},
set_cacheDynamicResults : function(value) {
if (this._cacheDynamicResults != value) {
this._cacheDynamicResults = value;this.raisePropertyChanged('cacheDynamicResults');}
},
add_populated : function(handler) {
this.get_events().addHandler("populated", handler);},
remove_populated : function(handler) {
this.get_events().removeHandler("populated", handler);},
raisePopulated : function(arg) {
var handler = this.get_events().getHandler("populated");if (handler) handler(this, arg);},
add_populating : function(handler) {
this.get_events().addHandler('populating', handler);},
remove_populating : function(handler) {
this.get_events().removeHandler('populating', handler);},
raisePopulating : function(eventArgs) {
var handler = this.get_events().getHandler('populating');if (handler) {
handler(this, eventArgs);}
}
}
AjaxControlToolkit.DynamicPopulateBehaviorBase.registerClass('AjaxControlToolkit.DynamicPopulateBehaviorBase', AjaxControlToolkit.BehaviorBase);AjaxControlToolkit.ControlBase = function(element) {
AjaxControlToolkit.ControlBase.initializeBase(this, [element]);this._clientStateField = null;this._callbackTarget = null;this._onsubmit$delegate = Function.createDelegate(this, this._onsubmit);this._oncomplete$delegate = Function.createDelegate(this, this._oncomplete);this._onerror$delegate = Function.createDelegate(this, this._onerror);}
AjaxControlToolkit.ControlBase.prototype = {
initialize : function() {
AjaxControlToolkit.ControlBase.callBaseMethod(this, "initialize");if (this._clientStateField) {
this.loadClientState(this._clientStateField.value);}
if (typeof(Sys.WebForms)!=="undefined" && typeof(Sys.WebForms.PageRequestManager)!=="undefined") {
Array.add(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);} else {
$addHandler(document.forms[0], "submit", this._onsubmit$delegate);}
},
dispose : function() {
if (typeof(Sys.WebForms)!=="undefined" && typeof(Sys.WebForms.PageRequestManager)!=="undefined") {
Array.remove(Sys.WebForms.PageRequestManager.getInstance()._onSubmitStatements, this._onsubmit$delegate);} else {
$removeHandler(document.forms[0], "submit", this._onsubmit$delegate);}
AjaxControlToolkit.ControlBase.callBaseMethod(this, "dispose");},
findElement : function(id) {
return $get(this.get_id() + '_' + id.split(':').join('_'));},
get_clientStateField : function() {
return this._clientStateField;},
set_clientStateField : function(value) {
if (this.get_isInitialized()) throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_CannotSetClientStateField);if (this._clientStateField != value) {
this._clientStateField = value;this.raisePropertyChanged('clientStateField');}
},
loadClientState : function(value) {
},
saveClientState : function() {
return null;},
_invoke : function(name, args, cb) {
if (!this._callbackTarget) {
throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_ControlNotRegisteredForCallbacks);}
if (typeof(WebForm_DoCallback)==="undefined") {
throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_PageNotRegisteredForCallbacks);}
var ar = [];for (var i = 0;i < args.length;i++) 
ar[i] = args[i];var clientState = this.saveClientState();if (clientState != null && !String.isInstanceOfType(clientState)) {
throw Error.invalidOperation(AjaxControlToolkit.Resources.ExtenderBase_InvalidClientStateType);}
var payload = Sys.Serialization.JavaScriptSerializer.serialize({name:name,args:ar,state:this.saveClientState()});WebForm_DoCallback(this._callbackTarget, payload, this._oncomplete$delegate, cb, this._onerror$delegate, true);},
_oncomplete : function(result, context) {
result = Sys.Serialization.JavaScriptSerializer.deserialize(result);if (result.error) {
throw Error.create(result.error);}
this.loadClientState(result.state);context(result.result);},
_onerror : function(message, context) {
throw Error.create(message);},
_onsubmit : function() {
if (this._clientStateField) {
this._clientStateField.value = this.saveClientState();}
return true;} 
}
AjaxControlToolkit.ControlBase.registerClass("AjaxControlToolkit.ControlBase", Sys.UI.Control);
Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.Resources={'PasswordStrength_InvalidWeightingRatios':'Кол-во весовых коэффициентов защищенности пароля должно быть равно 4.','Animation_ChildrenNotAllowed':'AjaxControlToolkit.Animation.createAnimation не может добавить дочернюю анимацию к типу \"{0}\", который не является производным от AjaxControlToolkit.Animation.ParentAnimation','PasswordStrength_RemainingSymbols':'Еще {0} символ(а|ов)','ExtenderBase_CannotSetClientStateField':'clientStateField может быть установлено только перед инициализацией','RTE_PreviewHTML':'Preview HTML','RTE_JustifyCenter':'Justify Center','PasswordStrength_RemainingUpperCase':'{0} more upper case characters','Animation_TargetNotFound':'AjaxControlToolkit.Animation.Animation.set_animationTarget требует ID типа Sys.UI.DomElement or Sys.UI.Control.  Невозможно найти элемент или контрол, соответствующий \"{0}\"','RTE_FontColor':'Font Color','RTE_LabelColor':'Label Color','Common_InvalidBorderWidthUnit':'Единица типа \"{0}\"\' не является правильной для parseBorderWidth','RTE_Heading':'Heading','Tabs_PropertySetBeforeInitialization':'{0} не может быть изменен перед инициализацией','RTE_OrderedList':'Ordered List','ReorderList_DropWatcherBehavior_NoChild':'Невозможно найти ребенка листа с id \"{0}\"','CascadingDropDown_MethodTimeout':'[Превышено время ожидания в методе]','RTE_Columns':'Columns','RTE_InsertImage':'Insert Image','RTE_InsertTable':'Insert Table','RTE_Values':'Values','RTE_OK':'OK','ExtenderBase_PageNotRegisteredForCallbacks':'Этот Page не был зарегистрирован для обратных вызовов','Animation_NoDynamicPropertyFound':'AjaxControlToolkit.Animation.createAnimation не нашел свойство, относящееся к \"{0}\" или \"{1}\"','Animation_InvalidBaseType':'AjaxControlToolkit.Animation.registerAnimation может регистрировать только типы, которые наследуются от AjaxControlToolkit.Animation.Animation','RTE_UnorderedList':'Unordered List','ResizableControlBehavior_InvalidHandler':'{0} обработчик не является функцией, именем функции или текстом функции','Animation_InvalidColor':'Цвето должен быть в 7-ми символьном шестнадцатеричном представлении (т.е. #246ACF), но не \"{0}\"','RTE_CellColor':'Cell Color','PasswordStrength_RemainingMixedCase':'Требуются символ(ы) в разных регистрах','RTE_Italic':'Italic','CascadingDropDown_NoParentElement':'Невозможно найти родителя элемента \"{0}\"','ValidatorCallout_DefaultErrorMessage':'Данный элемент не является валидным','RTE_Indent':'Indent','ReorderList_DropWatcherBehavior_CallbackError':'Ошибка пересортировки, дополнительная информация ниже.\\r\\n\\r\\n{0}','PopupControl_NoDefaultProperty':'Свойство по умолчанию типа \"{1}\" для control \"{0}\" не поддерживается.','RTE_Normal':'Normal','PopupExtender_NoParentElement':'Невозможно найти родителя элемента \"{0}\"','RTE_ViewValues':'View Values','RTE_Legend':'Legend','RTE_Labels':'Labels','RTE_CellSpacing':'Cell Spacing','PasswordStrength_RemainingNumbers':'Еще {0} цифр(ы)','RTE_Border':'Border','RTE_Create':'Create','RTE_BackgroundColor':'Background Color','RTE_Cancel':'Cancel','RTE_JustifyFull':'Justify Full','RTE_JustifyLeft':'Justify Left','RTE_Cut':'Cut','ResizableControlBehavior_CannotChangeProperty':'Изменения для {0} не поддерживаются','RTE_ViewSource':'View Source','Common_InvalidPaddingUnit':'Единица типа \"{0}\" не является правильной для parsePadding','RTE_Paste':'Paste','ExtenderBase_ControlNotRegisteredForCallbacks':'Этот Control не был зарегистрирован для обратных вызовов','Calendar_Today':'Сегодня: {0}','MultiHandleSlider_CssHeightWidthRequired':'You must specify a CSS width and height for all handle styles as well as the rail.','Common_DateTime_InvalidFormat':'Неверный формат','ListSearch_DefaultPrompt':'Тип для поиска','CollapsiblePanel_NoControlID':'Невозможно найти элемент \"{0}\"','RTE_ViewEditor':'View Editor','RTE_BarColor':'Bar Color','PasswordStrength_DefaultStrengthDescriptions':'Отсутствует;Очень слабый;Слабый;Неподходящий;Почти хороший;Примерно нормальный;Нормальный;Хороший;Сильный;Замечательный;Первокласный!','RTE_Inserttexthere':'Insert text here','Animation_UknownAnimationName':'AjaxControlToolkit.Animation.createAnimation невозможно найти Animation, относящуюся к имени \"{0}\"','ExtenderBase_InvalidClientStateType':'saveClientState должен возвращать значение типа String','Rating_CallbackError':'Возникло необработанное исключение:\\r\\n{0}','Tabs_OwnerExpected':'Владелец должен быть установлен перед инициализацией','DynamicPopulate_WebServiceTimeout':'Превышено время ожидания вебсервиса','PasswordStrength_RemainingLowerCase':'{0} more lower case characters','Animation_MissingAnimationName':'AjaxControlToolkit.Animation.createAnimation требует объект со свойством AnimationName','RTE_JustifyRight':'Justify Right','Tabs_ActiveTabArgumentOutOfRange':'Аргумент не является цленом коллекции закладок','RTE_CellPadding':'Cell Padding','RTE_ClearFormatting':'Clear Formatting','AlwaysVisible_ElementRequired':'AjaxControlToolkit.AlwaysVisibleControlBehavior: элемент должен быть указан','Slider_NoSizeProvided':'Пожалуйста, установите правильные значения высоты и ширины для CSS класса ползунка','DynamicPopulate_WebServiceError':'Неудачный вызов вебсервиса: {0}','PasswordStrength_StrengthPrompt':'Пароль: ','PasswordStrength_RemainingCharacters':'Еще {0} букв(ы)','PasswordStrength_Satisfied':'Все ок.','RTE_Hyperlink':'Hyperlink','Animation_NoPropertyFound':'AjaxControlToolkit.Animation.createAnimation не нашел свойство, относящееся к \"{0}\"','PasswordStrength_InvalidStrengthDescriptionStyles':'Text Strength description style classes must match the number of text descriptions.','PasswordStrength_GetHelpRequirements':'Получить помощь по требованиям к паролю','PasswordStrength_InvalidStrengthDescriptions':'Указано неверное кол-во описаний для уровней защищенности пароля','RTE_Underline':'Underline','Tabs_PropertySetAfterInitialization':'{0} не может быть изменен после инициализации','RTE_Rows':'Rows','RTE_Redo':'Redo','RTE_Size':'Size','RTE_Undo':'Undo','RTE_Bold':'Bold','RTE_Copy':'Copy','RTE_Font':'Font','CascadingDropDown_MethodError':'[Ошибка метода {0}]','RTE_BorderColor':'Border Color','RTE_Paragraph':'Paragraph','RTE_InsertHorizontalRule':'Insert Horizontal Rule','Common_UnitHasNoDigits':'Нет цифр','RTE_Outdent':'Outdent','Common_DateTime_InvalidTimeSpan':'\"{0}\" не является правильным TimeSpan форматом','Animation_CannotNestSequence':'AjaxControlToolkit.Animation.SequenceAnimation не может быть вложен в AjaxControlToolkit.Animation.ParallelAnimation','Shared_BrowserSecurityPreventsPaste':'Настройки безопасности Вашего браузера не позволяют автоматическое выполнение операции вставки. Пожалуйста, воспользуйтесь комбинацией Ctrl+V.'};
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
