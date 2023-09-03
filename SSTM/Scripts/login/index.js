$.validator.unobtrusive.parse("#frmLogin");
$('#frmLogin').on('submit', function (e) {
    $(".loadingDiv").show();
    e.preventDefault();
    if ($("#frmLogin").valid()) {
        $(this).prop('disabled', true);
        Ajaxhelper.post(indexUrl, $("#frmLogin").serialize(), onSuccess, onError, null);

        function onSuccess(data) {
            if (data.result)
                window.location = data.URL;
            else if (data.code == "AccountInactive" || data.code == "InValidCredentials")
                toastr.info(data.message);
            else if (data.code="NotAuthorized")
                window.location = data.URL;
            else
                toastr.error(data.message);

            $('#frmLogin').find('button[type="submit"]').prop('disabled', false);
            $(".loadingDiv").hide();

        };

        function onError() {
            toastr.error('Something went wrong! Please refresh the page and try again or contact our site administrator.');
            $(".loadingDiv").hide();
        };
        $(".loadingDiv").hide();
    }
    $(".loadingDiv").hide();
});