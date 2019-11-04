using Ardalis.Specification;
using Rankings.Core.Entities;

namespace Rankings.Core.Specifications
{
    public class SpecificGameType: BaseSpecification<GameType>
    {
        public SpecificGameType(string code): base(p=>p.Code == code)
        {
        }
    }
}