using EnglishWordsExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnglishWordsExam;

public class WordsSelector
{
    private readonly DictionaryWord[] words;
    private readonly int wordsToTranslate;

    public WordsSelector(
        IEnumerable<DictionaryWord> words,
        int wordsToTranslate)
    {
        this.words = words.ToArray();
        this.wordsToTranslate = wordsToTranslate;
    }

    public IEnumerable<DictionaryWord> GetWords(bool unique = true)
    {
        return unique ? GetRandomUniqueWords(this.words, this.wordsToTranslate)
            : GetRandomWords(this.words, this.wordsToTranslate);
    }

    private static IEnumerable<DictionaryWord> GetRandomWords(DictionaryWord[] words, int wordsToTranslate)
    {
        return Random.Shared
            .GetItems(words, wordsToTranslate);
    }

    private static IEnumerable<DictionaryWord> GetRandomUniqueWords(DictionaryWord[] words, int wordsToTranslate)
    {
        Random randomGenerator = new();
        DictionaryWord[] examWords = new DictionaryWord[wordsToTranslate];
        HashSet<int> usedRandomCrearedIndexes = new();

        for (int i = 0; i < wordsToTranslate; i++)
        {
            int randomIndex = randomGenerator.Next(0, words.Length);
            while (usedRandomCrearedIndexes.Contains(randomIndex))
            {
                randomIndex = randomGenerator.Next(0, words.Length);
            }

            usedRandomCrearedIndexes.Add(randomIndex);
            examWords[i] = words[randomIndex];
        }

        return examWords;
    }
}
