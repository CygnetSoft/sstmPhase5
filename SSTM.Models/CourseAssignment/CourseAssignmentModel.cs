﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.CourseAssignment
{
    public class CourseAssignmentModel
    {
        [Key]
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long DirectorId { get; set; }
        public long DeveloperId { get; set; }
        public long SMEId { get; set; }
        public long CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string StaffId { get; set; }
        public long HRId { get; set; }
    }
}