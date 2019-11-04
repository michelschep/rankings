using Ardalis.Specification;

namespace Rankings.Core.SharedKernel
{
    public abstract class BaseEntity: IEntity<int>
    {
        public int Id { get; set; }
    }
}