using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotReksaChat.Models;
using NotReksaChat.Services;
using NotReksaChat.Settings;

namespace NotReksaChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AdminSettings _adminSettings;

        private readonly IOnline _onlineService;
        private readonly IBanned _bannerService;

        public ChatHub(IOnline online, IBanned banned, IOptions<AdminSettings> adminSettings)
        {
            _onlineService = online;
            _bannerService = banned;

            this._adminSettings = adminSettings.Value;
        }

        public async Task NewUserEntered(string user)
        {
            if (_bannerService.Get(GetIp(Context)) is not null)
            {
                await Clients.Caller.SendAsync("MessageFromUser", user, "alert('Você está banido por IP.'); ");
                return;
            }

            user = user.Trim();

            // We will not aprove two users with the same nick:
            if (_onlineService.GetByName(user) is not null)
            {
                await Clients.Caller.SendAsync("AlreadyOnline");
                return;
            }

            // Using devTools, someone may try to flood and invoke this
            if (_onlineService.GetByContext(Context) is not null) return;

            User u = new User(user, Context, GetIp(Context));
            if (u.IsValid())
            {
                _onlineService.Add(u);

                await Clients.All.SendAsync("NewUserEntered", u.Name);
                await Clients.Caller.SendAsync("ListOnlineUsers", (from x in _onlineService.GetAll() select x.Name).ToArray());              
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

            if (_onlineService.GetByName(who) is null)
            {
                return;
            }
            else
            {
                u = _onlineService.GetByName(who);
                m = new Message(u, msg);
            }

            // If the contexts are different, maybe there are someone trying to pass as another online user.
            if (u.Context != Context) return;

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
                if (u.LastMsg == default(DateTimeOffset))
                    u.LastMsg = DateTimeOffset.UtcNow;

                if (u.LastMsg > DateTimeOffset.UtcNow.AddMilliseconds(-700))
                {
                    await Clients.Caller.SendAsync("SpamAlert");
                    return;
                }
                else
                {
                    u.LastMsg = DateTimeOffset.UtcNow;

                    var notSendToTheseUsers = from user in _onlineService.GetAll() where user.Muted.Contains(u) select user.Context.ConnectionId;
                    await Clients.AllExcept(notSendToTheseUsers).SendAsync("ReceiveMessage", u.Name, m.Text);
                }
            }
        }

        public async Task BanByIpAddress(string psswd, string usr)
        {
            if (string.IsNullOrEmpty(psswd)) {
                await Clients.Caller.SendAsync("ReceiveMessage", "Vigia", "Senha incorreta.");
                return;
            }

            if (BCrypt.Net.BCrypt.Verify(psswd, this._adminSettings.BanUserPasswordHash))
            {
                var toBan = _onlineService.GetByName(usr);
                if (toBan is not null)
                {
                    var userRequesting = _onlineService.GetByContext(this.Context);
                    if (toBan.Name == userRequesting.Name)
                    {
                        await Clients.Caller.SendAsync("ReceiveMessage", "Vigia", "Você não pode se banir.");
                        return;
                    }

                    _bannerService.Add(toBan.IpAddress);

                    await Clients.Caller.SendAsync("ReceiveMessage", "Vigia", "Você baniu o usuário " + usr + " por IP.");

                    await Clients.Client(toBan.Context.ConnectionId).SendAsync("ReceiveMessage", "Vigia", "Você foi banido por IP.");
                    await Clients.Client(toBan.Context.ConnectionId).SendAsync("MessageFromUser", "Vigia", "connection.stop();");

                    return;
                }

                await Clients.Caller.SendAsync("ReceiveMessage", "Vigia", "O usuário não foi encontrado.");
                return;
            }
            
            await Clients.Caller.SendAsync("ReceiveMessage", "Vigia", "Senha incorreta.");
        }

        public void MuteUser(string toMute)
        {
            User userToMute = _onlineService.GetByName(toMute);
            User userWillMute = _onlineService.GetByContext(Context);

            userWillMute.Muted.Add(userToMute);
            _onlineService.Update(_onlineService.GetByContext(Context), userWillMute);
        }

        public void UnmuteUser(string toUnmute)
        {
            User userToUnmute = _onlineService.GetByName(toUnmute);
            User userWillUnmute = _onlineService.GetByContext(Context);

            userWillUnmute.Muted.Remove(userToUnmute);
            _onlineService.Update(_onlineService.GetByContext(Context), userWillUnmute);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            User u = _onlineService.GetByContext(Context);
            if (u is null) return;

            _onlineService.Update(u, null);
            await Clients.All.SendAsync("UserDisconnected", u.Name); ;
        }

        private string GetIp(HubCallerContext cont)
        {
            var forwardeds = Context.GetHttpContext().Request.Headers["X-Forwarded-For"];
            Console.WriteLine(forwardeds);
            return forwardeds;
        }
    }
}