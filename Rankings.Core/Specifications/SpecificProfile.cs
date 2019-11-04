using System;
using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class SpecificProfile: BaseSpecification<Profile>
    {
        public SpecificProfile(string emailAddress): base(p=>string.Equals(p.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase))
        {
        }
    }
}