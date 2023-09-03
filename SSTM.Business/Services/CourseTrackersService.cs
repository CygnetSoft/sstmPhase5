using SSTM.Business.Interfaces;
using SSTM.Core.CourseTrackers;
using SSTM.Data.Infrastructure;
using SSTM.Models.CourseTrackers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{  
    public class CourseTrackersService : RepositoryBase<CourseTrackers>, ICourseTrackersService
    {
        public CourseTrackersService(IRepositoryContext repositoryContext) : base(repositoryContext) { }


        public CourseTrackingDataModel GetCoursesTrackWithData(long CourseId)
        {
            return DataContext.Database.SqlQuery<CourseTrackingDataModel>("EXEC sstmo.DocumentTrack @p0", CourseId).FirstOrDefault();
        }

        public NewCourseTrackingDataModel GetNewCoursesTrackWithData(long CourseId)
        {
            return DataContext.Database.SqlQuery<NewCourseTrackingDataModel>("EXEC sstmo.NewCourseDocumentTrack @p0", CourseId).FirstOrDefault();
        }

        public CourseTrackers GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }
        
        public CourseTrackers GetDocument(long Documentid)
        {
            return Table.AsNoTracking().Where(a => a.Courseid == Documentid).FirstOrDefault();
        }

        public long SaveRecord(CourseTrackers entity)
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
