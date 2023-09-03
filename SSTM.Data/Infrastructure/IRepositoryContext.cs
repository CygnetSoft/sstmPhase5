namespace SSTM.Data.Infrastructure
{
    public interface IRepositoryContext
    {
        SSTMDbContext Get();
    }
}