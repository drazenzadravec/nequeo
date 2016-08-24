Type.registerNamespace('AjaxControlToolkit.Animation'); var $AA = AjaxControlToolkit.Animation; $AA.registerAnimation = function(name, type) {
    if (type && ((type === $AA.Animation) || (type.inheritsFrom && type.inheritsFrom($AA.Animation)))) {
        if (!$AA.__animations) {
            $AA.__animations = {};
        }
        $AA.__animations[name.toLowerCase()] = type; type.play = function() {
            var animation = new type(); type.apply(animation, arguments); animation.initialize(); var handler = Function.createDelegate(animation,
function() {
    animation.remove_ended(handler); handler = null; animation.dispose();
}); animation.add_ended(handler); animation.play();
        }
    } else {
        throw Error.argumentType('type', type, $AA.Animation, AjaxControlToolkit.Resources.Animation_InvalidBaseType);
    }
}
$AA.buildAnimation = function(json, defaultTarget) {
    if (!json || json === '') {
        return null;
    }
    var obj; json = '(' + json + ')'; if (!Sys.Debug.isDebug) {
        try { obj = Sys.Serialization.JavaScriptSerializer.deserialize(json); } catch (ex) { }
    } else {
        obj = Sys.Serialization.JavaScriptSerializer.deserialize(json);
    }
    return $AA.createAnimation(obj, defaultTarget);
}
$AA.createAnimation = function(obj, defaultTarget) {
    if (!obj || !obj.AnimationName) {
        throw Error.argument('obj', AjaxControlToolkit.Resources.Animation_MissingAnimationName);
    }
    var type = $AA.__animations[obj.AnimationName.toLowerCase()]; if (!type) {
        throw Error.argument('type', String.format(AjaxControlToolkit.Resources.Animation_UknownAnimationName, obj.AnimationName));
    }
    var animation = new type(); if (defaultTarget) {
        animation.set_target(defaultTarget);
    }
    if (obj.AnimationChildren && obj.AnimationChildren.length) {
        if ($AA.ParentAnimation.isInstanceOfType(animation)) {
            for (var i = 0; i < obj.AnimationChildren.length; i++) {
                var child = $AA.createAnimation(obj.AnimationChildren[i]); if (child) {
                    animation.add(child);
                }
            }
        } else {
            throw Error.argument('obj', String.format(AjaxControlToolkit.Resources.Animation_ChildrenNotAllowed, type.getName()));
        }
    }
    var properties = type.__animationProperties; if (!properties) {
        type.__animationProperties = {}; type.resolveInheritance(); for (var name in type.prototype) {
            if (name.startsWith('set_')) {
                type.__animationProperties[name.substr(4).toLowerCase()] = name;
            }
        }
        delete type.__animationProperties['id']; properties = type.__animationProperties;
    }
    for (var property in obj) {
        var prop = property.toLowerCase(); if (prop == 'animationname' || prop == 'animationchildren') {
            continue;
        }
        var value = obj[property]; var setter = properties[prop]; if (setter && String.isInstanceOfType(setter) && animation[setter]) {
            if (!Sys.Debug.isDebug) {
                try { animation[setter](value); } catch (ex) { }
            } else {
                animation[setter](value);
            }
        } else {
            if (prop.endsWith('script')) {
                setter = properties[prop.substr(0, property.length - 6)]; if (setter && String.isInstanceOfType(setter) && animation[setter]) {
                    animation.DynamicProperties[setter] = value;
                } else if (Sys.Debug.isDebug) {
                    throw Error.argument('obj', String.format(AjaxControlToolkit.Resources.Animation_NoDynamicPropertyFound, property, property.substr(0, property.length - 5)));
                }
            } else if (Sys.Debug.isDebug) {
                throw Error.argument('obj', String.format(AjaxControlToolkit.Resources.Animation_NoPropertyFound, property));
            }
        }
    }
    return animation;
}
$AA.Animation = function(target, duration, fps) {
    $AA.Animation.initializeBase(this); this._duration = 1; this._fps = 25; this._target = null; this._tickHandler = null; this._timer = null; this._percentComplete = 0; this._percentDelta = null; this._owner = null; this._parentAnimation = null; this.DynamicProperties = {}; if (target) {
        this.set_target(target);
    }
    if (duration) {
        this.set_duration(duration);
    }
    if (fps) {
        this.set_fps(fps);
    }
}
$AA.Animation.prototype = {
    dispose: function() {
        if (this._timer) {
            this._timer.dispose(); this._timer = null;
        }
        this._tickHandler = null; this._target = null; $AA.Animation.callBaseMethod(this, 'dispose');
    },
    play: function() {
        if (!this._owner) {
            var resume = true; if (!this._timer) {
                resume = false; if (!this._tickHandler) {
                    this._tickHandler = Function.createDelegate(this, this._onTimerTick);
                }
                this._timer = new Sys.Timer(); this._timer.add_tick(this._tickHandler); this.onStart(); this._timer.set_interval(1000 / this._fps); this._percentDelta = 100 / (this._duration * this._fps); this._updatePercentComplete(0, true);
            }
            this._timer.set_enabled(true); this.raisePropertyChanged('isPlaying'); if (!resume) {
                this.raisePropertyChanged('isActive');
            }
        }
    },
    pause: function() {
        if (!this._owner) {
            if (this._timer) {
                this._timer.set_enabled(false); this.raisePropertyChanged('isPlaying');
            }
        }
    },
    stop: function(finish) {
        if (!this._owner) {
            var t = this._timer; this._timer = null; if (t) {
                t.dispose(); if (this._percentComplete !== 100) {
                    this._percentComplete = 100; this.raisePropertyChanged('percentComplete'); if (finish || finish === undefined) {
                        this.onStep(100);
                    }
                }
                this.onEnd(); this.raisePropertyChanged('isPlaying'); this.raisePropertyChanged('isActive');
            }
        }
    },
    onStart: function() {
        this.raiseStarted(); for (var property in this.DynamicProperties) {
            try {
                this[property](eval(this.DynamicProperties[property]));
            } catch (ex) {
                if (Sys.Debug.isDebug) {
                    throw ex;
                }
            }
        }
    },
    onStep: function(percentage) {
        this.setValue(this.getAnimatedValue(percentage)); this.raiseStep();
    },
    onEnd: function() {
        this.raiseEnded();
    },
    getAnimatedValue: function(percentage) {
        throw Error.notImplemented();
    },
    setValue: function(value) {
        throw Error.notImplemented();
    },
    interpolate: function(start, end, percentage) {
        return start + (end - start) * (percentage / 100);
    },
    _onTimerTick: function() {
        this._updatePercentComplete(this._percentComplete + this._percentDelta, true);
    },
    _updatePercentComplete: function(percentComplete, animate) {
        if (percentComplete > 100) {
            percentComplete = 100;
        }
        this._percentComplete = percentComplete; this.raisePropertyChanged('percentComplete'); if (animate) {
            this.onStep(percentComplete);
        }
        if (percentComplete === 100) {
            this.stop(false);
        }
    },
    setOwner: function(owner) {
        this._owner = owner;
    },
    raiseStarted: function() {
        var handlers = this.get_events().getHandler('started'); if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    add_started: function(handler) {
        this.get_events().addHandler("started", handler);
    },
    remove_started: function(handler) {
        this.get_events().removeHandler("started", handler);
    },
    raiseEnded: function() {
        var handlers = this.get_events().getHandler('ended'); if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    add_ended: function(handler) {
        this.get_events().addHandler("ended", handler);
    },
    remove_ended: function(handler) {
        this.get_events().removeHandler("ended", handler);
    },
    raiseStep: function() {
        var handlers = this.get_events().getHandler('step'); if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    add_step: function(handler) {
        this.get_events().addHandler("step", handler);
    },
    remove_step: function(handler) {
        this.get_events().removeHandler("step", handler);
    },
    get_target: function() {
        if (!this._target && this._parentAnimation) {
            return this._parentAnimation.get_target();
        }
        return this._target;
    },
    set_target: function(value) {
        if (this._target != value) {
            this._target = value; this.raisePropertyChanged('target');
        }
    },
    set_animationTarget: function(id) {
        var target = null; var element = $get(id); if (element) {
            target = element;
        } else {
            var ctrl = $find(id); if (ctrl) {
                element = ctrl.get_element(); if (element) {
                    target = element;
                }
            }
        }
        if (target) {
            this.set_target(target);
        } else {
            throw Error.argument('id', String.format(AjaxControlToolkit.Resources.Animation_TargetNotFound, id));
        }
    },
    get_duration: function() {
        return this._duration;
    },
    set_duration: function(value) {
        value = this._getFloat(value); if (this._duration != value) {
            this._duration = value; this.raisePropertyChanged('duration');
        }
    },
    get_fps: function() {
        return this._fps;
    },
    set_fps: function(value) {
        value = this._getInteger(value); if (this.fps != value) {
            this._fps = value; this.raisePropertyChanged('fps');
        }
    },
    get_isActive: function() {
        return (this._timer !== null);
    },
    get_isPlaying: function() {
        return (this._timer !== null) && this._timer.get_enabled();
    },
    get_percentComplete: function() {
        return this._percentComplete;
    },
    _getBoolean: function(value) {
        if (String.isInstanceOfType(value)) {
            return Boolean.parse(value);
        }
        return value;
    },
    _getInteger: function(value) {
        if (String.isInstanceOfType(value)) {
            return parseInt(value);
        }
        return value;
    },
    _getFloat: function(value) {
        if (String.isInstanceOfType(value)) {
            return parseFloat(value);
        }
        return value;
    },
    _getEnum: function(value, type) {
        if (String.isInstanceOfType(value) && type && type.parse) {
            return type.parse(value);
        }
        return value;
    }
}
$AA.Animation.registerClass('AjaxControlToolkit.Animation.Animation', Sys.Component); $AA.registerAnimation('animation', $AA.Animation); $AA.ParentAnimation = function(target, duration, fps, animations) {
    $AA.ParentAnimation.initializeBase(this, [target, duration, fps]); this._animations = []; if (animations && animations.length) {
        for (var i = 0; i < animations.length; i++) {
            this.add(animations[i]);
        }
    }
}
$AA.ParentAnimation.prototype = {
    initialize: function() {
        $AA.ParentAnimation.callBaseMethod(this, 'initialize'); if (this._animations) {
            for (var i = 0; i < this._animations.length; i++) {
                var animation = this._animations[i]; if (animation && !animation.get_isInitialized) {
                    animation.initialize();
                }
            }
        }
    },
    dispose: function() {
        this.clear(); this._animations = null; $AA.ParentAnimation.callBaseMethod(this, 'dispose');
    },
    get_animations: function() {
        return this._animations;
    },
    add: function(animation) {
        if (this._animations) {
            if (animation) {
                animation._parentAnimation = this;
            }
            Array.add(this._animations, animation); this.raisePropertyChanged('animations');
        }
    },
    remove: function(animation) {
        if (this._animations) {
            if (animation) {
                animation.dispose();
            }
            Array.remove(this._animations, animation); this.raisePropertyChanged('animations');
        }
    },
    removeAt: function(index) {
        if (this._animations) {
            var animation = this._animations[index]; if (animation) {
                animation.dispose();
            }
            Array.removeAt(this._animations, index); this.raisePropertyChanged('animations');
        }
    },
    clear: function() {
        if (this._animations) {
            for (var i = this._animations.length - 1; i >= 0; i--) {
                this._animations[i].dispose(); this._animations[i] = null;
            }
            Array.clear(this._animations); this._animations = []; this.raisePropertyChanged('animations');
        }
    }
}
$AA.ParentAnimation.registerClass('AjaxControlToolkit.Animation.ParentAnimation', $AA.Animation); $AA.registerAnimation('parent', $AA.ParentAnimation); $AA.ParallelAnimation = function(target, duration, fps, animations) {
    $AA.ParallelAnimation.initializeBase(this, [target, duration, fps, animations]);
}
$AA.ParallelAnimation.prototype = {
    add: function(animation) {
        $AA.ParallelAnimation.callBaseMethod(this, 'add', [animation]); animation.setOwner(this);
    },
    onStart: function() {
        $AA.ParallelAnimation.callBaseMethod(this, 'onStart'); var animations = this.get_animations(); for (var i = 0; i < animations.length; i++) {
            animations[i].onStart();
        }
    },
    onStep: function(percentage) {
        var animations = this.get_animations(); for (var i = 0; i < animations.length; i++) {
            animations[i].onStep(percentage);
        }
    },
    onEnd: function() {
        var animations = this.get_animations(); for (var i = 0; i < animations.length; i++) {
            animations[i].onEnd();
        }
        $AA.ParallelAnimation.callBaseMethod(this, 'onEnd');
    }
}
$AA.ParallelAnimation.registerClass('AjaxControlToolkit.Animation.ParallelAnimation', $AA.ParentAnimation); $AA.registerAnimation('parallel', $AA.ParallelAnimation); $AA.SequenceAnimation = function(target, duration, fps, animations, iterations) {
    $AA.SequenceAnimation.initializeBase(this, [target, duration, fps, animations]); this._handler = null; this._paused = false; this._playing = false; this._index = 0; this._remainingIterations = 0; this._iterations = (iterations !== undefined) ? iterations : 1;
}
$AA.SequenceAnimation.prototype = {
    dispose: function() {
        this._handler = null; $AA.SequenceAnimation.callBaseMethod(this, 'dispose');
    },
    stop: function() {
        if (this._playing) {
            var animations = this.get_animations(); if (this._index < animations.length) {
                animations[this._index].remove_ended(this._handler); for (var i = this._index; i < animations.length; i++) {
                    animations[i].stop();
                }
            }
            this._playing = false; this._paused = false; this.raisePropertyChanged('isPlaying'); this.onEnd();
        }
    },
    pause: function() {
        if (this.get_isPlaying()) {
            var current = this.get_animations()[this._index]; if (current != null) {
                current.pause();
            }
            this._paused = true; this.raisePropertyChanged('isPlaying');
        }
    },
    play: function() {
        var animations = this.get_animations(); if (!this._playing) {
            this._playing = true; if (this._paused) {
                this._paused = false; var current = animations[this._index]; if (current != null) {
                    current.play(); this.raisePropertyChanged('isPlaying');
                }
            } else {
                this.onStart(); this._index = 0; var first = animations[this._index]; if (first) {
                    first.add_ended(this._handler); first.play(); this.raisePropertyChanged('isPlaying');
                } else {
                    this.stop();
                }
            }
        }
    },
    onStart: function() {
        $AA.SequenceAnimation.callBaseMethod(this, 'onStart'); this._remainingIterations = this._iterations - 1; if (!this._handler) {
            this._handler = Function.createDelegate(this, this._onEndAnimation);
        }
    },
    _onEndAnimation: function() {
        var animations = this.get_animations(); var current = animations[this._index++]; if (current) {
            current.remove_ended(this._handler);
        }
        if (this._index < animations.length) {
            var next = animations[this._index]; next.add_ended(this._handler); next.play();
        } else if (this._remainingIterations >= 1 || this._iterations <= 0) {
            this._remainingIterations--; this._index = 0; var first = animations[0]; first.add_ended(this._handler); first.play();
        } else {
            this.stop();
        }
    },
    onStep: function(percentage) {
        throw Error.invalidOperation(AjaxControlToolkit.Resources.Animation_CannotNestSequence);
    },
    onEnd: function() {
        this._remainingIterations = 0; $AA.SequenceAnimation.callBaseMethod(this, 'onEnd');
    },
    get_isActive: function() {
        return true;
    },
    get_isPlaying: function() {
        return this._playing && !this._paused;
    },
    get_iterations: function() {
        return this._iterations;
    },
    set_iterations: function(value) {
        value = this._getInteger(value); if (this._iterations != value) {
            this._iterations = value; this.raisePropertyChanged('iterations');
        }
    },
    get_isInfinite: function() {
        return this._iterations <= 0;
    }
}
$AA.SequenceAnimation.registerClass('AjaxControlToolkit.Animation.SequenceAnimation', $AA.ParentAnimation); $AA.registerAnimation('sequence', $AA.SequenceAnimation); $AA.SelectionAnimation = function(target, duration, fps, animations) {
    $AA.SelectionAnimation.initializeBase(this, [target, duration, fps, animations]); this._selectedIndex = -1; this._selected = null;
}
$AA.SelectionAnimation.prototype = {
    getSelectedIndex: function() {
        throw Error.notImplemented();
    },
    onStart: function() {
        $AA.SelectionAnimation.callBaseMethod(this, 'onStart'); var animations = this.get_animations(); this._selectedIndex = this.getSelectedIndex(); if (this._selectedIndex >= 0 && this._selectedIndex < animations.length) {
            this._selected = animations[this._selectedIndex]; if (this._selected) {
                this._selected.setOwner(this); this._selected.onStart();
            }
        }
    },
    onStep: function(percentage) {
        if (this._selected) {
            this._selected.onStep(percentage);
        }
    },
    onEnd: function() {
        if (this._selected) {
            this._selected.onEnd(); this._selected.setOwner(null);
        }
        this._selected = null; this._selectedIndex = null; $AA.SelectionAnimation.callBaseMethod(this, 'onEnd');
    }
}
$AA.SelectionAnimation.registerClass('AjaxControlToolkit.Animation.SelectionAnimation', $AA.ParentAnimation); $AA.registerAnimation('selection', $AA.SelectionAnimation); $AA.ConditionAnimation = function(target, duration, fps, animations, conditionScript) {
    $AA.ConditionAnimation.initializeBase(this, [target, duration, fps, animations]); this._conditionScript = conditionScript;
}
$AA.ConditionAnimation.prototype = {
    getSelectedIndex: function() {
        var selected = -1; if (this._conditionScript && this._conditionScript.length > 0) {
            try {
                selected = eval(this._conditionScript) ? 0 : 1;
            } catch (ex) {
            }
        }
        return selected;
    },
    get_conditionScript: function() {
        return this._conditionScript;
    },
    set_conditionScript: function(value) {
        if (this._conditionScript != value) {
            this._conditionScript = value; this.raisePropertyChanged('conditionScript');
        }
    }
}
$AA.ConditionAnimation.registerClass('AjaxControlToolkit.Animation.ConditionAnimation', $AA.SelectionAnimation); $AA.registerAnimation('condition', $AA.ConditionAnimation); $AA.CaseAnimation = function(target, duration, fps, animations, selectScript) {
    $AA.CaseAnimation.initializeBase(this, [target, duration, fps, animations]); this._selectScript = selectScript;
}
$AA.CaseAnimation.prototype = {
    getSelectedIndex: function() {
        var selected = -1; if (this._selectScript && this._selectScript.length > 0) {
            try {
                var result = eval(this._selectScript)
                if (result !== undefined)
                    selected = result;
            } catch (ex) {
            }
        }
        return selected;
    },
    get_selectScript: function() {
        return this._selectScript;
    },
    set_selectScript: function(value) {
        if (this._selectScript != value) {
            this._selectScript = value; this.raisePropertyChanged('selectScript');
        }
    }
}
$AA.CaseAnimation.registerClass('AjaxControlToolkit.Animation.CaseAnimation', $AA.SelectionAnimation); $AA.registerAnimation('case', $AA.CaseAnimation); $AA.FadeEffect = function() {
    throw Error.invalidOperation();
}
$AA.FadeEffect.prototype = {
    FadeIn: 0,
    FadeOut: 1
}
$AA.FadeEffect.registerEnum("AjaxControlToolkit.Animation.FadeEffect", false); $AA.FadeAnimation = function(target, duration, fps, effect, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $AA.FadeAnimation.initializeBase(this, [target, duration, fps]); this._effect = (effect !== undefined) ? effect : $AA.FadeEffect.FadeIn; this._max = (maximumOpacity !== undefined) ? maximumOpacity : 1; this._min = (minimumOpacity !== undefined) ? minimumOpacity : 0; this._start = this._min; this._end = this._max; this._layoutCreated = false; this._forceLayoutInIE = (forceLayoutInIE === undefined || forceLayoutInIE === null) ? true : forceLayoutInIE; this._currentTarget = null; this._resetOpacities();
}
$AA.FadeAnimation.prototype = {
    _resetOpacities: function() {
        if (this._effect == $AA.FadeEffect.FadeIn) {
            this._start = this._min; this._end = this._max;
        } else {
            this._start = this._max; this._end = this._min;
        }
    },
    _createLayout: function() {
        var element = this._currentTarget; if (element) {
            this._originalWidth = $common.getCurrentStyle(element, 'width'); var originalHeight = $common.getCurrentStyle(element, 'height'); this._originalBackColor = $common.getCurrentStyle(element, 'backgroundColor'); if ((!this._originalWidth || this._originalWidth == '' || this._originalWidth == 'auto') &&
(!originalHeight || originalHeight == '' || originalHeight == 'auto')) {
                element.style.width = element.offsetWidth + 'px';
            }
            if (!this._originalBackColor || this._originalBackColor == '' || this._originalBackColor == 'transparent' || this._originalBackColor == 'rgba(0, 0, 0, 0)') {
                element.style.backgroundColor = $common.getInheritedBackgroundColor(element);
            }
            this._layoutCreated = true;
        }
    },
    onStart: function() {
        $AA.FadeAnimation.callBaseMethod(this, 'onStart'); this._currentTarget = this.get_target(); this.setValue(this._start); if (this._forceLayoutInIE && !this._layoutCreated && Sys.Browser.agent == Sys.Browser.InternetExplorer) {
            this._createLayout();
        }
    },
    getAnimatedValue: function(percentage) {
        return this.interpolate(this._start, this._end, percentage);
    },
    setValue: function(value) {
        if (this._currentTarget) {
            $common.setElementOpacity(this._currentTarget, value);
        }
    },
    get_effect: function() {
        return this._effect;
    },
    set_effect: function(value) {
        value = this._getEnum(value, $AA.FadeEffect); if (this._effect != value) {
            this._effect = value; this._resetOpacities(); this.raisePropertyChanged('effect');
        }
    },
    get_minimumOpacity: function() {
        return this._min;
    },
    set_minimumOpacity: function(value) {
        value = this._getFloat(value); if (this._min != value) {
            this._min = value; this._resetOpacities(); this.raisePropertyChanged('minimumOpacity');
        }
    },
    get_maximumOpacity: function() {
        return this._max;
    },
    set_maximumOpacity: function(value) {
        value = this._getFloat(value); if (this._max != value) {
            this._max = value; this._resetOpacities(); this.raisePropertyChanged('maximumOpacity');
        }
    },
    get_forceLayoutInIE: function() {
        return this._forceLayoutInIE;
    },
    set_forceLayoutInIE: function(value) {
        value = this._getBoolean(value); if (this._forceLayoutInIE != value) {
            this._forceLayoutInIE = value; this.raisePropertyChanged('forceLayoutInIE');
        }
    },
    set_startValue: function(value) {
        value = this._getFloat(value); this._start = value;
    }
}
$AA.FadeAnimation.registerClass('AjaxControlToolkit.Animation.FadeAnimation', $AA.Animation); $AA.registerAnimation('fade', $AA.FadeAnimation); $AA.FadeInAnimation = function(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $AA.FadeInAnimation.initializeBase(this, [target, duration, fps, $AA.FadeEffect.FadeIn, minimumOpacity, maximumOpacity, forceLayoutInIE]);
}
$AA.FadeInAnimation.prototype = {
    onStart: function() {
        $AA.FadeInAnimation.callBaseMethod(this, 'onStart'); if (this._currentTarget) {
            this.set_startValue($common.getElementOpacity(this._currentTarget));
        }
    }
}
$AA.FadeInAnimation.registerClass('AjaxControlToolkit.Animation.FadeInAnimation', $AA.FadeAnimation); $AA.registerAnimation('fadeIn', $AA.FadeInAnimation); $AA.FadeOutAnimation = function(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $AA.FadeOutAnimation.initializeBase(this, [target, duration, fps, $AA.FadeEffect.FadeOut, minimumOpacity, maximumOpacity, forceLayoutInIE]);
}
$AA.FadeOutAnimation.prototype = {
    onStart: function() {
        $AA.FadeOutAnimation.callBaseMethod(this, 'onStart'); if (this._currentTarget) {
            this.set_startValue($common.getElementOpacity(this._currentTarget));
        }
    }
}
$AA.FadeOutAnimation.registerClass('AjaxControlToolkit.Animation.FadeOutAnimation', $AA.FadeAnimation); $AA.registerAnimation('fadeOut', $AA.FadeOutAnimation); $AA.PulseAnimation = function(target, duration, fps, iterations, minimumOpacity, maximumOpacity, forceLayoutInIE) {
    $AA.PulseAnimation.initializeBase(this, [target, duration, fps, null, ((iterations !== undefined) ? iterations : 3)]); this._out = new $AA.FadeOutAnimation(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE); this.add(this._out); this._in = new $AA.FadeInAnimation(target, duration, fps, minimumOpacity, maximumOpacity, forceLayoutInIE); this.add(this._in);
}
$AA.PulseAnimation.prototype = {
    get_minimumOpacity: function() {
        return this._out.get_minimumOpacity();
    },
    set_minimumOpacity: function(value) {
        value = this._getFloat(value); this._out.set_minimumOpacity(value); this._in.set_minimumOpacity(value); this.raisePropertyChanged('minimumOpacity');
    },
    get_maximumOpacity: function() {
        return this._out.get_maximumOpacity();
    },
    set_maximumOpacity: function(value) {
        value = this._getFloat(value); this._out.set_maximumOpacity(value); this._in.set_maximumOpacity(value); this.raisePropertyChanged('maximumOpacity');
    },
    get_forceLayoutInIE: function() {
        return this._out.get_forceLayoutInIE();
    },
    set_forceLayoutInIE: function(value) {
        value = this._getBoolean(value); this._out.set_forceLayoutInIE(value); this._in.set_forceLayoutInIE(value); this.raisePropertyChanged('forceLayoutInIE');
    },
    set_duration: function(value) {
        value = this._getFloat(value); $AA.PulseAnimation.callBaseMethod(this, 'set_duration', [value]); this._in.set_duration(value); this._out.set_duration(value);
    },
    set_fps: function(value) {
        value = this._getInteger(value); $AA.PulseAnimation.callBaseMethod(this, 'set_fps', [value]); this._in.set_fps(value); this._out.set_fps(value);
    }
}
$AA.PulseAnimation.registerClass('AjaxControlToolkit.Animation.PulseAnimation', $AA.SequenceAnimation); $AA.registerAnimation('pulse', $AA.PulseAnimation); $AA.PropertyAnimation = function(target, duration, fps, property, propertyKey) {
    $AA.PropertyAnimation.initializeBase(this, [target, duration, fps]); this._property = property; this._propertyKey = propertyKey; this._currentTarget = null;
}
$AA.PropertyAnimation.prototype = {
    onStart: function() {
        $AA.PropertyAnimation.callBaseMethod(this, 'onStart'); this._currentTarget = this.get_target();
    },
    setValue: function(value) {
        var element = this._currentTarget; if (element && this._property && this._property.length > 0) {
            if (this._propertyKey && this._propertyKey.length > 0 && element[this._property]) {
                element[this._property][this._propertyKey] = value;
            } else {
                element[this._property] = value;
            }
        }
    },
    getValue: function() {
        var element = this.get_target(); if (element && this._property && this._property.length > 0) {
            var property = element[this._property]; if (property) {
                if (this._propertyKey && this._propertyKey.length > 0) {
                    return property[this._propertyKey];
                }
                return property;
            }
        }
        return null;
    },
    get_property: function() {
        return this._property;
    },
    set_property: function(value) {
        if (this._property != value) {
            this._property = value; this.raisePropertyChanged('property');
        }
    },
    get_propertyKey: function() {
        return this._propertyKey;
    },
    set_propertyKey: function(value) {
        if (this._propertyKey != value) {
            this._propertyKey = value; this.raisePropertyChanged('propertyKey');
        }
    }
}
$AA.PropertyAnimation.registerClass('AjaxControlToolkit.Animation.PropertyAnimation', $AA.Animation); $AA.registerAnimation('property', $AA.PropertyAnimation); $AA.DiscreteAnimation = function(target, duration, fps, property, propertyKey, values) {
    $AA.DiscreteAnimation.initializeBase(this, [target, duration, fps, property, propertyKey]); this._values = (values && values.length) ? values : [];
}
$AA.DiscreteAnimation.prototype = {
    getAnimatedValue: function(percentage) {
        var index = Math.floor(this.interpolate(0, this._values.length - 1, percentage)); return this._values[index];
    },
    get_values: function() {
        return this._values;
    },
    set_values: function(value) {
        if (this._values != value) {
            this._values = value; this.raisePropertyChanged('values');
        }
    }
}
$AA.DiscreteAnimation.registerClass('AjaxControlToolkit.Animation.DiscreteAnimation', $AA.PropertyAnimation); $AA.registerAnimation('discrete', $AA.DiscreteAnimation); $AA.InterpolatedAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue) {
    $AA.InterpolatedAnimation.initializeBase(this, [target, duration, fps, ((property !== undefined) ? property : 'style'), propertyKey]); this._startValue = startValue; this._endValue = endValue;
}
$AA.InterpolatedAnimation.prototype = {
    get_startValue: function() {
        return this._startValue;
    },
    set_startValue: function(value) {
        value = this._getFloat(value); if (this._startValue != value) {
            this._startValue = value; this.raisePropertyChanged('startValue');
        }
    },
    get_endValue: function() {
        return this._endValue;
    },
    set_endValue: function(value) {
        value = this._getFloat(value); if (this._endValue != value) {
            this._endValue = value; this.raisePropertyChanged('endValue');
        }
    }
}
$AA.InterpolatedAnimation.registerClass('AjaxControlToolkit.Animation.InterpolatedAnimation', $AA.PropertyAnimation); $AA.registerAnimation('interpolated', $AA.InterpolatedAnimation); $AA.ColorAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue) {
    $AA.ColorAnimation.initializeBase(this, [target, duration, fps, property, propertyKey, startValue, endValue]); this._start = null; this._end = null; this._interpolateRed = false; this._interpolateGreen = false; this._interpolateBlue = false;
}
$AA.ColorAnimation.prototype = {
    onStart: function() {
        $AA.ColorAnimation.callBaseMethod(this, 'onStart'); this._start = $AA.ColorAnimation.getRGB(this.get_startValue()); this._end = $AA.ColorAnimation.getRGB(this.get_endValue()); this._interpolateRed = (this._start.Red != this._end.Red); this._interpolateGreen = (this._start.Green != this._end.Green); this._interpolateBlue = (this._start.Blue != this._end.Blue);
    },
    getAnimatedValue: function(percentage) {
        var r = this._start.Red; var g = this._start.Green; var b = this._start.Blue; if (this._interpolateRed)
            r = Math.round(this.interpolate(r, this._end.Red, percentage)); if (this._interpolateGreen)
            g = Math.round(this.interpolate(g, this._end.Green, percentage)); if (this._interpolateBlue)
            b = Math.round(this.interpolate(b, this._end.Blue, percentage)); return $AA.ColorAnimation.toColor(r, g, b);
    },
    set_startValue: function(value) {
        if (this._startValue != value) {
            this._startValue = value; this.raisePropertyChanged('startValue');
        }
    },
    set_endValue: function(value) {
        if (this._endValue != value) {
            this._endValue = value; this.raisePropertyChanged('endValue');
        }
    }
}
$AA.ColorAnimation.getRGB = function(color) {
    if (!color || color.length != 7) {
        throw String.format(AjaxControlToolkit.Resources.Animation_InvalidColor, color);
    }
    return { 'Red': parseInt(color.substr(1, 2), 16),
        'Green': parseInt(color.substr(3, 2), 16),
        'Blue': parseInt(color.substr(5, 2), 16)
    };
}
$AA.ColorAnimation.toColor = function(red, green, blue) {
    var r = red.toString(16); var g = green.toString(16); var b = blue.toString(16); if (r.length == 1) r = '0' + r; if (g.length == 1) g = '0' + g; if (b.length == 1) b = '0' + b; return '#' + r + g + b;
}
$AA.ColorAnimation.registerClass('AjaxControlToolkit.Animation.ColorAnimation', $AA.InterpolatedAnimation); $AA.registerAnimation('color', $AA.ColorAnimation); $AA.LengthAnimation = function(target, duration, fps, property, propertyKey, startValue, endValue, unit) {
    $AA.LengthAnimation.initializeBase(this, [target, duration, fps, property, propertyKey, startValue, endValue]); this._unit = (unit != null) ? unit : 'px';
}
$AA.LengthAnimation.prototype = {
    getAnimatedValue: function(percentage) {
        var value = this.interpolate(this.get_startValue(), this.get_endValue(), percentage); return Math.round(value) + this._unit;
    },
    get_unit: function() {
        return this._unit;
    },
    set_unit: function(value) {
        if (this._unit != value) {
            this._unit = value; this.raisePropertyChanged('unit');
        }
    }
}
$AA.LengthAnimation.registerClass('AjaxControlToolkit.Animation.LengthAnimation', $AA.InterpolatedAnimation); $AA.registerAnimation('length', $AA.LengthAnimation); $AA.MoveAnimation = function(target, duration, fps, horizontal, vertical, relative, unit) {
    $AA.MoveAnimation.initializeBase(this, [target, duration, fps, null]); this._horizontal = horizontal ? horizontal : 0; this._vertical = vertical ? vertical : 0; this._relative = (relative === undefined) ? true : relative; this._horizontalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'left', null, null, unit); this._verticalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'top', null, null, unit); this.add(this._verticalAnimation); this.add(this._horizontalAnimation);
}
$AA.MoveAnimation.prototype = {
    onStart: function() {
        $AA.MoveAnimation.callBaseMethod(this, 'onStart'); var element = this.get_target(); this._horizontalAnimation.set_startValue(element.offsetLeft); this._horizontalAnimation.set_endValue(this._relative ? element.offsetLeft + this._horizontal : this._horizontal); this._verticalAnimation.set_startValue(element.offsetTop); this._verticalAnimation.set_endValue(this._relative ? element.offsetTop + this._vertical : this._vertical);
    },
    get_horizontal: function() {
        return this._horizontal;
    },
    set_horizontal: function(value) {
        value = this._getFloat(value); if (this._horizontal != value) {
            this._horizontal = value; this.raisePropertyChanged('horizontal');
        }
    },
    get_vertical: function() {
        return this._vertical;
    },
    set_vertical: function(value) {
        value = this._getFloat(value); if (this._vertical != value) {
            this._vertical = value; this.raisePropertyChanged('vertical');
        }
    },
    get_relative: function() {
        return this._relative;
    },
    set_relative: function(value) {
        value = this._getBoolean(value); if (this._relative != value) {
            this._relative = value; this.raisePropertyChanged('relative');
        }
    },
    get_unit: function() {
        this._horizontalAnimation.get_unit();
    },
    set_unit: function(value) {
        var unit = this._horizontalAnimation.get_unit(); if (unit != value) {
            this._horizontalAnimation.set_unit(value); this._verticalAnimation.set_unit(value); this.raisePropertyChanged('unit');
        }
    }
}
$AA.MoveAnimation.registerClass('AjaxControlToolkit.Animation.MoveAnimation', $AA.ParallelAnimation); $AA.registerAnimation('move', $AA.MoveAnimation); $AA.ResizeAnimation = function(target, duration, fps, width, height, unit) {
    $AA.ResizeAnimation.initializeBase(this, [target, duration, fps, null]); this._width = width; this._height = height; this._horizontalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'width', null, null, unit); this._verticalAnimation = new $AA.LengthAnimation(target, duration, fps, 'style', 'height', null, null, unit); this.add(this._horizontalAnimation); this.add(this._verticalAnimation);
}
$AA.ResizeAnimation.prototype = {
    onStart: function() {
        $AA.ResizeAnimation.callBaseMethod(this, 'onStart'); var element = this.get_target(); this._horizontalAnimation.set_startValue(element.offsetWidth); this._verticalAnimation.set_startValue(element.offsetHeight); this._horizontalAnimation.set_endValue((this._width !== null && this._width !== undefined) ?
this._width : element.offsetWidth); this._verticalAnimation.set_endValue((this._height !== null && this._height !== undefined) ?
this._height : element.offsetHeight);
    },
    get_width: function() {
        return this._width;
    },
    set_width: function(value) {
        value = this._getFloat(value); if (this._width != value) {
            this._width = value; this.raisePropertyChanged('width');
        }
    },
    get_height: function() {
        return this._height;
    },
    set_height: function(value) {
        value = this._getFloat(value); if (this._height != value) {
            this._height = value; this.raisePropertyChanged('height');
        }
    },
    get_unit: function() {
        this._horizontalAnimation.get_unit();
    },
    set_unit: function(value) {
        var unit = this._horizontalAnimation.get_unit(); if (unit != value) {
            this._horizontalAnimation.set_unit(value); this._verticalAnimation.set_unit(value); this.raisePropertyChanged('unit');
        }
    }
}
$AA.ResizeAnimation.registerClass('AjaxControlToolkit.Animation.ResizeAnimation', $AA.ParallelAnimation); $AA.registerAnimation('resize', $AA.ResizeAnimation); $AA.ScaleAnimation = function(target, duration, fps, scaleFactor, unit, center, scaleFont, fontUnit) {
    $AA.ScaleAnimation.initializeBase(this, [target, duration, fps]); this._scaleFactor = (scaleFactor !== undefined) ? scaleFactor : 1; this._unit = (unit !== undefined) ? unit : 'px'; this._center = center; this._scaleFont = scaleFont; this._fontUnit = (fontUnit !== undefined) ? fontUnit : 'pt'; this._element = null; this._initialHeight = null; this._initialWidth = null; this._initialTop = null; this._initialLeft = null; this._initialFontSize = null;
}
$AA.ScaleAnimation.prototype = {
    getAnimatedValue: function(percentage) {
        return this.interpolate(1.0, this._scaleFactor, percentage);
    },
    onStart: function() {
        $AA.ScaleAnimation.callBaseMethod(this, 'onStart'); this._element = this.get_target(); if (this._element) {
            this._initialHeight = this._element.offsetHeight; this._initialWidth = this._element.offsetWidth; if (this._center) {
                this._initialTop = this._element.offsetTop; this._initialLeft = this._element.offsetLeft;
            }
            if (this._scaleFont) {
                this._initialFontSize = parseFloat(
$common.getCurrentStyle(this._element, 'fontSize'));
            }
        }
    },
    setValue: function(scale) {
        if (this._element) {
            var width = Math.round(this._initialWidth * scale); var height = Math.round(this._initialHeight * scale); this._element.style.width = width + this._unit; this._element.style.height = height + this._unit; if (this._center) {
                this._element.style.top = (this._initialTop +
Math.round((this._initialHeight - height) / 2)) + this._unit; this._element.style.left = (this._initialLeft +
Math.round((this._initialWidth - width) / 2)) + this._unit;
            }
            if (this._scaleFont) {
                var size = this._initialFontSize * scale; if (this._fontUnit == 'px' || this._fontUnit == 'pt') {
                    size = Math.round(size);
                }
                this._element.style.fontSize = size + this._fontUnit;
            }
        }
    },
    onEnd: function() {
        this._element = null; this._initialHeight = null; this._initialWidth = null; this._initialTop = null; this._initialLeft = null; this._initialFontSize = null; $AA.ScaleAnimation.callBaseMethod(this, 'onEnd');
    },
    get_scaleFactor: function() {
        return this._scaleFactor;
    },
    set_scaleFactor: function(value) {
        value = this._getFloat(value); if (this._scaleFactor != value) {
            this._scaleFactor = value; this.raisePropertyChanged('scaleFactor');
        }
    },
    get_unit: function() {
        return this._unit;
    },
    set_unit: function(value) {
        if (this._unit != value) {
            this._unit = value; this.raisePropertyChanged('unit');
        }
    },
    get_center: function() {
        return this._center;
    },
    set_center: function(value) {
        value = this._getBoolean(value); if (this._center != value) {
            this._center = value; this.raisePropertyChanged('center');
        }
    },
    get_scaleFont: function() {
        return this._scaleFont;
    },
    set_scaleFont: function(value) {
        value = this._getBoolean(value); if (this._scaleFont != value) {
            this._scaleFont = value; this.raisePropertyChanged('scaleFont');
        }
    },
    get_fontUnit: function() {
        return this._fontUnit;
    },
    set_fontUnit: function(value) {
        if (this._fontUnit != value) {
            this._fontUnit = value; this.raisePropertyChanged('fontUnit');
        }
    }
}
$AA.ScaleAnimation.registerClass('AjaxControlToolkit.Animation.ScaleAnimation', $AA.Animation); $AA.registerAnimation('scale', $AA.ScaleAnimation); $AA.Action = function(target, duration, fps) {
    $AA.Action.initializeBase(this, [target, duration, fps]); if (duration === undefined) {
        this.set_duration(0);
    }
}
$AA.Action.prototype = {
    onEnd: function() {
        this.doAction(); $AA.Action.callBaseMethod(this, 'onEnd');
    },
    doAction: function() {
        throw Error.notImplemented();
    },
    getAnimatedValue: function() {
    },
    setValue: function() {
    }
}
$AA.Action.registerClass('AjaxControlToolkit.Animation.Action', $AA.Animation); $AA.registerAnimation('action', $AA.Action); $AA.EnableAction = function(target, duration, fps, enabled) {
    $AA.EnableAction.initializeBase(this, [target, duration, fps]); this._enabled = (enabled !== undefined) ? enabled : true;
}
$AA.EnableAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            element.disabled = !this._enabled;
        }
    },
    get_enabled: function() {
        return this._enabled;
    },
    set_enabled: function(value) {
        value = this._getBoolean(value); if (this._enabled != value) {
            this._enabled = value; this.raisePropertyChanged('enabled');
        }
    }
}
$AA.EnableAction.registerClass('AjaxControlToolkit.Animation.EnableAction', $AA.Action); $AA.registerAnimation('enableAction', $AA.EnableAction); $AA.HideAction = function(target, duration, fps, visible) {
    $AA.HideAction.initializeBase(this, [target, duration, fps]); this._visible = visible;
}
$AA.HideAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            $common.setVisible(element, this._visible);
        }
    },
    get_visible: function() {
        return this._visible;
    },
    set_visible: function(value) {
        if (this._visible != value) {
            this._visible = value; this.raisePropertyChanged('visible');
        }
    }
}
$AA.HideAction.registerClass('AjaxControlToolkit.Animation.HideAction', $AA.Action); $AA.registerAnimation('hideAction', $AA.HideAction); $AA.StyleAction = function(target, duration, fps, attribute, value) {
    $AA.StyleAction.initializeBase(this, [target, duration, fps]); this._attribute = attribute; this._value = value;
}
$AA.StyleAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            element.style[this._attribute] = this._value;
        }
    },
    get_attribute: function() {
        return this._attribute;
    },
    set_attribute: function(value) {
        if (this._attribute != value) {
            this._attribute = value; this.raisePropertyChanged('attribute');
        }
    },
    get_value: function() {
        return this._value;
    },
    set_value: function(value) {
        if (this._value != value) {
            this._value = value; this.raisePropertyChanged('value');
        }
    }
}
$AA.StyleAction.registerClass('AjaxControlToolkit.Animation.StyleAction', $AA.Action); $AA.registerAnimation('styleAction', $AA.StyleAction); $AA.OpacityAction = function(target, duration, fps, opacity) {
    $AA.OpacityAction.initializeBase(this, [target, duration, fps]); this._opacity = opacity;
}
$AA.OpacityAction.prototype = {
    doAction: function() {
        var element = this.get_target(); if (element) {
            $common.setElementOpacity(element, this._opacity);
        }
    },
    get_opacity: function() {
        return this._opacity;
    },
    set_opacity: function(value) {
        value = this._getFloat(value); if (this._opacity != value) {
            this._opacity = value; this.raisePropertyChanged('opacity');
        }
    }
}
$AA.OpacityAction.registerClass('AjaxControlToolkit.Animation.OpacityAction', $AA.Action); $AA.registerAnimation('opacityAction', $AA.OpacityAction); $AA.ScriptAction = function(target, duration, fps, script) {
    $AA.ScriptAction.initializeBase(this, [target, duration, fps]); this._script = script;
}
$AA.ScriptAction.prototype = {
    doAction: function() {
        try {
            eval(this._script);
        } catch (ex) {
        }
    },
    get_script: function() {
        return this._script;
    },
    set_script: function(value) {
        if (this._script != value) {
            this._script = value; this.raisePropertyChanged('script');
        }
    }
}
$AA.ScriptAction.registerClass('AjaxControlToolkit.Animation.ScriptAction', $AA.Action); $AA.registerAnimation('scriptAction', $AA.ScriptAction);
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
