using Microsoft.AspNetCore.SignalR;

namespace NotReksaChat.Models
{
    public class Username
    {
        public string Name { get; set; }

        public Username(string n)
        {
            Name = Users.FormatName(n);
        }

        public bool IsValid()
        {
            return IsNotReserved() && OnlyValidCharacters() && ValidLength();
        }

        private bool IsNotReserved()
        {
            if (Name.ToUpper() == "VIGIA" || Name.ToUpper() == "CMD" || String.IsNullOrEmpty(Name)) 
                return false;

            return true;
        }

        private bool OnlyValidCharacters()
        {
            foreach(char c in Name)
            {
                if (!Char.IsLetterOrDigit(c) && !Char.IsWhiteSpace(c))
                    return false;
            }

            return true;
        }

        private bool ValidLength()
        {
            if (Name.Length < 20)
                return true;

            return false;
        }
    }
}