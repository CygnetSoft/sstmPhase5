

function InittblAssesment() {

    $('#tblassesment > tbody > tr').on('click', '.btnEdit', function (e) {
        e.preventDefault();
        var id = $(this).closest('tr').attr('id');
        OpenAddOrUpdateAssessmentModal(id);
    });

    $('#tblassesment > tbody > tr').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).closest('tr').attr('id');
        Ajaxhelper.post(DeleteAssessmentPaperCourseDocumentUrl, { id: id }, onSuccessDeleteAssessment, null, null);

        function onSuccessDeleteAssessment(data) {
            if (data.result) {
                $(this).closest('tr').remove();

                if ($('#tblassesment > tbody > tr').length == 0) {
                    $('#tblassesment > tbody').append('<tr><td colspan="4" class="text-center no-data">No documents found.</td></tr>');
                }
            }
            else
                toastr.error(data.message);
        }
    });
    var aoColumns = [null, { "bVisible": true }, { "bVisible": true },  { "bVisible": false }, { "bSortable": true }];
    
    if (currentlogin == "CI")
        aoColumns = [null, { "bVisible": true }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false }];
    else
        aoColumns = [null, { "bVisible": true }, { "bVisible": true },  { "bVisible": true }, { "bSortable": true }];


    $('#tblassesment').dataTable({
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

function OpenAddOrUpdateAssessmentModal(id) {
    Ajaxhelper.post(getAssessmentUrl, { id: id }, onSuccessGetAssessment, null, null);

    function onSuccessGetAssessment(data) {
        $('#divAddOrEditAssessmentModal').html(data);
        $('#AddOrEditAssessmentModal').modal('show');
        //GetAssessmentlist();
    }
}

function GetAssessmentlist() {
    var dt = $("#selectdate").val();

    if (dt == "")
        dt = "";

    Ajaxhelper.get(getAssessmentListUrl, { date: dt },
                    onSuccessgetAssessmentList, null, null);

    function onSuccessgetAssessmentList(data) {
        $("#tblassesment").dataTable().fnDestroy()
        $('#tblassesment > tbody').html(data.content);
        InittblAssesment();
    }
}

function GetAssessment_student_list() {
    Ajaxhelper.get(getAssessment_studentListUrl, { coureseid: $("#courseid").val(), batchid :$("#batchid").val()},
                    onSuccessgetAssessment_studentList, null, null);

    function onSuccessgetAssessment_studentList(data) {
        $("#tblassesment_student").dataTable().fnDestroy()
        $('#tblassesment_student > tbody').html(data.content);

        $('#tblassesment_student').dataTable({
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "info": true,
            "autoWidth": false,
            "responsive": true,
            'order': [0, 'asc'],
        }).fnDraw();
    }
}


function SaveAssessment() {
    // Checking whether FormData is available in browser  
    //if (window.FormData !== undefined) {

        //if ($("#frmaddAssessment").valid()) {

            //var fileUpload = $("#AssessmentFile").get(0);
            //var files = fileUpload.files;

            // Create FormData object  
            var fileData = new FormData();

            // Looping over all files and add it to FormData object  
            //for (var i = 0; i < files.length; i++) {
            //    fileData.append(files[i].name, files[i]);

    //}
            if (files == "")
            {
                toastr.error("Without File Can't Save Data.");
                return true;
            }
            
            fileData.append('id', $("#ass_id").val());
            fileData.append('courseid', $("#AirLineCourseId").val());
            fileData.append('batchid', $("#drpBatchid").val());
            fileData.append('qty', $("#txtquantity").val());
            fileData.append('trainer_id', $(".drptrainer").val());
            fileData.append('fin_number', $(".drptrainer option:selected").attr('data-fin'));
            fileData.append('filename', files);
            fileData.append('course_name', $('#AirLineCourseId').select2('data')[0].text);

            $.ajax({
                url: saveAssessmentUrl,
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  ,
                data: fileData,
                success: function (data) {
                    try {
                        if (data.result) 
                            toastr.success("Successfully Saved.");                        
                        else
                            toastr.error(data.message);
                    } catch (e) {
                        toastr.error(e.message);
                    }
                },
                error: function (err) {
                    toastr.error(err.statusText + ". Please refresh the page and try again or contact our site administrator.");
                }
            }).done(function () {
                $('#AddOrEditAssessmentModal').modal('hide');
                if ($('#tblCourseDocuments > tbody > tr').length > 0 && $('#tblCourseDocuments > tbody > tr').find('.no-data').length == 0)
                    $('#btnSubmitCourseDocs').prop('disabled', false);
            });
        //}
        //else
        //    toastr.error("FormData is not supported.");
    //}

}

$("#btnDeveloperMonitorsubmit").on('click', function () {
    if ($("#userlist").val() == "") {
        toastr.error("Please select developer");
        $("#userlist").focus();
        return;
    }
    if ($("#startdate").val() == "") {
        toastr.error("Enter Start Date");
        ("#startdate").focus();
        return;
    }
    if ($("#enddate").val() == "") {
        toastr.error("Enter End Date");
        ("#enddate").focus();
        return;
    }
    var sdate = datefomat($("#startdate").val());
    var edate = datefomat($("#enddate").val());
    
    Ajaxhelper.get(GetDeveloperMonitorListUrl, { sdate: sdate, edate: edate, userid: $("#userlist").val() }, onSuccessgetDeveloperMonitorList, null, null);

    function onSuccessgetDeveloperMonitorList(data) {
        $("#tblDeveloperMonitor").dataTable().fnDestroy()
        $('#tblDeveloperMonitor > tbody').html(data.content);
        $('#tblDeveloperMonitor').dataTable({
            "paging": true,
            "lengthChange": true,
            "searching": true,
            "info": true,
            "autoWidth": false,
            "responsive": true,
            'order': [0, 'asc'],
        }).fnDraw();
    }
});
function datefomat(date) //complage working in pay period
{
    return date.split("/").reverse().join("-");// convert date dd-mm-yyyy to yyyy-mm-dd
}
