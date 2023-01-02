using System.Linq;

namespace System
{
    internal static class Extensions
    {
        public static class ConsoleEx
        {
            internal static void WriteColoured(ConsoleColor color, string text)
            {
                Console.ForegroundColor = color;
                Console.Out.Write(text);
                Console.ForegroundColor = ConsoleColor.White;
            }

            internal static void WriteColouredLine(ConsoleColor color, string text)
            {
                Console.ForegroundColor = color;
                Console.Out.WriteLine(text);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        internal static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }

        internal static bool StringContainsIn(this string inputStr, params string[] containsList)
        {
            if (containsList == null)
                throw new ArgumentNullException("items");

            for (int i = 0; i < containsList.Length; i++)
                if (inputStr.ToLower().Contains(containsList[i].ToLower()))
                    return true;

            return false;
        }
    }
}
