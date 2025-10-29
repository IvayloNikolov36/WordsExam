﻿using EnglishWordsExam.Enums;
using EnglishWordsExam.Models;
using EnglishWordsExam.Strategies.Contracts;
using System;
using System.Collections.Generic;
using static EnglishWordsExam.Constants;

namespace EnglishWordsExam
{
    public class ExamProcessor
    {
        private readonly DictionaryWord[] words;
        private readonly int wordsToTranslate;
        private readonly TranslationType translationType;
        private readonly IExamStrategy examStrategy;

        public ExamProcessor(
            IEnumerable<DictionaryWord> words,
            int wordsToTranslate,
            TranslationType translationType,
            IExamStrategy examStrategy)
        {
            this.words = [..words];
            this.wordsToTranslate = wordsToTranslate;
            this.translationType = translationType;
            this.examStrategy = examStrategy;
        }

        public void Start()
        {
            Console.WriteLine($"Input {HintCommand}/{HintCommandCyrilic} for a help.");

            IEnumerable<DictionaryWord> examWords = Random
                .Shared
                .GetItems(this.words, this.wordsToTranslate);

            this.examStrategy.ConductExam(examWords, this.translationType);
        }
    }
}
