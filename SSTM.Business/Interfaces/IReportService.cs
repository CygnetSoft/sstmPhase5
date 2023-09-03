using SSTM.Core.Config;

namespace SSTM.Business.Interfaces
{
    public interface IReportService
    {
        Report GetFirstRecord();

        Report GetRecordById(long Id);

        void SaveRecord(Report Entity);
    }
}