using EnglishWordsExam.Models;
using EnglishWordsExam.Parsers;
using System.Collections.Generic;
using System.IO;

namespace EnglishWordsExam
{
    public class FileReader : IReader
    {
        private readonly string filePath;

        public FileReader(string filePath)
        {
            this.filePath = filePath;
        }

        public LoadWordsResult LoadWords()
        {
            StreamReader reader = new(this.filePath);
            HashSet<DictionaryWord> words = new();

            string line;
            while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
            {
                words.Add(ParseLineInfo(line));
            }

            reader.Dispose();

            return new LoadWordsResult
            { 
                Words = words, 
                WordsCount = words.Count 
            };
        }

        public string[] GetWordLines()
        {
            string[] lines = File.ReadAllLines(this.filePath);

            return lines;
        }

        private static DictionaryWord ParseLineInfo(string line)
        {
            string[] lineTokens = FileDataParser.GetLineTokens(line);

            string enWord = lineTokens[0];
            string[] translations = FileDataParser.GetWordTranslationTokens(lineTokens);

            return new DictionaryWord(enWord, translations);
        }
    }
}
