using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSTM.Core.IntroPage
{
    public class StudentIntroPage
    {
        public long StudentIntroPageId { get; set; }
        public long CourseId { get; set; }
        public long StudentId { get; set; }
        public string BatchId { get; set; }
        public string StudentName { get; set; }
        public string CompanyName { get; set; }
        public string IndustryType { get; set; }
        public string Qualification { get; set; }
        public string PurposeOfStudy { get; set; }
        public string StudentImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        [NotMapped]
        public string TrainerId { get; set; }

        //public string Rating { get; set; }

    }

    public class AlterStudentIntroPage
    {
        public long StudentIntroPageId { get; set; }
        public long CourseId { get; set; }
        public long StudentId { get; set; }
        public string BatchId { get; set; }
        public string StudentName { get; set; }
        public string CompanyName { get; set; }
        public string IndustryType { get; set; }
        public string Qualification { get; set; }
        public string PurposeOfStudy { get; set; }
        public string StudentImage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Rating { get; set; }

    }

    public class FeedbackView
    {
        public string TotalStudent { get; set; }
        public string FeedbackCompletedStudent { get; set; }
        public string FeedbackPendingStudent { get; set; }
        public string Rating1 { get; set; }
        public string Rating2 { get; set; }
        public string Rating3 { get; set; }

    }

    public class FeedbackViewDetails
    {
        public List<AlterStudentIntroPage> StudentFeedback { get; set; }
        public FeedbackView FeedbackView { get; set; }

    }
    public class TrainerSectionDetails
    {
        public long TrainerBatchId { get; set; }
        public long TrainerCourseid { get; set; }


        public long BatchId { get; set; }
        public string dateofbatch { get; set; }
        public long Courseid { get; set; }
        public string coursename { get; set; }
        public long trainerID { get; set; }
        public string section1SectionName { get; set; }
        public string trainername { get; set; }
    }

    public class TrainerDetails
    {
        public double batchid { get; set; }
        public string dateofbatch { get; set; }
        public int Courseid { get; set; }
        public string coursename { get; set; }
        public int section1Trainer { get; set; }
        public string section1SectionName { get; set; }
        public string trainername { get; set; }
        public string fin { get; set; }
        public string Photopath { get; set; }
        public string designation { get; set; }
        public string industry { get; set; }
        public string Experince { get; set; }
        public string Aboutme { get; set; }

    }
    public class TrainerExperience
    {
        public string industry { get; set; }
        public string Experince { get; set; }
        public string Aboutme { get; set; }


    }

    public class RiskAssessmentDeclaration
    {
        public long Id { get; set; }
        public int CourseId { get; set; }
        public int BatchId { get; set; }
        public long? StudentId { get; set; }
        public long? TrainerId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}