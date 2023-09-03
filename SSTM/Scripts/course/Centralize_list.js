var isAuthorized = (userRole === 'Administration' || userRole === 'Director' || userRole === 'Manager') ? true : false;
var selectedDocumentId = 0;
var Centralized_Document_files = [];
var stringArray = "", coursedoc = "";

function OpenAddOrUpdate(id) {
    //alert(id);
    //localStorage.setItem("master_centeral_doc_id", id);
    //window.location.href = GotoaddUrl;

    Ajaxhelper.post(AddNewCourseUrl, { id: id }, onSuccessNewCourse, null, null);

    function onSuccessNewCourse(data) {
        $('#divNewCourseDocsModal').html('');
        $('#divNewCourseDocsModal').html(data);

        $("#CentralNewCourseModal").modal('show');
    }
}


function Placeholderreplacement(id) {
    Ajaxhelper.post(AddPlaceholderPageUrl, { id: id }, onSuccessPlaceholder, null, null);

    function onSuccessPlaceholder(data) {
        $('#divCoursePlaceholderModal').html('');
        $('#divCoursePlaceholderModal').html(data);
        $("#CoursePlaceholderModal").modal('show');
    }
}
window.onload = function () {
    $('#ddlCourseStatus').on('change', function (e) {
        e.preventDefault();
        GetCoursesList();
        return;
    });

    InittblCourses();
    GetCoursesList();
};


var selectedCourseId = 0;
function InittblCourses() {
    //if (isAuthorized) {
    $('#tblCourses > tbody > tr').on('click', '.btnEditCourse', function (e) {
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');
        OpenAddOrUpdate(Id);
    });
   
    


    $('#tblCourses > tbody > tr').on('click', '.btnPlaceholder', function (e) {
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');
        Placeholderreplacement(Id);
    });


    $('#tblCourses > tbody > tr').on('click', '.btnCourseDocs', function (e) {
        e.preventDefault();
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();
    });

    var aoColumns = [null, { "bVisible": true }, { "bVisible": true }, { "bSortable": false }, { "bVisible": true },
             { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
             { "bVisible": true }, { "bVisible": true }, { "bVisible": true }, { "bVisible": true }];
    //alert(userRole);
    if (isAuthorized) {
        var aoColumns = [null, { "bVisible": true }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
             { "bVisible": false }, { "bVisible": true }, { "bVisible": true }, { "bVisible": false }, { "bVisible": true },
             { "bVisible": true }, { "bVisible": true }, { "bVisible": true }, { "bVisible": true }];

    }
    else if (userRole === 'Developer') {
        aoColumns = [null, { "bVisible": true }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
          { "bVisible": false }, { "bVisible": true }, { "bVisible": true }, { "bVisible": true }];

    }
    else if (userRole === 'SME') {
        aoColumns = [null, { "bVisible": true }, { "bVisible": false }, { "bVisible": false }, { "bVisible": true },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
          { "bVisible": false }, { "bVisible": true }, { "bVisible": true }, { "bVisible": false }];

    }
    else if (userRole === 'Trainer') {
        aoColumns = [null, { "bVisible": true }, { "bVisible": false }, { "bVisible": false }, { "bVisible": true },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }];

    }
    else if (userRole === 'Print Incharge') {
        aoColumns = [null, { "bVisible": true }, { "bVisible": false }, { "bVisible": false }, { "bVisible": true },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }];

    }
    else if (userRole === 'Print Incharge' || userRole === "HR" || userRole === "Staff" || userRole === "Manager"
            || userRole === "AEB" || userRole === "DownloadLogin" || userRole === "QP_Approval_Level1"
            || userRole === "QP_Approval_Level2" || userRole === "QP_Approval_Level3"
            || userRole === "Aassociate Developer" | userRole === "Trainer" || userRole === "DirectorStaffs" || userRole === "CI") {
        aoColumns = [{ "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
          { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }];

    }



    $('#tblCourses').dataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": false,
        'order': [0, 'asc'],

        'aoColumns': aoColumns
    }).fnDraw();



    $('#tblCourses > tbody > tr').on('click', '.btnAssignCourse', function (e) {
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(openSMEListUrl, null, onSuccessOpenSMEList, null, null);

        function onSuccessOpenSMEList(data) {
            selectedDocumentId = Id;

            $('#divAssignCourseToSMEModal').html(data);
            $('#h4SMEListModal').html('Assign ' + courseName + ' to SME');
            Ajaxhelper.post(SelectedSMEListUrl, { courseId: selectedDocumentId }, onSuccessSelectedSMEList, null, null);
            function onSuccessSelectedSMEList(result) {
                if (result != null || result != 0) {
                    $("#ddlSMEList").val(result.sme_assign_id)
                }
            }

            $('#AssignCourseToSMEModal').modal('show');
        }
    });


    $('#tblCourses > tbody > tr').on('click', '.btnSMEDocComment', function (e) {
        var Id = $(this).closest('tr').attr('id');

        Ajaxhelper.post(openSMECommentUrl, null, onSuccessopenSMECommentUrl, null, null);

        function onSuccessopenSMECommentUrl(data) {
            selectedDocumentId = Id;

            $('#divSMECommentModal').html(data);

            Ajaxhelper.post(SelectedSMEListUrl, { courseId: selectedDocumentId }, onSuccessSelectedSMEList, null, null);
            function onSuccessSelectedSMEList(result) {
                if (result != null || result != 0) {

                    $("#txtSMEComment").val(result.sme_comment);
                    $("#txtDeveloperComment").val(result.developer_sme_comment_reply);
                }
            }

            $('#SMECommentModal').modal('show');
        }
    });

    
    $('#tblCourses > tbody > tr').on('click', '.btnCourseDocs', function (e) {
        var Id = $(this).closest('tr').attr('id');
        currentstatus = $(this).closest('tr').attr('data-status');
        
        Ajaxhelper.post(openDocumentsUrl, { id: Id }, onSuccessopenDocuments, null, null);

        function onSuccessopenDocuments(data) {
            selectedDocumentId = Id;
          

            $('#divDocumentModal').html("");
            $('#divDocumentModal').html(data);

            //get_Central_data(selectedDocumentId);


            $('#DocumentList').modal('show');
        }
    });


    $('#tblCourses > tbody > tr').on('click', '.btnReleaseCourse', function (e) {
        CourseType = $(".current_CourseType").val();
        var Id = $(this).closest('tr').attr('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "You wish to release selected Central Course!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, release it!'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.post(releaseCentralDocUrl, { CentralMaster_id: Id }, onSuccessReleaseCourse, null, null);

                function onSuccessReleaseCourse(data) {
                    if (data.result) {
                        toastr.success("Selected Central course is released successfully");

                        GetCoursesList();
                    }
                    else
                        toastr.error(data.message);
                }
            }
        });
    });


    $('#tblCourses > tbody > tr').on('click', '.btnShareCourse', function (e) {
        var Id = $(this).closest('tr').attr('id');

        Ajaxhelper.post(openShareCourseUrl, { docId: Id }, onSuccessShareCourse, null, null);

        function onSuccessShareCourse(data) {
            selectedDocumentId = Id;

            $('#divCourseDocsSharingModal').html(data);

            $('#CourseDocsSharingModal').modal('show');
        }
    });

}

function View_documentFile(path, type,courseId,status) {
    window.location = Central_CourseDoclistViewerURL + "?path=" + path + "&type=" + type + "&courseid=" + courseId + "&status=" + status;
}

function GetCoursesList() {
    //alert(MasterCourse);
    $('#tblCourses').dataTable().fnDestroy();
    $('#tblCourses > tbody').empty();
    //alert(MasterCourse)
    var params = {
        statusId: $('#ddlCourseStatus').val() === undefined ? 0 : $('#ddlCourseStatus').val(),
    };

    Ajaxhelper.get(getCentralCoursesListUrl, params, onSuccessGetCentralCoursesList, null, null);

    function onSuccessGetCentralCoursesList(data) {
        if (data.result) {
            $('#tblCourses > tbody').append(data.content);
        }
        else
            toastr.error(data.message);

        InittblCourses();
        //if (userRole === 'Administration') {
        //    $("#divFilterByDocStatus").show();
        //}
    }
}


function get_Central_data(master_id) {
    Ajaxhelper.post(get_Central_dataURL,
               { document_id: master_id },
               onSuccessget_Central_data, null, null);
}

function onSuccessget_Central_data(data) {
    stringArray = "", coursedoc = "";
    if (data.Master != null) {
        coursedoc = data.Master.document_type.split(',');
        stringArray = data.Master.document_type.split(',');

    }
    if (data.result) {
        Centralized_Document_files = [];
        $.each(data.Document, function (key, item) {
            Centralized_Document = {};
            Centralized_Document.Document_File_Name = item.Document_File_Name.trim();
            Centralized_Document.Document_Type_Name = item.Document_Type_Name.trim();
            Centralized_Document_files.push(Centralized_Document);
        });

        var lilist = "";

        $("#tbodyCentraldocumentlist > tbody").empty();
        //alert(Centralized_Document_files);
        $.each(Centralized_Document_files, function (key, item) {

            if (item.Document_Type_Name == "PPT") {
                lilist += "<tr><td>"
                //
                var doc = "onclick='View_documentFile(\"" + item.Document_File_Name + "\",\"ppt\")'";
                lilist += '<a target="_parent" href="javascript:void(0)" class="btn btn-info btn-sm  ml-2"' + doc + '><i class="fa fa-file-powerpoint-o" style="font-size:24px"></i> View PPT</a>';
                lilist += "</td></tr>"
            }

        });

        for (i = 0; i < coursedoc.length; i++) {
            var counter = i + 1;

            $.each(Centralized_Document_files, function (key, item) {
                if (item.Document_Type_Name == coursedoc[i]) {
                    lilist += "<tr><td>"

                    var doc = "onclick='View_documentFile(\"" + item.Document_File_Name + "\",\"doc\")'";
                    lilist += '<a target="_parent" href="javascript:void(0)" class="btn btn-info btn-sm ml-2"  ' + doc + '><i class="fa fa-file-word-o" style="font-size:24px"></i> View ' + coursedoc[i] + ' Document</a>';

                    lilist += "</td></tr>";
                }
            });

        }

        $("#tbodyCentraldocumentlist").append(lilist);

    }
    else
        toastr.error("something went to wrong please contact to admininstrator");
}