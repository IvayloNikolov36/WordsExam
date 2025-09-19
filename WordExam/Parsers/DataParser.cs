using System.Linq;
using SplitOptions = System.StringSplitOptions;
using static EnglishWordsExam.Constants;

namespace EnglishWordsExam.Parsers;

public static class FileDataParser
{
    public static string[] GetLineTokens(string dataLine)
    {
        return dataLine
            .Split([DelimiterWordTranslation], SplitOptions.RemoveEmptyEntries)
            .Select(word => word.Trim())
            .ToArray();
    }

    public static string[] GetWordTranslationTokens(string[] lineTokens)
    {
        return lineTokens[1]
            .Split([TranslationsDelimiter], SplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim())
            .ToArray();
    }
}
