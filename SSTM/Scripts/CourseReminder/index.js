$(function () {
    //GetCoursesList();
});
window.onload = function () {
    InittblCourses();
    GetNewCoursesList();
};
function GetNewCoursesList() {
    $('#tblNewCourses').dataTable().fnDestroy();
    $('#tblNewCourses > tbody').empty();

    Ajaxhelper.get(getNewCoursesListUrl, {
        MasterCourse: MasterCourse,
        MasterCourseId: MasterCourseId,
    }, onSuccessGetNewCoursesList, null, null);

    function onSuccessGetNewCoursesList(data) {
        if (data.result) {
            $('#tblNewCourses > tbody').append(data.content);

        }
        else
            toastr.error(data.message);

        InittblCourses();
    }
}

function InittblCourses() {

    $('#tblNewCourses > tbody > tr').on('click', '.btnTraking', function (e) {//md
       
        var Id = $(this).closest('tr').attr('id');      
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        window.location = '/CourseReminder/NewCourseStatus?Courseid=' + Id + '&MasterCourse=' + $("#MasterCourse").val() + '&MasterCourseId=' + $("#MasterCourseId").val() + '&courseName=' + courseName + '&flag=1';
    });


    $('#tblNewCourses > tbody > tr').on('click', '.btnSubCourse', function (e) {

        // alert(localStorage.getItem("SubfolderCount"));
        var Subcount = parseInt(localStorage.getItem("SubfolderCount")) + 1;
        localStorage.setItem("SubfolderCount", Subcount);

        if (Subcount == 0) {
            // localStorage.setItem("coursestatus0", $("#ddlCourseStatus").val());
        }
        if (Subcount == 1) {
            localStorage.setItem("url0", document.URL);
            //localStorage.setItem("coursestatus0", $("#ddlCourseStatus").val());
        }

        if (Subcount == 2) {
            localStorage.setItem("url1", document.URL);
            // localStorage.setItem("coursestatus1", $("#ddlCourseStatus").val());
        }

        if (Subcount == 3) {
            //localStorage.setItem("url2", $("#ddlCourseStatus").val());
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
        window.location.href = GotonewCourseUrl + "?MasterCourse=false&MasterCourseId=" + Id + "&Coursename=" + $(this).closest('tr').find('td:eq(0)').html();

    });

    if (userRole == "Administration" || userRole == "Director") {
        var aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": true },
                  { "bSortable": true }, { "bSortable": false }, { "bSortable": false }, { "bSortable": false },
                  { "bSortable": false }, { "bSortable": true }, { "bSortable": false },{ "bVisible": true }, { "bSortable": false }];
    }
    else if (userRole == "Administration" || userRole == "Director" || userRole == "AEB") {
        var aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": true },
                { "bSortable": true }, { "bSortable": false }, { "bSortable": false }, { "bVisible": false },
                { "bVisible": false }, { "bSortable": true }, { "bSortable": false },{ "bSortable": false }, { "bSortable": false }];
    }
    else {
        var aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": true },
                { "bSortable": true }, { "bSortable": false }, { "bSortable": false }, { "bVisible": true },
                { "bSortable": false }, { "bSortable": true },{ "bSortable": false }, { "bSortable": false }, { "bSortable": false }];
    }


    $('#tblNewCourses').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": false,
        'order': [0, 'asc'],
        'aoColumns': aoColumns
    }).fnDraw();

}

function OpenAddOrUpdateCourseModal(id) {

    Ajaxhelper.post(getNewCourseByIdUrl, { Id: id }, onSuccessGetNewCourseById, null, null);

    function onSuccessGetNewCourseById(data) {
        $('#divAddOrEditCourseModal').html(data);
        $('#AddOrEditNewCourseModal').modal('show');
    }
}

function FixCourseDeveloperModal(id) {

    Ajaxhelper.post(getFixCourseDeveloperModalUrl, { Id: id }, onSuccessFixCourseDeveloperModal, null, null);

    function onSuccessFixCourseDeveloperModal(data) {
        $('#divFixCourseDeveloperModal').html(data);
        $('#FixCourseDeveloperModal').modal('show');
    }
}


function OpenAddOrUpdateReminderCourseModal(id) {

    Ajaxhelper.post(getReminderCourseByIdUrl, {
        Id: id,
        MasterCourse: MasterCourse,
        MasterCourseId: MasterCourseId,
    }, onSuccessGetReminderCourseById, null, null);

    function onSuccessGetReminderCourseById(data) {
        $('#divAddOrEditReminderCourseModal').html(data);
        $('#AddOrEditReminderCourseModal').modal('show');
    }
}


function OpenLatterCourseModal(id) {

    Ajaxhelper.post(GetlatterCourseFormUrl, { Id: id }, onSuccessGetLatterCourseById, null, null);

    function onSuccessGetLatterCourseById(data) {
        $('#divLatterCourseModal').html(data);
        $('#LatterCourseModal').modal('show');
    }
}

function NewCourseShowCourse(id, coursename) {
    window.location.href = GotoCourseUrl + "?MasterCourse=true&MasterCourseId=0&Coursename=" + coursename + "&NewCourseId=" + id;
}

