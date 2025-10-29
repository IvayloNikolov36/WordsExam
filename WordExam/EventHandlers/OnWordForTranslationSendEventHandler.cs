using EnglishWordsExam.EventHandlers.EventArguments;

namespace EnglishWordsExam.EventHandlers;

public delegate void OnWordForTranslationSendEventHandler(object examBaseStrategy, TranslationEventArgs eventArgs);
