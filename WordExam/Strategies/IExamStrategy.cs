using EnglishWordsExam.Enums;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies
{
    public interface IExamStrategy
    {
        void ProcessExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType);
    }
}
