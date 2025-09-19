﻿using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies
{
    public class WrongWordsSupplementaryExamStrategy : ExamBaseStrategy
    {
        private const int DefaultSupplementaryExamRounds = 1;

        protected override int SupplementaryExamRounds => DefaultSupplementaryExamRounds;

        public override void ConductExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType)
        {
            (_, HashSet<int> wrongTranslatedWords) = this.Process(examWords, translationType);

            this.ProcessSupplementaryExam(examWords, wrongTranslatedWords, translationType);
        }
    }
}
