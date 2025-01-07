using System;
using System.Linq;

namespace EnglishWordsExam.Parsers
{
    public static class FileDataParser
    {
        public static string[] GetLineTokens(string dataLine)
        {
            return dataLine
                .Split(new[] { Constants.DelimiterWordTranslation }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.Trim())
                .ToArray();
        }

        public static string[] GetWordTranslationTokens(string[] lineTokens)
        {
            return lineTokens[1]
            .Split(new[] { Constants.TranslationsDelimiter }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim())
            .ToArray();
        }
    }
}
