using EnglishWordsExam.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnglishWordsExam
{
    public class ExamProcessor
    {
        private readonly IList<DictionaryWord> words;
        private readonly int wordsToTranslate;
        private readonly TranslationType translationType;

        public ExamProcessor(
            IEnumerable<DictionaryWord> words,
            int wordsToTranslate,
            TranslationType translationType)
        {
            this.words = words.ToList();
            this.wordsToTranslate = wordsToTranslate;
            this.translationType = translationType;
        }

        public void Start()
        {
            Console.WriteLine($"Input {Constants.HintCommand}/{Constants.HintCommandCyrilic} for a help.");

            int correctlyTranslated = 0;

            Random randomGenerator = new();
            DictionaryWord[] examWords = new DictionaryWord[wordsToTranslate];
            HashSet<int> usedRandomCrearedIndexes = new();

            for (int i = 0; i < wordsToTranslate; i++)
            {
                int randomIndex = randomGenerator.Next(0, this.words.Count);
                while (usedRandomCrearedIndexes.Contains(randomIndex))
                {
                    randomIndex = randomGenerator.Next(0, this.words.Count);
                }

                usedRandomCrearedIndexes.Add(randomIndex);
                examWords[i] = this.words[randomIndex];
            }

            HashSet<int> hintAndWrongTranslatedWordsIndexes = new();

            int wordIndex = -1;
            foreach (DictionaryWord dictionaryWord in examWords)
            {
                wordIndex++;

                string englishWord = dictionaryWord.Word;
                string[] translations = dictionaryWord.Translations;

                string textToTranslate = this.translationType == TranslationType.EnglishToBulgarian
                    ? englishWord
                    : string.Join(", ", translations);

                string[] translationsData = this.translationType == TranslationType.EnglishToBulgarian
                    ? translations
                    : new[] { englishWord };

                Console.WriteLine();
                Console.Write($"{wordIndex + 1}. Translate: '{textToTranslate}': ");
                string inputTranslation = Console.ReadLine();

                if (IsHintCommand(inputTranslation))
                {
                    PrintHints(translationsData, Constants.SymbolsToReveal);
                    hintAndWrongTranslatedWordsIndexes.Add(wordIndex);
                    Console.Write("Your Translation: ");
                    inputTranslation = Console.ReadLine();
                }

                if (!translationsData.Contains(inputTranslation))
                {
                    hintAndWrongTranslatedWordsIndexes.Add(wordIndex);
                    ConsoleWrite.ErrorLine("Wrong!");
                    ConsoleWrite.InfoLine("Correct translation:");
                    ConsoleWrite.Info($"{Environment.NewLine}---");
                    ConsoleWrite.InfoLine($"{string.Join($"{Environment.NewLine}---", translationsData)}");
                }
                else
                {
                    ConsoleWrite.SuccessLine("Right!");
                    correctlyTranslated++;

                    if (this.translationType == TranslationType.EnglishToBulgarian)
                    {
                        ConsoleWrite.InfoLine("All translations: ");
                        ConsoleWrite.Info($"{Environment.NewLine}---");
                        ConsoleWrite.InfoLine(string.Join($"{Environment.NewLine}---", translationsData));
                    }
                }
            }

            this.PrintResult(correctlyTranslated, examWords.Length);

            if (hintAndWrongTranslatedWordsIndexes.Any())
            {
                Console.WriteLine("Do you want to run exam with the words that did not translate right and you took hints?");

                Console.WriteLine("Click on y/n: ");

                ConsoleKeyInfo key = Console.ReadKey();

                if (key.KeyChar == 'y')
                {
                    DictionaryWord[] hintedWords = examWords
                        .Where((word, index) => hintAndWrongTranslatedWordsIndexes.Contains(index))
                        .ToArray();

                    ExamProcessor newExamProcessor = new ExamProcessor(hintedWords, hintedWords.Length, this.translationType);
                    newExamProcessor.Start();
                }
            }
        }

        private bool IsHintCommand(string command)
        {
            return command == Constants.HintCommand
                || command == Constants.HintCommandCyrilic;
        }

        private void PrintHints(string[] translations, int symbolsToReveal)
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
