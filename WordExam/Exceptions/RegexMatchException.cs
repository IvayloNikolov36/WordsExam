using System;

namespace EnglishWordsExam.Exceptions;

public class RegexMatchException : Exception
{
    public RegexMatchException()
    {
    }

    public RegexMatchException(string message) : base(message)
    {
    }
}
