using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;

namespace WpfBlazor.Models;

public class ExamStartParameters(
    IEnumerable<DictionaryWord> words,
    int wordsToTranslate,
    TranslationType translationType)
{
    public IEnumerable<DictionaryWord> Words { get; } = words;

    public int WordsToTranslate { get; } = wordsToTranslate;

    public TranslationType TranslationType { get; } = translationType;
}
