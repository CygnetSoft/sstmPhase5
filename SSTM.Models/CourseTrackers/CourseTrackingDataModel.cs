using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Models.CourseTrackers
{
    public class CourseTrackingDataModel
    {
        public long Id { get; set; }
        public long Courseid { get; set; }
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
    }
}
