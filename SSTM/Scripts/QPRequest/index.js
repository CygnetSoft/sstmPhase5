
function GetQPRequestList() {
    Ajaxhelper.get(QPRequestListUrl, null, onSuccessQPRequestList, null, null);

    function onSuccessQPRequestList(data) {
        $('#tblQPRequest').dataTable().fnDestroy();
        $('#tblQPRequest > tbody').empty();
        if (data.result) {
            $('#tblQPRequest > tbody').append(data.content);
        }
        else
            toastr.error(data.message);

        InittblQPRequest();
       
    }
}

function  InittblQPRequest()
{

    $('#tblQPRequest > tbody > tr').on('click', '.btnDeleteCourse', function (e) {
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
                Ajaxhelper.post(DeleteQPRequestUrl, { Id: Id }, onSuccessDeleteQPRequest, null, null);
        });

        function onSuccessDeleteQPRequest(data) {
            if (data.result) {
                toastr.success("Successfully deleted.");
                GetQPRequestList();

            }
            else
                toastr.error(data.message);
        }
    });



    $('#tblQPRequest').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [0, 'asc'],
    }).fnDraw();
}

function OpenAddOrUpdateQPModal(Id) {
    Ajaxhelper.post(GetQPRequestByIdUrl, { Id: Id }, onSuccessGetQPRequest, null, null);

    function onSuccessGetQPRequest(data) {
        $('#divAddOrEditQPRequestModal').html(data);
        $('#AddOrEditQPRequestModal').modal('show');
    }
}
