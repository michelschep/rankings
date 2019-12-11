using System;
using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class SpecificProfile: BaseSpecification<Profile>
    {
        public SpecificProfile(string emailAddress): base(p=>p.EmailAddress.ToLower() == emailAddress.ToLower())
        {
        }

        public SpecificProfile(int id): base(p=>p.Id == id)
        {
        }
    }
}