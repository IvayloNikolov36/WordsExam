using System;
using System.Collections.Generic;

namespace EnglishWordsExam.EventHandlers.EventArguments;

public class TranslationResultEventArgs : EventArgs
{
    public TranslationResultEventArgs(bool isCorrect, IEnumerable<string> allTranslations)
    {
        this.IsCorrect = isCorrect;
        this.AllTranslations = [.. allTranslations];
    }

    public bool IsCorrect { get; private set; }

    public string[] AllTranslations { get; private set; }
}
