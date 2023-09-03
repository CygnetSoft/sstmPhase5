namespace SSTM.Models.CourseDocRemarks
{
    public class CourseDocRemarksGridModel
    {
        public long Id { get; set; }
        public long DocId { get; set; }
        public long CourseId { get; set; }

        public string DocName { get; set; }
        public string Remarks { get; set; }
        public string Suggestion { get; set; }
        public string ReferenceFile { get; set; }

        public bool isCompleted { get; set; }
        public bool isApproved { get; set; }
        public bool isReassigned { get; set; }
        public bool isDeleted { get; set; }
    }
}