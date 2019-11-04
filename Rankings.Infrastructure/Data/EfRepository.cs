using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Rankings.Core.Interfaces;
using Rankings.Core.SharedKernel;

namespace Rankings.Infrastructure.Data
{
    public class EfRepository: IRepository
    {
        private readonly DbContext _dbContext;

        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById<T>(int id) where T : BaseEntity
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id == id);
        }

        public List<T> List<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>().ToList();
        }

        public List<T> List<T>(ISpecification<T> spec) where T : BaseEntity
        {
            return ApplySpecification<T>(spec).ToList();
        }

        public T Add<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public void Update<T>(T entity) where T : BaseEntity
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        private IQueryable<T> ApplySpecification<T>(ISpecification<T> spec) where T : BaseEntity
        {
            return EfSpecificationEvaluator<T, int>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}
