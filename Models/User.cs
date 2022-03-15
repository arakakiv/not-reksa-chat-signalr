using Microsoft.AspNetCore.SignalR;

namespace NotReksaChat.Models
{
    public class User
    {
        public string Name { get; init; }
        public List<User> Muted { get; set; } = new List<User>();
        public DateTime LastMsg { get; set; }
        public HubCallerContext Context { get; set; }
        public string IpAddress { get; set; }

        public User(string name, HubCallerContext context, string ip)
        {
            Name = name.Trim();
            Context = context;
            IpAddress = ip;
        }

        public bool IsValid()
        {
            return NameValid();
        }

        private bool NameValid()
        {
            bool validLength = Name.Length <= 20 && Name.Length > 1;
            
            bool validCharacters = true;
            foreach(char c in Name)
            {
                if (!Char.IsLetterOrDigit(c) && !Char.IsWhiteSpace(c))
                    return false;
            }

            if (validLength && validCharacters && !GeneralFunctions.ContainsScript(Name))
                return true;
            else
                return false;
        }
    }
}