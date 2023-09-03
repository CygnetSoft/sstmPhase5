var courseDocStatus = "";

$(function () {
   
    $('.dialog-background').show();
    $('#btnAddNewCourseDoc').on('click', function (e) {
        CourseType = $(".current_CourseType").val();
        e.preventDefault();

        courseDocStatus = "";

        if ($('#tblCourseDocuments > tbody > tr').find('.no-data').length > 0)
            $('#tblCourseDocuments > tbody').empty();

        var courseId = selectedCourseId;
        var remotdmove="";
        //if(userRole=="Director"||userRole=="Director" )
        //{
        //    remotdmove= "<td></td>"; 
        //}
        
        $('#tblCourseDocuments > tbody').append(
            "<tr id='0' course='" + courseId + "'>" +
            "<td><input type='text' class='form-control docnamedata' placeholder='Document Name'>" +
            " <input type='text' id='verion' class='form-control' placeholder='Version'/>" +
             "<input type='text' id='verionDate' class='form-control  masking' readonly  placeholder='DD/MM/YYYY'/>" +
            "</td>" +
            "<td><input type='file' /></td>" +
            "<td class='text-center'></td>" +
            "<td class='text-center'>" +
            '<input type="button" class="btn btn-success btn-sm" title="Save course document" value="Save" onclick="SaveCourseDoc(this);" />' +
            '<input type="button" class="btn btn-default btn-sm ml-1" title="Clear line item" value="Cancel" onclick="CancelCourseDoc(this,0);" />');// +
            //"</td>" +
            ////remotdmove +
            
            //"</tr>"

        $('#btnAddNewCourseDoc, #btnSubmitCourseDocs').prop('disabled', true);
        $('#tblCourseDocuments > tbody > tr:last').find('td:eq(0)').find('input[type="text"]').focus();
        //$('#verionDate').datepicker().inputmask("date", { placeholder: "DD/MM/YYYY", yearrange: { minyear: 1700 } });
        //$('.masking').inputmask("99/99/9999", { placeholder: 'dd/mm/yyyy' });
       
        $(".docnamedata").focus();
        $('.masking').datepicker({
            todayBtn: "linked",
            autoclose: true,
            todayHighlight: true,
            format: "dd/mm/yyyy"
        });
    });
  

    $('#btnSubmitCourseDocs').on('click', function (e) {
        CourseType = $(".current_CourseType").val();
        e.preventDefault();

        var courseId = $('#tblCourseDocuments > tbody > tr:eq(0)').attr('course');
        if (courseId === undefined || courseId === 0)
            toastr.error('Please upload and complete all the documents and then try again.');
        else
            Ajaxhelper.post(submitCourseDocumentsUrl, { CourseId: courseId, MasterCourse: MasterCourse }, onSuccessSubmitCourseDocs, null, null);
    });
    $('.dialog-background').hide();
});

function onSuccessSubmitCourseDocs(data) {
    if (data.result) {
        toastr.success(data.message);

        if (CourseType == "")
            CourseType = "other";

        GetCoursesList(CourseType);
        $('#AddOrEditCourseDocsModal').modal('hide');
    }
    else
        toastr.error(data.message);
}

function ChangeCourseDoc(this1) {
    courseDocStatus = $(this1).closest('tr').find('td:eq(2)').html();

    $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').prop('disabled', false);
    $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').css('display', 'block').prop('disabled', false);
    $(this1).closest('tr').find('td:eq(2)').html('');
    $(this1).closest('tr').find('td:eq(3)').html(
        '<input type="button" class="btn btn-success btn-sm" value="Update" onclick="SaveCourseDoc(this);" />' +
        '<input type="button" class="btn btn-default btn-sm ml-1" value="Cancel" onclick="CancelCourseDoc(this,1);" />');
}
function datefomat(date) //complage working in pay period
{
    return date.split("/").reverse().join("-");// convert date dd-mm-yyyy to yyyy-mm-dd
}
function SaveCourseDoc(this1) {
    // Checking whether FormData is available in browser  
    if (window.FormData !== undefined) {
        
        var fileUpload = $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
           
        }
        if ($('#currentcoursestatus').val() == "")
            $('#currentcoursestatus').val(0);

        //alert($('#currentcoursestatus').val());
        // Adding one more key to FormData object  
        fileData.append('Id', $(this1).closest('tr').attr('id'));
        fileData.append('CourseId', $(this1).closest('tr').attr('course').trim());
        fileData.append('DocName', $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val().trim());
        fileData.append('currentcoursestatus',$('#currentcoursestatus').val());
        fileData.append('isreplace', 0);
        fileData.append('version', $(this1).closest('tr').find('td:eq(0)').find('#verion').val().trim());
        fileData.append('versiondate', datefomat($(this1).closest('tr').find('td:eq(0)').find('#verionDate').val().trim()));

        $.ajax({
            url: saveCourseDocumentUrl,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  ,
            data: fileData,
            success: function (data) {
                try {
                    if (data.result) {
                        $(this1).closest('tr').attr('id', data.Id);
                        $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').prop('disabled', true);
                        $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').css('display', 'none').prop('disabled', true);

                        $(this1).closest('tr').find('td:eq(1)').html(
                            '<input type="file" style="display:none; padding-bottom:10px;" value=' + data.Filename + ' disabled />' +
                            '<div onclick="editCourseDocument(' + data.Id + ',1)" style="color:#dc3545;cursor:pointer">' +
                            //<a href="/CourseDoc/GetCourseDocFile?d=' + data.Id + '&s=1" target="_blank">' +
                                '<strong>' + data.FileName + '</strong></div>');

                        $(this1).closest('tr').find('td:eq(2)').html('<label class="badge badge-info">Complete</label>');

                        var DownoadCourseButton = (userRole == "Director" ) ?
                            "<a href='/CourseDoc/DownoadCourseDocFile?d=" + data.Id + "&s=1' class='btn btn-info btn-sm ml-1 Download'>Download</a>" : "";

                        var viewCourseButton = "<a href='javascript:void(0);' onclick='ViewCourseDoc(this, \"" + "Course" + "\");' class='btn btn-info btn-sm ml-1 docview' title='View course document'>View </a>";

                        var deleteCourseButton = (userRole == "Director") ?
                            '<input type="button" class="btn btn-danger btn-sm ml-1" title="Delete course document" value="Delete" onclick="DeleteCourseDoc(this);" />' : "";
                        var MoveToOldDoc = (userRole == "Director" || userRole == "Administration" || userRole == "Developer") ?
                            '<input type="button" class="btn btn-danger btn-sm ml-1" title="Move Old course document" value="Move To Old" onclick="MoveCourseToOldDoc(this);" />' : "";

                        $(this1).closest('tr').find('td:eq(3)').html(viewCourseButton + DownoadCourseButton +
                            '<input type="button" class="btn btn-primary btn-sm ml-1" title="Change course document" value="Change" onclick="ChangeCourseDoc(this);" />' +
                            '<input type="button" class="btn btn-default btn-sm ml-1" title="Skip course document" value="Skip" onclick="SkipCourseDoc(this);" />' +
                            deleteCourseButton + MoveToOldDoc);

                        if ($('#tblCourseDocuments > tbody > tr').length > 0 && $('#tblCourseDocuments > tbody > tr').find('.no-data').length == 0)
                            $('#btnSubmitCourseDocs').prop('disabled', false);
                    }
                    else
                        toastr.error(data.message);
                } catch (e) {
                    alert(e.message);
                }
            },
            error: function (err) {
                toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
            }
        }).done(function () {
            $('#btnAddNewCourseDoc').prop('disabled', false);

            if ($('#tblCourseDocuments > tbody > tr').length > 0 && $('#tblCourseDocuments > tbody > tr').find('.no-data').length == 0)
                $('#btnSubmitCourseDocs').prop('disabled', false);
        });
    }
    else
        toastr.error("FormData is not supported.");
}

function SkipCourseDoc(this1) {
    var Id = $(this1).closest('tr').attr('id');

    Ajaxhelper.post(skipCourseDocumentUrl, { Id: Id }, onSuccessSkipCourseDoc, null, null);

    function onSuccessSkipCourseDoc(data) {
        if (data.result) {
            $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val("N/A").prop('disabled', true);
            $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').css('display', 'none').prop('disabled', true);
            $(this1).closest('tr').find('td:eq(2)').html('<label class="badge badge-secondary">Skipped</label>');
            $(this1).closest('tr').find('td:eq(3)').html(
                '<input type="button" class="btn btn-warning btn-sm" value="Reattach" onclick="ReattachCourseDoc(this);" />');
        }
        else
            toastr.error(data.message);
    }
}

function ReattachCourseDoc(this1) {
    courseDocStatus = $(this1).closest('tr').find('td:eq(2)').html();

    $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val("").prop('disabled', false);
    $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').css('display', 'none').prop('disabled', true);
    $(this1).closest('tr').find('td:eq(2)').html("");
    $(this1).closest('tr').find('td:eq(3)').html(
        '<input type="button" class="btn btn-success btn-sm" value="Update" onclick="SaveReattachCourseDoc(this);" />' +
        '<input type="button" class="btn btn-default btn-sm ml-1" value="Cancel" onclick="CancelCourseDoc(this,2);" />');
}

function SaveReattachCourseDoc(this1) {
    var params = {
        Id: $(this1).closest('tr').attr('id'),
        DocName: $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val()
    }

    Ajaxhelper.post(reattachCourseDocumentUrl, params, onSuccessReattachCourseDoc, null, null);

    function onSuccessReattachCourseDoc(data) {
        if (data.result) {
            $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val(data.DocName).prop('disabled', true);
            $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').css('display', 'none').prop('disabled', true);
            $(this1).closest('tr').find('td:eq(2)').html('<label class="badge badge-info">Complete</label>');

            var DownoadCourseButton = (userRole == "Director") ?
                "<a href='/CourseDoc/DownoadCourseDocFile?d=" + $(this1).closest('tr').attr('id') + "&s=1' class='btn btn-info btn-sm ml-1 Download'>Download</a>" : "";

            var viewCourseButton = "<a href='javascript:void(0);' onclick='ViewCourseDoc(this, \"" + "Course" + "\");' class='btn btn-info btn-sm ml-1 docview' title='View course document'>View </a>";

            var deleteCourseButton = (userRole == "Director") ?
                '<input type="button" class="btn btn-danger btn-sm ml-1" title="Delete course document" value="Delete" onclick="DeleteCourseDoc(this);" />' : "";
            var MoveToOldDoc = (userRole == "Director" || userRole == "Administration" || userRole == "Developer") ?
                          '<input type="button" class="btn btn-danger btn-sm ml-1" title="Move Old course document" value="Move To Old" onclick="MoveCourseToOldDoc(this);" />' : "";
            $(this1).closest('tr').find('td:eq(3)').html(viewCourseButton + DownoadCourseButton +
                '<input type="button" class="btn btn-primary btn-sm ml-1" title="Change course document" value="Change" onclick="ChangeCourseDoc(this);" />' +
                '<input type="button" class="btn btn-default btn-sm ml-1" title="Skip course document" value="Skip" onclick="SkipCourseDoc(this);" />' +
                deleteCourseButton + MoveToOldDoc);
        }
        else
            toastr.error(data.message);
    }
}

function CancelCourseDoc(this1, operation) {
    $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').prop('disabled', true);
    $(this1).closest('tr').find('td:eq(1)').find('input[type="file"]').css('display', 'none').prop('disabled', true);
    $(this1).closest('tr').find('td:eq(2)').html(courseDocStatus);

    if (operation === 1) {
        var downloadCourseButton = (userRole == "Director" || userRole == "Developer") ?
            "<a href='/CourseDoc/DownoadCourseDocFile?d=" + $(this1).closest('tr').attr('id') + "&s=1' class='btn btn-info btn-sm  ml-1 Download'>Download</a>" : "";

        var viewCourseButton = "<a href='javascript:void(0);' onclick='ViewCourseDoc(this, \"" + "Course" + "\");' class='btn btn-info btn-sm ml-1 docview' title='View course document'>View </a>";

        var deleteCourseButton = (userRole == "Director") ?
            '<input type="button" class="btn btn-danger btn-sm ml-1" title="Delete course document" value="Delete" onclick="DeleteCourseDoc(this);" />' : "";
        var MoveToOldDoc = (userRole == "Director" || userRole == "Administration" || userRole == "Developer") ?
                 '<input type="button" class="btn btn-danger btn-sm ml-1" title="Move Old course document" value="Move To Old" onclick="MoveCourseToOldDoc(this);" />' : "";
        $(this1).closest('tr').find('td:eq(3)').html(viewCourseButton + downloadCourseButton +
            '<input type="button" class="btn btn-primary btn-sm ml-1" title="Change course document" value="Change" onclick="ChangeCourseDoc(this);" />' +
            '<input type="button" class="btn btn-default btn-sm ml-1" title="Skip course document" value="Skip" onclick="SkipCourseDoc(this);" />' +
            deleteCourseButton + MoveToOldDoc);
    }
    else if (operation === 2) {
        $(this1).closest('tr').find('td:eq(0)').find('input[type="text"]').val('N/A');
        $(this1).closest('tr').find('td:eq(3)').html(
            '<input type="button" class="btn btn-primary btn-sm" value="Reattach" onclick="ReattachCourseDoc(this);" />');
    }
    else if (operation === 0) {
        $(this1).closest('tr').remove();

        if ($('#tblCourseDocuments > tbody > tr').length == 0) {
            $('#tblCourseDocuments > tbody').append('<tr><td colspan="4" class="text-center no-data">No documents found.</td></tr>');
            $('#btnSubmitCourseDocs').prop('disabled', true);
        }
    }

    $('#btnAddNewCourseDoc').prop('disabled', false);

    if ($('#tblCourseDocuments > tbody > tr').length > 0 && $('#tblCourseDocuments > tbody > tr').find('.no-data').length == 0)
        $('#btnSubmitCourseDocs').prop('disabled', false);
}

function DeleteCourseDoc(this1) {
    var Id = $(this1).closest('tr').attr('id');

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
            Ajaxhelper.post(deleteCourseDocumentUrl, { Id: Id }, onSuccessDeleteCourseDoc, null, null);
    });

    function onSuccessDeleteCourseDoc(data) {
        if (data.result) {
            $(this1).closest('tr').remove();

            if ($('#tblCourseDocuments > tbody > tr').length == 0) {
                $('#tblCourseDocuments > tbody').append('<tr><td colspan="4" class="text-center no-data">No documents found.</td></tr>');
                $('#btnSubmitCourseDocs').prop('disabled', true);
            }
        }
        else
            toastr.error(data.message);
    }
}


function MoveCourseToOldDoc(this1) {
    var Id = $(this1).closest('tr').attr('id');
  
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't to move this document in old section!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, Move To Old!'
    }).then((result) => {
        if (result.value)
            Ajaxhelper.post(MoveOldCourseDocumentUrl, { Id: Id }, onSuccessMoveOldCourseDoc, null, null);
    });

    function onSuccessMoveOldCourseDoc(data) {
        if (data.result) {
            $(this1).closest('tr').remove();
            if ($('#tblCourseDocuments > tbody > tr').length == 0) {
                $('#tblCourseDocuments > tbody').append('<tr><td colspan="4" class="text-center no-data">No documents found.</td></tr>');
                $('#btnSubmitCourseDocs').prop('disabled', true);
            }
            toastr.success("Succesfully course document move to old Folder");
        }
        else
            toastr.error(data.message);
    }
}

function ViewCourseDoc(this1, docType) {
    var courseId = $(this1).closest('tr').attr('course');
    var docId = $(this1).closest('tr').attr('id');
    
    Ajaxhelper.post(loadCourseDocumentUrl, { courseId: courseId, docId: docId, docType: docType }, onSuccessLoadDocument, null, null);

    function onSuccessLoadDocument(data) {
        if (data.result)
            window.location = courseDocViewerUrl + data.fs;
        else
            toastr.error(data.message);
    }
}

function editCourseDocument(documentid, value) {
    Ajaxhelper.post(LoadEditor, { d: documentid, s: value }, onSuccessLoadEditor, null, null);
    function onSuccessLoadEditor(data) {
        $("#ZohoOffice").show();
        $("#divZohoOffice").html(data);
        
        $('#ZohoOffice').modal('show');
    }
}

///Course move one course to other course

$('#tblCourseDocuments > tbody > tr').on('change', '.MainCourseId', function (e) {
    
    var docId = $(this).closest('tr').attr('id');
    var rowEdit = $(this).closest('tr');
    rowEdit.find('.Subcourseid').html('');
    rowEdit.find('.drpSubCourseId').html('');
    rowEdit.find('.drpSubCourseId').html('--Manage Course--');
   
    Ajaxhelper.get(getdrpSubCoursListUrl, {
        CourseType: CourseType,
        MasterCourseId: this.value,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);

    function onSuccessGetMainCourseToCoursesList(data) {
        rowEdit.find(".drpSubCourseId").html("");
        if (data)
            rowEdit.find('.drpSubCourseId').html(data);
        else
            toastr.error(data.message);
    }
});

$('#tblCourseDocuments > tbody > tr').on('change', '.drpSubCourseId', function (e) {
    var rowEdit = $(this).closest('tr');
    
    Ajaxhelper.get(getdrpSubCoursListUrl, {
        CourseType: CourseType,
        MasterCourseId: this.value,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);

    function onSuccessGetMainCourseToCoursesList(data) {
        rowEdit.find(".drpSubCourseId1").html("");
        if (data)
            rowEdit.find('.drpSubCourseId1').html(data);
        
        else
            toastr.error(data.message);
    }
});

$('#tblCourseDocuments > tbody > tr').on('change', '.drpSubCourseId1', function (e) {
    var rowEdit = $(this).closest('tr');
    
    Ajaxhelper.get(getdrpSubCoursListUrl, {
        CourseType: CourseType,
        MasterCourseId: this.value,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);

    function onSuccessGetMainCourseToCoursesList(data) {
        rowEdit.find(".drpSubCourseId2").html("");
        if (data)
            rowEdit.find('.drpSubCourseId2').html(data);
        else
            toastr.error(data.message);
    }
});

function GetCourseList(rowEdit,courseid)
{
    Ajaxhelper.get(getMainCourseToCoursesListUrl, {
        CourseType: CourseType,
        MasterCourseId: courseid,
        selectedSubCourse: 0
    }, onSuccessGetMainCourseToCoursesList, null, null);

    function onSuccessGetMainCourseToCoursesList(data) {
        rowEdit.find(".Subcourseid").html("");
        if (data)
            rowEdit.find('.Subcourseid').html(data);
        else
            toastr.error(data.message);
    }
};

$('#tblCourseDocuments > tbody > tr').on('click', '.btnmovecourse', function (e) {
    //e.preventDefault();
    $('.dialog-background').show();
    var data = $(this).closest('tr').find('td');
    var courseidLast=0;
    if (data.find('.MainCourseId').val()!=0)
        courseidLast = data.find('.MainCourseId').val();
     if (data.find('.drpSubCourseId').val() != 0)
        courseidLast = data.find('.drpSubCourseId').val();
     if (data.find('.drpSubCourseId1').val() != 0)
        courseidLast = data.find('.drpSubCourseId1').val();
     if (data.find('.drpSubCourseId2').val() != 0)
        courseidLast = data.find('.drpSubCourseId2').val();
    if (courseidLast == 0)
    {
        toastr.danger("Select Course in Dropdown !!");
        $('.dialog-background').hide();
        return;
    }
    var OldCourseId = $(this).closest('tr').attr('course');
   
    var data1 = $(this).closest('tr');

    $(this).prop('disabled', true);
    //$(this).val("Please Wait");
    //var MainCourseIds = data.find('.drpSubCourseId').val();
    var courseIds = courseidLast;
    var DocumentIds = $(this).closest('tr').attr('id');
    //if (MainCourseIds == '') {
    //    toastr.warning("Select Sub Course !!");
    //    $(this).prop('disabled', false);
    //    $(this).val("Document Move");
    //    $('.dialog-background').hide();
    //    return;
    //}
    //if (courseIds == '') {
    //    toastr.warning("Select Sub Course !!");
    //    $(this).prop('disabled', false);
    //    $(this).val("Document Move");
    //    $('.dialog-background').hide();
    //    return;
    //}

    Ajaxhelper.get(MoveDocumentOtherCourse,
        {  
            courseId: courseIds,
            DocumentId: DocumentIds,
            OldcourseId: OldCourseId,
            FileName: $(this).closest('tr').find('td:eq(1)').text().trim()
        }
        , onSuccessMoveDocumentOtherCourse, null, null);

    function onSuccessMoveDocumentOtherCourse(data) {
        if (data) {
            toastr.warning("Course Successfully Move !!");
            data1.remove();
            $(this).prop('disabled', false);
            $(this).val("Document Move");
        }
        else {
            toastr.error(data);
            $(this).prop('disabled', false);
            $(this).val("Document Move");
        }
        $('.dialog-background').hide();
    }
});