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

    public class CourseReminderTrackerService : RepositoryBase<NewCourseTrackingData>, ICourseReminderTrackerService
    {
        public CourseReminderTrackerService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public NewCourseTrackingModel GetNewCoursesTrackWithData(long CourseId,int flag)
        {
            return DataContext.Database.SqlQuery<NewCourseTrackingModel>("EXEC sstmo.NewCourseDocumentTrack @p0,@p1", CourseId, flag).FirstOrDefault();
        }

        public NewCourseTrackingData GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }

        public NewCourseTrackingData GetDocument(long Documentid)
        {
            return Table.AsNoTracking().Where(a => a.NewCourseId == Documentid).FirstOrDefault();
        }

        public long SaveRecord(NewCourseTrackingData entity)
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

        public void DeleteNewCourseRecord(long Id)
        {
            var entity = Table.Where(a => a.NewCourseId == Id).FirstOrDefault();
            if (entity != null)
                Delete(entity);
        }
       

    }
}
