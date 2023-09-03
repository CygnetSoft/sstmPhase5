$(function () {
    GetDownloadCoursesList();
});
window.onload = function () {
    InittblDownloadCourses();
    GetDownloadCoursesList();
    // InittblCourses()
};
function GetDownloadCoursesList() {
    //alert(MasterCourse);
    $('#tblDownloadCourses').dataTable().fnDestroy();
    $('#tblDownloadCourses > tbody').empty();
    //alert(MasterCourse)
    var params = {
        MasterCourse: MasterCourse,
        MasterCourseId: MasterCourseId,
    };

    Ajaxhelper.get(getDownloadCoursesListUrl, params, onSuccessGetDownloadCoursesList, null, null);

    function onSuccessGetDownloadCoursesList(data) {
        if (data.result) {
            $('#tblDownloadCourses > tbody').append(data.content);
        }
        else
            toastr.error(data.message);

        InittblDownloadCourses();
       
    }
}
var selectedCourseId = 0;
function  InittblDownloadCourses()
{
    $('#tblDownloadCourses > tbody > tr').on('click', '.btnSubCourse', function (e) {
        // alert(localStorage.getItem("SubfolderCount"));
        var Subcount = parseInt(localStorage.getItem("SubfolderCount")) + 1;
        localStorage.setItem("SubfolderCount", Subcount);
        if (Subcount == 0) {
            localStorage.setItem("coursestatus0", $("#ddlCourseStatus").val());
        }
        if (Subcount == 1) {
            localStorage.setItem("url0", document.URL);
            localStorage.setItem("coursestatus0", $("#ddlCourseStatus").val());
        }

        if (Subcount == 2) {
            localStorage.setItem("url1", document.URL);
            localStorage.setItem("coursestatus1", $("#ddlCourseStatus").val());
        }

        if (Subcount == 3) {
            localStorage.setItem("url2", $("#ddlCourseStatus").val());
            localStorage.setItem("coursestatus2", document.URL);
        }
        //alert(Subcount);
        //alert($("#ddlCourseStatus").val());
        if (localStorage.getItem("SubfolderCount") > 3) {
            toastr.error("Sub Folder at Atlease 3 Create");
            localStorage.setItem("SubfolderCount", 3);
            return;
        }
        e.preventDefault();
        var Id = $(this).closest('tr').attr('id');
        window.location.href = GotoCourseUrl + "?MasterCourse=false&MasterCourseId=" + Id + "&Coursename=" + $(this).closest('tr').find('td:eq(0)').html();
    });


    $('#tblDownloadCourses > tbody > tr').on('click', '.btnCourseDocs', function (e) {
        e.preventDefault();
        // alert(MasterCourse)
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(openCourseDocumentsListUrl, { courseId: Id, courseType: CourseType, MasterCourse: MasterCourse }, onSuccessGetCourseDocs, null, null);

        function onSuccessGetCourseDocs(data) {
            selectedCourseId = Id;

            $('#divAddOrEditCourseDocsModal').html(data);

            $('#h4CourseDocsModal').html('Manage documents for ' + courseName);
            $('#btnSubmitCourseDocAssessment').css('display', 'none');
            $('#btnSubmitCourseDocs').css('display', 'none');
            $('.Download').css('display', '');
            $('#btnSubmitCourseDocs').css('display', 'none');
            $('#btnAddNewCourseDoc').css('display', 'none');
            $('.btnChangeDocument').css('display', 'none');
            $('.btnSkipDocument').css('display', 'none');
            $('.docview').css('display', 'none');

            $('#AddOrEditCourseDocsModal').modal('show');
        }
    });

    var aoColumns = [{ "bSortable": true }, { "bSortable": true }, { "bSortable": false }];
    $('#tblDownloadCourses').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [0, 'asc'],
        'aoColumns': aoColumns
    }).fnDraw();
}