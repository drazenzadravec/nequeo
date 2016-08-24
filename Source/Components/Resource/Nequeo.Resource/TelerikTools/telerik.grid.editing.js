(function($) {

    $.validator.addMethod("regex", function(value, element, params) {
        if (this.optional(element)) {
            return true;
        }

        var match = new RegExp(params).exec(value);
        return (match && (match.index == 0) && (match[0].length == value.length));
    });

    function __MVC_ApplyValidator_Range(object, min, max) {
        object["range"] = [min, max];
    }

    function __MVC_ApplyValidator_RegularExpression(object, pattern) {
        object["regex"] = pattern;
    }

    function __MVC_ApplyValidator_Required(object) {
        object["required"] = true;
    }

    function __MVC_ApplyValidator_StringLength(object, maxLength) {
        object["maxlength"] = maxLength;
    }

    function __MVC_ApplyValidator_Unknown(object, validationType, validationParameters) {
        object[validationType] = validationParameters;
    }

    function __MVC_CreateFieldToValidationMessageMapping(validationFields) {
        var mapping = {};

        for (var i = 0; i < validationFields.length; i++) {
            var thisField = validationFields[i];
            mapping[thisField.FieldName] = "#" + thisField.ValidationMessageId;
        }

        return mapping;
    }

    function __MVC_CreateErrorMessagesObject(validationFields) {
        var messagesObj = {};

        for (var i = 0; i < validationFields.length; i++) {
            var thisField = validationFields[i];
            var thisFieldMessages = {};
            messagesObj[thisField.FieldName] = thisFieldMessages;
            var validationRules = thisField.ValidationRules;

            for (var j = 0; j < validationRules.length; j++) {
                var thisRule = validationRules[j];
                if (thisRule.ErrorMessage) {
                    var jQueryValidationType = thisRule.ValidationType;
                    switch (thisRule.ValidationType) {
                        case "regularExpression":
                            jQueryValidationType = "regex";
                            break;

                        case "stringLength":
                            jQueryValidationType = "maxlength";
                            break;
                    }

                    thisFieldMessages[jQueryValidationType] = thisRule.ErrorMessage;
                }
            }
        }

        return messagesObj;
    }

    function __MVC_CreateRulesForField(validationField) {
        var validationRules = validationField.ValidationRules;

        // hook each rule into jquery
        var rulesObj = {};
        for (var i = 0; i < validationRules.length; i++) {
            var thisRule = validationRules[i];
            switch (thisRule.ValidationType) {
                case "range":
                    __MVC_ApplyValidator_Range(rulesObj,
                        thisRule.ValidationParameters["minimum"], thisRule.ValidationParameters["maximum"]);
                    break;

                case "regularExpression":
                    __MVC_ApplyValidator_RegularExpression(rulesObj,
                        thisRule.ValidationParameters["pattern"]);
                    break;

                case "required":
                    __MVC_ApplyValidator_Required(rulesObj);
                    break;

                case "stringLength":
                    __MVC_ApplyValidator_StringLength(rulesObj,
                        thisRule.ValidationParameters["maximumLength"]);
                    break;

                default:
                    __MVC_ApplyValidator_Unknown(rulesObj,
                        thisRule.ValidationType, thisRule.ValidationParameters);
                    break;
            }
        }

        return rulesObj;
    }

    function __MVC_CreateValidationOptions(validationFields) {
        var rulesObj = {};
        for (var i = 0; i < validationFields.length; i++) {
            var validationField = validationFields[i];
            var fieldName = validationField.FieldName;
            rulesObj[fieldName] = __MVC_CreateRulesForField(validationField);
        }

        return rulesObj;
    }

    function __MVC_EnableClientValidation(validationContext) {
        var theForm = $("#" + validationContext.FormId);

        var fields = validationContext.Fields;
        var rulesObj = __MVC_CreateValidationOptions(fields);
        var fieldToMessageMappings = __MVC_CreateFieldToValidationMessageMapping(fields);
        var errorMessagesObj = __MVC_CreateErrorMessagesObject(fields);

        var options = {
            errorClass: "input-validation-error",
            errorElement: "span",
            errorPlacement: function(error, element) {
                var messageSpan = fieldToMessageMappings[element.attr("name")];
                $(messageSpan).empty()
                                .removeClass("field-validation-valid")
                                .addClass("field-validation-error")

                error.removeClass("input-validation-error")
                     .attr("_for_validation_message", messageSpan)
                     .appendTo(messageSpan);
            },
            messages: errorMessagesObj,
            rules: rulesObj,
            success: function(label) {
                var messageSpan = $(label.attr("_for_validation_message"));
                $(messageSpan).empty()
                              .addClass("field-validation-valid")
                              .removeClass("field-validation-error");
            }
        };
        theForm.validate(options);
    }

    var $t = $.telerik;

    $t.editing = {};

    $t.editing.initialize = function(grid) {
        $.extend(grid, this.implementation);

        if (grid.isAjax()) {

            $('.t-grid-edit', grid.element).live('click', function(e) {
                e.preventDefault();
                grid.editRow($(this).closest('tr'));
            });

            $('.t-grid-cancel', grid.element).live('click', function(e) {
                e.preventDefault();
                grid.cancel();
            });

            $('.t-grid-delete', grid.element).live('click', function(e) {
                e.preventDefault();
                grid.deleteRow($(this).closest('tr'));
            });

            $('.t-grid-update', grid.element).live('click', function(e) {
                e.preventDefault();
                grid.save(this, grid.updateRow);
            });

            $('.t-grid-add', grid.element).live('click', function(e) {
                e.preventDefault();
                grid.addRow();
            });

            $('.t-edit-form .t-grid-insert', grid.element).live('click', function(e) {
                e.preventDefault();
                grid.save(this, grid.insertRow);
            });
        } else {
            $('.t-grid-delete', grid.element).live('click', function(e) {
                if (grid.editing.confirmDelete !== false && !confirm(grid.localization.deleteConfirmation))
                    e.preventDefault();
            });

            grid.validation();
        }

        $(':input', grid.element).live('keydown', function(e) {
            var keyMap = { 13: '.t-grid-update, .t-grid-insert', 27: '.t-grid-cancel' };
            $(this).closest('tr').find(keyMap[e.keyCode]).click();
        });
    }

    $t.editing.implementation = {
        insertRow: function($tr) {
            this.sendValues(this.extractValues($tr), 'insertUrl');
        },

        updateRow: function($tr) {
            this.sendValues(this.extractValues($tr, true), 'updateUrl');
        },

        deleteRow: function($tr) {
            if (this.editing.confirmDelete === false || confirm(this.localization.deleteConfirmation))
                this.sendValues(this.extractValues($tr, true), 'deleteUrl');
        },

        editRow: function($tr) {
            this.cancel();

            var html = new $t.stringBuilder();
            this.form(html, [{ name: 'update' }, { name: 'cancel'}]);

            var dataItem = this.dataItem($tr);
            var $td = $(html.string());
            var cells = $td.find('tr:first td:not(.t-groupcell)');

            $.each(this.columns, function(i) {
                if (this.editor)
                    cells.eq(i).find(':input[name="' + this.member + '"]')
                         .val(this.edit(dataItem) + "");
                if (this.readonly)
                    cells.eq(i).html(this.display(dataItem));
            });

            $tr.html($td);

            this.validation();
        },

        dataItem: function($tr) {
            return this.data[this.$tbody.find('tr:not(t-grouping-row)').index($tr)];
        },

        addRow: function() {
            this.cancel();

            var html = new $t.stringBuilder();
            html.cat('<tr class="t-grid-new-row">');
            this.form(html, [{ name: 'insert' }, { name: 'cancel'}]);
            html.cat('</tr>');
            $(html.string()).prependTo(this.$tbody);
            this.validation();
        },

        extractValues: function($tr, extractKeys) {
            var values = {};
            $tr.find(':input').each(function() {
                values[this.name] = $(this).val();
            });

            if (extractKeys) {
                var dataItem = this.dataItem($tr);

                for (var dataKey in this.dataKeys)
                    values[this.ws ? dataKey : this.dataKeys[dataKey]] = this.valueFor({ member: dataKey })(dataItem);
            }
            return values;
        },

        cancelRow: function($tr) {
            if (!$tr.length)
                return;

            if ($tr.is('.t-grid-new-row')) {
                $tr.remove();
                return;
            }

            var dataItem = this.dataItem($tr);
            var html = new $t.stringBuilder();

            html.rep('<td class="t-groupcell" />', this.groups.length);

            $.each(this.columns, $.proxy(function(i, c) {
                html.cat('<td')
                  .cat(c.attr)
                  .catIf(' class="t-last"', i == this.columns.length - 1)
                  .cat('>');

                if (c.editor)
                    html.cat(c.display(dataItem));

                this.appendCommandHtml(c.commands, html);

                html.cat('</td>');

            }, this));

            $tr.html(html.string());
        },

        form: function(html, commands) {
            var colgroup = this.$tbody.siblings('colgroup');

            html.cat('<td class="t-edit-container" colspan="')
                .cat(this.columns.length + this.groups.length)
                .cat('"><form class="t-edit-form" action="#" method="post" id="')
                .cat(this.formId())
                .cat('"><table cellspacing="0">');

            var columnPrototype = $.browser.mozilla ? this.$tbody.siblings('colgroup').children()
                                                    : this.$tbody.children(':not(.t-grouping-row)').eq(0).find('> td');

            columnPrototype.each(function(i) {
                html.cat('<col style="width:')
                    .catIf($(this).width(), $.browser.mozilla)
                    .catIf(this.offsetWidth - (document.documentMode < 8), !$.browser.mozilla)
                    .cat('px" />');
            });

            html.cat('<tr>')
                .rep('<td class="t-groupcell" />', this.groups.length);

            $.each(this.columns, $.proxy(function(i, c) {
                html.cat('<td')
                    .cat(c.attr)
                    .catIf(' class="t-last"', i == this.columns.length - 1)
                    .cat('>')
                    .catIf(c.editor, c.editor);

                if (c.commands)
                    this.appendCommandHtml(commands, html);

                html.cat('</td>');
            }, this));

            html.cat('</tr></table></form></td>');
        },

        save: function(element, callback) {
            var $form = $(element).closest('form');
            if ($form.validate().form())
                callback.call(this, $form.closest('tr'));
        },

        cancel: function() {
            this.cancelRow($('#' + this.formId()).closest('tr'));
        },

        sendValues: function(values, url) {
            if (this.ws)
                for (var key in values) {
                var column = this.columnFromMember(key);
                if (column && column.type == 'Date') {
                    var date = new Date(Date.parse(values[key]));
                    values[key] = '\\/Date(' + date.getTime() + ')\\/';
                }
            }

            $.ajax(this.ajaxOptions({
                data: this.ws ? { value: values} : values,
                url: this.url(url)
            }));
        },

        formId: function() {
            return $(this.element).attr('id') + 'form';
        },

        validation: function() {
            if (window.mvcClientValidationMetadata) {
                var formId = this.formId();
                var metadata = $.grep(window.mvcClientValidationMetadata, function(item) {
                    return item.FormId == formId;
                })[0];

                if (metadata && __MVC_EnableClientValidation)
                    __MVC_EnableClientValidation(metadata);
            }
        }
    }
})(jQuery);