using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EnglishWordsExam.Constants;

namespace EnglishWordsExam
{
    public class ExamProcessor
    {
        private readonly IEnumerable<DictionaryWord> words;
        private readonly TranslationType translationType;
        private readonly IExamStrategy examStrategy;

        public ExamProcessor(
            IEnumerable<DictionaryWord> selectedWords,
            TranslationType translationType,
            IExamStrategy examStrategy)
        {
            this.words = selectedWords;
            this.translationType = translationType;
            this.examStrategy = examStrategy;
        }

        public async Task Start()
        {
            await this.examStrategy.ConductExam(words, this.translationType);
        }    
    }
}
