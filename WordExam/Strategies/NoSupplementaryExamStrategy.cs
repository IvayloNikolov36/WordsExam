using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies
{
    public class NoSupplementaryExamStrategy : ExamBaseStrategy
    {
        public override void ProcessExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType)
        {
            this.Process(examWords, translationType);
        }
    }
}
