using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;

namespace NotReksaChat.Services
{
    public class Online : IOnline
    {
        private readonly List<User> _users = new List<User>();

        public void Add(User u) =>
            _users.Add(u);

        public List<User> GetAll() =>
            _users;

        public void Update(User old, User newU)
        {
            if (newU is null)
            {
                _users.Remove(old);
                return;
            }
            
            _users.Remove(old);
            _users.Add(newU);
        }

        public User GetByContext(HubCallerContext c) =>
            _users.Find(x => x.Context == c);

        public User GetByName(string n) =>
            _users.Find(x => x.Name.ToUpper() == n.ToUpper());
    }
}