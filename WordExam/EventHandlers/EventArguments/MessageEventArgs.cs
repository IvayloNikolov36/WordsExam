using System;

namespace EnglishWordsExam.EventHandlers.EventArguments;

public class MessageEventArgs(string message) : EventArgs
{
    public string Message { get; private set; } = message;
}
