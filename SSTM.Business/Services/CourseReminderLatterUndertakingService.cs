using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class CourseReminderLatterUndertakingService : RepositoryBase<Course_Reminder_Latter_Undertaking>, ICourseReminderLatterUndertakingService
    {
        public CourseReminderLatterUndertakingService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        //public IEnumerable<CourseListModel> GetList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        //{
        //    return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        //}

        public IEnumerable<Course_Reminder_Latter_Undertaking> GetAllRecord()
        {
            return Table.OrderBy(a => a.Id).ToList();
        }
        public Course_Reminder_Latter_Undertaking GetRecordById(long Id)
        {
            return Table.Where(a => a.Id == Id).FirstOrDefault();
        }
        public Course_Reminder_Latter_Undertaking GetFirstRecord()
        {
            return Table.OrderByDescending(a => a.Id).FirstOrDefault();
        }
        public long SaveRecord(Course_Reminder_Latter_Undertaking entity)
        {
            if (entity.Id > 0)
                Update(entity);
            else
                Add(entity);

            return entity.Id;
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
    }
}
