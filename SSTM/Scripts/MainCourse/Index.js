var isAuthorized = (userRole === 'Administration' || userRole === 'Director' || userRole === 'Manager') ? true : false;

function GetMainCoursesList(ctype) {
    
    $('#tblMainCourses').dataTable().fnDestroy();
    $('#tblMainCourses > tbody').empty();
    var params = {
        type: ctype,
        isActive: isAuthorized ? $('input[name="rdbCourseStatusFilter"]:checked').val() : 1
    };

    Ajaxhelper.get(getMainCoursesListUrl, params, onSuccessGetMainCoursesList, null, null);

    function onSuccessGetMainCoursesList(data) {
        if (data.result)
            $('#tblMainCourses > tbody').append(data.content);
        else
            toastr.error(data.message);

        InittblMainCourses();
    }
}

function InittblMainCourses() {
    
    if (isAuthorized) {
        $('#tblMainCourses > tbody > tr').on('click', '.btnMainEditCourse', function (e) {
            e.preventDefault();
            var Id = $(this).closest('tr').attr('id');
            OpenAddOrUpdateMainCourseModal(Id);
        });

        $('#tblMainCourses > tbody > tr').on('click', '.btnMainDeleteCourse', function (e) {
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
                    Ajaxhelper.post(deleteCourseUrl, { Id: Id, }, onSuccessDeleteCourse, null, null);
            });

            function onSuccessDeleteCourse(data) {
                if (data.result) {
                    toastr.success("Your selected user has been deleted.");

                    GetCoursesList(CourseType);
                    GetMainCoursesList(CourseType)

                }
                else
                    toastr.error(data.message);
            }
        });
    }

    $('#tblMainCourses > tbody > tr').on('click', '.btnMainSubCourse', function (e) {
       
        e.preventDefault();
        var Id = $(this).closest('tr').attr('id');
        
        if (userRole === 'Trainer' || userRole === 'Print Incharge') 
            window.location.href = gotoSubCourseList.replace('__id__', Id);
        else
        {
            if (CourseType == "other")
                window.location.href = gotoSubCourseList.replace('__id__', Id);
            else
                window.location.href = gotoStaffSubCourseList.replace('__id__', Id);
        }
    });

    var aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }];

    if (userRole === "Director") {
        if (CourseType != "staff") {
            aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }];
        }
        else {
            aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }];
        }
    }
    else if (userRole === "Manager") {
        if (CourseType != "staff") {
            aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }];
        }
        else {
            aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }];
        }
    }

    else if (userRole === 'Developer') {
            aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bVisible": false }];
    }
    else if (userRole === 'Staff') {
        aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bVisible": false }];
    }
    else if (userRole === 'SME') {
            aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bVisible": false }];
    }
    else if (userRole === 'HR')
        aoColumns = [null, { "bSortable": false }, { "bSortable": false }, { "bVisible": false }];

    $('#tblMainCourses').dataTable({
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


function OpenAddOrUpdateMainCourseModal(Id) {

    Ajaxhelper.post(getMainCourseByIdUrl, { Id: Id, type: CourseType }, onSuccessGetMainCourseById, null, null);

    function onSuccessGetMainCourseById(data) {
        
        $('#divAddOrEditmainCourseModal').html(data);
        $('#AddOrEditMainCourseModal').modal('show');
    }
}