using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using EnglishWordsExam.Parsers;
using EnglishWordsExam.Strategies;
using EnglishWordsExam.Utilities;
using System;

namespace EnglishWordsExam
{
    public class AppEngine
    {
        private readonly IReader reader;

        public AppEngine(IReader reader)
        {
            this.reader = reader;
        }

        public void Run()
        {
            LoadWordsResult wordsResult = this.reader.LoadWords();

            PrintTotalWords(wordsResult.WordsCount);

            bool newTurn = true;
            while (newTurn)
            {
                int wordsCountForTranslation = ConsoleInputParser
                    .GetWordsCountForTranslation(wordsResult.WordsCount);

                TranslationType selectedTranslationType = ConsoleInputParser.GetTranslationType();

                ExamProcessor exam = new(
                    wordsResult.Words,
                    wordsCountForTranslation,
                    selectedTranslationType,
                    new SpaciousSupplementaryExamStrategy());

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
