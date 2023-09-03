$(function () {
    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        zIndexOffset: 1000000,
    });
    $('.datepicker1').datepicker({
        dateFormat: 'dd/mm/yy',
        zIndexOffset: 1000000,
    });
        
    //if ($("#ddlCourses").val() != "") {
    //    Ajaxhelper.post(getCourseDocumentsUrl, { courseId: $("#ddlCourses").val() }, onSuccessGetCourseDocs, null, null);
    //}
    //$('#ddlCourses').on('change', function (e) {
        
    //    e.preventDefault();

    //    var courseId = $('#ddlCourses').val() !== undefined && $('#ddlCourses').val() !== null ?
    //        $('#ddlCourses').val() : 0;

    //    $('#ddlCourseDocs').empty();
    //    $('#tblCourseDocVersions').dataTable().fnDestroy();
    //    $('#tblCourseDocVersions > tbody').empty();

    //    if (courseId > 0)
    //        Ajaxhelper.post(getCourseDocumentsUrl, { courseId: courseId }, onSuccessGetCourseDocs, null, null);

    //    InittblCourseDocVersions();
    //});

    $('#ddlCourseDocs').on('change', function (e) {
        e.preventDefault();

        var docId = $('#ddlCourseDocs').val() !== undefined && $('#ddlCourseDocs').val() !== null ?
            $('#ddlCourseDocs').val() : 0;

        $('#tblCourseDocVersions').dataTable().fnDestroy();
        $('#tblCourseDocVersions > tbody').empty();

        if (docId > 0)
            Ajaxhelper.post(getCourseDocVersionsUrl, { docId: docId }, onSuccessCourseDocVersion, null, null);
        else
            InittblCourseDocVersions();
    });

    InittblCourseDocVersions();
});

function InittblCourseDocVersions() {
    
    $('#tblCourseDocVersions > tbody > tr').find('input[name="radioActiveVersion"]').on('change', function (e) {
        e.preventDefault();

        var params = { docVersionId: $(this).val(), isActive: this.checked };

        Ajaxhelper.post(updateDocVersionUrl, params, onSuccessUpdateDocVersion, null, null);
    });
  
    $('#tblCourseDocVersions').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        'order': [2, 'desc'],
        'aoColumns': [{ "bSortable": false }, null, null, null, { "bVisible": false }, { "bVisible": false }, null, null, null, userRole != "Developer" ? null : { "bVisible": false }, userRole == "Administration" || userRole == "Director" ? { "bVisible": true } : { "bVisible": false }, null]
    }).fnDraw();
}

function onSuccessUpdateDocVersion(data) {
    if (data.result)
        toastr.success("Course document file version is changed successfully.");
    else
        toastr.error(data.message);
}

function onSuccessGetCourseDocs(data) {
    $('#ddlCourseDocs').html('');
    if (data.result) {
        $('#ddlCourseDocs').append($('<option></option>').val(0).html('Select Course Document'));

        $.each(data.list, function (key, value) {
            $('#ddlCourseDocs').append($('<option></option>').val(value.Id).html(value.DocName));
        });
    }
    else
        toastr.error(data.message);
}

function onSuccessCourseDocVersion(data) {
    if (data.result)
        $('#tblCourseDocVersions > tbody').html(data.content);
    else
        toastr.error(data.message);

    InittblCourseDocVersions();
}

function EditVersion(id)
{
    var tr = $('#tblCourseDocVersions > tbody > tr');
    
    $("#currentDocid").val(id);
    //var version = tr.closest('tr').find('td:eq(2)').html().split("&");
    $("#Version").val(tr.closest('tr').find('td:eq(2)').html());
    $("#VersionDate").val(tr.closest('tr').find('td:eq(3)').html());
    $("#Revision").val('');
    if (formatDate3($("#RevisionDate").val()) == "01/01/1900")
        $("#RevisionDate").val('');
    else
        $("#RevisionDate").val('');
    $("#AddEditVersionRevision").modal('show');
}
function formatDate3(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join('/');
}
$('#btneditversion').on('click', function (e) {
    Ajaxhelper.post(loadEditverionUrl,
        {
            id: $("#currentDocid").val(),
            data: JSON.stringify(SetVersionData())
        }, onSuccessloadEditverion, null, null);
});
function onSuccessloadEditverion(result)
{
    alert("Successfully Update version");
    $('#ddlCourseDocs').trigger('change');
    $("#AddEditVersionRevision").modal('hide');
}
function SetVersionData()
{
    courseDocVersionEntity = {};
    courseDocVersionEntity.Version = $("#Version").val();
    if (formatDate3($("#VersionDate").val()) != "01/01/1900")
        courseDocVersionEntity.VersionDate = datefomat($("#VersionDate").val());
    courseDocVersionEntity.revision = $("#Revision").val();
    if (formatDate3($("#RevisionDate").val()) != "01/01/1900")
        courseDocVersionEntity.revisionDate = datefomat($("#RevisionDate").val());
    return courseDocVersionEntity;
}

function ViewCourseDoc(this1, docType) {
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

function dateTimeFormat(dateTimeValue) {
    var dt = new Date(parseInt(dateTimeValue.replace(/(^.*\()|([+-].*$)/g, '')));
    var dateTimeFormat = dt.getDate() + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
    return dateTimeFormat;
}
function datefomat(date) //complage working in pay period
{
    return date.split("/").reverse().join("-");// convert date dd-mm-yyyy to yyyy-mm-dd
}

$('.ddlCourses').on('change', function () {
   
        $(".drpSubCourseId").html('');
        $(".drpSubCourseId1").html('');
        $(".drpSubCourseId2").html('');

        $(".drpSubCourseId").html("<option value = '' >-- Select Sub Course 1 --</ option >");
        $(".drpSubCourseId1").html("<option value = '' >-- Select Sub Course 2 --</ option >");
        $(".drpSubCourseId2").html("<option value = '' >-- Select Sub Course 3 --</ option >");
    
    Ajaxhelper.get(getdrpSubCoursListUrl, {
        CourseType: CourseType,
        MasterCourseId: this.value,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);
    
    if (this.value != '')
        LoadCouseDocumentlist(this.value);

    function onSuccessGetMainCourseToCoursesList(data) {
        $(".drpSubCourseId").html("");
        if (data)
            $('.drpSubCourseId').html(data);
        else
            toastr.error(data.message);
    }
});
$('.drpSubCourseId').on('change', function () {
   
        $(".drpSubCourseId1").html("<option value = '' >-- Select Sub Course 2 --</ option >");
        $(".drpSubCourseId2").html(" <option value = '' >-- Select Sub Course 3 --</ option >");
   
    Ajaxhelper.get(getdrpSubCoursListUrl, {
        CourseType: CourseType,
        MasterCourseId: this.value,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);

    if (this.value != '')
        LoadCouseDocumentlist(this.value);
    
    function onSuccessGetMainCourseToCoursesList(data) {
        $(".drpSubCourseId1").html("");
        if (data)
          $('.drpSubCourseId1').html(data);
        else
            toastr.error(data.message);
    }
});

$('.drpSubCourseId1').on('change', function (e) {
   
        $(".drpSubCourseId2").html(" <option value = '' >-- Select Sub Course 3 --</ option >");
   
    Ajaxhelper.get(getdrpSubCoursListUrl, {
        CourseType: CourseType,
        MasterCourseId: this.value,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);

    if (this.value != '')
        LoadCouseDocumentlist(this.value);

    function onSuccessGetMainCourseToCoursesList(data) {
       $(".drpSubCourseId2").html("");
        if (data)
           $('.drpSubCourseId2').html(data);
        else
            toastr.error(data.message);
    }
});

$('.drpSubCourseId2').on('change', function (e) {
    if (this.value != '')
        LoadCouseDocumentlist(this.value);
});


function LoadCouseDocumentlist(CourseId)
{
    if (CourseId != '')
        Ajaxhelper.post(getCourseDocumentsUrl, { courseId: CourseId }, onSuccessGetCourseDocs, null, null);

}