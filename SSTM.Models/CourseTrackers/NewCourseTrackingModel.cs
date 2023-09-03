using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.CourseTrackers
{
   public class NewCourseTrackingModel
    {

        public long? Id { get; set; }
        public long? NewCourseId { get; set; }
        public long? Courseid { get; set; }
        public string NewCourseSubmitUser { get; set; }
        public DateTime? NewCourseSubmitDate { get; set; }

        public string NeedanalysisUser { get; set; }
        public DateTime? NeedanalysisDate { get; set; }

        public string CourseProposalUser { get; set; }
        public DateTime? CourseProposalDate { get; set; }

        public string AEBMeetingUser { get; set; }
        public DateTime? AEBMeetingDate { get; set; }

        public string LetterofundertakingUser { get; set; }
        public DateTime? LetterofundertakingDate { get; set; }

        public string submitedUser { get; set; }
        public DateTime? submitedDate { get; set; }

        public string SMEAssignUser { get; set; }
        public DateTime? AssignDate { get; set; }

        public string SMEReviewUser { get; set; }
        public DateTime? SMEReviewDate { get; set; }

        public string ImproveUser { get; set; }
        public DateTime? ImproveDate { get; set; }

        public string SMEAcceptUser { get; set; }
        public DateTime? SMEAcceptDate { get; set; }

        public string ReleseUser { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string RenewReminderUser { get; set; }
        public DateTime? RenewReminderDate { get; set; }
    }
}
