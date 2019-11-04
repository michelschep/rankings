using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class AllGameTypes: BaseSpecification<GameType>
    {
        public AllGameTypes(): base(p=>true)
        {
        }
    }
}