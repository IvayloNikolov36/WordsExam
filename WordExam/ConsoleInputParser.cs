using EnglishWordsExam.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnglishWordsExam
{
    public class ConsoleInputParser
    {
        public static int GetWordsCountForTranslation(int totalWordsCount)
        {
            uint wordsToTranslate = 0;

            bool isValid = false;
            bool isParsed;
            while (!isValid)
            {
                Console.Write("Enter words count that you want to translate: ");
                isParsed = uint.TryParse(Console.ReadLine(), out wordsToTranslate);

                if (!isParsed)
                {
                    Console.WriteLine("Words count should be a positive integer number.");
                    continue;
                }

                if (wordsToTranslate > totalWordsCount)
                {
                    Console.WriteLine($"Selected words count for translation should be less than or equal to the words amount in the dictionary ({totalWordsCount}).");
                    continue;
                }

                if (wordsToTranslate == 0)
                {
                    Console.WriteLine($"Words count should be more than 0!");
                    continue;
                }

                isValid = true;
            }

            return (int)wordsToTranslate;
        }

        public static TranslationType GetTranslationType()
        {
            IEnumerable<string> translationTypes = Enum
                .GetValues(typeof(TranslationType))
                .Cast<int>()
                .Select(x => x.ToString());

            string inputTranslationType;
            while(true)
            {
                Console.WriteLine("---Input 1 for English-Bulgarian translate");
                Console.WriteLine("---Input 2 for Bulgarian-English translate");

                inputTranslationType = Console.ReadLine().Trim();
                if (!translationTypes.Any(tt => tt == inputTranslationType))
                {
                    Console.WriteLine("Invalid selection!");
                    continue;
                }

                return (TranslationType)int.Parse(inputTranslationType);
            }
        }
    }
}
