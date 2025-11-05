
using EnglishWordsExam;
using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using Microsoft.AspNetCore.Components;
using WpfBlazor.Models;

namespace WpfBlazor.Components;

public partial class ExamStart
{
    [Parameter]
    public EventCallback<ExamStartParameters> SelectedExamParameters { get; set; }

    private int totalWordsCount;
    private int wordsToTranslate;
    private IEnumerable<DictionaryWord> words = [];

    protected override void OnInitialized()
    {
        FileReader reader = new(@"../../../assets/words.txt");
        LoadWordsResult wordsResult = reader.LoadWords();
        this.words = wordsResult.Words;
        this.totalWordsCount = wordsResult.WordsCount;
    }

    private async Task SelectEnglishToBulgarian()
    {
        if (this.wordsToTranslate > 0)
        {
            await this.SelectedExamParameters
                .InvokeAsync(this.GetExamParams(TranslationType.EnglishToBulgarian));
        }
    }

    private async Task SelectBulgarianToEnglish()
    {
        if (this.wordsToTranslate > 0)
        {
            await this.SelectedExamParameters
                .InvokeAsync(this.GetExamParams(TranslationType.BulgarianToEnglish));
        }
    }

    private ExamStartParameters GetExamParams(TranslationType translationType)
    {
        return new ExamStartParameters(this.words, this.wordsToTranslate, translationType);
    }
}
