function OpenAddOrUpdateReminderCourseModal() {
   
    Ajaxhelper.post(getAllReminderCoursedUrl, null, onSuccessGetAllReminderCoursedUrl, null, null);

    function onSuccessGetAllReminderCoursedUrl(data) {
        $('#divRenewalReminder').html(data);
        $('#RenewalReminderModel').modal('show');
    }
}