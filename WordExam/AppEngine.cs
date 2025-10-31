using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.EventHandlers.EventArguments;
using EnglishWordsExam.Models;
using EnglishWordsExam.Parsers;
using EnglishWordsExam.Strategies;
using EnglishWordsExam.Strategies.Contracts;
using EnglishWordsExam.Utilities;
using System;
using System.Linq;

namespace EnglishWordsExam
{
    public class AppEngine : IEventTranslationSender
    {
        private readonly IReader reader;

        public event OnTranslationSendEventHandler OnTranslationSendEvent;

        private string inputTranslation = null;

        public AppEngine(IReader reader)
        {
            this.reader = reader;
        }

        public TranslationType SelectedTranslationType { get; private set; }

        public void Run()
        {
            LoadWordsResult wordsResult = this.reader.LoadWords();

            PrintTotalWords(wordsResult.WordsCount);

            bool newTurn = true;
            while (newTurn)
            {
                int wordsCountForTranslation = ConsoleInputParser
                    .GetWordsCountForTranslation(wordsResult.WordsCount);

                this.SelectedTranslationType = ConsoleInputParser.GetTranslationType();

                IExamStrategy examStrategy = new SpaciousSupplementaryExamStrategy(this);

                examStrategy.OnWordForTranslationSending += ExamStrategy_OnWordForTranslationSending;
                examStrategy.OnTranslationResultSending += ExamStrategy_OnTranslationResultSending;
                examStrategy.OnExamMessageSend += ExamStrategy_OnExamMessageSend;
                examStrategy.OnTranslationHintsSending += ExamStrategy_OnTranslationHintsSending;
                examStrategy.OnExamCompleted += ExamStrategy_OnExamCompleted;

                ExamProcessor exam = new(
                    wordsResult.Words,
                    wordsCountForTranslation,
                    this.SelectedTranslationType,
                    examStrategy
                );

                exam.Start();

                ConsoleWrite
                    .AnnouncementLine("If you want to proceed with another exam, input new. (Every other input will terminate the program");

                string choice = Console.ReadLine().Trim().ToLower();
                if (choice.Equals(Constants.ExitCommand, StringComparison.CurrentCultureIgnoreCase))
                {
                    Environment.Exit(0);
                }
            }
        }

        private void ExamStrategy_OnExamCompleted(object sender, ExamComplitionEventArgs eventArgs)
        {
            string message = $"Translated correctly {eventArgs.CorrectWordsCount}/{eventArgs.TotalWordsCount}.";
            ConsoleWrite.InfoLine(message);
        }

        private void ExamStrategy_OnTranslationHintsSending(object sender, TranslationHintsEventArgs eventArgs)
        {
            string[] hintedTranslation = eventArgs.TranslationTokens.ToArray();
            ConsoleWrite.InfoLine(string.Join(Environment.NewLine, hintedTranslation));
            string input = Console.ReadLine().Trim();
            this.OnTranslationSendEvent(this, new TranslationEventArgs(input));
        }

        private void ExamStrategy_OnExamMessageSend(object sender, MessageEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Message);
        }

        private void ExamStrategy_OnTranslationResultSending(object sender, TranslationResultEventArgs resultArgs)
        {
            if (resultArgs.IsCorrect)
            {
                ConsoleWrite.SuccessLine("That's right!");

                if (this.SelectedTranslationType == Enums.TranslationType.EnglishToBulgarian)
                {
                    this.ShowTranslationInfo(resultArgs.AllTranslations);
                }
            }
            else
            {
                ConsoleWrite.ErrorLine("Wrong!");
                this.ShowTranslationInfo(resultArgs.AllTranslations);
            }      
        }

        private void ExamStrategy_OnWordForTranslationSending(object sender, TranslationEventArgs eventArgs)
        {
            string wordToTranslate = eventArgs.Text;

            Console.WriteLine();
            string wordIndex = eventArgs.TranslationIndex.HasValue ? eventArgs.TranslationIndex.ToString() : "Translate";
            Console.WriteLine($"{wordIndex}: {wordToTranslate}:");
            this.inputTranslation = Console.ReadLine();
            this.OnTranslationSendEvent(this, new TranslationEventArgs(this.inputTranslation));
            this.inputTranslation = null;
        }

        private static void PrintTotalWords(int count)
        {
            Console.WriteLine($"Total words in dictionary {count}.");
        }

        private void ShowTranslationInfo(string[] translationsData)
        {
            string message = "All translations:";
            message += $"{Environment.NewLine}---";
            message += $"{string.Join($"{Environment.NewLine}---", translationsData)}";
            ConsoleWrite.InfoLine(message);
        }
    }
}
