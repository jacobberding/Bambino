using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Api
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get();
        IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        void Insert(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected EDCContext context;
        protected DbSet<TEntity> dbSet;
        
        public GenericRepository(EDCContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        
        public IQueryable<TEntity> Get()
        {
            IQueryable<TEntity> query = dbSet;
            return query;
        }

        public virtual IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet.Where(predicate);
            query.ToString();
            return query;
            //return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }
        
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        
    }
}