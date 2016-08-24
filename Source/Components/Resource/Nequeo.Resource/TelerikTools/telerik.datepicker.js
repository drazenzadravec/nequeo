(function($) {

    var $t = $.telerik;

    var sharedCalendar = null;
    var dateCheck = /\d/;

    $t.datepicker = function(element, options) {
        this.element = element;
        this.isValueChanged = false;

        $.extend(this, options);

        $('> .t-icon', element)
            .bind('click', $t.delegate(this, this.togglePopup))

        var input = $('.t-input', element)
                        .keydown($t.delegate(this, this.keyDown))
                        .focus($t.delegate(this, this.show))
                        .attr('autocomplete', 'off');

        this.focusedDate = this.selectedDate || ($t.calendar.isInRange(this.focusedDate, this.minDate, this.maxDate)
                                                                ? this.focusedDate : new $t.datetime(this.minDate.value));

        $t.bind(this, {
            open: this.onOpen,
            close: this.onClose,
            change: this.onChange,
            load: this.onLoad
        });

        $t.trigger(element, 'load');
    }

    $.extend($t.datepicker, {
        hideSharedCalendar: function(e) {

            var associatedDatePicker = sharedCalendar.data('associatedDatePicker');

            if (associatedDatePicker) {
                if ($.contains(associatedDatePicker, e.target))
                    return;

                var datepicker = $(associatedDatePicker).data('tDatePicker');

                if (datepicker) {
                    datepicker.parseDate($('.t-input', associatedDatePicker).val());
                    datepicker.hide();
                }
            }
        },

        adjustDate: function(viewIndex, date, monthValue, otherViewValue) {
            if (viewIndex == 0)
                $t.datetime.modify(date, $t.datetime.msPerDay * monthValue);
            else if (viewIndex == 1)
                date.addMonth(otherViewValue);
            else
                date.addYear((viewIndex == 2 ? otherViewValue : 10 * otherViewValue));
        }
    });

    $t.datepicker.prototype = {
        $calendar: function() {
            if (!sharedCalendar) {
                sharedCalendar = $($t.calendar.html(this.focusedDate, this.selectedDate, this.minDate, this.maxDate))
                                    .hide()
                                    .addClass('t-datepicker-calendar')
                                    .bind('click', function(e) { e.stopPropagation(); })
                                    .appendTo(document.body)
                                    .tCalendar({
                                        selectedDate: this.selectedDate,
                                        minDate: this.minDate,
                                        maxDate: this.maxDate
                                    });
                $(document).click($t.datepicker.hideSharedCalendar);
            }

            if (sharedCalendar.data('associatedDatePicker') != this.element) {

                calendar = sharedCalendar.data('tCalendar');
                calendar.minDate = this.minDate;
                calendar.maxDate = this.maxDate;
                calendar.selectedDate = this.selectedDate;
                calendar.goToView(0, this.focusedDate);

                // reposition & rewire the shared calendar

                var elementPosition = $(this.element).offset();

                elementPosition.top += $(this.element).height();

                var animationContainer = $t.fx._wrap(sharedCalendar);

                animationContainer.css($.extend({
                    position: 'absolute'
                }, elementPosition));

                sharedCalendar
                    .unbind('change')
                    .bind('change', $.proxy(this.calendarChange, this))
                    .unbind('navigate')
                    .bind('navigate', $.proxy(this.viewedMonthChanged, this))
                    .data('associatedDatePicker', this.element);

                if (this.selectedDate)
                    this.value(this.focusedDate); //if selectedDate - the focusedDate = selectedDate.
            }

            var viewIndex = calendar.currentView.index;

            if (!sharedCalendar.is(':visible') && calendar.viewedMonth.value - this.focusedDate.value != 0) {
                calendar.goToView(viewIndex, this.focusedDate)
                        .value(this.selectedDate);
            }

            $t.calendar.focusDate(this.focusedDate, viewIndex, sharedCalendar);

            return sharedCalendar;
        },

        isOpened: function() {
            return sharedCalendar && sharedCalendar.data('associatedDatePicker') == this.element && sharedCalendar.is(':visible');
        },

        viewedMonthChanged: function(e) {

            var calendar = sharedCalendar.data('tCalendar');
            var viewedMonth = calendar.viewedMonth;
            var viewIndex = calendar.currentView.index;

            if (viewIndex == 0) {
                this.focusedDate = this.selectedDate || this.focusedDate;
            } else {
                this.focusedDate.year(viewedMonth.year(), viewedMonth.month(), this.focusedDate.date());
            }
            $t.calendar.focusDate(this.focusedDate, viewIndex, sharedCalendar, e.direction);
        },

        value: function(date) {
            if (arguments.length == 0) return this.selectedDate === null ? null : this.selectedDate.toDate();

            var parsedValue = date === null ? null : date.getDate || date.value ? date : this.parse(date);
            var isNull = parsedValue === null;

            this.selectedDate = isNull ? null : parsedValue.value ? parsedValue : new $t.datetime(parsedValue);
            if (!isNull) this.focusedDate = this.selectedDate;

            $('.t-input', this.element).val(isNull ? '' : $t.calendar.formatDate(this.selectedDate.toDate(), this.format));

            if (this.isOpened())
                this.$calendar().data('tCalendar').value(this.selectedDate);

            return this;
        },

        calendarChange: function(e) {

            var newlySelectedDate = new $t.datetime(e.date);

            if (this.checkSelectedDate(this.selectedDate, newlySelectedDate))
                return this;

            $('.t-input', this.element).removeClass('t-state-error');
            this.hide();
        },

        checkSelectedDate: function(selectedDate, newlySelectedDate) {
            if (!selectedDate || (selectedDate.value > newlySelectedDate.value || newlySelectedDate.value > selectedDate.value)) {

                this.value(newlySelectedDate);

                return $t.trigger(this.element, 'change', {
                    previousDate: selectedDate === null ? null : selectedDate.toDate(),
                    date: newlySelectedDate.toDate()
                });
            }
        },

        togglePopup: function(e) {
            e.preventDefault();

            if (this.isOpened()) {
                var $input = $('.t-input', this.element);
                this.parseDate($input.val());
                $input.blur();
                this.hide();
            } else {
                $('.t-input', this.element).focus();
            }
        },

        showPopup: function() {
            var parsedValue = this.parse($(':input', this.element).val());
            this.selectedDate = parsedValue;
            if (parsedValue !== null)
                this.focusedDate = new $t.datetime(parsedValue.value);

            $t.fx.play(this.effects, this.$calendar(), { direction: 'bottom' });
        },

        hidePopup: function() {
            if (this.isOpened())
                $t.fx.rewind(this.effects, this.$calendar(), { direction: 'bottom' });
        },

        show: function() {
            $t.trigger(this.element, 'open');
            this.showPopup();
        },

        hide: function() {
            if (this.isOpened())
                $t.trigger(this.element, 'close');
            this.hidePopup();
        },

        keyDown: function(e) {
            var inputValue = $(e.target).val();

            if (e.keyCode == 9) {
                this.parseDate(inputValue);
                this.hide();
            }

            if (e.keyCode == 27)
                this.hide();

            if (this.isOpened() && $('.t-overlay', sharedCalendar).length > 0)
                return;

            var isFuture;
            var isNavProcessed = false;
            var $calendar = this.$calendar();
            var calendar = $calendar.data('tCalendar');
            var viewedMonth = calendar.viewedMonth;
            var currentView = calendar.currentView;
            var viewIndex = calendar.currentView.index;
            var date = new $t.datetime(this.focusedDate.value)

            var navigate = function(className, method, futureNav) {
                if (!$(className, $calendar).hasClass('t-state-disabled')) {
                    if ('navigateUp' == method) viewIndex += 1;
                    isFuture = futureNav || false;
                    calendar[method]();
                    return true;
                }
                else return false;
            }

            var navigateDown = function() {
                var target = $t.calendar.findTarget(date, viewIndex, $calendar, false)[0];
                calendar.navigateDown(e, target, viewIndex);
                viewIndex = viewIndex == 0 ? 0 : viewIndex - 1;
                isFuture = true;
            }

            var navPrevNext = function(className, method, futureNav) {
                var diff = !futureNav ? -1 : 1;
                if (!navigate(className, method, futureNav)) return false;
                if (viewIndex == 0)
                    date.addMonth(diff);
                else
                    date.addYear(diff * (viewIndex == 1 ? 1 : viewIndex == 2 ? 10 : 100));
                return true;
            }

            if ($calendar.is(':visible') && !e.shiftKey) {
                switch (e.keyCode) {
                    case 37:
                        isNavProcessed = true;
                        if (e.ctrlKey) {
                            if (!navPrevNext('.t-nav-prev', 'navigateToPast')) return;
                        } else {
                            $t.datepicker.adjustDate(viewIndex, date, -1, -1); // date modified by reference
                            if (currentView.navCheck(date, viewedMonth, false))
                                if (!navigate('.t-nav-prev', 'navigateToPast')) return;
                        }
                        break;
                    case 38:
                        isNavProcessed = true;
                        if (e.ctrlKey) {
                            navigate('.t-nav-fast', 'navigateUp');
                        } else {
                            $t.datepicker.adjustDate(viewIndex, date, -7, -4); // date modified by reference
                            if (currentView.navCheck(date, viewedMonth, false))
                                if (!navigate('.t-nav-prev', 'navigateToPast')) return;
                        }
                        break;
                    case 39:
                        isNavProcessed = true;
                        if (e.ctrlKey) {
                            if (!navPrevNext('.t-nav-next', 'navigateToFuture', true)) return;
                        } else {
                            $t.datepicker.adjustDate(viewIndex, date, 1, 1); // date modified by reference
                            if (currentView.navCheck(date, viewedMonth, true))
                                if (!navigate('.t-nav-next', 'navigateToFuture', true)) return;
                        }
                        break;
                    case 40:
                        isNavProcessed = true;
                        if (e.ctrlKey) {
                            navigateDown();
                        } else {
                            $t.datepicker.adjustDate(viewIndex, date, 7, 4); // date modified by reference
                            if (currentView.navCheck(date, viewedMonth, true))
                                if (!navigate('.t-nav-next', 'navigateToFuture', true)) return;
                        }
                        break;
                    case 33:
                        if (!navPrevNext('.t-nav-prev', 'navigateToPast')) return;
                        isNavProcessed = true;
                        break;
                    case 34:
                        if (!navPrevNext('.t-nav-next', 'navigateToFuture', true)) return;
                        isNavProcessed = true;
                        break;
                    case 35: //end
                        date = $t.calendar.views[viewIndex].firstLastDay(date, false, calendar);
                        isNavProcessed = true;
                        break;
                    case 36: //home
                        date = $t.calendar.views[viewIndex].firstLastDay(date, true, calendar);
                        isNavProcessed = true;
                        break;
                    case 13:
                        if (this.isValueChanged) {
                            this.parseDate(inputValue);
                            this.isValueChanged = false;
                            break;
                        }
                        isNavProcessed = true;
                        if (viewIndex == 0) {
                            $(e.target).removeClass('t-state-error');
                            if (this.checkSelectedDate(this.selectedDate, this.focusedDate))
                                return;
                            this.hide();
                        } else navigateDown();
                        break;
                }
            } else {
                if (e.altKey && e.keyCode == 40) {
                    this.show();
                    var result = this.parseDate(inputValue);
                    if (inputValue != "" && result === null)
                        isNavProcessed = true;
                } else if (e.keyCode == 13) {
                    this.parseDate(inputValue);
                }
            }

            if (isNavProcessed) {
                e.preventDefault();
                date = $t.calendar.fitDateToRange(date, this.minDate, this.maxDate);

                $t.calendar.focusDate(date, viewIndex, $calendar, isFuture);
                this.focusedDate = date;
            } else {
                var key = e.keyCode;
                var isInRange = function(code, min, max) { return code > min && code < max; }
                if (isInRange(key, 47, 57) || isInRange(key, 65, 90) || isInRange(key, 95, 105)
                    || key == 8 || key == 32 || key == 47)
                    this.isValueChanged = true;
            }
        },

        parseDate: function(value) {
            var result = null;
            var setNull = function() {
                this.selectedDate = null;
                if (this.isOpened()) {
                    this.$calendar().data('tCalendar').selectedDate = null;
                    $('.t-state-selected', this.$calendar()).removeClass('t-state-selected');
                }
            }
            if (value != "") {
                result = this.parse(value, this.format);
                var isNull = result === null;

                if (!isNull) {
                    this.focusedDate = result;

                    if (this.checkSelectedDate(this.selectedDate, result))
                        return;
                } else {
                    $.proxy(setNull, this)();
                }

                $('.t-input', this.element)
                    .toggleClass('t-state-error', isNull)
                    .val(isNull ? value : $t.calendar.formatDate(result.toDate(), this.format));
            } else {
                $('.t-input', this.element).removeClass('t-state-error');
                $.proxy(setNull, this)();
            }
            return result;
        },

        parse: function(value, format, today) {
            return dateCheck.test(value) ? this.parseMachineDate(value, format) : this.parseByToken(value, today);
        },

        parseMachineDate: function(value, format) {
            format = format || this.format; // required for unit tests
            var year = -1;
            var month = -1;
            var day = -1;
            var shortYearCutoff = '+10';
            var literal = false;

            // Returns count of the format character in the date format string
            var lookAhead = function(match) {
                var index = 0;
                while (Matches(match)) {
                    index++;
                    formatPosition++
                }
                return index;
            };
            var lookForLiteral = function() {
                var matches = Matches("'");
                if (matches)
                    formatPosition++;
                return matches;
            };
            var Matches = function(match) {
                return (formatPosition + 1 < format.length && format.charAt(formatPosition + 1) == match);
            }
            // Extract a number from the string value
            var getNumber = function(match) {
                var size = match == 'y' ? 4 : 2;
                var digits = new RegExp('^\\d{1,' + size + '}');
                var num = value.substring(currentTokenIndex).match(digits);
                if (num) {
                    currentTokenIndex += num[0].length;
                    return parseInt(num[0], 10);
                } else {
                    return -1;
                }
            };
            // Extract a name from the string value and convert to an index
            var getName = function(names) {
                for (var i = 0; i < names.length; i++) {
                    if (value.substr(currentTokenIndex, names[i].length) == names[i]) {
                        currentTokenIndex += names[i].length;
                        return i + 1;
                    }
                }
                return -1;
            };

            var checkLiteral = function() {
                if (value.charAt(currentTokenIndex) == format.charAt(formatPosition)) {
                    currentTokenIndex++;
                }
            };
            var currentTokenIndex = 0;
            for (var formatPosition = 0, flength = format.length; formatPosition < flength; formatPosition++) {
                if (literal) {
                    checkLiteral();
                    if (format.charAt(formatPosition) == "'")
                        literal = false;
                } else {
                    switch (format.charAt(formatPosition)) {
                        case 'd':
                            var count = lookAhead('d');
                            day = count <= 1 ? getNumber('d') : getName($t.cultureInfo[count == 3 ? 'days' : 'abbrDays']);
                            break;
                        case 'M':
                            var count = lookAhead('M');
                            month = count <= 1 ? getNumber('M') : getName($t.cultureInfo[count == 3 ? 'months' : 'abbrMonths']);
                            break;
                        case 'y':
                            lookAhead('y')
                            year = getNumber('y');
                            break;
                        case "'":
                            checkLiteral();
                            literal = true;
                            break;
                        default:
                            checkLiteral();
                    }
                }
            }

            if (currentTokenIndex < value.length - 1)
                return null;

            var tempDate = new $t.datetime();
            if (year == -1)
                year = tempDate.year();
            else if (year < 100)
                year += tempDate.year() - tempDate.year() % 100 +
                                (year <= shortYearCutoff ? 0 : -100);

            var date = new $t.datetime(year, month - 1, day);

            // honor DST
            date.hours(date.hours() > 12 ? date.hours() + 2 : 0);

            if (date.year() != year && date.month() + 1 != month && date.date() != day)
                return null;

            date = $t.calendar.isInRange(date, this.minDate, this.maxDate) ? date : null;

            return date;
        },

        parseByToken: function(value, today) {
            today = today || new $t.datetime(); // required for unit tests
            var firstToken = null;
            var secondToken = null;
            var tokenType = null;
            var pos = 0;

            var Matches = function(name) {
                var token = null;
                if (name && value.substring(pos, pos + name.length).toLowerCase() == name.toLowerCase()) {
                    token = name;
                }
                return token;
            }

            var searchForDayMonth = function() {
                var token = null;
                $.each(['days', 'abbrDays', 'months', 'abbrMonths'], function(index, key) {
                    if (token !== null) return;

                    $.each($t.cultureInfo[key], function(index, name) {
                        if (token !== null) return;
                        token = Matches(name);
                    });

                    tokenType = key;
                });
                return token;
            }

            var adjustDate = function() {
                var gap;
                var modifyDate = function(mod, isday) {
                    today[isday ? 'date' : 'month']
                    (today[isday ? 'date' : 'month']()
                     + (gap != 0 ? ((gap + ((gap > 0 ? 1 : -1) * mod)) % mod) : 0)
                        + (secondToken ?
                            (firstToken == $t.cultureInfo['next'] ? 1 : -1) * mod : 0));
                }
                var arrayPosition = $.inArray(secondToken || firstToken, $t.cultureInfo[tokenType]);
                if (tokenType.toLowerCase().indexOf('day') > -1) {
                    gap = (arrayPosition == 0 ? 7 : arrayPosition) - today.day();
                    modifyDate(7, true)
                } else {
                    gap = arrayPosition - today.month();
                    modifyDate(12, false)
                }
            }

            var adjustDateBySecondToken = function() {
                var gapDiff = function(possition) {
                    var gap;
                    switch (secondToken) {
                        case 'year': gap = possition == 1 ? 1 : 0; break;
                        case 'month': gap = possition == 2 ? 1 : 0; break;
                        case 'week': gap = possition == 3 ? 7 : 0; break;
                        case 'day': gap = possition == 3 ? 1 : 0; break;
                    }
                    return gap;
                }
                var direction = (firstToken == $t.cultureInfo['next'] ? 1 : -1);
                today.year(
                    today.year() + gapDiff(1) * direction,
                    today.month() + gapDiff(2) * direction,
                    today.date() + gapDiff(3) * direction
                );
            }

            // search for first token
            $.each(['today', 'tomorrow', 'yesterday', 'next', 'last'], function(index, name) {
                if (firstToken !== null) return;
                firstToken = Matches($t.cultureInfo[name]);
            })

            if (firstToken !== null) {
                pos += firstToken.length;

                if (/[^\s\d]\s+[^\s\d]/i.test(value)) {
                    pos++;
                    $.each(['year', 'month', 'week', 'day'], function(index, name) {
                        if (secondToken !== null) return;
                        secondToken = Matches($t.cultureInfo[name]);
                    })
                    tokenType = null;

                    if (secondToken === null) {
                        secondToken = searchForDayMonth();
                    }
                    if (secondToken === null)
                        return null; // invalid date.
                } else {
                    switch (firstToken) {
                        case $t.cultureInfo['today']: break;
                        case $t.cultureInfo['tomorrow']:
                            today.date(today.date() + 1);
                            break;
                        case $t.cultureInfo['yesterday']:
                            today.date(today.date() - 1);
                            break;
                        default:
                            today = null; // incorrect token
                            break;
                    }
                    today = $t.calendar.isInRange(today, this.minDate, this.maxDate) ? today : null;
                    return today;
                }

            } else {
                firstToken = searchForDayMonth();
                if (firstToken != null) {
                    adjustDate();
                    today = $t.calendar.isInRange(today, this.minDate, this.maxDate) ? today : null;
                    return today;
                } else {
                    return null;
                }
            }

            // first and second tokens are not null
            if (tokenType !== null)
                adjustDate();
            else // second token is year, month, week, day
                adjustDateBySecondToken();

            today = $t.calendar.isInRange(today, this.minDate, this.maxDate) ? today : null;

            return today;
        }
    }

    $.fn.tDatePicker = function(options) {
        options = $.extend({}, $.fn.tDatePicker.defaults, options);

        return this.each(function() {
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            if (!$(this).data('tDatePicker'))
                $(this).data('tDatePicker', new $t.datepicker(this, options));
        });
    };

    $.fn.tDatePicker.defaults = {
        effects: $t.fx.slide.defaults(),
        selectedDate: null,
        format: $t.cultureInfo.shortDate,
        focusedDate: new $t.datetime(),
        minDate: new $t.datetime(1899, 11, 31),
        maxDate: new $t.datetime(2100, 0, 1)
    };
})(jQuery);