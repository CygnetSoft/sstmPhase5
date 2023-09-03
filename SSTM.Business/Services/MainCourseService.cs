using SSTM.Core.MainCourse;
using System.Linq;
using SSTM.Data.Infrastructure;
using System.Data.Entity;
using SSTM.Business.Interfaces;
using System.Collections.Generic;

namespace SSTM.Business.Services
{
   public class MainCourseService : RepositoryBase<MainCourse>, IMainCourseService
    {
        public MainCourseService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<MainCourse> GetMainCourseList(int isActive)
        {
            return DataContext.Database.SqlQuery<MainCourse>("EXEC sstmo.GetMainCourseList @p0", isActive);
        }

        public IEnumerable<MainCourse> GetMainStaffCourseList(int isActive)
        {
            return DataContext.Database.SqlQuery<MainCourse>("EXEC sstmo.GetMainStaffCourseList @p0", isActive);
        }
        public MainCourse GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }

      

        public long SaveRecord(MainCourse entity)
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

        public bool isCourseNameExists(long Id, string courseName,string CourseType)
        {
            var entity = Table.Where(a => !a.isDeleted && a.Id != Id && a.CourseName == courseName && a.CourseType== CourseType).FirstOrDefault();

            if (entity != null)
                return true;
            else
                return false;
        }
    }
}
