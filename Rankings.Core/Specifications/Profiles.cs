using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class Profiles : BaseSpecification<Profile>
    {
        public Profiles(string displayName): base(profile => profile.DisplayName == displayName)
        {
        }
    }
}