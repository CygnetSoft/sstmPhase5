using SSTM.Core.TrainingCenter;
using SSTM.Models.JQueryDataTablesModel;
using SSTM.Models.TrainingCenter;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SSTM.Business.Interfaces
{
    public interface ITrainingCenterService
    {
        TrainingCenter GetRecordById(long? Id);

        IEnumerable<TrainingCenter> GetList(int isActive);

        IEnumerable<TrainingCenterDataGridModel> GetListForGrid(int isActive);

        IEnumerable<TrainingCenterDataGridModel> GetTrainingCentersGrid
            (int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount, out int searchRecordCount,
            string searchString, int isActive);

        void SaveRecord(TrainingCenter entity);

        void DeleteRecord(long Id);
    }
}