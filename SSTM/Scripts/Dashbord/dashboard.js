$(document).ready(function () {
    $("#load").show();
    $.ajax({
        type: "GET",
        url: DashboardUrl,
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            successFuncLabourPieChart(response);
            $("#load").hide();
        },
    });
    $("#load").hide();

    function successFuncLabourPieChart(jsondata) {
        var data = {};
        var countryNames = [];
       
        var active = 0;
        var pending = 0;
        var nonactive = 0;
        jsondata.forEach(function (e) {
            var totalcourse = e.ReleaseCourse + e.PendingDeveloperCourse + e.PendingSMECourse + e.PendinSharingCourse;
            var totalPendingcourse = e.PendingDeveloperCourse + e.PendingSMECourse + e.PendinSharingCourse;
            $("#Totalcourse").text(totalcourse);
            $("#TotalPendingcourse").text(totalPendingcourse);
            
              google.load("visualization", "1", { packages: ["corechart"] });
            google.setOnLoadCallback(drawBikePieChart);
            function drawBikePieChart() {

                var data = google.visualization.arrayToDataTable([
                  ['name', 'Total'],
                  ['Release Course', e.ReleaseCourse],
                  ['Pending Developer Course', e.PendingDeveloperCourse],
                  ['Pending SME Course', e.PendingSMECourse],
                  ['Pending Release Course', e.PendinSharingCourse],
                  ['Pending Renewal course', e.PendinRenewalCourse],
                  ['Pending New course', e.PendingNewCourse],
                ]);

                var options = {
                    title: 'Course Chart',
                    is3D: true,
                    pieStartAngle: 100,

                   // pieHole: 0.4,

                    colors: ['#A6C661', '#39D8FF', '#F2750F', '#7B59A4','#be2d0e','#f94603'],
                };

                var chart = new google.visualization.PieChart(document.getElementById('Course_chart'));
                function selectHandler() {
                    var selectedItem = chart.getSelection()[0];
                    if (selectedItem) {
                       
                        var topping = data.getValue(selectedItem.row, 0);
                        if (topping == "Release Course" || topping=="Pending Release Course")
                        {
                            localStorage.setItem("track", "0");
                            localStorage.setItem("trackname", "Released");
                            window.location.href =CourseStatusUrl;
                        }
                        else if (topping == "Pending Developer Course" ) {
                            localStorage.setItem("track", "0");
                            localStorage.setItem("trackname", "Pending");
                            window.location.href =CourseStatusUrl;
                        }
                        else if (topping == "Pending SME Course") {
                            localStorage.setItem("track", "0");
                            localStorage.setItem("trackname", "Under Review");
                            window.location.href = CourseStatusUrl;
                        }
                        else if (topping == "Pending Renewal course") {
                            Ajaxhelper.post(NewCourseStatusUrl, { "status": "Renewal" }, onSuccessNewCourseStatus, null, null);
                            $("#NewCourseModelTitle").text("Pending Renewal course");
                        }
                        else if (topping == "Pending New course") {
                            Ajaxhelper.post(NewCourseStatusUrl, { "status": "pending" }, onSuccessNewCourseStatus, null, null);
                            $("#NewCourseModelTitle").text("Pending New course");
                        }
                        
                    }
                }
                google.visualization.events.addListener(chart, 'select', selectHandler);
                chart.draw(data, options);
            }
        });
    }
});

function onSuccessNewCourseStatus(data) {
    if (data) {
        $("#bodyNewCourseData").html("");
        $.each(data, function (key, item) {
            var tr = "<tr><td>" + item.course_name + "</td></tr>";
            $("#bodyNewCourseData").append(tr);
        });
      
        $("#ShowNewCourseSatus").modal('show');
    }
    else
        toastr.error(data.message);
}