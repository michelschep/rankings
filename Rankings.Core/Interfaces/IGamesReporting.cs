using System.Collections.Generic;
using Ardalis.Specification;
using Rankings.Core.SharedKernel;

namespace Rankings.Core.Interfaces
{
    public interface IGamesReporting
    {
        IEnumerable<T> List<T>(ISpecification<T> specification) where T : BaseEntity;
        T Item<T>(ISpecification<T> specification) where T : BaseEntity;
    }
}