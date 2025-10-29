using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies
{
    public class NoSupplementaryExamStrategy : ExamBaseStrategy
    {
        public NoSupplementaryExamStrategy() : base()
        {

        }

        public NoSupplementaryExamStrategy(IEventTranslationSender eventSender)
            : base(eventSender)
        {

        }

        public override void ConductExam(
            IEnumerable<DictionaryWord> examWords, 
            TranslationType translationType)
        {
            this.Process(examWords, translationType);
        }
    }
}
