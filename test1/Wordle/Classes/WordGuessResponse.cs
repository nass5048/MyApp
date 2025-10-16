using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser.Classes
{
    public class WordGuessResponse
    {
        public string Word { get; set; }

        public List<ResponseColor> Colors { get; set; }

        public WordGuessResponse()
        {

        }

        public WordGuessResponse(string CorrectWord, string GuessedWord)
        {
            Word = GuessedWord;
            Colors = new List<ResponseColor>();

            for (int i = 0; i < 5; i++)
            {
                if (CorrectWord[i] == GuessedWord[i])
                {
                    Colors.Add(ResponseColor.green);
                }
                else if (ShouldYellow(i, GuessedWord, CorrectWord))
                {
                    Colors.Add(ResponseColor.yellow);
                }
                else
                {
                    Colors.Add(ResponseColor.grey);
                }
            }

        }

        private bool ShouldYellow(int index, string guess, string correct)
        {
            int correctLetterCheck = correct.Where(p => p == guess[index]).Count();
            //gets if the letters can be yellow or grey
            //check numbe rof green letters
            int greenLetterCount = 0;
            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == correct[i] && guess[index] == guess[i])
                    greenLetterCount++;
            }
            //if there are the exact amount of green letters as number of letters than you have all the correct letters no letters should be yellow
            if (greenLetterCount == correctLetterCheck)
            {
                return false;
            }
            //check number of letters in guess up to index
            int letterCheck = 0;
            for (int i = 0; i <= index; i++)
            {
                if (guess[i] == guess[index])
                    letterCheck++;
            }
            //if there are more green letters than number of letters than you know the rezt have to be grey
            if (letterCheck > correctLetterCheck)
            {
                return false;
            }

            if (correct.Contains(guess[index]))
            {
                return true;
            }
            return false;
        }

    }

    public enum ResponseColor
    {
        grey,
        yellow,
        green
    }


}
