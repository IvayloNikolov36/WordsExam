using EnglishWordsExam.EventHandlers.EventArguments;

namespace EnglishWordsExam.EventHandlers;

public delegate void OnTranslationSendEventHandler(object sender, TranslationEventArgs translation);
