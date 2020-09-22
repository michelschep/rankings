using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class NamedVenue : BaseSpecification<Venue>
    {
        public NamedVenue(string name) : base(g => g.DisplayName.ToLower() == name.ToLower())
        {
        }
    }
}