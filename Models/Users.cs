using Microsoft.AspNetCore.SignalR;

namespace AskerChat.Models
{
    public static class Users
    {
        internal static List<User> Online { get; set; } = new List<User>();

        internal static string FormatName(string n)
        {
            n = n.Trim();
            while(n.Contains("  "))
            {
                n = n.Replace("  ", " ");
            }

            while(n.Contains("'")) 
            {
                n = n.Replace("'", "");
            }

            return n;
        }

        internal static bool UserOnline(string usr)
        {
            User online = Users.Online.Find(x => x.Name.ToUpper() == FormatName(usr.ToUpper()));
            if (online is null)
            {
                return false;
            }

            return true;
        }
    
        internal static User GetByName(string usr)
        {
            if (UserOnline(usr))
            {
                return Users.Online.Find(x => x.Name == FormatName(usr));
            }

            return null;
        }

        internal static User GetByContext(HubCallerContext context)
        {
            return Users.Online.Find(x => x.ContextCaller == context);
        }
    }
}