$(function () {
    $('input[name="rdbTCStatusFilter"]').on('change', function (e) {
        e.preventDefault();

        GetTrainingCentersList();
    })

    $('#btnAddTC').on('click', function (e) {
        e.preventDefault();

        OpenAddOrUpdateTCModal(0);
    });

    InittblTrainingCenter();
    GetTrainingCentersList();
});

function InittblTrainingCenter() {
    $('#tblTrainingCenter > tbody > tr').on('click', '.btnEditTC', function (e) {
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');

        OpenAddOrUpdateTCModal(Id);
    });

    $('#tblTrainingCenter > tbody > tr').on('click', '.btnDeleteTC', function (e) {
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
                Ajaxhelper.post(deleteTrainingCenterUrl, { Id: Id, }, onSuccessDeleteTC, null, null);
        });

        function onSuccessDeleteTC(data) {
            if (data.result) {
                toastr.success("Your selected training center has been deleted.");

                GetTrainingCentersList();
            }
            else
                toastr.error(data.message);
        }
    });

    $('#tblTrainingCenter').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [0, 'asc'],
        'aoColumns': [null, null, null, null, { "bSortable": false }]
    }).fnDraw();
}

function GetTrainingCentersList() {
    $('#tblTrainingCenter').dataTable().fnDestroy();
    $('#tblTrainingCenter > tbody').empty();

    var params = { isActive: $('input[name="rdbTCStatusFilter"]:checked').val() };

    Ajaxhelper.get(getTrainingCentersListUrl, params, onSuccessGetTCList, null, null);

    function onSuccessGetTCList(data) {
        if (data.result)
            $('#tblTrainingCenter > tbody').append(data.content);
        else
            toastr.error(data.message);

        InittblTrainingCenter();
    }
}

function OpenAddOrUpdateTCModal(Id) {
    Ajaxhelper.post(getTrainingCenterByIdUrl, { Id: Id, }, onSuccessGetTCById, null, null);

    function onSuccessGetTCById(data) {
        $('#divAddOrEditTCModalContent').html(data);
        $('#AddOrEditTCModal').modal('show');
    }
}

//var handleTrainingCenterTable = function () {
//    var isActive = $('input[name="rdbTCStatusFilter"]:checked').val();

//    var table = $('#tblTrainingCenter');
//    var oTable = table.dataTable({
//        "bProcessing": true,
//        "bServerSide": true,
//        "bDestroy": true,
//        "sAjaxSource": getTrainingCenterDataGridJsonResultUrl + "?isActive=" + isActive,
//        "sServerMethod": "POST",
//        "aoColumns": [
//            { "mDataProp": "Name", "sTitle": "Name" },
//            { "mDataProp": "PhysicalAddress", "sTitle": "Physical Address" },
//            { "mDataProp": "PostalCode", "sTitle": "Postal Code" },
//            { "mDataProp": "Status", "sTitle": "Status" },
//            { "mDataProp": "Action", "sTitle": "Action", "bSortable": false }
//        ],
//        "pageLength": 0,
//        "bPaginate": false,
//        "language": {
//            "lengthMenu": " _MENU_ records"
//        },
//        "autoWidth": false,
//        //"fnRowCallback": function (nRow, aaData, iDisplayIndex, iDisplayIndexFull) {
//        //    $(nRow).attr('id', aaData.ID);
//        //},
//        "columnDefs": [{
//            // set default column settings
//            'orderable': true,
//            'targets': [1, 2, 3, 4]
//        }, {
//            "searchable": true,
//            "targets": [1, 2, 3, 4]
//        }],
//        "order": [[1, "asc"]] // set first column as a default sort by asc
//    });

//    oTable.on('click', '.btnOpenCompany', function (e) {
//        e.preventDefault();

//        var dataObject = {};
//        dataObject.companyId = $(this).attr('data-cid');;
//        dataObject.email = '@Convert.ToString(Session["AdminEmail"])';

//        $.ajax({
//            type: "post",
//            url: loginCompanyUrl,
//            data: dataObject,
//            success: function (data) {
//                if (data.result) {
//                    var url = liveSitePath + "UserLogin.aspx" + data.message + "&l=direct&pg=cashbook";
//                    // window.location.href = url;
//                    window.open(url, "_blank");
//                } else { alert(data.message); }
//            }
//        });
//    });
//};