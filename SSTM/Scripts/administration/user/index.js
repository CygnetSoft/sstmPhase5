$(function () {
    $('input[name="rdbUserStatusFilter"]').on('change', function (e) {
        e.preventDefault();

        GetUsersList();
    })

    $('#btnAddUser').on('click', function (e) {
        e.preventDefault();

        OpenAddOrUpdateUserModal(0);
    });

    InittblUsers();
    GetUsersList();
});

function InittblUsers() {
    $('#tblUsers > tbody > tr').on('click', '.btnEditUser', function (e) {
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');

        OpenAddOrUpdateUserModal(Id);
    });

    $('#tblUsers > tbody > tr').on('click', '.btnDeleteUser', function (e) {
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value)
                Ajaxhelper.post(deleteUserUrl, { Id: Id, }, onSuccessDeleteUser, null, null);
        });

        function onSuccessDeleteUser(data) {
            if (data.result) {
                toastr.success("Your selected user has been deleted.");

                GetUsersList();
            }
            else
                toastr.error(data.message);
        }
    });


    $('#tblUsers > tbody > tr').on('click', '.btnInactive', function (e) {
        
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't Inactive selected User!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value) {
                alert("Df");
                Ajaxhelper.post(ChangeStatusUrl, { userid: Id, status: "false" }, onSuccessChangeStatus, null, null);
            }
        });

        function onSuccessChangeStatus(data) {
            if (data.result) {
                toastr.success("Sucessfully Change Status.");

                GetUsersList();
            }
            else
                toastr.error(data.message);
        }
    });

    $('#tblUsers > tbody > tr').on('click', '.btnActive', function (e) {
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't Inactive selected User!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes'
        }).then((result) => {
            if (result.value)
                Ajaxhelper.post(ChangeStatusUrl, { userid: Id, status: "true" }, onSuccessChangeStatus, null, null);
        });

        function onSuccessChangeStatus(data) {
            if (data.result) {
                toastr.success("Sucessfully Change Status.");

                GetUsersList();
            }
            else
                toastr.error(data.message);
        }
    });

    $('#tblUsers').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [0, 'asc'],
        'aoColumns': [null, null, null, null, null, { "bSortable": false }]
    }).fnDraw();

    $("#AddOrEditUserModal").on('shown.bs.modal', function () {
        $(this).find('input[type="text"]:first').focus();
    });
}

function GetUsersList() {
    $('#tblUsers').dataTable().fnDestroy();
    $('#tblUsers > tbody').empty();

    var params = { isActive: $('input[name="rdbUserStatusFilter"]:checked').val() };

    Ajaxhelper.get(getUsersListUrl, params, onSuccessGetUsersList, null, null);

    function onSuccessGetUsersList(data) {
        if (data.result)
            $('#tblUsers > tbody').append(data.content);
        else
            toastr.error(data.message);

        InittblUsers();
    }
}

function OpenAddOrUpdateUserModal(Id) {
    Ajaxhelper.post(getUserByIdUrl, { Id: Id, }, onSuccessGetUserById, null, null);

    function onSuccessGetUserById(data) {
        $('#divAddOrEditUserModalContent').html(data);
        $('#AddOrEditUserModal').modal('show');
    }
}