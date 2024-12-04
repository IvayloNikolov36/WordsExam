using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnglishWordsExam
{
    public class ExamProcessor
    {
        private readonly IList<DictionaryWord> words;
        private readonly int wordsToTranslate;
        private readonly TranslationType translationType;
        private readonly IExamStrategy examStrategy;

        public ExamProcessor(
            IEnumerable<DictionaryWord> words,
            int wordsToTranslate,
            TranslationType translationType,
            IExamStrategy examStrategy)
        {
            this.words = words.ToList();
            this.wordsToTranslate = wordsToTranslate;
            this.translationType = translationType;
            this.examStrategy = examStrategy;
        }

        public void Start()
        {
            Console.WriteLine($"Input {Constants.HintCommand}/{Constants.HintCommandCyrilic} for a help.");

            IEnumerable<DictionaryWord> examWords = this.GetWordsRandomly();

            examStrategy.ProcessExam(examWords, this.translationType);
        }

        private IEnumerable<DictionaryWord> GetWordsRandomly()
        {
            Random randomGenerator = new();
            DictionaryWord[] examWords = new DictionaryWord[this.wordsToTranslate];
            HashSet<int> usedRandomCrearedIndexes = new();

            for (int i = 0; i < wordsToTranslate; i++)
            {
                int randomIndex = randomGenerator.Next(0, this.words.Count);
                while (usedRandomCrearedIndexes.Contains(randomIndex))
                {
                    randomIndex = randomGenerator.Next(0, this.words.Count);
                }

                usedRandomCrearedIndexes.Add(randomIndex);
                examWords[i] = this.words[randomIndex];
            }

            return examWords;
        }
    }
}
