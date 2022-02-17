using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Models
{
    public class User
    {
        public string Name { get; }
        public HubCallerContext ContextCaller { get; }

        public User(string n, HubCallerContext context) {
            Name = n.Trim();
            ContextCaller = context;
        }

        public bool IsValid()
        {
            return ValidName() && ValidContextCaller();
        }

        private bool ValidName()
        {
            // Trying to protect from XSS or empty nam 

            if (Name.ToUpper() == "VIGIA" || String.IsNullOrEmpty(Name.Trim()) ||  Name.Contains("<") || Name.Contains(">"))
            {
                return false;
            }
            
            return true;
        }

        private bool ValidContextCaller()
        {
            return (ContextCaller is not null);
        }
    }
}