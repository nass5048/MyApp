using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGuesser
{
    public class CharacterDictionary
    {
        public char Character { get; set; }

        public long Count { get; set; }

        //can be number 0-4
        public int Location { get; set; }

        public CharacterDictionary(char character, long count, int location)
        {
            Character = character;
            Count = count;
            Location = location;
        }   
    }

    
}
