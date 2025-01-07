using EnglishWordsExam.Models;
using EnglishWordsExam.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace EnglishWordsExam.Utilities
{
    public class DataScanner
    {
        public RowDataDuplicationInfo[] FindLinesWithDuplicateTranslations(IEnumerable<string> lines)
        {
            Dictionary<string, IList<int>> duplicateTranslations = new();

            int lineNumber = 1;
            foreach (string line in lines)
            {
                string[] lineTokens = FileDataParser.GetLineTokens(line);
                string[] translationsTokens = FileDataParser.GetWordTranslationTokens(lineTokens);

                string translations = string.Join(",", translationsTokens).ToLower();

                if (!duplicateTranslations.ContainsKey(translations))
                {
                    duplicateTranslations.Add(translations, new List<int>());
                }

                duplicateTranslations[translations].Add(lineNumber);

                lineNumber++;
            }

            RowDataDuplicationInfo[] duplicatOnRows = duplicateTranslations
                .Where(x => x.Value.Count > 1)
                .Select(x => new RowDataDuplicationInfo
                {
                    RowNumber = x.Value[0],
                    DuplicateRows = x.Value.Skip(1).ToArray()
                })
                .ToArray();

            return duplicatOnRows;
        }
    }
}
