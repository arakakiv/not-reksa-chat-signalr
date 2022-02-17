using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Models
{
    public static class Users
    {
        public static List<User> Online { get; set; } = new List<User>();
    }
}