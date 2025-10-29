using EnglishWordsExam.EventHandlers.EventArguments;

namespace EnglishWordsExam.EventHandlers;

public delegate void OnTranslationResultSendEventHandler(object sender, TranslationResultEventArgs resultArgs);
