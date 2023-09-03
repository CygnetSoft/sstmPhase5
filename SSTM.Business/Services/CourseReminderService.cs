using SSTM.Business.Interfaces;
using SSTM.Core.Course_Reminder;
using SSTM.Data.Infrastructure;
using System.Collections.Generic;
using System.Linq;

using System.Data.Entity;

namespace SSTM.Business.Services
{
    public class CourseReminderService : RepositoryBase<CourseReminder>, ICourseReminderService
    {
        public CourseReminderService(IRepositoryContext repositoryContext) : base(repositoryContext) { }
        //public IEnumerable<CourseListModel> GetList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        //{
        //    return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        //}

        public IEnumerable<CourseReminder> GetAllRecord(bool MasterCourse, long MasterCourseId)
        {
            return Table.Where(a=>a.MasterCourse== MasterCourse && a.MasterCoursId== MasterCourseId).OrderBy(a => a.Id).ToList();
        }
        public IEnumerable<CourseReminder> GetAllRecordwithDeveloper(bool MasterCourse, long MasterCourseId,long developerid)
        {
            return Table.Where(a => a.MasterCourse == MasterCourse && a.MasterCoursId == MasterCourseId && a.DeveloperId== developerid).OrderBy(a => a.Id).ToList();
        }
        public CourseReminder GetRecordById(long Id)
        {
            return Table.Where(a =>  a.Id == Id).FirstOrDefault();
        }
        public  List<CourseReminder> GetAllRecord()
        {
            return Table.OrderByDescending(a => a.Id).ToList();
        }

        public IEnumerable<CourseReminder> GetPendingCourseList()
        {
            string status = "Pending";
            return DataContext.Database.SqlQuery<CourseReminder>("EXEC sstmo.Get_Dashbord_New_Course @p0", status);
            //return Table.Where(a => Convert.ToDateTime(a.renew_date).Subtract(DateTime.Today).TotalDays <=0).ToList();
        }
        public IEnumerable<CourseReminder> GetRenewalCourseList()
        {
            string status = "Renewal";
            return DataContext.Database.SqlQuery<CourseReminder>("EXEC sstmo.Get_Dashbord_New_Course @p0", status);
        }
        public CourseReminder GetRecordByLiId(long Id)
        {
            return Table.Where(a => a.li_course_id == Id).FirstOrDefault();
        }
        public long SaveRecord(CourseReminder entity)
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
        public bool isCourseNameExists(long Id, string courseName)
        {
            var entity = Table.Where(a => a.Id != Id && a.course_name == courseName).FirstOrDefault();

            if (entity != null)
                return true;
            else
                return false;
        }

        //public IEnumerable<CourseListModel> Get_CourseAndSubCourse(string CourseType, bool MasterCourse, long MainCourseId, long selectdSubid)
        //{
        //    return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.Get_CourseAndSubCourse @p0, @p1,@p2,@p3", CourseType, MasterCourse, MainCourseId, selectdSubid);
        //}
    }
}
