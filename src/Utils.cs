using System.Text.RegularExpressions;

namespace ReadyOrNotOpenMic
{
    public static class Utils
    {
        public static string AddSpaceToCamelCase(string txt)
        {
            return Regex.Replace(txt, "(\\B[A-Z])", " $1");
        }
    }
}
