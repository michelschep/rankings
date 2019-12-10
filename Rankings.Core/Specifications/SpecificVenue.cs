using System;
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

    public class NamedVenue : BaseSpecification<Venue>
    {
        public NamedVenue(string name) : base(g => g.DisplayName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        {
        }
    }
}