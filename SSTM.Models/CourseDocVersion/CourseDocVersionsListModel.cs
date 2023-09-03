using System;

namespace SSTM.Models.CourseDocVersion
{
    public class CourseDocVersionsListModel
    {
        public long Id { get; set; }
        public long DocId { get; set; }

        public string FileName { get; set; }

        public string Version { get; set; }

        public string VersionDate { get; set; }
        public string revision { get; set; }

        public string revisionDate { get; set; }

        public bool isActive { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }
        public string DocName { get; set; }
        public string createdby_User { get; set; }
        public string Updateby_User { get; set; }
        
    }
}