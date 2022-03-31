using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;

namespace NotReksaChat.Services
{
    public interface IOnline
    {
        public void Add(User u);

        public List<User> GetAll();

        public void Update(User old, User newU);

        public User GetByContext(HubCallerContext c);

        public User GetByName(string n);
    }
}