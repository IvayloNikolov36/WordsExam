using EnglishWordsExam;
using EnglishWordsExam.Enums;
using System;
using System.Collections.Generic;

namespace WordExam
{
    class Program
    {
        static void Main()
        {
            IList<DictionaryWord> words = WordsFileLoader.Load();

            PrintTotalWords(words.Count);

            int wordsToTranslate = ConsoleInputParser.GetWordsToTranslate(words.Count);

            TranslationType selectedTranslation = ConsoleInputParser.GetTranslationType();

            ExamProcessor exam = new ExamProcessor(words, wordsToTranslate, selectedTranslation);
            exam.Start();
        }

        private static void PrintTotalWords(int count)
        {
            Console.WriteLine($"Total words in dictionary {count}.");
        }
    }  
}
