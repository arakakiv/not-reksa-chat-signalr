using AskerChat.Models;
using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NewUserEntered(string usr)
        {
            try
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
                if (Users.Online.Any(x => x.Name == User.FormatName(usr))) {
                    await Clients.Caller.SendAsync("AlreadyOnline");
                    return;
                }

                User u = new User(usr, Context);
                
                if (u.IsValid()) 
                {
                    Users.Online.Add(u);
                    await Clients.All.SendAsync("NewUserEntered", u.Name);
                }
                else {
                    await Clients.Caller.SendAsync("NotAllowedName");
                }  
            }
            catch
            {
                await Clients.Caller.SendAsync("CouldNotUnderstand");
            }
        }

        public async Task SendMessage(string usr, string msg)
        {
            try
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
                if (!Users.Online.Any(x => x.Name == User.FormatName(usr))) {
                    return; 
                }
                else {
                    user = Users.Online.Find(x => x.Name == User.FormatName(usr));
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
                                    await Clients.Caller.SendAsync(command, User.FormatName(msg.Replace("/ban ", "").Trim()));
                                    break;
                                case Message.Commands.HelpRequest:
                                    await Clients.Caller.SendAsync(command);
                                    break;
                                case Message.Commands.PrivateMessageRequest:
                                    await PrivateMessageRequest(user.Name, m.Text, Context);
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
            catch
            {
                await Clients.Caller.SendAsync("CouldNotUnderstand");
            }
        }

        public async Task PrivateMessageRequest(string usrSender, string cmd, HubCallerContext context)
        {
            try 
            {
                string usrReceiver;
                string msg;

                msg = cmd.Substring(cmd.IndexOf("'", 0) + 1, cmd.Length - cmd.IndexOf("'", 0) - 2);

                usrReceiver = cmd.Replace("/msg ", "");
                usrReceiver = usrReceiver.Replace(msg, "");

                while(msg.Contains("  ")) { msg = msg.Replace("  ", " ").Trim(); }

                while(usrReceiver.Contains("'")) { usrReceiver = usrReceiver.Replace("'", "").Trim(); }
                while(usrReceiver.Contains("  ")) { usrReceiver = usrReceiver.Replace("  ", " ").Trim(); }

                User sender = Users.Online.Find(x => x.ContextCaller == Context);
                User receiver = Users.Online.Find(x => x.Name == User.FormatName(usrReceiver));

                if (sender is null || receiver is null) 
                {
                    await Clients.Client(sender.ContextCaller.ConnectionId).SendAsync("UserNotFounded", receiver.Name);
                    return;
                }
                else if (cmd.IndexOf("'", 0) == cmd.IndexOf("'", cmd.IndexOf("'") + 1))
                {
                    throw new Exception();
                }

                await Clients.Client(Context.ConnectionId).SendAsync("ReceivePrivateMsg", receiver.Name, msg, false);
                await Clients.Client(receiver.ContextCaller.ConnectionId).SendAsync("ReceivePrivateMsg", sender.Name, msg, true);
            }
            catch 
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CouldNotUnderstand");
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
            try
            {
                /* If and only if the psswd passed is that we want */
                if (psswd == "!..364a5") {
                    User toBan = Users.Online.Find(x => x.Name == User.FormatName(usr));

                    if (toBan is null) {
                        await Clients.Caller.SendAsync("UserNotFounded", User.FormatName(usr));
                        return;
                    }

                    await Clients.Client(toBan.ContextCaller.ConnectionId).SendAsync("BanResponse");
                    await Clients.All.SendAsync("SomeoneBanned", User.FormatName(usr));
                }
                else {
                    await Clients.Caller.SendAsync("NotAuthorized");
                }
            }
            catch
            {
                await Clients.Caller.SendAsync("CouldNotUnderstand");
            }
        }
    }
}