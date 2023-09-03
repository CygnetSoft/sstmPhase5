var body = $('body');

$(function () {
    'use strict';

    body.on('keyup', 'input[type="password"]', goToNextInput);
    body.on('keydown', 'input[type="password"]', onKeyDown);
    body.on('click', 'input[type="password"]', onFocus);

    $("#btnResendOTP").on('click', function (e) {
        e.preventDefault();

        $(this).prop('disabled', true);

        Ajaxhelper.get(resendOTPUrl, { Id: encryptedId }, onSuccessResendOTP, null, null);

        function onSuccessResendOTP(data) {
            if (data.result) {
                timer(60);

                toastr.success('OTP is sent to your registered mobile number. Please check your mobile.');

                $("input[type='password'], #btnVerifyOTP").prop("disabled", false);

                $("#btnVerifyOTP, #timerid").css('display', 'block');

                $("input[type='password']").val('');

                $("#btnResendOTP").css('display', 'none');
            }
            else
                toastr.success(exceptionMessage);

            $('#btnResendOTP').prop('disabled', false);
        }
    });

    $('#btnVerifyOTP').on('click', function (e) {
        e.preventDefault();

        $("input[type='password'], #btnVerifyOTP").prop('disabled', true);

        var otp = "";
        $("input[type='password']").each(function () {
            if ($(this).val() != "")
                otp += $(this).val();
        });

        if (otp.length == 0) {
            toastr.error(invalidOTPMessage);

            $('input[type="password"]').val("");
            $('input[type="password"]').first().focus();
            $('#btnVerifyOTP').prop('disabled', false);

            return false;
        }

        Ajaxhelper.post(verifyOTPUrl, { Id: encryptedId, otp: otp }, onSuccessVerifyOTP, null, null);

        function onSuccessVerifyOTP(data) {
            if (data.result) {
                window.location = data.URL;
            }
            else if (data.code == "InvalidOTP")
                toastr.error(invalidOTPMessage);
            else
                toastr.error(exceptionMessage);

            $('input[type="password"]').val("");
            $('input[type="password"]').first().focus();
            $('input[type="password"], #btnVerifyOTP').prop('disabled', false);
        };
    });

    $('input[type="password"]').first().focus();
});

function goToNextInput(e) {
    var key = e.which,
      t = $(e.target),
      sib = t.next('input');

    if (key != 9 && key != 8 && key != 13 && (key < 48 || key > 57)) {
        e.preventDefault();
        return false;
    }

    if (key === 9)
        return true;

    if (key === 8) {
        //if (t.val() == "")
        //    t.prev('input[type="password"]').focus();

        t.val("");

        return true;
    }

    if (sib.length <= $('input[type="password"]').length) {
        if (t.prev().val() != "")
            t.next().focus();
        else
            t.prev().focus();

        return true;
    }

    //if (!sib || !sib.length)
    //    sib = body.find('input').eq(0);

    if (key === 13)
        $("#btnVerifyOTP").trigger('click');

    //sib.select().focus();
}

function onKeyDown(e) {
    var key = e.which;

    if (key === 9 || key === 8 || key === 13 || (key >= 48 && key <= 57)) {
        return true;
    }

    e.preventDefault();
    return false;
}

function onFocus(e) {
    $(e.target).select();
}

timer(60); //set time otp

function timer(remaining) {
    var m = Math.floor(remaining / 60);
    var s = remaining % 60;

    m = m < 10 ? '0' + m : m;
    s = s < 10 ? '0' + s : s;
    //m + ':' + s;
    document.getElementById('timer').innerHTML = s;
    remaining -= 1;

    if (remaining >= 0 && timerOn) {
        setTimeout(function () { timer(remaining); }, 1000);
        return;
    }

    if (!timerOn) {
        // Do validate stuff here
        return;
    }

    // Do timeout stuff here
    timeoutProcedure();
}

function timeoutProcedure() {
    toastr.error(expiredOTPMessage);

    $('#btnVerifyOTP, #timerid').css('display', 'none');

    $("input[type='password']").val('').prop('disabled', true);

    Ajaxhelper.post(otpExpiredUrl, null, null, null, null);

    $("#btnResendOTP").css('display', 'block');
}
$("#lastdigit").keyup(function () {
        $('#btnVerifyOTP').trigger('click');
        $('input[type="password"]').first().focus();
});