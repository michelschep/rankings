namespace Rankings.Web.Models
{
    public class ProfileViewModel
    {
        public string EmailAddress { get; }
        public string DisplayName { get; }

        public ProfileViewModel(string emailAddress, string displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }
    }
}