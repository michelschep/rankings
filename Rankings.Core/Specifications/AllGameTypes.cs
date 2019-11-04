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

    public class Profiles : BaseSpecification<Profile>
    {
        public Profiles(string displayName): base(profile => profile.DisplayName == displayName)
        {
        }
    }
}