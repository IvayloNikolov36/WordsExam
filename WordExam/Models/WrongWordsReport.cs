using EnglishWordsExam.Enums;
using System.Collections.Generic;

namespace EnglishWordsExam.Models;

internal class WrongWordsReport
{
    public IEnumerable<string> Words { get; set; }

    public TranslationType TranslationType { get; set; }
}
