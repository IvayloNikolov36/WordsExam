using EnglishWordsExam;
using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.EventHandlers.EventArguments;
using EnglishWordsExam.Strategies;
using EnglishWordsExam.Strategies.Contracts;
using WpfBlazor.Models;

namespace WpfBlazor;

public partial class Exam : IEventTranslationSender
{
    private const string CompletedExamMessage = "You have completed the exam. Congratulations!";

    public event EventHandler<TranslationEventArgs>? OnTranslationSendEvent;

    private int wordsToTranslate;
    private int questionNumber;
    private string? answer = null;
    private string? answerMemo = null;
    private string? hints = null;
    private int hintsTextAreaRows = 1;
    private int questionTextAreaRows = 1;
    private string examResult = string.Empty;
    private string examTitle = string.Empty;
    private bool isStarted = false;
    private bool? isRight = null;
    private TranslationType translationType;
    private bool showAllTranslations = false;

    protected string? Question { get; private set; }

    protected string? Answer
    {
        get => this.answer;
        set
        {
            this.answer = value;
            if (answer != null)
            {
                this.answerMemo = value;
                this.examResult = string.Empty;
                this.OnTranslationSendEvent?.Invoke(this, new TranslationEventArgs(value));
            }
        }
    }

    protected string[]? AllTranslations { get; private set; } = null;

    protected string? WordForTranslation { get; private set; } = null;

    private async Task StartExamWithSelectedParameters(ExamStartParameters parameters)
    {
        await this.StartExam(parameters);
    }

    private void OnHintRequired()
    {
        this.OnTranslationSendEvent?.Invoke(this, new TranslationEventArgs(Constants.HintCommand));
    }

    private async Task StartExam(ExamStartParameters parameters)
    {
        this.translationType = parameters.TranslationType;
        this.wordsToTranslate = parameters.WordsToTranslate;

        IExamStrategy examStrategy = new SpaciousSupplementaryExamStrategy(this);
        this.SubscribeToExamEvents(examStrategy);

        ExamProcessor exam = new(
            parameters.Words,
            parameters.WordsToTranslate,
            parameters.TranslationType,
            examStrategy
        );

        this.isStarted = true;
        await InvokeAsync(this.StateHasChanged);

        await Task.Run(() => exam.Start());
    }

    private async void ExamStrategy_OnSupplementaryExamStarted(
        object sender,
        SupplementaryExamEventArgs eventArgs)
    {
        string message = $"Supplementary Exam round {eventArgs.Round}/{eventArgs.RoundsCount}";
        this.examTitle = message;

        this.wordsToTranslate = eventArgs.WordsCount;

        await InvokeAsync(this.StateHasChanged);
    }

    private void ExamStrategy_OnExamMessageSend(object sender, MessageEventArgs eventArgs)
    {
        this.examTitle = eventArgs.Message;
    }

    private async void ExamStrategy_OnTranslationHintsSending(object sender, TranslationHintsEventArgs eventArgs)
    {
        this.hintsTextAreaRows = eventArgs.TranslationTokens.Count();
        this.hints = string.Join(Environment.NewLine, eventArgs.TranslationTokens);

        await InvokeAsync(this.StateHasChanged);
    }

    private async void ExamStrategy_OnExamCompleted(object sender, ExamComplitionEventArgs eventArgs)
    {
        int correct = eventArgs.CorrectWordsCount;
        int total = eventArgs.TotalWordsCount;
        bool allAreCorrect = correct == total;

        bool isCompleted = allAreCorrect && eventArgs.SupplementaryExamRound is null
            || allAreCorrect && eventArgs.SupplementaryExamRound == eventArgs.SupplementaryExamRounds;

        this.examResult = isCompleted
            ? CompletedExamMessage
            : $"Correct translations: {eventArgs.CorrectWordsCount}/{eventArgs.TotalWordsCount}.";

        await InvokeAsync(this.StateHasChanged);
    }

    private async void ExamStrategy_OnTranslationResultSending(
        object sender,
        TranslationResultEventArgs resultArgs)
    {
        this.isRight = resultArgs.IsCorrect;
        this.hints = null;
        this.AllTranslations = resultArgs.AllTranslations;
        this.showAllTranslations = this.ShowAllTranslations(resultArgs.IsCorrect, this.translationType);
        this.WordForTranslation = this.Question;

        await InvokeAsync(this.StateHasChanged);
    }

    private async void ExamStrategy_OnWordForTranslation(
        object sender,
        TranslationEventArgs eventArgs)
    {
        string wordToTranslate = eventArgs.Text;
        int questionNumber = eventArgs.TranslationIndex!.Value + 1;

        if (questionNumber > 1)
        {
            await Task.Delay(1000);
        }

        this.questionNumber = questionNumber;
        this.Question = wordToTranslate;
        this.questionTextAreaRows = this.CalculateQuestionTextAreaRowsCount(this.Question.Length);
        this.Answer = null;
        this.isRight = null;

        await InvokeAsync(this.StateHasChanged);
    }

    private void SubscribeToExamEvents(IExamStrategy examStrategy)
    {
        examStrategy.OnWordForTranslationSending += ExamStrategy_OnWordForTranslation;
        examStrategy.OnTranslationResultSending += ExamStrategy_OnTranslationResultSending;
        examStrategy.OnTranslationHintsSending += ExamStrategy_OnTranslationHintsSending;
        examStrategy.OnExamMessageSend += ExamStrategy_OnExamMessageSend;
        examStrategy.OnSupplementaryExamStarted += ExamStrategy_OnSupplementaryExamStarted;
        examStrategy.OnExamCompleted += ExamStrategy_OnExamCompleted;
    }

    private bool ShowAllTranslations(bool isCorrect, TranslationType translationType)
    {
        if (translationType == TranslationType.EnglishToBulgarian)
        {
            return true;
        }

        return !isCorrect;
    }

    private int CalculateQuestionTextAreaRowsCount(int questionLength)
    {
        const int symbolsOnRow = 30;

        return (int)Math.Ceiling((decimal)questionLength / symbolsOnRow);
    }
}
