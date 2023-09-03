
function GetMacAddress() {
    Ajaxhelper.get(MacAddressListUrl,null, onSuccessMacAddressList, null, null);
}

function onSuccessMacAddressList(data) {
    if (data.result)
    {
        $('#tblMacAddress').dataTable().fnDestroy();
        $('#tblMacAddress > tbody').empty();
        $("#bodymacAddress").html('');
        $("#bodymacAddress").append(data.listdata);
        $("#tblMacAddress").DataTable({
            'aoColumnDefs': [{
                'bSortable': false,
                'aTargets': [-1] /* 1st one, start by the right */
            }]
        });
        $("#txtmacAddress").val('');
    }
    else
        toastr.error(data.message);
}

function SaveMacAddress() {
    if ($("#txtmacAddress").val() == "")
    {
        toastr.error("Mac Address required");
        $("#txtmacAddress").focus();
        return;
    }
    Ajaxhelper.get(SaveMacAddressUrl, {
        MacAddress: $("#txtmacAddress").val(),
    }, onSuccessSaveMacAddress, null, null);
}

function onSuccessSaveMacAddress(data) {
    if (data.result) {
        toastr.success(data.message);
        GetMacAddress();
    }
    else
        toastr.error(data.message);
}

function DeleteMacAddress(id) {
    var confirmMessageBox = confirm('Are you sure you wish to delete this MAC Address ?');
    if (confirmMessageBox) {
        Ajaxhelper.get(DeleteMacAddressUrl, {
            id: id,
        }, onSuccessDeleteMacAddress, null, null);
    }
}

function onSuccessDeleteMacAddress(data) {
    if (data.result) {
        toastr.success(data.message);
        GetMacAddress();
    }
    else
        toastr.error(data.message);
}