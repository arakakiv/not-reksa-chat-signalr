using AskerChat.Models;
using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Hubs
{
    public class ChatHub : Hub
    {
        public void NewUserEntered(string usr)
        {
            /* Security Tests */

            // If there is at least two users with the same context, this means that
            // the same user is triyng to invoke this method with this other name, 'usr'.

            // To prevent this, we just need to verify if there's someone with the Caller context
            // in online users.

            bool atLeastOne = Users.Online.Any(x => x.ContextCaller == Context);
            if (atLeastOne) { return; }

            /* Logical Tests */

            // Not allowing two users with the same name, sorry =)
            if (Users.Online.Any(x => x.Name == FormatName(usr))) {
                Clients.Caller.SendAsync("AlreadyOnline");
                return;
            }

            User u = new User(usr, Context);
            
            if (u.IsValid()) 
            {
                Users.Online.Add(u);
                Clients.All.SendAsync("NewUserEntered", u.Name);
            }
            else {
                Clients.Caller.SendAsync("NotAllowedName");
            }  
        }

        public async Task SendMessage(string usr, string msg)
        {
            User user;
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
            // * Als0, remember that we have sure the 'usr' is online, because we tested it before.

            /* If 'usr' is not online, do nothing */
            if (!Users.Online.Any(x => x.Name == FormatName(usr))) {
                return; 
            }
            else {
                user = Users.Online.Find(x => x.Name == FormatName(usr));
                m = new Message(user, msg);
            }

            /* If the contexts are different */
            if (user.ContextCaller != Context) { return; }

            /* Logical Tests */
            if(user.IsValid())
            {
                if (m.IsValid())
                {
                    if (m.IsCommand())
                    {
                        var command = "Cmd" + m.Command.ToString();

                        switch(m.Command) {
                            case Message.Commands.OnlineRequest:
                                var names = from u in Users.Online select u.Name;
                                await Clients.Caller.SendAsync(command, names.ToArray());
                                break;
                            case Message.Commands.ClearRequest:
                                await Clients.Caller.SendAsync(command);
                                break;
                            case Message.Commands.BanRequest:
                                await Clients.Caller.SendAsync(command, msg.Replace("/ban ", "").Trim());
                                break;
                            case Message.Commands.HelpRequest:
                                await Clients.Caller.SendAsync(command);
                                break;
                        }   
                    }
                    else
                    {
                        await Clients.All.SendAsync("ReceiveMsg", user.Name, m.Text);
                    }
                }
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            User u = Users.Online.Find(x => x.ContextCaller.ConnectionId == Context.ConnectionId);

            if (u is null) {
                return new Task(() => { });
            }

            Users.Online.Remove(u);
            return Clients.All.SendAsync("UserDisconnected", u.Name);;
        }

        public async Task BanResponse(string usr, string psswd)
        {
            /* If and only if the psswd passed is that we want */
            if (psswd == "!..364a5") {
                User toBan = Users.Online.Find(x => x.Name == FormatName(usr));

                if (toBan is null) {
                    await Clients.Caller.SendAsync("UserNotFounded", FormatName(usr));
                    return;
                }

                await Clients.Client(toBan.ContextCaller.ConnectionId).SendAsync("BanResponse");
                await Clients.All.SendAsync("SomeoneBanned", FormatName(usr));
            }
            else {
                await Clients.Caller.SendAsync("NotAuthorized");
            }
        }

        public string FormatName(string n)
        {
            n = n.Trim();
            while(n.Contains("  "))
            {
                n = n.Replace("  ", " ");
            }

            return n;
        }
    }
}