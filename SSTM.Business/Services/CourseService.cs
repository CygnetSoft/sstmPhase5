using SSTM.Business.Interfaces;
using SSTM.Core.Course;
using SSTM.Data.Infrastructure;
using SSTM.Models.Course;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{
    public class CourseService : RepositoryBase<Course>, ICourseService
    {
        public CourseService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<CourseListModel> GetList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        }

        public IEnumerable<CourseListModel> GetNewList(int isActive, long statusId, bool MasterCourse, long MainCourseId, long NewCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetNewCoursesList @p0, @p1,@p2,@p3,@p4", isActive, statusId, MasterCourse, MainCourseId, NewCourseId);
        }
        public IEnumerable<CourseListModel> Get_Iso_Edu_CoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.Get_Iso_Edu_CoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        }
        public IEnumerable<CourseListModel> GetList_All(int isActive, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesList @p0, @p1,@p2", isActive, MasterCourse, MainCourseId);
        }
        public IEnumerable<CourseListModel> GetStaffSubCoursesList(int isActive, long statusId, long CurrentuserLogin, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetStaffSubCoursesList @p0, @p1,@p2,@p3,@p4", isActive, statusId, CurrentuserLogin, MasterCourse, MainCourseId);
        }

        public IEnumerable<DashbordCourseCountModel> Get_Dashboard_data()
        {
            return DataContext.Database.SqlQuery<DashbordCourseCountModel>("EXEC sstmo.GetDashbord_chart");
        }
        public IEnumerable<CourseListModel> GetStaffList(int isActive, long statusId, long CurrentuserLogin)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetStaffCoursesList @p0, @p1,@p2", isActive, statusId, CurrentuserLogin);
        }
        public IEnumerable<CourseListModel> GetComonCoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetComonCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        }

        public IEnumerable<CourseListModel> GetComonNewCoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId, long NewCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetNewComonCoursesList @p0, @p1,@p2,@p3,@p4", isActive, statusId, MasterCourse, MainCourseId, NewCourseId);
        }


        public IEnumerable<CourseListModel> Get_Iso_Edu_ComonCoursesList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.Get_Iso_Edu_ComonCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        }
        public IEnumerable<CourseListModel> GetComonCoursesList_All(int isActive,  bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetComonCoursesList_All @p0, @p1,@p2", isActive, MasterCourse, MainCourseId);
        }
        public IEnumerable<CourseListModel> Get_Iso_Edu_ComonCoursesList_All(int isActive, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.Get_Iso_Edu_ComonCoursesList_All @p0, @p1,@p2", isActive, MasterCourse, MainCourseId);
        }
        public IEnumerable<CourseListModel> GetCoursesWithStatus(int isActive, long statusId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesWithStatus @p0, @p1", isActive, statusId);
        }

        public IEnumerable<CourseListModel> GetCoursesWithotStatus(int isActive, long statusId)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesWithoutStatus @p0, @p1", isActive, statusId);
        }

        public Course GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }
        public List<Course> GetAllRecord()
        {
            return Table.AsNoTracking().OrderByDescending(a => a.Id).Where(a=>a.CourseType== "other").ToList();
        }
        public Course GetRecordNewcourseById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.NewCourseId == Id).FirstOrDefault();
        }
        public IEnumerable<Course> GetMainCourseToCourseList(string CourseType, long MainCourseId)
        {
            return Table.AsNoTracking().Where(a => a.MasterCoursId == MainCourseId && a.CourseType == CourseType).ToList();
        }
        public Course GetMainCourseToCourseRecordById(long MainCourseId)
        {
            return Table.AsNoTracking().Where(a => !a.isDeleted && a.MasterCoursId == MainCourseId).FirstOrDefault();
        }

        public long SaveRecord(Course entity)
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

        public bool isCourseNameExists(long Id, string courseName, string CourseType)
        {
            var entity = Table.Where(a => !a.isDeleted && a.Id != Id && a.CourseName == courseName && a.CourseType == CourseType).FirstOrDefault();

            if (entity != null)
                return true;
            else
                return false;
        }

        public IEnumerable<CourseListModel> Get_CourseAndSubCourse(string CourseType, bool MasterCourse, long MainCourseId,long selectdSubid)
        {
            return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.Get_CourseAndSubCourse @p0, @p1,@p2,@p3", CourseType, MasterCourse, MainCourseId, selectdSubid);
        }

        //public long SubFolderCount(long? MasterCoursId, string CourseType)
        //{
        //    var entity = Table.Where(a => !a.isDeleted && a.CourseType == CourseType && a.Masterid == MasterCoursId).ToList();
        //    return entity.ToList().Count();
        //}

        //public long GetMasterid(long? MasterCoursId, string CourseType)
        //{
        //    var entity = Table.Where(a => !a.isDeleted && a.CourseType == CourseType && a.Id == MasterCoursId).FirstOrDefault();
        //    return long.Parse(entity.Masterid.ToString());
        //}
    }
}