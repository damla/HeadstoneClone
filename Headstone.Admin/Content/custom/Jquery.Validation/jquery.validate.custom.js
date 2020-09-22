function validationReady() {
    setValidatorDefaultSettings();
    setValidatorMessages();

    $('[data-validate=true]').each(function (index, elem) {
        $(elem).validate();
    });
}

//-> Changing validator basic behaviours

function setValidatorDefaultSettings() {
    jQuery.validator.setDefaults({
        highlight: function (element, errorClass, validClass) {
            $(element).addClass(errorClass).removeClass(validClass)
                .parent().addClass("parent" + errorClass).removeClass("parent" + validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass(errorClass).addClass(validClass)
                .parent().removeClass("parent" + errorClass).addClass("parent" + validClass);
        },
        focusInvalid: false,
        invalidHandler: function (form, validator) {
            // Scroll to first invalid element if any exists.
            if (!validator.numberOfInvalids())
                return;

            // if form is in modal then return
            var isModal = $(validator.currentForm).parents('.modal').length === 1;

            if (!isModal) {
                var timerVal = 500;

                $('html, body').animate({
                    scrollTop: parseInt($(validator.errorList[0].element).offset().top - 200)
                }, timerVal);

                setTimeout(function () {
                    $(validator.errorList[0].element).focus();
                }, timerVal);
            }
            else {
                $(validator.errorList[0].element).focus();
            }
        }
    });

    $.validator.addMethod(
        'depends',
        function (val, elem) {
            var ret = true;
            var senderElemProps = elementInfo($(elem).attr("id"));

            //-> Check if 'depends' is based on a container specific validation control.
            //-> If container exists, then validate elements included in it.
            var dependentContainerId = $(elem).attr("data-dependent-container");
            var dependentIsMultiple = $(elem).attr("multiple");

            if (dependentContainerId) {
                var isValidating = $('#' + dependentContainerId).attr('data-validating');
                if (String(isValidating) == "true") {
                    if (senderElemProps.info == "checkbox" || senderElemProps.info == "radio") {
                        ret = $("[name=" + senderElemProps.name + "]").is(":checked");
                    }
                    else {
                        ret = ($(elem).val().length == 0 ? false : true);
                    }
                }
            }
            else if (dependentIsMultiple) {
                ret = elem.options.length > 0 ? true : false;
            }
            else {
                var dependentElementInfoArr = $(elem).attr("data-dependent-src").split('#');
                var dependentElementId = dependentElementInfoArr[0];
                var dependentElementVal = dependentElementInfoArr[1];
                var dependentElemProps = elementInfo(dependentElementId);

                if (dependentElemProps.info == "checkbox" || dependentElemProps.info == "radio") {
                    if (String(dependentElemProps.isChecked) == dependentElementVal) {
                        if (senderElemProps.info == "checkbox" || senderElemProps.info == "radio") {
                            ret = $("[name=" + senderElemProps.name + "]").is(":checked");
                        }
                        else {
                            ret = ($(elem).val().length == 0 ? false : true);
                        }
                    }
                }
                else {
                    if (dependentElemProps.val == dependentElementVal) {
                        if (senderElemProps.info == "checkbox" || senderElemProps.info == "radio") {
                            ret = $("[name=" + senderElemProps.name + "]").is(":checked");
                        }
                        else {
                            ret = ($(elem).val().length == 0 ? false : true);
                        }
                    }
                }
            }

            return ret;
        }, "Zorunlu alan");

    $.validator.addMethod(
        "time24",
        function (value, element) {
            if (value != '') {
                if (!/^\d{2}:\d{2}$/.test(value)) return false;
                var parts = value.split(':');
                if (parts[0] > 23 || parts[1] > 59) return false;
            }
            return true;
        }, "Hatalı çalışma saati");

    $.validator.addMethod(
        'turkishDate',
        function (currVal, element) {
            if (currVal == '')
                return false;

            //Declare Regex  
            var rxDatePattern = /^(\d{1,2})(\/|-|.)(\d{1,2})(\/|-|.)(\d{4})$/;
            var dtArray = currVal.match(rxDatePattern); // is format OK?

            if (dtArray == null)
                return false;

            //Checks for dd/mm/yyyy format.
            var dtDay = dtArray[1];
            var dtMonth = dtArray[3];
            var dtYear = dtArray[5];
            var dateRange = $(element).attr('data-year-range').split(':');

            if (parseInt(dtYear) > parseInt(dateRange[1]) || parseInt(dtYear) < parseInt(dateRange[0])) {
                return false;
            }

            if (dtMonth < 1 || dtMonth > 12)
                return false;
            else if (dtDay < 1 || dtDay > 31)
                return false;
            else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
                return false;
            else if (dtMonth == 2) {
                var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
                if (dtDay > 29 || (dtDay == 29 && !isleap))
                    return false;
            }

            return true;
        }
    );
}

/// End of changing validator basic behaviours


///-> Overwriting validator default messages

function setValidatorMessages() {
    jQuery.extend(jQuery.validator.messages, {
        required: "Zorunlu alan",
        remote: "Please fix this field.",
        email: "Hatalı e-posta adresi",
        url: "Please enter a valid URL.",
        date: "Please enter a valid date.",
        dateISO: "Please enter a valid date (ISO).",
        number: "Please enter a valid number.",
        digits: "Please enter only digits.",
        creditcard: "Please enter a valid credit card number.",
        equalTo: "Please enter the same value again.",
        accept: "Please enter a value with a valid extension.",
        maxlength: jQuery.validator.format("Please enter no more than {0} characters."),
        minlength: jQuery.validator.format("Eksik bilgi"),
        rangelength: jQuery.validator.format("Please enter a value between {0} and {1} characters long."),
        range: jQuery.validator.format("Please enter a value between {0} and {1}."),
        max: jQuery.validator.format("Please enter a value less than or equal to {0}."),
        min: jQuery.validator.format("Please enter a value greater than or equal to {0}."),
        turkishDate: "Geçerli bir tarih giriniz: gg.aa.yyyy"
    });
}

/// End overwriting validator default messages


//-> Cleaning validation elements and classes

function cleanValidation(containerId) {
    $('#' + containerId).find('.parenterror').each(function (index, elem) {
        $(elem).removeClass('parenterror');
        $(elem).find('label.error').remove();
        $(elem).find('.error').removeClass('error');
    });
}

/// End of cleaning validation elements and classes