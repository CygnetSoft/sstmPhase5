$.validator.unobtrusive.parse("#frmLogin");
$('#frmLogin').on('submit', function (e) {
    e.preventDefault();

    $('#frmLogin').find('button[type="submit"]').prop('disabled', true);

    if ($("#frmLogin").valid()) {
        Ajaxhelper.post(indexUrl, $("#frmLogin").serialize(), onSuccess, onError, null);

        function onSuccess(data) {
            if (data.result)
                window.location = data.URL;
            else if (data.code == "NotAdministrator")
                toastr.error(data.message);
            else if (data.code == "AccountInactive" || data.code == "InValidCredentials")
                toastr.info(data.message);
            else
                toastr.error(data.message);

            $('#frmLogin').find('button[type="submit"]').prop('disabled', false);
        };

        function onError() {
            toastr.error('Something went wrong! Please refresh the page and try again or contact our site administrator.');
        };
    }
});