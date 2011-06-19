// This is needed for extension methods to work
namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}

namespace NetduinoPlusWebServer
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Replace characters in a string
        /// </summary>
        /// <param name="stringToSearch"></param>
        /// <param name="charToFind"></param>
        /// <param name="charToSubstitute"></param>
        /// <returns></returns>
        public static string Replace(this string stringToSearch, char charToFind, char charToSubstitute)
        {
            // Surely there must be nicer way than this?
            char[] chars = stringToSearch.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] == charToFind) chars[i] = charToSubstitute;

            return new string(chars);
        }

    }
}
