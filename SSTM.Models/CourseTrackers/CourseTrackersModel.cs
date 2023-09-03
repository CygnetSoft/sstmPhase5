using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.CourseTrackers
{
    public class CourseTrackersModel
    {
        [Key]
        public long Id { get; set; }
        public long Courseid { get; set; }
        public int? submitedUserId { get; set; }
        public DateTime? submitedDate { get; set; }
        public int? SMEAssignUserId { get; set; }
        public DateTime? AssignDate { get; set; }
        public int? SMEReviewUserId { get; set; }
        public DateTime? SMEReviewDate { get; set; }
        public int? ImproveUserId { get; set; }
        public DateTime? ImproveDate { get; set; }
        public int? SMEAcceptUserId { get; set; }
        public DateTime? SMEAcceptDate { get; set; }
        public int? ReleseUserid { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? CreateDated { get; set; }
        public DateTime? UpdateDated { get; set; }
    }
}
