using SSTM.Core.TrainnerMacAddress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Business.Interfaces
{
    public interface ITrainnerMacAddressService
    {
        IEnumerable<TrainnerMacAddress> GetAllMacAddress();
        TrainnerMacAddress GetRecordById(long Id);
        long SaveRecord(TrainnerMacAddress entity);
        void DeleteRecord(long Id);
    }
}
