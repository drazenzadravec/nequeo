(function($) {

    var $t = $.telerik;

    $.extend($t, {
        tabstrip: function(element, options) {
            this.element = element;

            // attach options to object
            $.extend(this, options);

            // attach event handlers

            $('.t-item:not(.t-state-disabled)', element)
				.live('mouseenter', $t.hover)
				.live('mouseleave', $t.leave)
				.live(options.activateEvent, $t.delegate(this, this._click))

            $t.bind(this, {
                select: this.onSelect,
                error: this.onError,
                load: this.onLoad
            });

            var selectedItems = $('li.t-state-active', element);
            var content = this.getContentElement($('> .t-content', element), selectedItems.parent().children().index(selectedItems));
            if (content && content.length > 0 && content.children().length == 0) {
                this.activateTab($(selectedItems[0]));
            }
            
            $(element).trigger('load');
        }
    });

    $.extend($t.tabstrip.prototype, {

        select: function(li) {
            $(li).each($.proxy(function(index, item) {
                var $item = $(item);
                if ($item.hasClass('t-state-disabled') || $item.hasClass('t-state-active'))
                    return;

                this.activateTab($item);
            }, this));
        },

        enable: function(li) {
            $(li).each(function() {
                $(this)
                    .addClass('t-state-default')
				    .removeClass('t-state-disabled');
            });
        },

        disable: function(li) {
            $(li).each(function() {
                $(this)
                    .removeClass('t-state-default')
                    .removeClass('t-state-active')
				    .addClass('t-state-disabled');
            });
        },

        _click: function(e, element) {

            var $item = $(element);
            var $link = $item.find('.t-link');
            var href = $link.attr('href');

            var content = this.getContentElement($('> .t-content', this.element), $item.parent().children().index($item));

            if ($item.hasClass('t-state-disabled') || $item.hasClass('t-state-active')) {
                e.preventDefault();
                return;
            }

            if ($t.trigger(this.element, 'select', { item: $item[0], contentElement: content })) {
                e.preventDefault();
            }

            if ((href && (href.charAt(href.length - 1) == '#' || href.indexOf('#' + this.element.id + '-') != -1)) ||
                    ($(content).length > 0 && $(content).children().length == 0))
                e.preventDefault();
            else return;

            if (this.activateTab($item))
                e.preventDefault();
        },

        activateTab: function($item) {
            // deactivate previously active tab
            var itemIndex =
				$item.parent().children()
					.removeClass('t-state-active')
					.addClass('t-state-default')
					.index($item);

            // activate tab
            $item.removeClass('t-state-default').addClass('t-state-active');

            // handle content elements
            var contentTabElements = $item.parent().parent().find('> .t-content');
            if (contentTabElements.length > 0) {

                var visibleContentElements = contentTabElements.filter('.t-state-active');

                // find associated content element
                var contentElement = this.getContentElement(contentTabElements, itemIndex);

                var tabstrip = this;
                if (!contentElement) {
                    visibleContentElements.removeClass('t-state-active');

                    $t.fx.rewind(tabstrip.effects, visibleContentElements, {});

                    return false;
                }

                var isAjaxContent = $.trim(contentElement.html()).length == 0;

                var showContentElement = function() {
                    contentElement.addClass('t-state-active');

                    $t.fx.play(tabstrip.effects, contentElement, {});
                };

                visibleContentElements.removeClass('t-state-active').stop(false, true);

                $t.fx.rewind(
                    tabstrip.effects,
			        visibleContentElements, {},
			        function() {
			            if ($item.hasClass('t-state-active')) {
			                if (!isAjaxContent) {
			                    showContentElement();
			                } else if (isAjaxContent) {
			                    tabstrip.ajaxRequest($item, contentElement, function() {
			                        if ($item.hasClass('t-state-active')) {
			                            showContentElement();
			                        }
			                    });
			                }
			            }
			        });

                return true;
            }
            return false;
        },

        getContentElement: function(contentTabElements, itemIndex) {
            var idTest = new RegExp('-' + (itemIndex + 1) + '$');

            for (var i = 0, len = contentTabElements.length; i < len; i++) {
                if (idTest.test($(contentTabElements[i]).attr('id'))) {
                    return $(contentTabElements[i]);
                }
            }
        },

        ajaxRequest: function($element, contentElement, complete) {
            var me = this;

            var statusIcon = null;
            var loadingIconTimeout = setTimeout(function() {
                statusIcon = $('<span class="t-icon t-loading"></span>').prependTo($element.find('.t-link'))
            }, 100);

            var data = {};
            $.ajax({
                type: 'GET',
                url: $element.find('.t-link').attr('href'),
                dataType: 'html',
                data: data,

                error: $.proxy(function(xhr, status) {
                    if ($t.ajaxError(this.element, 'error', xhr, status))
                        return;
                }, this),

                complete: function() {
                    clearTimeout(loadingIconTimeout);
                    if (statusIcon !== null)
                        statusIcon.remove();
                },

                success: $.proxy(function(data, textStatus) {
                    contentElement.html(data);

                    var $link = $element.find('.t-link');
                    $link.data('ContentUrl', $link.attr('href'))
                         .attr('href', '#');

                    if (complete)
                        complete.call(me, contentElement);
                }, this)
            });
        }
    });

    // Plugin declaration
    $.fn.tTabStrip = function(options) {
        options = $.extend({}, $.fn.tTabStrip.defaults, options);

        return this.each(function() {
            // support for Metadata plugin
            options = $.meta ? $.extend({}, options, $(this).data()) : options;

            //Initialize only if needed
            if (!$(this).data('tTabStrip'))
                $(this).data('tTabStrip', new $t.tabstrip(this, options));
        });
    }

    // default options
    $.fn.tTabStrip.defaults = {
        activateEvent: 'click',
        effects: $t.fx.toggle.defaults()
    };
})(jQuery);