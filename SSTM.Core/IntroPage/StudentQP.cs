using System;

namespace SSTM.Core.IntroPage
{
    public class StudentQP
    {
        public long StudentQpId { get; set; }
        public int QuestionNo { get; set; }
        public long? CourseId { get; set; }
        public long? ChapterId { get; set; }
        public bool? IsQp { get; set; }
        public string Qp_Doc_Name { get; set; }
        public string Question { get; set; }
        public bool Is_Url_Choice_A { get; set; }
        public string Choice_A { get; set; }
        public string Choice_A_Url { get; set; }
        public bool Is_Url_Choice_B { get; set; }
        public string Choice_B { get; set; }
        public string Choice_B_Url { get; set; }
        public bool Is_Url_Choice_C { get; set; }
        public string Choice_C { get; set; }
        public string Choice_C_Url { get; set; }
        public bool Is_Url_Choice_D { get; set; }
        public string Choice_D { get; set; }
        public string Choice_D_Url { get; set; }
        public string Correct_Choice { get; set; }
        public long PerQuestionMark { get; set; }
        public long TotalMarks { get; set; }
        public bool IsActive { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class StudentQP_Choice
    {
        public long StudentQPChoiceId { get; set; }
        public long StudentQpId { get; set; }
        public bool IsText { get; set; }
        public string QpChoice { get; set; }
        public int Orderby { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
    public class StudentQP_Written
    {
        public long StudentQpWrittenId { get; set; }
        public long StudentQpId { get; set; }
        public string StudentNo { get; set; }
        public int QuestionNo { get; set; }
        public long CourseId { get; set; }
        public long ChapterId { get; set; }
        public long BatchId { get; set; }
        public string Choice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
