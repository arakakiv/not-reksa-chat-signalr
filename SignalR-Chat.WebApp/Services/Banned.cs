using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;

namespace NotReksaChat.Services
{
    public class Banned : IBanned
    {
        private readonly List<string> _banned = new List<string>();
        
        public void Add(string ip) =>
            _banned.Add(ip);

        public string Get(string ip) =>
            _banned.Find(x => x == ip);
    }
}