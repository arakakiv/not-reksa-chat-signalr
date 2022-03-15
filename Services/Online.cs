using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;

namespace NotReksaChat.Services
{
    public class Online
    {
        private static List<User> Users = new List<User>();
        public static List<string> banned = new List<string>();

        public void Add(User u)
        {
            Users.Add(u);
        }

        public List<User> GetAll()
        {
            return Users;
        }

        public void Update(User old, User newU)
        {
            if (newU is null)
            {
                Users.Remove(old);
                return;
            }
            
            Users.Remove(old);
            Users.Add(newU);
        }

        public User GetByContext(HubCallerContext c)
        {
            return Users.Find(x => x.Context == c);
        }

        public User GetByName(string n)
        {
            return Users.Find(x => x.Name.ToUpper() == n.ToUpper());
        }
    }
}