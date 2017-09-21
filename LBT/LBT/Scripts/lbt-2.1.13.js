$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
};

$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
};

// LBT general namespace
var LBT = LBT || {};

LBT.AutomaticLogout = function (urlAccountLogin, automaticLogoutInterval) {
    if (location.href.indexOf(urlAccountLogin) == -1) {
        var totalSecondsToLogout = automaticLogoutInterval * 60;
        LBT.CountDownTimer('#alertElement', totalSecondsToLogout, 120, function () { $('#logoutForm').submit(); });
    }
};

LBT.CountDownTimer = function (alertElem, totalSecondsToLogout, remainingAlertTimeSeconds, functionAfterTimeIsUp) {

    var seconds = remainingAlertTimeSeconds;
    var milisecondsToStartCounter = (totalSecondsToLogout - remainingAlertTimeSeconds) * 1000;
    var startCountDownTimeoutHandler;
    var countDownTimeoutHandler;

    var init = function () {
        $("body").on("mousemove", resetTimer);
        startCountDownTimeoutHandler = setTimeout(startCountDown, milisecondsToStartCounter);
    };

    var startCountDown = function () {
        seconds = remainingAlertTimeSeconds;
        countDown();
    };

    var countDown = function () {
        $(alertElem).show();
        var template = LBT.LocalizedResources.GetString("UserWillBeLogoutMessage");
        var text = LBT.StringFormat(template, [seconds]);
        $(alertElem).html(text);

        if (seconds <= 0) {
            functionAfterTimeIsUp();
            seconds += 1;
            return;
        } else
            seconds -= 1;

        countDownTimeoutHandler = setTimeout(countDown, 1000);
    };

    var resetTimer = function () {
        $(alertElem).hide();
        $(alertElem).html('');

        if (startCountDownTimeoutHandler != undefined)
            clearTimeout(startCountDownTimeoutHandler);

        if (countDownTimeoutHandler != undefined)
            clearTimeout(countDownTimeoutHandler);

        startCountDownTimeoutHandler = setTimeout(startCountDown, milisecondsToStartCounter);
        countDownTimeoutHandler = undefined;
    };

    init();
};

LBT.StringFormat = function (template, argv) {
    var text = template;
    for (var i = 0; i < argv.length; i++) {
        text = text.replace("{" + i + "}", argv[i]);
    }

    return text;
};

LBT.SortOptions = function (a, b) {
    var value1 = $(a).text();
    var value2 = $(b).text();
    return value1.toLowerCase().localeCompare(value2.toLowerCase());
};

LBT.ApplyDatePicker = function () {
    $('input.datepicker').datepicker(
            {
                duration: '',
                changeMonth: false,
                changeYear: false,
                yearRange: '2010:2020',
                showTime: false,
                time24h: true
            });
};

LBT.ValidateGoogleCredentialsJson = function (testGoogleCredentialsJsonCookieKey, testGoogleCalendarIdCookieKey, url) {
    $('#popup').bPopup();

    $.cookie(testGoogleCredentialsJsonCookieKey, $('#GoogleCredentialsJson').val(), { expires: 1, path: '/' });
    $.cookie(testGoogleCalendarIdCookieKey, $('#GoogleCalendarId').val(), { expires: 1, path: '/' });
    $.ajax({
        type: "GET",
        headers: { 'Access-Control-Allow-Origin': '*' },
        crossDomain: true,
        url: url
    })
        .done(function (result) {
            var win = window.open('', 'Google Credentials JSON Validation', 'height=600, width=650, left=300, top=100, resizable=yes, scrollbars=yes, toolbar=yes, menubar=no, location=no, directories=no, status=yes');
            win.document.open();
            win.document.write(result);
        });
};

LBT.EditTask = function (url) {
    $('#popup').bPopup({
        content: 'iframe', //'ajax', 'iframe' or 'image'
        contentContainer: '.content',
        loadUrl: url,
        onClose: function () { location.reload(); }
    });
};

LBT.StopWorkflow = function (url, onCloseUrl) {
    $('#popup').bPopup({
        content: 'iframe', //'ajax', 'iframe' or 'image'
        contentContainer: '.content',
        loadUrl: url,
        onClose: function () { window.location.href = onCloseUrl; }
    });
    $('#popup iframe').css('min-height', '500px');
};

LBT.SetScrollPosition = function (scrollTopCookie) {
    var messageError = $(".message-error").text();
    if (messageError != '') {
        $.cookie(scrollTopCookie, null, { path: '/' });
        return;
    }

    var scrollTop = $.cookie(scrollTopCookie);
    $.cookie(scrollTopCookie, null, { path: '/' });
    if (scrollTop != undefined && scrollTop != null) {
        $(window).scrollTop(scrollTop);
    }
};

LBT.GetScrollPosition = function (scrollTopCookie) {
    var scrollTop = $(window).scrollTop();
    $.cookie(scrollTopCookie, scrollTop, { expires: new Date(2100, 1, 1, 0, 0, 0), path: '/' });
};

LBT.ApplyScrollPosition = function (scrollTopCookie) {
    LBT.SetScrollPosition(scrollTopCookie);

    $('.UnsignButton').on('click', function () {
        LBT.GetScrollPosition(scrollTopCookie);
        return confirm(LBT.LocalizedResources.GetString("UnsignUserQuestion"));
    });
    $('.SignButton').on('click', function () {
        LBT.GetScrollPosition(scrollTopCookie);
    });
    $('input[type="submit"]').on('click', function () {
        LBT.GetScrollPosition(scrollTopCookie);
    });
    $('.List a').on('click', function () {
        LBT.GetScrollPosition(scrollTopCookie);
    });
};

LBT.ConfirmUnsignUser = function () {
    $('.buttons .delete').on('click', function () {
        return confirm(LBT.LocalizedResources.GetString("UnsignUserQuestion"));
    });
};

LBT.ApplyInfoPopup = function () {
    $(".infoPopup").each(function () {
        $(this).attr("title", $(this).attr("data-bpopup"));
    });

    $(".infoPopup").click(function () {
        var self = $(this);
        var content = $(".content");

        $("#popup").bPopup({
            onOpen: function () {
                content.html(self.data('bpopup') || '');
            },
            onClose: function () {
                content.empty();
            }
        });
    });
};

LBT.MultipleCreateAddRow = function () {
    $("#multipleForm tr:last input").unbind("keyup");
    $("#multipleForm").append($("#hiddenRowTemplate").html());
    $("#multipleForm tr:last input").bind("keyup", function () {
        LBT.MultipleCreateAddRow();
    });
};

LBT.MultipleCreateFillData = function () {
    var json = $("#MultiplePeopleContactsJson").val();
    if (json == "")
        return;

    var multiplePeopleContacts = $.parseJSON(json);
    if (multiplePeopleContacts == null)
        return;

    for (var i = 0; i < multiplePeopleContacts.length; i++) {
        var lastName = multiplePeopleContacts[i].LastName;
        var firstName = multiplePeopleContacts[i].FirstName;
        var city = multiplePeopleContacts[i].City;
        var phoneNumber1 = multiplePeopleContacts[i].PhoneNumber1;
        var email1 = multiplePeopleContacts[i].Email1;
        var skype = multiplePeopleContacts[i].Skype;
        if (lastName == "" && firstName == "" && city == "" && phoneNumber1 == "" && email1 == "" && skype == "")
            continue;

        $("#multipleForm tr:last input[name='LastName']").val(lastName);
        $("#multipleForm tr:last input[name='FirstName']").val(firstName);
        $("#multipleForm tr:last input[name='City']").val(city);
        $("#multipleForm tr:last select[name='PhoneNumberPrefix1Id']").val(multiplePeopleContacts[i].PhoneNumberPrefix1Id);
        $("#multipleForm tr:last input[name='PhoneNumber1']").val(phoneNumber1);
        $("#multipleForm tr:last input[name='Email1']").val(email1);
        $("#multipleForm tr:last input[name='Skype']").val(skype);
        LBT.MultipleCreateAddRow();
    }
};

LBT.MultipleCreateOnSubmit = function () {
    var multiplePeopleContacts = [];
    var i = 0;
    $("#multipleForm tr").each(function () {
        var lastName = $("input[name='LastName']", this).val();
        var firstName = $("input[name='FirstName']", this).val();
        var city = $("input[name='City']", this).val();
        var phoneNumber1 = $("input[name='PhoneNumber1']", this).val();
        var email1 = $("input[name='Email1']", this).val();
        var skype = $("input[name='Skype']", this).val();
        if (lastName == "" && firstName == "" && city == "" && phoneNumber1 == "" && email1 == "" && skype == "")
            return;

        multiplePeopleContacts[i] = {
            LastName: lastName,
            FirstName: firstName,
            City: city,
            PhoneNumberPrefix1Id: $("select[name='PhoneNumberPrefix1Id']", this).val(),
            PhoneNumber1: phoneNumber1,
            Email1: email1,
            Skype: skype
        };
        i++;
    });
    var json = JSON.stringify(multiplePeopleContacts);
    $("#MultiplePeopleContactsJson").val(json);
};

LBT.ApplyCreateSharedContacts = function () {
    $("#ToDownlineUserId").attr('disabled', 'disabled');
    $("#ToAnyUserId").attr('disabled', 'disabled');

    $("#RadioToUserId[value='RadioToUplineUserId']").on('change', function () {
        if ($(this).is(':checked')) {
            $("#ToUplineUserId").removeAttr('disabled', 'disabled');
            $('#ToDownlineUserId').val('');
            $("#ToDownlineUserId").attr('disabled', 'disabled');
            $('#ToAnyUserId').val('');
            $("#ToAnyUserId").attr('disabled', 'disabled');
            $("#ToUserId").val($("#ToUplineUserId").val());
        }
    });
    $("#ToUplineUserId").on('change', function () { $("#ToUserId").val($(this).val()); });

    $("#RadioToUserId[value='RadioToDownlineUserId']").on('change', function () {
        if ($(this).is(':checked')) {
            $('#ToUplineUserId').val('');
            $("#ToUplineUserId").attr('disabled', 'disabled');
            $("#ToDownlineUserId").removeAttr('disabled', 'disabled');
            $('#ToAnyUserId').val('');
            $("#ToAnyUserId").attr('disabled', 'disabled');
            $("#ToUserId").val($("#ToDownlineUserId").val());
        }
    });
    $("#ToDownlineUserId").on('change', function () { $("#ToUserId").val($(this).val()); });

    $("#RadioToUserId[value='RadioToAnyUserId']").on('change', function () {
        if ($(this).is(':checked')) {
            $('#ToUplineUserId').val('');
            $("#ToUplineUserId").attr('disabled', 'disabled');
            $('#ToDownlineUserId').val('');
            $("#ToDownlineUserId").attr('disabled', 'disabled');
            $("#ToAnyUserId").removeAttr('disabled', 'disabled');
            $("#ToUserId").val($("#ToAnyUserId").val());
        }
    });
    $("#ToAnyUserId").on('change', function () { $("#ToUserId").val($(this).val()); });
};

LBT.ApplyVideoUsers = function () {
    var allUsersFunction = function () {
        $("#UserIdsMultiSelectBox").find("input").each(function () {
            $(this).attr('disabled', 'disabled');
            $(this).css('color', 'gray');
        });
        $("#UserIdsMultiSelectBox").find("select").each(function () {
            $(this).attr('disabled', 'disabled');
        });
    };

    var selectedUsersFunction = function () {
        $("#UserIdsMultiSelectBox").find("input").each(function () {
            $(this).removeAttr('disabled');
            $(this).css('color', '');
        });
        $("#UserIdsMultiSelectBox").find("select").each(function () {
            $(this).removeAttr('disabled');
        });
    };

    $("#AllUsers[value='True']").on('change', function () {
        if ($(this).is(':checked')) {
            allUsersFunction();
        }
    });

    $("#AllUsers[value='False']").on('change', function () {
        if ($(this).is(':checked')) {
            selectedUsersFunction();
        }
    });

    if ($("#AllUsers[value='True']").is(':checked')) {
        allUsersFunction();
    } else {
        selectedUsersFunction();
    }
};

LBT.SwitchToIndexTree = function (switchToUserProfileIndexTreeCookieKey, isDefaultView) {
    $.cookie(switchToUserProfileIndexTreeCookieKey, isDefaultView, { expires: new Date(2100, 1, 1, 0, 0, 0), path: '/' });
};

LBT.ApplyTinyMCE = function () {
    tinymce.init({
        selector: "#Note, #Tasks, #Task, #BringYourOwn",
        theme: "modern",
        language: "cs",
        plugins: [
        "advlist autolink autoresize charmap code colorpicker contextmenu directionality emoticons hr insertdatetime nonbreaking paste preview searchreplace table textcolor visualblocks visualchars wordcount"
        ],
        toolbar1: "undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent",
        toolbar2: "forecolor backcolor | table nonbreaking emoticons insertdatetime | preview code visualblocks visualchars",
        insertdatetime_formats: ["%d. %m. %Y", "%H:%M"],
        nonbreaking_force_tab: true
    });
};

LBT.CheckPrivacy = function (termsAndConditionsId, personalDataId, contactsId, question) {
    if (!$(termsAndConditionsId).prop("checked") || !$(personalDataId).prop("checked") || !$(contactsId).prop("checked")) {
        return confirm(question);
    }

    return true;
};

LBT.CheckDeleteAccount = function (confirmDeleteAccountId, question) {
    if ($(confirmDeleteAccountId).prop("checked")) {
        return confirm(question);
    }

    return true;
};

LBT.ApplyPeopleContactEditFieldEvents = function () {
    $(".PeopleContactEditFields input").change(function () {
        $('#confirmPolicies input[type="checkbox"]').prop("checked", "");
    });
};

LBT.ApplyClaAccessChangedCurrency = function () {
    $("#ClaAccessChangedCurrency").change(function () {
        LBT.ClaAccessCurrencyWasChanged();
    });

    LBT.ClaAccessCurrencyWasChanged();
};

LBT.ClaAccessCurrencyWasChanged = function () {
    var currencyType = $("#ClaAccessChangedCurrency").val();
    var className = ".ClaAccessChangedInfo_" + currencyType;
    $(".ClaAccessChangedInfoList").hide();
    $(className).show();
};

LBT.SendLcidRequest = function (lcid) {
    var params = {};
    params.LCID = lcid;
    var uri = LBT.AddParamsToUri(location.href, params);
    location.href = uri;
};

LBT.GetUrlVars = function () {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
};

LBT.AddParamsToUri = function (params) {
    var uri = LBT.AddParamsToUri(location.href, params);
    return uri;
};

LBT.AddParamsToUri = function (uri, params) {
    var url = $.url(uri);
    var queryString = url.param();
    $.extend(queryString, params);

    // Check another '?' symbol in uri
    var indexUri = uri.indexOf("?");
    if (indexUri != -1) {
        uri = uri.substr(0, indexUri);
    }

    uri += "?" + $.param(queryString);
    return uri;
};

LBT.InitUsingCookiesInfo = function (isUsingCookiesClosedCookieKey) {
    var isUsingCookiesClosed = $.cookie(isUsingCookiesClosedCookieKey);
    if (isUsingCookiesClosed)
        return;

    $("#cookie-info").addClass("active");
    $("#cookie-info a.cookieButtonClose").on('click', function() {
        $.cookie(isUsingCookiesClosedCookieKey, true, { expires: new Date(2100, 1, 1, 0, 0, 0), path: '/' });
        $("#cookie-info").removeClass("active");
    });
};