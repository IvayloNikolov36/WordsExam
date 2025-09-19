using EnglishWordsExam.Enums;
using EnglishWordsExam.Exceptions;
using EnglishWordsExam.Models;
using EnglishWordsExam.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnglishWordsExam.Strategies
{
    public abstract class ExamBaseStrategy : IExamStrategy
    {
        private const int DefaultSupplementaryExamRounds = 0;
        private const string TranslationProcessException = "An exception occurred, so your answer would be accepted as true.";

        protected ExamBaseStrategy()
        {
            this.WordsForSupplementaryExam = [];
        }

        protected List<DictionaryWord> WordsForSupplementaryExam { get; }

        protected virtual int SupplementaryExamRounds { get; } = DefaultSupplementaryExamRounds;

        public abstract void ConductExam(
            IEnumerable<DictionaryWord> examWords,
            TranslationType translationType);

        protected (HashSet<int> hintedWords, HashSet<int> wrongTranslatedWords) Process(
            IEnumerable<DictionaryWord> examWords,
            TranslationType translationType)
        {
            HashSet<int> hintedWordsIndexes = [];
            HashSet<int> wrongTranslatedWordsIndexes = [];

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
                    : [englishWord];

                Console.WriteLine();
                Console.Write($"{wordIndex + 1}. Translate: '{textToTranslate}': ");
                string inputTranslation = Console.ReadLine();

                bool needsHint = this.IsHintCommand(inputTranslation);

                if (needsHint)
                {
                    ConsoleWrite.InfoLine(
                        ExamUtiliser.GetTranslationHints(translationsData, Constants.SymbolsToReveal));
                    hintedWordsIndexes.Add(wordIndex);
                    Console.Write("Your Translation: ");
                    inputTranslation = Console.ReadLine();
                }

                bool isTranslationRight = true;

                try
                {
                    isTranslationRight = this.IsGivenTranslationRight(
                        inputTranslation,
                        translationsData,
                        translationType);
                }
                catch (TranslationParseException trEx)
                {
                    if (trEx.InnerException is RegexMatchException)
                    {
                        // save info in txt file so the regex or the word to be revised
                    }
                    this.ShowExceptionalProcessTranslationMessage();
                }
                catch (Exception)
                {
                    this.ShowExceptionalProcessTranslationMessage();
                }

                if (!isTranslationRight)
                {
                    wrongTranslatedWordsIndexes.Add(wordIndex);
                    ConsoleWrite.ErrorLine("Wrong!");
                    this.ShowTranslationInfo(translationsData);
                    continue;
                }

                correctlyTranslated++;
                ConsoleWrite.SuccessLine("Right!");

                if (translationType == TranslationType.EnglishToBulgarian)
                {
                    this.ShowTranslationInfo(translationsData);
                }
            }

            this.ShowExamResult(correctlyTranslated, wordIndex + 1);

            return (hintedWordsIndexes, wrongTranslatedWordsIndexes);
        }

        protected void ProcessSupplementaryExam(
            IEnumerable<DictionaryWord> examWords,
            HashSet<int> resultWordsIndexes,
            TranslationType translationType)
        {
            IEnumerable<DictionaryWord> wordsPortion = this.GetWordsPortion(examWords, resultWordsIndexes);

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

                this.ConductExam(wordsPortion, translationType);

                return;
            }


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

        protected bool IsHintCommand(string command)
        {
            return command == Constants.HintCommand
                || command == Constants.HintCommandCyrilic;
        }

        private bool IsGivenTranslationRight(
            string givenTranslation,
            string[] translationsData,
            TranslationType translationType)
        {
            if (translationType == TranslationType.EnglishToBulgarian)
            {
                try
                {
                    List<string> allTranslations = ExamUtiliser.CompileAllTranslations(translationsData);

                    return allTranslations.Contains(givenTranslation);
                }
                catch (Exception ex)
                {
                    throw new TranslationParseException(
                        "Translations cannot be processed.",
                        ex);
                }
            }

            return translationsData[0] == givenTranslation;
        }

        private IEnumerable<DictionaryWord> GetWordsPortion(
            IEnumerable<DictionaryWord> examWords,
            HashSet<int> wordsIndexes)
        {
            return examWords
                .Where((word, index) => wordsIndexes.Contains(index));
        }

        private void ShowTranslationInfo(string[] translationsData)
        {
            string message = string.Empty;
            message += $"Translations:{Environment.NewLine}";
            message += $"{Environment.NewLine}---";
            message += $"{string.Join($"{Environment.NewLine}---", translationsData)}";

            ConsoleWrite.InfoLine(message);
        }

        private void ShowExamResult(int correct, int wordsCount)
        {
            string message = $"Translated correctly {correct}/{wordsCount}.";
            ConsoleWrite.InfoLine(message);
        }

        private void ShowExceptionalProcessTranslationMessage()
        {
            ConsoleWrite.ExceptionalInfoLine(TranslationProcessException);
        }
    }
}
