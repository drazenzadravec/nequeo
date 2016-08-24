(function($) {
    var $t = $.telerik;

    $.extend($t, {
        menu: function(element, options) {
            this.element = element;
            this.nextItemZIndex = 100;

            $.extend(this, options);

            $('.t-item:not(.t-state-disabled)', element)
                .live('mouseenter', $t.delegate(this, this.mouseenter), true)
				.live('mouseleave', $t.delegate(this, this.mouseleave), true)
				.live('click', $t.delegate(this, this.click));

            $('.t-item:not(.t-state-disabled) > .t-link', element)
				.live('mouseenter', $t.hover)
				.live('mouseleave', $t.leave);

            $(document).click($t.delegate(this, this.documentClick));
            
            $t.bind(this, {
                select: this.onSelect,
                open: this.onOpen,
                close: this.onClose,
                load: this.onLoad
            });
            
            $t.trigger(element, 'load');
        }
    });

    var getEffectOptions = function(item) {
        var parent = item.parent();
        return {
            direction: parent.hasClass('t-menu') ? parent.hasClass('t-menu-vertical') ? 'right' : 'bottom' : 'right'
        };
    };

    $t.menu.prototype = {
        
        toggle: function(li, enable) {
            $(li).each(function() {
                $(this)
                    .toggleClass('t-state-default', enable)
				    .toggleClass('t-state-disabled', !enable);
            });
        },

        enable: function(li) {
            this.toggle(li, true);
        },

        disable: function(li) {
            this.toggle(li, false);
        },

        open: function($li) {
            var menu = this;

            $($li).each($.proxy(function(index, item) {
                var $item = $(item);

                clearTimeout($item.data('timer'));

                $item.data('timer', setTimeout($.proxy(function() {
                    var ul = $item.find('.t-group:first');
                    if (ul.length == 0) return;

                    $item.css('z-index', menu.nextItemZIndex++);

                    $t.fx.play(this.effects, ul, getEffectOptions($item));
                }, this)));
            }, this));
        },

        close: function($li) {
            var menu = this;

            $($li).each($.proxy(function(index, item) {

                var $item = $(item);

                clearTimeout($item.data('timer'));

                $item.data('timer', setTimeout($.proxy(function() {
                    var ul = $item.find('.t-group:first');
                    if (ul.length == 0) return;

                    $t.fx.rewind(this.effects, ul, getEffectOptions($item), function() {
                        $item.css('zIndex', '');
                        if ($(menu.element).find('.t-group:visible').length == 0)
                            menu.nextItemZIndex = 100;
                    });

                    ul.find('.t-group').stop(false, true);
                }, this)));
            }, this));
        },

        mouseenter: function(e, element) {
            if (!this.openOnClick || this.clicked) {
                this.triggerEvent('open', $(element));

                this.open($(element));
                
                $(element.parentNode).trigger(e);
            }

            if (this.openOnClick && this.clicked) {

                this.triggerEvent('close', $(element));

                $(element).siblings().each($.proxy(function(i, sibling) {
                    this.close($(sibling));
                }, this));
            }
        },

        mouseleave: function(e, element) {
            if (!this.openOnClick) {

                this.triggerEvent('close', $(element));

                this.close($(element));

                $(element.parentNode).trigger(e);
            }
        },

        click: function(e, element) {
            var $li = $(element);

            $t.trigger(this.element, 'select', { item: $li[0] });

            if (!$li.parent().hasClass('t-menu') || !this.openOnClick)
                return;

            e.preventDefault();

            this.clicked = true;

            this.triggerEvent('open', $li);

            this.open($li);
        },

        documentClick: function(e, element) {
            if ($.contains(this.element, e.target))
                return;

            if (this.clicked) {
                this.clicked = false;
                $(this.element).children('.t-item').each($.proxy(function(i, item) {
                    this.close($(item));
                }, this));
            }
        },

        hasChildren: function($li) {
            return $li.find('.t-group:first').length;
        },

        triggerEvent: function(eventName, $li) {
            if (this.hasChildren($li))
                $t.trigger(this.element, eventName, { item: $li[0] });
        }
    }

    $.fn.tMenu = function(options) {
        options = $.extend({}, $.fn.tMenu.defaults, options);

        return this.each(function() {
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            if (!$(this).data('tMenu'))
                $(this).data('tMenu', new $t.menu(this, options));
        });
    };

    // default options
    $.fn.tMenu.defaults = {
        orientation: 'horizontal',
        effects: $t.fx.slide.defaults(),
        openOnClick: false
    };
})(jQuery);