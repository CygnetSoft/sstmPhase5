using SSTM.Business.Interfaces;
using SSTM.Core.CourseDownload;
using SSTM.Data.Infrastructure;
using SSTM.Models.CourseDownload;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class CourseDownloadUserService : RepositoryBase<CourseDownloadUser>, ICourseDownloadUserService
    {
        public CourseDownloadUserService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<CourseDownloadUserDetailsModel> GetDownloadList(long UserId, bool MasterCourse, long MainCourseId)
        {
            return DataContext.Database.SqlQuery<CourseDownloadUserDetailsModel>("EXEC sstmo.GetCourseDownloadUserList @p0, @p1,@p2", UserId, MasterCourse, MainCourseId);
        }

        public CourseDownloadUser GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }
      
        public long SaveRecord(CourseDownloadUser entity)
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

        public bool isCourseNameExists( long CourseId, long Userid)
        {
            var entity = Table.Where(a => a.CourseId == CourseId && a.User_id == Userid).FirstOrDefault();

            if (entity != null)
                return true;
            else
                return false;
        }
    }
}
