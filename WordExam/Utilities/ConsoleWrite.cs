using System;

namespace EnglishWordsExam.Utilities
{
    public class ConsoleWrite
    {
        public static void ErrorLine(string text)
        {
            ColorLine(text, ConsoleColor.Red);
        }

        public static void InfoLine(string text)
        {
            ColorLine(text, ConsoleColor.Blue, ConsoleColor.White);
        }

        public static void AnnouncementLine(string text)
        {
            Console.WriteLine();
            ColorLine(text, ConsoleColor.Yellow);
        }

        public static void Info(string text)
        {
            ColorLine(text, ConsoleColor.Blue, ConsoleColor.White, isLine: false);
        }

        public static void SuccessLine(string text)
        {
            ColorLine(text, ConsoleColor.Green);
        }

        private static void ColorLine(
            string text,
            ConsoleColor consoleColor,
            ConsoleColor foreGroundColor = ConsoleColor.Black,
            bool isLine = true)
        {
            Console.BackgroundColor = consoleColor;
            Console.ForegroundColor = foreGroundColor;

            if (isLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
            
            Console.ResetColor();
        }
    }
}
