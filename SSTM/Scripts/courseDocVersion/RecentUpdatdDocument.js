$(function () {
    RecentAddedDocument(0);
});

function RecentAddedDocument(days)
{
    Ajaxhelper.post(getRecentDocsListUrl, { Days: days }, onSuccessGetRecentDocs, null, null);
}
function onSuccessGetRecentDocs(data)
{
    if ($.fn.DataTable.isDataTable('#tblRecentCourseDocList')) {
        $('#tblRecentCourseDocList').DataTable().destroy();
    }

    $('#tblRecentCourseDocList tbody').empty();

    if (data.result)
        $('#tblRecentCourseDocList > tbody').html(data.content);
    else
        toastr.error(data.message);

    $('#tblRecentCourseDocList').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [2, 'desc'],
        'aoColumns': [{ "bSortable": false }, null, null, null]
    }).fnDraw();
}

$("#ddlDaysFilter").change(function () {
    RecentAddedDocument($(this).val());
});

function ViewCourseDoc(this1, docType) {
    alert("Sdf");
    var courseId = $(this1).closest('tr').attr('course');
    var docId = $(this1).closest('tr').attr('id');

    Ajaxhelper.post(loadCourseDocumentUrl, { courseId: courseId, docId: docId, docType: docType }, onSuccessLoadDocument, null, null);
}

function onSuccessLoadDocument(data) {
    if (data.result)
        window.location = courseDocViewerUrl + data.fs;
    else
        toastr.error(data.message);
}