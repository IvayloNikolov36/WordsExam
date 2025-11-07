using EnglishWordsExam.Enums;
using Microsoft.AspNetCore.Components;

namespace WpfBlazor.Components;

public partial class TranslationResult
{
    [Parameter]
    public string? Word { get; set; }

    [Parameter]
    public string[]? Translations { get; set; }

    [Parameter]
    public string? Answer {  get; set; }

    [Parameter]
    public TranslationType TranslationType { get; set; }
}
