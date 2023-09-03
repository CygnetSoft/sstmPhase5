using SSTM.Business.Interfaces;
using SSTM.Core.TrainerUploadDocument;
using SSTM.Data.Infrastructure;
using SSTM.Models.TrainerUploadDocumentModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Services
{
    public class TrainerUploadDocumentService : RepositoryBase<TrainerUploadDocument>, ITrainerUploadDocumentService
    {
        public TrainerUploadDocumentService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        //public IEnumerable<CourseListModel> GetList(int isActive, long statusId, bool MasterCourse, long MainCourseId)
        //{
        //    return DataContext.Database.SqlQuery<CourseListModel>("EXEC sstmo.GetCoursesList @p0, @p1,@p2,@p3", isActive, statusId, MasterCourse, MainCourseId);
        //}
        public IEnumerable<TrainerUploadDataModel> GetUploadDocsList(int status)
        {
            return DataContext.Database.SqlQuery<TrainerUploadDataModel>("EXEC sstmo.Get_TrainerUploadDocument @p0", status);
           // return Table.Where(a => !a.isDeleted && a.Status== status).OrderByDescending(a => a.Id);
        }
        public IEnumerable<TrainerUploadDocument> GetCommonUploadDocsList(int status, bool? MasterDoc, long? MasterDocId)
        {
            //return DataContext.Database.SqlQuery<TrainerUploadDataModel>("EXEC sstmo.Get_TrainerUploadDocument @p0", status);
            return Table.Where(a => !a.isDeleted && a.Status== status && a.MasterDoc== MasterDoc && a.MasterDocId== MasterDocId).OrderByDescending(a => a.Id);
        }
        //public IEnumerable<TrainerUploadDocument> GetVideoList()
        //{
        //    return Table.Where(a => !a.isDeleted && a.Status == 1).OrderByDescending(a => a.Id);
        //}
        //public IEnumerable<TrainerUploadDocument> GetCommonDocList()
        //{
        //    return Table.Where(a => !a.isDeleted && a.Status == 2).OrderByDescending(a => a.Id);
        //}
        public TrainerUploadDocument GetRecordById(long Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }
        public long SaveRecord(TrainerUploadDocument entity)
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
