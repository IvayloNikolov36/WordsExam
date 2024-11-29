using EnglishWordsExam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static EnglishWordsExam.Constants;

namespace WordExam
{
    class Program
    {
        static void Main()
        {
            HashSet<DictionaryWord> words = LoadWordsFromFile(FilePath);

            Console.WriteLine($"Total words in dictionary {words.Count}.");

            uint wordsToTranslate = GetWordsToTranslateInput(words.Count);

            Console.Write("Input 1 for En-Bg translate or 2 for Bg-En: ");
            int selection = int.Parse(Console.ReadLine().First().ToString());

            Console.WriteLine("Input 'hint' when you can't remember the translation.");

            switch (selection)
            {
                case 1:
                    ProcessExam(words.ToArray(), wordsToTranslate, translateEnToBg: true);
                    break;
                case 2:
                    ProcessExam(words.ToArray(), wordsToTranslate, translateEnToBg: false);
                    break;
                default:
                    break;
            }

            Console.ReadLine();
        }

        private static uint GetWordsToTranslateInput(int wordsCount)
        {
            uint wordsToTranslate = 0;

            bool isValid = false;
            bool isParsed;
            while (!isValid)
            {
                Console.Write("Enter words count that you want to translate: ");
                isParsed = uint.TryParse(Console.ReadLine(), out wordsToTranslate);

                if (!isParsed)
                {
                    Console.WriteLine("Words count should be a positive integer number.");
                    continue;
                }

                if (wordsToTranslate > wordsCount)
                {
                    Console.WriteLine($"Words count should be less than or equal to the words in the dictionary ({wordsCount}).");
                    continue;
                }


                if (wordsToTranslate == 0)
                {
                    Console.WriteLine($"Words count should be more than 0.");
                    continue;
                }

                isValid = true;
            }

            return wordsToTranslate;
        }

        private static HashSet<DictionaryWord> LoadWordsFromFile(string filePath)
        {
            StreamReader reader = new(filePath);
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

            return words;
        }

        private static void ProcessExam(DictionaryWord[] words, uint wordsToTranslate, bool translateEnToBg)
        {
            int correctlyTranslated = 0;

            Random randomGenerator = new();
            DictionaryWord[] examWords = new DictionaryWord[wordsToTranslate];
            HashSet<int> usedRandomCrearedIndexes = new HashSet<int>();
            for (int i = 0; i < wordsToTranslate; i++)
            {
                int randomIndex = randomGenerator.Next(0, words.Length);
                while (usedRandomCrearedIndexes.Contains(randomIndex))
                {
                    randomIndex = randomGenerator.Next(0, words.Length);
                }

                usedRandomCrearedIndexes.Add(randomIndex);
                examWords[i] = words[randomIndex];
            }

            HashSet<int> hintAndWrongTranslatedWordsIndexes = new HashSet<int>();

            int wordIndex = -1;
            foreach (DictionaryWord dictionaryWord in examWords)
            {
                wordIndex++;
                string englishWord = dictionaryWord.Word;
                string[] translations = dictionaryWord.Translations;

                string textToTranslate = translateEnToBg ? englishWord : string.Join(", ", translations);
                string[] translationsData = translateEnToBg ? translations : new[] { englishWord };

                Console.WriteLine();
                Console.Write($"{wordIndex + 1}. Translate: '{textToTranslate}': ");
                string inputTranslation = Console.ReadLine();

                if (IsHintCommand(inputTranslation))
                {
                    PrintHints(translationsData, SymbolsToReveal);
                    hintAndWrongTranslatedWordsIndexes.Add(wordIndex);
                    Console.Write("Your Translation: ");
                    inputTranslation = Console.ReadLine();
                }

                if (!translationsData.Contains(inputTranslation))
                {
                    hintAndWrongTranslatedWordsIndexes.Add(wordIndex);
                    Console.WriteLine($"Wrong! Correct translation: {string.Join(", ", translationsData)}");
                }
                else
                {
                    Console.Write("Right! ");
                    correctlyTranslated++;

                    if (translateEnToBg)
                    {
                        Console.WriteLine("All translations: ");
                        Console.Write($"{Environment.NewLine}---");
                        Console.WriteLine(string.Join($"{Environment.NewLine}---", translationsData));
                    }
                }
            }

            PrintResult(correctlyTranslated, examWords.Length);

            if (hintAndWrongTranslatedWordsIndexes.Any())
            {
                Console.WriteLine("Do you want to run exam with the words that did not translate right and you took hints?");
                Console.Write("Click on y/n: ");
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.KeyChar == 'y')
                {
                    DictionaryWord[] hintedWords = examWords
                        .Where((word, index) => hintAndWrongTranslatedWordsIndexes.Contains(index))
                        .ToArray();

                    ProcessExam(hintedWords, (uint)hintedWords.Length, translateEnToBg);
                }
            }
        }

        private static bool IsHintCommand(string command)
        {
            return command == HintCommand || command == HintCommandCyrilic;
        }

        private static void PrintHints(string[] translations, int symbolsToReveal)
        {
            StringBuilder hints = new StringBuilder();

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

                        return HintMaskSymbol;
                    })
                    .ToArray();

                hints.Append(new string(concealed));
                hints.AppendLine($". ({word.Length}) symbols.");
            }

            Console.WriteLine(hints.ToString());
        }

        private static void PrintResult(int correct, int wordsCount)
        {
            Console.WriteLine();
            Console.WriteLine($"Translated correctly {correct}/{wordsCount}.");
        }
    }  
}
