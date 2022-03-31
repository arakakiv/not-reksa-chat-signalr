using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;

namespace NotReksaChat.Services
{
    public class Banned : IBanned
    {
        public readonly List<string> banned = new List<string>();
        
        public void Add(string ip)
        {
            banned.Add(ip);
        }

        public string Get(string ip)
        {
            return banned.Find(x => x == ip);
        }
    }
}