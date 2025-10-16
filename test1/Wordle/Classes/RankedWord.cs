using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser.Classes
{
    public class RankedWord
    {
        public long Ranking { get; set; }
        public string Word { get; set; }

        public RankedWord(long ranking, string word) 
        {
            Ranking = ranking;
            Word = word;
        }
    }
}
