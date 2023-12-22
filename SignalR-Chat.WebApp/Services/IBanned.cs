using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;

namespace NotReksaChat.Services
{
    public interface IBanned
    {
        public void Add(string ip);
        public string Get(string ip);
    }
}