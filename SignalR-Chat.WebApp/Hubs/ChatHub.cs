using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using NotReksaChat.Models;
using NotReksaChat.Services;

namespace NotReksaChat.Hubs
{
    public class ChatHub : Hub
    {
        public IOnline Online { get; }
        public IBanned Banned { get; }
        public ChatHub(IOnline online, IBanned banned)
        {
            Online = online;
            Banned = banned;
        }

        public string GetIp(HubCallerContext cont)
        {
            var forwardeds = Context.GetHttpContext().Request.Headers["X-Forwarded-For"];
            return forwardeds;
        }

        public async Task NewUserEntered(string user)
        {
            if (Banned.Get(GetIp(Context)) is not null)
            {
                await Clients.Caller.SendAsync("MessageFromUser", user, "alert('Você está banido por IP.'); ");
                return;
            }

            user = user.Trim();

            // We will not aprove two users with the same nick:
            if (Online.GetByName(user) is not null)
            {
                await Clients.Caller.SendAsync("AlreadyOnline");
                return;
            }

            // Using devTools, someone may try to flood and invoke this
            if (Online.GetByContext(Context) is not null)
                return;

            User u = new User(user, Context, GetIp(Context));
            if (u.IsValid())
            {
                Online.Add(u);

                await Clients.All.SendAsync("NewUserEntered", u.Name);
                await Clients.Caller.SendAsync("ListOnlineUsers", (from x in Online.GetAll() select x.Name).ToArray());              
            }
            else
            {
                await Clients.Caller.SendAsync("InvalidUser");
            }
        }

        public async Task SendMsg(string who, string msg)
        {
            User u;
            Message m;

            /* Security tests */

            // Maybe, someone try to request this method with diferent names, using the console
            // from devTools. Then, we need to check if the people's name who send the request
            // equals to the finded User, 'user'. BUT, our application is made in that way:
            // When the user connects to the chat, the input for name isn't enable.
            // This makes hard to send a msg, so we create a variable named 'user' with the input value,
            // and send requests in function of this value.

            // But the value, by devTools can be changed. So, we don't make sure if the value was or not
            // changed.

            // Then, how could we know if the user that is sending the request really has the name of 'usr'?

            // 1. Well, the user is online? It exists? If not, we can just return and do nothing.

            // 2. But maybe, our caller user is trying to pass as someone that is online.
            // Then, we need to make sure if the caller's Context equals to the 'someone' context.
            // * Also, remember that we have sure the 'usr' is online, because we tested it before.

            if (Online.GetByName(who) is null)
            {
                return;
            }
            else
            {
                u = Online.GetByName(who);
                m = new Message(u, msg);
            }

            // If the contexts are different, maybe there are someone trying to pass as another online user.
            if (u.Context != Context)
                return;

            if (m.Text.Contains("/ban "))
            {
                int n1 = m.Text.IndexOf("'");
                int n2 = m.Text.LastIndexOf("'");

                var user = m.Text.Substring(n1 + 1, m.Text.Length - n1 - 2);
                
                await Clients.Caller.SendAsync("RequestPsswd", user);
                return;
            }

            if (m.IsValid())
            {
                if (u.LastMsg == default(DateTime))
                    u.LastMsg = DateTime.MinValue;

                if (u.LastMsg > DateTime.Now.AddMilliseconds(-700))
                {
                    await Clients.Caller.SendAsync("SpamAlert");
                    return;
                }
                else
                {
                    u.LastMsg = DateTime.Now;

                    var notSendToTheseUsers = from user in Online.GetAll() where user.Muted.Contains(u) select user.Context.ConnectionId;
                    await Clients.AllExcept(notSendToTheseUsers).SendAsync("ReceiveMessage", u.Name, m.Text);
                }
            }
        }

        public async Task FuncAmig(string psswd, string usr)
        {
            if (psswd == "423!!u2")
            {
                var toBan = Online.GetByName(usr);
                if (toBan is not null)
                {
                    Banned.Add(toBan.IpAddress);
                    await Clients.Client(toBan.Context.ConnectionId).SendAsync("MessageFromUser", "Vigia", "connection.stop();");
                }
            }
        }

        public void MuteUser(string toMute)
        {
            User userToMute = Online.GetByName(toMute);
            User userWillMute = Online.GetByContext(Context);

            userWillMute.Muted.Add(userToMute);
            Online.Update(Online.GetByContext(Context), userWillMute);
        }

        public void UnmuteUser(string toUnmute)
        {
            User userToUnmute = Online.GetByName(toUnmute);
            User userWillUnmute = Online.GetByContext(Context);

            userWillUnmute.Muted.Remove(userToUnmute);
            Online.Update(Online.GetByContext(Context), userWillUnmute);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            User u = Online.GetByContext(Context);

            if (u is null)
            {
                return new Task(() => { });
            }

            Online.Update(u, null);
            return Clients.All.SendAsync("UserDisconnected", u.Name); ;
        }
    }
}