using System;

namespace EnglishWordsExam.EventHandlers.EventArguments;

public class TranslationEventArgs(string text, int? translationIndex = null) : EventArgs
{
    public string Text { get; private set; } = text;

    public int? TranslationIndex { get; set; } = translationIndex;
}
