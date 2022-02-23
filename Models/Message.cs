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
            PrivateMessageRequest,
            MuteRequest,
            UnmuteRequest
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
            if (HitsCommand("/online")) {
                Command = Commands.OnlineRequest;
            }
            else if (HitsCommand("/clear")) {
                Command = Commands.ClearRequest;
            }
            else if (HitsCommand("/ban ")) {                
                Command = Commands.BanRequest;
            }
            else if (HitsCommand("/help")) {
                Command = Commands.HelpRequest;
            }
            else if (HitsCommand("/msg ")) {
                Command = Commands.PrivateMessageRequest;
            }
            else if (HitsCommand("/mute ")) {
                Command = Commands.MuteRequest;
            }
            else if (HitsCommand("/unmute ")) {
                Command = Commands.UnmuteRequest;
            }
            else {
                Command = null;
            }
        }

        private bool HitsCommand(string whichCmd)
        {
            // Index and length must refer to a location within the string. (Parameter 'length')
            if (whichCmd.Length > Text.Length)
                return false;

            return Text.Substring(0, whichCmd.Length) == whichCmd;
        }
    }
}