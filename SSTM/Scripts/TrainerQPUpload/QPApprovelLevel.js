window.onload = function () {
    InittblCourses();
};

function Get_approval_list() {
    Ajaxhelper.get(Url_Get_approval_list, null, onSuccesApproval_list, null, null);
}

function onSuccesApproval_list(data) {

    if (data.result) {
        $('#tblLevelApproval').dataTable().fnDestroy();
        $('#tblLevelApproval > tbody').empty();
        $("#bodyqp").html('');
        $("#bodyqp").append(data.content);
        $("#tblLevelApproval").DataTable({
            'aoColumnDefs': [{
                'bSortable': false,
                'aTargets': [-1] /* 1st one, start by the right */
            }]
        });
        InittblCourses();
    }
    else
        toastr.error(data.message);
}
function InittblCourses() {
    

    $('#tblLevelApproval > tbody > tr').on('click', '.viewdoc', function (e) {
        var courseName = $(this).closest('tr').find('td:eq(2)').text();
        ViewCourseDoc(courseName);
    });

    $('#tblLevelApproval > tbody > tr').on('click', '.btnlevel1Approved', function (e) {
        var id = $(this).closest('tr').attr('id');
        var comment = "";
        var isaccept = "Accept";
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't approved this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#80D880',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.get(Url_Get_approval_level1,
                    {
                        id: id,
                        comment: comment,
                        IsAccept: isaccept
                    },
                    onSuccesGet_approval_level1, null, null);
            }
        });
    });
    $('#tblLevelApproval > tbody > tr').on('click', '.btnlevel1Cancel', function (e) {
        var id = $(this).closest('tr').attr('id');
        var comment = "";
        var isaccept = "Reject";
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't rejected this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#80D880',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {

                Ajaxhelper.get(Url_Get_approval_level1,
                    {
                        id: id,
                        comment: comment,
                        IsAccept: isaccept
                    },
                    onSuccesGet_approval_level1, null, null);

            }
        });
    });

  // level2 
    $('#tblLevelApproval > tbody > tr').on('click', '.btnlevel2Approved', function (e) {
        var id = $(this).closest('tr').attr('id');
        var comment = "";
        var isaccept = "Accept";
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't approved this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#80D880',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.get(Url_Get_approval_level2,
                    {
                        id: id,
                        comment: comment,
                        IsAccept: isaccept
                    },
                    onSuccesGet_approval_level1, null, null);
            }
        });
    });
    $('#tblLevelApproval > tbody > tr').on('click', '.btnlevel2Cancel', function (e) {
        var id = $(this).closest('tr').attr('id');
        var comment = "";
        var isaccept = "Reject";
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't rejected this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#80D880',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {

                Ajaxhelper.get(Url_Get_approval_level2,
                    {
                        id: id,
                        comment: comment,
                        IsAccept: isaccept
                    },
                    onSuccesGet_approval_level1, null, null);

            }
        });
    });

    //level 3
   
    $('#tblLevelApproval > tbody > tr').on('click', '.btnlevel3Approved', function (e) {
        var id = $(this).closest('tr').attr('id');
        var comment = "";
        var isaccept = "Accept";
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't approved this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#80D880',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.get(Url_Get_approval_level3,
                    {
                        id: id,
                        comment: comment,
                        IsAccept: isaccept
                    },
                    onSuccesGet_approval_level1, null, null);
            }
        });
    });
    $('#tblLevelApproval > tbody > tr').on('click', '.btnlevel3Cancel', function (e) {
        var id = $(this).closest('tr').attr('id');
        var comment = "";
        var isaccept = "Reject";
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't rejected this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#80D880',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {

                Ajaxhelper.get(Url_Get_approval_level3,
                    {
                        id: id,
                        comment: comment,
                        IsAccept: isaccept
                    },
                    onSuccesGet_approval_level1, null, null);

            }
        });
    });


    function onSuccesGet_approval_level1(data) {
        if (data.result) {
            Get_approval_list();
        }
        else
            toastr.error(data.message);
    }
}