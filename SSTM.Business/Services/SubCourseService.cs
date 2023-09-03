using SSTM.Business.Interfaces;
using SSTM.Core.SubCourse;
using SSTM.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class SubCourseService : RepositoryBase<SubCourse>, ISubCourseService
    {
        public SubCourseService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<SubCourse> GetSubCourseList(int isActive, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<SubCourse>("EXEC sstmo.GetSubCourseList @p0,@p1", isActive, MainCourseId);
        }

        public IEnumerable<SubCourse> GetSubStaffCourseList(int isActive, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<SubCourse>("EXEC sstmo.GetSubStaffCourseList @p0,@p1", isActive, MainCourseId);
        }
        public SubCourse GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }

        public IEnumerable<SubCourse> GetMainCourseList(string CourseType)
        {
            return Table.AsNoTracking().Where(a => a.CourseType == CourseType).ToList();
        }

        public IEnumerable<SubCourse> GetSubCourseList(string CourseType, long MainCourseId)
        {
            return Table.AsNoTracking().Where(a => a.CourseType == CourseType && a.MainCourseId == MainCourseId).ToList();
        }

        public long SaveRecord(SubCourse entity)
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

        public bool isCourseNameExists(long Id, string SubcourseName, string CourseType)
        {
            var entity = Table.Where(a => !a.isDeleted && a.Id != Id && a.SubCourseName == SubcourseName && a.CourseType == CourseType).FirstOrDefault();

            if (entity != null)
                return true;
            else
                return false;
        }
    }
}
