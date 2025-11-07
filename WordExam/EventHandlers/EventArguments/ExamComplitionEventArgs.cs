using System;

namespace EnglishWordsExam.EventHandlers.EventArguments;

public class ExamComplitionEventArgs(
    int correct, 
    int total, 
    int? supplementaryExamRound = null,
    int? supplementaryExamRounds = null) : EventArgs
{
    public int CorrectWordsCount { get; set; } = correct;

    public int TotalWordsCount { get; set; } = total;

    public int? SupplementaryExamRound { get; set; } = supplementaryExamRound;

    public int? SupplementaryExamRounds { get; set; } = supplementaryExamRounds;
}
