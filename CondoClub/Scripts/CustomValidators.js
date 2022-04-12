/*Validator is date after*/
$.validator.unobtrusive.adapters.add(
'isdateafter', ['propertytested', 'allowequaldates'], function (options) {
    options.rules['isdateafter'] = options.params;
    options.messages['isdateafter'] = options.message;
});
$.validator.addMethod("isdateafter", function (value, element, params) {
    var parts = element.name.split(".");
    var prefix = "";
    if (parts.length > 1)
        prefix = parts[0] + ".";
    var startdatevalue = $(element).parents('form:first').find(('input[name="' + prefix + params.propertytested + '"]'));
    if (!value || !startdatevalue)
        return true;

    var valueDate = $(element).datepicker("getDate");
    var startdatevalueDate = $(element).parents('form:first').find(('input[name="' + prefix + params.propertytested + '"]')).datepicker("getDate");
    
    var allowequaldates = params.allowequaldates.toLowerCase() === "true";
    return (allowequaldates) ? startdatevalueDate <= valueDate : startdatevalueDate < valueDate;
});

/*Validator is date after today*/
$.validator.unobtrusive.adapters.add(
'isdateaftertoday', ['allowtoday'], function (options) {
    options.rules['isdateaftertoday'] = options.params;
    options.messages['isdateaftertoday'] = options.message;
});
$.validator.addMethod("isdateaftertoday", function (value, element, params) {
    var parts = element.name.split(".");
    var prefix = "";
    if (parts.length > 1)
        prefix = parts[0] + ".";

    if (!value)
        return true;

    var dateToday = new Date();
    dateToday.setHours(0, 0, 0, 0);

    var valueDate = $(element).datepicker("getDate");

    var allowtoday = params.allowtoday.toLowerCase() === "true";
    return (allowtoday) ? dateToday <= valueDate : dateToday < valueDate;
});
/*Validator range double*/
$.validator.unobtrusive.adapters.add(
    'rangedouble',
    ['min', 'max'],
    function (options) {
        options.rules['rangedouble'] = options.params;
        options.messages['rangedouble'] = options.message;
    });

$.validator.addMethod("rangedouble", function (value, element, params) {
    if (value != "") {
        if (value.indexOf(",") >= 0) {
            value = value.replace(",", ".");
        }
        if ($(element).val().toString().indexOf(".") >= 0) {
            $(element).val($(element).val().toString().replace(".", ","));
        }
        return (!isNaN(value) && (parseFloat(params.min) <= parseFloat(value) && parseFloat(value) <= parseFloat(params.max)));
    }
    return true;
});

