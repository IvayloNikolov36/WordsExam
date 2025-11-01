using System;

namespace EnglishWordsExam.EventHandlers.EventArguments
{
    public class SupplementaryExamEventArgs(int round, int roundsCount, int wordsCount) : EventArgs
    {
        public int Round { get; private set; } = round;

        public int RoundsCount { get; private set; } = roundsCount;

        public int WordsCount { get; private set; } = wordsCount;
    }
}
