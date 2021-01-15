using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Profile: BaseEntity
    {
        public string Identifier { get; set;  }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set;  }
        public bool IsActive { get; set; }
    }
}