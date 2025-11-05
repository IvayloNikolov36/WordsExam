using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnglishWordsExam.Strategies
{
    public class HintedAndWrongWordsSupplementaryExamStrategy : ExamBaseStrategy
    {
        public HintedAndWrongWordsSupplementaryExamStrategy() : base()
        {

        }

        public HintedAndWrongWordsSupplementaryExamStrategy(IEventTranslationSender eventSender)
            : base(eventSender)
        {

        }

        private const int DefaultSupplementaryExamRounds = 1;

        protected override int SupplementaryExamRounds => DefaultSupplementaryExamRounds;

        public override async Task ConductExam(
            IEnumerable<DictionaryWord> examWords, 
            TranslationType translationType)
        {
            ExamProcessResult result = await this.Process(examWords, translationType);

            HashSet<int> wordIndexes = [.. result.HintedWords.Union(result.WrongWords)];

            IEnumerable<DictionaryWord> wordsPortion = this.GetWordsPortion(examWords, wordIndexes);

            await this.SaveWordsToFile(wordsPortion, translationType);

            await this.ProcessSupplementaryExam(wordsPortion, translationType);
        }
    }
}
