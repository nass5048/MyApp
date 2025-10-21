using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WordleGuesser.Classes;

namespace WordleGuesser
{
    public class Guesser
    {
        string Word { get; set; }

        List<string> ValidWords { get; set; }

        List<string> GuessedWord { get; set; }

        List<RankedWord> rankedWords { get; set; }

        // Load and normalize the embedded word list once
        private static readonly List<string> AllWords = Utilitys.GetEmbeddedTextResource("WordleWords.txt")
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim().ToLowerInvariant())
            .Where(w => w.Length == 5)
            .ToList();

        List<CharacterDictionary> CharacterDictionary { get; set; }

        public List<WordGuessResponse> FailedGuesses { get; set; } = new List<WordGuessResponse>();

        public Guesser(string correctWord)
        {
            getWords();
            bool solved = false;

            while (!solved)
            {
                string guess = GuessWord(correctWord);
                if (guess == correctWord)
                    solved = true;
            }
        }

        public string GuessWord(string correctWord)
        {
            getWords();
            RemoveFailureWords();
            FillCharacterDictionary();
            RankWords();

            var bestGuess = rankedWords.OrderByDescending(x => x.Ranking).FirstOrDefault();
            if (bestGuess == null)
                return "";

            FailedGuesses.Add(new WordGuessResponse(correctWord, bestGuess.Word));
            return bestGuess.Word;
        }

        public void getWords()
        {
            // Copy full normalized list to working list
            ValidWords = new List<string>(AllWords);
        }

        public void RemoveFailureWords()
        {
            var requiredLetters = new HashSet<char>();
            var forbiddenLetters = new HashSet<char>();
            var positionConstraints = new char?[5];
            var excludedPositions = new Dictionary<int, HashSet<char>>();

            foreach (var fail in FailedGuesses)
            {
                for (int i = 0; i < 5; i++)
                {
                    char c = char.ToLowerInvariant(fail.Word[i]);
                    switch (fail.Colors[i])
                    {
                        case ResponseColor.green:
                            positionConstraints[i] = c;
                            requiredLetters.Add(c);
                            break;
                        case ResponseColor.yellow:
                            requiredLetters.Add(c);
                            if (!excludedPositions.ContainsKey(i))
                                excludedPositions[i] = new HashSet<char>();
                            excludedPositions[i].Add(c);
                            break;
                        case ResponseColor.grey:
                            forbiddenLetters.Add(c);
                            break;
                    }
                }
            }

            ValidWords = ValidWords.Where(word =>
            {
                for (int i = 0; i < 5; i++)
                {
                    if (positionConstraints[i].HasValue && word[i] != positionConstraints[i].Value)
                        return false;

                    if (excludedPositions.TryGetValue(i, out var excluded) && excluded.Contains(word[i]))
                        return false;
                }

                foreach (var f in forbiddenLetters)
                    if (word.Contains(f) && !requiredLetters.Contains(f))
                        return false;

                foreach (var r in requiredLetters)
                    if (!word.Contains(r))
                        return false;

                return true;
            }).ToList();
        }

        public void FillCharacterDictionary()
        {
            var counts = new Dictionary<(char letter, int position), int>();

            foreach (var word in ValidWords)
            {
                for (int i = 0; i < 5; i++)
                {
                    var key = (word[i], i);
                    if (!counts.TryAdd(key, 1))
                        counts[key]++;
                }
            }

            CharacterDictionary = counts
                .Select(kv => new CharacterDictionary(kv.Key.letter, kv.Value, kv.Key.position))
                .ToList();
        }

        public LetterLocation GetBestLetterLocation(int location)
        {
            var validLetters = CharacterDictionary.Where(t => t.Location == location);
            var letter = validLetters.OrderByDescending(t => t.Count).First();
            return new LetterLocation(letter.Character, location);
        }

        public void RankWords()
        {
            var charCount = CharacterDictionary
                .GroupBy(cd => cd.Character)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(cd => cd.Count)
                );

            rankedWords = ValidWords.Select(word =>
            {
                long rank = 0;
                foreach (char c in word)
                    if (charCount.TryGetValue(c, out long count))
                        rank += count;

                if (!IsWordUniqueChar(word))
                    rank /= 2; // penalize duplicates

                return new RankedWord(rank, word);
            }).ToList();
        }

        public bool IsWordUniqueChar(string word)
        {
            return word.Distinct().Count() == word.Length;
        }
    }
}
