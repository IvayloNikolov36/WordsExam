using EnglishWordsExam.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EnglishWordsExam
{
    public class FileReader : IReader
    {
        public LoadWordsResult LoadWords()
        {
            StreamReader reader = new(Constants.FilePath);
            HashSet<DictionaryWord> words = new();

            string line;
            while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
            {
                words.Add(ParseLineInfo(line));
            }

            reader.Dispose();

            return new LoadWordsResult { Words = words, WordsCount = words.Count };
        }

        private static DictionaryWord ParseLineInfo(string line)
        {
            string[] lineTokens = line
                .Split(new[] { Constants.DelimiterWordTranslation }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim())
                .ToArray();

            string enWord = lineTokens[0];

            string[] translations = lineTokens[1]
                .Split(new[] { Constants.TranslationsDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .ToArray();

            return new DictionaryWord(enWord, translations);
        }
    }
}
