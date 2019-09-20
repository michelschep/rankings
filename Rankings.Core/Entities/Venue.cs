using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Venue : BaseEntity
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }
}