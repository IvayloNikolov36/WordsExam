using EnglishWordsExam.Models;

namespace EnglishWordsExam
{
    public interface IReader
    {
        LoadWordsResult LoadWords();

        string[] GetWordLines();
    }
}
