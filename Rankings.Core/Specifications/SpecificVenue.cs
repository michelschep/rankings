using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class SpecificVenue : BaseSpecification<Venue>
    {
        public SpecificVenue(string code) : base(g => g.Code == code)
        {
        }

        public SpecificVenue(int id) : base(g => g.Id == id)
        {
        }
    }
}