using EnglishWordsExam;
using EnglishWordsExam.Enums;
using EnglishWordsExam.Strategies;
using System;
using System.Collections.Generic;

namespace WordExam
{
    class Program
    {
        static void Main()
        {
            (IEnumerable<DictionaryWord> words, int count) = WordsFileLoader.Load();

            PrintTotalWords(count);

            bool newTurn = true;
            while (newTurn)
            {
                int wordsToTranslate = ConsoleInputParser.GetWordsToTranslate(count);

                TranslationType selectedTranslation = ConsoleInputParser.GetTranslationType();

                ExamProcessor exam = new ExamProcessor(
                    words,
                    wordsToTranslate,
                    selectedTranslation,
                    new HintedAndWrongWordsSupplementaryExamStrategy());

                exam.Start();

                ConsoleWrite.AnnouncementLine("If you don't want to take another exam, input exit. (Every other input will start a new exam)");

                string choice = Console.ReadLine().Trim().ToLower();
                if (choice == Constants.ExitCommand)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void PrintTotalWords(int count)
        {
            Console.WriteLine($"Total words in dictionary {count}.");
        }
    }  
}
