using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Models
{
    public class Message
    {
        public enum Commands
        {
            OnlineRequest,
            ClearRequest,
            BanRequest
        }

        public User User { get; }

        public string Text { get; }
        public Commands? Command { get; private set; }

        public Message(User u, string txt)
        {
            // Hello!
            
            User = u;
            Text = txt;

            SetCommand();
        }

        public bool IsValid()
        {
            if (Text.Contains("<") || Text.Contains(">") || Text.Trim() == "")
            {
                return false;
            }

            return true;
        }

        public bool IsCommand() 
        {
            return (Command != null);
        }

        private void SetCommand()
        {
            if (Text.Trim() == "/online") {
                Command = Commands.OnlineRequest;
            }
            else if (Text.Trim() == "/clear") {
                Command = Commands.ClearRequest;
            }
            else if (Text.Trim().Contains("/ban ")) {                
                Command = Commands.BanRequest;
            }
            else {
                Command = null;
            }
        }
    }
}