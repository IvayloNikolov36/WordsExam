using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnglishWordsExam.Strategies
{
    public class WrongWordsSupplementaryExamStrategy : ExamBaseStrategy
    {
        public WrongWordsSupplementaryExamStrategy() : base()
        {

        }

        public WrongWordsSupplementaryExamStrategy(IEventTranslationSender eventSender)
            : base(eventSender)
        {

        }

        private const int DefaultSupplementaryExamRounds = 1;

        protected override int SupplementaryExamRounds => DefaultSupplementaryExamRounds;

        public override async Task ConductExam(IEnumerable<DictionaryWord> words, TranslationType translationType)
        {
            ExamProcessResult result = await this.Process(words, translationType);

            IEnumerable<DictionaryWord> examWords = this.GetWordsPortion(words, result.WrongWords);

            await this.SaveWordsToFile(examWords, translationType);

            await this.ProcessSupplementaryExam(examWords, translationType);
        }
    }
}
