using Microsoft.AspNetCore.SignalR;

namespace NotReksaChat.Models
{
    public class User
    {
        public string Name { get; init; }
        public DateTimeOffset LastMsg { get; set; }
        
        public HubCallerContext Context { get; set; }

        public string IpAddress { get; set; }
        public List<User> Muted { get; set; } = new List<User>();

        public User(string name, HubCallerContext context, string ip)
        {
            Name = name.Trim();
            Context = context;
            IpAddress = ip;
        }

        public bool IsValid() =>
            NameValid();

        private bool NameValid()
        {
            bool validLength = Name.Length <= 20 && Name.Length > 1;
            bool validCharacters = !Name.Any(x => !Char.IsLetterOrDigit(x) && !Char.IsWhiteSpace(x));

            return validLength && 
            validCharacters && !GeneralFunctions.ContainsScript(Name);
        }
    }
}