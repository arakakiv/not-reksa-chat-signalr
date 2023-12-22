namespace NotReksaChat.Models
{
    public class GeneralFunctions
    {
        internal static bool ContainsScript(string str) =>
            str.Contains("</") || (str.Contains("<") && str.Contains(">"));

        internal static string RemoveWhiteSpaces(string str)
        {
            str = str.Trim();
            while(str.Contains("  "))
                str = str.Replace("  ", " ");

            return str;
        }
    }
}