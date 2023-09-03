$(function () {
    $('.datepicker').datepicker({
        autoclose: true,
        format: 'dd/mm/yyyy',
        todayHighlight: true
    });

    $('.datepicker').datepicker('setDate', new Date());

    InittblActivityLogs();

    $('#btnSearchLogs').on('click', function (e) {
        e.preventDefault();

        $('#tblActivityLogs').dataTable().fnDestroy();
        $('#tblActivityLogs > tbody').empty();

        Ajaxhelper.post(getActivityLogsUrl, {
            dateFrom: $('#txtDateFrom').val(),
            dateTo: $('#txtDateTo').val()
        }, onSuccessGetRecords, null, null);
    });

    $('#btnDeleteLogs').on('click', function (e) {
        e.preventDefault();

        Swal.fire({
            title: 'Are you sure?',
            text: "Do you wish to delete activity logs for selected date range? You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value)
                Ajaxhelper.post(deleteActivityLogsUrl, {
                    dateFrom: $('#txtDateFrom').val(),
                    dateTo: $('#txtDateTo').val()
                }, onSuccessDeleteRecords, null, null);
        });
    });

    $('#btnSearchLogs').trigger('click');
});

function InittblActivityLogs() {
    $('#tblActivityLogs').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [0, 'desc'],
        'aoColumns': [null, null, null]
    }).fnDraw();
}

function OpenCalendar(element) { $(element).datepicker('show'); }

function onSuccessGetRecords(data) {
    if (data.result)
        $('#tblActivityLogs > tbody').append(data.content);
    else
        toastr.error(data.message);

    InittblActivityLogs();
}

function onSuccessDeleteRecords(data) {
    if (data.result) {
        toastr.success("Your selected training center has been deleted.");

        $('#btnSearchLogs').trigger('click');
    }
    else
        toastr.error(data.message);
}