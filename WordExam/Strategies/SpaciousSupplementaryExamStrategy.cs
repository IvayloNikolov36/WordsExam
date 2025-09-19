using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using System.Collections.Generic;
using System.Linq;

namespace EnglishWordsExam.Strategies
{
    public class SpaciousSupplementaryExamStrategy : ExamBaseStrategy
    {
        private const int DefaultSupplementaryExamRounds = 2;

        protected override int SupplementaryExamRounds => DefaultSupplementaryExamRounds;

        public override void ConductExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType)
        {
            (HashSet<int> hinted, HashSet<int> wrongTranslated) = this.Process(examWords, translationType);

            HashSet<int> wordIndexes = hinted.Union(wrongTranslated).ToHashSet();

            this.ProcessSupplementaryExam(examWords, wordIndexes, translationType);
        }
    }
}
