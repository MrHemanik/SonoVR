
namespace SonoGame
{
    internal class Credentials
    {
        public string Username { get; private set; }
        public AccountRole Role { get; private set; }

        private readonly string password;

        internal Credentials(string u, string p, AccountRole r)
        {
            Username = u;
            password = p;
            Role = r;
        }

        internal bool IsAdmin => (Role == AccountRole.Admin);

        internal bool IsSpecialAccount => (Role != AccountRole.User);

        internal bool ValidatePassword(string checkPassword)
        {
            return password == checkPassword;
        }

        internal string PrefsKey_TimeAccount { get => Username + "_TimeAccount"; }

        internal string PrefsKey_CampaignSelectedNodeID { get => Username + "_campaignMenuSelectedGraphNodeID"; }

    }
}
