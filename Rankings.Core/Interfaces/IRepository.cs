using System.Collections.Generic;
using Ardalis.Specification;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Interfaces
{
    public interface IRepository
    {
        T GetById<T>(int id) where T : BaseEntity;
        List<T> List<T>() where T : BaseEntity;
        List<T> List<T>(ISpecification<T> spec) where T : BaseEntity;
        // ReSharper disable once UnusedMethodReturnValue.Global
        T Add<T>(T entity) where T : BaseEntity;
        void Update<T>(T entity) where T : BaseEntity;
        void Delete<T>(T entity) where T : BaseEntity;
    }
}