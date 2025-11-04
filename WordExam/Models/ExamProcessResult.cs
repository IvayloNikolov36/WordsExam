using System.Collections.Generic;

namespace EnglishWordsExam.Models;

public class ExamProcessResult
{
    public ExamProcessResult(HashSet<int> hinted, HashSet<int> wrong)
    {
        this.HintedWords = hinted;
        this.WrongWords = wrong;
    }

    public HashSet<int> HintedWords { get; set; }

    public HashSet<int> WrongWords { get; set; }
}
