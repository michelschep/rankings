using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Profile: BaseEntity
    {
        public string EmailAddress { get; set; }

        public string DisplayName { get; set;  }
    }
}