using Microsoft.AspNetCore.SignalR;

namespace NotReksaChat.Models
{
    public class Message
    {
        public enum Commands
        {
            OnlineRequest,
            ClearRequest,
            BanRequest,
            HelpRequest,
            PrivateMessageRequest
        }

        public User User { get; }

        public string Text { get; }

        public Commands? Command { get; private set; }

        public Message(User u, string txt)
        {
            User = u;
            Text = txt.Trim();

            SetCommand();
        }

        public bool IsValid()
        {
            // Protect from XSS or empty msg
            if ((Text.Contains("<") && Text.Contains(">")) || Text.Contains("/>") || Text.Trim() == "")
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
            if (Text == "/online") {
                Command = Commands.OnlineRequest;
            }
            else if (Text == "/clear") {
                Command = Commands.ClearRequest;
            }
            else if (Text.Contains("/ban ")) {                
                Command = Commands.BanRequest;
            }
            else if (Text == "/help") {
                Command = Commands.HelpRequest;
            }
            else if (Text.Contains("/msg ")) {
                Command = Commands.PrivateMessageRequest;
            }
            else {
                Command = null;
            }
        }
    }
}