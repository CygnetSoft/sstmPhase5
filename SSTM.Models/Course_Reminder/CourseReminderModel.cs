using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.Course_Reminder
{
    public class CourseReminderModel
    {
        [Key]
        public long Id { get; set; }
        public long DeveloperId { get; set; }
        public long? li_course_id { get; set; }
        public string course_type { get; set; }
        public string course_level { get; set; }
        public string course_type_name { get; set; }
        public string course_level_name { get; set; }
        public int? steps { get; set; }
        public string renewal_reminder { get; set; }
        public string course_name { get; set; }
        public int? reminder_days { get; set; }
        public string course_duration { get; set; }
        public string remark { get; set; }
        public string doc_file { get; set; }
        public DateTime? renew_date { get; set; }
        public int? total_renew_counter { get; set; }
        public DateTime? reminder_created_date { get; set; }
        public string course_proposal_link { get; set; }
        public string need_analysis_link { get; set; }
        public bool? is_renew_required { get; set; }
        public bool? latter_undertaking { get; set; }
        public string latter_signature { get; set; }
        public string director_latter_signature { get; set; }
        public bool? MasterCourse { get; set; }
        public long? MasterCoursId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public long? UpdatedBy { get; set; }      
    }
}
