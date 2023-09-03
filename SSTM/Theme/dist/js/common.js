//function toasterMessage(cssClass, title, autohide, delay, body) {
//    $(document).Toasts('create', { class: cssClass, title: title, autohide: autohide, delay: delay, body: body });
//}

// Restricts input for the set of matched elements to the given inputFilter function.
(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    };
}(jQuery));

$(function () {
    //var fnTimeOut = function () {
    //    jQuery.timeoutDialog.setupDialogTimer({
    //        timeout: 3000,
    //        countdown: 120,
    //        logout_redirect_url: signOutUrl,
    //        keep_alive_url: '/'
    //    });
    //};
    //fnTimeOut();

    //$('body').toggleClass('loaded');

    $('.dialog-background').hide();
});

function isNumberKey(evt, obj) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    var value = obj.value;
    var dotcontains = value.indexOf(".") != -1;
    if (dotcontains)
        if (charCode == 46) return false;
    if (charCode == 46) return true;
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}