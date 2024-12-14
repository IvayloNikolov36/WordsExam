using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using EnglishWordsExam.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

                if (IsHintCommand(inputTranslation))
                {
                    PrintHints(translationsData, Constants.SymbolsToReveal);
                    hintedWordsIndexes.Add(wordIndex);
                    Console.Write("Your Translation: ");
                    inputTranslation = Console.ReadLine();
                }

                if (!translationsData.Contains(inputTranslation))
                {
                    wrongTranslatedWordsIndexes.Add(wordIndex);
                    ConsoleWrite.ErrorLine("Wrong!");
                    ConsoleWrite.InfoLine("Correct translation:");
                    ConsoleWrite.Info($"{Environment.NewLine}---");
                    ConsoleWrite.InfoLine($"{string.Join($"{Environment.NewLine}---", translationsData)}");
                }
                else
                {
                    ConsoleWrite.SuccessLine("Right!");
                    correctlyTranslated++;

                    if (translationType == TranslationType.EnglishToBulgarian)
                    {
                        ConsoleWrite.InfoLine("All translations: ");
                        ConsoleWrite.Info($"{Environment.NewLine}---");
                        ConsoleWrite.InfoLine(string.Join($"{Environment.NewLine}---", translationsData));
                    }
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
            this.WordsForSupplementaryExam.AddRange(this.GetWordsPortion(examWords, resultWordsIndexes));

            if (this.WordsForSupplementaryExam.Count == 0)
            {
                return;
            }

            if (this.SupplementaryExamRounds == 1)
            {
                ConsoleWrite.AnnouncementLine(
                    $"Supplementary exam ({this.WordsForSupplementaryExam.Count} word(s)).");

                DictionaryWord[] wordsForSupplementaryExam = this.WordsForSupplementaryExam.ToArray();
                this.WordsForSupplementaryExam.Clear();

                this.ProcessExam(wordsForSupplementaryExam, translationType);
            }

            if (this.SupplementaryExamRounds > 1)
            {
                HashSet<int> hintedAndWrong = new HashSet<int>();

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

                    hintedAndWrong.UnionWith(hinted.Union(wrong));
                }

                if (hintedAndWrong.Count == 0)
                {
                    return;
                }

                this.WordsForSupplementaryExam.Clear();
                this.ProcessSupplementaryExam(examWords, hintedAndWrong, translationType);
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

        private void PrintResult(int correct, int wordsCount)
        {
            Console.WriteLine();
            ConsoleWrite.InfoLine($"Translated correctly {correct}/{wordsCount}.");
        }
    }
}
