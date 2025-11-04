using EnglishWordsExam.ConsoleApp.Parsers;
using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.EventHandlers.EventArguments;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies;
using EnglishWordsExam.Strategies.Contracts;
using EnglishWordsExam.Utilities;

namespace EnglishWordsExam.ConsoleApp;

public class AppEngine : IEventTranslationSender
{
    private readonly IReader reader;

    public event OnTranslationSendEventHandler OnTranslationSendEvent = default!;

    private string? inputTranslation = null;

    public AppEngine(IReader reader)
    {
        this.reader = reader;
    }

    public TranslationType SelectedTranslationType { get; private set; }

    public async Task Run()
    {
        LoadWordsResult wordsResult = reader.LoadWords();

        PrintTotalWords(wordsResult.WordsCount);

        while (true)
        {
            int wordsCountForTranslation = ConsoleInputParser
                .GetWordsCountForTranslation(wordsResult.WordsCount);

            SelectedTranslationType = ConsoleInputParser.GetTranslationType();

            IExamStrategy examStrategy = new SpaciousSupplementaryExamStrategy(this);

            examStrategy.OnWordForTranslationSending += ExamStrategy_OnWordForTranslationSending;
            examStrategy.OnTranslationResultSending += ExamStrategy_OnTranslationResultSending;
            examStrategy.OnExamMessageSend += ExamStrategy_OnExamMessageSend;
            examStrategy.OnTranslationHintsSending += ExamStrategy_OnTranslationHintsSending;
            examStrategy.OnSupplementaryExamStarted += ExamStrategy_OnSupplementaryExamStarted;
            examStrategy.OnExamCompleted += ExamStrategy_OnExamCompleted;

            ExamProcessor exam = new(
                wordsResult.Words,
                wordsCountForTranslation,
                SelectedTranslationType,
                examStrategy
            );

            ConsoleWrite
                .InfoLine($"Input {Constants.HintCommand}/{Constants.HintCommandCyrilic} for a help.");

            await exam.Start();

            ConsoleWrite
                .AnnouncementLine("If you want to proceed with another exam, input new. (Every other input will terminate the program");

            string choice = Console.ReadLine()!.Trim().ToLower();
            if (choice.Equals(Constants.ExitCommand, StringComparison.CurrentCultureIgnoreCase))
            {
                Environment.Exit(0);
            }
        }
    }

    private void ExamStrategy_OnSupplementaryExamStarted(object sender, SupplementaryExamEventArgs eventArgs)
    {
        ConsoleWrite.AnnouncementLine(
            $"Supplementary exam round {eventArgs.Round}/{eventArgs.RoundsCount} ({eventArgs.WordsCount} word(s)).");
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
        string input = Console.ReadLine()!.Trim();
        OnTranslationSendEvent(this, new TranslationEventArgs(input));
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

            if (SelectedTranslationType == TranslationType.EnglishToBulgarian)
            {
                ShowTranslationInfo(resultArgs.AllTranslations);
            }
        }
        else
        {
            ConsoleWrite.ErrorLine("Wrong!");
            ShowTranslationInfo(resultArgs.AllTranslations);
        }
    }

    private void ExamStrategy_OnWordForTranslationSending(object sender, TranslationEventArgs eventArgs)
    {
        string wordToTranslate = eventArgs.Text;

        Console.WriteLine();
        string wordIndex = eventArgs.TranslationIndex.HasValue
            ? eventArgs.TranslationIndex.Value.ToString()
            : "Translate";
        Console.WriteLine($"{wordIndex}: {wordToTranslate}:");
        inputTranslation = Console.ReadLine();
        OnTranslationSendEvent(this, new TranslationEventArgs(inputTranslation));
        inputTranslation = null;
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
