using System;

namespace EnglishWordsExam.EventHandlers.EventArguments;

public class ExamComplitionEventArgs(int correct, int total) : EventArgs
{
    public int CorrectWordsCount { get; set; } = correct;

    public int TotalWordsCount { get; set; } = total;
}
