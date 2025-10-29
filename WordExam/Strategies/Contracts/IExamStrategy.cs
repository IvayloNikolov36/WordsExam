using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;

namespace EnglishWordsExam.Strategies.Contracts;

public interface IExamStrategy
{
    event OnTranslationSendEventHandler OnWordForTranslationSending;
    event OnTranslationResultSendEventHandler OnTranslationResultSending;
    event OnTranslationHintsSendEventHandler OnTranslationHintsSending;
    event OnExamMessageSendEventHandler OnExamMessageSend;
    event OnExamCompletedEventHandler OnExamCompleted;

    void ConductExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType);
}
