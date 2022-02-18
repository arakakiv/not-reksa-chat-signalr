using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Models
{
    public class User
    {
        public string Name { get; set; }

        public HubCallerContext ContextCaller { get; }

        public User(string n, HubCallerContext context) {
            Name = n.Trim();
            
            while(Name.Contains("  "))
            {
                Name = Name.Replace("  ", " ");
            }

            ContextCaller = context;
        }

        public bool IsValid()
        {
            return ValidName() && ValidContextCaller();
        }

        private bool ValidName()
        {
            // Protect from XSS or empty name
            if (Name.ToUpper() == "VIGIA" || String.IsNullOrEmpty(Name) ||  (Name.Contains("<") && Name.Contains(">")) || Name.Contains("</"))
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