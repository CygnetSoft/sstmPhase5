function ViewCourseDoc(this1, docType) {
    var courseId = $(this1).closest('tr').attr('course');
    var docId = $(this1).closest('tr').attr('id');

    Ajaxhelper.post(loadCourseDocumentUrl, { courseId: courseId, docId: docId, docType: docType }, onSuccessLoadDocument, null, null);

    function onSuccessLoadDocument(data) {
        if (data.result)
            window.location = viewCourseDocumentRemarksUrl + data.fs;
        else
            toastr.error(data.message);
    }
}

function RenameCourseDoc(this1) {
    var oldDocName = $(this1).closest('tr').find('td:eq(0)').find('a').html().trim();

    $(this1).closest('tr').find('td:eq(0)').html('<input type="text" class="form-control" value="' + oldDocName + '" />');
    $(this1).closest('tr').find('td:eq(2)').html(
        '<input type="button" class="btn btn-success btn-sm" value="Save" onclick="SaveCourseDocRename(this);" />&nbsp;' +
        '<input type="button" class="btn btn-default btn-sm" value="Cancel" onclick="CancelCourseDocRename(this);" />');
}

function SaveCourseDocRename(this1) {
    var courseId = $(this1).closest('tr').attr('course');
    var docId = $(this1).closest('tr').attr('id');
    var docName = $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val().trim();

    Ajaxhelper.post(renameCourseDocUrl, { docId: docId, docName: docName }, onSuccessRenameCourseDoc, null, null);

    function onSuccessRenameCourseDoc(data) {
        if (data.result) {
            $(this1).closest('tr').find('td:eq(0)').html(
                "<strong><a href='javascript:void(0);' onclick='ViewCourseDoc(this, \"" + "Course" + "\");'>" + docName + "</a></strong>");

            $(this1).closest('tr').find('td:eq(2)').html(
                '<input type="button" class="btn btn-warning btn-sm" value="Rename" onclick="RenameCourseDoc(this);" />');
        }
        else
            toastr.error(data.message);
    }
}

function CancelCourseDocRename(this1) {
    var docName = $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val().trim();

    $(this1).closest('tr').find('td:eq(0)').html(
        "<strong><a href='javascript:void(0);' onclick='ViewCourseDoc(this, \"" + "Course" + "\");'>" + docName + "</a></strong>");

    $(this1).closest('tr').find('td:eq(2)').html(
        '<input type="button" class="btn btn-warning btn-sm" value="Rename" onclick="RenameCourseDoc(this);" />');
}

function RefreshCourseDocsList() {
    var header = $('#h4CourseDocsModal').html();

    Ajaxhelper.post(openCourseDocumentsListUrl, { courseId: selectedCourseId }, onSuccessGetCourseDocs, null, null);

    function onSuccessGetCourseDocs(data) {
        $('#divAddOrEditCourseDocsModal').html(data);
        $('#h4CourseDocsModal').html(header);

        if ($('#ddlCourseStatus').find("option:selected").text() == "Under Improvement" ||
            $('#ddlCourseStatus').find("option:selected").text() == "Reviewed") {
            $('#btnSubmitCourseDocAssessment').css('display', 'none');

            //$('#tblCourseDocuments > thead > tr').find('th:nth-child(4)').hide();
            //$('#tblCourseDocuments > tbody > tr').find('td:nth-child(4)').hide();
        }
    }
}

function SubmitCourseDocsAssessment() {
    Ajaxhelper.post(submitCourseDocsAssessmentUrl, { courseId: selectedCourseId, MasterCourse: MasterCourse }, onSuccessSubmitCourseDocsAssessment, null, null);

    function onSuccessSubmitCourseDocsAssessment(data) {
        if (data.result) {
            if (CourseType == "")
                CourseType = "other";

            GetCoursesList(CourseType);
            $('#ViewCourseDocRemarksModal').modal('hide');
            $('#AddOrEditCourseDocsModal').modal('hide');
            
            toastr.success(data.message);
        }
        else
            toastr.error(data.message);
    }
}

function editCourseDocument(documentid, value) {

    Ajaxhelper.get(LoadEditor, { d: documentid, s: value }, onSuccessLoadEditor, null, null);

    function onSuccessLoadEditor(data) {
        $("#ZohoOffice").show();
        $("#divZohoOffice").html(data);

        $('#ZohoOffice').modal('show');
    }
}

