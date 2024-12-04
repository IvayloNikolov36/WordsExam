using System.Collections.Generic;

namespace EnglishWordsExam.Models
{
    public class LoadWordsResult
    {
        public IEnumerable<DictionaryWord> Words { get; set; }

        public int WordsCount { get; set; }
    }
}
