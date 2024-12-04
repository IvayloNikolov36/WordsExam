using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies
{
    public class WrongWordsSupplementaryExamStrategy : ExamBaseStrategy
    {
        public override void ProcessExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType)
        {
            (_, HashSet<int> wrongTranslatedWords) = this.Process(examWords, translationType);

            this.ProcessSupplementaryExam(examWords, wrongTranslatedWords, translationType);
        }
    }
}
