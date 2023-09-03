namespace SSTM.Models.CourseSharing
{
    public class SharedCourseListModel
    {
        public long CourseId { get; set; }
        public long DocId { get; set; }

        public string CourseName { get; set; }
        public string DocName { get; set; }
        public long? AirLineCourseId { get; set; }

        public bool isTraining { get; set; }
        public bool isPrinting { get; set; }
        public bool isDeveloper { get; set; }
        public int? isCentral { get; set; }
    }
}