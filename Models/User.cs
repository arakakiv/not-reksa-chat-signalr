using Microsoft.AspNetCore.SignalR;

namespace NotReksaChat.Models
{
    public class User
    {
        private Username Username { get; set; }

        public string Name 
        {
            get { return Username.Name; }
            private set { }
        }
        
        public bool IsAdm { get; set; }

        public HubCallerContext ContextCaller { get; }

        public User(string n, HubCallerContext context) {
            Username = new Username(n);
            ContextCaller = context;
        }

        public bool IsValid()
        {
            return Username.IsValid() && ValidContextCaller();
        }

        private bool ValidContextCaller()
        {
            return (ContextCaller is not null);
        }
    }
}