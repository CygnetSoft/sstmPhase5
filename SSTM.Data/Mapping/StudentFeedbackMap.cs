using System.Data.Entity.ModelConfiguration;
using SSTM.Core.IntroPage;

namespace Data.Mapping
{
    public class StudentFeedbackMap : EntityTypeConfiguration<StudentFeedback>
    {
        public StudentFeedbackMap()
        {
            ToTable("StudentFeedback");
            HasKey(a => a.FeedbackId);
        }
    }
}
