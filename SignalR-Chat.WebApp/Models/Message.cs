namespace NotReksaChat.Models
{
    public class Message
    {
        public User Sender { get; set; }
        public string Text { get; set; }

        public Message(User sender, string msg)
        {
            Sender = sender;
            Text = GeneralFunctions.RemoveWhiteSpaces(msg);
        }

        public bool IsValid()
        {
            if (!GeneralFunctions.ContainsScript(Text))
                if (GeneralFunctions.RemoveWhiteSpaces(Text).Length <= 300)
                    return true;
            
            return false;
        }
    }
}