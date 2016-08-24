(function($) {

    var $t = $.telerik;

    var decimals = { '190': '.', '110': '.', '188': ',' };
    var keycodes = [8, 9, 37, 38, 39, 40, 46, 35, 36, 44]; //44 is ","

    $t.textbox = function(element, options) {
        this.element = element;

        $.extend(this, options);

        var builder = new $t.stringBuilder();
        builder.cat('[ |')
               .cat(this.groupSeparator)
               .catIf('|' + this.symbol, this.symbol)
               .cat(']');
        this.replaceRegExp = new RegExp(builder.string(), 'g');

        var pasteMethod = $.browser.msie ? 'paste' : 'input';

        var input = $('.t-input', element).hide();
        var inputValue = input.attr('value');

        $('<input>', {
            id: input.attr('id') + "-text",
            name: input.attr('name') + "-text",
            'class': input.attr('class'),
            value: (inputValue || this.text)
        })
        .bind({
            blur: $t.delegate(this, this.blur),
            focus: $t.delegate(this, this.focus),
            keydown: $t.delegate(this, this.keydown),
            keypress: $t.delegate(this, this.keypress),
            change: function(e) { e.stopPropagation(); return false; }
        })
        .bind(pasteMethod, $t.delegate(this, this[pasteMethod]))
        .insertAfter(input);

        var buttons = $('.t-arrow-up, .t-arrow-down', element)
                        .bind({
                            mouseup: $t.delegate(this, this.clearTimer),
                            mouseout: $t.delegate(this, this.clearTimer),
                            click: $t.preventDefault,
                            dragstart: $t.preventDefault,
                            dblclick: $t.delegate(this, this.clearTimer)
                        });

        $(buttons[0]).mousedown($.proxy(function(e) {
            this.isChange();
            this.stepper(e, 1);
        }, this));

        $(buttons[1]).mousedown($.proxy(function(e) {
            this.isChange();
            this.stepper(e, -1);
        }, this));

        var separator = this.separator;
        this.step = this.parse(this.step, separator);
        this.val = this.parse(this.val, separator);
        this.minValue = this.parse(this.minValue, separator);
        this.maxValue = this.parse(this.maxValue, separator);

        if (inputValue != '') //format the input value if it exists.
            this.value(inputValue);

        $t.bind(this, {
            change: this.onChange,
            load: this.onLoad
        });

        $t.trigger(element, 'load');
    }

    $t.textbox.prototype = {
        isChange: function() {
            var value = $('> .t-input:last', this.element).val();
            if (this.val != value.replace(this.replaceRegExp, ''))
                this.value(value)
        },
        input: function(e, element) {

            var val = $(element).val();

            if (val == '-') return true;

            var parsedValue = this.parse(val, this.separator);

            if (parsedValue || parsedValue == 0)
                this.trigger(this.round(parsedValue, this.digits));
        },

        paste: function(e, element) {

            var $input = $(element);
            var val = $input.val();

            var selectedText = element.document.selection.createRange().text;
            var text = window.clipboardData.getData("Text");

            if (selectedText > 0) val = val.replace(selectedText, text);
            else val += text;


            var parsedValue = this.parse(val, this.separator);
            if (parsedValue || parsedValue == 0)
                this.trigger(this.round(parsedValue, this.digits));
        },

        focus: function(e, element) {
            this.focused = true;
            this.isChange();

            var value = this.formatEdit(this.val);
            $(element).val(value || (value == 0 ? 0 : ''));
        },

        blur: function(e) {
            var $input = $(e.target);

            this.focused = false;
            var inputValue = $input.val();
            if (!inputValue && inputValue != '0' || !this.val && this.val != 0) {
                this.value(null);
                $input.removeClass('t-state-error')
                      .val(this.text || '');
                return true;
            } else {
                if (this.range(this.val, this.minValue, this.maxValue)) {
                    $input.removeClass('t-state-error')
                          .val(this.format(this.round(this.val, this.digits)));
                }
                else {
                    $input.addClass('t-state-error');
                }
            }
        },

        keydown: function(e, element) {
            var key = e.keyCode;
            var $input = $(element);
            var separator = this.separator;

            // Allow decimal
            var decimalSeparator = decimals[key];
            if (decimalSeparator) {
                if (decimalSeparator == separator && this.digits > 0
                    && this.caretPos($input[0]) != 0 && $input.val().indexOf(separator) == -1) {
                    return true;
                } else {
                    e.preventDefault();
                }
            }

            if (key == 8 || key == 46) { //backspace and delete
                setTimeout($t.delegate(this, function() {
                    this.parseTrigger($input.val())
                }));
                return true;
            }

            if (key == 38 || key == 40) {
                this.modifyInput($input, this.step * (key == 38 ? 1 : -1));
                return true;
            }

            if (key == 222) e.preventDefault();
        },

        keypress: function(e) {
            var $input = $(e.target);
            var key = e.keyCode || e.which;

            if (key == 0 || $.inArray(key, keycodes) != -1 || e.ctrlKey || (e.shiftKey && key == 45)) return true;

            if ((this.minValue < 0 && String.fromCharCode(key) == "-"
                && this.caretPos($input[0]) == 0 && $input.val().indexOf("-") == -1)
                || this.range(key, 48, 57)) {
                setTimeout($t.delegate(this, function() {
                    this.parseTrigger($input.val());
                }));
                return true;
            }

            e.preventDefault();
        },

        clearTimer: function(e) {
            clearTimeout(this.timeout);
            clearInterval(this.timer);
            clearInterval(this.acceleration);
        },

        stepper: function(e, stepMod) {
            if (e.which == 1) {
                var input = $('.t-input:last', this.element);
                var step = this.step;

                this.modifyInput(input, stepMod * step);

                this.timeout = setTimeout($t.delegate(this, function() {
                    this.timer = setInterval($t.delegate(this, function() {
                        this.modifyInput(input, stepMod * step);
                    }), 80);

                    this.acceleration = setInterval(function() { step += 1; }, 1000);
                }), 200);
            }
        },

        value: function(value) {
            if (arguments.length == 0) return this.val;

            var parsedValue = (typeof value === typeof 1) ? value : this.parse(value, this.separator);
            var isNull = parsedValue === null;

            this.val = parsedValue;
            $('.t-input:first', this.element).val(isNull ? '' : this.formatEdit(parsedValue));
            $('.t-input:last', this.element)
                    .toggleClass('t-state-error', !this.range(this.val, this.minValue, this.maxValue))
                    .val(isNull ? this.text : this.format(parsedValue));
            return this;
        },

        modifyInput: function($input, step) {

            var value = this.val;
            var min = this.minValue;
            var max = this.maxValue;

            value = value ? value + step : step;
            value = value < min ? min : value > max ? max : value;

            var fixedValue = this.round(value, this.digits);

            this.trigger(fixedValue);

            var formatedValue = this.focused ? this.formatEdit(fixedValue) : this.format(fixedValue);

            $input.removeClass('t-state-error').val(formatedValue);
        },

        formatEdit: function(value) {
            var separator = this.separator;
            if (value && separator != '.')
                value = value.toString().replace('.', separator);
            return value;
        },

        format: function(value) {
            var tokens = value.toString().split('.');
            var number = tokens[0].replace('-', '');
            var builder = new $t.stringBuilder();

            if (this.groupSeparator != '')
                for (var i = 0, j = number.length; i < j; i++) {
                builder.catIf(this.groupSeparator, i != 0 && (j - i) % this.groupSize == 0)
                           .cat(number.charAt(i));
            }
            else
                builder.cat(number);

            if (tokens.length > 1) builder.cat(this.separator).cat(tokens[1]);

            var pattern = this.val < 0 ? this.patterns['negative'][this.negative]
                        : this.symbol ? this.patterns['positive'][this.positive]
                        : null;

            return pattern ? pattern.replace('n', builder.string()).replace('*', this.symbol) : builder.string();
        },

        trigger: function(newValue) {
            if (this.val != newValue) {
                $t.trigger(this.element, 'change', { oldValue: this.val, newValue: newValue });
                $('.t-input:first', this.element).val(this.formatEdit(newValue));
                this.val = newValue;
            }
        },

        parseTrigger: function(value) {
            var parsedValue = this.parse(value, this.separator);
            if (parsedValue || parsedValue == 0)
                this.trigger(this.round(parsedValue, this.digits));
        },

        caretPos: function(element) {
            var pos = -1;

            if (document.selection) {
                var selection = element.document.selection.createRange();
                selection.moveStart('character', -element.value.length);
                pos = selection.text.length;
            } else if (element.selectionStart || element.selectionStart == "0") {
                pos = element.selectionStart;
            }

            return pos;
        },

        range: function(key, min, max) { return key >= min && key <= max; },

        parse: function(value, separator) {
            var result = null;
            if (value || value == "0") {
                if (typeof value == typeof 1) return value;

                value = value.replace(this.replaceRegExp, '');
                if (separator && separator != '.')
                    value = value.replace(separator, '.');

                result = parseFloat(value);
            }
            return isNaN(result) ? null : result;
        },

        round: function(value, digits) {
            if (value || value == 0)
                return parseFloat(value.toFixed(digits || 2));
            return null;
        }
    }

    $.fn.tTextBox = function(options) {

        var type = options.type;
        var defaults = $.fn.tTextBox.defaults[type];

        if ($t.cultureInfo[type + 'decimaldigits']) {
            defaults.digits = $t.cultureInfo[type + 'decimaldigits'];
            defaults.separator = $t.cultureInfo[type + 'decimalseparator'];
            defaults.groupSeparator = $t.cultureInfo[type + 'groupseparator'];
            defaults.groupSize = $t.cultureInfo[type + 'groupsize'];
            defaults.positive = $t.cultureInfo[type + 'positive'];
            defaults.negative = $t.cultureInfo[type + 'negative'];
            defaults.symbol = $t.cultureInfo[type + 'symbol'];
        }

        options = $.extend({}, defaults,
                           { 'patterns': $.fn.tTextBox.patterns[type] },
                           options);

        return this.each(function() {
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            if (!$(this).data('tTextBox'))
                $(this).data('tTextBox', new $t.textbox(this, options));
        });
    };

    $.fn.tTextBox.defaults = {
        numeric: {
            val: null,
            minValue: -100,
            maxValue: 100,
            step: 1,
            digits: 2,
            separator: '.',
            groupSeparator: ',',
            groupSize: 3,
            negative: 1
        },
        currency: {
            val: null,
            minValue: 0,
            maxValue: 1000,
            step: 1,
            symbol: '$',
            digits: 2,
            separator: '.',
            groupSeparator: ',',
            groupSize: 3,
            positive: 0,
            negative: 0
        },
        percent: {
            val: null,
            minValue: 0,
            maxValue: 100,
            step: 1,
            symbol: '%',
            digits: 2,
            separator: '.',
            groupSeparator: ',',
            groupSize: 3,
            positive: 0,
            negative: 0
        }
    };

    // * - placeholder for the symbol
    // n - placeholder for the number
    $.fn.tTextBox.patterns = {
        numeric: {
            negative: ['(n)', '-n', '- n', 'n-', 'n -']
        },
        currency: {
            positive: ['*n', 'n*', '* n', 'n *'],
            negative: ['(*n)', '-*n', '*-n', '*n-', '(n*)', '-n*', 'n-*', 'n*-', '-n *', '-* n', 'n *-', '* n-', '* -n', 'n- *', '(* n)', '(n *)']
        },
        percent: {
            positive: ['n *', 'n*', '*n'],
            negative: ['-n *', '-n*', '-*n']
        }
    };
})(jQuery);