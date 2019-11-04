using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class AllProfiles : BaseSpecification<Profile>
    {
        public AllProfiles() : base(g => true)
        {
        }
    }
}