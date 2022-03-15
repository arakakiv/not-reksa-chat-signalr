namespace NotReksaChat.Models
{
    public class GeneralFunctions
    {
        internal static bool ContainsScript(string what)
        {
            if (what.Contains("</") || (what.Contains("<") && what.Contains(">")))
                return true;
            
            return false;
        }

        internal static string RemoveWhiteSpaces(string what)
        {
            what = what.Trim();
            
            while(what.Contains("  "))
                what = what.Replace("  ", " ");

            return what;
        }
    }
}