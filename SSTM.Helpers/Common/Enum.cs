namespace SSTM.Helpers.Common
{
    public enum TrainingDocumentStatus:int
    {
        UploadDocs=0,
        video=1,
        CommonDoc=2,
        ConfidentialDoc = 3
    }
    public enum TrainingQPStatus : int
    {
        notshared = 0,
        shared = 1,
       
    }
    public enum QPStatus : int
    {  
        pending = 0,
        review =1,
        rework = 2,
        shared = 3,
        approved=4,
    }
    public enum courseRenewRequired : int //Course reminder
    {
        No = 0,
        Yes = 1,
    }
    public enum coursestep : int //Course reminder
    {
        mailtodeveloper = 1,
        courseproposal = 2,
        AEBfixDeveloper = 3,
        latter = 4,
        credantial = 4,
    }
    public enum Courseproposal  : int //Course proposal
    {
        Pending = 1,
        Approved = 2,
        Reject = 3,
    }

    public enum CenterCourseDocType  //Course proposal
    {
        LG = 1,
        FG = 2,
        CRM = 3,
        LP = 4,
        QA= 5,
        Qwithoutanswer = 6,
        AP = 7,
        AR = 8,
        Assessmentchecklist = 9,
        RA = 10,
        SWP = 11,
        //Answerkey = 12,
    }
}
