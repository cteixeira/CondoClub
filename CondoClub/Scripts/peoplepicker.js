//Plugin para apresentar o autocomplete de destinatários das mensagens
(function ($) {
    // Additional public (exposed) methods
    var methods = {
        init: function (options) {
            return this.each(function () {
                $(this).data("peoplePickerObject", new $.PeoplePickerImplementation(this, options));
                var a = "as";
            });
        },
        getSelectedItems: function () {
            return this.data("peoplePickerObject").getSelectedItems();
        },
        clearSelectedItems: function () {
            return this.data("peoplePickerObject").clearSelectedItems();
        },
        close: function () {
            this.data("peoplePickerObject").close();
        }
    };

    $.fn.PeoplePicker = function (method) {
        // Method calling and initialization logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else {
            return methods.init.apply(this, arguments);
        }
    };

    $.PeoplePickerImplementation = function (target, options) {
        //depende de jquery.ui.autocomplete
        var defaults = {
            inputSelector: '#messageTo',
            receiversContainerSelector: '#receivers',
            showAllPeopleSelector: '.search-receivers',
            dataUrl: '',
            avatarUrl: '',
            useCache: false,
            onSelectionChange: null
        };
        var cache = {};
        var selectedIds = [];
        var selectedObjs = [];
        var settings = $.extend(defaults, options);
        var input = $(target).find(settings.inputSelector);

        function removeSelectedItem(value) {
            for (var i = 0; i < selectedIds.length; i++) {
                if (selectedIds[i] === value) {
                    selectedIds.splice(i, 1);
                    selectedObjs.splice(i, 1);
                    if (settings.onSelectionChange) {
                        settings.onSelectionChange(selectedObjs);
                    }
                    break;
                }
            }
            if (selectedIds.length == 0) {
                $(settings.receiversContainerSelector).hide();
            }
        }

        function processItemsToShow(sugestions) {
            var filteredSugestions = [];
            $.each(sugestions, function (i, val) {
                //remover os destinatios que já estão seleccionados
                if ($.inArray(val.value, selectedIds) == -1) {
                    filteredSugestions.push(val);
                }
            });
            return filteredSugestions;
        }

        function init() {
            input.autocomplete({
                minLength: 0,
                delay: 300,
                open: function () {
                    //add a cutom class to autocomplete
                    $(this).data("uiAutocomplete").menu.element.addClass("message-to-autocomplete");
                },
                focus: function (event, ui) {
                    input.val(ui.item.label);
                    return false;
                },
                source: function (request, response) {
                    if (settings.useCache) {
                        if (cache.term == request.term && cache.content) {
                            response(processItemsToShow(cache.content));
                            return;
                        }
                        if (new RegExp(cache.term).test(request.term) && cache.content) {
                            response(processItemsToShow($.ui.autocomplete.filter(cache.content, request.term)));
                            return;
                        }
                    }
                    $.ajax({
                        url: settings.dataUrl,
                        dataType: "json",
                        data: request,
                        success: function (data) {
                            //create array for response objects  
                            var suggestions = [];
                            //process response  
                            $.each(data, function (i, val) {
                                suggestions.push({
                                    label: val.Designacao,
                                    value: val.ID,
                                    avatar: val.AvatarID ? val.AvatarID : '',
                                    serverObj: val
                                });
                            });

                            if (settings.useCache) {
                                cache.term = request.term;
                                cache.content = suggestions;
                            }

                            response(processItemsToShow(suggestions));
                        }
                    });
                },
                //define select handler  
                select: function (e, ui) {
                    e.preventDefault();
                    var label = ui.item.label;
                    var value = ui.item.value;
                    var serverObj = ui.item.serverObj;
                    if ($.inArray(serverObj, selectedIds) == -1) {

                        selectedIds.push(serverObj.ID);
                        selectedObjs.push(serverObj);

                        var span = $("<span class='receiver'>").text(label),
                        a = $("<a>").addClass("remove").attr({
                            title: "Remove " + label
                        }).click(function () { removeSelectedItem(value); $(this).parent().remove(); return false; }).html("&times;").appendTo(span);

                        span.appendTo(settings.receiversContainerSelector);
                        if (!$(settings.receiversContainerSelector).is(':visible')) {
                            $(settings.receiversContainerSelector).show();
                        }
                        if (settings.onSelectionChange) {
                            settings.onSelectionChange(selectedObjs);
                        }
                    }
                    input.val('').focus();
                    return false;
                },
                change: function () {
                    //prevent 'to' field being updated and correct position  
                    input.val('');
                    return false;
                }
            }).data("ui-autocomplete")._renderItem = function (ul, item) {
                return $("<li class='ui-menu-item'>")
                .append("<a class='ui-corner-all'><img class='user-image' src='" + settings.avatarUrl + "/" + item.avatar + "'/><span class='user-info'>" + item.label + '</span></a>')
                .appendTo(ul);
            };


            if (settings.showAllPeopleSelector) {
                $(settings.showAllPeopleSelector).click(function () {
                    if (!input.data('opened')) {
                        input.autocomplete("search", "");
                    } else {
                        input.autocomplete("close");
                    }
                    return false;
                });

                input.on("autocompleteopen", function (event, ui) {
                    input.data('opened', true);
                });

                input.on("autocompleteclose", function (event, ui) {
                    input.data('opened', false);
                });

            }

        }


        //public methods
        this.getSelectedItems = function () {
            return selectedObjs;
        }

        this.clearSelectedItems = function () {
            selectedObjs = [];
            selectedIds = [];
            $(settings.receiversContainerSelector).hide().find('.receiver').remove();
            if (settings.onSelectionChange) {
                settings.onSelectionChange(selectedObjs);
            }
        }

        this.close = function () {
            input.autocomplete("close");
        }

        init();
    }
})(jQuery);
