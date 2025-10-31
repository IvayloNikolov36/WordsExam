using System;
using System.Collections.Generic;

namespace EnglishWordsExam.EventHandlers.EventArguments;

public class TranslationHintsEventArgs(IEnumerable<string> tokens) : EventArgs
{
    public IEnumerable<string> TranslationTokens { get; private set; } = tokens;
}
