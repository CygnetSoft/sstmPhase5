
function GetAssignStudentToQP(id) {
    Ajaxhelper.get(QPAssignStudentListUrl,
        {
            id:id
        }, onSuccessQPList, null, null);
}

function onSuccessQPList(data) {
    if (data.result) {
        $('#tblQPStudent').dataTable().fnDestroy();
        $('#tblQPStudent > tbody').empty();
        $("#bodyqp").html('');
        $("#bodyqp").append(data.listdata);
        $("#tblQPStudent").DataTable({
            'aoColumnDefs': [{
                'bSortable': false,
                'aTargets': [-1] /* 1st one, start by the right */
            }]
        });
        //$("#txtmacAddress").val('');
    }
    else
        toastr.error(data.message);
}

function SaveQP() {
    if ($("#LiCourse").val() == "") {
        toastr.error("Li Course required");
        $("#LiCourse").focus();
        return;
    }
    if ($("#LiBatch").val() == "") {
        toastr.error("Li Batch required");
        $("#LiBatch").focus();
        return;
    }
    Ajaxhelper.get(SaveQPUrl, {
        QP_id: $("#QPId").val(),
        course_id: $("#LiCourse").val(),
        batch_id: $("#LiBatch").val(),
        course_name: $("#LiCourse option:selected").text(),
        batch_name: $("#LiBatch").val(),
    }, onSuccessSaveQP, null, null);
}

function onSuccessSaveQP(data) {
    if (data.result) {
        toastr.success(data.message);
        GetAssignStudentToQP(qpid);
    }
    else
        toastr.error(data.message);
}

function Delete(id) {
    var confirmMessageBox = confirm('Are you sure you wish to delete ?');
    if (confirmMessageBox) {
        Ajaxhelper.get(DeleteQPUrl, {
            id: id,
        }, onSuccessDeleteQP, null, null);
    }
}

function onSuccessDeleteQP(data) {
    if (data.result) {
        toastr.success(data.message);
        GetAssignStudentToQP(qpid);
    }
    else
        toastr.error(data.message);
}