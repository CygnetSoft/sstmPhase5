using System;
using System.ComponentModel.DataAnnotations;

namespace SSTM.Models.CourseDocVersion
{
    public class CourseDocVersionModel
    {
        [Key]
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public long DocId { get; set; }

        public string FileName { get; set; }

        public string Version { get; set; }

        public DateTime? VersionDate { get; set; }
        public string revision { get; set; }

        public DateTime? revisionDate { get; set; }

        public bool isActive { get; set; }
        public bool isDeleted { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}