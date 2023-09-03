
$("#submitedUser").single_double_click(function () {
    Ajaxhelper.post(SMESAssginMailCourseUrl,
        {
            Courseid:Courseid,
            MasterCourse:$("#MasterCourse").val(),
            MasterCourseId:$("#MasterCourseId").val(),
            courseName: $("#Coursename").val(),
            stage:'SMEAssign',
        },
        onSuccessSMESAssginMail, null, null);
}, function () {
    if (userRole === 'SME')
    {
        toastr.warning("SME have no access to go pending section.");
    }
    localStorage.setItem("track", "1");
    localStorage.setItem("trackname", "Pending");
    window.location.href = localStorage.getItem("urltrack");
});

$("#SMEAssignUser").single_double_click(function () {
    Ajaxhelper.post(SMESAssginMailCourseUrl,
        {
            Courseid:Courseid,
            MasterCourse:$("#MasterCourse").val(),
            MasterCourseId:$("#MasterCourseId").val(),
            courseName: $("#Coursename").val(),
            stage:'SMEReview',
        },
        onSuccessSMESAssginMail, null, null);
}, function () {
    if (userRole === 'Administration' || userRole === 'Developer' || userRole === 'Director' || userRole === 'Director') {
        localStorage.setItem("track", "2");
        localStorage.setItem("trackname", "Submitted");
        window.location.href = localStorage.getItem("urltrack");
    }else
        {
        toastr.warning("Current user Pending section not rights.");
    }
   
});

$("#SMEReviewUser").single_double_click(function () {
    Ajaxhelper.post(SMESAssginMailCourseUrl,
        {
            Courseid: Courseid,
            MasterCourse: $("#MasterCourse").val(),
            MasterCourseId: $("#MasterCourseId").val(),
            courseName: $("#Coursename").val(),
            stage: 'Improvement',
        },
        onSuccessSMESAssginMail, null, null);
}, function () {
    if (userRole === 'Administration' || userRole === 'Developer' || userRole === 'Director' || userRole === 'Director') {
        localStorage.setItem("track", "3");
        localStorage.setItem("trackname", "Under Review");
        window.location.href = localStorage.getItem("urltrack");
    } else {
        toastr.warning("Current user Pending section not rights.");
    }
    
});

$("#ImproveUser").single_double_click(function () {
    Ajaxhelper.post(SMESAssginMailCourseUrl,
        {
            Courseid: Courseid,
            MasterCourse: $("#MasterCourse").val(),
            MasterCourseId: $("#MasterCourseId").val(),
            courseName: $("#Coursename").val(),
            stage: 'SMEAccept',
        },
        onSuccessSMESAssginMail, null, null);
}, function () {
   
    if (userRole === 'Administration' || userRole === 'Developer' || userRole === 'Director' || userRole === 'Director') {
        localStorage.setItem("track", "4");
        localStorage.setItem("trackname", "Under Improvement");
        window.location.href = localStorage.getItem("urltrack");
    } else {
        toastr.warning("Current user Pending section not rights.");
    }

});

$("#SMEAccept").single_double_click(function () {
    Ajaxhelper.post(SMESAssginMailCourseUrl,
        {
            Courseid: Courseid,
            MasterCourse: $("#MasterCourse").val(),
            MasterCourseId: $("#MasterCourseId").val(),
            courseName: $("#Coursename").val(),
            stage: 'Release',
        },
        onSuccessSMESAssginMail, null, null);
}, function () {
    if (userRole === 'Administration' || userRole === 'Developer' || userRole === 'Director' || userRole === 'Director') {
        localStorage.setItem("track", "7");
        localStorage.setItem("trackname", "Released");
        window.location.href = localStorage.getItem("urltrack");
    } else {
        toastr.warning("Current user Pending section not rights.");
    }
   
});
$("#ReleaseCourse").single_double_click(function () {
    Ajaxhelper.post(SMESAssginMailCourseUrl,
        {
            Courseid: Courseid,
            MasterCourse: $("#MasterCourse").val(),
            MasterCourseId: $("#MasterCourseId").val(),
            courseName: $("#Coursename").val(),
            stage: 'Done',
        },
        onSuccessSMESAssginMail, null, null);
}, function () {
  //  alert("Double")
    if (userRole === 'Administration' || userRole === 'Developer' || userRole === 'Director' || userRole === 'Director') {
        localStorage.setItem("track", "7");
        localStorage.setItem("trackname", "Released");
        window.location.href = localStorage.getItem("urltrack");
    } else {
        toastr.warning("Current user Pending section not rights.");
    }
});

function onSuccessSMESAssginMail(data)
{
    toastr.success(data);
}

function backNewCourseButtonClick(MasterCourse, MasterCourseId, Coursename)
{   
    window.location.href = '/CourseReminder/' + "?MasterCourse=" + MasterCourse + "&MasterCourseId=" + MasterCourseId + "&Coursename=" + Coursename;
}