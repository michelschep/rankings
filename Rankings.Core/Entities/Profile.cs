using Rankings.Core.SharedKernel;

namespace Rankings.Core.Entities
{
    public class Profile: BaseEntity
    {
        public string EmailAddress { get; set; }

        public string DisplayName { get; set;  }

        public Profile(string emailAddress, string displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }
    }
    public class GameType: BaseEntity
    {
        public string Code { get; set;  }
        public string DisplayName { get; set;  }
    }
}