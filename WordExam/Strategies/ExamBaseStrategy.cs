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
        protected ExamBaseStrategy()
        {
            this.WordsForSupplementaryExam = new List<DictionaryWord>();
        }

        protected List<DictionaryWord> WordsForSupplementaryExam { get; }

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
            int index = 0;
            foreach (DictionaryWord item in examWords)
            {
                if (resultWordsIndexes.Contains(index++))
                {
                    this.WordsForSupplementaryExam.Add(item);
                }
            }

            if (this.WordsForSupplementaryExam.Count > 0)
            {
                ConsoleWrite.AnnouncementLine($"Supplementary exam for {this.WordsForSupplementaryExam.Count} words.");

                DictionaryWord[] wordsExam = this.WordsForSupplementaryExam.ToArray();
                this.WordsForSupplementaryExam.Clear();

                this.ProcessExam(wordsExam, translationType);
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

        private void PrintResult(int correct, int wordsCount)
        {
            Console.WriteLine();
            ConsoleWrite.InfoLine($"Translated correctly {correct}/{wordsCount}.");
        }
    }
}
