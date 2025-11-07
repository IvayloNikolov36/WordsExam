using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.EventHandlers.EventArguments;
using EnglishWordsExam.Exceptions;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies.Contracts;
using EnglishWordsExam.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
        public event OnSupplementaryExamStartedEventHandler OnSupplementaryExamStarted;

        private readonly IEventTranslationSender eventSender;

        protected ExamBaseStrategy()
        {
            this.WordsForSupplementaryExam = [];
        }

        protected ExamBaseStrategy(IEventTranslationSender eventSender)
            : this()
        {
            this.eventSender = eventSender;
        }

        protected List<DictionaryWord> WordsForSupplementaryExam { get; }

        protected virtual int SupplementaryExamRounds { get; } = DefaultSupplementaryExamRounds;

        public abstract Task ConductExam(
            IEnumerable<DictionaryWord> examWords,
            TranslationType translationType);

        protected async Task<ExamProcessResult> Process(
            IEnumerable<DictionaryWord> examWords,
            TranslationType translationType,
            int? supplementaryExamRound = null)
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

                string inputTranslation = await this.GetReceivedTranslation();

                bool needsHint = this.IsHintCommand(inputTranslation);
                if (needsHint)
                {
                    IEnumerable<string> translationHints = ExamUtiliser.GetTranslationHints(
                        translationsData,
                        Constants.SymbolsToReveal);

                    this.OnTranslationHintsSending(this, new TranslationHintsEventArgs(translationHints));

                    hintedWordsIndexes.Add(wordIndex);

                    inputTranslation = await this.GetReceivedTranslation();
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

            this.OnExamCompleted(this, new ExamComplitionEventArgs(
                correctlyTranslated,
                total: wordIndex + 1,
                supplementaryExamRound,
                supplementaryExamRound.HasValue ? this.SupplementaryExamRounds : null));

            return new ExamProcessResult(hintedWordsIndexes, wrongTranslatedWordsIndexes);
        }

        protected async Task ProcessSupplementaryExam(IEnumerable<DictionaryWord> words, TranslationType translationType)
        {
            this.WordsForSupplementaryExam.AddRange(words);

            if (this.WordsForSupplementaryExam.Count == 0)
            {
                return;
            }

            if (this.SupplementaryExamRounds == 1)
            {
                this.OnSupplementaryExamStarted(
                    this,
                    new SupplementaryExamEventArgs(1, 1, this.WordsForSupplementaryExam.Count));

                this.WordsForSupplementaryExam.Clear();

                await this.ConductExam(words, translationType);

                return;
            }

            HashSet<int> hintedAndWrongWordIndexes = [];

            for (int round = 0; round < this.SupplementaryExamRounds; round++)
            {
                if (this.WordsForSupplementaryExam.Count == 0)
                {
                    return;
                }

                this.OnSupplementaryExamStarted(
                    this,
                    new SupplementaryExamEventArgs(round + 1, this.SupplementaryExamRounds, this.WordsForSupplementaryExam.Count));

                ExamProcessResult result = await this.Process(
                    [.. this.WordsForSupplementaryExam],
                    translationType,
                    supplementaryExamRound: round + 1
                );

                hintedAndWrongWordIndexes.UnionWith(result.HintedWords.Union(result.WrongWords));
            }

            if (hintedAndWrongWordIndexes.Count == 0)
            {
                return;
            }

            this.WordsForSupplementaryExam.Clear();

            IEnumerable<DictionaryWord> wordsPortion = this.GetWordsPortion(words, hintedAndWrongWordIndexes);

            await this.ProcessSupplementaryExam(wordsPortion, translationType);
        }

        protected bool IsHintCommand(string command)
        {
            return command == Constants.HintCommand
                || command == Constants.HintCommandCyrilic;
        }

        protected IEnumerable<DictionaryWord> GetWordsPortion(
            IEnumerable<DictionaryWord> words,
            HashSet<int> wordsIndexes)
        {
            return words
                .Where((word, index) => wordsIndexes.Contains(index));
        }

        protected async Task SaveWordsToFile(IEnumerable<DictionaryWord> words, TranslationType translationType)
        {
            IEnumerable<string> examWords = words.Select(w => w.Word);

            WrongWordsReport report = new()
            {
                TranslationType = translationType,
                Words = examWords
            };

            string reportJson = JsonSerializer.Serialize(report);

            string fileName = @$"../../../Assets/Output/wrong_{Guid.NewGuid()}_{DateTime.Now:dd-MM-yyyy}.json";

            await File.WriteAllTextAsync(fileName, reportJson);
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

        private Task<string> GetReceivedTranslation()
        {
            TaskCompletionSource<string> tcs = new();

            EventHandler<TranslationEventArgs> callBack = null;

            callBack = (sender, eventArgs) =>
            {
                this.eventSender.OnTranslationSendEvent -= callBack;
                tcs.SetResult(eventArgs.Text);
            };

            this.eventSender.OnTranslationSendEvent += callBack;

            return tcs.Task;
        }
    }
}
