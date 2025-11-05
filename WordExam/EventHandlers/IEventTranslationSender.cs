using EnglishWordsExam.EventHandlers.EventArguments;
using System;

namespace EnglishWordsExam.EventHandlers;

public interface IEventTranslationSender
{
    event EventHandler<TranslationEventArgs> OnTranslationSendEvent;
}
