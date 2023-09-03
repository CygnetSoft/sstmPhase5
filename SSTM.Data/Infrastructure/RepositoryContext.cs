namespace SSTM.Data.Infrastructure
{
    public class RepositoryContext : IRepositoryContext
    {
        /// <summary>
        /// Private variable for DataContext
        /// </summary>
        private SSTMDbContext _dataContext;

        /// <summary>
        /// Getting DataContext object
        /// </summary>
        /// <returns></returns>
        public SSTMDbContext Get()
        {
            return _dataContext ?? (_dataContext = new SSTMDbContext());
        }
    }
}