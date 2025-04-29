using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using EnglishWordsExam.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EnglishWordsExam.Strategies
{
    public abstract class ExamBaseStrategy : IExamStrategy
    {
        private const int DefaultSupplementaryExamRounds = 0;

        protected ExamBaseStrategy()
        {
            this.WordsForSupplementaryExam = new List<DictionaryWord>();
        }

        protected List<DictionaryWord> WordsForSupplementaryExam { get; }

        protected virtual int SupplementaryExamRounds { get; } = DefaultSupplementaryExamRounds;

        public abstract void ProcessExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType);

        protected (HashSet<int> hintedWords, HashSet<int> wrongTranslatedWords) Process(
            IEnumerable<DictionaryWord> examWords,
            TranslationType translationType)
        {
            HashSet<int> hintedWordsIndexes = new();
            HashSet<int> wrongTranslatedWordsIndexes = new();

            int correctlyTranslated = 0;

            int wordIndex = -1;
            foreach (DictionaryWord dictionaryWord in examWords)
            {
                wordIndex++;

                string englishWord = dictionaryWord.Word;
                string[] translations = dictionaryWord.Translations;

                string textToTranslate = translationType == TranslationType.EnglishToBulgarian
                    ? englishWord
                    : string.Join(", ", translations);

                string[] translationsData = translationType == TranslationType.EnglishToBulgarian
                    ? translations
                    : new[] { englishWord };

                Console.WriteLine();
                Console.Write($"{wordIndex + 1}. Translate: '{textToTranslate}': ");
                string inputTranslation = Console.ReadLine();

                bool needsHint = this.IsHintCommand(inputTranslation);

                if (needsHint)
                {
                    this.PrintHints(translationsData, Constants.SymbolsToReveal);
                    hintedWordsIndexes.Add(wordIndex);
                    Console.Write("Your Translation: ");
                    inputTranslation = Console.ReadLine();
                }

                bool isTranslationRight = this.IsGivenTranslationRight(
                    inputTranslation,
                    translationsData,
                    translationType);

                if (!isTranslationRight)
                {
                    wrongTranslatedWordsIndexes.Add(wordIndex);
                    this.PrintCorrectTranslationInfo(translationsData);
                    continue;
                }

                correctlyTranslated++;
                ConsoleWrite.SuccessLine("Right!");

                if (translationType == TranslationType.EnglishToBulgarian)
                {
                    ConsoleWrite.InfoLine("All translations: ");
                    ConsoleWrite.Info($"{Environment.NewLine}---");
                    ConsoleWrite.InfoLine(string.Join($"{Environment.NewLine}---", translationsData));
                }
            }

            this.PrintResult(correctlyTranslated, wordIndex + 1);

            return (hintedWordsIndexes, wrongTranslatedWordsIndexes);
        }

        protected void ProcessSupplementaryExam(
            IEnumerable<DictionaryWord> examWords,
            HashSet<int> resultWordsIndexes,
            TranslationType translationType)
        {
            List<DictionaryWord> wordsPortion = this.GetWordsPortion(examWords, resultWordsIndexes);

            this.WordsForSupplementaryExam.AddRange(wordsPortion);

            if (this.WordsForSupplementaryExam.Count == 0)
            {
                return;
            }

            if (this.SupplementaryExamRounds == 1)
            {
                ConsoleWrite.AnnouncementLine(
                    $"Supplementary exam ({this.WordsForSupplementaryExam.Count} word(s)).");

                this.WordsForSupplementaryExam.Clear();

                this.ProcessExam(wordsPortion, translationType);
            }

            if (this.SupplementaryExamRounds > 1)
            {
                HashSet<int> hintedAndWrongWordIndexes = new();

                for (int round = 0; round < this.SupplementaryExamRounds; round++)
                {
                    if (this.WordsForSupplementaryExam.Count == 0)
                    {
                        return;
                    }

                    ConsoleWrite.AnnouncementLine(
                        $"Supplementary exam round {round + 1} ({this.WordsForSupplementaryExam.Count} word(s)).");

                    (HashSet<int> hinted, HashSet<int> wrong) = this.Process(
                        this.WordsForSupplementaryExam.ToArray(),
                        translationType);

                    hintedAndWrongWordIndexes.UnionWith(hinted.Union(wrong));
                }

                if (hintedAndWrongWordIndexes.Count == 0)
                {
                    return;
                }

                this.WordsForSupplementaryExam.Clear();
                this.ProcessSupplementaryExam(wordsPortion, hintedAndWrongWordIndexes, translationType);
            }
        }

        protected bool IsHintCommand(string command)
        {
            return command == Constants.HintCommand
                || command == Constants.HintCommandCyrilic;
        }

        protected void PrintHints(string[] translations, int symbolsToReveal)
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

            Console.WriteLine(hints.ToString());
        }

        private bool IsGivenTranslationRight(string givenTranslation, string[] translationsData, TranslationType translationType)
        {
            if (translationType == TranslationType.EnglishToBulgarian)
            {
                List<string> allTranslations = this.GetAllTranslations(translationsData);

                return allTranslations.Contains(givenTranslation);
            }

            return translationsData[0] == givenTranslation;
        }

        private List<DictionaryWord> GetWordsPortion(IEnumerable<DictionaryWord> examWords, HashSet<int> wordsIndexes)
        {
            int index = 0;
            List<DictionaryWord> result = new List<DictionaryWord>(wordsIndexes.Count);

            foreach (DictionaryWord item in examWords)
            {
                if (wordsIndexes.Contains(index++))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private List<string> GetAllTranslationsForVisaVersaType(List<string> translations)
        {
            Regex rgx = new(@"(?<base>[\w\s]+)\s(?<first>\w+)(\/(?<add>\w+))+");

            List<string> resultTranslations = new();

            translations.ForEach((string translation) =>
            {
                Match match = rgx.Match(translation);

                string basePart = match.Groups["base"].Value;
                string firstAddition = match.Groups["first"].Value;

                resultTranslations.Add($"{basePart} {firstAddition}");

                CaptureCollection additions = match.Groups["add"].Captures;
                for (int i = 0; i < additions.Count; i++)
                {
                    string addition = additions[i].Value;
                    resultTranslations.Add($"{basePart} {addition}");
                }
            });

            return resultTranslations;
        }

        private List<string> GetAllTranslationsForWordWithAddition(List<string> translations)
        {
            Regex rgx = new(@"(?<first>\w+)\s+\((?<second>\w+)\)");

            List<string> resultTranslations = new List<string>(translations.Count * 2);

            translations
                .ForEach((string x) =>
                {
                    Match match = rgx.Match(x);

                    string first = match.Groups["first"].Value;
                    string second = match.Groups["second"].Value;

                    resultTranslations.Add($"{first} {second}");
                    resultTranslations.Add(first);
                });

            return resultTranslations;
        }

        private void PrintCorrectTranslationInfo(string[] translationsData)
        {
            ConsoleWrite.ErrorLine("Wrong!");
            ConsoleWrite.InfoLine("Correct translation:");

            ConsoleWrite.Info($"{Environment.NewLine}---");
            ConsoleWrite.InfoLine($"{string.Join($"{Environment.NewLine}---", translationsData)}");
        }

        private void PrintResult(int correct, int wordsCount)
        {
            Console.WriteLine();
            ConsoleWrite.InfoLine($"Translated correctly {correct}/{wordsCount}.");
        }

        private List<string> GetAllTranslations(string[] translations)
        {
            char symbolToCheck = '(';
            char symbolVisaVersa = '/';

            IEnumerable<string> noProcessTranslations = translations
                .Where(x => !x.Contains(symbolToCheck) && !x.Contains(symbolVisaVersa));

            List<string> result = [.. noProcessTranslations];

            List<string> filteredTranslations = [.. translations.Where(x => x.Contains(symbolToCheck))];
            if (filteredTranslations.Count > 0)
            {
                List<string> translationsToAdd = this.GetAllTranslationsForWordWithAddition(filteredTranslations);
                result.AddRange(translationsToAdd);
            }

            filteredTranslations = [.. translations.Where(x => x.Contains(symbolVisaVersa))];
            if (filteredTranslations.Count > 0)
            {
                List<string> translationsViseVersa = this.GetAllTranslationsForVisaVersaType(filteredTranslations);
                result.AddRange(translationsViseVersa);
            }
            
            return result;
        }
    }
}
