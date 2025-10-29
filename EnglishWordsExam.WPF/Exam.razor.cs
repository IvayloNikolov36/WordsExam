using EnglishWordsExam;
using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.EventHandlers.EventArguments;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies;
using EnglishWordsExam.Strategies.Contracts;

namespace WpfBlazor;

public partial class Exam : IEventTranslationSender
{
    public event OnTranslationSendEventHandler OnTranslationSendEvent;

    private int totalWordsCount;
    private int wordsToTranslate;
    private IEnumerable<DictionaryWord> words = [];
    private int questionNumber;
    private string? answer = null;
    private string? hints = null;
    private string? examMessage;
    private bool isStarted = false;
    private bool? isRight = null;

    protected string? Question { get; private set; }

    protected string? Answer
    {
        get => this.answer;
        set
        {
            this.OnTranslationSendEvent(this, new TranslationEventArgs(value));
            this.answer = string.Empty;
        }
    }

    protected string? AllTranslations { get; private set; }

    protected override void OnInitialized()
    {
        FileReader reader = new(Constants.FilePath);
        LoadWordsResult wordsResult = reader.LoadWords();
        this.words = wordsResult.Words;
        this.totalWordsCount = wordsResult.WordsCount;
    }

    private async Task SelectEnglishToBulgarian()
    {
        if (this.wordsToTranslate > 0)
            await this.StartExam(TranslationType.EnglishToBulgarian);
    }

    private async Task SelectBulgarianToEnglish()
    {
        if (this.wordsToTranslate > 0)
            await this.StartExam(TranslationType.BulgarianToEnglish);
    }

    private void OnHintRequired()
    {
        this.OnTranslationSendEvent(this, new TranslationEventArgs(Constants.HintCommand));
    }

    private async Task StartExam(TranslationType translationType)
    {
        IExamStrategy examStrategy = new SpaciousSupplementaryExamStrategy(this);

        examStrategy.OnWordForTranslationSending += ExamStrategy_OnWordForTranslation;
        examStrategy.OnTranslationResultSending += ExamStrategy_OnTranslationResultSending;
        examStrategy.OnTranslationHintsSending += ExamStrategy_OnTranslationHintsSending;
        examStrategy.OnExamMessageSend += ExamStrategy_OnExamMessageSend;
        examStrategy.OnExamCompleted += ExamStrategy_OnExamCompleted;

        ExamProcessor exam = new(
            this.words,
            this.wordsToTranslate,
            translationType,
            examStrategy
        );

        await InvokeAsync(() =>
        {
            this.isStarted = true;
            this.StateHasChanged();
        });

        await Task.Run(() =>
        {
            exam.Start();
        });
    }

    private void ExamStrategy_OnExamMessageSend(object sender, MessageEventArgs eventArgs)
    {
        this.examMessage = eventArgs.Message;
    }

    private async void ExamStrategy_OnTranslationHintsSending(object sender, TranslationEventArgs eventArgs)
    {
        await InvokeAsync(() =>
        {
            this.hints = eventArgs.Text;
            StateHasChanged();
        });
    }

    private void ExamStrategy_OnExamCompleted(object sender, ExamComplitionEventArgs eventArgs)
    {
        int correct = eventArgs.CorrectWordsCount;
        int total = eventArgs.TotalWordsCount;

        if (correct == total)
        {
            this.examMessage = "You have completed the exam. Congratulations!";
            return;
        }

        this.examMessage = $"Correct: {eventArgs.CorrectWordsCount} of {eventArgs.TotalWordsCount}.";
    }

    private async void ExamStrategy_OnTranslationResultSending(
        object sender,
        TranslationResultEventArgs resultArgs)
    {
        await InvokeAsync(() =>
        {
            this.isRight = resultArgs.IsCorrect;
            this.hints = null;
            this.AllTranslations = string.Join(Environment.NewLine, resultArgs.AllTranslations);
            StateHasChanged();
        });
    }

    private async void ExamStrategy_OnWordForTranslation(
        object sender,
        TranslationEventArgs eventArgs)
    {
        string wordToTranslate = eventArgs.Text;
        this.questionNumber = eventArgs.TranslationIndex!.Value + 1;

        if (this.questionNumber > 1)
        {
            int waitFor = this.isRight == true ? 1000 : 3000;
            await Task.Delay(waitFor);
        }

        await InvokeAsync(() =>
        {
            this.Question = wordToTranslate;
            this.AllTranslations = null;
            this.isRight = null;
            StateHasChanged();
        });
    }
}
