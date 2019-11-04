using System;
using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class Oponents: BaseSpecification<Profile>
    {
        public Oponents(string emailAddress): base(p=>!string.Equals(p.EmailAddress, emailAddress, StringComparison.CurrentCultureIgnoreCase))
        {
        }
    }
}