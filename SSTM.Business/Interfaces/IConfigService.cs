using SSTM.Core.Config;

namespace SSTM.Business.Interfaces
{
    public interface IConfigService
    {
        Config GetFirstRecord();

        Config GetRecordById(long Id);

        void SaveRecord(Config Entity);
    }
}