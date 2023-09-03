using SSTM.Business.Interfaces;
using SSTM.Core.TrainingCenter;
using SSTM.Data.Infrastructure;
using SSTM.Helpers.Common;
using SSTM.Models.JQueryDataTablesModel;
using SSTM.Models.TrainingCenter;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace SSTM.Business.Services
{
    public class TrainingCenterService : RepositoryBase<TrainingCenter>, ITrainingCenterService
    {
        public TrainingCenterService(IRepositoryContext repositoryContext) : base(repositoryContext) { }

        public TrainingCenter GetRecordById(long? Id)
        {
            return Table.AsNoTracking().Where(a => a.Id == Id).FirstOrDefault();
        }

        public IEnumerable<TrainingCenter> GetList(int isActive)
        {
            if (isActive == 0)
                return Table.Where(a => !a.isDeleted && !a.isActive);
            else if (isActive == 1)
                return Table.Where(a => !a.isDeleted && a.isActive);
            else
                return Table.Where(a => !a.isDeleted);
        }

        public IEnumerable<TrainingCenterDataGridModel> GetListForGrid(int isActive)
        {
            return DataContext.SqlQuery<TrainingCenterDataGridModel>("EXEC GetTrainingCentersList @p0", isActive);
        }

        public IEnumerable<TrainingCenterDataGridModel> GetTrainingCentersGrid
            (int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount,
            string searchString, int isActive)
        {
            var dataList = GetListForGrid(isActive).ToList();

            totalRecordCount = dataList.Count();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                dataList = dataList
                    .Where(a => a.Name.ToLower().Contains(searchString.ToLower()) ||
                    a.PhysicalAddress.ToString().ToLower().Contains(searchString.ToLower()) || a.PostalCode.ToLower().Contains(searchString.ToLower())).ToList();
            }

            searchRecordCount = dataList.Count();

            IOrderedEnumerable<TrainingCenterDataGridModel> sortedData = null;

            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "Name":
                        sortedData = sortedData == null ? dataList.CustomSort(sortedColumn.Direction, h => h.Name)
                            : sortedData.CustomSort(sortedColumn.Direction, h => h.Name);
                        break;

                    case "PhysicalAddress":
                        sortedData = sortedData == null ? dataList.CustomSort(sortedColumn.Direction, h => h.PhysicalAddress)
                            : sortedData.CustomSort(sortedColumn.Direction, h => h.PhysicalAddress);
                        break;

                    case "PostalCode":
                        sortedData = sortedData == null ? dataList.CustomSort(sortedColumn.Direction, h => h.PostalCode)
                            : sortedData.CustomSort(sortedColumn.Direction, h => h.PostalCode);
                        break;

                    case "Status":
                        sortedData = sortedData == null ? dataList.CustomSort(sortedColumn.Direction, h => h.Status)
                            : sortedData.CustomSort(sortedColumn.Direction, h => h.Status);
                        break;
                }
            }

            if (pageSize == -1)
                return sortedData.Skip(startIndex).ToList();
            else
                return sortedData.Skip(startIndex).Take(pageSize).ToList();
        }

        public void SaveRecord(TrainingCenter entity)
        {
            if (entity.Id > 0)
                Update(entity);
            else
                Add(entity);
        }

        public void DeleteRecord(long Id)
        {
            var entity = Table.Where(a => a.Id == Id).FirstOrDefault();

            if (entity != null)
                Delete(entity);
        }
    }
}