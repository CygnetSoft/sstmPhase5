using System;

namespace SSTM.Core.IntroPage
{
    public class StudentFeedback
    {
        public long FeedbackId { get; set; }
        public string StudentName { get; set; }
        public long StudentId { get; set; }
        public long BatchId { get; set; }
        public long CourseId { get; set; }
        public string Rating { get; set; }
        public string Rating_Description { get; set; }
        public bool IsActive { get; set; }
        public string trainerid { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
