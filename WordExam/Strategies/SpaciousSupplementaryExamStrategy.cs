using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnglishWordsExam.Strategies
{
    public class SpaciousSupplementaryExamStrategy : ExamBaseStrategy
    {
        public SpaciousSupplementaryExamStrategy(): base()
        {
            
        }

        public SpaciousSupplementaryExamStrategy(IEventTranslationSender eventSender)
            : base(eventSender) 
        {
            
        }

        private const int DefaultSupplementaryExamRounds = 2;

        protected override int SupplementaryExamRounds => DefaultSupplementaryExamRounds;

        public override async Task ConductExam(IEnumerable<DictionaryWord> words, TranslationType translationType)
        {
            ExamProcessResult result = this.Process(words, translationType);

            HashSet<int> wordIndexes = [.. result.HintedWords.Union(result.WrongWords)];

            IEnumerable<DictionaryWord> examWords = this.GetWordsPortion(words, wordIndexes);

            await this.SaveWordsToFile(examWords, translationType);

            await this.ProcessSupplementaryExam(examWords, translationType);
        }
    }
}
