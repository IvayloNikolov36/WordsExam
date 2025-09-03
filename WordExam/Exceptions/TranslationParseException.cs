using System;

namespace EnglishWordsExam.Exceptions;

public class TranslationParseException : Exception
{
    public TranslationParseException()
    {
    }

    public TranslationParseException(string message) : base(message)
    {
    }

    public TranslationParseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
