using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.CourseTrackers
{
    public class NewCourseTrackingDataModel
    {
        [Key]
        public long Id { get; set; }
        public long NewCourseId { get; set; }

        public long? NewCourseSubmitUserid { get; set; }
        public DateTime? NewCourseSubmitDate { get; set; }

        public long? NeedanalysisUserid { get; set; }
        public DateTime? NeedanalysisDate { get; set; }

        public long? CourseProposalUserid { get; set; }
        public DateTime? CourseProposalDate { get; set; }

        public long? AEBMeetingUserid { get; set; }
        public DateTime? AEBMeetingDate { get; set; }

        public long? LetterofundertakingUserId { get; set; }
        public DateTime? LetterofundertakingDate { get; set; }

        public string RenewReminderUser { get; set; }
        public DateTime? RenewReminderDate { get; set; }
    }
}
