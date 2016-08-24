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
Type.registerNamespace('AjaxControlToolkit');AjaxControlToolkit.Resources={'PasswordStrength_InvalidWeightingRatios':'密碼複雜性的權重比例必須有 4 種','Animation_ChildrenNotAllowed':'AjaxControlToolkit.Animation.createAnimation 無法加入一個不是衍生自 AjaxControlToolkit.Animation.ParentAnimation 且類型為 {0} 的子動畫','PasswordStrength_RemainingSymbols':'還需要 {0} 個符號','ExtenderBase_CannotSetClientStateField':'只能在初始化之前設定 clientStateField','RTE_PreviewHTML':'Preview HTML','RTE_JustifyCenter':'Justify Center','PasswordStrength_RemainingUpperCase':'{0} more upper case characters','Animation_TargetNotFound':'AjaxControlToolkit.Animation.Animation.set_animationTarget 需要一個 Sys.UI.DomElement 或 Sys.UI.Control 類別的控制項  ID。找不到相對應的  {0} 之項目或控制項','RTE_FontColor':'Font Color','RTE_LabelColor':'Label Color','Common_InvalidBorderWidthUnit':'單位類型 {0} 對 parseBorderWidth 而言無效','RTE_Heading':'Heading','Tabs_PropertySetBeforeInitialization':'於初始化之前，無法變更 {0}','RTE_OrderedList':'Ordered List','ReorderList_DropWatcherBehavior_NoChild':'無法找到 ID 為 {0} 的子清單','CascadingDropDown_MethodTimeout':'[方法逾時]','RTE_Columns':'Columns','RTE_InsertImage':'Insert Image','RTE_InsertTable':'Insert Table','RTE_Values':'Values','RTE_OK':'OK','ExtenderBase_PageNotRegisteredForCallbacks':'這個頁面尚未註冊，無法提供回呼','Animation_NoDynamicPropertyFound':'AjaxControlToolkit.Animation.createAnimation 找不到相對應的  {0} 或 {1} 屬性','Animation_InvalidBaseType':'AjaxControlToolkit.Animation.registerAnimation 只能註冊那些繼承自 AjaxControlToolkit.Animation.Animation 的類型','RTE_UnorderedList':'Unordered List','ResizableControlBehavior_InvalidHandler':'{0} 處理常式不是函式、函式名稱、或是函式文字','Animation_InvalidColor':'標記名稱 Color 必須是 7 個字元的 16 進位字串（例如：#246ACF），不能是 {0}','RTE_CellColor':'Cell Color','PasswordStrength_RemainingMixedCase':'大小寫混合','RTE_Italic':'Italic','CascadingDropDown_NoParentElement':'無法找到父項目 {0}','ValidatorCallout_DefaultErrorMessage':'這個控制項無效','RTE_Indent':'Indent','ReorderList_DropWatcherBehavior_CallbackError':'無法重新排列，請參考下面的說明：\\r\\n\\r\\n{0}','PopupControl_NoDefaultProperty':'類型 {1} 的 控制項 {0} 不支援預設屬性','RTE_Normal':'Normal','PopupExtender_NoParentElement':'無法找到父項目 {0}','RTE_ViewValues':'View Values','RTE_Legend':'Legend','RTE_Labels':'Labels','RTE_CellSpacing':'Cell Spacing','PasswordStrength_RemainingNumbers':'還需要 {0} 個數字','RTE_Border':'Border','RTE_Create':'Create','RTE_BackgroundColor':'Background Color','RTE_Cancel':'Cancel','RTE_JustifyFull':'Justify Full','RTE_JustifyLeft':'Justify Left','RTE_Cut':'Cut','ResizableControlBehavior_CannotChangeProperty':'不支援對 {0} 的變更','RTE_ViewSource':'View Source','Common_InvalidPaddingUnit':'單位類型 {0} 對 parsePadding 而言無效','RTE_Paste':'Paste','ExtenderBase_ControlNotRegisteredForCallbacks':'這個控制項尚未註冊，無法提供回呼','Calendar_Today':'今天:  {0}','MultiHandleSlider_CssHeightWidthRequired':'You must specify a CSS width and height for all handle styles as well as the rail.','Common_DateTime_InvalidFormat':'格式無效','ListSearch_DefaultPrompt':'請鍵入以便搜尋','CollapsiblePanel_NoControlID':'無法找到項目 {0}','RTE_ViewEditor':'View Editor','RTE_BarColor':'Bar Color','PasswordStrength_DefaultStrengthDescriptions':'沒有;很弱;弱;差;差強人意;尚可;普通;好;很好;非常好;臻於完美！','RTE_Inserttexthere':'Insert text here','Animation_UknownAnimationName':'AjaxControlToolkit.Animation.createAnimation 找不到名稱為 {0} 的動畫','ExtenderBase_InvalidClientStateType':'saveClientState 必須傳回 String 型別的值','Rating_CallbackError':'發生未處理的例外狀況：\\r\\n{0}','Tabs_OwnerExpected':'於初始化之前，必須設定擁有者','DynamicPopulate_WebServiceTimeout':'呼叫 Web 服務逾時','PasswordStrength_RemainingLowerCase':'{0} more lower case characters','Animation_MissingAnimationName':'AjaxControlToolkit.Animation.createAnimation 必須持有一個 AnimationName 屬性的物件','RTE_JustifyRight':'Justify Right','Tabs_ActiveTabArgumentOutOfRange':'參數不是索引標籤 (Tab) 集合的成員','RTE_CellPadding':'Cell Padding','RTE_ClearFormatting':'Clear Formatting','AlwaysVisible_ElementRequired':'必須替 AjaxControlToolkit.AlwaysVisibleControlBehavior 指定一個項目','Slider_NoSizeProvided':'請在 Slider 的 CSS Class 中，設定高度與寬度屬性的有效值','DynamicPopulate_WebServiceError':'無法呼叫 Web 服務：{0}','PasswordStrength_StrengthPrompt':'複雜性：','PasswordStrength_RemainingCharacters':'還需要 {0} 個字元','PasswordStrength_Satisfied':'密碼複雜性已經足夠','RTE_Hyperlink':'Hyperlink','Animation_NoPropertyFound':'AjaxControlToolkit.Animation.createAnimation 找不到相對應的 {0} 屬性','PasswordStrength_InvalidStrengthDescriptionStyles':'密碼複雜性文字說明的樣式表，必須符合文字內容之個數','PasswordStrength_GetHelpRequirements':'取得密碼複雜性的要求說明','PasswordStrength_InvalidStrengthDescriptions':'所指定的密碼複雜性文字內容個數無效','RTE_Underline':'Underline','Tabs_PropertySetAfterInitialization':'於初始化之後，無法變更 {0}','RTE_Rows':'Rows','RTE_Redo':'Redo','RTE_Size':'Size','RTE_Undo':'Undo','RTE_Bold':'Bold','RTE_Copy':'Copy','RTE_Font':'Font','CascadingDropDown_MethodError':'[方法錯誤 {0}]','RTE_BorderColor':'Border Color','RTE_Paragraph':'Paragraph','RTE_InsertHorizontalRule':'Insert Horizontal Rule','Common_UnitHasNoDigits':'沒有數字','RTE_Outdent':'Outdent','Common_DateTime_InvalidTimeSpan':'{0} 的 TimeSpan 格式無效','Animation_CannotNestSequence':'AjaxControlToolkit.Animation.ParallelAnimation 不能內含 AjaxControlToolkit.Animation.SequenceAnimation','Shared_BrowserSecurityPreventsPaste':'您的瀏覽器安全性設定，不允許執行自動貼上的操作。請改用鍵盤快速鍵 Ctrl + V。'};
if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();
