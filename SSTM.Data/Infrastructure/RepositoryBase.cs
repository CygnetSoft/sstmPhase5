using EntityFramework.Triggers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SSTM.Data.Infrastructure
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private SSTMDbContext _dbContext;

        private readonly IDbSet<T> _entities;

        protected RepositoryBase(IRepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
            _entities = DataContext.Set<T>();
        }

        protected SSTMDbContext DataContext
        {
            get
            {
                return _dbContext ?? (_dbContext = RepositoryContext.Get());
            }
        }

        protected IRepositoryContext RepositoryContext
        {
            get;
            private set;
        }

        public virtual IQueryable<T> Table
        {
            get { return _entities; }
        }

        public void Add(T entity)
        {
            _entities.Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            _entities.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            var manager = ((IObjectContextAdapter)_dbContext).ObjectContext.ObjectStateManager;
            var entry = manager.GetObjectStateEntry(entity);
            //entry.RejectPropertyChanges("CreatedOn");
            //entry.RejectPropertyChanges("CreatedBy");
            //_dbContext.SaveChanges();
            _dbContext.SaveChangesWithTriggers();
        }

        public void Delete(T entity)
        {
            _entities.Remove(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            foreach (var item in _entities.Where(where))
            {
                _entities.Remove(item);
            }
            _dbContext.SaveChanges();
        }

        public T GetById(object id)
        {
            return _entities.Find(id);
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            return _entities.Find(where);
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.ToList();
        }

        public IEnumerable<T> GetAll(IQueryable<T> source, int pageIndex, int pageSize)
        {
            return source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<T> GetMany(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            return _entities.Where(where);
        }

        public string SqlQuery(string query)
        {
            return DataContext.SqlQuery<string>(query).FirstOrDefault();
        }

        public int SqlQueryInt(string query)
        {
            return DataContext.SqlQuery<int>(query).FirstOrDefault();
        }

        public long SqlQueryInt64(string query)
        {
            return DataContext.SqlQuery<long>(query).FirstOrDefault();
        }

        public object ExecuteSqlCommand(string query)
        {
            return DataContext.ExecuteSqlCommand(query);
        }
    }
}