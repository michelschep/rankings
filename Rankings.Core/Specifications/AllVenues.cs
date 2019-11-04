using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class AllVenues : BaseSpecification<Venue>
    {
        public AllVenues() : base(g => true)
        {
        }
    }
}