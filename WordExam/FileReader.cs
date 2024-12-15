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
                string[] lineTokens = line
                    .Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.Trim())
                    .ToArray();

                string enWord = lineTokens[0];

                string[] translations = lineTokens[1]
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w.Trim())
                    .ToArray();

                words.Add(new DictionaryWord(enWord, translations));
            }

            reader.Dispose();

            return new LoadWordsResult { Words = words, WordsCount = words.Count };
        }
    }
}
