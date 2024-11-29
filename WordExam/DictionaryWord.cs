using System;

namespace EnglishWordsExam
{
    public class DictionaryWord : IEquatable<DictionaryWord>
    {
        public DictionaryWord(string word, string[] translations)
        {
            this.Word = word;
            this.Translations = translations;
        }

        public string Word { get; }

        public string[] Translations { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Word, this.Translations[0]);
        }

        public bool Equals(DictionaryWord other)
        {
            return other != null
                && other.Word == this.Word;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DictionaryWord);
        }
    }
}
