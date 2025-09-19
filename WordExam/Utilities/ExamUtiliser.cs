using EnglishWordsExam.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EnglishWordsExam.Utilities;

public static class ExamUtiliser
{
    public static string GetTranslationHints(string[] translations, int symbolsToReveal)
    {
        StringBuilder hints = new();

        foreach (string word in translations)
        {
            string visiblePart = new string(word.Take(symbolsToReveal).ToArray());
            hints.Append(visiblePart);

            IEnumerable<char> symbolsToConceal = word
                .Skip(symbolsToReveal)
                .Take(word.Length - symbolsToReveal);

            char[] concealed = symbolsToConceal
                .Select(symbol =>
                {
                    if (symbol == ' ')
                    {
                        return symbol;
                    }

                    return Constants.HintMaskSymbol;
                })
                .ToArray();

            hints.Append(new string(concealed));
            hints.AppendLine($". ({word.Length}) symbols.");
        }

        return hints.ToString();
    }

    public static List<string> CompileAllTranslations(string[] translations)
    {
        char symbolToCheck = '(';
        char symbolVisaVersa = '/';

        IEnumerable<string> noProcessTranslations = translations
            .Where(x => !x.Contains(symbolToCheck) && !x.Contains(symbolVisaVersa));

        List<string> result = [.. noProcessTranslations];

        List<string> filteredTranslations = [.. translations.Where(x => x.Contains(symbolToCheck))];

        if (filteredTranslations.Count > 0)
        {
            List<string> translationsToAdd = GetAllTranslationsForWordWithAdditionInParenthesis(filteredTranslations);
            result.AddRange(translationsToAdd);
        }

        filteredTranslations = [.. translations.Where(x => x.Contains(symbolVisaVersa))];
        if (filteredTranslations.Count > 0)
        {
            List<string> translationsViseVersa = GetAllTranslationsForVisaVersaType(filteredTranslations);
            result.AddRange(translationsViseVersa);
        }

        return result;
    }

    private static List<string> GetAllTranslationsForVisaVersaType(List<string> translations)
    {
        Regex rgx = new(@"(?<before>[\w\s]*?)((?<left>\w+)(\/(?<right>\w+))+)(?<after>[\w\s]*)");

        List<string> resultTranslations = new();

        translations.ForEach((string translation) =>
        {
            Match match = rgx.Match(translation);

            if (!match.Success)
            {
                throw new RegexMatchException($"""Translation "{translation}" cannot be processed.""");
            }

            string beforePart = match.Groups["before"].Value.Trim();

            string left = match.Groups["left"].Value.Trim();
            string right = match.Groups["right"].Value.Trim();

            string afterPart = match.Groups["after"].Value.Trim();

            if (beforePart.Trim().Length == 0)
            {
                resultTranslations.Add($"{left} {afterPart}");
                resultTranslations.Add($"{right} {afterPart}");
            }

            if (afterPart.Trim().Length == 0)
            {
                resultTranslations.Add($"{beforePart} {left}");
                resultTranslations.Add($"{beforePart} {right}");
            }
        });

        return resultTranslations;
    }

    private static List<string> GetAllTranslationsForWordWithAdditionInParenthesis(List<string> translations)
    {
        // case1: лея (се) => ["лея", "лея се"]
        // case2: (из)обилен => ["изобилен", "обилен"]
        // case3: изключвам (възможността за) => ["изключвам", "изключвам възможността за", изключвам (възможността за)]
        // case4: прекалено (из)обилен => ["прекалено изобилен", "прекалено обилен"]
        // case5: извинявам (се) предварително => ["извинявам предварително", "извинявам се предварително"]

        Regex rgx = new(@"(?<before>[\w\s]*)\s*\((?<incompassed>[\w\s]+)\)\s*(?<after>[\w\s]*)");

        List<string> resultTranslations = new(translations.Count * 2);

        translations
            .ForEach((string translation) =>
            {
                Match match = rgx.Match(translation);

                if (!match.Success)
                {
                    throw new RegexMatchException($"""Translation "{translation}" cannot be processed.""");
                }

                string before = match.Groups["before"].Value.Trim();
                string incompassed = match.Groups["incompassed"].Value.Trim();
                string after = match.Groups["after"].Value.Trim();

                bool hasSpaceBefore = HasSpaceBeforeOpenParenthesis(translation);
                bool hasSpaceAfter = HasSpaceAfterCloseParenthesis(translation);
                string spaceOrNotBefore = hasSpaceBefore ? " " : string.Empty;
                string spaceOrNotAfter = hasSpaceAfter ? " " : string.Empty;

                resultTranslations.Add(translation);
                resultTranslations.Add(
                    $"{before}{spaceOrNotBefore}{incompassed}{spaceOrNotAfter}{after}".Trim()
                );
                resultTranslations.Add($"{before} {after}".Trim());
            });

        return resultTranslations;
    }

    private static bool HasSpaceBeforeOpenParenthesis(string word)
    {
        const char parenthesisSymbol = '(';

        if (word.First() == parenthesisSymbol)
        {
            return false;
        }

        int parenthesisIndex = word.IndexOf(parenthesisSymbol);

        int indexOfSpace = word.IndexOf(' ', parenthesisIndex - 1, 1);

        return parenthesisIndex - indexOfSpace == 1;
    }

    private static bool HasSpaceAfterCloseParenthesis(string word)
    {
        const char parenthesisSymbol = ')';

        if (word.Last() == parenthesisSymbol)
        {
            return false;
        }

        int parenthesisIndex = word.IndexOf(parenthesisSymbol);

        int indexOfSpace = word.IndexOf(' ', parenthesisIndex + 1, 1);

        return indexOfSpace - parenthesisIndex == 1;
    }
}