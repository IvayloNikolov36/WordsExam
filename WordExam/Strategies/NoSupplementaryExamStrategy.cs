using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public override async Task ConductExam(
            IEnumerable<DictionaryWord> examWords, 
            TranslationType translationType)
        {
            ExamProcessResult result = await this.Process(examWords, translationType);

            IEnumerable<DictionaryWord> words = this.GetWordsPortion(examWords, result.WrongWords);

            await this.SaveWordsToFile(words, translationType);
        }
    }
}
