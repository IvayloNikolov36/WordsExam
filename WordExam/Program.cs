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
            (IEnumerable<DictionaryWord> words, int count) = WordsFileLoader.Load();

            PrintTotalWords(count);

            int wordsToTranslate = ConsoleInputParser.GetWordsToTranslate(count);

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
