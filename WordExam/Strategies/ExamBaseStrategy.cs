using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.EventHandlers.EventArguments;
using EnglishWordsExam.Exceptions;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies.Contracts;
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

        public event OnWordForTranslationSendEventHandler OnWordForTranslationSending;
        public event OnTranslationResultSendEventHandler OnTranslationResultSending;
        public event OnTranslationHintsSendEventHandler OnTranslationHintsSending;
        public event OnExamMessageSendEventHandler OnExamMessageSend;
        public event OnExamCompletedEventHandler OnExamCompleted;

        private string receivedTranslation = null;

        protected ExamBaseStrategy()
        {
            this.WordsForSupplementaryExam = [];
        }

        protected ExamBaseStrategy(IEventTranslationSender eventSender)
            : this()
        {
            eventSender.OnTranslationSendEvent += EventTranslation_OnTranslationEvent;
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

                this.OnWordForTranslationSending(this, new TranslationEventArgs(textToTranslate, wordIndex));

                while (this.receivedTranslation == null)
                {
                    continue;
                }

                string inputTranslation = this.receivedTranslation;
                this.receivedTranslation = null;

                bool needsHint = this.IsHintCommand(inputTranslation);
                if (needsHint)
                {
                    IEnumerable<string> translationHints = ExamUtiliser.GetTranslationHints(
                        translationsData,
                        Constants.SymbolsToReveal);

                    this.OnTranslationHintsSending(this, new TranslationHintsEventArgs(translationHints));

                    hintedWordsIndexes.Add(wordIndex);

                    while (this.receivedTranslation == null)
                    {
                        continue;
                    }

                    inputTranslation = this.receivedTranslation;
                    this.receivedTranslation = null;
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

                    this.OnExamMessageSend(this, new MessageEventArgs(TranslationProcessException));
                }
                catch (Exception)
                {
                    this.OnExamMessageSend(this, new MessageEventArgs(TranslationProcessException));
                }

                if (!isTranslationRight)
                {
                    wrongTranslatedWordsIndexes.Add(wordIndex);
                    this.OnTranslationResultSending(this, new TranslationResultEventArgs(false, translationsData));
                    continue;
                }

                correctlyTranslated++;

                this.OnTranslationResultSending(this, new TranslationResultEventArgs(
                    isCorrect: true,
                    translationType == TranslationType.EnglishToBulgarian
                        ? translationsData
                        : []));
            }

            this.OnExamCompleted(this, new ExamComplitionEventArgs(correctlyTranslated, wordIndex + 1));

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
                this.OnExamMessageSend(this, new MessageEventArgs($"Supplementary exam ({this.WordsForSupplementaryExam.Count} word(s))."));

                this.WordsForSupplementaryExam.Clear();

                this.ConductExam(wordsPortion, translationType);

                return;
            }

            HashSet<int> hintedAndWrongWordIndexes = [];

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

        private void EventTranslation_OnTranslationEvent(object sender, TranslationEventArgs translation)
        {
            this.receivedTranslation = translation.Text;
        }
    }
}
