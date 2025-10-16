using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WordleGuesser.Classes;

namespace WordleGuesser
{
    public class Guesser
    {
        string Word { get; set; }

        List<string> ValidWords {get; set;}

        List<string> GuessedWord { get; set; }

        List<RankedWord> rankedWords {  get; set; }

        List<CharacterDictionary> CharacterDictionary { get; set; }

        public List<WordGuessResponse> FailedGuesses { get; set; } = new List<WordGuessResponse>();
        
        //saaes is the best letter for everything
        // will not work because we will want to remove letters and find an actual word

        //first guess should have all unique letters
        //for ranking word for every letter location sumn the count and that is the words ranking
        //we should remove words with duplicated letter for first and second guess
        //also need to remove letter that are not in the word 

        public Guesser(string correctWord)
        {
            getWords();
            var solved = false;
            //FailedGuesses.Add(new WordGuessResponse(correctWord, "coals"));
            //FailedGuesses.Add(new WordGuessResponse(correctWord, "niter"));
            while (!solved) {
               if(GuessWord(correctWord) == correctWord)
                {
                    solved = true;
                }
            }
        }

        public string GuessWord(string correctWord)
        {
            getWords();
            RemoveFailureWords();
            FillCharacterDictionary();
            RankWords();
            var test = rankedWords.OrderByDescending(x => x.Ranking).ToList();
            FailedGuesses.Add(new WordGuessResponse(correctWord, test.First().Word));
            return test.First().Word;
        }

        public void getWords()
        {
            ValidWords = new List<string>();

            var wordList = Utilitys.GetEmbeddedTextResource("WordleWords.txt");

            ValidWords = wordList.Split('\n').ToList();

        }
        
        public void RemoveFailureWords()
        {
            List<string> words = new List<string>();
            foreach(var word in ValidWords)
            {
                var validWord = true;
                var Failed = false;
                foreach(var fail in FailedGuesses)
                {
                    
                    for(int i = 0; i < 5; i++)
                    {
                        
                        //remove grey letters
                        if (fail.Colors[i] == ResponseColor.grey) 
                        {
                            if (word.Contains(fail.Word[i]))
                                Failed = true;
                            
                        }
                        else if (fail.Colors[i] == ResponseColor.green)
                        {
                            if (word[i] != fail.Word[i])
                                Failed = true;
                        }
                        else
                        {
                            if (word[i] != fail.Word[i] && word.Contains(fail.Word[i]))
                                validWord = true;
                            else
                                Failed = true;
                        }
                        if (Failed)
                        {
                            validWord = false;
                        }
                    }
                    if (!validWord)
                        break;


                }
                if(validWord)
                    words.Add(word);
            }
            ValidWords = words;
        }

        public void FillCharacterDictionary()
        {
            List<LetterLocation> list = new List<LetterLocation>();
            foreach (var temp in ValidWords)
            {
                for (int i = 0; i < 5; i++)
                {
                    list.Add(new LetterLocation(temp[i], i));
                }
            }
            var comparer = new CharacterComparer();
            CharacterDictionary = new List<CharacterDictionary>();

            var m = list.Distinct(comparer).ToList();
            foreach(var temp in list.Distinct(comparer))
            {
                CharacterDictionary.Add(new CharacterDictionary(
                    temp.letter, 
                    list.Where(t => t.letter == temp.letter && t.location == temp.location).Count(), 
                    temp.location));
            }
        }

        public LetterLocation GetBestLetterLocation(int location)
        {
            var validLetters = CharacterDictionary.Where(t => t.Location == location);

            var letter = validLetters.OrderByDescending(t => t.Count).First();

            return new LetterLocation(letter.Character, location);
        }
        
        public string GetBestWord()
        {
            

            return "";
        }

        public void RankWords()
        {
            rankedWords = new List<RankedWord>();
            foreach(var word in ValidWords)
            {
                long rank = 0;
                for(int i = 0; i < 5; i++)
                {
                    rank += CharacterDictionary.Where(t => t.Character == word[i]).FirstOrDefault().Count;
                }
                if(!IsWordUniqueChar(word))
                    rank /= 2;
                rankedWords.Add(new RankedWord(rank, word));
            }
        }

        public bool IsWordUniqueChar(string word)
        {

            foreach(var letter in word)
            {
                if (word.Where(t => t == letter).Count() > 1)
                    return false;
            }    
            
            return true;
        }

    }
}
