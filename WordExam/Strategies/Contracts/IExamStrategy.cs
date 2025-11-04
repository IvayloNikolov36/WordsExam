using EnglishWordsExam.Enums;
using EnglishWordsExam.EventHandlers;
using EnglishWordsExam.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnglishWordsExam.Strategies.Contracts;

public interface IExamStrategy
{
    event OnWordForTranslationSendEventHandler OnWordForTranslationSending;
    event OnTranslationResultSendEventHandler OnTranslationResultSending;
    event OnTranslationHintsSendEventHandler OnTranslationHintsSending;
    event OnExamMessageSendEventHandler OnExamMessageSend;
    event OnExamCompletedEventHandler OnExamCompleted;
    event OnSupplementaryExamStartedEventHandler OnSupplementaryExamStarted;

    Task ConductExam(IEnumerable<DictionaryWord> examWords, TranslationType translationType);
}
