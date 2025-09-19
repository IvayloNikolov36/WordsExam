using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies
{
    public interface IExamStrategy
    {
        void ConductExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType);
    }
}
