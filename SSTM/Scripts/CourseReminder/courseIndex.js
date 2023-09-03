var isAuthorized = (userRole === 'Administration' || userRole === 'Director' || userRole === 'Manager') ? true : false;

$(function () {
    

    //localStorage.setItem("coursestatus0", '');
    //localStorage.setItem("url0", '');
    //localStorage.setItem("url1", '');
    //localStorage.setItem("url2", '');

    if (isAuthorized)
        $('input[name="rdbCourseStatusFilter"]').on('change', function (e) {
            e.preventDefault();
            GetCoursesList(CourseType);
        });
    else
        $('#divFilterByDocStatus').removeClass('col-md-9').addClass('col-md-12');

    //if (userRole === 'Director' || userRole === 'Manager')
    //{
    //    $('#ddlCourseStatus option:contains("Submitted")').prop('selected', true);
    //}
    //if (userRole === 'SME')
    //{
    //    $('#ddlCourseStatus option:contains("Under Review")').prop('selected', true);
    //}



    $('#ddlCourseStatus').on('change', function (e) {
        e.preventDefault();
        GetCoursesList(CourseType);
    });

    $("#AddOrEditCourseModal").on('shown.bs.modal', function () {
        $(this).find('input[type="text"]:first').focus();
    });

    if (MasterCourseId == 0) {
        //    if (localStorage.getItem("coursestatus0") != "") {
        //        $("#ddlCourseStatus").val(localStorage.getItem("coursestatus0"));
        //        if (localStorage.getItem("coursestatus0") = '') {
        //            if (userRole === 'Director' || userRole === 'Manager')
        //                $('#ddlCourseStatus option:contains("Submitted")').prop('selected', true);
        //             else if (userRole === 'SME')
        //                $('#ddlCourseStatus option:contains("Under Review")').prop('selected', true);
        //        }
        //        if (localStorage.getItem("track") != null || localStorage.getItem("track") != "") {
        //            $("#ddlCourseStatus").val(localStorage.getItem("track"));
        //            localStorage.setItem("track", "");
        //        }
        //        $("#ddlCourseStatus").trigger('click');
        //    }
        //    else {
        //        if (userRole === 'Director' || userRole === 'Manager')
        //            $('#ddlCourseStatus option:contains("Submitted")').prop('selected', true);
        //        else
        //            $('#ddlCourseStatus option:contains("Submitted")').prop('selected', true);

        //    }

        localStorage.setItem("SubfolderCount", 0);
        localStorage.setItem("coursestatus0", '');
        localStorage.setItem("coursestatus1", '');
        localStorage.setItem("coursestatus2", '');
        localStorage.setItem("coursestatus", '');
        localStorage.setItem("url0", '');
        localStorage.setItem("url1", '');
        localStorage.setItem("url2", '');
        
    }

    else {
    }

    if (localStorage.getItem("track") != "") {

        var coursestatus = localStorage.getItem("trackname");
        //$('#ddlCourseStatus option:contains("' + coursestatus + '")').prop('selected', true);
        //$("#ddlCourseStatus").trigger('click');
        localStorage.setItem("track", "");
        localStorage.setItem("trackname", "");
    }

    //InittblCourses();
    //GetCoursesList(CourseType);
});
window.onload = function () {

    InittblCourses();
    GetCoursesList(CourseType);

    // InittblCourses()
};
function backButtonClick() {
    if (MasterCourse == true)
        localStorage.setItem("SubfolderCount", 0);
    //window .history.back();
    localStorage.setItem("SubfolderCount", localStorage.getItem("SubfolderCount") - 1);
}

var selectedCourseId = 0;
function InittblCourses() {
    $('#tblCourses > tbody > tr').on('click', '.btnSubCourse', function (e) {
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
        window.location.href = GotonewCourseUrl + "?MasterCourse=false&MasterCourseId=" + Id + "&Coursename=" + $(this).closest('tr').find('td:eq(3)').html();

    });
    if (isAuthorized) {
        $('#tblCourses > tbody > tr').on('click', '.btnEditCourse', function (e) {
            CourseType = $(".current_CourseType").val();
            e.preventDefault();

            var Id = $(this).closest('tr').attr('id');

            OpenAddOrUpdateCourseModal(Id);
        });




        $('#tblCourses > tbody > tr').on('click', '.btnDeleteCourse', function (e) {
            CourseType = $(".current_CourseType").val();
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
                    Ajaxhelper.post(deleteCourseUrl, { Id: Id, MasterCourse: MasterCourse }, onSuccessDeleteCourse, null, null);
            });

            function onSuccessDeleteCourse(data) {
                if (data.result) {
                    toastr.success("Your selected user has been deleted.");

                    GetCoursesList(CourseType);

                }
                else
                    toastr.error(data.message);
            }
        });
    }
    $('#tblCourses > tbody > tr').on('click', '.btnTraking', function (e) {//md
        
        CourseType = $(".current_CourseType").val();
        localStorage.setItem("urltrack", window.location.href);
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        window.location = '/CourseReminder/NewCourseStatus?Courseid=' + Id + '&MasterCourse=' + $("#MasterCourse").val() + '&MasterCourseId=' + $("#MasterCourseId").val() + '&courseName=' + courseName + '&flag=0';
    });
    $('#tblCourses > tbody > tr').on('click', '.btnSkipAll', function (e) {//md
        CourseType = $(".current_CourseType").val();
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't to Skip all the review Steps ?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, Skip!'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.post(SMESKIPCourseUrl, { courseId: Id, MasterCourse: MasterCourse }, onSuccessSMESKIPCourse, null, null);

                function onSuccessSMESKIPCourse(data) {
                    GetCoursesList(CourseType);
                    toastr.success("Successfully Skip All the review Steps and check Filter in Release !");
                }
            }
        });
    });


    $('#tblCourses > tbody > tr').on('click', '.btnCourseDocs', function (e) {
        e.preventDefault();
        CourseType = $(".current_CourseType").val();
        // alert(MasterCourse)
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(openCourseDocumentsListUrl, { courseId: Id, courseType: CourseType, MasterCourse: MasterCourse }, onSuccessGetCourseDocs, null, null);

        function onSuccessGetCourseDocs(data) {
            selectedCourseId = Id;

            $('#divAddOrEditCourseDocsModal').html(data);

            $('#h4CourseDocsModal').html('Manage documents for ' + courseName);

            if (userRole === "SME") {
                $('#h4CourseDocsModal').html(courseName + ' Course Documents');

                if ($('#ddlCourseStatus').find("option:selected").text() === "Under Improvement" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Reviewed") {
                    $('#btnSubmitCourseDocAssessment').css('display', 'none');

                    //$('#tblCourseDocuments > thead > tr').find('th:nth-child(4)').hide();
                    //$('#tblCourseDocuments > tbody > tr').find('td:nth-child(4)').hide();
                }
            }
            else if (userRole === 'Director') {
                if ($('#ddlCourseStatus').find("option:selected").text() === "Submitted" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Under Review" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Under Improvement" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Reviewed" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Approved" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Released") {
                    $('#btnSubmitCourseDocs').css('display', 'none');
                }
                //else if (
                //    $('#ddlCourseStatus').find("option:selected").text() === "Approved")
                //    //||$('#ddlCourseStatus').find("option:selected").text() === "Released") 
                //{
                //    $('#btnAddNewCourseDoc, #btnSubmitCourseDocs').css('display', 'none');

                //    //$('#tblCourseDocuments > thead > tr').find('th:nth-child(4)').hide();
                //    //$('#tblCourseDocuments > tbody > tr').find('td:nth-child(4)').hide();
                //}
            }
            else if (userRole === 'Manager') {
                if ($('#ddlCourseStatus').find("option:selected").text() === "Submitted" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Under Review" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Under Improvement" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Reviewed" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Approved" ||
                    $('#ddlCourseStatus').find("option:selected").text() === "Released") {
                    $('#btnSubmitCourseDocs').css('display', 'none');
                    $('.Download').css('display', 'none');

                }
                //else if (
                //    $('#ddlCourseStatus').find("option:selected").text() === "Approved")
                //    //||$('#ddlCourseStatus').find("option:selected").text() === "Released") 
                //{
                //    $('#btnAddNewCourseDoc, #btnSubmitCourseDocs').css('display', 'none');

                //    //$('#tblCourseDocuments > thead > tr').find('th:nth-child(4)').hide();
                //    //$('#tblCourseDocuments > tbody > tr').find('td:nth-child(4)').hide();
                //}
            }
            else if (userRole === 'Developer') {
                if ($('#ddlCourseStatus').find("option:selected").text() == "Submitted" ||
                      $('#ddlCourseStatus').find("option:selected").text() == "Under Review" ||
                      $('#ddlCourseStatus').find("option:selected").text() == "Reviewed" ||
                      $('#ddlCourseStatus').find("option:selected").text() == "Approved" ||
                      $('#ddlCourseStatus').find("option:selected").text() == "Released") {
                    $('#btnSubmitCourseDocs').css('display', 'none');
                    $('#btnAddNewCourseDoc').css('display', '');
                    $('.btnChangeDocument').css('display', 'none');
                    $('.btnSkipDocument').css('display', 'none');
                }
            }

            $('#AddOrEditCourseDocsModal').modal('show');
        }
    });

    $('#tblCourses > tbody > tr').on('click', '.btnCourseDocsRemarks', function (e) {
        CourseType = $(".current_CourseType").val();
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(openCourseDocsRemarksListUrl, { courseId: Id, MasterCourse: MasterCourse }, onSuccessGetCourseDocsRemarks, null, null);

        function onSuccessGetCourseDocsRemarks(data) {
            selectedCourseId = Id;

            $('#divViewCourseDocRemarksModal').html(data);
            $('#h4CourseDocsRemarksModal').html(courseName + ' Course Documents');

            $('#ViewCourseDocRemarksModal').modal('show');
        }
    });

    $('#tblCourses > tbody > tr').on('click', '.btnAssignCourse', function (e) {
        CourseType = $(".current_CourseType").val();
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(openSMEListUrl, null, onSuccessOpenSMEList, null, null);

        function onSuccessOpenSMEList(data) {
            selectedCourseId = Id;

            $('#divAssignCourseToSMEModal').html(data);
            $('#h4SMEListModal').html('Assign ' + courseName + ' to SME');
            Ajaxhelper.post(SelectedSMEListUrl, { courseId: selectedCourseId }, onSuccessSelectedSMEList, null, null);
            function onSuccessSelectedSMEList(result) {
                if (result != null || result != 0) {
                    $("#ddlSMEList").val(result.SMEId)
                }
            }

            $('#AssignCourseToSMEModal').modal('show');
        }
    });

    $('#tblCourses > tbody > tr').on('click', '.btnAssingDownloadUsers', function (e) {
        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(OpenDownloadUserListUrl, null, onSuccessOpenDownloadUserList, null, null);

        function onSuccessOpenDownloadUserList(data) {
            selectedCourseId = Id;

            $('#divCourseDownloadModal').html(data);
            //$('#h4DownloadListModal').html('Assign ' + courseName + ' to SME');
            //Ajaxhelper.post(SelectedSMEListUrl, { courseId: selectedCourseId }, onSuccessSelectedSMEList, null, null);
            //function onSuccessSelectedSMEList(result) {
            //    if (result != null || result != 0) {
            //        $("#ddlSMEList").val(result.SMEId)
            //    }
            //}

            $('#CourseDownloadModal').modal('show');
        }
    });

    $('#tblCourses > tbody > tr').on('click', '.btnApproveCourse', function (e) {
        CourseType = $(".current_CourseType").val();
        var Id = $(this).closest('tr').attr('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "You wish to approve selected course! It will send notification and available to all the HR.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, approve it!'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.post(approveCourseUrl, { courseId: Id, MasterCourse: MasterCourse }, onSuccessApproveCourse, null, null);

                function onSuccessApproveCourse(data) {
                    if (data.result) {
                        toastr.success("Selected course is approved successfully and notification sent to all the HR.");

                        GetCoursesList(CourseType);
                    }
                    else
                        toastr.error(data.message);
                }
            }
        });
    });

    $('#tblCourses > tbody > tr').on('click', '.btnReleaseCourse', function (e) {
        CourseType = $(".current_CourseType").val();
        var Id = $(this).closest('tr').attr('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "You wish to release selected course!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, release it!'
        }).then((result) => {
            if (result.value) {
                Ajaxhelper.post(releaseCourseUrl, { courseId: Id }, onSuccessReleaseCourse, null, null);

                function onSuccessReleaseCourse(data) {
                    if (data.result) {
                        toastr.success("Selected course is released successfully and notification sent to all the HR.");

                        GetCoursesList(CourseType);
                    }
                    else
                        toastr.error(data.message);
                }
            }
        });
    });

    $('#tblCourses > tbody > tr').on('click', '.btnShareCourse', function (e) {
        CourseType = $(".current_CourseType").val();
        e.preventDefault();

        var Id = $(this).closest('tr').attr('id');
        var courseName = $(this).closest('tr').find('td:eq(0)').html();

        Ajaxhelper.post(openCourseDocsSharingUrl, { courseId: Id, MasterCourse: MasterCourse }, onSuccessCourseSharing, null, null);

        function onSuccessCourseSharing(data) {
            selectedCourseId = Id;

            $('#divCourseDocsSharingModal').html(data);
            $('#h4CourseDocsSharingModal').html('Share ' + courseName + ' Course Documents');

            $('#CourseDocsSharingModal').modal('show');
        }
    });
    
    if ($("#ddlCourseStatus").val() != 0) //filter with course show in list
    {
        
        var aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": false },
            { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": false },
            { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": false },
            { "bVisible": false }, { "bVisible": false }, { "bVisible": false }];

        if (userRole === "Administration") {
            var aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                 { "bVisible": true }, { "bVisible": true }, { "bVisible": false }, { "bVisible": true },
                 { "bVisible": false }, { "bVisible": false }, { "bVisible": true }, { "bVisible": true },
                 { "bVisible": true }, { "bVisible": true }, { "bVisible": false }];
            $('#divFilterByDocStatus').hide();
        }
        if (userRole === "Director") {
            if (CourseType != "staff") {
                if ($('#ddlCourseStatus').find("option:selected").text() == "Pending")

                    aoColumns = [null, { "bVisible": false }, { "bVisible": false },{ "bSortable": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
                    { "bVisible": true }, { "bVisible": true }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Submitted")
                    aoColumns = [null, { "bVisible": false }, { "bVisible": false },{ "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                        { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
                        { "bVisible": true }, { "bVisible": true }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Under Review" ||
                    $('#ddlCourseStatus').find("option:selected").text() == "Under Improvement")
                    aoColumns = [null, { "bVisible": false }, null,{ "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bVisible": false },
                        { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
                        { "bVisible": true }, { "bVisible": true }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Reviewed")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bSortable": false },
                        { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
                        { "bVisible": true }, { "bVisible": true }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Approved")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                        { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
                        { "bVisible": true }, { "bVisible": true }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Released")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                        { "bVisible": false }, { "bSortable": false }, { "bSortable": false }, { "bVisible": false },
                        { "bVisible": true }, { "bVisible": true }];
            }
            else {
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }];
            }
        }
        else if (userRole === "Manager") {
            if (CourseType != "staff") {
                if ($('#ddlCourseStatus').find("option:selected").text() == "Pending")
                    aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                   localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                   { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                   { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                   { "bVisible": false }, { "bVisible": false }];


                else if ($('#ddlCourseStatus').find("option:selected").text() == "Submitted")
                    aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false }
                        , { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                         { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Under Review" ||
                    $('#ddlCourseStatus').find("option:selected").text() == "Under Improvement")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                         localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                         { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bVisible": false },
                         { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                         { "bVisible": false }, { "bVisible": false }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Reviewed")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false }
                        , { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bSortable": false },
                         { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                        { "bVisible": false }, { "bVisible": false }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Approved")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false }
                        , { "bSortable": false }, { "bVisible": false }, { "bVisible": true }, { "bVisible": false },
                         { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                         { "bVisible": false }, { "bVisible": false }];

                else if ($('#ddlCourseStatus').find("option:selected").text() == "Released")
                    aoColumns = [null, { "bVisible": false }, null, { "bSortable": false }, null,
                        localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                         { "bVisible": false }, { "bSortable": false }, { "bSortable": false }, { "bVisible": false },
                        { "bVisible": false }, { "bVisible": false }];
            }
            else {
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false }
                    , { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }];
            }
        }

        else if (userRole === 'Developer') {
            if (($('#ddlCourseStatus').find("option:selected").text() === "Pending"))
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];
            else if ($('#ddlCourseStatus').find("option:selected").text() === "Under Improvement")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];

            else if ($('#ddlCourseStatus').find("option:selected").text() == "Submitted" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Under Review" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Reviewed" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Approved" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Released")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];

        }
        else if (userRole === 'Aassociate Developer') {
            if (($('#ddlCourseStatus').find("option:selected").text() === "Pending"))
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];
            else if ($('#ddlCourseStatus').find("option:selected").text() === "Under Improvement")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];

            else if ($('#ddlCourseStatus').find("option:selected").text() == "Submitted" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Under Review" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Reviewed" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Approved" ||
                $('#ddlCourseStatus').find("option:selected").text() == "Released")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];

        }
        else if (userRole === 'Staff') {
            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bSortable": false },
                localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                { "bVisible": false }, { "bVisible": false }];

        }
        else if (userRole === 'SME') {
            if ($('#ddlCourseStatus').find("option:selected").text() === "Under Review" ||
                $('#ddlCourseStatus').find("option:selected").text() === "Under Improvement" ||
                $('#ddlCourseStatus').find("option:selected").text() === "Reviewed")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bSortable": false },
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": true }, { "bVisible": false }];

            else if ($('#ddlCourseStatus').find("option:selected").text() == "Approved")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                      localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                     { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                     { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                     { "bVisible": true }], { "bVisible": false };

            else if ($('#ddlCourseStatus').find("option:selected").text() == "Released")
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, null,
                     localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                       { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                       { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                       { "bVisible": true }, { "bVisible": false }];

        }
        else if (userRole === 'HR')
            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bSortable": false },
                localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, , { "bVisible": false },
                { "bVisible": true }, { "bVisible": false }];
    }
    else { //show all course without filter
        if (userRole === "Administration") {
            var aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": true },
						localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                        { "bSortable": false }, { "bVisible": true }, { "bVisible": true },
                        { "bVisible": false }, { "bVisible": false }, { "bVisible": true }, { "bVisible": true },
                        { "bVisible": true }, { "bVisible": true }, { "bVisible": false }];
            $('#divFilterByDocStatus').hide();
        }
        if (userRole === "Director") {
            if (CourseType != "staff") {
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, null,
                       localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                       { "bSortable": false }, { "bVisible": true }, { "bSortable": false }, { "bVisible": false },
                       { "bVisible": false }, { "bVisible": true }, { "bSortable": false }, { "bVisible": true },
                       { "bVisible": true }, { "bVisible": true }];
            }
            else {
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                    { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": true }, { "bSortable": true }, { "bVisible": true },
                    { "bVisible": true }, { "bVisible": false }];
            }
        }
        else if (userRole === "Manager") {
            if (CourseType != "staff") {

                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, null,
               localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
               { "bSortable": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
               { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": true },
               { "bVisible": true }, { "bVisible": false }];
            }
            else {
                aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, null,
                    localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false }
                    , { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }, { "bSortable": false }, { "bVisible": false },
                    { "bVisible": false }, { "bVisible": false }];
            }
        }
        else if (userRole === 'Developer') {

            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                { "bSortable": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
                { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                 { "bVisible": true }, { "bVisible": false }];
        }
        else if (userRole === 'Aassociate Developer') {

            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                { "bSortable": false }, { "bVisible": true }, { "bVisible": false }, { "bVisible": false },
                { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                 { "bVisible": true }, { "bVisible": false }];
        }

        else if (userRole === 'Staff') {
            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bSortable": false },
                 localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                 { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                 { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                 { "bVisible": false }, { "bVisible": false }];

        }
        else if (userRole === 'SME') {
            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bSortable": false },
              localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
              { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
              { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
              { "bVisible": true }, { "bVisible": false }];
        }
        else if (userRole === 'HR')
            aoColumns = [null, { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, { "bSortable": false },
                 localStorage.getItem("SubfolderCount") == 3 ? { "bVisible": false } : { "bSortable": false },
                 { "bSortable": false }, { "bVisible": false }, { "bVisible": false }, { "bVisible": false },
                 { "bVisible": false }, { "bVisible": false }, { "bVisible": false }, , { "bVisible": false },
                 { "bVisible": false }, { "bVisible": false }];

    }
    $('#tblCourses').dataTable({
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

function GetCoursesList(ctype) {
   
    //alert(MasterCourse);
    $('#tblCourses').dataTable().fnDestroy();
    $('#tblCourses > tbody').empty();
    //alert(MasterCourse)
    var params = {
        isActive: isAuthorized ? $('input[name="rdbCourseStatusFilter"]:checked').val() : 1,
        statusId: $('#ddlCourseStatus').val() === undefined ? 0 : $('#ddlCourseStatus').val(),
        type: ctype,
        MasterCourse: MasterCourse,
        MasterCourseId: MasterCourseId,
        NewcourseId: $("#NewcourseId").val(),
    };

    Ajaxhelper.get(getCoursesListUrl, params, onSuccessGetCoursesList, null, null);

    function onSuccessGetCoursesList(data) {
        if (data.result) {
            $('#tblCourses > tbody').append(data.content);

        }
        else
            toastr.error(data.message);

        InittblCourses();
        if (userRole === 'Administration') {
            $("#divFilterByDocStatus").show();
        }
    }
}


function OpenAddOrUpdateCourseModal(Id) {

    Ajaxhelper.post(getCourseByIdUrl, { Id: Id, type: CourseType, MasterCourse: MasterCourse, MasterCourseId: MasterCourseId }, onSuccessGetCourseById, null, null);

    function onSuccessGetCourseById(data) {
        $('#divAddOrEditCourseModal').html(data);
        $('#AddOrEditCourseModal').modal('show');
    }
}


