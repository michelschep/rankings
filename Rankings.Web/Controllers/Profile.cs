namespace Rankings.Web.Controllers
{
    public class Profile
    {
        public string EmailAddress { get; }

        public string DisplayName { get; }

        public Profile(string emailAddress, string displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }
    }
}